using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactableLayer;
    public TextMeshProUGUI interactText;
    public KeyCode interactKey = KeyCode.E;

    private Interactable currentTarget;

    public Camera playerCamera; 

    void Update()
    {
        CheckForInteractable();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact();
        }
    }

    void CheckForInteractable()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("Player camera not assigned.");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Scene View ray visualization
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null && interactable.enabled)
            {
                currentTarget = interactable;
                interactText.text = $"Press [{interactKey}] to {interactable.GetInteractionVerb()} {interactable.GetObjectID()}";
                interactText.enabled = true;
                return;
            }
        }



        currentTarget = null;
        interactText.enabled = false;
    }
}
