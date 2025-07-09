using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    public string requiredKeyID = "Rusty Key";

    public string objectID = "Suspicious door";

    private bool isOpened = false;

    public override void Interact()
    {
        if (isOpened) return;

        if (InventoryManager.Instance.HasKeyItem(requiredKeyID))
        {
            Debug.Log("Door opened.");
            // TODO: Add door animation or open logic here

            // Removes key (One-time use item)
            //InventoryManager.Instance.RemoveKeyItem(requiredKeyID);

            isOpened = true;
            this.enabled = false; // Disable this script so it can't be interacted with again
        }
        else
        {
            Debug.Log("Door is locked. You need a key.");
        }
    }

    public override string GetInteractionVerb()
    {
        return "open";
    }

    public override string GetObjectID()
    {
        return objectID;
    }

    public override string GetObjectName()
    {
        return "door";
    }
}


/*
public class DoorInteractable : Interactable
{
    public override void Interact()
    {
        Debug.Log("Door interacted.");
        // TODO: Open door logic
    }

    public override string GetInteractionVerb()
    {
        return "open";
    }

    public override string GetObjectName()
    {
        return "door";
    }
}
*/