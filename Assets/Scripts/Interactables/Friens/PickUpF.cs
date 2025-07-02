using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpF : MonoBehaviour
{
    public float interactRange = 2f;
    public LayerMask friendLayer;
    public LayerMask safepointLayer;
    public GameObject pickupUIObject;
    public GameObject safepointUIObject;
    public float carrySpeed = 2.5f;
    public float normalSpeed = 5f;

    private GameObject carriedFriend = null;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (carriedFriend == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactRange, friendLayer))
            {
                pickupUIObject.SetActive(true);
                safepointUIObject.SetActive(false);

                if (Keyboard.current.eKey.wasPressedThisFrame)
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
                pickupUIObject.SetActive(false);
                safepointUIObject.SetActive(false);
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactRange, safepointLayer))
            {
                pickupUIObject.SetActive(false);
                safepointUIObject.SetActive(true);

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    DropFriend(hit.point);
                }
            }
            else
            {
                pickupUIObject.SetActive(false);
                safepointUIObject.SetActive(false);
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
}