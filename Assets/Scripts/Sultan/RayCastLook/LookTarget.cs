using System.Collections;
using UnityEngine;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private int priority = 5;
    [SerializeField] private bool disableWhenDone = false;
    [SerializeField] private float defaultLineDuration = 5f;

    [SerializeField] private VoicedLine[] lookNarrations;

    private int _lookIndex;
    private bool _exhausted;
    private Coroutine _narrationRoutine;
    private bool _isLooking;
    private bool _completingLine;

    public void OnLookEnter()
    {
        _isLooking = true;
        NarratorManager.Instance.Hold(this);
        if (_exhausted || _narrationRoutine != null) return;
        _narrationRoutine = StartCoroutine(NarrationLoop());
    }

    public void OnLookExit()
    {
        _isLooking = false;
        if (_completingLine) return;

        NarratorManager.Instance.ReleaseHold(this);
        if (_narrationRoutine != null)
        {
            StopCoroutine(_narrationRoutine);
            _narrationRoutine = null;
        }
    }

    private IEnumerator NarrationLoop()
    {
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

            if (!hasConditional)
                _lookIndex++;

            float wait = line.clip != null ? line.clip.length : defaultLineDuration;
            yield return new WaitForSeconds(wait);

            if (_completingLine)
            {
                _completingLine = false;
                if (!_isLooking)
                {
                    NarratorManager.Instance.ReleaseHold(this);
                    break;
                }
            }
        }

        _narrationRoutine = null;
    }
}