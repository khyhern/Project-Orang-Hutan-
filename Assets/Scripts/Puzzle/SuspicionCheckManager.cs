using System.Collections;
using UnityEngine;

public class SuspicionCheckManager : MonoBehaviour
{
    public static SuspicionCheckManager Instance { get; private set; }

    [Header("Suspicion Settings")]
    [Tooltip("Seconds the player has to reset the items after solving the puzzle.")]
    [SerializeField] private float resetTimeLimit = 30f;

    [Tooltip("All puzzle slots involved in the check.")]
    [SerializeField] private PuzzleSlotInteractable[] slotsToCheck;

    [Tooltip("Called if reset is successful.")]
    [SerializeField] private GameObject successEvent;

    [Tooltip("Called if reset failed (e.g., enemy becomes suspicious).")]
    [SerializeField] private GameObject failureEvent;

    private Coroutine checkRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Starts or restarts the suspicion countdown timer.
    /// </summary>
    public void BeginInspectionCountdown()
    {
        if (checkRoutine != null)
            StopCoroutine(checkRoutine);

        checkRoutine = StartCoroutine(InspectionCountdown());
    }

    /// <summary>
    /// Delays and then runs suspicion check.
    /// </summary>
    private IEnumerator InspectionCountdown()
    {
        Debug.Log("[SuspicionCheck] Countdown started.");
        yield return new WaitForSeconds(resetTimeLimit);
        CheckSlotStates();
    }

    /// <summary>
    /// Checks whether all slots have been reset to their original state.
    /// </summary>
    private void CheckSlotStates()
    {
        foreach (var slot in slotsToCheck)
        {
            PuzzleItemData placed = slot.GetPlacedItem();
            PuzzleItemData original = slot.GetOriginalItem();

            Debug.Log($"[SuspicionCheck] Checking slot: {slot.name}");
            Debug.Log($"  - Placed item: {(placed ? placed.itemName : "None")} ({placed?.GetInstanceID()})");
            Debug.Log($"  - Original item: {(original ? original.itemName : "None")} ({original?.GetInstanceID()})");

            if (placed == null || original == null)
            {
                Debug.Log("[SuspicionCheck] ❌ Null item detected.");
                TriggerFailure();
                return;
            }

            if (placed != original)
            {
                Debug.LogWarning("[SuspicionCheck] ⚠️ Reference mismatch. Falling back to name comparison...");

                if (placed.itemName != original.itemName)
                {
                    Debug.Log("[SuspicionCheck] ❌ Name mismatch detected.");
                    TriggerFailure();
                    return;
                }

                Debug.Log("[SuspicionCheck] ✅ Names match — assuming correct.");
            }
            else
            {
                Debug.Log("[SuspicionCheck] ✅ Reference match confirmed.");
            }
        }

        TriggerSuccess();
    }

    private void TriggerSuccess()
    {
        Debug.Log("✅ [SuspicionCheck] All items restored to original positions. No suspicion.");
        successEvent?.SetActive(true);
    }

    private void TriggerFailure()
    {
        Debug.Log("❌ [SuspicionCheck] Items were NOT returned correctly. Curator becomes suspicious!");
        failureEvent?.SetActive(true);
    }
}
