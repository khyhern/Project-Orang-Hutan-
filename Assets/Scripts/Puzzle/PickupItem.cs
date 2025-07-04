using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    [Tooltip("The item data this pickup represents.")]
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
        gameObject.SetActive(false); // or Destroy(gameObject)
    }
}
