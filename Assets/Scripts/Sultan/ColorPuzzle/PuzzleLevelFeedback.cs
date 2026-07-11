using System.Collections;
using UnityEngine;

public class PuzzleLevelFeedback : MonoBehaviour
{
    [SerializeField] private Light feedbackLight;
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private float successIntensity = 200f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float lightDuration = 3f;
    [SerializeField] private AudioSource spatialSource;
    [SerializeField] private AudioClip[] levelCompleteClips;

    private Color _originalColor;
    private Coroutine _routine;

    private void Start()
    {
        if (feedbackLight != null)
            _originalColor = feedbackLight.color;
    }

    public void OnLevelComplete(int levelIndex)
    {
        if (_routine != null)
            StopCoroutine(_routine);
        _routine = StartCoroutine(PlayFeedback(levelIndex));
    }

    private IEnumerator PlayFeedback(int levelIndex)
    {
        if (feedbackLight != null)
        {
            feedbackLight.color = successColor;
            feedbackLight.intensity = successIntensity;
            feedbackLight.enabled = true;
        }

        float holdTime = lightDuration;

        if (spatialSource != null && levelCompleteClips != null
            && levelCompleteClips.Length > 0)
        {
            if (levelIndex < levelCompleteClips.Length
                && levelCompleteClips[levelIndex] != null)
            {
                spatialSource.clip = levelCompleteClips[levelIndex];
                spatialSource.Play();
                holdTime = Mathf.Max(holdTime, spatialSource.clip.length);
            }
        }

        yield return new WaitForSeconds(holdTime);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (feedbackLight != null)
                feedbackLight.intensity =
                    Mathf.Lerp(successIntensity, 0f, elapsed / fadeDuration);
            yield return null;
        }

        if (feedbackLight != null)
        {
            feedbackLight.intensity = 0f;
            feedbackLight.color = _originalColor;
        }

        _routine = null;
    }
}