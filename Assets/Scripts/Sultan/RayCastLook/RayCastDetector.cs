using UnityEngine;

public class RayCastDetector : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float raycastDistance = 15f;

    private LookTarget _previousTarget;

    private void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        LookTarget currentTarget = null;
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            hit.collider.TryGetComponent(out currentTarget);

        if (currentTarget != null && currentTarget != _previousTarget)
            currentTarget.OnLooked();

        _previousTarget = currentTarget;
    }
}