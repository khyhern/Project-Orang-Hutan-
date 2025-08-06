using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Puzzle Item")]
public class PuzzleItemData : BaseItemData
{
    public bool isCombinable;
    public List<ItemCombination> combinableWith;

    public override ItemType GetItemType() => ItemType.Puzzle;
}
