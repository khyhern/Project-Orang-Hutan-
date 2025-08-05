using System.Collections.Generic;
using UnityEngine;

public class FlashlightSpawn : MonoBehaviour
{
    public static FlashlightSpawn Instance { get; private set; }

    public bool flashlightExists { get; private set; } = false;

    private List<string> keyItems = new List<string>();

    private Transform flashlightSpawnPointA; // Flashlight point (R)
    private Transform flashlightSpawnPointB; // Flashlight point (L)
    private Transform flashlightSpawnPointFallback; // Flashlight point (Fallback)
    private Transform currentSpawnPoint;

    [SerializeField] private float rotateA = -10f;
    [SerializeField] private float rotateB = -4f;

    public GameObject flashlightPrefab;
    private GameObject spawnedFlashlight;
    private bool flashlightSpawned = false;

    private bool manualSwitchAllowed = true; // Lock switching when needed

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GameObject pointAObj = GameObject.Find("Flashlight point (R)");
        GameObject pointBObj = GameObject.Find("Flashlight point (L)");
        GameObject pointFallbackObj = GameObject.Find("Flashlight point (N)");

        if (pointAObj != null) flashlightSpawnPointA = pointAObj.transform;
        else Debug.LogError("Flashlight point (R) not found.");

        if (pointBObj != null) flashlightSpawnPointB = pointBObj.transform;
        else Debug.LogError("Flashlight point (L) not found.");

        if (pointFallbackObj != null) flashlightSpawnPointFallback = pointFallbackObj.transform;
        else Debug.LogError("Flashlight point (N) not found.");

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
            CheckHandDamageState();

            if (manualSwitchAllowed && Input.GetKeyDown(KeyCode.Tab))
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

        currentSpawnPoint = (currentSpawnPoint == flashlightSpawnPointA) ? flashlightSpawnPointB : flashlightSpawnPointA;

        ReattachFlashlight();
        Debug.Log($"Flashlight switched to {currentSpawnPoint.name}");
    }

    void ReattachFlashlight()
    {
        spawnedFlashlight.transform.SetParent(currentSpawnPoint);
        spawnedFlashlight.transform.localPosition = Vector3.zero;
        spawnedFlashlight.transform.localRotation = Quaternion.identity;
        spawnedFlashlight.transform.localScale = Vector3.one; // Force normal size
    }

    void FaceFlashlightToScreenCenter()
    {
        if (spawnedFlashlight == null || Camera.main == null) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 lookDirection = ray.direction;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        if (currentSpawnPoint == flashlightSpawnPointB)
            targetRotation *= Quaternion.Euler(90f, rotateA, 0f);
        else if (currentSpawnPoint == flashlightSpawnPointA)
            targetRotation *= Quaternion.Euler(90f, rotateB, 0f);
        else
            targetRotation *= Quaternion.Euler(90f, 0f, 0f); // Fallback is centered

        spawnedFlashlight.transform.rotation = targetRotation;
    }

    void CheckHandDamageState()
    {
        var health = PlayerHealth.Instance;
        if (health == null) return;

        bool rightBroken = health.IsPartDestroyed(BodyPart.RightArm);
        bool leftBroken = health.IsPartDestroyed(BodyPart.LeftArm);

        if (rightBroken && leftBroken)
        {
            if (currentSpawnPoint != flashlightSpawnPointFallback)
            {
                currentSpawnPoint = flashlightSpawnPointFallback;
                ReattachFlashlight();
                Debug.Log("Both arms are broken. Flashlight moved to fallback position.");
            }
            manualSwitchAllowed = false;
        }
        else if (rightBroken)
        {
            if (currentSpawnPoint != flashlightSpawnPointB)
            {
                currentSpawnPoint = flashlightSpawnPointB;
                ReattachFlashlight();
                Debug.Log("Right arm broken. Flashlight switched to left hand.");
            }
            manualSwitchAllowed = false;
        }
        else if (leftBroken)
        {
            if (currentSpawnPoint != flashlightSpawnPointA)
            {
                currentSpawnPoint = flashlightSpawnPointA;
                ReattachFlashlight();
                Debug.Log("Left arm broken. Flashlight switched to right hand.");
            }
            manualSwitchAllowed = false;
        }
        else
        {
            manualSwitchAllowed = true;
        }
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