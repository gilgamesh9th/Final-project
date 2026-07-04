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

    public void OnLookEnter()
    {
        NarratorManager.Instance.Hold(this);
        if (_exhausted || _narrationRoutine != null) return;
        _narrationRoutine = StartCoroutine(NarrationLoop());
    }

    public void OnLookExit()
    {
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

            if (!NarratorManager.Instance.SayImmediate(
                line.text, priority, this, line.clip))
            {
                continue;
            }

            if (!hasConditional)
                _lookIndex++;

            float wait = line.clip != null ? line.clip.length : defaultLineDuration;
            yield return new WaitForSeconds(wait);
        }

        _narrationRoutine = null;
    }
}