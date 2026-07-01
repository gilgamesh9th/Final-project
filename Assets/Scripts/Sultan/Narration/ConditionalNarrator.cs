using UnityEngine;

public enum Comparison { Equals, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, Even, Odd }

[System.Serializable]
public class Condition
{
    public string variableKey;
    public Comparison comparison;
    public int value;
}

[System.Serializable]
public class NarrationRule
{
    public Condition[] conditions;
    public VoicedLine[] narrations;
}

public class ConditionalNarrator : MonoBehaviour
{
    [SerializeField] private int priority = 4;
    [SerializeField] private bool disableWhenDone = false;

    [SerializeField] private NarrationRule[] rules;

    [SerializeField] private VoicedLine[] fallbackNarrations;

    [SerializeField] private bool immediate = false;

    private int[] _ruleIndices;
    private int _fallbackIndex;
    private bool _playerInside;
    private bool _exhausted;

    private void Awake()
    {
        _ruleIndices = new int[rules.Length];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_playerInside) return;
        if (_exhausted) return;
        if (NarratorManager.Instance == null) return;
        _playerInside = true;

        VoicedLine line = Evaluate();
        if (line == null) return;

        if (immediate)
            NarratorManager.Instance.SayImmediate(
                line.text, priority, this, line.clip);
        else
            NarratorManager.Instance.Say(
                line.text, priority, this, line.clip);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
    }

    public VoicedLine Evaluate()
    {
        if (_exhausted) return null;

        for (int i = 0; i < rules.Length; i++)
        {
            if (AllConditionsMet(rules[i]))
            {
                if (_ruleIndices[i] >= rules[i].narrations.Length)
                {
                    if (disableWhenDone) _exhausted = true;
                    return null;
                }
                return rules[i].narrations[_ruleIndices[i]++];
            }
        }

        if (_fallbackIndex >= fallbackNarrations.Length)
        {
            if (disableWhenDone) _exhausted = true;
            return null;
        }
        return fallbackNarrations[_fallbackIndex++];
    }

    private bool AllConditionsMet(NarrationRule rule)
    {
        foreach (var c in rule.conditions)
        {
            int actual = GameVarStore.Instance.Get(c.variableKey);
            bool pass = c.comparison switch
            {
                Comparison.Equals             => actual == c.value,
                Comparison.GreaterThan        => actual > c.value,
                Comparison.LessThan           => actual < c.value,
                Comparison.GreaterThanOrEqual => actual >= c.value,
                Comparison.LessThanOrEqual    => actual <= c.value,
                Comparison.Even               => actual % 2 == 0,
                Comparison.Odd                => actual % 2 != 0,
                _ => false
            };
            if (!pass) return false;
        }
        return true;
    }
}