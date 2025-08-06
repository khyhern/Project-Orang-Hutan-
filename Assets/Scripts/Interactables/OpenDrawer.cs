using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class OpenDrawer : MonoBehaviour, IDescriptiveInteractable
{
    [Header("Animation & Audio")]
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource openSound;
    [SerializeField] private AudioSource closeSound;

    [Tooltip("Fail messages shown when drawer is locked.")]
    [SerializeField] private string[] failMessages;
    [SerializeField] private Color lockedTextColor = Color.red;

    [Header("Message Display")]
    private TextMeshProUGUI messageText;
    [SerializeField] private float messageDuration = 3f;

    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = true;

    private bool isOpen = false;
    private int interactionCount = 0;


    private void Awake()
    {
        // Auto-assign text box by name if not set manually
        if (messageText == null)
        {
            GameObject obj = GameObject.Find("SubtitleText");
            if (obj != null && obj.TryGetComponent(out TextMeshProUGUI tmp))
            {
                messageText = tmp;
            }
            else
            {
                Debug.LogWarning("[Drawer] 'SubtitleText' TextMeshProUGUI not found.");
            }
        }
    }

    public void Interact()
    {
        if (isLocked)
        {
            DisplayFailMessage();
            return;
        }

        if (!isOpen)
        {
            if (openSound != null)
                openSound.Play();
            animator?.SetTrigger("openDrawer");
        }
        else
        {
            if (closeSound != null)
                closeSound.Play();
            animator?.SetTrigger("closeDrawer");
        }

        isOpen = !isOpen;
    }

    private void DisplayFailMessage()
    {
        if (failMessages == null || failMessages.Length == 0)
        {
            Debug.Log("[Drawer] Drawer is locked.");
            return;
        }

        // Clamp index to max length - 1
        int messageIndex = Mathf.Min(interactionCount, failMessages.Length - 1);
        string message = failMessages[messageIndex];
        Debug.Log("[Drawer] " + message);

        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = lockedTextColor;
            messageText.enabled = true;

            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), messageDuration);
        }

        // Only increment if not already at the last message
        if (interactionCount < failMessages.Length - 1)
            interactionCount++;
    }

    private void HideMessage()
    {
        if (messageText != null)
            messageText.enabled = false;
    }

    public string GetInteractionVerb()
    {
        if (isLocked) return "inspect";
        return isOpen ? "close" : "open";
    }

    public string GetObjectName() => "drawer";
    public string GetObjectID() => "Drawer";
    public bool IsOpen => isOpen;


    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;

    public void SetLocked(bool state) => isLocked = state;
}

