using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlashlightItem : MonoBehaviour, IDescriptiveInteractable
{
    [SerializeField] private string itemID = "Flashlight";
    [SerializeField] private GameObject flashlightPrefab;

    public void Interact()
    {
        FlashlightSpawn.Instance.SetFlashlightExists(true);
        Debug.Log($"[Flashlight] {itemID} collected.");
        Destroy(gameObject);
    }

    public string GetInteractionVerb() => "pick up";
    public string GetObjectID() => itemID;
    public string GetObjectName() => itemID.ToLower();
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
