using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    [Tooltip("Data representing this physical puzzle item.")]
    public PuzzleItemData itemData;

    [Tooltip("Which interaction group this pickup belongs to.")]
    [SerializeField] private InteractionGroup interactionGroup = InteractionGroup.Pickup;

    public void Interact()
    {
        if (itemData == null || InventorySystem.Instance == null) return;

        InventorySystem.Instance.PickUp(itemData);
        Debug.Log($"[PickupItem] Picked up: {itemData.itemName}");
        Destroy(gameObject);
    }

    public InteractionGroup GetInteractionGroup() => interactionGroup;
}