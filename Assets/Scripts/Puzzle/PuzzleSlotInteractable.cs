using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzleSlotInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("The item this slot expects to receive.")]
    public string expectedItemName;

    [Tooltip("Reference to the puzzle manager that owns this slot.")]
    public PuzzleManager manager;

    private PuzzleItemData placedItem;

    public void Interact()
    {
        if (placedItem != null)
        {
            Debug.Log("[PuzzleSlot] Slot already filled.");
            return;
        }

        var selectedItem = InventoryUI.Instance.GetSelectedItem();
        if (selectedItem == null)
        {
            Debug.Log("[PuzzleSlot] No item selected in inventory.");
            return;
        }

        if (!InventorySystem.Instance.HasItem(selectedItem))
        {
            Debug.Log("[PuzzleSlot] Selected item is no longer in inventory.");
            return;
        }

        placedItem = selectedItem;
        InventorySystem.Instance.RemoveItem(selectedItem);
        InventoryUI.Instance.RefreshDisplay();

        Debug.Log($"[PuzzleSlot] Placed {selectedItem.itemName} into slot expecting {expectedItemName}");

        manager?.CheckPuzzleState();
    }

    public string GetPlacedItemName() => placedItem?.itemName;
}
