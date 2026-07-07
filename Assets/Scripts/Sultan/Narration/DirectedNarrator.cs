using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NarrationLine
{
    [TextArea] public string text;
    public AudioClip clip;
    public bool isImportant;
    public string destinationIfUnsaid;
    [System.NonSerialized] public bool moved;
    public bool discardOnInterrupt;
    [System.NonSerialized] public bool started;
}

[System.Serializable]
public class NarrationChannel
{
    public string channelName;
    public int priority = 2;
    public Condition[] conditions;
    public NarrationLine[] narrations;
    public VoicedLine[] returnTransitions;
    public int returnLinesCount = 0;
    public bool loopReturnTransitions = false;
    public bool loopLastLine = false;
    public bool restartOnActivate = false;
    public string[] disableChannelsOnComplete;
    public bool disableAllTransitionsOnComplete;
    public string[] disableTransitionsOnComplete;
    public bool disableIdleOnComplete;

    [System.NonSerialized] public List<NarrationLine> queue;
    [System.NonSerialized] public int index;
    [System.NonSerialized] public int returnIndex;
}

[System.Serializable]
public class ChannelTransition
{
    public string label;
    public string fromChannel;
    public string toChannel;
    public VoicedLine[] narrations;
    public int linesPerSwitch = 1;
    public bool loop = false;

    [System.NonSerialized] public int index;
}

public class DirectedNarrator : MonoBehaviour
{
    [SerializeField] private NarrationChannel[] channels;
    [SerializeField] private ChannelTransition[] transitions;
    [SerializeField] private VoicedLine[] idleNarrations;
    [SerializeField] private float lineDuration = 4f;
    [SerializeField] private float lineGap = 0.5f;
    [SerializeField] private int idlePriority = 1;
    [SerializeField] private float idleThreshold = 10f;
    [SerializeField] private float moveToResumeTime = 2.5f;
    [SerializeField] private float movementGraceWindow = 0.3f;
    [SerializeField] private bool loopIdle = true;
    [SerializeField] private Transform playerTransform;

    private NarrationChannel _activeChannel;
    private Coroutine _sequenceCoroutine;
    private Coroutine _idleCoroutine;
    private Dictionary<string, NarrationChannel> _channelLookup;
    private bool _paused;
    private bool _isIdle;
    private bool _inTransition;
    private float _idleTimer;
    private float _moveTimer;
    private Vector3 _lastPosition;
    private float _timeSinceLastMoved;
    private int _idleIndex;
    private readonly HashSet<string> _disabledChannels = new HashSet<string>();
    private readonly HashSet<string> _disabledTransitions = new HashSet<string>();
    private bool _allTransitionsDisabled;
    private bool _idleDisabled;

    private void Start()
    {
        _channelLookup = new Dictionary<string, NarrationChannel>();

        foreach (var ch in channels)
        {
            ch.queue = new List<NarrationLine>(ch.narrations);
            ch.index = 0;
            ch.returnIndex = 0;
            if (!string.IsNullOrEmpty(ch.channelName))
                _channelLookup[ch.channelName] = ch;
        }

        if (playerTransform != null)
            _lastPosition = playerTransform.position;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        TrackMovement();

        if (!_isIdle)
            CheckChannels();
    }

    private void TrackMovement()
    {
        float moved = Vector3.Distance(playerTransform.position, _lastPosition);
        _lastPosition = playerTransform.position;
        bool frameMoved = moved > 0.001f;
        bool isEngaged = RayCastDetector.IsEngaged;

        if (frameMoved)
            _timeSinceLastMoved = 0f;
        else
            _timeSinceLastMoved += Time.deltaTime;

        bool isMoving = _timeSinceLastMoved <= movementGraceWindow;

        if (isMoving || isEngaged)
        {
            _idleTimer = 0f;

            if (_isIdle)
            {
                if (isEngaged)
                {
                    ResumeFromIdle();
                }
                else
                {
                    _moveTimer += Time.deltaTime;
                    if (_moveTimer >= moveToResumeTime)
                        ResumeFromIdle();
                }
            }
        }
        else
        {
            _moveTimer = 0f;

            if (!_isIdle && !_inTransition && !_idleDisabled
                && _activeChannel != null && _sequenceCoroutine != null)
            {
                _idleTimer += Time.deltaTime;
                if (_idleTimer >= idleThreshold)
                    GoIdle();
            }
        }
    }

    private void GoIdle()
    {
        _isIdle = true;
        _paused = true;
        _moveTimer = 0f;

        NarratorManager.Instance.ClearText(this);
        NarratorManager.Instance.Release(this);
        _idleCoroutine = StartCoroutine(PlayIdleLoop());
    }

    private void ResumeFromIdle()
    {
        _isIdle = false;
        _paused = false;
        _idleTimer = 0f;
        _moveTimer = 0f;

        if (_idleCoroutine != null)
        {
            StopCoroutine(_idleCoroutine);
            _idleCoroutine = null;
        }

        NarratorManager.Instance.ClearText(this);
        NarratorManager.Instance.Release(this);
    }

    private void CheckChannels()
    {
        NarrationChannel matched = null;
        bool matchWasDisabled = false;
        foreach (var ch in channels)
        {
            if (AllConditionsMet(ch.conditions))
            {
                if (!string.IsNullOrEmpty(ch.channelName)
                    && _disabledChannels.Contains(ch.channelName))
                {
                    matchWasDisabled = true;
                    continue;
                }
                matched = ch;
                break;
            }
        }

        if (matchWasDisabled && matched == null && _activeChannel != null)
            return;

        if (matched == _activeChannel) return;

        NarrationChannel previous = _activeChannel;

        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
            _sequenceCoroutine = null;
            NarratorManager.Instance.ClearText(this);
            NarratorManager.Instance.Release(this);
        }

        if (previous != null)
            ProcessUnsaidLines(previous);

        _activeChannel = matched;

        if (_activeChannel != null && _activeChannel.restartOnActivate)
        {
            _activeChannel.queue = new List<NarrationLine>(_activeChannel.narrations);
            _activeChannel.index = 0;
            _activeChannel.returnIndex = 0;
            foreach (var nl in _activeChannel.queue)
            {
                nl.started = false;
                nl.moved = false;
            }
        }

        if (_activeChannel != null && _activeChannel.index < _activeChannel.queue.Count)
        {
            ChannelTransition transition = FindTransition(previous, _activeChannel);
            if (transition != null && IsTransitionDisabled(transition))
                transition = null;
            if (transition != null && transition.loop
                && transition.index >= transition.narrations.Length)
                transition.index = 0;
            if (transition != null
                && transition.index < transition.narrations.Length
                && transition.linesPerSwitch > 0)
            {
                _sequenceCoroutine = StartCoroutine(
                    PlayTransitionThenSequence(transition, _activeChannel));
            }
            else
            {
                _sequenceCoroutine = StartCoroutine(PlaySequence(_activeChannel));
            }
        }
    }

    private void ProcessUnsaidLines(NarrationChannel channel)
    {
        var insertionOffsets = new Dictionary<NarrationChannel, int>();

        for (int i = channel.index; i < channel.queue.Count; i++)
        {
            NarrationLine line = channel.queue[i];
            if (!line.isImportant || string.IsNullOrEmpty(line.destinationIfUnsaid))
                continue;
            if (line.moved) continue;

            if (!_channelLookup.TryGetValue(line.destinationIfUnsaid, out var dest))
                continue;

            if (!insertionOffsets.ContainsKey(dest))
                insertionOffsets[dest] = 0;

            line.moved = true;
            dest.queue.Insert(dest.index + insertionOffsets[dest], line);
            insertionOffsets[dest]++;
        }

        for (int i = channel.queue.Count - 1; i >= channel.index; i--)
        {
            if (channel.queue[i].moved)
                channel.queue.RemoveAt(i);
        }
    }

    private IEnumerator PlayTransitionThenSequence(
        ChannelTransition transition, NarrationChannel channel)
    {
        _inTransition = true;

        int linesToPlay = Mathf.Min(
            transition.linesPerSwitch,
            transition.narrations.Length - transition.index);

        for (int i = 0; i < linesToPlay; i++)
        {
            if (_activeChannel != channel) { _inTransition = false; yield break; }
            if (transition.index >= transition.narrations.Length) break;

            VoicedLine vl = transition.narrations[transition.index];
            transition.index++;

            while (!NarratorManager.Instance.ShowText(
                vl.text, channel.priority, this, vl.clip))
            {
                yield return null;
                if (_activeChannel != channel) { _inTransition = false; yield break; }
            }

            float duration = GetDuration(vl.clip);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (NarratorManager.Instance.CurrentOwner != this)
                {
                    while (NarratorManager.Instance.CurrentOwner != null
                           && NarratorManager.Instance.CurrentOwner != this)
                    {
                        yield return null;
                        if (_activeChannel != channel)
                        { _inTransition = false; yield break; }
                    }
                    while (!NarratorManager.Instance.ShowText(
                        vl.text, channel.priority, this, vl.clip))
                    {
                        yield return null;
                        if (_activeChannel != channel)
                        { _inTransition = false; yield break; }
                    }
                    elapsed = 0f;
                    continue;
                }
                if (_activeChannel != channel) { _inTransition = false; yield break; }
                elapsed += Time.deltaTime;
                yield return null;
            }

            NarratorManager.Instance.ClearText(this);

            if (i < linesToPlay - 1 || channel.index < channel.queue.Count)
            {
                elapsed = 0f;
                while (elapsed < lineGap)
                {
                    if (_activeChannel != channel) { _inTransition = false; yield break; }
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        _inTransition = false;

        if (_activeChannel == channel && channel.index < channel.queue.Count)
            _sequenceCoroutine = StartCoroutine(PlaySequence(channel));
        else
            _sequenceCoroutine = null;
    }

    private IEnumerator PlayReturnTransitions(NarrationChannel channel)
    {
        if (channel.returnTransitions == null || channel.returnTransitions.Length == 0)
            yield break;
        if (channel.returnLinesCount <= 0) yield break;

        if (channel.loopReturnTransitions
            && channel.returnIndex >= channel.returnTransitions.Length)
            channel.returnIndex = 0;

        int linesToPlay = Mathf.Min(
            channel.returnLinesCount,
            channel.returnTransitions.Length - channel.returnIndex);
        if (linesToPlay <= 0) yield break;

        _inTransition = true;

        for (int r = 0; r < linesToPlay; r++)
        {
            if (_activeChannel != channel) { _inTransition = false; yield break; }
            if (channel.returnIndex >= channel.returnTransitions.Length) break;

            VoicedLine vl = channel.returnTransitions[channel.returnIndex];
            channel.returnIndex++;

            if (!NarratorManager.Instance.ShowText(
                vl.text, channel.priority, this, vl.clip))
            {
                _inTransition = false;
                yield break;
            }

            float duration = GetDuration(vl.clip);
            float re = 0f;
            while (re < duration)
            {
                if (NarratorManager.Instance.CurrentOwner != this)
                {
                    _inTransition = false;
                    yield break;
                }
                if (_activeChannel != channel) { _inTransition = false; yield break; }
                re += Time.deltaTime;
                yield return null;
            }

            NarratorManager.Instance.ClearText(this);

            if (r < linesToPlay - 1)
            {
                re = 0f;
                while (re < lineGap)
                {
                    if (_activeChannel != channel) { _inTransition = false; yield break; }
                    re += Time.deltaTime;
                    yield return null;
                }
            }
        }

        _inTransition = false;

        float gapElapsed = 0f;
        while (gapElapsed < lineGap)
        {
            if (_activeChannel != channel) yield break;
            gapElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator DisplayLine(NarrationLine nl, NarrationChannel channel)
    {
        nl.started = true;
        while (!NarratorManager.Instance.ShowText(
            nl.text, channel.priority, this, nl.clip))
        {
            yield return null;
            if (_activeChannel != channel) yield break;
            while (_paused) yield return null;
        }

        float duration = GetDuration(nl.clip);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (_paused)
            {
                NarratorManager.Instance.ClearText(this);
                NarratorManager.Instance.Release(this);

                while (_paused) yield return null;
                if (_activeChannel != channel) yield break;
                if (nl.discardOnInterrupt) yield break;

                while (!NarratorManager.Instance.ShowText(
                    nl.text, channel.priority, this, nl.clip))
                {
                    yield return null;
                    if (_activeChannel != channel) yield break;
                    while (_paused) yield return null;
                }
                elapsed = 0f;
                continue;
            }

            if (NarratorManager.Instance.CurrentOwner != this)
            {
                while (NarratorManager.Instance.CurrentOwner != null
                       && NarratorManager.Instance.CurrentOwner != this)
                {
                    yield return null;
                    if (_activeChannel != channel) yield break;
                    while (_paused) yield return null;
                }

                if (nl.discardOnInterrupt) yield break; 
                yield return PlayReturnTransitions(channel);
                if (_activeChannel != channel) yield break;

                while (!NarratorManager.Instance.ShowText(
                    nl.text, channel.priority, this, nl.clip))
                {
                    yield return null;
                    if (_activeChannel != channel) yield break;
                }
                elapsed = 0f;
                continue;
            }

            if (_activeChannel != channel) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator WaitGap(NarrationChannel channel)
    {
        float elapsed = 0f;
        while (elapsed < lineGap)
        {
            while (_paused) yield return null;
            if (_activeChannel != channel) yield break;

            if (NarratorManager.Instance.CurrentOwner == null
                || NarratorManager.Instance.CurrentOwner == this)
                elapsed += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator PlaySequence(NarrationChannel channel)
    {
        while (channel.index < channel.queue.Count)
        {
            while (_paused) yield return null;
            if (_activeChannel != channel) yield break;

            NarrationLine nl = channel.queue[channel.index];

            if (nl.discardOnInterrupt && nl.started)
            {
                channel.index++;
                continue;
            }

            yield return DisplayLine(nl, channel);
            if (_activeChannel != channel) yield break;

            channel.index++;
            NarratorManager.Instance.ClearText(this);

            if (channel.index < channel.queue.Count)
            {
                yield return WaitGap(channel);
                if (_activeChannel != channel) yield break;
            }
        }

        DisableLinkedChannels(channel);

        if (channel.loopLastLine && channel.queue.Count > 0)
        {
            NarrationLine lastLine = channel.queue[channel.queue.Count - 1];
            while (_activeChannel == channel)
            {
                while (_paused) yield return null;
                if (_activeChannel != channel) yield break;

                yield return DisplayLine(lastLine, channel);
                if (_activeChannel != channel) yield break;

                NarratorManager.Instance.ClearText(this);

                yield return WaitGap(channel);
                if (_activeChannel != channel) yield break;
            }
            yield break;
        }

        NarratorManager.Instance.Release(this);
        _sequenceCoroutine = null;
    }

    private void DisableLinkedChannels(NarrationChannel channel)
    {
        if (channel.disableChannelsOnComplete != null)
        {
            foreach (var name in channel.disableChannelsOnComplete)
            {
                if (!string.IsNullOrEmpty(name))
                    _disabledChannels.Add(name);
            }
        }

        if (channel.disableAllTransitionsOnComplete)
            _allTransitionsDisabled = true;

        if (channel.disableTransitionsOnComplete != null)
        {
            foreach (var label in channel.disableTransitionsOnComplete)
            {
                if (!string.IsNullOrEmpty(label))
                    _disabledTransitions.Add(label);
            }
        }

        if (channel.disableIdleOnComplete)
            _idleDisabled = true;
    }

    private bool IsTransitionDisabled(ChannelTransition transition)
    {
        if (_allTransitionsDisabled) return true;
        return !string.IsNullOrEmpty(transition.label)
            && _disabledTransitions.Contains(transition.label);
    }


    private IEnumerator PlayIdleLoop()
    {
        while (_isIdle)
        {
            VoicedLine vl = idleNarrations[_idleIndex];

            while (_isIdle
                   && !NarratorManager.Instance.ShowText(
                       vl.text, idlePriority, this, vl.clip))
                yield return null;

            if (!_isIdle) break;

            float duration = GetDuration(vl.clip);
            float elapsed = 0f;
            while (elapsed < duration && _isIdle)
            {
                if (NarratorManager.Instance.CurrentOwner != this)
                {
                    while (_isIdle
                           && NarratorManager.Instance.CurrentOwner != null
                           && NarratorManager.Instance.CurrentOwner != this)
                        yield return null;
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!_isIdle) break;

            if (elapsed >= duration)
            {
                if (loopIdle)
                    _idleIndex = (_idleIndex + 1) % idleNarrations.Length;
                else
                {
                    _idleIndex++;
                    if (_idleIndex >= idleNarrations.Length) break;
                }
                NarratorManager.Instance.ClearText(this);

                if (idleNarrations.Length > 1)
                {
                    elapsed = 0f;
                    while (elapsed < lineGap && _isIdle)
                    {
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                }
            }
        }

        NarratorManager.Instance.ClearText(this);
        NarratorManager.Instance.Release(this);
    }

    private ChannelTransition FindTransition(
        NarrationChannel from, NarrationChannel to)
    {
        if (from == null || to == null || transitions == null) return null;

        foreach (var t in transitions)
        {
            if (t.fromChannel == from.channelName
                && t.toChannel == to.channelName)
                return t;
        }
        return null;
    }

    private bool AllConditionsMet(Condition[] conditions)
    {
        if (conditions == null || conditions.Length == 0) return false;

        foreach (var c in conditions)
        {
            int actual = GameVarStore.Instance.Get(c.variableKey);
            bool pass = c.comparison switch
            {
                Comparison.Equals             => actual == c.value,
                Comparison.GreaterThan        => actual > c.value,
                Comparison.LessThan           => actual < c.value,
                Comparison.GreaterThanOrEqual => actual >= c.value,
                Comparison.LessThanOrEqual    => actual <= c.value,
                Comparison.Even               => actual % 2 == 0,
                Comparison.Odd                => actual % 2 != 0,
                _ => false
            };
            if (!pass) return false;
        }
        return true;
    }

    private float GetDuration(AudioClip clip)
    {
        return clip != null ? clip.length : lineDuration;
    }
    
}