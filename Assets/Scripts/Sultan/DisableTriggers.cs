using UnityEngine;

public class DisableTriggers : MonoBehaviour
{
    [SerializeField] private GameObject[] triggers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject trigger in triggers)
            {
                trigger.SetActive(false);
            }
        }
    }
}
