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

    public void ShowNote(string content)
    {
        if (notePanel == null || noteText == null) return;

        noteText.text = content;
        notePanel.SetActive(true);
        isNoteOpen = true;

        Time.timeScale = 0f;

        // Disable gameplay UI
        ToggleGameplayUI(false);

        // Optional: block player input
        InputBlocker.IsInputBlocked = true;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.None;   // Unlock if you want free mouse
        Cursor.visible = false;                   // Keep it hidden for immersion
    }

    public void CloseNote()
    {
        if (notePanel == null) return;

        notePanel.SetActive(false);
        isNoteOpen = false;

        Time.timeScale = 1f;

        // Re-enable gameplay UI
        ToggleGameplayUI(true);

        // Optional: unblock player input
        InputBlocker.IsInputBlocked = false;

        // Lock and hide cursor for first-person mode
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
