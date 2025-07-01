using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private List<string> keyItems = new List<string>(); // could be item IDs or names

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional if you want it to persist
    }

    public void AddKeyItem(string itemName)
    {
        if (!keyItems.Contains(itemName))
        {
            keyItems.Add(itemName);
            Debug.Log($"Key item '{itemName}' added to inventory.");
        }
    }

    public bool HasKeyItem(string itemName)
    {
        return keyItems.Contains(itemName);
    }

    public void RemoveKeyItem(string itemName)
    {
        keyItems.Remove(itemName);
    }

    public List<string> GetAllKeyItems()
    {
        return new List<string>(keyItems);
    }
}
