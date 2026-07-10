using UnityEngine;

public class PuzzleUnlockTrigger : MonoBehaviour
{
    [SerializeField] private SequenceLight sequenceLight1;
    [SerializeField] private SequenceLight sequenceLight2;
    [SerializeField] private ColorPuzzleManager puzzleManager;
    [SerializeField] private DoorInteractable door;
    [SerializeField] private GameObject trigger1;
    [SerializeField] private GameObject trigger2;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (door != null)
            door.CloseAndLock();

        if (puzzleManager != null)
            puzzleManager.enabled = true;

        if (sequenceLight1 != null)
            sequenceLight1.Activate();

        if (sequenceLight2 != null)
            sequenceLight2.Activate();

        gameObject.SetActive(false);
        trigger1.SetActive(false);
        trigger2.SetActive(false);
    }
}