using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Universal Player Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Per-Body-Part Health")]
    [SerializeField] private List<BodyPartHealth> bodyParts = new();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentHealth = maxHealth;

        if (bodyParts == null || bodyParts.Count == 0)
            InitializeBodyParts();
    }

    private void InitializeBodyParts()
    {
        bodyParts.Clear();
        foreach (BodyPart part in System.Enum.GetValues(typeof(BodyPart)))
        {
            bodyParts.Add(new BodyPartHealth(part, 100));
        }
    }

    public void DamagePart(BodyPart part, int amount)
    {
        GetBodyPart(part)?.ApplyDamage(amount);
    }

    public void BandageLimb(BodyPart part)
    {
        GetBodyPart(part)?.Bandage();
    }

    public void DamagePlayer(int amount)
    {
        if (IsDead()) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        Debug.Log($"Player took {amount} global damage. Current HP: {currentHealth}");

        if (IsDead())
        {
            Debug.Log("Player has died.");
            // TODO: trigger death logic
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0 || IsPartDestroyed(BodyPart.Head);
    }

    public bool IsPartDestroyed(BodyPart part)
    {
        return GetBodyPart(part)?.IsDestroyed ?? false;
    }

    public BodyPartHealth GetBodyPart(BodyPart part)
    {
        return bodyParts.Find(p => p.part == part);
    }

    public float GetMovementSpeedMultiplier()
    {
        int brokenLegs = 0;
        int brokenArms = 0;

        if (IsPartDestroyed(BodyPart.LeftLeg)) brokenLegs++;
        if (IsPartDestroyed(BodyPart.RightLeg)) brokenLegs++;
        if (IsPartDestroyed(BodyPart.LeftArm)) brokenArms++;
        if (IsPartDestroyed(BodyPart.RightArm)) brokenArms++;

        if (brokenLegs == 2 && brokenArms == 2) return 0.1f;
        if (brokenLegs == 2 && brokenArms >= 1) return 0.25f;
        if (brokenLegs == 2) return 0.4f;
        if (brokenLegs == 1) return 0.7f;

        return 1f;
    }

    public int CountActiveBleedingLimbs()
    {
        int count = 0;
        foreach (var part in bodyParts)
        {
            if (part.part == BodyPart.Head) continue;
            if (part.IsDestroyed && part.isBleeding) count++;
        }
        return count;
    }
}
