using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("Puzzle Setup")]
    [Tooltip("All puzzle slots to check for correctness.")]
    public PuzzleSlotInteractable[] slots;

    [Header("Success Activation")]
    public GameObject drawer;
    public GameObject clockHand;
    public AudioSource clunkSound;

    private bool puzzleSolved = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CheckPuzzleState()
    {
        foreach (var slot in slots)
        {
            if (slot.GetSlotState() != PuzzleSlotInteractable.SlotState.Correct)
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
        Debug.Log("✅ [PuzzleManager] Puzzle solved. Unlocking drawer...");

        // clunkSound?.Play();
        drawer?.SetActive(true);
        clockHand?.SetActive(true);

        SuspicionCheckManager.Instance?.BeginInspectionCountdown();
    }
}
