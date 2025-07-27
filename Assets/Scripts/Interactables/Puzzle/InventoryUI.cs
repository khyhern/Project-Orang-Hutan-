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

    public static InventoryUI Instance { get; private set; }

    private PuzzleItemData selectedItem;
    private GameObject selectedButton;
    private readonly List<GameObject> buttonInstances = new();

    private PuzzleItemData combineCandidate = null;
    private bool wasOpenedFromSlot = false;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        useButton.onClick.AddListener(OnUse);
        combineButton.onClick.AddListener(OnCombine);
    }

    private void OnEnable() => TryRegisterToInventory();
    private void OnDisable() => UnregisterFromInventory();

    private void Start()
    {
        StartCoroutine(RegisterWhenReady());
        playerMovement = FindObjectOfType<PlayerMovement>(); // Assign PlayerMovement reference
    }

    private System.Collections.IEnumerator RegisterWhenReady()
    {
        while (InventorySystem.Instance == null)
            yield return null;

        TryRegisterToInventory();
    }

    private void TryRegisterToInventory()
    {
        if (InventorySystem.Instance == null) return;

        InventorySystem.Instance.OnInventoryChanged -= RefreshDisplay;
        InventorySystem.Instance.OnInventoryChanged += RefreshDisplay;
    }

    private void UnregisterFromInventory()
    {
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.OnInventoryChanged -= RefreshDisplay;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
    }

    private void ToggleInventory()
    {
        if (inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(false);
            if (playerMovement != null)
                playerMovement.canMove = true; // Re-enable movement
        }
        else
        {
            OpenInventory(false);
        }
    }

    public void OpenInventory(bool fromSlot = false)
    {
        wasOpenedFromSlot = fromSlot;
        inventoryCanvas.SetActive(true);
        RefreshDisplay();

        if (playerMovement != null)
            playerMovement.canMove = false; // Disable movement
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

            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;

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

        if (combineCandidate != null && combineCandidate != selectedItem)
        {
            TryPerformCombination(combineCandidate, selectedItem);
            combineCandidate = null;
        }

        Debug.Log($"[InventoryUI] Selected: {item.itemName}");
    }

    private void DeselectItem()
    {
        selectedItem = null;
        combineCandidate = null;

        foreach (var btn in buttonInstances)
            btn.GetComponent<Image>().color = Color.white;
    }

    private void OnUse()
    {
        if (selectedItem == null)
        {
            Debug.Log("[InventoryUI] No item selected to use.");
            return;
        }

        if (PuzzleSlotInteractable.ActiveSlot != null)
        {
            PuzzleSlotInteractable.ActiveSlot.PlaceItem(selectedItem);

            if (wasOpenedFromSlot)
            {
                inventoryCanvas.SetActive(false);
                wasOpenedFromSlot = false;

                if (playerMovement != null)
                    playerMovement.canMove = true; // Re-enable movement after using from slot
            }
        }
        else
        {
            Debug.Log("[InventoryUI] No puzzle slot is active to use this item on.");
        }
    }

    private void OnCombine()
    {
        if (selectedItem == null || !selectedItem.isCombinable)
        {
            Debug.Log("[COMBINE] No valid item selected.");
            return;
        }

        combineCandidate = selectedItem;
        Debug.Log($"[COMBINE] Now select item to combine with: {combineCandidate.itemName}");
    }

    private void TryPerformCombination(PuzzleItemData a, PuzzleItemData b)
    {
        if (InventorySystem.Instance.TryCombineItems(a, b, out var result))
        {
            Debug.Log($"[COMBINE SUCCESS] {a.itemName} + {b.itemName} = {result.itemName}");
            RefreshDisplay();
        }
        else
        {
            Debug.Log($"[COMBINE FAILED] {a.itemName} and {b.itemName} cannot be combined.");
        }
    }

    public PuzzleItemData GetSelectedItem() => selectedItem;
}
