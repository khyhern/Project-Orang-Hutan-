using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour, IDescriptiveInteractable
{
    public bool showPrompt = true;

    [Header("Lock Settings")]
    [Tooltip("Is this door currently locked?")]
    [SerializeField] private bool isLocked = true;

    [Tooltip("Does this door require an item to unlock?")]
    [SerializeField] private bool requiresKey = true;

    [Tooltip("ID of the key required to unlock this door.")]
    [SerializeField] private string requiredKeyID = "Rusty Key";

    [Header("Identification")]
    [Tooltip("Unique name or ID for this door.")]
    [SerializeField] private string objectID = "Suspicious door";

    [Header("Messages")]
    [SerializeField] private string lockedMessage = "The door is locked.";
    [SerializeField] private string successMessage = "The door opens.";
    [SerializeField] private Color lockedTextColor = Color.red;
    [SerializeField] private Color successTextColor = Color.green;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageDuration = 3f;

    private bool isOpened = false;


    private void Awake()
    {
        // Auto-find the dialogue text by name if not set in Inspector
        if (messageText == null)
        {
            GameObject obj = GameObject.Find("InteractionDialogue");
            if (obj != null && obj.TryGetComponent(out TextMeshProUGUI tmp))
            {
                messageText = tmp;
            }
            else
            {
                Debug.LogWarning("[DOOR] 'InteractionDialogue' TextMeshProUGUI not found in scene.");
            }
        }
    }

    public void Interact()
    {
        if (isOpened) return;

        // Completely locked — must be unlocked externally
        if (isLocked)
        {
            DisplayMessage(lockedMessage, messageText, lockedTextColor);
            return;
        }

        // Unlock externally — check key (if required)
        if (!requiresKey || InventoryManager.Instance.HasKeyItem(requiredKeyID))
        {
            TryOpenDoor();
            DisplayMessage(successMessage, messageText, successTextColor);
        }
        else
        {
            DisplayMessage(lockedMessage, messageText, lockedTextColor);
        }
    }

    private void TryOpenDoor()
    {
        Debug.Log("[Door] Door opened.");

        isOpened = true;
        showPrompt = false; // Hide interaction prompt after opening
        enabled = false;

        // TODO: Add animation, sound, or open logic
        // Optional: InventoryManager.Instance.RemoveKeyItem(requiredKeyID);
    }

    private void DisplayMessage(string message, TextMeshProUGUI targetText, Color textColor)
    {
        if (string.IsNullOrWhiteSpace(message) || targetText == null)
            return;

        Debug.Log("[Door] " + message);

        targetText.text = message;
        targetText.color = textColor;
        targetText.enabled = true;

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), messageDuration);
    }

    private void HideMessage()
    {
        if (messageText != null)
            messageText.enabled = false;
    }

    public string GetInteractionVerb() => "open";
    public string GetObjectName() => "door";
    public string GetObjectID() => objectID;
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;

    // Allow other scripts to control lock state
    public void SetLocked(bool state) => isLocked = state;
    public void SetRequiresKey(bool state) => requiresKey = state;
}
