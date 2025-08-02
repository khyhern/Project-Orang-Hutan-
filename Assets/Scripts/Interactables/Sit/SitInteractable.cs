using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SitInteractable : MonoBehaviour, IDescriptiveInteractable
{
    [Tooltip("Sit position and rotation.")]
    public Transform sitSpot; // Made public for direct access

    public void Interact()
    {
        var player = PlayerSittingController.Instance;
        if (player == null)
        {
            Debug.LogWarning("[SitInteractable] No PlayerSittingController found.");
            return;
        }

        player.SitAt(this);
    }

    public string GetInteractionVerb() => "sit on";
    public string GetObjectName() => gameObject.name;
    public string GetObjectID() => gameObject.name;
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
