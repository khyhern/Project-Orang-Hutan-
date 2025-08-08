using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Cinemachine;

public class PickUpF : MonoBehaviour
{
    public float interactRange = 2f;
    public LayerMask friendLayer;
    public LayerMask safepointLayer;
    public LayerMask groundLayer;
    public GameObject pickupUIObject;
    public GameObject safepointUIObject;
    public GameObject AnimationObject;
    public GameObject transitionObject;
    [SerializeField] private SitInteractable targetchair;
    public GameObject videoPlayerParent; // Assign this in the inspector to the VideoPlayer prefab parent
    public GameObject videoStoreObject; // Assign this in the inspector to the child GameObject with VideoPlayer component
    public float videoFallbackDuration = 5f; // Fallback duration if no VideoPlayer found
    public float transitionWaitTime = 1f; // Wait time for transition animation
    public float carrySpeed = 2.5f;
    public float normalSpeed = 3f;
    [SerializeField] private float dropRange = 2f; // Max distance to drop friend

    [SerializeField] private GameObject _enemy;
    private GameObject carriedFriend = null;
    private PlayerMovement playerMovement;
    private InputAction interactAction;
    private InputAction dropAction;
    private Camera cameraRef;
    private int friendsDelivered = 0; // Track number of friends delivered
    private Vector3 friendOffset = new Vector3(0, 1, -1); // Behind the player
    private bool isInCutscene = false;

    void Awake()
    {
        cameraRef = Camera.main;
        var actions = InputSystem.actions;
        interactAction = actions.FindAction("Interact");
        dropAction = actions.FindAction("Drop");
    }

    void OnEnable()
    {
        if (interactAction != null) interactAction.performed += OnInteract;
        if (dropAction != null) dropAction.performed += OnDrop;
    }

    void OnDisable()
    {
        if (interactAction != null) interactAction.performed -= OnInteract;
        if (dropAction != null) dropAction.performed -= OnDrop;
    }

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (videoPlayerParent != null)
            videoPlayerParent.SetActive(false);
    }

    void Update()
    {
        if (carriedFriend == null)
        {
            Ray ray = new Ray(cameraRef.transform.position, cameraRef.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactRange, friendLayer))
            {
                pickupUIObject.SetActive(true);
                safepointUIObject.SetActive(false);
            }
            else
            {
                pickupUIObject.SetActive(false);
                safepointUIObject.SetActive(false);
            }
        }
        else
        {
            // Update friend position to stay behind player
            carriedFriend.transform.position = transform.position + transform.TransformDirection(friendOffset);
            
            Ray ray = new Ray(cameraRef.transform.position, cameraRef.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactRange, safepointLayer))
            {
                pickupUIObject.SetActive(false);
                safepointUIObject.SetActive(true);
            }
            else
            {
                pickupUIObject.SetActive(false);
                safepointUIObject.SetActive(false);
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (carriedFriend == null)
        {
            Ray ray = new Ray(cameraRef.transform.position, cameraRef.transform.forward);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, interactRange, friendLayer))
            {
                Debug.Log("hi friend");
                carriedFriend = hit.collider.gameObject;
                carriedFriend.transform.SetParent(transform);
                carriedFriend.transform.localPosition = friendOffset;
                carriedFriend.GetComponent<Rigidbody>().isKinematic = true;

                // Set carrying state and speed
                playerMovement.isCarryingFriend = true;
                playerMovement.carryingSpeed = carrySpeed;
                playerMovement.SetMoveSpeed(carrySpeed);
            }
        }
        else
        {
            // If looking at a safepoint, drop at safepoint
            Ray ray = new Ray(cameraRef.transform.position, cameraRef.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactRange, safepointLayer))
            {
                DropFriend(hit.point);
            }
        }
    }

    private void OnDrop(InputAction.CallbackContext context)
    {
        if (carriedFriend != null)
        {
            Ray ray = new Ray(cameraRef.transform.position, cameraRef.transform.forward);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * dropRange, Color.white);
            if (Physics.Raycast(ray, out hit, dropRange, groundLayer))
            {
                PutDownFriend(hit.point + Vector3.up * 0.1f); // Slightly above ground
            }
            else
            {
                // Optionally: Show a UI message "Can't drop here!"
            }
        }
    }

    public void DropFriend(Vector3 dropPosition)
    {
        if (carriedFriend != null)
        {
            friendsDelivered++;
            
            // Play the friend's delivery animation (child will be deleted after animation)
            FriendBehavior friendBehavior = carriedFriend.GetComponent<FriendBehavior>();
            if (friendBehavior != null)
            {
                friendBehavior.OnDeliveredToSafepoint();
            }

            // Destroy the friend after animation (adjust delay to match animation length)
            Destroy(carriedFriend, 1.5f);
            carriedFriend = null;

            // Reset carrying state and speed
            playerMovement.isCarryingFriend = false;
            playerMovement.SetMoveSpeed(normalSpeed);

            // Change UI text for 3 seconds
            StartCoroutine(ShowDeliveryMessage());

            // If this is the second friend delivered, trigger cutscene
            if (friendsDelivered == 2)
            {
                StartCoroutine(TriggerCutscene());
            }
        }
    }

    private System.Collections.IEnumerator ShowDeliveryMessage()
    {
        // Store original text
        TMP_Text originalText = null;
        if (safepointUIObject != null)
        {
            originalText = safepointUIObject.GetComponentInChildren<TMP_Text>();
        }

        // Change text to delivery message
        if (originalText != null)
        {
            string originalMessage = originalText.text;
            originalText.text = "You put your friend";
            
            yield return new WaitForSeconds(1f);
            
            // Restore original text
            originalText.text = originalMessage;
        }
    }

    private System.Collections.IEnumerator TriggerCutscene()
    {
        isInCutscene = true;
        
        // Disable enemy
        _enemy.SetActive(false);

        transitionObject.SetActive(true);
        
        // 1. Disable player input
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null) playerInput.enabled = false;
        
        // 2. Activate video GameObject first (use public reference)
        Debug.Log("[Cutscene] videoPlayerParent: " + videoPlayerParent);
        if (videoPlayerParent != null)
        {
            Debug.Log("[Cutscene] Setting videoPlayerParent active");
            videoPlayerParent.SetActive(true);
            
            // Use the assigned child GameObject with the VideoPlayer component
            Debug.Log("[Cutscene] videoStoreObject: " + videoStoreObject);
            UnityEngine.Video.VideoPlayer videoPlayer = null;
            if (videoStoreObject != null)
            {
                videoPlayer = videoStoreObject.GetComponent<UnityEngine.Video.VideoPlayer>();
                Debug.Log("[Cutscene] videoPlayer component: " + videoPlayer);
            }
            else
            {
                Debug.LogWarning("[Cutscene] videoStoreObject is null!");
            }
            
            if (videoPlayer != null)
            {
                Debug.Log("[Cutscene] Waiting 3.5 seconds before transition");
                yield return new WaitForSeconds(3.5f);

                // Play transition
                Animator transitionAnimator = null;
                if (AnimationObject != null)
                {
                    transitionAnimator = AnimationObject.GetComponent<Animator>();
                    if (transitionAnimator != null)
                    {
                        transitionAnimator.SetTrigger("End");
                        Debug.Log("[Cutscene] Triggered transition at 9s");
                    }
                }

                yield return new WaitForSeconds(1f); // Wait 1 more second (total 10s)
                Debug.Log("[Cutscene] 10 seconds passed, deactivating videoPlayerParent");
                videoPlayerParent.SetActive(false);

                // 4. End of animation, teleport player & trigger animation trigger "start"
                if (transitionAnimator != null)
                {
                    SitTeleportService.TeleportAndSitAt(targetchair);
                    transitionAnimator.SetTrigger("Start");
                }
            }
            else
            {
                Debug.LogWarning("[Cutscene] No VideoPlayer component found, waiting fallback duration");
                // If no VideoPlayer component, wait for a default duration
                yield return new WaitForSeconds(videoFallbackDuration); // Adjust time as needed
                videoPlayerParent.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("[Cutscene] videoPlayerParent is null!");
        }
        
        // Re-enable player components
        if (playerInput != null) playerInput.enabled = true;
        
        yield return new WaitForSeconds(2f);
        transitionObject.SetActive(false);
        
        isInCutscene = false;
    }

    public void PutDownFriend(Vector3 dropPosition)
    {
        if (carriedFriend != null)
        {
            carriedFriend.transform.SetParent(null);
            carriedFriend.transform.position = dropPosition;
            Rigidbody rb = carriedFriend.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
            carriedFriend = null;

            // Reset carrying state and speed
            playerMovement.isCarryingFriend = false;
            playerMovement.SetMoveSpeed(normalSpeed);
        }
    }
}