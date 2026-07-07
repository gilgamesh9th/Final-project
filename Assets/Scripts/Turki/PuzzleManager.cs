using System.Collections;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Correct Order")]
    public int[] correctOrder = { 1, 2, 3, 4 };

    [Header("Puzzle Items")]
    public PuzzleItem[] puzzleItems;

    [Header("Puzzle Buttons")]
    public PuzzleButton[] puzzleButtons;

    [Header("Screen")]
    public GameObject codeScreen;

    [Header("Door")]
    public Animator doorAnimator;

    [Header("Room Lock")]
    public GameObject doorBlocker;

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

    public void SelectButton(PuzzleButton button)
    {
        if (puzzleSolved) return;
        if (isResetting) return;
        if (button == null || button.statueItem == null) return;

        PuzzleItem item = button.statueItem;

        if (item.itemNumber == correctOrder[currentStep])
        {
            button.PressDown();
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

        yield return new WaitForSeconds(0.5f);

        foreach (PuzzleItem item in puzzleItems)
        {
            if (item != null)
                item.MoveBack();
        }

        foreach (PuzzleButton button in puzzleButtons)
        {
            if (button != null)
                button.RiseUp();
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

        if (doorBlocker != null)
            doorBlocker.SetActive(false);

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");
    }
}