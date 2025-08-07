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
    [SerializeField] public float messageDuration = 3f;

    [Header("Animatior")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSuccessClip;
    [SerializeField] private AudioClip unlockFailClip;

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
                Debug.LogWarning("[DOOR] 'SubtitleText' TextMeshProUGUI not found.");
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

                // Play success sound
                PlaySound(unlockSuccessClip);
            }
            else
            {
                DisplayMessage(lockedMessage, lockedTextColor);

                // Play fail sound
                PlaySound(unlockFailClip);
            }
        }
        else
        {
            OpenDoor();

            // Play success sound
            PlaySound(unlockSuccessClip);
        }
    }

    private void OpenDoor()
    {
        isOpened = true;

        // TODO: Play animation, sound, etc.
        Debug.Log("[Door] Door opened.");
        DisplayMessage(successMessage, successTextColor);

        // Play door opening animation
        if (animator != null)
            animator.SetTrigger("OpenDoor");

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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }


    // For interaction prompt
    public string GetInteractionVerb() => "open";
    public string GetObjectName() => "door";
    public string GetObjectID() => "Door";
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
