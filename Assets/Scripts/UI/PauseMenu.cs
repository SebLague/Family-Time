using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button continueBut;
    public Button settingsBut;
    public Button quitBut;
    public SettingsMenu settings;

    void Start()
    {
        quitBut.onClick.AddListener(Quit);
        settingsBut.onClick.AddListener(SettingsGo);
        continueBut.onClick.AddListener(Cont);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cont();
        }
    }

    void Quit()
    {
        GameManager.Instance.SetPaused(false);
        GameManager.Instance.RestartGame();
    }

    void Cont()
    {
        GameManager.Instance.SetPaused(false);
    }
    
    void SettingsGo()
    {
        gameObject.SetActive(false);
        settings.NotifyFromPauseMenu(this);
        settings.gameObject.SetActive(true);
    }
}
