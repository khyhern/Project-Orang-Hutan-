using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyItem : MonoBehaviour, IDescriptiveInteractable
{
    [SerializeField] private string itemID = "Rusty Key";

    public void Interact()
    {
        InventoryManager.Instance.AddKeyItem(itemID);
        Debug.Log($"[KeyItem] {itemID} added to inventory.");
        Destroy(gameObject);
    }

    public string GetInteractionVerb() => "pick up";
    public string GetObjectID() => itemID;
    public string GetObjectName() => itemID.ToLower();
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
