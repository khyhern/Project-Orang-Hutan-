using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Interactable
{
    public string keyID = "Rusty Key"; // Unique name or ID

    public override void Interact()
    {
        InventoryManager.Instance.AddKeyItem(keyID);
        Destroy(gameObject);
    }

    public override string GetInteractionVerb()
    {
        return "pick up";
    }

    public override string GetObjectID()
    {
        return keyID;
    }

    public override string GetObjectName()
    {
        return keyID.ToLower();     // convert string ID into all lowercase
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