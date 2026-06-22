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
    public bool playerInRange = false;

    private bool isHeld = false;
    private bool isStriking = false;
    private Transform cam;
    private Vector3 originalScale;
    private Rigidbody rb;
    private Collider mainCol;

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
    }

    void Pickup()
    {
        isHeld = true;

        if (rb)
            rb.isKinematic = true;
        if (mainCol)
            mainCol.enabled = false;
        if (pickupZoneCol)
            pickupZoneCol.enabled = false;

        originalScale = transform.localScale;
        transform.SetParent(cam);
        transform.localPosition = holdPos;
        transform.localRotation = Quaternion.Euler(holdRot);
        transform.localScale = originalScale;
    }

    void Drop()
    {
        transform.SetParent(null);

        if (rb)
            rb.isKinematic = false;
        if (mainCol)
            mainCol.enabled = true;
        if (pickupZoneCol)
            pickupZoneCol.enabled = true;

        isHeld = false;
        playerInRange = false;
    }

    IEnumerator Strike()
    {
        isStriking = true;
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
}