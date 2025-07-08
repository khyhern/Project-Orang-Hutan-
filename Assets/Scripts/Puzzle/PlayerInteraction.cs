using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Max distance to interact with objects.")]
    [SerializeField] private float interactRange = 3f;

    [Tooltip("Which layers contain interactable objects.")]
    [SerializeField] private LayerMask interactLayer;

    [Tooltip("UI Text to show interaction prompt.")]
    [SerializeField] private TextMeshProUGUI interactText;

    [Tooltip("Key used to trigger interaction.")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Tooltip("Optional: Use a specific camera for raycasting. If null, defaults to Camera.main.")]
    [SerializeField] private Camera raycastCamera;

    private IInteractable currentTarget;

    private void Awake()
    {
        if (raycastCamera == null)
            raycastCamera = Camera.main;

        if (raycastCamera == null)
            Debug.LogError("[PlayerInteraction] No camera assigned and no MainCamera found.");
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
                currentTarget = interactable;

                if (interactText != null)
                {
                    if (interactable is Interactable fullInteractable)
                    {
                        string verb = fullInteractable.GetInteractionVerb();
                        string objectID = fullInteractable.GetObjectID();
                        interactText.text = $"Press [{interactKey}] to {verb} {objectID}";
                    }
                    else
                    {
                        interactText.text = $"Press [{interactKey}] to interact";
                    }

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
