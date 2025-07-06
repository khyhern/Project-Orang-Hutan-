using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QTETrigger : MonoBehaviour
{
    [Header("QTE Settings")]
    public float interactRange = 2f;
    public LayerMask playerLayer;
    public GameObject qteCanvas;
    
    [Header("UI Feedback")]
    public GameObject interactUIObject;
    
    [Header("Door Animation")]
    public Animator doorAnimator; // Door's own animator
    public string doorAnimationName = "OpenDoor"; // Door animation trigger name
    
    private InputAction interactAction;
    private Camera mainCamera;
    private bool isQTEActive = false;
    private PointerController pointerController;

    void Awake()
    {
        mainCamera = Camera.main;
        var actions = InputSystem.actions;
        interactAction = actions.FindAction("Interact");
    }

    void OnEnable()
    {
        if (interactAction != null) interactAction.performed += OnInteract;
    }

    void OnDisable()
    {
        if (interactAction != null) interactAction.performed -= OnInteract;
    }

    void Start()
    {
        // Ensure QTE canvas is initially hidden
        if (qteCanvas != null)
        {
            qteCanvas.SetActive(false);
        }
        
        // Ensure interact UI is initially hidden
        if (interactUIObject != null)
        {
            interactUIObject.SetActive(false);
        }
        
        // Get reference to PointerController on the QTE canvas
        if (qteCanvas != null)
        {
            pointerController = qteCanvas.GetComponentInChildren<PointerController>();
            if (pointerController != null)
            {
                // Subscribe to success event
                StartCoroutine(WaitForPointerController());
            }
        }
    }
    
    IEnumerator WaitForPointerController()
    {
        // Wait a frame to ensure PointerController is initialized
        yield return null;
        
        if (pointerController != null)
        {
            // Set the QTETrigger reference so PointerController can call individual door methods
            pointerController.qteTrigger = this;
        }
    }

    void Update()
    {
        if (isQTEActive) return; // Don't show interact UI if QTE is active
        
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, playerLayer))
        {
            // Check if the hit object is this QTE trigger (door)
            if (hit.collider.gameObject == gameObject)
            {
                if (interactUIObject != null)
                {
                    interactUIObject.SetActive(true);
                }
            }
            else
            {
                if (interactUIObject != null)
                {
                    interactUIObject.SetActive(false);
                }
            }
        }
        else
        {
            if (interactUIObject != null)
            {
                interactUIObject.SetActive(false);
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isQTEActive) return; // Don't trigger if QTE is already active
        
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactRange, playerLayer))
        {
            // Check if the hit object is this QTE trigger (door)
            if (hit.collider.gameObject == gameObject)
            {
                StartQTE();
            }
        }
    }

    private void StartQTE()
    {
        Debug.Log("QTE Started for door: " + gameObject.name);
        isQTEActive = true;
        
        // Hide interact UI
        if (interactUIObject != null)
        {
            interactUIObject.SetActive(false);
        }
        
        // Show QTE canvas
        if (qteCanvas != null)
        {
            qteCanvas.SetActive(true);
        }
        
        // You can add additional logic here like:
        // - Disable player movement
        // - Play QTE start sound
        // - Start QTE sequence
    }

    public void EndQTE()
    {
        Debug.Log("QTE Ended for door: " + gameObject.name);
        isQTEActive = false;
        
        // Hide QTE canvas
        if (qteCanvas != null)
        {
            qteCanvas.SetActive(false);
        }
        
        // You can add additional logic here like:
        // - Re-enable player movement
        // - Play QTE end sound
        // - Handle QTE result
    }
    
    // Method to play door animation (can be called from PointerController or directly)
    public void PlayDoorAnimation()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(doorAnimationName);
            Debug.Log("Door animation triggered: " + doorAnimationName + " on door: " + gameObject.name);
        }
    }
    
    // Called when QTE succeeds
    public void OnQTESuccess()
    {
        Debug.Log("QTE Success for door: " + gameObject.name);
        
        // Play door animation
        PlayDoorAnimation();
        
        // You can add additional success actions here like:
        // - Play success sound
        // - Show success particles
        // - Unlock achievements
        // - Update game state
    }
}
