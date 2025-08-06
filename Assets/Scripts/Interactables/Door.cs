using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour, IDescriptiveInteractable
{
    public bool showPrompt = true;

    [Header("Lock Settings")]
    public bool requiresKey = true;
    public PuzzleItemData requiredKeyItem; // Assign in Inspector (ScriptableObject)
    public bool consumeKeyOnUse = false;

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
        if (messageText == null)
        {
            GameObject obj = GameObject.Find("InteractionDialogue");
            if (obj != null && obj.TryGetComponent(out TextMeshProUGUI tmp))
                messageText = tmp;
            else
                Debug.LogWarning("[DOOR] 'InteractionDialogue' TextMeshProUGUI not found.");
        }
    }

    public void Interact()
    {
        if (isOpened) return;

        if (requiresKey)
        {
            if (requiredKeyItem == null)
            {
                Debug.LogError("[Door] No PuzzleItemData key assigned.");
                return;
            }

            if (InventorySystem.Instance.HasItem(requiredKeyItem))
            {
                OpenDoor();

                if (consumeKeyOnUse)
                    InventorySystem.Instance.RemoveItem(requiredKeyItem);
            }
            else
            {
                DisplayMessage(lockedMessage, lockedTextColor);
            }
        }
        else
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpened = true;

        // TODO: Play animation, sound, etc.
        Debug.Log("[Door] Door opened.");
        DisplayMessage(successMessage, successTextColor);

        // Disable interaction prompt after opening
        enabled = false;
    }

    private void DisplayMessage(string message, Color color)
    {
        if (messageText == null) return;

        messageText.text = message;
        messageText.color = color;
        messageText.enabled = true;

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), messageDuration);
    }

    private void HideMessage()
    {
        if (messageText != null)
            messageText.enabled = false;
    }

    // For interaction prompt
    public string GetInteractionVerb() => "open";
    public string GetObjectName() => "door";
    public string GetObjectID() => "Door";
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
