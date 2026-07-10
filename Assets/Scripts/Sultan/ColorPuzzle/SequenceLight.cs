using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ColorEntry
{
    public PuzzleColor puzzleColor;
    public Material material;
}

public class SequenceLight : MonoBehaviour
{
    public ColorPuzzleManager manager;
    public Light spotLight;
    public Renderer sphereRenderer;
    public float colorDuration = 1f;
    public float pauseDuration = 0.3f;
    public float loopPause = 1.5f;
    public float lightIntensity = 5f;
    public float lightRange = 4f;
    public float spotAngle = 60f;
    public float innerSpotAngle = 30f;
    public ColorEntry[] colors;

    private Material _originalMat;
    private int colorIndex = 0;
    private float timer = 0f;
    private bool inPause = false;
    private bool _active = false;
    private PuzzleColor[] lastSequence;
    private Dictionary<PuzzleColor, Material> materialMap = new Dictionary<PuzzleColor, Material>();
    private Dictionary<PuzzleColor, Color> colorMap = new Dictionary<PuzzleColor, Color>();

    void Start()
    {
        spotLight.enabled = false;
    }

    public void Activate()
    {
        if (_active) return;
        _active = true;

        _originalMat = sphereRenderer.sharedMaterial;

        spotLight.enabled = true;
        spotLight.type = LightType.Spot;
        spotLight.range = lightRange;
        spotLight.spotAngle = spotAngle;
        spotLight.innerSpotAngle = innerSpotAngle;

        foreach (var entry in colors)
        {
            materialMap[entry.puzzleColor] = entry.material;

            Color c = entry.material.HasProperty("_BaseColor")
                ? entry.material.GetColor("_BaseColor")
                : entry.material.color;
            colorMap[entry.puzzleColor] = c;
        }

        ShowColor(0);
    }

    void Update()
    {
        if (!_active) return;

        PuzzleColor[] seq = manager.GetCurrentSequence();

        if (seq == null)
        {
            TurnOff();
            _active = false;
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

        PuzzleColor pc = seq[index];

        if (materialMap.TryGetValue(pc, out Material m))
            sphereRenderer.material = m;

        spotLight.color = GetBarColor(pc);
        spotLight.intensity = lightIntensity;
    }

    void Dim()
    {
        sphereRenderer.material = _originalMat;
        spotLight.intensity = 0f;
    }

    void TurnOff()
    {
        sphereRenderer.material = _originalMat;
        spotLight.intensity = 0f;
        spotLight.enabled = false;
    }

    Color GetBarColor(PuzzleColor pc)
    {
        if (colorMap.TryGetValue(pc, out Color c))
            return c;
        return Color.white;
    }
}