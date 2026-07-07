// using System.Collections;
// using UnityEngine;

// public class PuzzleFailFeedback : MonoBehaviour
// {
//     [SerializeField] private Light failLight;
//     [SerializeField] private float peakIntensity = 8f;
//     [SerializeField] private float fadeDuration = 1.5f;
//     [SerializeField] private AudioClip failSound;

//     private AudioSource _audio;
//     private Coroutine _flashRoutine;

//     void Awake()
//     {
//         _audio = gameObject.AddComponent<AudioSource>();
//         _audio.playOnAwake = false;

//         if (failLight != null)
//             failLight.intensity = 0f;
//     }

//     public void Play()
//     {
//         if (_flashRoutine != null)
//             StopCoroutine(_flashRoutine);

//         _flashRoutine = StartCoroutine(FlashAndFade());
//     }

//     private IEnumerator FlashAndFade()
//     {
//         if (failSound != null)
//             _audio.PlayOneShot(failSound);

//         failLight.intensity = peakIntensity;

//         float elapsed = 0f;
//         while (elapsed < fadeDuration)
//         {
//             elapsed += Time.deltaTime;
//             failLight.intensity = Mathf.Lerp(peakIntensity, 0f, elapsed / fadeDuration);
//             yield return null;
//         }

//         failLight.intensity = 0f;
//         _flashRoutine = null;
//     }
// }

using System.Collections;
using UnityEngine;

public class PuzzleFailFeedback : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light failLight;
    [SerializeField] private float peakIntensity = 8f;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Sound")]
    [SerializeField] private AudioClip failSound;

    [Header("Narrator")]
    [SerializeField] private string failVarKey = "PuzzleFail";
    [SerializeField] private float narrationDuration = 5f;

    [Header("Var Writes After Fail")]
    [SerializeField] private VarWrite[] onFailWrites;

    private AudioSource _audio;
    private Coroutine _flashRoutine;

    void Awake()
    {
        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;

        if (failLight != null)
            failLight.intensity = 0f;
    }

    public void Play()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);

        _flashRoutine = StartCoroutine(FlashAndFade());
    }

    private IEnumerator FlashAndFade()
    {
        if (GameVarStore.Instance != null)
            GameVarStore.Instance.Set(failVarKey, 1);

        if (failSound != null)
            _audio.PlayOneShot(failSound);

        failLight.intensity = peakIntensity;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            failLight.intensity = Mathf.Lerp(peakIntensity, 0f, elapsed / fadeDuration);
            yield return null;
        }
        failLight.intensity = 0f;

        if (narrationDuration > fadeDuration)
            yield return new WaitForSeconds(narrationDuration - fadeDuration);

        if (GameVarStore.Instance != null)
            GameVarStore.Instance.Set(failVarKey, 1);

        ApplyWrites(onFailWrites);

        _flashRoutine = null;
    }

    private void ApplyWrites(VarWrite[] writes)
    {
        if (writes == null || GameVarStore.Instance == null) return;
        foreach (var w in writes)
        {
            if (w.mode == UpdateMode.Increment)
                GameVarStore.Instance.Add(w.key, w.value);
            else
                GameVarStore.Instance.Set(w.key, w.value);
                Debug.Log(w.key);
                Debug.Log(GameVarStore.Instance.Get(w.key));
        }
    }
}