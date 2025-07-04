using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private List<PuzzleItemData> items = new List<PuzzleItemData>();

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void PickUp(PuzzleItemData item)
    {
        items.Add(item);
        Debug.Log($"Picked up: {item.itemName}");
    }

    public void RemoveItem(PuzzleItemData item)
    {
        items.Remove(item);
    }

    public List<PuzzleItemData> GetAllItems() => items;

    public bool HasItem(PuzzleItemData item) => items.Contains(item);

    public bool HasAnyItems() => items.Count > 0;
}
