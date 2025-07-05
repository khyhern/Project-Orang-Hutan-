using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private List<string> keyItems = new List<string>();

    [Header("Flashlight")]
    private string flashlightID;
    public GameObject flashlightPrefab;            
    public Transform flashlightSpawnPoint;         
    private GameObject spawnedFlashlight;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddKeyItem(string itemName)
    {
        if (!keyItems.Contains(itemName))
            keyItems.Add(itemName);
    }

    public bool HasKeyItem(string itemName) => keyItems.Contains(itemName);
    public void RemoveKeyItem(string itemName) => keyItems.Remove(itemName);
    public List<string> GetAllKeyItems() => new List<string>(keyItems);

    public void RegisterFlashlight(string id, GameObject prefab)
    {
        flashlightID = id;
        flashlightPrefab = prefab;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TrySpawnFlashlight();
        }
    }

    void TrySpawnFlashlight()
    {
        if (flashlightPrefab == null || flashlightSpawnPoint == null)
        {
            Debug.LogWarning("Flashlight prefab or spawn point is missing.");
            return;
        }

        if (spawnedFlashlight != null) return;

        spawnedFlashlight = Instantiate(flashlightPrefab, flashlightSpawnPoint.position, flashlightSpawnPoint.rotation);
        spawnedFlashlight.transform.SetParent(flashlightSpawnPoint);

        FlashlightController controller = spawnedFlashlight.GetComponent<FlashlightController>();
        if (controller != null)
        {
            controller.Activate();
        }

        Debug.Log("Flashlight spawned.");
    }
}


/*
 * public class InventoryManager : MonoBehaviour
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
*/