using UnityEngine;

public class ZoneNarrator : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string[] narrations = { " " };

    [SerializeField] private bool immediate = false;

    private int _index;
    private bool _playerInside;
    

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_index >= narrations.Length)
            return;
        if (_playerInside)
            return;

        if (NarratorManager.Instance == null)
            return;
        
        _playerInside = true;

        if (TryGetComponent<ConditionalNarrator>(out var conditional))
        {
            string line = conditional.Evaluate();
            if (string.IsNullOrEmpty(line)) return;

            if (immediate)
                NarratorManager.Instance.SayImmediate(line);
            else
                NarratorManager.Instance.Say(line);
            return;
        }


        _playerInside = true;

        if (immediate)
            NarratorManager.Instance.SayImmediate(narrations[_index]);
        else
            NarratorManager.Instance.Say(narrations[_index]);

        _index++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        _playerInside = false;
    }
}