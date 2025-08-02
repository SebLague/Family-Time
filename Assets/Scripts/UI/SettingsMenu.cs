using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public MainMenu mainMenu;
    public Button res1;
    public Button res2;
    public Button res3;
    public Button borderless;
    public Button fullscreen;
   
    public Slider volumeSlider;
    public Button back;
    
    
    void Start()
    {
        res1.onClick.AddListener(() => SetRes(1280, 720));
        res2.onClick.AddListener(() => SetRes(1920, 1080));
        res3.onClick.AddListener(() => SetRes(2560, 1440));
        borderless.onClick.AddListener(() => Screen.fullScreenMode = FullScreenMode.FullScreenWindow);
        fullscreen.onClick.AddListener(() => Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen);
        volumeSlider.onValueChanged.AddListener(Vol);
        
        back.onClick.AddListener(Back);
        
        volumeSlider.value = AudioListener.volume;
    }

    void Vol(float v)
    {
        AudioListener.volume = v;
    }
    

    void SetRes(int x, int y)
    {
        Screen.SetResolution(x,y, FullScreenMode.Windowed);
        
    }
    void Back()
    {
        gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    
}
