using UnityEngine;

public enum UpdateMode { Set, Increment }

[System.Serializable]
public class VarUpdate
{
    public string key;
    public UpdateMode mode;
    public int value = 1;
}

public class VarUpdater : MonoBehaviour
{
    [SerializeField] private VarUpdate[] updates;
    private bool _playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_playerInside)
            return;
        if (GameVarStore.Instance == null)
            return;
       _playerInside = true;
       
        foreach (var u in updates)
        {
            if (u.mode == UpdateMode.Increment)
                GameVarStore.Instance.Add(u.key, u.value);
            else
                GameVarStore.Instance.Set(u.key, u.value);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        _playerInside = false;
    }
}