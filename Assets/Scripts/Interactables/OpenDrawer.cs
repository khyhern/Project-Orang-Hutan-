using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OpenDrawer : MonoBehaviour, IDescriptiveInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource openSound;
    [SerializeField] private AudioSource closeSound;

    private bool isOpen = false;

    public void Interact()
    {
        if (!isOpen)
        {
            // openSound?.Play();
            animator?.SetTrigger("openDrawer");
        }
        else
        {
            // closeSound?.Play();
            animator?.SetTrigger("closeDrawer");
        }

        isOpen = !isOpen;
    }

    public string GetInteractionVerb() => isOpen ? "close" : "open";
    public string GetObjectName() => "drawer";
    public string GetObjectID() => "Drawer";
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
