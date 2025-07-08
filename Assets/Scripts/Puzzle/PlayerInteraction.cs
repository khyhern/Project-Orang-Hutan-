using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private Camera raycastCamera;

    private IInteractable currentTarget;

    private void Awake()
    {
        if (raycastCamera == null)
            raycastCamera = Camera.main;
    }

    private void Update()
    {
        CheckForInteractable();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        if (raycastCamera == null) return;

        Ray ray = new Ray(raycastCamera.transform.position, raycastCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                if (!InteractionManager.IsGroupEnabled(interactable.GetInteractionGroup()))
                {
                    interactText.enabled = false;
                    currentTarget = null;
                    return;
                }

                currentTarget = interactable;

                if (interactText != null)
                {
                    interactText.text = $"Press [{interactKey}] to interact";
                    interactText.enabled = true;
                }
                return;
            }
        }

        currentTarget = null;
        if (interactText != null)
            interactText.enabled = false;
    }
}