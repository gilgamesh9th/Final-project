using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameVar
{
    public string key;
    public int value;
}

public class GameVarStore : MonoBehaviour
{
    public static GameVarStore Instance { get; private set; }

    public event Action<string, int> OnVarChanged;

    [SerializeField] private GameVar[] initialVars;

    private readonly Dictionary<string, int> _vars = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var v in initialVars)
            _vars[v.key] = v.value;
    }

    public int Get(string key)
    {
        return _vars.TryGetValue(key, out int val) ? val : 0;
    }

    public void Set(string key, int value)
    {
        _vars[key] = value;
        OnVarChanged?.Invoke(key, value);
    }

    public void Add(string key, int amount = 1)
    {
        Set(key, Get(key) + amount);
    }

    public void LogVarsWithValue(int value)
    {
        foreach (var kvp in _vars)
        {
            if (kvp.Value == value)
                Debug.Log($"[GameVarStore] {kvp.Key} = {kvp.Value}");
        }
    }
}