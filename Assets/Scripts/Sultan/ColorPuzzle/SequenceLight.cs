using UnityEngine;
using System.Collections.Generic;

public class SequenceLight : MonoBehaviour
{
    public ColorPuzzleManager manager;
    public Light pointLight;
    public Renderer sphereRenderer;
    public float colorDuration = 1f;
    public float pauseDuration = 0.3f;
    public float loopPause = 1.5f;
    public float lightIntensity = 5f;
    private Material mat;
    private int colorIndex = 0;
    private float timer = 0f;
    private bool inPause = false;
    private PuzzleColor[] lastSequence;
    private Dictionary<PuzzleColor, Color> colorMap = new Dictionary<PuzzleColor, Color>();

    void Start()
    {
        mat = sphereRenderer.material;
        mat.SetColor("_EmissionColor", Color.black);

        XylophoneBar[] bars = FindObjectsOfType<XylophoneBar>();
        foreach (var bar in bars)
        {
            if (!colorMap.ContainsKey(bar.barColor))
            {
                colorMap[bar.barColor] = bar.GetComponent<Renderer>().material.color;
            }
        }

        ShowColor(0);
    }

    void Update()
    {
        PuzzleColor[] seq = manager.GetCurrentSequence();

        if (seq == null)
        {
            TurnOff();
            enabled = false;
            return;
        }

        if (seq != lastSequence)
        {
            lastSequence = seq;
            colorIndex = 0;
            timer = 0f;
            inPause = false;
            ShowColor(0);
        }

        timer += Time.deltaTime;

        if (!inPause && timer >= colorDuration)
        {
            inPause = true;
            timer = 0f;
            Dim();
        }
        else if (inPause)
        {
            bool lastColor = colorIndex >= seq.Length - 1;
            float wait = lastColor ? loopPause : pauseDuration;

            if (timer >= wait)
            {
                inPause = false;
                timer = 0f;
                colorIndex = (colorIndex + 1) % seq.Length;
                ShowColor(colorIndex);
            }
        }
    }

    void ShowColor(int index)
    {
        PuzzleColor[] seq = manager.GetCurrentSequence();
        if (seq == null || index >= seq.Length)
            return;

        Color c = GetBarColor(seq[index]);
        mat.SetColor("_BaseColor", c);
        pointLight.color = c;
        pointLight.intensity = lightIntensity;
    }

    void Dim()
    {
        mat.SetColor("_BaseColor", Color.black);
        pointLight.intensity = 0f;
    }

    void TurnOff()
    {
        mat.SetColor("_BaseColor", Color.black);
        pointLight.intensity = 0f;
    }

    Color GetBarColor(PuzzleColor pc)
    {
        if (colorMap.TryGetValue(pc, out Color c))
            return c;
        return Color.white;
    }
}