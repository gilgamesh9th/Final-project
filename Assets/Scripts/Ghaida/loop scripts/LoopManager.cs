using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    public int loopCount = 0; // tracks how many loops completed
    public bool hasCompletedLoop = false; // true only after player hits forward trigger
    public GameObject normalObject; // assign in inspector
    public GameObject anomalyObject; // assign in inspector
    void Awake() => Instance = this;

    // odd loops show anomaly, even loops are normal
    public bool IsAnomalyVersion => loopCount % 2 == 1;

    // called by forward trigger
    public void OnPlayerReturned()
    {
        loopCount++;
        hasCompletedLoop = true;
    }

    // swaps anomaly on/off based on loop count
    public void UpdateHallway()
    {
        Debug.Log("updating hallway");
        normalObject.SetActive(!IsAnomalyVersion);
        anomalyObject.SetActive(IsAnomalyVersion);
    }

    // called when player correctly identifies anomaly
    public void BreakLoop()
    {
        Debug.Log("loop broken");
        // disable all triggers
        foreach (var trigger in FindObjectsByType<PlayerTeleporter>(FindObjectsSortMode.None))
        {
            Debug.Log("disabling " + trigger.gameObject.name);
            trigger.enabled = false;
            trigger.GetComponent<Collider>().enabled = false;
        }
        FindObjectsByType<HallwayEntrance>(FindObjectsSortMode.None)[0].enabled = false;
    }
}