using UnityEngine;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private float lookCooldown = 2f;

    [TextArea]
    [SerializeField] private string[] lookNarrations = { " " };

    private int _lookIndex;
    private float _lastTriggerTime = -99f;

    public void OnLooked()
    {
        if (_lookIndex >= lookNarrations.Length)
            return;

        if (Time.time - _lastTriggerTime < lookCooldown)
            return;

        NarratorManager.Instance.SayImmediate(lookNarrations[_lookIndex]);
        _lookIndex++;
        _lastTriggerTime = Time.time;
    }
}