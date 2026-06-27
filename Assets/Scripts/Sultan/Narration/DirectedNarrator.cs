using System.Collections;
using UnityEngine;

[System.Serializable]
public class NarrationChannel
{
    public string channelName;
    public int priority = 2;
    public Condition[] conditions;
    [TextArea] public string[] narrations;
    [System.NonSerialized] public int index;
}

public class DirectedNarrator : MonoBehaviour
{
    [SerializeField] private NarrationChannel[] channels;
    [TextArea]
    [SerializeField] private string[] idleNarrations;
    [SerializeField] private float lineDuration = 4f;
    [SerializeField] private float lineGap = 0.5f;
    [SerializeField] private int idlePriority = 1;
    [SerializeField] private float idleThreshold = 10f;
    [SerializeField] private float moveToResumeTime = 2.5f;
    [SerializeField] private Transform playerTransform;

    private NarrationChannel _activeChannel;
    private Coroutine _sequenceCoroutine;
    private Coroutine _idleCoroutine;

    private bool _paused;
    private bool _isIdle;
    private float _idleTimer;
    private float _moveTimer;
    private Vector3 _lastPosition;
    private int _idleIndex;

    private void Start()
    {
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
        bool isMoving = moved > 0.001f;
        bool isEngaged = RayCastDetector.IsEngaged;

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

            if (!_isIdle && _activeChannel != null && _sequenceCoroutine != null)
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
        foreach (var ch in channels)
        {
            if (AllConditionsMet(ch.conditions))
            {
                matched = ch;
                break;
            }
        }

        if (matched == _activeChannel) return;

        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
            _sequenceCoroutine = null;
            NarratorManager.Instance.ClearText(this);
            NarratorManager.Instance.Release(this);
        }

        _activeChannel = matched;

        if (_activeChannel != null && _activeChannel.index < _activeChannel.narrations.Length)
            _sequenceCoroutine = StartCoroutine(PlaySequence(_activeChannel));
    }

    private IEnumerator PlaySequence(NarrationChannel channel)
    {
        while (channel.index < channel.narrations.Length)
        {
            while (_paused) yield return null;
            if (_activeChannel != channel) yield break;

            string currentLine = channel.narrations[channel.index];

            while (!NarratorManager.Instance.ShowText(currentLine, channel.priority, this))
            {
                yield return null;
                if (_activeChannel != channel) yield break;
                while (_paused) yield return null;
            }

            float elapsed = 0f;
            while (elapsed < lineDuration)
            {
                if (_paused)
                {
                    NarratorManager.Instance.ClearText(this);
                    NarratorManager.Instance.Release(this);

                    while (_paused) yield return null;
                    if (_activeChannel != channel) yield break;

                    while (!NarratorManager.Instance.ShowText(currentLine, channel.priority, this))
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

                    while (!NarratorManager.Instance.ShowText(currentLine, channel.priority, this))
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

            channel.index++;
            NarratorManager.Instance.ClearText(this);

            if (channel.index < channel.narrations.Length)
            {
                elapsed = 0f;
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
        }

        NarratorManager.Instance.Release(this);
        _sequenceCoroutine = null;
    }

    private IEnumerator PlayIdleLoop()
    {
        while (_isIdle)
        {
            while (_isIdle
                   && !NarratorManager.Instance.ShowText(idleNarrations[_idleIndex], idlePriority, this))
                yield return null;

            if (!_isIdle) break;

            float elapsed = 0f;
            while (elapsed < lineDuration && _isIdle)
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

            if (elapsed >= lineDuration)
            {
                _idleIndex = (_idleIndex + 1) % idleNarrations.Length;
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
}