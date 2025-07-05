using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    [Tooltip("The data representing this physical puzzle item.")]
    public PuzzleItemData itemData;

    public void Interact()
    {
        if (itemData == null)
        {
            Debug.LogWarning("[PickupItem] No item data assigned.");
            return;
        }

        if (InventorySystem.Instance == null)
        {
            Debug.LogError("[PickupItem] InventorySystem instance not found.");
            return;
        }

        InventorySystem.Instance.PickUp(itemData);
        Debug.Log($"[PickupItem] Picked up: {itemData.itemName}");

        Destroy(gameObject); // Will trigger PuzzleSlotReference.OnDestroy if attached
    }
}
