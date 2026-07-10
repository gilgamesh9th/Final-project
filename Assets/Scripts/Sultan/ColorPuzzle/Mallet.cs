using UnityEngine;
using System.Collections;

public class Mallet : MonoBehaviour
{
    public Vector3 holdPos = new Vector3(0.35f, -0.25f, 0.5f);
    public Vector3 holdRot = new Vector3(0f, 0f, -45f);
    public float strikeAngle = 45f;
    public float downTime = 0.08f;
    public float upTime = 0.12f;
    public Collider pickupZoneCol;
    public MalletTip tip;
    public Material outlineMaterial;
    public bool playerInRange = false;

    private bool isHeld = false;
    private bool isStriking = false;
    private Transform cam;
    private Vector3 originalScale;
    private Rigidbody rb;
    private Collider mainCol;
    private Highlightable highlightedBar;

    void Start()
    {
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        mainCol = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHeld)
                Drop();
            else if (playerInRange)
                Pickup();
        }

        if (isHeld && !isStriking && Input.GetMouseButtonDown(0))
            StartCoroutine(Strike());

        if (isHeld && !isStriking)
            UpdateBarHighlight();
        else
            ClearBarHighlight();
    }

    void Pickup()
    {
        isHeld = true;
        Highlightable h = GetComponent<Highlightable>();
        if (h != null) h.Unhighlight();

        if (rb) rb.isKinematic = true;
        if (mainCol) mainCol.enabled = false;
        if (pickupZoneCol) pickupZoneCol.enabled = false;

        originalScale = transform.localScale;
        transform.SetParent(cam);
        transform.localPosition = holdPos;
        transform.localRotation = Quaternion.Euler(holdRot);
        transform.localScale = originalScale;
    }

    void Drop()
    {
        ClearBarHighlight();

        transform.SetParent(null);

        if (rb) rb.isKinematic = false;
        if (mainCol) mainCol.enabled = true;
        if (pickupZoneCol) pickupZoneCol.enabled = true;

        isHeld = false;
        playerInRange = false;
    }

    IEnumerator Strike()
    {
        isStriking = true;
        ClearBarHighlight();
        tip.canDetect = true;

        Quaternion startRot = transform.localRotation;
        Quaternion hitRot = startRot * Quaternion.Euler(strikeAngle, 0f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / downTime;
            transform.localRotation = Quaternion.Lerp(startRot, hitRot, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / upTime;
            transform.localRotation = Quaternion.Lerp(hitRot, startRot, t);
            yield return null;
        }

        tip.canDetect = false;
        isStriking = false;
    }

    void UpdateBarHighlight()
    {
        Vector3 tipStart = tip.transform.position;
        Quaternion strikeRot = transform.localRotation * Quaternion.Euler(strikeAngle, 0f, 0f);
        Vector3 tipInCamSpace = transform.localPosition + strikeRot * tip.transform.localPosition;
        Vector3 tipEnd = cam.TransformPoint(tipInCamSpace);

        Vector3 direction = tipEnd - tipStart;
        float distance = direction.magnitude;
        float tipRadius = tip.GetComponent<SphereCollider>().radius;
        int ignoreSelf = ~LayerMask.GetMask("Player");
        Debug.DrawLine(tipStart, tipEnd, Color.yellow);

        Highlightable newTarget = null;

        if (distance > 0.001f && Physics.SphereCast(
            tipStart, tipRadius, direction.normalized,
            out RaycastHit hit, distance, ignoreSelf,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.GetComponent<XylophoneBar>() != null)
                newTarget = hit.collider.GetComponent<Highlightable>();
        }

        if (newTarget != highlightedBar)
        {
            if (highlightedBar != null) highlightedBar.Unhighlight();
            if (newTarget != null) newTarget.Highlight(outlineMaterial);
            highlightedBar = newTarget;
        }
    }

    void ClearBarHighlight()
    {
        if (highlightedBar != null)
        {
            highlightedBar.Unhighlight();
            highlightedBar = null;
        }
    }
}