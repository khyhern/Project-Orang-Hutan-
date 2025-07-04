using UnityEngine;

public class PuzzleSlotInteractable : MonoBehaviour, IInteractable
{
    public string expectedItemName;
    public PuzzleManager manager;
    private PuzzleItemData placedItem;

    public void Interact()
    {
        if (placedItem != null)
        {
            Debug.Log("Slot already filled.");
            return;
        }

        var inventory = InventorySystem.Instance;
        var items = inventory.GetAllItems();

        if (items.Count == 0)
        {
            Debug.Log("Inventory is empty.");
            return;
        }

        // Just use the first item
        PuzzleItemData chosenItem = items[0];
        placedItem = chosenItem;
        inventory.RemoveItem(chosenItem);

        Debug.Log($"Placed {chosenItem.itemName} into slot expecting {expectedItemName}");
        manager.CheckPuzzleState();
    }

    public string GetPlacedItemName() => placedItem?.itemName;
}
