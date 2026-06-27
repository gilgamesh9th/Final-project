using UnityEngine;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private int priority = 5;
    [SerializeField] private bool disableWhenDone = false;
    [SerializeField] private float lookCooldown = 2f;

    [TextArea]
    [SerializeField] private string[] lookNarrations = { " " };

    private int _lookIndex;
    private float _lastTriggerTime = -99f;
    private bool _exhausted;

    public void OnLooked()
    {
        if (_exhausted) return;

        if (TryGetComponent<ConditionalNarrator>(out var conditional))
        {
            string line = conditional.Evaluate();
            if (string.IsNullOrEmpty(line))
            {
                if (disableWhenDone) _exhausted = true;
                return;
            }

            if (!NarratorManager.Instance.SayImmediate(line, priority, this))
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

        if (!NarratorManager.Instance.SayImmediate(lookNarrations[_lookIndex], priority, this))
            return;

        _lookIndex++;
        _lastTriggerTime = Time.time;

        if (_lookIndex >= lookNarrations.Length && disableWhenDone)
            _exhausted = true;
    }
}