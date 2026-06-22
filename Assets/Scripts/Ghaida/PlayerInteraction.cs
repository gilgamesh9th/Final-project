using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 15f;
    [SerializeField] private InputActionReference interactAction;

    private void OnEnable()
    {
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
    }

    private void Update()
    {
        if (interactAction.action.WasPressedThisFrame())
        {
            Interact();
        }
    }

    private void Interact()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
            }
        }
    }
}