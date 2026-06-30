using UnityEngine;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private int priority = 5;
    [SerializeField] private bool disableWhenDone = false;
    [SerializeField] private float lookCooldown = 2f;

    [SerializeField] private VoicedLine[] lookNarrations;

    private int _lookIndex;
    private float _lastTriggerTime = -99f;
    private bool _exhausted;

    public void OnLooked()
    {
        if (_exhausted) return;

        if (TryGetComponent<ConditionalNarrator>(out var conditional))
        {
            VoicedLine line = conditional.Evaluate();
            if (line == null)
            {
                if (disableWhenDone) _exhausted = true;
                return;
            }

            if (!NarratorManager.Instance.SayImmediate(
                line.text, priority, this, line.clip))
                return;

            _lastTriggerTime = Time.time;
            return;
        }

        if (_lookIndex >= lookNarrations.Length)
        {
            if (disableWhenDone) _exhausted = true;
            return;
        }

        if (Time.time - _lastTriggerTime < lookCooldown) return;

        var entry = lookNarrations[_lookIndex];
        if (!NarratorManager.Instance.SayImmediate(
            entry.text, priority, this, entry.clip))
            return;

        _lookIndex++;
        _lastTriggerTime = Time.time;

        if (_lookIndex >= lookNarrations.Length && disableWhenDone)
            _exhausted = true;
    }
}