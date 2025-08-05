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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentHealth = maxHealth;

        if (bodyParts == null || bodyParts.Count == 0)
            InitializeBodyParts();

        // Auto-link UI Icons for each body part
        foreach (var part in bodyParts)
            part.InitializeUIReference();
    }

    private void Update()
    {
        foreach (var part in bodyParts)
        {
            part.ForceUpdateUI();
        }
    }

    private void InitializeBodyParts()
    {
        bodyParts.Clear();
        foreach (BodyPart part in System.Enum.GetValues(typeof(BodyPart)))
        {
            bodyParts.Add(new BodyPartHealth(part, 100));
        }
    }

    public void DamagePlayer(int amount)
    {
        if (IsDead()) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        //Debug.Log($"Player took {amount} global damage. Current HP: {currentHealth}");

        if (IsDead())
        {
            Debug.Log("Player has died.");
            // TODO: trigger death logic
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

    public int CountBrokenLegs()
    {
        int count = 0;
        if (IsPartDestroyed(BodyPart.LeftLeg)) count++;
        if (IsPartDestroyed(BodyPart.RightLeg)) count++;
        return count;
    }

    public float GetMovementSpeedMultiplier()
    {
        int brokenLegs = CountBrokenLegs();

        if (brokenLegs == 2)
            return 0.8f; // 80% speed if both legs broken

        return 1f; // full speed if 0–1 leg broken
    }

    public float InstanceHealthPercent()
    {
        return maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
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
