using UnityEngine;

public enum Comparison { Equals, GreaterThan, LessThan }

[System.Serializable]
public class NarrationRule
{
    public string variableKey;
    public Comparison comparison;
    public int value;
    [TextArea] public string[] narrations;
}

public class ConditionalNarrator : MonoBehaviour
{
    [SerializeField] private NarrationRule[] rules;

    [TextArea]
    [SerializeField] private string[] fallbackNarrations;

    [SerializeField] private bool immediate = false;

    private int[] _ruleIndices;
    private int _fallbackIndex;
    private bool _playerInside;

    private void Awake()
    {
        _ruleIndices = new int[rules.Length];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_playerInside) return;
        if (NarratorManager.Instance == null) return;
        _playerInside = true;

        string line = Evaluate();
        if (string.IsNullOrEmpty(line)) return;

        if (immediate)
            NarratorManager.Instance.SayImmediate(line);
        else
            NarratorManager.Instance.Say(line);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
    }

    public string Evaluate()
    {
        for (int i = 0; i < rules.Length; i++)
        {
            var rule = rules[i];
            int actual = GameVarStore.Instance.Get(rule.variableKey);
            bool match = rule.comparison switch
            {
                Comparison.Equals      => actual == rule.value,
                Comparison.GreaterThan => actual > rule.value,
                Comparison.LessThan    => actual < rule.value,
                _ => false
            };

            if (match)
            {
                if (_ruleIndices[i] >= rule.narrations.Length)
                    return null;
                return rule.narrations[_ruleIndices[i]++];
            }
        }

        if (_fallbackIndex >= fallbackNarrations.Length)
            return null;
        return fallbackNarrations[_fallbackIndex++];
    }
}