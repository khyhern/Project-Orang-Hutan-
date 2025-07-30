using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("Puzzle Setup")]
    [SerializeField] private PuzzleSlotInteractable[] slots;

    [Header("Drawer Audio Source")]
    [SerializeField] private AudioSource drawerAudioSource;

    private bool puzzleSolved = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (drawerAudioSource == null)
        {
            Debug.LogWarning("[PuzzleManager] Drawer AudioSource not assigned.");
        }
        else if (drawerAudioSource.clip == null)
        {
            Debug.LogWarning("[PuzzleManager] Drawer AudioSource has no clip assigned.");
        }
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
        Debug.Log("✅ [PuzzleManager] Puzzle solved.");
        PlayDrawerSound();
        SuspicionCheckManager.Instance?.BeginInspectionCountdown();
    }

    private void PlayDrawerSound()
    {
        if (drawerAudioSource == null) return;
        drawerAudioSource.Play();
    }
}