using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public List<BodyPartHealth> bodyParts = new();

    private void Awake()
    {
        InitializeBodyParts();
    }

    private void InitializeBodyParts()
    {
        bodyParts.Clear();
        foreach (BodyPart part in System.Enum.GetValues(typeof(BodyPart)))
        {
            bodyParts.Add(new BodyPartHealth(part, 100)); // Default 100 HP
        }
    }

    public void DamagePart(BodyPart part, int amount)
    {
        BodyPartHealth partHealth = GetBodyPart(part);
        if (partHealth != null)
        {
            partHealth.ApplyDamage(amount);
        }
    }

    public bool IsPartDestroyed(BodyPart part)
    {
        var partHealth = GetBodyPart(part);
        return partHealth != null && partHealth.IsDestroyed;
    }

    public BodyPartHealth GetBodyPart(BodyPart part)
    {
        return bodyParts.Find(p => p.part == part);
    }

    public bool IsPlayerDead()
    {
        // Customize death condition
        return IsPartDestroyed(BodyPart.Head) || IsPartDestroyed(BodyPart.Torso);
    }
}
