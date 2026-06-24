using UnityEngine;

public class HallwayEntrance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entrance hit by " + other.name);
        // only swap if player completed a full loop through reception
        if (other.CompareTag("Player") && LoopManager.Instance.hasCompletedLoop)
        {
            LoopManager.Instance.UpdateHallway();
            LoopManager.Instance.hasCompletedLoop = false; // reset for next loop
        }
    }
}