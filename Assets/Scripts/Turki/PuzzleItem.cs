using System.Collections;
using UnityEngine;

public class PuzzleItem : MonoBehaviour
{
    [Header("Item Number")]
    public int itemNumber;

    [Header("Puzzle Manager")]
    public PuzzleManager puzzleManager;

    [Header("Movement")]
    public Transform forwardPoint;
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private Coroutine moveCoroutine;

    void Start()
    {
        startPosition = transform.position;
    }

    public void Interact()
    {
        puzzleManager.SelectItem(this);
    }

    public void MoveForward()
    {
        MoveToPosition(forwardPoint.position);
    }

    public void MoveBack()
    {
        MoveToPosition(startPosition);
    }

    private void MoveToPosition(Vector3 targetPosition)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveSmooth(targetPosition));
    }

    private IEnumerator MoveSmooth(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
    }
}