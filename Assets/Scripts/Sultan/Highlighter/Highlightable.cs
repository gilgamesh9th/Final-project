using UnityEngine;

public class Highlightable : MonoBehaviour
{
    private GameObject outlineObj;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        outlineObj = new GameObject("_Outline");
        outlineObj.transform.SetParent(transform);
        outlineObj.transform.localPosition = Vector3.zero;
        outlineObj.transform.localRotation = Quaternion.identity;
        outlineObj.transform.localScale = Vector3.one;

        outlineObj.AddComponent<MeshFilter>().mesh = mf.mesh;
        outlineObj.AddComponent<MeshRenderer>();
        outlineObj.SetActive(false);
    }

    public void Highlight(Material outlineMat)
    {
        if (outlineObj == null) return;
        outlineObj.GetComponent<MeshRenderer>().material = outlineMat;
        outlineObj.SetActive(true);
    }

    public void Unhighlight()
    {
        if (outlineObj == null) return;
        outlineObj.SetActive(false);
    }
}