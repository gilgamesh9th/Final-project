using UnityEngine;

public class HallwayObject : MonoBehaviour, IInteractable
{
    public bool isAnomaly = false;

    public void Interact()
    {
        if (!LoopManager.Instance.InteractionEnabled) return;

        if (isAnomaly)
            LoopManager.Instance.CorrectGuess();
        else
            LoopManager.Instance.WrongGuess();
    }
}