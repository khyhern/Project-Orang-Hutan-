<<<<<<< HEAD
using UnityEngine;
=======
﻿using UnityEngine;
>>>>>>> parent of da63deb (organize scripts)

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
<<<<<<< HEAD
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
=======
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
>>>>>>> parent of da63deb (organize scripts)
