using UnityEngine;

public class WallToggle : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToEnable;
    [SerializeField] private GameObject[] objectsToDisable;

    [SerializeField] private bool onlyOnce = true;

    private bool _playerInside;
    private bool _spent;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_playerInside) return;
        if (_spent) return;
        _playerInside = true;

        foreach (var go in objectsToEnable)
            if (go != null) go.SetActive(true);

        foreach (var go in objectsToDisable)
            if (go != null) go.SetActive(false);

        if (onlyOnce) _spent = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
    }
}