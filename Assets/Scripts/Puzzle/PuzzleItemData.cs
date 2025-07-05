using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/Item Data")]
public class PuzzleItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    [TextArea] public string description;
    public bool isCombinable;
}
