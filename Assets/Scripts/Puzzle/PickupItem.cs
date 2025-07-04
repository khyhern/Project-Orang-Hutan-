using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public PuzzleItemData itemData;

    public void Interact()
    {
        InventorySystem.Instance.PickUp(itemData);
        gameObject.SetActive(false);
    }
}
