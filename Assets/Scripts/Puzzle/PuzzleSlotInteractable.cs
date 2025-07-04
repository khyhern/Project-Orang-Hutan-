using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzleSlotInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("The item this slot expects to receive.")]
    public string expectedItemName;

    [Tooltip("Reference to the puzzle manager that owns this slot.")]
    public PuzzleManager manager;

    private PuzzleItemData placedItem;
    private GameObject spawnedInstance;

    public static PuzzleSlotInteractable ActiveSlot { get; private set; }

    public void Interact()
    {
        if (placedItem != null)
        {
            Debug.Log("[PuzzleSlot] Slot already filled.");
            return;
        }

        ActiveSlot = this;
        InventoryUI.Instance.OpenInventory();
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

        // Spawn the actual visual object at the slot
        if (item.prefab != null)
        {
            spawnedInstance = Instantiate(item.prefab, transform.position, transform.rotation, transform);
        }
        else
        {
            Debug.LogWarning($"[PuzzleSlot] Item '{item.itemName}' has no prefab assigned.");
        }

        InventorySystem.Instance.RemoveItem(item);
        InventoryUI.Instance.RefreshDisplay();

        Debug.Log($"[PuzzleSlot] Placed {item.itemName} into slot expecting {expectedItemName}");

        manager?.CheckPuzzleState();

        // Clear the global slot reference after use
        ActiveSlot = null;
    }

    public string GetPlacedItemName() => placedItem?.itemName;
}
