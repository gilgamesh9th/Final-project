using System.Collections;
using UnityEngine;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private int priority = 5;
    [SerializeField] private bool disableWhenDone = false;
    [SerializeField] private float defaultLineDuration = 5f;
    [SerializeField] private float lookDelay = 0f;

    [SerializeField] private VoicedLine[] lookNarrations;
    [SerializeField] private VoicedLine[] exitTransitions;
    [SerializeField] private bool loopExitTransitions = true;

    private int _lookIndex;
    private bool _exhausted;
    private Coroutine _narrationRoutine;
    private Coroutine _exitRoutine;
    private bool _isLooking;
    private bool _completingLine;
    private bool _beganNarrating;
    private int _exitIndex;

    public void OnLookEnter()
    {
        _isLooking = true;
        NarratorManager.Instance.Hold(this);

        if (_exitRoutine != null)
        {
            StopCoroutine(_exitRoutine);
            _exitRoutine = null;
        }

        if (_exhausted || _narrationRoutine != null) return;
        _beganNarrating = false;
        _narrationRoutine = StartCoroutine(NarrationLoop());
    }

    public void OnLookExit()
    {
        _isLooking = false;
        if (_completingLine) return;

        if (_narrationRoutine != null)
        {
            StopCoroutine(_narrationRoutine);
            _narrationRoutine = null;
        }

        if (_beganNarrating
            && exitTransitions != null && exitTransitions.Length > 0
            && (loopExitTransitions || _exitIndex < exitTransitions.Length))
        {
            _exitRoutine = StartCoroutine(PlayExitTransition());
        }
        else
        {
            NarratorManager.Instance.ReleaseHold(this);
        }
    }

    private IEnumerator NarrationLoop()
    {
        if (lookDelay > 0f)
        {
            float elapsed = 0f;
            while (elapsed < lookDelay)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        bool hasConditional = TryGetComponent<ConditionalNarrator>(out var conditional);

        while (!_exhausted)
        {
            VoicedLine line;

            if (hasConditional)
            {
                line = conditional.Evaluate();
                if (line == null)
                {
                    if (disableWhenDone) _exhausted = true;
                    break;
                }
            }
            else
            {
                if (_lookIndex >= lookNarrations.Length)
                {
                    if (disableWhenDone) _exhausted = true;
                    break;
                }
                line = lookNarrations[_lookIndex];
            }

            if (line.mustComplete) _completingLine = true;

            if (!NarratorManager.Instance.SayImmediate(
                line.text, priority, this, line.clip))
            {
                continue;
            }

            _beganNarrating = true;

            if (!hasConditional)
                _lookIndex++;

            float wait = line.clip != null ? line.clip.length : defaultLineDuration;
            yield return new WaitForSeconds(wait);

            if (_completingLine)
            {
                _completingLine = false;
                if (!_isLooking)
                {
                    _narrationRoutine = null;
                    if (exitTransitions != null && exitTransitions.Length > 0
                        && (loopExitTransitions || _exitIndex < exitTransitions.Length))
                    {
                        _exitRoutine = StartCoroutine(PlayExitTransition());
                    }
                    else
                    {
                        NarratorManager.Instance.ReleaseHold(this);
                    }
                    break;
                }
            }
        }

        _narrationRoutine = null;
    }

    private IEnumerator PlayExitTransition()
    {
        _beganNarrating = false;

        VoicedLine vl = exitTransitions[_exitIndex];

        if (loopExitTransitions)
            _exitIndex = (_exitIndex + 1) % exitTransitions.Length;
        else
            _exitIndex++;

        if (!NarratorManager.Instance.SayImmediate(
            vl.text, priority, this, vl.clip))
        {
            NarratorManager.Instance.ReleaseHold(this);
            _exitRoutine = null;
            yield break;
        }

        float wait = vl.clip != null ? vl.clip.length : defaultLineDuration;
        yield return new WaitForSeconds(wait);

        NarratorManager.Instance.ClearText(this);
        NarratorManager.Instance.ReleaseHold(this);
        _exitRoutine = null;
    }
}