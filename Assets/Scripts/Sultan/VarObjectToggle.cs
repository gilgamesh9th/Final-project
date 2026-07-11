using UnityEngine;

public class VarObjectToggle : MonoBehaviour
{
    [SerializeField] private Condition[] conditions;
    [SerializeField] private GameObject[] targets;
    [SerializeField] private bool reEnableWhenFalse = false;

    private void Start()
    {
        GameVarStore.Instance.OnVarChanged += OnVarChanged;
        Evaluate();
    }

    private void OnDestroy()
    {
        if (GameVarStore.Instance != null)
            GameVarStore.Instance.OnVarChanged -= OnVarChanged;
    }

    private void OnVarChanged(string key, int value)
    {
        Evaluate();
    }

    private void Evaluate()
    {
        bool met = AllConditionsMet();

        if (met)
        {
            foreach (var go in targets)
                if (go != null) go.SetActive(false);
        }
        else if (reEnableWhenFalse)
        {
            foreach (var go in targets)
                if (go != null) go.SetActive(true);
        }
    }

    private bool AllConditionsMet()
    {
        if (conditions == null || conditions.Length == 0) return false;

        foreach (var c in conditions)
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