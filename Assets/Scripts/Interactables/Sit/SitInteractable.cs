using UnityEngine;

public class SitInteractable : Interactable
{
    [Tooltip("Sit position and rotation.")]
    public Transform sitSpot;

    public override void Interact()
    {
        var player = PlayerSittingController.Instance;
        if (player == null)
        {
            Debug.LogWarning("[SitInteractable] No PlayerSittingController found.");
            return;
        }

        player.SitAt(this);
    }

    public override string GetInteractionVerb() => "sit on";
    public override string GetObjectName() => gameObject.name;
    public override string GetObjectID() => gameObject.name;
}
