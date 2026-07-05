using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class PuzzleLevel
{
    public string levelName;
    public PuzzleColor[] sequence;
    public bool isComplete = false;
}

public class ColorPuzzleManager : MonoBehaviour
{
    public PuzzleLevel[] levels;
    public UnityEvent onPuzzleSolved;

    private int currentLevel = 0;
    private int currentStep = 0;
    private Dictionary<PuzzleColor, XylophoneBar> barMap = new Dictionary<PuzzleColor, XylophoneBar>();
    private List<XylophoneBar> emittingBars = new List<XylophoneBar>();

    public bool Level1Done => levels.Length > 0 && levels[0].isComplete;
    public bool Level2Done => levels.Length > 1 && levels[1].isComplete;
    public bool Level3Done => levels.Length > 2 && levels[2].isComplete;

    void Start()
    {
        XylophoneBar[] bars = FindObjectsOfType<XylophoneBar>();
        foreach (var bar in bars)
        {
            barMap[bar.barColor] = bar;
        }
    }

    public void CheckColor(PuzzleColor color)
    {
        if (currentLevel >= levels.Length)
            return;

        PuzzleLevel level = levels[currentLevel];

        if (color == level.sequence[currentStep])
        {
            if (barMap.TryGetValue(color, out XylophoneBar bar))
            {
                bar.StartEmitting();
                emittingBars.Add(bar);
            }

            currentStep++;
            Debug.Log("Correct! " + color + " (" + currentStep + "/" + level.sequence.Length + ")");

            if (currentStep >= level.sequence.Length)
            {
                level.isComplete = true;
                ResetAllBars();
                currentLevel++;
                currentStep = 0;
                Debug.Log("=== " + level.levelName + " COMPLETE ===");

                if (currentLevel >= levels.Length)
                {
                    Debug.Log("ALL LEVELS COMPLETE. PUZZLE SOLVED");
                    onPuzzleSolved?.Invoke();
                }
            }
        }
        else
        {
            ResetAllBars();
            currentStep = 0;
            Debug.Log("WRONG! Hit " + color + ", expected " + level.sequence[currentStep] + ". Resetting " + level.levelName);
        }
    }

    private void ResetAllBars()
    {
        foreach (var bar in emittingBars)
        {
            bar.StopEmitting();
        }
        emittingBars.Clear();
    }

    public PuzzleColor[] GetCurrentSequence()
    {
        if (currentLevel >= levels.Length)
            return null;

        return levels[currentLevel].sequence;
    }
}