using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private readonly List<PuzzleItemData> items = new();

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PickUp(PuzzleItemData item)
    {
        if (item == null) return;

        items.Add(item);
        Debug.Log($"[Inventory] Picked up: {item.itemName}");
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(PuzzleItemData item)
    {
        if (items.Remove(item))
        {
            Debug.Log($"[Inventory] Removed: {item.itemName}");
            OnInventoryChanged?.Invoke();
        }
    }

    public IReadOnlyList<PuzzleItemData> GetAllItems() => items;

    public bool HasItem(PuzzleItemData item) => items.Contains(item);

    public bool TryCombineItems(PuzzleItemData a, PuzzleItemData b, out PuzzleItemData result)
    {
        result = null;

        if (a == null || b == null || !a.isCombinable || !b.isCombinable)
            return false;

        foreach (var combo in a.combinableWith)
        {
            if (combo.otherItem == b)
            {
                result = combo.resultItem;

                RemoveItem(a);
                RemoveItem(b);
                PickUp(result);

                Debug.Log($"[Inventory] Combined {a.itemName} + {b.itemName} -> {result.itemName}");
                return true;
            }
        }

        return false;
    }
}
