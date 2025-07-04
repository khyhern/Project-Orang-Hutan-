using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightItem : Interactable
{
    public string itemID = "Flashlight";
    public GameObject flashlightPrefab;

    public override void Interact()
    {
        Debug.Log("Flashlight pickup collected.");
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
        return itemID.ToLower();
    }
}


    
