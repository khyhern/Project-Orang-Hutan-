using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnManager : MonoBehaviour
{	

	public GameObject PauseMenu;
	public static bool isPaused;
	
	//Main Menu Buttons
	
	public void PlayButton() 
	{
		SceneManager.LoadScene(""); //change to select save file / Level selection
	}
	
	public void Level1Button() 
	{
		SceneManager.LoadScene("Level 1");
	}
	
	public void Level2Button() 
	{
		SceneManager.LoadScene("Level 2");
	}
	
	public void Level3Button() 
	{
		SceneManager.LoadScene("Level 3");
	}
	
    // Quit the game (Main Menu, Game Over & Pause Game)
    public void QuitGameButton()
    {
        Application.Quit(); // For quitting the application when built
    }

    // Restart the level
    public void RestartButton()
    {
        SceneManager.LoadScene(""); // Replace "level 0" with your actual scene name
    }

    // Go to the main menu
    public void MainMenuButton()
    {
		PauseMenu.SetActive(false);
		Time.timeScale = 1f;
		isPaused = false;
		SceneManager.LoadScene("Main Menu");
    }
}
