using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BodyPartHealth
{
    public BodyPart part;
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Settings")]
    public Image UIIcon;
    public Color healthyColor = Color.white;
    public Color injuredColor = Color.yellow;
    public Color criticalColor = Color.red;

    [Header("Destroyed State Settings")]
    public bool disableUIOnDestroyed = false;  
    public Color destroyedColor = Color.gray;

    public bool isBleeding { get; private set; }
    public bool IsDestroyed => currentHealth <= 0;

    public BodyPartHealth(BodyPart part, int health)
    {
        this.part = part;
        maxHealth = health;
        currentHealth = health;
        isBleeding = false;
    }

    public void ForceUpdateUI()
    {
        UpdateUI();
    }

    public void InitializeUIReference()
    {
        if (UIIcon != null) return; // Already assigned manually? Skip.

        string uiObjectName = $"UI_{part}";
        GameObject uiObj = GameObject.Find(uiObjectName);

        if (uiObj != null)
        {
            UIIcon = uiObj.GetComponent<Image>();
            if (UIIcon == null)
                Debug.LogWarning($"[BodyPartHealth] No Image component found on {uiObjectName}.");
        }
        else
        {
            Debug.LogWarning($"[BodyPartHealth] UI GameObject '{uiObjectName}' not found.");
        }
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

        UpdateUI();
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
        UpdateUI();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isBleeding = false;
        UpdateUI();
    }

    private void StartBleeding()
    {
        isBleeding = true;
        Debug.Log($"{part} has been destroyed and is bleeding.");
    }

    private void UpdateUI()
    {
        if (UIIcon == null) return;

        if (IsDestroyed)
        {
            if (disableUIOnDestroyed)
            {
                UIIcon.gameObject.SetActive(false);
            }
            else
            {
                UIIcon.gameObject.SetActive(true);
                UIIcon.color = destroyedColor;
            }
            return;
        }

        UIIcon.gameObject.SetActive(true);

        float healthPercent = (float)currentHealth / maxHealth;

        if (healthPercent > 0.5f)
            UIIcon.color = healthyColor;
        else if (healthPercent > 0.2f)
            UIIcon.color = injuredColor;
        else
            UIIcon.color = criticalColor;
    }
}
