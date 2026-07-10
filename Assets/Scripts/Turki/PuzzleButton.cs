using System.Collections;
using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    [Header("Target Statue")]
    public PuzzleItem statueItem;

    [Header("Puzzle Manager")]
    public PuzzleManager puzzleManager;

    [Header("Button Movement")]
    public Transform buttonVisual;
    public float pressDownAmount = 0.02f;
    public float moveSpeed = 5f;

    private Vector3 startLocalPosition;
    private Vector3 pressedLocalPosition;
    private Coroutine moveCoroutine;

    void Start()
    {
        if (buttonVisual == null)
            buttonVisual = transform;

        startLocalPosition = buttonVisual.localPosition;
        pressedLocalPosition = startLocalPosition + Vector3.down * pressDownAmount;
    }

    public void Interact()
    {
        if (puzzleManager != null)
        {
            puzzleManager.SelectButton(this);
        }
    }

    public void PressDown()
    {
        MoveButton(pressedLocalPosition);
    }

    public void RiseUp()
    {
        MoveButton(startLocalPosition);
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