using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    [Tooltip("Data representing this physical puzzle item.")]
    public BaseItemData itemData; // ✅ Use BaseItemData for flexibility

    [Tooltip("Which interaction group this pickup belongs to.")]
    [SerializeField] private InteractionGroup interactionGroup = InteractionGroup.Pickup;

    public void Interact()
    {
        if (itemData == null || InventorySystem.Instance == null) return;

        bool success = InventorySystem.Instance.PickUp(itemData);
        if (success)
        {
            Debug.Log($"[PickupItem] Picked up: {itemData.itemName}");
            Destroy(gameObject);
        }
        else
        {
            // ❌ Pickup failed — show feedback
            Debug.Log($"[PickupItem] Inventory full. Could not pick up: {itemData.itemName}");
            // AudioManager.Instance?.PlaySFX("InventoryFull"); // optional
            // UIManager.Instance?.ShowMessage("Inventory is full."); // optional

            PlayerInteraction player = FindObjectOfType<PlayerInteraction>();
            if (player != null)
                player.ShowMessage($"Cannot pick up {itemData.itemName}. Inventory is full.", 2f); // Duration 2 seconds
        }
    }

    public InteractionGroup GetInteractionGroup() => interactionGroup;
}
