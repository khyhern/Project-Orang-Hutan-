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
        foreach (var slot in slots)
        {
            if (slot.GetPlacedItemName() != slot.expectedItemName)
            {
                Debug.Log("[PuzzleManager] Puzzle not solved yet.");
                return;
            }
        }

        if (!puzzleSolved)
        {
            puzzleSolved = true;
            UnlockPuzzle();
        }
    }

    private void UnlockPuzzle()
    {
        Debug.Log("✅ Puzzle solved. Unlocking drawer...");

        clunkSound?.Play();
        drawer?.SetActive(true);
        clockHand?.SetActive(true);

        // Future logic:
        // CuratorReturnManager.Instance?.StartWarning(warningTime);
    }
}
