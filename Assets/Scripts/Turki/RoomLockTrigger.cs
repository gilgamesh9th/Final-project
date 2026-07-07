using UnityEngine;

public class RoomLockTrigger : MonoBehaviour
{
    public GameObject doorBlocker;
    private bool hasLocked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasLocked) return;

        if (other.CompareTag("Player"))
        {
            hasLocked = true;

            if (doorBlocker != null)
                doorBlocker.SetActive(true);
        }
    }
}