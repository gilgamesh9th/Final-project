using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NarratorManager : MonoBehaviour
{
    public static NarratorManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI narratorText;
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private float gapBetweenLines = 0.5f;

    private readonly Queue<string> _queue = new Queue<string>();
    private bool _isDisplaying;

    private int _currentPriority = -1;
    private object _currentOwner;

    public int CurrentPriority => _currentPriority;
    public object CurrentOwner => _currentOwner;


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

    public bool Say(string text, int priority, object owner)
    {
        if (!TryClaim(priority, owner)) return false;

        StopAllCoroutines();
        _isDisplaying = false;
        _queue.Enqueue(text);
        StartCoroutine(DrainQueue(owner));
        return true;
    }

    public bool SayImmediate(string text, int priority, object owner)
    {
        if (!TryClaim(priority, owner)) return false;

        StopAllCoroutines();
        _queue.Clear();
        _isDisplaying = false;
        _queue.Enqueue(text);
        StartCoroutine(DrainQueue(owner));
        return true;
    }

    public bool ShowText(string text, int priority, object owner)
    {
        if (!TryClaim(priority, owner)) return false;

        StopAllCoroutines();
        _queue.Clear();
        _isDisplaying = false;
        narratorText.text = text;
        return true;
    }

    public void ClearText(object owner)
    {
        if (_currentOwner != null && _currentOwner != owner) return;

        StopAllCoroutines();
        _queue.Clear();
        _isDisplaying = false;
        narratorText.text = "";
    }

    public void Say(string text)
    {
        Say(text, 0, null);
    }

    public void SayImmediate(string text)
    {
        SayImmediate(text, 0, null);
    }

    public void ShowText(string text)
    {
        ShowText(text, 0, null);
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
            narratorText.text = _queue.Dequeue();
            yield return new WaitForSeconds(displayDuration);
            narratorText.text = "";
            if (_queue.Count > 0)
                yield return new WaitForSeconds(gapBetweenLines);
        }
        _isDisplaying = false;
        Release(owner);
    }
}