using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	public MainMenu mainMenu;
	public Button res1;
	public Button res2;
	public Button res3;
	public Button borderless;

	public Slider volumeSlider;
	public Slider mouseSlider;
	public Button back;
	public Button muteMusic;
	PauseMenu pauseMenu;


	void Start()
	{
		res1.onClick.AddListener(() => SetRes(1280, 720));
		res2.onClick.AddListener(() => SetRes(1920, 1080));
		res3.onClick.AddListener(() => SetRes(2560, 1440));
		borderless.onClick.AddListener(FullWin);
		muteMusic.onClick.AddListener(ToggleMuteMusic);
		volumeSlider.onValueChanged.AddListener(Vol);
		mouseSlider.onValueChanged.AddListener(Mouse);

		back.onClick.AddListener(Back);

		volumeSlider.value = AudioListener.volume;
		mouseSlider.value = GameManager.mouseSensitivityT;
	}

	void FullWin()
	{
		Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
		//Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
	}
	

	public void NotifyFromPauseMenu(PauseMenu pauseMenu)
	{
		this.pauseMenu = pauseMenu;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) Back();
	}

	void ToggleMuteMusic()
	{
		var a = FindFirstObjectByType<AudioListener>().gameObject.GetComponent<AudioSource>();
		a.mute = !a.mute;
	}

	void Vol(float v)
	{
		AudioListener.volume = v;
	}

	void Mouse(float v)
	{
		GameManager.mouseSensitivityT = v;
	}


	void SetRes(int x, int y)
	{
		Screen.SetResolution(x, y, FullScreenMode.Windowed);
	}

	void Back()
	{
		gameObject.SetActive(false);

		if (pauseMenu)
		{
			pauseMenu.gameObject.SetActive(true);
			pauseMenu = null;
		}
		else
		{
			mainMenu.gameObject.SetActive(true);
		}
	}
}