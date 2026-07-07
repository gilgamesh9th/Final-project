using UnityEngine;

public enum UpdateMode { Set, Increment }

[System.Serializable]
public class VarWrite
{
    public string key;
    public UpdateMode mode;
    public int value = 1;
}

[System.Serializable]
public class VarUpdate
{
    public Condition[] conditions;
    public VarWrite[] writes;
}

public class VarUpdater : MonoBehaviour
{
    [SerializeField] private VarUpdate[] updates;
    private bool _playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_playerInside) return;
        if (GameVarStore.Instance == null) return;
        _playerInside = true;

        foreach (var u in updates)
        {
            if (!AllConditionsMet(u.conditions)) continue;

            foreach (var w in u.writes)
            {
                if (w.mode == UpdateMode.Increment)
                {
                    GameVarStore.Instance.Add(w.key, w.value);
                    Debug.Log(w.key);
                    Debug.Log("Increment");
                    Debug.Log(w.value);
                }
                else
                {
                    GameVarStore.Instance.Set(w.key, w.value);
                    Debug.Log(w.key);
                    Debug.Log("Set");
                    Debug.Log(w.value);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
    }

    private bool AllConditionsMet(Condition[] conditions)
    {
        if (conditions == null || conditions.Length == 0) return true;

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