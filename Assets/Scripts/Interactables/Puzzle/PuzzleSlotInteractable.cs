using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzleSlotInteractable : MonoBehaviour, IInteractable
{
    [Header("Puzzle Settings")]
    [Tooltip("The item this slot expects (correct solution).")]
    public PuzzleItemData expectedItemData;

    [Tooltip("The item this slot originally started with.")]
    public PuzzleItemData originalItemData;

    [Tooltip("Spawn point for item visual prefab.")]
    public Transform itemSpawnPoint;

    [Header("Audio")]
    [Tooltip("Optional dedicated audio source for SFX.")]
    [SerializeField] private AudioSource sfxSource;

    [Tooltip("Sound played when item is placed.")]
    [SerializeField] private AudioClip placeItemSFX;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private PuzzleItemData placedItem;
    private GameObject spawnedInstance;

    public static PuzzleSlotInteractable ActiveSlot { get; private set; }

    public enum SlotState
    {
        Empty,
        Correct,
        Original,
        Wrong
    }

    public void Interact()
    {
        if (placedItem != null)
        {
            Debug.Log("[PuzzleSlot] Slot already filled.");
            return;
        }

        ActiveSlot = this;
        InventoryUI.Instance.OpenInventory(true);
        Debug.Log("[PuzzleSlot] Waiting for item via 'Use' button...");
    }

    public void PlaceItem(PuzzleItemData item)
    {
        if (item == null || placedItem != null)
        {
            Debug.LogWarning("[PuzzleSlot] Cannot place item: already filled or null.");
            return;
        }

        placedItem = item;

        if (item.prefab != null && itemSpawnPoint != null)
        {
            spawnedInstance = Instantiate(item.prefab);
            spawnedInstance.transform.rotation = item.prefab.transform.rotation;
            spawnedInstance.transform.localScale = item.prefab.transform.localScale;

            Transform bottomMarker = spawnedInstance.transform.Find("BottomMarker");
            if (bottomMarker != null)
            {
                Vector3 offset = spawnedInstance.transform.position - bottomMarker.position;
                spawnedInstance.transform.position = itemSpawnPoint.position + offset;
            }
            else
            {
                Debug.LogWarning($"[PuzzleSlot] '{item.itemName}' is missing a BottomMarker — using default pivot.");
                spawnedInstance.transform.position = itemSpawnPoint.position;
            }

            spawnedInstance.transform.SetParent(itemSpawnPoint, true);

            var refComponent = spawnedInstance.AddComponent<PuzzleSlotReference>();
            refComponent.AssignSlot(this);
        }
        else
        {
            Debug.LogWarning($"[PuzzleSlot] Item '{item.itemName}' has no prefab or spawn point.");
        }

        InventorySystem.Instance.RemoveItem(item);
        InventoryUI.Instance.RefreshDisplay();

        PlaySFX(placeItemSFX);

        Debug.Log($"[PuzzleSlot] Placed {item.itemName} into slot (Expected: {expectedItemData?.itemName})");

        PuzzleManager.Instance?.CheckPuzzleState();
        ActiveSlot = null;
    }

    public void ClearSlot()
    {
        Debug.Log($"[PuzzleSlot] Slot cleared (was holding: {placedItem?.itemName})");
        placedItem = null;
        spawnedInstance = null;
        PuzzleManager.Instance?.CheckPuzzleState();
    }

    public PuzzleItemData GetPlacedItem() => placedItem;
    public PuzzleItemData GetOriginalItem() => originalItemData;
    public string GetPlacedItemName() => placedItem?.itemName;

    public SlotState GetSlotState()
    {
        if (placedItem == null) return SlotState.Empty;
        if (placedItem == expectedItemData) return SlotState.Correct;
        if (placedItem == originalItemData) return SlotState.Original;
        return SlotState.Wrong;
    }

    public InteractionGroup GetInteractionGroup() => InteractionGroup.PuzzleA;

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, sfxVolume);
        }
    }
}
