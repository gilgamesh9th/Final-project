using UnityEngine;

public enum Comparison { Equals, GreaterThan, LessThan }

[System.Serializable]
public class NarrationRule
{
    public string variableKey;
    public Comparison comparison;
    public int value;
    [TextArea] public string narration;
}

public class ConditionalNarrator : MonoBehaviour
{
    [SerializeField] private NarrationRule[] rules;
    [TextArea]
    [SerializeField] private string fallbackNarration;

    private string _lastSpoken;

    public string Evaluate()
    {
        string result = fallbackNarration;

        foreach (var rule in rules)
        {
            int actual = GameVarStore.Instance.Get(rule.variableKey);
            bool match = rule.comparison switch
            {
                Comparison.Equals      => actual == rule.value,
                Comparison.GreaterThan => actual > rule.value,
                Comparison.LessThan    => actual < rule.value,
                _ => false
            };
            if (match) { result = rule.narration; break; }
        }

        if (result == _lastSpoken)
            return null;

        _lastSpoken = result;

        return result;
    }
}