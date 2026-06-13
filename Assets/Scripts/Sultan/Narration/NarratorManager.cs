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

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        narratorText.text = "";
    }

    public void Say(string text)
    {
        _queue.Enqueue(text);
        if (!_isDisplaying)
            StartCoroutine(DrainQueue());
    }

    private IEnumerator DrainQueue()
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
    }

    public void SayImmediate(string text)
    {
        StopAllCoroutines();
        _queue.Clear();
        _isDisplaying = false;
        _queue.Enqueue(text);
        StartCoroutine(DrainQueue());
    }
}