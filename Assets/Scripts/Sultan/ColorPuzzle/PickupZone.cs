using UnityEngine;

public class PickupZone : MonoBehaviour
{
    private Mallet mallet;

    void Start()
    {
        mallet = GetComponentInParent<Mallet>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            mallet.playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            mallet.playerInRange = false;
    }
}