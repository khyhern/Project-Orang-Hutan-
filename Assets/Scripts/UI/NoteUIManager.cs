using UnityEngine;
using TMPro;

public class NoteUIManager : MonoBehaviour
{
    [Header("Note UI")]
    public GameObject notePanel;
    public TextMeshProUGUI noteText;

    [Header("Gameplay UI Elements")]
    public GameObject healthUI;
    public GameObject staminaUI;
    public GameObject crosshairUI;
    public GameObject interactionTextUI;

    [Header("Settings")]
    public KeyCode closeKey = KeyCode.E;

    private bool isNoteOpen = false;

    private void Start()
    {
        if (notePanel != null)
            notePanel.SetActive(false);
    }

    private void Update()
    {
        if (isNoteOpen && Input.GetKeyDown(closeKey))
        {
            CloseNote();
        }
    }

    private void LateUpdate()
    {
        // Ensure cursor stays hidden during note reading (Unity sometimes enables it unexpectedly)
        if (isNoteOpen && Cursor.visible)
        {
            Cursor.visible = false;
        }
    }

    public void ShowNote(string content)
    {
        if (notePanel == null || noteText == null) return;

        noteText.text = content;
        notePanel.SetActive(true);
        isNoteOpen = true;

        Time.timeScale = 0f;

        ToggleGameplayUI(false);

        // Optional: block player input
        InputBlocker.IsInputBlocked = true;

        // Lock mouse inside window & hide it (prevents flicker)
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void CloseNote()
    {
        if (notePanel == null) return;

        notePanel.SetActive(false);
        isNoteOpen = false;

        Time.timeScale = 1f;

        ToggleGameplayUI(true);

        // Optional: unblock player input
        InputBlocker.IsInputBlocked = false;

        // Lock and hide cursor for first-person gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ToggleGameplayUI(bool show)
    {
        if (healthUI != null) healthUI.SetActive(show);
        if (staminaUI != null) staminaUI.SetActive(show);
        if (crosshairUI != null) crosshairUI.SetActive(show);
        if (interactionTextUI != null) interactionTextUI.SetActive(show);
    }
}
