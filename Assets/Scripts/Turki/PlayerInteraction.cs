using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private PuzzleItem currentItem;
    private PuzzleButton currentButton;
    private RedRecordButton currentRecordButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentRecordButton != null)
            {
                currentRecordButton.Interact();
                return;
            }

            if (currentButton != null)
            {
                currentButton.Interact();
                return;
            }

            if (currentItem != null)
            {
                currentItem.Interact();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        RedRecordButton recordButton = other.GetComponent<RedRecordButton>();
        if (recordButton != null)
        {
            currentRecordButton = recordButton;
            return;
        }

        PuzzleButton button = other.GetComponent<PuzzleButton>();
        if (button != null)
        {
            currentButton = button;
            return;
        }

        PuzzleItem item = other.GetComponent<PuzzleItem>();
        if (item != null)
        {
            currentItem = item;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RedRecordButton recordButton = other.GetComponent<RedRecordButton>();
        if (recordButton != null && recordButton == currentRecordButton)
        {
            currentRecordButton = null;
            return;
        }

        PuzzleButton button = other.GetComponent<PuzzleButton>();
        if (button != null && button == currentButton)
        {
            currentButton = null;
            return;
        }

        PuzzleItem item = other.GetComponent<PuzzleItem>();
        if (item != null && item == currentItem)
        {
            currentItem = null;
        }
    }
}