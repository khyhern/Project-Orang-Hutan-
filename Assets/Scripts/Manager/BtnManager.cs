using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnManager : MonoBehaviour
{	

	public GameObject PauseMenu;
	public GameObject DeletePanel;
	public static bool isPaused;
	
	
	public void PlayButton() 
	{
		SceneManager.LoadScene(""); 
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

    public void QuitGameButton()
    {
        Application.Quit(); 
    }
    
    public void DeleteButton()
    {
	    DeletePanel.SetActive(true);
    }
    
    public void TrueDeleteButton()
    {
	   
    }
    public void DeleteBackButton()
    {
	    DeletePanel.SetActive(false);
    }


    public void RestartButton()
    {
        SceneManager.LoadScene(""); 
    }


    public void MainMenuButton()
    {
		PauseMenu.SetActive(false);
		Time.timeScale = 1f;
		isPaused = false;
		SceneManager.LoadScene("Main Menu");
    }
}
