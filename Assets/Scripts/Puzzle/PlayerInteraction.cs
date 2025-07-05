using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Max distance to interact with objects.")]
    public float interactRange = 3f;

    [Tooltip("Which layers contain interactable objects.")]
    public LayerMask interactLayer;

    [Tooltip("Optional: Assign the camera that will be used for raycasting. If left empty, Camera.main will be used.")]
    public Camera raycastCamera;

    private void Awake()
    {
        // Fallback to main camera if none assigned
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
                Debug.Log($"[PlayerInteraction] Hit {hit.collider.name}, but it doesn't implement IInteractable.");
            }
        }
        else
        {
            Debug.DrawRay(origin, direction * interactRange, Color.red, 1f);
        }
    }
}
