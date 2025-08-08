using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class QTETrigger : MonoBehaviour
{
    [Header("QTE Settings")]
    public float interactRange = 2f;
    public LayerMask playerLayer;
    public GameObject qteCanvas;
    public float cooldownTime = 3f; // 3 seconds cooldown
    
    [Header("UI Feedback")]
    public GameObject interactUIObject;
    
    [Header("Door Animation")]
    public Animator doorAnimator; // Door's own animator
    public string doorAnimationName = "OpenDoor"; // Door animation trigger name

    [Header("Sound Range")]
    public float soundRange = 15f;

    [Header("Success Spawn")]
    public GameObject successSpawnPrefab; // Prefab to spawn on success
    public float spawnRadius = 2f; // Distance from trigger to spawn
    public float spawnHeight = 0f; // Height offset for spawn
    public bool hasSpawned = false; // Track if this trigger has already spawned

    private InputAction interactAction;
    private Camera mainCamera;
    private bool isQTEActive = false;
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private PointerController pointerController;
    private bool _playSound;

    // Camera
    private CinemachineInputAxisController _playerCameraController;

    public static Action<bool> OnQTEActive;

    void Awake()
    {
        mainCamera = Camera.main;
        var actions = InputSystem.actions;
        interactAction = actions.FindAction("Interact");
        _playerCameraController = GameObject.Find("FPCamera").GetComponent<CinemachineInputAxisController>();
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
        if (_playSound == true)
        {
            var sound = new Sound(transform.position, soundRange);
            Sounds.MakeSound(sound);
            Debug.Log($"Sound made at {transform.position} with range 10.");
        }

        // Handle cooldown timer
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
                Debug.Log("QTE cooldown finished for door: " + gameObject.name);
            }
        }
        
        if (isQTEActive || isOnCooldown) return; // Don't show interact UI if QTE is active or on cooldown
        
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

    // Gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isQTEActive || isOnCooldown) return; // Don't trigger if QTE is already active or on cooldown
        
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
        
        // Reset PointerController state if it exists (for reactivation)
        if (pointerController != null)
        {
            pointerController.ResetQTEState();
            // Activate random behaviors for this QTE session
            pointerController.ActivateQTE();
        }

        // You can add additional logic here like:
        // - Disable player movement
        OnQTEActive?.Invoke(false); // boolean to indicate player movement
        _playerCameraController.enabled = false; // Disable camera input during QTE

        // Play QTE start sound
        _playSound = true;
        AudioManager.Instance.PlaySFXQTE();

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

    // Spawn success object around this trigger
    private void SpawnSuccessObject()
    {
        if (hasSpawned || successSpawnPrefab == null) return;

        // Calculate random position around the trigger
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, spawnHeight, randomCircle.y);
        
        // Spawn the object
        GameObject spawnedObject = Instantiate(successSpawnPrefab, spawnPosition, Quaternion.identity);
        
        // Mark this trigger as having spawned
        hasSpawned = true;
        
        Debug.Log($"Success object spawned at {spawnPosition} for trigger: {gameObject.name}");
    }
    
    // Called when QTE succeeds
    public void OnQTESuccess()
    {
        Debug.Log("QTE Success for door: " + gameObject.name);
        
        // Play door animation
        PlayDoorAnimation();

        // Spawn success object (only if not already spawned)
        SpawnSuccessObject();

        // You can add additional success actions here like:
        OnQTEActive?.Invoke(true); // boolean to indicate player movement
        _playerCameraController.enabled = true; // Enable camera input after QTE

        // - Play success sound
        _playSound = false;
        AudioManager.Instance.StopSFXQTE();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.QTESuccess);

        // - Show success particles
        // - Unlock achievements
        // - Update game state
    }
    
    // Called when QTE is exited (ESC key)
    public void OnQTEExit()
    {
        Debug.Log("QTE Exited for door: " + gameObject.name);
        
        // Reset QTE state so it can be triggered again
        isQTEActive = false;
        
        // Start cooldown
        StartCooldown();

        // You can add additional exit actions here like:
        OnQTEActive?.Invoke(true); // boolean to indicate player movement
        _playerCameraController.enabled = true; // Enable camera input after QTE

        // - Play exit sound
        _playSound = false;
        AudioManager.Instance.StopSFXQTE();

        // - Show exit message
        // - Reset door state
        // - Don't play door animation (since it wasn't completed)
    }
    
    // Start the cooldown timer
    private void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        Debug.Log("QTE cooldown started for door: " + gameObject.name + " - " + cooldownTime + " seconds");
    }
    
    // Check if QTE is available (not active and not on cooldown)
    public bool IsQTEAvailable()
    {
        return !isQTEActive && !isOnCooldown;
    }
    
    // Get remaining cooldown time (for UI display)
    public float GetRemainingCooldown()
    {
        return isOnCooldown ? cooldownTimer : 0f;
    }

    // Reset spawn state (useful for testing or if you want to allow respawning)
    public void ResetSpawnState()
    {
        hasSpawned = false;
        Debug.Log($"Spawn state reset for trigger: {gameObject.name}");
    }
}
