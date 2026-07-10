using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalCutscene : MonoBehaviour, IInteractable
{
    [Header("Scene To Load")]
    [SerializeField] private string sceneName;

    [Header("Lock State")]
    [SerializeField] private bool isLocked = true;

    public void Interact()
    {
        if (isLocked)
        {
            Debug.Log("Door is locked.");
            return;
        } 

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"[DoorInteractable] No scene name set on {gameObject.name}");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void Unlock()
    {
        isLocked = false;
    }
}