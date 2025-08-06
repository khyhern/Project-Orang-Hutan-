using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BtnManager : MonoBehaviour
{	

	public GameObject PauseMenu;
	public static bool isPaused;
	
	public AudioMixer mixer;
	
	[SerializeField] private GameObject MainPanel;
	[SerializeField] private GameObject OptionPanel;
	
	public void PlayButton() 
	{
		SceneManager.LoadScene(""); //change to select save file / Level selection
	}
	
	public void OptionButton() 
	{
		MainPanel.SetActive(false);
		OptionPanel.SetActive(true);
	}
	
    public void QuitGameButton()
    {
        Application.Quit(); // For quitting the application when built
    }
    
    public void RestartButton()
    {
        SceneManager.LoadScene(""); // Replace "level 0" with your actual scene name
    }

       
    public void UpdateBGMVolume(float volume)
    {
	    mixer.SetFloat("BGMVolume", volume);
    }
    
    public void UpdateSFXVolume(float volume)
    {
	    mixer.SetFloat("SFXVolume", volume);
    }
    
    public void SaveVolume()
    {
	    mixer.GetFloat("BGMVolume", out float bgmVolume);
	    PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
	    
	    mixer.GetFloat("SFXVolume", out float sfxVolume);
	    PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    
    // Go to the main menu
    public void MainMenuButton()
    {
		PauseMenu.SetActive(false);
		Time.timeScale = 1f;
		isPaused = false;
		SceneManager.LoadScene("MainMenu");
    }
}
