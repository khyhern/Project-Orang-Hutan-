using UnityEngine;

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

    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = true;

    private bool isOpen = false;
    private int interactionCount = 0;

    public void Interact()
    {
        if (isLocked)
        {
            DisplayFailMessage();
            return;
        }

        if (!isOpen)
        {
            openSound?.Play();
            animator?.SetTrigger("openDrawer");
        }
        else
        {
            closeSound?.Play();
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

        int messageIndex = Mathf.Min(interactionCount, failMessages.Length - 1);
        string message = failMessages[messageIndex];
        Debug.Log("[Drawer] " + message);

        if (SubtitleUI.Instance != null)
        {
            SubtitleUI.Instance.ShowSubtitle(message);
        }

        if (interactionCount < failMessages.Length - 1)
            interactionCount++;
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
