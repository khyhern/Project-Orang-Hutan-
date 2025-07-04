using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PuzzleSlotInteractable[] slots;
    public GameObject drawer;
    public GameObject clockHand;
    public AudioSource clunkSound;
    public float warningTime = 60f;

    private bool puzzleSolved = false;

    public void CheckPuzzleState()
    {
        foreach (var slot in slots)
        {
            if (slot.GetPlacedItemName() != slot.expectedItemName)
                return;
        }

        if (!puzzleSolved)
        {
            puzzleSolved = true;
            UnlockDrawer();
        }
    }

    void UnlockDrawer()
    {
        Debug.Log("✅ Puzzle solved. Drawer unlocked.");
        clunkSound?.Play();
        drawer.SetActive(true);
        clockHand.SetActive(true);
        // CuratorReturnManager.Instance?.StartWarning(warningTime);
    }
}
