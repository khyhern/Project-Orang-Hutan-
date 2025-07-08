using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public abstract void Interact();
    public abstract string GetInteractionVerb();
    public abstract string GetObjectName();
    public abstract string GetObjectID();

    public virtual InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
