using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    void Start()
    {
        playButton.onClick.AddListener(PlayClick);
        settingsButton.onClick.AddListener(SettingsClick);
        quitButton.onClick.AddListener(QuitClick);
    }

    void PlayClick()
    {
        Debug.Log("Play");
        FindFirstObjectByType<GameManager>().StartGame();
    }
    
    void SettingsClick()
    {
        Debug.Log("Settings");
    }
    
    void QuitClick()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    
   
    void Update()
    {
        
    }
}
