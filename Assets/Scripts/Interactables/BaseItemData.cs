using UnityEngine;

public abstract class BaseItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    [TextArea] public string description;

    public abstract ItemType GetItemType();
}

public enum ItemType
{
    Puzzle,
    Consumable,
    Weapon,
    KeyItem,
    // Add more as needed
}
