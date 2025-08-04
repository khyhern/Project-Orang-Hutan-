using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyItem : MonoBehaviour, IDescriptiveInteractable
{
    [SerializeField] private PuzzleItemData puzzleItemData;  // Assign this in Inspector

    public void Interact()
    {
        if (puzzleItemData != null)
        {
            bool success = InventorySystem.Instance.PickUp(puzzleItemData);
            if (success)
            {
                Debug.Log($"[KeyItem] {puzzleItemData.itemName} added to inventory.");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"[KeyItem] Could not pick up {puzzleItemData.itemName}. Inventory might be full.");
            }
        }
        else
        {
            Debug.LogWarning("[KeyItem] PuzzleItemData is not assigned.");
        }
    }

    public string GetInteractionVerb() => "pick up";
    public string GetObjectID() => puzzleItemData != null ? puzzleItemData.itemName : "Key Item";
    public string GetObjectName() => puzzleItemData != null ? puzzleItemData.itemName.ToLower() : "key item";
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
