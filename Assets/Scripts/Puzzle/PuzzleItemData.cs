using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/Item Data")]
public class PuzzleItemData : ScriptableObject
{
    public string itemName;
    public GameObject prefab; // For future instantiation if needed
}
