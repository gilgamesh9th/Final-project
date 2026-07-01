using UnityEngine;
using UnityEngine.Rendering;

public class XylophoneBar : MonoBehaviour
{
    public PuzzleColor barColor;
    public Color emissionColor = Color.white;
    [Range(0.5f, 5f)]
    public float emissionIntensity = 2f;

    private GameObject glowOverlay;
    private Material glowMat;

    void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        glowOverlay = new GameObject("GlowOverlay");
        glowOverlay.transform.SetParent(transform, false);
        glowOverlay.transform.localScale = Vector3.one * 1.01f;

        glowOverlay.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
        MeshRenderer mr = glowOverlay.AddComponent<MeshRenderer>();
        mr.shadowCastingMode = ShadowCastingMode.Off;
        mr.receiveShadows = false;

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        glowMat = new Material(shader);
        glowMat.SetFloat("_Surface", 1f);
        glowMat.SetInt("_SrcBlend", (int)BlendMode.One);
        glowMat.SetInt("_DstBlend", (int)BlendMode.One);
        glowMat.SetInt("_ZWrite", 0);
        glowMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        glowMat.renderQueue = (int)RenderQueue.Transparent;
        glowMat.SetColor("_BaseColor", emissionColor * emissionIntensity);

        mr.material = glowMat;
        glowOverlay.SetActive(false);
    }

    public void StartEmitting()
    {
        glowMat.SetColor("_BaseColor", emissionColor * emissionIntensity);
        glowOverlay.SetActive(true);
    }

    public void StopEmitting()
    {
        glowOverlay.SetActive(false);
    }
}