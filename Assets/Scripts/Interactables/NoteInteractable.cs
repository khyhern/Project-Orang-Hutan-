using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NoteInteractable : MonoBehaviour, IDescriptiveInteractable
{
    [Header("Note Data")]
    [TextArea(5, 20)]
    [SerializeField] private string noteContent;

    [SerializeField] private string objectName = "Note";
    [SerializeField] private string objectID = "Note001";
    [SerializeField] private string interactionVerb = "read";

    [Header("Settings")]
    [SerializeField] private bool disableAfterReading = false;
    [SerializeField] private InteractionGroup interactionGroup = InteractionGroup.Default;

    private bool hasBeenRead = false;

    public void Interact()
    {
        if (hasBeenRead && disableAfterReading) return;

        NoteUIManager ui = FindObjectOfType<NoteUIManager>();
        if (ui != null)
        {
            ui.ShowNote(noteContent);
        }

        if (disableAfterReading)
        {
            hasBeenRead = true;
        }
    }

    public InteractionGroup GetInteractionGroup() => interactionGroup;

    public string GetInteractionVerb() => interactionVerb;

    public string GetObjectName() => objectName;

    public string GetObjectID() => objectID;
}
