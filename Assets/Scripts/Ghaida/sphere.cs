using UnityEngine;

public class sphere : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("this is a sphere");
    }
}