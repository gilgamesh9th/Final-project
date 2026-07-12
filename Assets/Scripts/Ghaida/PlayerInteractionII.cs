using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionII : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 15f;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private GameObject interactionDot; // drag your dot UI Image here

    private bool isLookingAtInteractable;

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
        CheckForInteractable();

        if (interactAction.action.WasPressedThisFrame() && isLookingAtInteractable)
        {
            Interact();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, interactDistance);

        bool showDot = hitSomething &&
            (hit.collider.TryGetComponent(out IInteractable _) ||
             hit.collider.TryGetComponent(out InteractionDotMarker _));

        if (showDot != isLookingAtInteractable)
        {
            isLookingAtInteractable = showDot;
            interactionDot.SetActive(isLookingAtInteractable);
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