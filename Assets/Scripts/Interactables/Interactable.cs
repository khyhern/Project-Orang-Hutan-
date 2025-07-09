using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact();
    public abstract string GetInteractionVerb();    // Action words. e.g., "pick up", "open"
    public abstract string GetObjectName();         // e.g., "key", "door"
    public abstract string GetObjectID();           // Item name
}
