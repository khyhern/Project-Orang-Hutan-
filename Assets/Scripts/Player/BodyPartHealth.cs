using UnityEngine;

[System.Serializable]
public class BodyPartHealth
{
    public BodyPart part;
    public int maxHealth = 100;
    public int currentHealth;

    public bool isBleeding { get; private set; }
    public bool IsDestroyed => currentHealth <= 0;

    public BodyPartHealth(BodyPart part, int health)
    {
        this.part = part;
        maxHealth = health;
        currentHealth = health;
        isBleeding = false;
    }

    public void ApplyDamage(int amount)
    {
        if (IsDestroyed || amount <= 0) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartBleeding();
        }
    }

    public void Bandage()
    {
        if (isBleeding)
        {
            isBleeding = false;
            Debug.Log($"{part} has been bandaged. Bleeding stopped.");
        }
    }

    public void Heal(int amount)
    {
        if (IsDestroyed || amount <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isBleeding = false;
    }

    private void StartBleeding()
    {
        isBleeding = true;
        Debug.Log($"{part} has been destroyed and is bleeding.");
    }
}
