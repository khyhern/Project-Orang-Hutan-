using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
	public GameObject PauseMenu;
	public static bool isPaused;
	
    void Start()
    {
        PauseMenu.SetActive(false);
		isPaused = false;
		Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (NoteUIManager.IsNoteOpen) return; // Don't allow pausing when note is open

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


    public void PauseGame()
	{
		PauseMenu.SetActive(true);
		Time.timeScale = 0f;
		isPaused = true;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		
	}
	
	public void ResumeGame()
	{
		
		PauseMenu.SetActive(false);
		Time.timeScale = 1f;
		isPaused = false;
		
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}
