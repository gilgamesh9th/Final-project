using UnityEngine;

public class MalletTip : MonoBehaviour
{
    public bool canDetect = false;
    private ColorPuzzleManager manager;

    void Start()
    {
        manager = FindObjectOfType<ColorPuzzleManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canDetect)
            return;

        XylophoneBar bar = other.GetComponent<XylophoneBar>();
        if (bar != null)
        {
            manager.CheckColor(bar.barColor);
            canDetect = false;
        }
    }
}