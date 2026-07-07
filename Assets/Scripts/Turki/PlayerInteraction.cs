using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
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
        }
    }
}