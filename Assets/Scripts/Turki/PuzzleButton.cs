using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    [Header("Target Statue")]
    public PuzzleItem statueItem;

    public void Interact()
    {
        if (statueItem != null)
        {
            statueItem.Interact();
        }
    }
}