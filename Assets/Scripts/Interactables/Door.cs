using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour, IDescriptiveInteractable
{
    [Header("Door Requirements")]
    [SerializeField] private string requiredKeyID = "Rusty Key";

    [Header("Identification")]
    [SerializeField] private string objectID = "Suspicious door";

    private bool isOpened = false;

    public void Interact()
    {
        if (isOpened) return;

        if (InventoryManager.Instance.HasKeyItem(requiredKeyID))
        {
            Debug.Log("[Door] Door opened.");
            // TODO: Add animation or open logic here

            // Optional: remove the key from inventory
            // InventoryManager.Instance.RemoveKeyItem(requiredKeyID);

            isOpened = true;
            enabled = false;
        }
        else
        {
            Debug.Log("[Door] Door is locked. You need the correct key.");
        }
    }

    public string GetInteractionVerb() => "open";
    public string GetObjectName() => "door";
    public string GetObjectID() => objectID;
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
