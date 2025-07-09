using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Interactable
{
    public string itemID = "Rusty Key"; // Unique name or ID

    public override void Interact()
    {
        InventoryManager.Instance.AddKeyItem(itemID);
        Destroy(gameObject);
    }

    public override string GetInteractionVerb()
    {
        return "pick up";
    }

    public override string GetObjectID()
    {
        return itemID;
    }

    public override string GetObjectName()
    {
        return itemID.ToLower();     // convert string ID into all lowercase
    }
}

/*
 * public class KeyItem : Interactable
{
    public override void Interact()
    {
        Debug.Log("Key collected!");
        Destroy(gameObject);
    }

    public override string GetInteractionVerb()
    {
        return "pick up";
    }

    public override string GetObjectName()
    {
        return "key";
    }
}
*/