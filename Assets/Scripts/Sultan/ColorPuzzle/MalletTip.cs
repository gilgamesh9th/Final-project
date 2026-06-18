using UnityEngine;

public class MalletTip : MonoBehaviour
{
    [HideInInspector] public bool canDetect = false;

    void OnTriggerEnter(Collider other)
    {
        if (!canDetect)
            return;

        XylophoneBar bar = other.GetComponent<XylophoneBar>();
        if (bar != null)
        {
            Debug.Log("Struck color: " + bar.barColor);
            canDetect = false;
        }
    }
}