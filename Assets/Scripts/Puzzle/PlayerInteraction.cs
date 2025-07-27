using UnityEngine;
<<<<<<< HEAD
=======
using TMPro;
>>>>>>> parent of da63deb (organize scripts)

[RequireComponent(typeof(Collider))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
<<<<<<< HEAD
    [Tooltip("Max distance to interact with objects.")]
    [SerializeField] private float interactRange = 3f;

    [Tooltip("Which layers contain interactable objects.")]
    [SerializeField] private LayerMask interactLayer;

    [Tooltip("Optional: Use a specific camera for raycasting. If null, will default to Camera.main.")]
    [SerializeField] private Camera raycastCamera;

    private void Awake()
    {
        if (raycastCamera == null)
        {
            raycastCamera = Camera.main;
        }

        if (raycastCamera == null)
        {
            Debug.LogError("[PlayerInteraction] No camera assigned and no MainCamera found.");
        }
=======
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
>>>>>>> parent of da63deb (organize scripts)
    }

    private void Update()
    {
<<<<<<< HEAD
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (raycastCamera == null) return;

        Vector3 origin = raycastCamera.transform.position;
        Vector3 direction = raycastCamera.transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
                Debug.DrawRay(origin, direction * interactRange, Color.green, 1f);
            }
            else
            {
                Debug.LogWarning($"[PlayerInteraction] Hit '{hit.collider.name}' but it doesn't implement IInteractable.");
            }
        }
        else
        {
            Debug.DrawRay(origin, direction * interactRange, Color.red, 1f);
        }
=======
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
                    if (interactable is IDescriptiveInteractable descriptive)
                    {
                        interactText.text = $"Press [{interactKey}] to {descriptive.GetInteractionVerb()} {descriptive.GetObjectName()}";
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
>>>>>>> parent of da63deb (organize scripts)
    }
}
