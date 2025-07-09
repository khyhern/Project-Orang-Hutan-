using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemCombination
{
    public PuzzleItemData otherItem;
    public PuzzleItemData resultItem;
}

[CreateAssetMenu(menuName = "Puzzle/Item Data")]
public class PuzzleItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    [TextArea] public string description;
    public bool isCombinable;

    [Header("Combinations")]
    public List<ItemCombination> combinableWith;
}
