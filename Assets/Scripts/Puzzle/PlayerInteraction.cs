using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
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
    }

    private void Update()
    {
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
    }
}
