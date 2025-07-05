using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpF : MonoBehaviour
{
    public float interactRange = 2f;
    public LayerMask friendLayer;
    public LayerMask safepointLayer;
    public LayerMask groundLayer;
    public GameObject pickupUIObject;
    public GameObject safepointUIObject;
    public float carrySpeed = 2.5f;
    public float normalSpeed = 5f;
    [SerializeField] private float dropRange = 2f; // Max distance to drop friend

    private GameObject carriedFriend = null;
    private PlayerMovement playerMovement;
    private InputAction interactAction;
    private InputAction dropAction;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
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
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
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
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
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
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactRange, friendLayer))
            {
                carriedFriend = hit.collider.gameObject;
                carriedFriend.transform.SetParent(transform);
                carriedFriend.transform.localPosition = new Vector3(0, 1, 1);
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
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
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
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
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
        }
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