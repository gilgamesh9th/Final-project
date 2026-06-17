using UnityEngine;
using System.Collections;

public class MalletController : MonoBehaviour
{
    public float pickupRange = 3f;
    public Vector3 holdPos = new Vector3(0.35f, -0.25f, 0.5f);
    public Vector3 holdRot = new Vector3(0f, 0f, -45f);
    public float strikeAngle = 45f;
    public float downTime = 0.08f;
    public float upTime = 0.12f;
    public float strikeRange = 2.5f;

    private GameObject mallet;
    private bool holding = false;
    private bool striking = false;

    private Vector3 originalScale;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (holding)
                Drop();
            else
                TryPickup();
        }

        if (holding && !striking && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Strike());
        }
    }

    void TryPickup()
    {
        int ignoreSelf = ~LayerMask.GetMask("Player");
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, ignoreSelf))
        {
            Debug.Log("Ray hit: " + hit.collider.gameObject.name 
                    + " | Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("Mallet"))
            {
                Grab(hit.collider.gameObject);
            }
        }
    }

    void Grab(GameObject obj)
    {
        mallet = obj;
        holding = true;

        Rigidbody rb = mallet.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = mallet.GetComponent<Collider>();
        if (col) col.enabled = false;

        originalScale = mallet.transform.localScale;
        mallet.transform.SetParent(transform);
        mallet.transform.localPosition = holdPos;
        mallet.transform.localRotation = Quaternion.Euler(holdRot);
        mallet.transform.localScale = originalScale;
    }

    void Drop()
    {
        if (!mallet) return;

        mallet.transform.SetParent(null);

        Rigidbody rb = mallet.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        Collider col = mallet.GetComponent<Collider>();
        if (col) col.enabled = true;

        mallet = null;
        holding = false;
    }

    IEnumerator Strike()
    {
        striking = true;

        Quaternion startRot = mallet.transform.localRotation;
        Quaternion hitRot = startRot * Quaternion.Euler(strikeAngle, 0f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / downTime;
            mallet.transform.localRotation = Quaternion.Lerp(startRot, hitRot, t);
            yield return null;
        }

        CheckHit();

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / upTime;
            mallet.transform.localRotation = Quaternion.Lerp(hitRot, startRot, t);
            yield return null;
        }

        striking = false;
    }

    void CheckHit()
    {
        int ignoreSelf = ~LayerMask.GetMask("Player");
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, strikeRange, ignoreSelf))
        {
            XylophoneBar bar = hit.collider.GetComponent<XylophoneBar>();
            if (bar != null)
            {
                Debug.Log("Struck color: " + bar.barColor);
            }
        }
    }

}