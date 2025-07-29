using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BatteryItem : MonoBehaviour, IDescriptiveInteractable
{
    [SerializeField] private string itemID = "Battery";
    [SerializeField] private int batteryAmount = 40;

    public void Interact()
    {
        FlashlightController flashlight = FindObjectOfType<FlashlightController>();

        if (flashlight != null)
        {
            flashlight.AddBattery(batteryAmount);
            Debug.Log($"[Battery] Added {batteryAmount} to flashlight.");
        }
        else
        {
            Debug.LogWarning("[Battery] No active flashlight found.");
        }

        Destroy(gameObject); // Battery is consumed
    }

    public string GetInteractionVerb() => "use";
    public string GetObjectID() => itemID;
    public string GetObjectName() => itemID.ToLower();
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
