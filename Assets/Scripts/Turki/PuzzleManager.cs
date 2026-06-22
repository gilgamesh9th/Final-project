using System.Collections;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Correct Order")]
    public int[] correctOrder = { 1, 2, 3, 4 };

    [Header("Puzzle Items")]
    public PuzzleItem[] puzzleItems;

    [Header("Screen")]
    public GameObject codeScreen;

    [Header("Door")]
    public Animator doorAnimator;

    [Header("Settings")]
    public float resetDelay = 1.5f;

    private int currentStep = 0;
    private bool puzzleSolved = false;
    private bool isResetting = false;

    void Start()
    {
        if (codeScreen != null)
            codeScreen.SetActive(false);
    }

    public void SelectItem(PuzzleItem item)
    {
        if (puzzleSolved) return;
        if (isResetting) return;

        if (item.itemNumber == correctOrder[currentStep])
        {
            item.MoveForward();
            currentStep++;

            if (currentStep >= correctOrder.Length)
            {
                PuzzleSolved();
            }
        }
        else
        {
            StartCoroutine(ResetPuzzle());
        }
    }

    private IEnumerator ResetPuzzle()
    {
        isResetting = true;

        yield return new WaitForSeconds(resetDelay);

        foreach (PuzzleItem item in puzzleItems)
        {
            item.MoveBack();
        }

        currentStep = 0;

        yield return new WaitForSeconds(resetDelay);

        isResetting = false;
    }

    private void PuzzleSolved()
    {
        puzzleSolved = true;

        if (codeScreen != null)
            codeScreen.SetActive(true);

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");
    }
}