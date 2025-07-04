using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Setup")]
    [Tooltip("All puzzle slots to check for correctness.")]
    public PuzzleSlotInteractable[] slots;

    [Header("Objects to activate on success")]
    public GameObject drawer;
    public GameObject clockHand;
    public AudioSource clunkSound;

    [Tooltip("Time before final event triggers (e.g., curator returns).")]
    public float warningTime = 60f;

    private bool puzzleSolved = false;

    public void CheckPuzzleState()
    {
        if (puzzleSolved) return;

        foreach (var slot in slots)
        {
            if (slot.GetSlotState() != PuzzleSlotInteractable.SlotState.Correct)
            {
                Debug.Log("[PuzzleManager] Puzzle not solved yet.");
                return;
            }
        }

        puzzleSolved = true;
        UnlockPuzzle();
    }

    private void UnlockPuzzle()
    {
        Debug.Log("✅ Puzzle solved. Unlocking drawer...");

        // clunkSound?.Play();
        drawer?.SetActive(true);
        clockHand?.SetActive(true);

        // Optional: Trigger enemy return timer
        // CuratorReturnManager.Instance?.StartWarning(warningTime);
    }

    public bool AreAllSlotsInOriginalOrder()
    {
        foreach (var slot in slots)
        {
            if (slot.GetSlotState() != PuzzleSlotInteractable.SlotState.Original)
                return false;
        }

        return true;
    }

    public bool AreAllSlotsEmpty()
    {
        foreach (var slot in slots)
        {
            if (slot.GetSlotState() != PuzzleSlotInteractable.SlotState.Empty)
                return false;
        }

        return true;
    }

    public void ForceReset()
    {
        Debug.Log("[PuzzleManager] Forcing full puzzle reset.");
        foreach (var slot in slots)
        {
            slot.ClearSlot(); // Slot handles destroying the instance and nulling state
        }
    }
}
