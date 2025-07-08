public interface IDescriptiveInteractable : IInteractable
{
    string GetInteractionVerb();
    string GetObjectName();
    string GetObjectID();
}