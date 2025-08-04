using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable Item")]
public class ConsumableItemData : BaseItemData
{
    public ConsumableEffectType effectType;
    public int effectAmount = 10;

    public override ItemType GetItemType() => ItemType.Consumable;

    public void ApplyEffect()
    {
        switch (effectType)
        {
            case ConsumableEffectType.Heal:
                //PlayerStats.Instance?.RestoreHealth(effectAmount);
                break;

            case ConsumableEffectType.Stamina:
                PlayerMovement player = GameObject.FindObjectOfType<PlayerMovement>();
                if (player != null)
                    player.RestoreStamina(effectAmount);
                else
                    Debug.LogWarning("[Stamina] PlayerMovement not found.");
                break;


            case ConsumableEffectType.Buff:
                Debug.Log($"[Buff] Boost applied. (amount: {effectAmount})");
                break;

            case ConsumableEffectType.Custom:
                Debug.Log("[Custom] You can hook into an event or animation here.");
                break;

            case ConsumableEffectType.Battery:
                FlashlightController flashlight = GameObject.FindObjectOfType<FlashlightController>();
                if (flashlight != null)
                {
                    flashlight.AddBattery(effectAmount);
                    Debug.Log($"[Battery] Added {effectAmount} battery to flashlight.");
                }
                else
                {
                    Debug.LogWarning("[Battery] No flashlight found in scene.");
                }
                break;
        }
    }
}
