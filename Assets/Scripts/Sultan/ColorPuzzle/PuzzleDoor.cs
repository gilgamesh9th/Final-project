using UnityEngine;
using System.Collections;

public class PuzzleDoor : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private Quaternion closedRot;
    private Quaternion openRot;
    private bool isOpen = false;

    void Start()
    {
        closedRot = transform.rotation;
        openRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            StartCoroutine(SwingOpen());
        }
    }

    IEnumerator SwingOpen()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Lerp(closedRot, openRot, t);
            yield return null;
        }
        transform.rotation = openRot;
    }
}