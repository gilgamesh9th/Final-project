using UnityEngine;

public class VCamPitchSync : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    void LateUpdate()
    {
        transform.localRotation = cameraTransform.localRotation;
    }
}