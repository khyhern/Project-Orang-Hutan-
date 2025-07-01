using UnityEngine;

[System.Serializable]
public class BodyPartHealth
{
    public BodyPart part;
    public int maxHealth = 100;
    public int currentHealth;

    public bool IsDestroyed => currentHealth <= 0;

    public BodyPartHealth(BodyPart part, int health)
    {
        this.part = part;
        this.maxHealth = health;
        this.currentHealth = health;
    }

    public void ApplyDamage(int amount)
    {
        if (IsDestroyed) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log($"{part} has been destroyed.");
            // Trigger effects here (e.g., disable ability)
        }
    }

    public void Heal(int amount)
    {
        if (IsDestroyed) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
