using UnityEngine;

public class HallwayObject : MonoBehaviour, IInteractable
{
    public bool isAnomaly = false; // only true on the one anomaly object during odd loops

    // called by interaction system
    public void Interact()
    {
        PuzzleUI.Instance.Show(this);
    }

    // called by UI buttons
    public void OnAnomalyChosen(bool playerSaidAnomaly)
    {
        if (playerSaidAnomaly && isAnomaly)
            LoopManager.Instance.BreakLoop(); // correct guess
        else if (playerSaidAnomaly && !isAnomaly)
            Debug.Log("wrong guess - narrator line here"); // wrong guess

        PuzzleUI.Instance.Hide();
    }
}