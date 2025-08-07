using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour, IDescriptiveInteractable
{
    public bool showPrompt = true;

    [Header("Lock Settings")]
    public bool requiresKey = true;
    public PuzzleItemData requiredKeyItem;
    public bool consumeKeyOnUse = false;

    [Header("Messages")]
    [SerializeField] private string lockedMessage = "The door is locked.";
    [SerializeField] private string successMessage = "The door is now unlocked.";
    [SerializeField] private string openMessage = "The door opens.";
    [SerializeField] private string closeMessage = "The door closes.";
    [SerializeField] private Color lockedTextColor = Color.red;
    [SerializeField] private Color successTextColor = Color.green;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] public float messageDuration = 3f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSuccessClip;
    [SerializeField] private AudioClip unlockFailClip;
    [SerializeField] private AudioClip openDoorClip;
    [SerializeField] private AudioClip closeDoorClip;

    private bool isUnlocked = false;
    private bool isOpened = false;

    private void Awake()
    {
        if (animator == null)
            Debug.LogError("[Door] Animator component not found!");

        if (audioSource == null)
            Debug.LogError("[Door] AudioSource component not found!");

        if (messageText == null)
        {
            GameObject obj = GameObject.Find("SubtitleText");
            if (obj != null && obj.TryGetComponent(out TextMeshProUGUI tmp))
                messageText = tmp;
            else
                Debug.LogWarning("[Door] 'SubtitleText' TextMeshProUGUI not found.");
        }
    }

    public void Interact()
    {
        if (!isUnlocked)
        {
            // First time interaction – check for key
            if (requiresKey)
            {
                if (requiredKeyItem == null)
                {
                    Debug.LogError("[Door] No PuzzleItemData key assigned.");
                    return;
                }

                if (InventorySystem.Instance.HasItem(requiredKeyItem))
                {
                    isUnlocked = true;

                    PlaySound(unlockSuccessClip);
                    DisplayMessage(successMessage, successTextColor);

                    if (consumeKeyOnUse)
                        InventorySystem.Instance.RemoveItem(requiredKeyItem);

                    return; // Stop here. Wait for next interaction to open door
                }
                else
                {
                    DisplayMessage(lockedMessage, lockedTextColor);
                    PlaySound(unlockFailClip);
                    return;
                }
            }
            else
            {
                // No key required
                isUnlocked = true;
                PlaySound(unlockSuccessClip);
                DisplayMessage(successMessage, successTextColor);
                return;
            }
        }

        // If unlocked, then open or close the door based on current state
        if (!isOpened)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        isOpened = true;

        Debug.Log("[Door] Door opened.");
        DisplayMessage(openMessage, successTextColor);

        if (animator != null)
            animator.SetTrigger("OpenDoor");

        PlaySound(openDoorClip); // <-- Play door opening sound
    }


    private void CloseDoor()
    {
        isOpened = false;

        Debug.Log("[Door] Door closed.");
        DisplayMessage(closeMessage, successTextColor);

        if (animator != null)
            animator.SetTrigger("CloseDoor");

        PlaySound(closeDoorClip); // <-- Play door closing sound
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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // Interaction Prompt
    public string GetInteractionVerb() => isOpened ? "close" : "open";
    public string GetObjectName() => "door";
    public string GetObjectID() => "Door";
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
