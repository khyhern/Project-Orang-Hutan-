using System.Collections;
using UnityEngine;

public class SuspicionCheckManager : MonoBehaviour
{
    public static SuspicionCheckManager Instance { get; private set; }

    [Header("Suspicion Settings")]
    [Tooltip("Seconds the player has to reset the items after solving the puzzle.")]
    public float resetTimeLimit = 30f;

    [Tooltip("All puzzle slots involved in the check.")]
    public PuzzleSlotInteractable[] slotsToCheck;

    [Tooltip("Called if reset is successful.")]
    public GameObject successEvent;

    [Tooltip("Called if reset failed (e.g., enemy becomes suspicious).")]
    public GameObject failureEvent;

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

    public void BeginInspectionCountdown()
    {
        if (checkRoutine != null)
            StopCoroutine(checkRoutine);

        checkRoutine = StartCoroutine(InspectionCountdown());
    }

    private IEnumerator InspectionCountdown()
    {
        Debug.Log("[SuspicionCheck] Countdown started.");
        yield return new WaitForSeconds(resetTimeLimit);
        CheckSlotStates();
    }

    private void CheckSlotStates()
    {
        foreach (var slot in slotsToCheck)
        {
            var placed = slot.GetPlacedItem();
            var original = slot.GetOriginalItem();

            Debug.Log($"[SuspicionCheck] Checking slot: {slot.name}");
            Debug.Log($"[SuspicionCheck] Placed item: {(placed != null ? placed.itemName : "None")} ({placed?.GetInstanceID()})");
            Debug.Log($"[SuspicionCheck] Original item: {(original != null ? original.itemName : "None")} ({original?.GetInstanceID()})");

            if (placed == null || original == null)
            {
                Debug.Log("[SuspicionCheck] One of the items is null.");
                TriggerFailure();
                return;
            }

            if (placed != original)
            {
                Debug.LogWarning("[SuspicionCheck] Reference mismatch! Falling back to name check...");

                if (placed.itemName != original.itemName)
                {
                    Debug.Log("❌ [SuspicionCheck] Fallback comparison also failed. Mismatch confirmed.");
                    TriggerFailure();
                    return;
                }
                else
                {
                    Debug.Log("⚠️ [SuspicionCheck] Reference mismatch but names match — assuming correct.");
                }
            }
            else
            {
                Debug.Log("✅ [SuspicionCheck] Reference match confirmed.");
            }
        }

        TriggerSuccess();
    }

    private void TriggerSuccess()
    {
        Debug.Log("✅ [SuspicionCheck] All items restored to original positions. No suspicion.");
        // successEvent?.SetActive(true);
    }

    private void TriggerFailure()
    {
        Debug.Log("❌ [SuspicionCheck] Items were NOT returned correctly. Curator becomes suspicious!");
        // failureEvent?.SetActive(true);
    }
}
