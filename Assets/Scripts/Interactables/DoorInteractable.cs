using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorInteractable : MonoBehaviour, IDescriptiveInteractable
{
    [Header("Unlock Requirements")]
    [SerializeField] private BaseItemData requiredItem;
    [SerializeField] private bool consumeItem = true;

    [Header("Optional Animation")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTrigger = "Open";

    [Header("Optional Teleport")]
    [SerializeField] private bool teleportAfterUnlock = false;
    [SerializeField] private Transform teleportTarget;

    [Header("Interaction Settings")]
    [SerializeField] private bool showPrompt = true;
    [SerializeField] private InteractionGroup interactionGroup = InteractionGroup.Default;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorOpenSFX;
    [SerializeField][Range(0f, 1f)] private float doorSFXVolume = 1f;

    private bool isUnlocked = false;

    public void Interact()
    {
        if (isUnlocked) return;

        if (CutsceneEnemyController.IsChasing)
        {
            Debug.Log("[DoorInteractable] Cannot unlock: enemy is chasing.");
            return;
        }

        if (SuspicionCheckManager.Instance?.IsCountdownRunning == true)
        {
            SubtitleUI.Instance?.ShowSubtitle("It’s coming... I need to pretend to be asleep.", 3f);
            Debug.Log("[DoorInteractable] Blocked door interaction during inspection countdown.");
            return;
        }

        if (InventorySystem.Instance == null || requiredItem == null)
        {
            Debug.LogWarning("[DoorInteractable] Missing InventorySystem or required item.");
            return;
        }

        foreach (var item in InventorySystem.Instance.GetAllItems())
        {
            if (item == requiredItem)
            {
                UnlockDoor();

                if (consumeItem)
                    InventorySystem.Instance.RemoveItem(item);

                if (teleportAfterUnlock && teleportTarget != null)
                    TeleportPlayer();

                return;
            }
        }

        Debug.Log("[DoorInteractable] Player does not have required item.");
        SubtitleUI.Instance?.ShowSubtitle("It's locked.", 2f);
    }

    private void UnlockDoor()
    {
        isUnlocked = true;

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openTrigger);
            Debug.Log("[DoorInteractable] Door opened via animation.");
        }
        else
        {
            Debug.Log("[DoorInteractable] Door unlocked (no animation).");
        }

        PlayDoorSFX();
    }

    private void PlayDoorSFX()
    {
        if (doorOpenSFX == null) return;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(doorOpenSFX, doorSFXVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(doorOpenSFX, transform.position, doorSFXVolume);
        }
    }

    private void TeleportPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = teleportTarget.position;
                controller.enabled = true;
                Debug.Log("[DoorInteractable] Player teleported (CharacterController safe).");
            }
            else
            {
                player.transform.position = teleportTarget.position;
                Debug.LogWarning("[DoorInteractable] Player teleported without CharacterController (fallback).");
            }
        }
        else
        {
            Debug.LogWarning("[DoorInteractable] Player not found.");
        }
    }

    public bool ShowPrompt => showPrompt;
    public string GetInteractionVerb() => isUnlocked ? "enter" : "unlock";
    public string GetObjectName() => gameObject.name;
    public string GetObjectID() => gameObject.name;
    public InteractionGroup GetInteractionGroup() => interactionGroup;
}
