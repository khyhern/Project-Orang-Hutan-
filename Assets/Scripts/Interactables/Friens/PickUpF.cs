using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PickUpF : MonoBehaviour
{
    public float interactRange = 2f;
    public LayerMask friendLayer;
    public LayerMask safepointLayer;
    public LayerMask groundLayer;
    public GameObject pickupUIObject;
    public GameObject safepointUIObject;
    public float carrySpeed = 2.5f;
    public float normalSpeed = 3f;
    [SerializeField] private float dropRange = 2f; // Max distance to drop friend
    [SerializeField] private GameObject enemyPrefab; // Enemy to spawn in cutscene
    [SerializeField] private Camera mainCamera; // Reference to main camera for screen shake

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
        if (pickupUIObject != null)
        {
            originalText = pickupUIObject.GetComponentInChildren<TMP_Text>();
        }

        // Change text to delivery message
        if (originalText != null)
        {
            string originalMessage = originalText.text;
            originalText.text = "You put your friend";
            
            yield return new WaitForSeconds(3f);
            
            // Restore original text
            originalText.text = originalMessage;
        }
    }

    private System.Collections.IEnumerator TriggerCutscene()
    {
        isInCutscene = true;
        
        // Disable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Rotate player 180 degrees
        Vector3 targetRotation = transform.eulerAngles + new Vector3(0, 180, 0);
        float rotationTime = 1f;
        float elapsedTime = 0f;
        Vector3 startRotation = transform.eulerAngles;

        while (elapsedTime < rotationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationTime;
            transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        // Spawn enemy in front of player
        if (enemyPrefab != null)
        {
            Vector3 spawnPosition = transform.position + transform.forward * 3f;
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }

        // Screen shake for 1.5 seconds
        Vector3 originalCameraPosition = cameraRef.transform.localPosition;
        float shakeTime = 1.5f;
        elapsedTime = 0f;

        while (elapsedTime < shakeTime)
        {
            elapsedTime += Time.deltaTime;
            float shakeAmount = 0.1f * (1f - elapsedTime / shakeTime); // Decrease shake over time
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                0
            );
            cameraRef.transform.localPosition = originalCameraPosition + shakeOffset;
            yield return null;
        }

        // Reset camera position
        cameraRef.transform.localPosition = originalCameraPosition;

        // Teleport player to specified coordinates
        transform.position = new Vector3(-97.10668f, -10.08f, -83.05457f);
        //sitteleporterservice

        // Re-enable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

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