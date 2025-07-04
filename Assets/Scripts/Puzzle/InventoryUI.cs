using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryCanvas;
    public Transform gridContainer;
    public GameObject itemButtonPrefab;
    public Button useButton;
    public Button combineButton;

    private PuzzleItemData selectedItem;
    private GameObject selectedButton;
    private readonly List<GameObject> buttonInstances = new();

    public static InventoryUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        useButton.onClick.AddListener(OnUse);
        combineButton.onClick.AddListener(OnCombine);
    }

    private void OnEnable()
    {
        TryRegisterToInventory();
    }

    private void Start()
    {
        // Ensures it catches InventorySystem if it loaded late
        StartCoroutine(RegisterWhenReady());
    }

    private IEnumerator RegisterWhenReady()
    {
        while (InventorySystem.Instance == null)
            yield return null;

        TryRegisterToInventory();
    }

    private void TryRegisterToInventory()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged -= RefreshDisplay;
            InventorySystem.Instance.OnInventoryChanged += RefreshDisplay;
        }
    }

    private void OnDisable()
    {
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.OnInventoryChanged -= RefreshDisplay;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        bool isActive = !inventoryCanvas.activeSelf;
        inventoryCanvas.SetActive(isActive);

        if (isActive)
        {
            RefreshDisplay();
        }
    }

    public void RefreshDisplay()
    {
        foreach (var go in buttonInstances)
            Destroy(go);
        buttonInstances.Clear();

        var items = InventorySystem.Instance.GetAllItems();
        GameObject firstSelectable = null;

        foreach (var item in items)
        {
            GameObject buttonObj = Instantiate(itemButtonPrefab, gridContainer);
            buttonInstances.Add(buttonObj);

            var label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            label.text = item.itemName;

            var button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnItemSelected(item, buttonObj));

            if (firstSelectable == null)
                firstSelectable = buttonObj;
        }

        DeselectItem();

        if (firstSelectable != null)
            EventSystem.current.SetSelectedGameObject(firstSelectable);
    }

    private void OnItemSelected(PuzzleItemData item, GameObject buttonObj)
    {
        selectedItem = item;

        foreach (var btn in buttonInstances)
            btn.GetComponent<Image>().color = Color.white;

        selectedButton = buttonObj;
        selectedButton.GetComponent<Image>().color = Color.yellow;

        Debug.Log($"[InventoryUI] Selected: {item.itemName}");
    }

    private void DeselectItem()
    {
        selectedItem = null;

        foreach (var btn in buttonInstances)
            btn.GetComponent<Image>().color = Color.white;
    }

    private void OnUse()
    {
        if (selectedItem == null) return;
        Debug.Log($"[USE] {selectedItem.itemName}");
        // TODO: Implement actual use logic
    }

    private void OnCombine()
    {
        if (selectedItem == null || !selectedItem.isCombinable)
        {
            Debug.Log("[COMBINE] No valid item selected.");
            return;
        }

        Debug.Log($"[COMBINE] {selectedItem.itemName}");
        // TODO: Implement actual combine logic
    }

    public PuzzleItemData GetSelectedItem() => selectedItem;
}
