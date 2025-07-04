using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzleSlotInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("The item this slot expects (correct solution).")]
    public string expectedItemName;

    [Tooltip("The item this slot originally started with.")]
    public string originalItemName;

    [Tooltip("Reference to the puzzle manager that owns this slot.")]
    public PuzzleManager manager;

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

        if (item.prefab != null)
        {
            spawnedInstance = Instantiate(item.prefab, transform.position, transform.rotation, transform);

            var refComponent = spawnedInstance.AddComponent<PuzzleSlotReference>();
            refComponent.AssignSlot(this);
        }
        else
        {
            Debug.LogWarning($"[PuzzleSlot] Item '{item.itemName}' has no prefab assigned.");
        }

        InventorySystem.Instance.RemoveItem(item);
        InventoryUI.Instance.RefreshDisplay();

        Debug.Log($"[PuzzleSlot] Placed {item.itemName} into slot expecting {expectedItemName}");

        manager?.CheckPuzzleState();
        ActiveSlot = null;
    }

    public void ClearSlot()
    {
        Debug.Log($"[PuzzleSlot] Slot cleared (was holding: {placedItem?.itemName})");
        placedItem = null;
        spawnedInstance = null;
        manager?.CheckPuzzleState();
    }

    public string GetPlacedItemName() => placedItem?.itemName;

    public SlotState GetSlotState()
    {
        if (placedItem == null) return SlotState.Empty;
        if (placedItem.itemName == expectedItemName) return SlotState.Correct;
        if (placedItem.itemName == originalItemName) return SlotState.Original;
        return SlotState.Wrong;
    }
}
