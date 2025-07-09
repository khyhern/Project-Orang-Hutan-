using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDrawer : Interactable
{
    public Animator animator;
    public AudioSource openSound;
    public AudioSource closeSound;

    private bool isOpen = false;

    public override void Interact()
    {
        if (!isOpen)
        {
            if (openSound != null) openSound.Play();
            animator.SetTrigger("openDrawer");
            isOpen = true;
        }
        else
        {
            if (closeSound != null) closeSound.Play();
            animator.SetTrigger("closeDrawer");
            isOpen = false;
        }
    }

    public override string GetInteractionVerb()
    {
        return isOpen ? "close" : "open";
    }

    public override string GetObjectName()
    {
        return "Drawer";
    }

    public override string GetObjectID()
    {
        return "Drawer";
    }
}
