using UnityEngine;

public class ZoneNarrator : MonoBehaviour
{
    [SerializeField] private int priority = 3;

    [SerializeField] private VoicedLine[] narrations;

    [SerializeField] private bool immediate = false;

    private int _index;
    private bool _playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_playerInside) return;
        if (NarratorManager.Instance == null) return;

        _playerInside = true;

        if (TryGetComponent<ConditionalNarrator>(out var conditional))
        {
            VoicedLine line = conditional.Evaluate();
            if (line == null) return;

            if (immediate)
                NarratorManager.Instance.SayImmediate(
                    line.text, priority, this, line.clip);
            else
                NarratorManager.Instance.Say(
                    line.text, priority, this, line.clip);
            return;
        }

        if (_index >= narrations.Length) return;

        var entry = narrations[_index];
        if (immediate)
            NarratorManager.Instance.SayImmediate(
                entry.text, priority, this, entry.clip);
        else
            NarratorManager.Instance.Say(
                entry.text, priority, this, entry.clip);

        _index++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
    }
}