using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private readonly List<BaseItemData> items = new(); // 🔄 Now stores all item types
    private const int MaxInventorySize = 4;

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

    public bool PickUp(BaseItemData item)
    {
        if (item == null)
            return false;

        /*if (items.Count >= MaxInventorySize)
        {
            Debug.LogWarning("[Inventory] Cannot pick up item: Inventory is full.");
            return false;
        }*/

        if (item.GetItemType() == ItemType.Consumable)
        {
            int consumableCount = 0;
            foreach (var i in items)
            {
                if (i.GetItemType() == ItemType.Consumable)
                    consumableCount++;
            }

            if (consumableCount >= MaxInventorySize)
            {
                Debug.LogWarning("[Inventory] Cannot pick up consumable: Limit reached.");
                return false;
            }
        }

        items.Add(item);
        Debug.Log($"[Inventory] Picked up: {item.itemName}");
        OnInventoryChanged?.Invoke();
        return true;
    }


    public void RemoveItem(BaseItemData item)
    {
        if (items.Remove(item))
        {
            Debug.Log($"[Inventory] Removed: {item.itemName}");
            OnInventoryChanged?.Invoke();
        }
    }

    public IReadOnlyList<BaseItemData> GetAllItems() => items;

    public bool HasItem(BaseItemData item) => items.Contains(item);

    public bool TryCombineItems(BaseItemData a, BaseItemData b, out BaseItemData result)
    {
        result = null;

        // ✅ Only allow combination if both are PuzzleItemData
        if (a is PuzzleItemData puzzleA && b is PuzzleItemData puzzleB)
        {
            if (!puzzleA.isCombinable || !puzzleB.isCombinable)
                return false;

            foreach (var combo in puzzleA.combinableWith)
            {
                if (combo.otherItem == puzzleB)
                {
                    result = combo.resultItem;

                    RemoveItem(puzzleA);
                    RemoveItem(puzzleB);
                    PickUp(result);

                    Debug.Log($"[Inventory] Combined {puzzleA.itemName} + {puzzleB.itemName} -> {result.itemName}");
                    return true;
                }
            }
        }

        return false;
    }

    public bool HasItemByName(string itemName)
    {
        foreach (var item in items)
        {
            if (item.itemName.Trim().Equals(itemName.Trim(), System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public BaseItemData GetItemByName(string itemName)
    {
        foreach (var item in items)
        {
            if (item.itemName.Trim().Equals(itemName.Trim(), System.StringComparison.OrdinalIgnoreCase))
                return item;
        }
        return null;
    }
}

/*
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private readonly List<BaseItemData> items = new(); // 🔄 Now stores all item types
    private const int MaxInventorySize = 4;

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

    public bool PickUp(BaseItemData item)
    {
        if (item == null)
            return false;

        if (items.Count >= MaxInventorySize)
        {
            Debug.LogWarning("[Inventory] Cannot pick up item: Inventory is full.");
            return false;
        }

        items.Add(item);
        Debug.Log($"[Inventory] Picked up: {item.itemName}");
        OnInventoryChanged?.Invoke();
        return true;
    }


    public void RemoveItem(BaseItemData item)
    {
        if (items.Remove(item))
        {
            Debug.Log($"[Inventory] Removed: {item.itemName}");
            OnInventoryChanged?.Invoke();
        }
    }

    public IReadOnlyList<BaseItemData> GetAllItems() => items;

    public bool HasItem(BaseItemData item) => items.Contains(item);

    public bool TryCombineItems(BaseItemData a, BaseItemData b, out BaseItemData result)
    {
        result = null;

        // ✅ Only allow combination if both are PuzzleItemData
        if (a is PuzzleItemData puzzleA && b is PuzzleItemData puzzleB)
        {
            if (!puzzleA.isCombinable || !puzzleB.isCombinable)
                return false;

            foreach (var combo in puzzleA.combinableWith)
            {
                if (combo.otherItem == puzzleB)
                {
                    result = combo.resultItem;

                    RemoveItem(puzzleA);
                    RemoveItem(puzzleB);
                    PickUp(result);

                    Debug.Log($"[Inventory] Combined {puzzleA.itemName} + {puzzleB.itemName} -> {result.itemName}");
                    return true;
                }
            }
        }

        return false;
    }
}
*/