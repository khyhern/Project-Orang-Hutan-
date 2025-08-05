using System.Collections.Generic;
using UnityEngine;

public class FlashlightSpawn : MonoBehaviour
{
    public static FlashlightSpawn Instance { get; private set; }

    public bool flashlightExists { get; private set; } = false;

    private List<string> keyItems = new List<string>();

    private Transform flashlightSpawnPointA; // Flashlight point (R)
    private Transform flashlightSpawnPointB; // Flashlight point (L)
    private Transform currentSpawnPoint;
    [SerializeField] private float rotateA = -10f;
    [SerializeField] private float rotateB = -4f;

    public GameObject flashlightPrefab;
    private GameObject spawnedFlashlight;
    private bool flashlightSpawned = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Auto-find spawn points by name in the scene
        GameObject pointAObj = GameObject.Find("Flashlight point (R)");
        GameObject pointBObj = GameObject.Find("Flashlight point (L)");

        if (pointAObj != null) flashlightSpawnPointA = pointAObj.transform;
        else Debug.LogError("Flashlight point (R) not found.");

        if (pointBObj != null) flashlightSpawnPointB = pointBObj.transform;
        else Debug.LogError("Flashlight point (L) not found.");

        currentSpawnPoint = flashlightSpawnPointA;
    }

    void Update()
    {
        if (flashlightExists && !flashlightSpawned)
        {
            TrySpawnFlashlight();
        }

        if (flashlightSpawned)
        {
            FaceFlashlightToScreenCenter();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchFlashlightPosition();
            }
        }
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
        flashlightPrefab = prefab;
    }

    public void SetFlashlightExists(bool value)
    {
        flashlightExists = value;
    }

    void TrySpawnFlashlight()
    {
        if (flashlightPrefab == null || currentSpawnPoint == null)
        {
            Debug.LogWarning("Flashlight prefab or spawn point is missing.");
            return;
        }

        if (spawnedFlashlight != null) return;

        spawnedFlashlight = Instantiate(flashlightPrefab, currentSpawnPoint.position, currentSpawnPoint.rotation);
        spawnedFlashlight.transform.SetParent(currentSpawnPoint);
        spawnedFlashlight.transform.localScale = currentSpawnPoint.localScale;

        // Set FPView layer
        spawnedFlashlight.layer = LayerMask.NameToLayer("FPView");
        foreach (Transform child in spawnedFlashlight.transform)
            child.gameObject.layer = LayerMask.NameToLayer("FPView");

        FlashlightController controller = spawnedFlashlight.GetComponent<FlashlightController>();
        if (controller != null) controller.Activate();

        flashlightSpawned = true;
        Debug.Log("Flashlight equipped.");
    }

    void SwitchFlashlightPosition()
    {
        if (spawnedFlashlight == null || flashlightSpawnPointA == null || flashlightSpawnPointB == null)
            return;

        // Toggle spawn point
        currentSpawnPoint = (currentSpawnPoint == flashlightSpawnPointA) ? flashlightSpawnPointB : flashlightSpawnPointA;

        // Re-parent and reposition flashlight
        spawnedFlashlight.transform.SetParent(currentSpawnPoint);
        spawnedFlashlight.transform.localPosition = Vector3.zero;
        spawnedFlashlight.transform.localRotation = Quaternion.identity;
        spawnedFlashlight.transform.localScale = currentSpawnPoint.localScale;

        Debug.Log($"Flashlight switched to {currentSpawnPoint.name}");
    }

    void FaceFlashlightToScreenCenter()
    {
        if (spawnedFlashlight == null || Camera.main == null) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 lookDirection = ray.direction;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        // Slight right offset when at (L) point
        if (currentSpawnPoint == flashlightSpawnPointB)
            targetRotation *= Quaternion.Euler(90f, rotateA, 0f); // More right
        else
            targetRotation *= Quaternion.Euler(90f, rotateB, 0f);  // Default

        spawnedFlashlight.transform.rotation = targetRotation;
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