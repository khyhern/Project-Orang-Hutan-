using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable Item")]
public class ConsumableItemData : BaseItemData
{
    [TextArea] public string useText; // Optional: e.g. "You drank the potion. You feel stronger."

    public override ItemType GetItemType() => ItemType.Consumable;

    // You can expand this to actual behavior like events or delegates if needed
    public virtual void ApplyEffect()
    {
        Debug.Log($"[CONSUMABLE] Used: {itemName} - {useText}");

        // Example: Play VFX, trigger event, change state, etc.
        // For now it's just a log — you can hook into gameplay systems later
    }
}
