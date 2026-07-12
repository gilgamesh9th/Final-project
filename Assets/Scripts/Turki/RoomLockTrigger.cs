using UnityEngine;

public class RoomLockTrigger : MonoBehaviour
{
    public DoorInteractable door;
    private bool hasLocked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasLocked) return;

        if (other.CompareTag("Player"))
        {
            hasLocked = true;

            if (door != null)
                door.CloseAndLock();
        }
    }
}