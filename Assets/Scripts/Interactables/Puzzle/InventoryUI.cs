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

        ApplyButtonHighlightColors(useButton);
        ApplyButtonHighlightColors(combineButton);
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
            button.onClick.AddListener(() => OnItemSelected(item));
            button.colors = GetButtonColors(false);

            if (firstSelectable == null)
                firstSelectable = buttonObj;
        }

        DeselectItem();

        if (firstSelectable != null)
            EventSystem.current.SetSelectedGameObject(firstSelectable);
    }

    private void OnItemSelected(BaseItemData item)
    {
        selectedItem = item;

        foreach (var buttonGO in buttonInstances)
        {
            var btn = buttonGO.GetComponent<Button>();
            var isThis = btn.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == item.itemName;
            btn.colors = GetButtonColors(isThis);
        }

        GameObject current = EventSystem.current.currentSelectedGameObject;
        if (current != null && current.GetComponent<Button>() != null)
        {
            EventSystem.current.SetSelectedGameObject(current);
        }

        if (combineCandidate != null && combineCandidate != selectedItem)
        {
            if (combineCandidate is PuzzleItemData a && selectedItem is PuzzleItemData b)
                TryPerformCombination(a, b);

            combineCandidate = null;
        }

        UpdateButtonStates();

        Debug.Log($"[InventoryUI] Selected: {item.itemName}");
    }

    private void DeselectItem()
    {
        selectedItem = null;
        combineCandidate = null;

        foreach (var btn in buttonInstances)
        {
            btn.GetComponent<Button>().colors = GetButtonColors(false);
        }

        UpdateButtonStates();
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
                    DeselectItem();

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
                if (wasOpenedFromSlot)
                {
                    Debug.Log("[InventoryUI] Cannot use consumables when inventory was opened from puzzle slot.");
                    return;
                }

                if (selectedItem is ConsumableItemData consumable)
                {
                    consumable.ApplyEffect();
                    InventorySystem.Instance.RemoveItem(consumable);
                    DeselectItem();
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
            DeselectItem();
            RefreshDisplay();
        }
        else
        {
            Debug.Log($"[COMBINE FAILED] {a.itemName} and {b.itemName} cannot be combined.");
        }
    }

    private static ColorBlock GetButtonColors(bool isSelected = false)
    {
        return new ColorBlock
        {
            normalColor = isSelected ? new Color(1f, 0.85f, 0.3f) : new Color(0.9f, 0.9f, 0.9f),
            highlightedColor = new Color(1f, 1f, 1f),
            pressedColor = new Color(0.75f, 0.75f, 0.75f),
            selectedColor = new Color(1f, 0.85f, 0.3f),
            disabledColor = new Color(0.5f, 0.5f, 0.5f),
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };
    }

    private void ApplyButtonHighlightColors(Button button)
    {
        var colors = button.colors;
        colors.normalColor = new Color(0.9f, 0.9f, 0.9f);
        colors.highlightedColor = new Color(1f, 1f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        colors.selectedColor = new Color(1f, 0.85f, 0.3f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f);
        button.colors = colors;
    }

    private void UpdateButtonStates()
    {
        useButton.interactable = selectedItem != null;
        combineButton.interactable = selectedItem is PuzzleItemData p && p.isCombinable;
    }

    public BaseItemData GetSelectedItem() => selectedItem;
}
