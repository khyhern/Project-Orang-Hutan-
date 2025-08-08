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
    [Tooltip("Array of prefabs to randomly choose from when spawning on success")]
    public GameObject[] successSpawnPrefabs; // Array of prefabs to spawn on success
    public float spawnRadius = 2f; // Distance from trigger to spawn
    public float spawnHeight = 0f; // Height offset for spawn
    public bool hasSpawned = false; // Track if this trigger has already spawned
    [Tooltip("Distance in front of the QTE trigger to spawn items")]
    public float spawnDistanceInFront = 2f; // Distance in front of trigger to spawn

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
    
    // Static variables to manage multiple QTE triggers
    private static QTETrigger currentActiveTrigger = null;
    private static List<QTETrigger> allTriggers = new List<QTETrigger>();
    
    // Public property to check if QTE is active
    public bool IsQTEActive => isQTEActive;
    
    // Static property to check if any QTE is currently active
    public static bool IsAnyQTEActive
    {
        get
        {
            foreach (var trigger in allTriggers)
            {
                if (trigger != null && trigger.IsQTEActive)
                {
                    return true;
                }
            }
            return false;
        }
    }

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
        
        // Register this trigger
        if (!allTriggers.Contains(this))
        {
            allTriggers.Add(this);
        }
    }

    void OnDisable()
    {
        if (interactAction != null) interactAction.performed -= OnInteract;
        
        // Unregister this trigger
        allTriggers.Remove(this);
        
        // If this was the current active trigger, clear it
        if (currentActiveTrigger == this)
        {
            currentActiveTrigger = null;
            HideAllInteractUI();
        }
    }
    
    void OnDestroy()
    {
        // Clean up when object is destroyed
        allTriggers.Remove(this);
        if (currentActiveTrigger == this)
        {
            currentActiveTrigger = null;
        }
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
            Debug.Log($"PointerController reference set for {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"PointerController not found for {gameObject.name}");
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
        
        // If this trigger has already spawned, disable it completely
        if (hasSpawned)
        {
            // Hide UI if it was showing
            if (currentActiveTrigger == this)
            {
                currentActiveTrigger = null;
                HideInteractUI();
            }
            return;
        }
        
        if (isQTEActive || isOnCooldown) return; // Don't show interact UI if QTE is active or on cooldown
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }
        
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        // Use a simple approach - raycast against everything and check if we hit this object
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Check if the hit object is this QTE trigger (door)
            if (hit.collider.gameObject == gameObject)
            {
                // This trigger is being looked at
                if (currentActiveTrigger != this)
                {
                    // Hide UI from previous trigger and show this one's UI
                    if (currentActiveTrigger != null)
                    {
                        currentActiveTrigger.HideInteractUI();
                    }
                    currentActiveTrigger = this;
                    ShowInteractUI();
                    Debug.Log($"QTE UI shown for {gameObject.name} - Distance: {hit.distance}, Layer: {gameObject.layer}");
                }
            }
            else
            {
                // This trigger is not being looked at
                if (currentActiveTrigger == this)
                {
                    currentActiveTrigger = null;
                    HideInteractUI();
                }
            }
        }
        else
        {
            // No hit detected
            if (currentActiveTrigger == this)
            {
                currentActiveTrigger = null;
                HideInteractUI();
            }
        }
    }
    
    // Helper methods for UI management
    private void ShowInteractUI()
    {
        if (interactUIObject != null)
        {
            interactUIObject.SetActive(true);
        }
    }
    
    private void HideInteractUI()
    {
        if (interactUIObject != null)
        {
            interactUIObject.SetActive(false);
        }
    }
    
    private static void HideAllInteractUI()
    {
        foreach (var trigger in allTriggers)
        {
            if (trigger != null)
            {
                trigger.HideInteractUI();
            }
        }
    }
    
    // Method to check if this trigger should be the active one
    private bool ShouldBeActiveTrigger()
    {
        if (mainCamera == null) return false;
        
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            return hit.collider.gameObject == gameObject;
        }
        
        return false;
    }

    // Gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // If this trigger has already spawned, don't allow interaction
        if (hasSpawned)
        {
            Debug.Log($"QTE trigger {gameObject.name} has already spawned items - interaction disabled");
            return;
        }
        
        if (isQTEActive || isOnCooldown) return; // Don't trigger if QTE is already active or on cooldown
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }
        
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        
        // Use a simple approach - raycast against everything and check if we hit this object
        if (Physics.Raycast(ray, out hit, interactRange))
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
        // If this trigger has already spawned, don't start QTE
        if (hasSpawned)
        {
            Debug.Log($"QTE trigger {gameObject.name} has already spawned items - QTE not started");
            return;
        }
        
        Debug.Log("QTE Started for door: " + gameObject.name);
        isQTEActive = true;
        
        // Hide interact UI for this trigger
        HideInteractUI();
        
        // Clear the current active trigger since we're starting QTE
        if (currentActiveTrigger == this)
        {
            currentActiveTrigger = null;
        }
        
        // Show QTE canvas
        if (qteCanvas != null)
        {
            qteCanvas.SetActive(true);
        }
        
        // Reset PointerController state if it exists (for reactivation)
        if (pointerController != null)
        {
            // Set this trigger as the active one for the PointerController
            pointerController.qteTrigger = this;
            pointerController.ResetQTEState();
            // Activate random behaviors for this QTE session
            pointerController.ActivateQTE();
            Debug.Log($"QTE started for {gameObject.name} - PointerController reference set");
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

    // Spawn success object in front of the QTE trigger
    private void SpawnSuccessObject()
    {
        if (hasSpawned || successSpawnPrefabs == null || successSpawnPrefabs.Length == 0) return;

        // Randomly choose a prefab from the array
        GameObject selectedPrefab = successSpawnPrefabs[UnityEngine.Random.Range(0, successSpawnPrefabs.Length)];
        
        if (selectedPrefab == null) return;

        // Spawn in front of the QTE trigger
        Vector3 spawnPosition;
        
        // Get the trigger's forward direction (assuming the trigger faces forward)
        Vector3 triggerForward = transform.forward;
        // Remove the Y component to keep it at ground level
        triggerForward.y = 0;
        triggerForward.Normalize();
        
        // Spawn at the specified distance in front of the trigger
        spawnPosition = transform.position + (triggerForward * spawnDistanceInFront);
        // Keep the same Y position as the trigger (or use spawnHeight)
        spawnPosition.y = transform.position.y + spawnHeight;
        
        // Spawn the object
        GameObject spawnedObject = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        
        // Mark this trigger as having spawned
        hasSpawned = true;
        
        Debug.Log($"Success object '{selectedPrefab.name}' spawned at {spawnPosition} for trigger: {gameObject.name}");
    }
    
    // Called when QTE succeeds
    public void OnQTESuccess()
    {
        Debug.Log("QTE Success for door: " + gameObject.name);
        
        // Play door animation
        PlayDoorAnimation();

        // Spawn success object (only if not already spawned)
        SpawnSuccessObject();

        // End QTE and restore UI state
        EndQTE();

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

        // End QTE and restore UI state
        EndQTE();

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

    // Debug method to check QTE trigger state
    [ContextMenu("Debug QTE Trigger State")]
    public void DebugQTEState()
    {
        Debug.Log($"=== QTE Trigger Debug for {gameObject.name} ===");
        Debug.Log($"Layer: {gameObject.layer} (Layer name: {LayerMask.LayerToName(gameObject.layer)})");
        Debug.Log($"Interact Range: {interactRange}");
        Debug.Log($"Player Layer Mask: {playerLayer.value}");
        Debug.Log($"Is QTE Active: {isQTEActive}");
        Debug.Log($"Is On Cooldown: {isOnCooldown}");
        Debug.Log($"Has Spawned: {hasSpawned}");
        Debug.Log($"Spawn Distance In Front: {spawnDistanceInFront}");
        Debug.Log($"Interact UI Object: {(interactUIObject != null ? interactUIObject.name : "NULL")}");
        Debug.Log($"QTE Canvas: {(qteCanvas != null ? qteCanvas.name : "NULL")}");
        Debug.Log($"Main Camera: {(mainCamera != null ? mainCamera.name : "NULL")}");
        Debug.Log($"Has Collider: {GetComponent<Collider>() != null}");
        Debug.Log($"Collider is trigger: {(GetComponent<Collider>() != null ? GetComponent<Collider>().isTrigger : "N/A")}");
        Debug.Log($"Is Current Active Trigger: {currentActiveTrigger == this}");
        Debug.Log($"Total Triggers Registered: {allTriggers.Count}");
        Debug.Log("==========================================");
    }
    
    // Static debug method to check all triggers
    [ContextMenu("Debug All QTE Triggers")]
    public static void DebugAllTriggers()
    {
        Debug.Log($"=== All QTE Triggers Debug ===");
        Debug.Log($"Total Triggers: {allTriggers.Count}");
        Debug.Log($"Current Active Trigger: {(currentActiveTrigger != null ? currentActiveTrigger.name : "None")}");
        
        for (int i = 0; i < allTriggers.Count; i++)
        {
            var trigger = allTriggers[i];
            if (trigger != null)
            {
                Debug.Log($"Trigger {i}: {trigger.name} - Active: {trigger.IsQTEAvailable() == false}, Cooldown: {trigger.GetRemainingCooldown() > 0}");
            }
            else
            {
                Debug.Log($"Trigger {i}: NULL (destroyed)");
            }
        }
        Debug.Log("===============================");
    }
}
