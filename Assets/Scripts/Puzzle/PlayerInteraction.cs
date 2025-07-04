using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask interactLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, interactRange, interactLayer))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                interactable?.Interact();
            }

            Debug.DrawRay(origin, direction * interactRange, Color.red, 1f);
        }


    }
}
