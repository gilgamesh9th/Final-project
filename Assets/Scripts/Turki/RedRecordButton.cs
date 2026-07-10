using System.Collections;
using UnityEngine;

public class RedRecordButton : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource storyAudio;

    [Header("Button Movement")]
    public Transform buttonVisual;
    public float pressDownAmount = 0.08f;
    public float moveSpeed = 5f;

    private Vector3 startLocalPosition;
    private Vector3 pressedLocalPosition;
    private Coroutine moveCoroutine;
    private bool wasPlaying = false;

    void Start()
    {
        if (buttonVisual == null)
            buttonVisual = transform;

        startLocalPosition = buttonVisual.localPosition;
        pressedLocalPosition = startLocalPosition + Vector3.down * pressDownAmount;
    }

    void Update()
    {
        if (wasPlaying && storyAudio != null && !storyAudio.isPlaying)
        {
            wasPlaying = false;
            MoveButton(startLocalPosition);
        }
    }

    public void Interact()
    {
        if (storyAudio == null) return;

        if (storyAudio.isPlaying)
        {
            storyAudio.Stop();
            wasPlaying = false;
            MoveButton(startLocalPosition);
        }
        else
        {
            storyAudio.Play();
            wasPlaying = true;
            MoveButton(pressedLocalPosition);
        }
    }

    private void MoveButton(Vector3 targetPosition)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveSmooth(targetPosition));
    }

    private IEnumerator MoveSmooth(Vector3 targetPosition)
    {
        while (Vector3.Distance(buttonVisual.localPosition, targetPosition) > 0.001f)
        {
            buttonVisual.localPosition = Vector3.MoveTowards(
                buttonVisual.localPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        buttonVisual.localPosition = targetPosition;
    }
}