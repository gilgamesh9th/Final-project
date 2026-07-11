using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class VoicedLine
{
    [TextArea] public string text;
    public AudioClip clip;
    public bool mustComplete;
    public float postDelay = 0.5f;
}

public class NarratorManager : MonoBehaviour
{
    public static NarratorManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI narratorText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private float gapBetweenLines = 0.5f;

    private struct QueuedLine
    {
        public string text;
        public AudioClip clip;
    }

    private readonly Queue<QueuedLine> _queue = new Queue<QueuedLine>();
    private bool _isDisplaying;
    private int _currentPriority = -1;
    private object _currentOwner;
    public int CurrentPriority => _currentPriority;
    public object CurrentOwner => _currentOwner;
    private object _holdOwner; //

    public bool TryClaim(int priority, object owner)
    {
        if (owner == _currentOwner)
        {
            _currentPriority = priority;
            return true;
        }
        if (priority < _currentPriority) return false;

        _currentPriority = priority;
        _currentOwner = owner;
        return true;
    }

    public void Hold(object owner) { _holdOwner = owner; }

    public void ReleaseHold(object owner)
    {
        if (_holdOwner != owner) return;
        _holdOwner = null;
        Release(owner);
    }

    public void Release(object owner)
    {
        if (_currentOwner != owner) return;
        _currentPriority = -1;
        _currentOwner = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        narratorText.text = "";
    }

    private void PlayClip(AudioClip clip)
    {
        if (audioSource == null) return;
        audioSource.Stop();
        if (clip == null) return;
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void StopClip()
    {
        if (audioSource == null) return;
        audioSource.Stop();
    }


    public bool Say(string text, int priority, object owner, AudioClip clip = null)
    {
        if (!TryClaim(priority, owner)) return false;

        StopAllCoroutines();
        StopClip();
        _isDisplaying = false;
        _queue.Enqueue(new QueuedLine { text = text, clip = clip });
        StartCoroutine(DrainQueue(owner));
        return true;
    }

    public bool SayImmediate(string text, int priority, object owner, AudioClip clip = null)
    {
        if (!TryClaim(priority, owner)) return false;

        StopAllCoroutines();
        StopClip();
        _queue.Clear();
        _isDisplaying = false;
        _queue.Enqueue(new QueuedLine { text = text, clip = clip });
        StartCoroutine(DrainQueue(owner));
        return true;
    }

    public bool ShowText(string text, int priority, object owner, AudioClip clip = null)
    {
        if (!TryClaim(priority, owner)) return false;

        StopAllCoroutines();
        _queue.Clear();
        _isDisplaying = false;
        narratorText.text = text;
        PlayClip(clip);
        return true;
    }

    public void ClearText(object owner)
    {
        if (_currentOwner != null && _currentOwner != owner) return;

        StopAllCoroutines();
        _queue.Clear();
        _isDisplaying = false;
        narratorText.text = "";
        StopClip();
    }

    public void Say(string text)
    {
        Say(text, 0, null, null);
    }

    public void SayImmediate(string text)
    {
        SayImmediate(text, 0, null, null);
    }

    public void ShowText(string text)
    {
        ShowText(text, 0, null, null);
    }

    public void ClearText()
    {
        ClearText(null);
    }

    private IEnumerator DrainQueue(object owner)
    {
        _isDisplaying = true;
        while (_queue.Count > 0)
        {
            var line = _queue.Dequeue();
            float duration = line.clip != null ? line.clip.length : displayDuration;
            narratorText.text = line.text;
            PlayClip(line.clip);
            yield return new WaitForSeconds(duration);
            narratorText.text = "";
            StopClip();
            if (_queue.Count > 0)
                yield return new WaitForSeconds(gapBetweenLines);
        }
        _isDisplaying = false;
        if (_holdOwner != owner) Release(owner);
    }

}