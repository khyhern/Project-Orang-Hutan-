using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    public PuzzleItemData itemData;

    public void Interact()
    {
        if (itemData == null)
        {
            Debug.LogWarning("[PickupItem] No item data assigned.");
            return;
        }

        InventorySystem.Instance.PickUp(itemData);
        Debug.Log($"[PickupItem] Picked up {itemData.itemName}");

        // Trigger OnDestroy to notify the slot
        Destroy(gameObject);
    }
}
