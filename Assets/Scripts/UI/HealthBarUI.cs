using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image fillImage;

    [Header("Non-linear Curve")]
    [Tooltip("Lower = fast drop early, slower later")]
    [SerializeField] private float visualExponent = 0.5f;

    private static readonly Color Green = new Color(0f, 1f, 0f);
    private static readonly Color Yellow = new Color(1f, 1f, 0f);
    private static readonly Color Red = new Color(1f, 0f, 0f);

    private void Update()
    {
        if (PlayerHealth.Instance == null || fillImage == null) return;

        float normalizedHealth = PlayerHealth.Instance.InstanceHealthPercent();
        float curvedFill = 1f - Mathf.Pow(1f - normalizedHealth, visualExponent);
        fillImage.fillAmount = curvedFill;

        fillImage.color = GetColorForHealth(normalizedHealth);
    }

    private Color GetColorForHealth(float health)
    {
        if (health > 0.6f)
        {
            // Green → Yellow
            float t = Mathf.InverseLerp(1f, 0.6f, health);
            return Color.Lerp(Green, Yellow, t);
        }
        else
        {
            // Yellow → Red
            float t = Mathf.InverseLerp(0.6f, 0f, health);
            return Color.Lerp(Yellow, Red, t);
        }
    }
}
