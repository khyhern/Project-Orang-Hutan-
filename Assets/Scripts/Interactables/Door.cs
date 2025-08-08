using UnityEngine;
using TMPro;
using System.Collections;

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
                    DisplayMessage(successMessage);

                    if (consumeKeyOnUse)
                        InventorySystem.Instance.RemoveItem(requiredKeyItem);

                    return; // Stop here. Wait for next interaction to open door
                }
                else
                {
                    DisplayMessage(lockedMessage);
                    PlaySound(unlockFailClip);
                    return;
                }
            }
            else
            {
                // No key required
                isUnlocked = true;
                PlaySound(unlockSuccessClip);
                DisplayMessage(successMessage);
                return;
            }
        }

        // If unlocked, then open or close the door based on current state
        if (!isOpened)
        {
            OpenDoor(false);
        }
        else
        {
            CloseDoor(false);
        }
    }

    // For enemy used
    public void ForceOpen()
    {
        PlaySound(unlockSuccessClip);
        
        if (!isOpened)
        {
            OpenDoor(true);
            StartCoroutine(CloseDelay());
        }
    }

    private IEnumerator CloseDelay()
    {
        yield return new WaitForSeconds(3f);
        CloseDoor(true);
    }

    private void OpenDoor(bool enemy)
    {
        isOpened = true;

        Debug.Log("[Door] Door opened.");

        if (!enemy) DisplayMessage(openMessage);

        if (animator != null)
            animator.SetTrigger("OpenDoor");

        PlaySound(openDoorClip); // <-- Play door opening sound
    }


    private void CloseDoor(bool enemy)
    {
        isOpened = false;

        Debug.Log("[Door] Door closed.");
        if (!enemy) DisplayMessage(closeMessage);

        if (animator != null)
            animator.SetTrigger("CloseDoor");

        PlaySound(closeDoorClip); // <-- Play door closing sound
    }

    private void DisplayMessage(string message)
    {
        if (SubtitleUI.Instance != null)
        {
            SubtitleUI.Instance.ShowSubtitle(message);
        }
        else
        {
            Debug.Log("[Door] " + message);
        }
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
