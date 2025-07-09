// --- PuzzleSlotReference.cs ---
using UnityEngine;

public class PuzzleSlotReference : MonoBehaviour
{
    private PuzzleSlotInteractable slot;

    public void AssignSlot(PuzzleSlotInteractable assignedSlot) => slot = assignedSlot;

    private void OnDestroy()
    {
        if (slot != null)
            slot.ClearSlot();
    }
}