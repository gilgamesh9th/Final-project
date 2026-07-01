using UnityEngine;
using System.Collections;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    [System.Serializable]
    public class AnomalyPair
    {
        public GameObject normalObject;
        public GameObject anomalyObject;
    }

    public AnomalyPair[] anomalyObjects; // assign pairs in inspector
    public AudioClip flickerSound;
    public PlayerTeleporter forwardTrigger; // assign end trigger to disable on solve

    private int loopCount = 0;
    private int guessesRemaining = 3;
    private int currentAnomalyIndex = 0;
    private bool interactionEnabled = true;
    private bool puzzleSolved = false;

    // narrator events
    public static event System.Action OnLoopStart;
    public static event System.Action OnWrongGuess;
    public static event System.Action OnGuessesExhausted;
    public static event System.Action OnLoopBreak;

    void Awake() => Instance = this;

    public bool InteractionEnabled => interactionEnabled && !puzzleSolved;

    public void OnLoopCompleted()
    {
        loopCount++;
        guessesRemaining = 3;
        interactionEnabled = true;

        if (loopCount >= 2)
            SwapAnomaly();

        OnLoopStart?.Invoke();
    }

    private void SwapAnomaly()
    {
        // hide previous anomaly
        if (currentAnomalyIndex < anomalyObjects.Length)
        {
            anomalyObjects[currentAnomalyIndex].anomalyObject.SetActive(false);
            anomalyObjects[currentAnomalyIndex].normalObject.SetActive(true);
        }

        // pick next
        currentAnomalyIndex = (loopCount - 2) % anomalyObjects.Length;

        // show new anomaly
        anomalyObjects[currentAnomalyIndex].normalObject.SetActive(false);
        anomalyObjects[currentAnomalyIndex].anomalyObject.SetActive(true);
    }

    public void WrongGuess()
    {
        guessesRemaining--;
        OnWrongGuess?.Invoke();

        if (guessesRemaining <= 0)
        {
            interactionEnabled = false;
            OnGuessesExhausted?.Invoke();
        }
    }

    public void CorrectGuess()
    {
        puzzleSolved = true;
        StartCoroutine(FlickerAndBreak());
    }

    private IEnumerator FlickerAndBreak()
    {
        AnomalyPair current = anomalyObjects[currentAnomalyIndex];
        AudioSource audio = GetComponent<AudioSource>();

        if (audio && flickerSound)
            audio.PlayOneShot(flickerSound);

        // flicker 4 times
        for (int i = 0; i < 4; i++)
        {
            current.anomalyObject.SetActive(true);
            current.normalObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            current.anomalyObject.SetActive(false);
            current.normalObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

        // settle on normal
        current.normalObject.SetActive(true);
        current.anomalyObject.SetActive(false);

        OnLoopBreak?.Invoke();

        // disable forward trigger
        if (forwardTrigger != null)
        {
            forwardTrigger.enabled = false;
            forwardTrigger.GetComponent<Collider>().enabled = false;
        }
    }
}