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

    private BaseItemData selectedItem;
    private BaseItemData combineCandidate;
    private GameObject selectedButton;
    private readonly List<GameObject> buttonInstances = new();

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
        playerMovement = FindObjectOfType<PlayerMovement>();
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
                playerMovement.canMove = true;
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
            playerMovement.canMove = false;
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

    private void OnItemSelected(BaseItemData item, GameObject buttonObj)
    {
        selectedItem = item;

        foreach (var btn in buttonInstances)
            btn.GetComponent<Image>().color = Color.white;

        selectedButton = buttonObj;
        selectedButton.GetComponent<Image>().color = Color.yellow;

        if (combineCandidate != null && combineCandidate != selectedItem)
        {
            if (combineCandidate is PuzzleItemData a && selectedItem is PuzzleItemData b)
                TryPerformCombination(a, b);

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

        switch (selectedItem.GetItemType())
        {
            case ItemType.Puzzle:
                if (PuzzleSlotInteractable.ActiveSlot != null && selectedItem is PuzzleItemData puzzleItem)
                {
                    PuzzleSlotInteractable.ActiveSlot.PlaceItem(puzzleItem);

                    if (wasOpenedFromSlot)
                    {
                        inventoryCanvas.SetActive(false);
                        wasOpenedFromSlot = false;

                        if (playerMovement != null)
                            playerMovement.canMove = true;
                    }
                }
                else
                {
                    Debug.Log("[InventoryUI] No puzzle slot is active to use this item on.");
                }
                break;

            case ItemType.Consumable:
                if (selectedItem is ConsumableItemData consumable)
                {
                    consumable.ApplyEffect();
                    InventorySystem.Instance.RemoveItem(consumable);
                    RefreshDisplay();
                }
                break;


            default:
                Debug.Log("[USE] This item type has no defined use.");
                break;
        }
    }

    private void OnCombine()
    {
        if (selectedItem is PuzzleItemData puzzleItem && puzzleItem.isCombinable)
        {
            combineCandidate = puzzleItem;
            Debug.Log($"[COMBINE] Now select item to combine with: {puzzleItem.itemName}");
        }
        else
        {
            Debug.Log("[COMBINE] No valid item selected.");
        }
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

    public BaseItemData GetSelectedItem() => selectedItem;
}
