using System.Linq;
using Seb.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	//public const KeyCode CatchFlyKey = KeyCode.Space;
	public const KeyCode PickupKey = KeyCode.F;
	public const KeyCode TaskEnterKey = KeyCode.F;


	public enum StartupMode
	{
		Game,
		DevTask,
	}

	public enum Players
	{
		Cat,
		Baby,
		Teenager,
		Mother,
		Father
	}

	public Transform title;
	public int numSecs = 60 * 3;
	public StartupMode startupMode;
	public Task startTask;
	public FirstPersonController[] players;
	public CamFixed menuCam;
	public GameObject mainMenu;
	public GameObject gameHud;
	public IntroUI introUI;
	public Color goalSuccessCol;
	public TMPro.TMP_Text timerText;
	public TMPro.TMP_Text playerGoalUI;
	public FailUI failUI;
	public float playerTimer { get; private set; }
	public VictoryUI victoryUI;
	public GameObject menuObjects;
	public PlayerVictoryUI miniVictoryUI;
	public SettingsMenu settingsMenu;
	public PauseMenu pauseMenu;
	bool isPaused;
	public static float mouseSensitivityT = 0.5f;
	Transform audioListener;
	public GameObject persistantPrefab;

	public Sfx[] impactSounds;
	public Sfx[] impactSoundsGlass;

	public float camAnimOffset;
	public float camAnimSpeed;
	static int numLoads;

	bool playbackTest;
	float playbackTime;

	int playerIndex;
	[HideInInspector] public AudioSource audioSource2D;
	public Sfx failSfx;

	bool waitingForPlayerConfirm;
	public FirstPersonController currentPlayer { get; private set; }
	public bool gameActive { get; private set; }
	[HideInInspector] public bool ignoreTimer;
	float waitStartTime;

	static GameManager instance;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void Init()
	{
		numLoads = 0;
	}

	void InitStartupSettings()
	{
		Debug.Log("init default settings");
		mouseSensitivityT = 0.5f;
		AudioListener.volume = 0.5f;
	}

	public void SetPaused(bool paused)
	{
		isPaused = paused;
		Time.timeScale = paused ? 0 : 1;
		gameActive = !paused;

		pauseMenu.gameObject.SetActive(paused);

		if (paused)
		{
			ShowCursor(true);
		}
	}

	public static GameManager Instance
	{
		get
		{
			if (instance == null) instance = FindFirstObjectByType<GameManager>();
			return instance;
		}
	}

	void Start()
	{
		ShowCursor(true);
		if (numLoads == 0)
		{
			GameObject a = Instantiate(persistantPrefab);
			audioListener = a.transform;
			GameObject.DontDestroyOnLoad(audioListener);
			InitStartupSettings();
		}
		
		audioSource2D = GetComponent<AudioSource>();
		audioListener = FindFirstObjectByType<AudioListener>().transform;

		foreach (FirstPersonController c in players)
		{
			c.SetControllable(false);
		}

		if (startupMode == StartupMode.Game)
		{
			mainMenu.SetActive(true);
			gameHud.SetActive(false);
			menuCam.gameObject.SetActive(true);

			if (PlaybackData.skipMenu)
			{
				playerIndex = PlaybackData.activePlayerIndex;
				StartGame();
			}
		}
		// Dev startup modes
		else
		{
			mainMenu.SetActive(false);
			gameHud.SetActive(true);
			menuCam.gameObject.SetActive(false);

			if (startupMode == StartupMode.DevTask) startTask.EnterTask();
		}
	}

	public void NotifyAllTasksCompleted()
	{
		gameActive = false;

		//playerGoalUI.gameObject.SetActive(false);
		PlaybackData.playbacks[currentPlayer.playerType] = currentPlayer.playbackKeyframes;

		if (currentPlayer.playerType == Players.Father)
		{
			Victory();
		}
		else
		{
			miniVictoryUI.gameObject.SetActive(true);
		}
	}

	public void NotifyContinueAfterIndividualVictory()
	{
		playerIndex++;
		timerText.gameObject.SetActive(false);
		RestartTimeLoop();
	}

	void Victory()
	{
		victoryUI.gameObject.SetActive(true);
	}

	public void OnFailTimeRanOut()
	{
		OnFail("Oh no...\n<size=50%>You ran out of time!");
	}

	public void OnFailCatNapTooSoon()
	{
		OnFail("Oh no...\n<size=50%>You fell asleep before accomplishing your goals!");
	}

	void OnFail(string reason)
	{
		
		audioSource2D.PlayOneShot(failSfx.clip, failSfx.volumeT);
		failUI.reason.text = reason;
		gameActive = false;
		failUI.gameObject.SetActive(true);
		ShowCursor(true);
	}

	public static void ShowCursor(bool show)
	{
		Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = show;
	}

	public void RestartTimeLoop()
	{
		SetPaused(false);
		PlaybackData.skipMenu = true;
		PlaybackData.activePlayerIndex = playerIndex;
		numLoads++;
		SceneManager.LoadScene(0);
	}

	public void RestartGame()
	{
		SetPaused(false);
		gameActive = false;
		PlaybackData.Wipe();
		numLoads++;
		SceneManager.LoadScene(0);
	}

	public void StartGame()
	{
		mainMenu.SetActive(false);
		currentPlayer = players[playerIndex];
		StartNextPlayer();
	}

	void StartNextPlayer()
	{
		mainMenu.gameObject.SetActive(false);
		introUI.gameObject.SetActive(true);
		introUI.Set(currentPlayer.playerType);
		waitingForPlayerConfirm = true;
		waitStartTime = Time.time;
		
		audioListener.transform.position = menuCam.transform.position;
		//menuCam.GetComponent<CamFixed>().active = false;
	}

	void Update()
	{
		if (waitingForPlayerConfirm)
		{
			title.position += Vector3.up * (Time.deltaTime * 3);
			Vector3 targetPos = currentPlayer.cam.transform.position - currentPlayer.transform.forward * camAnimOffset + currentPlayer.menuCamOffset;
			menuCam.transform.position = Vector3.Lerp(menuCam.transform.position, targetPos, Time.deltaTime * camAnimSpeed);
			menuCam.transform.rotation = Quaternion.Slerp(menuCam.transform.rotation, Quaternion.LookRotation(-currentPlayer.transform.forward, Vector3.up), Time.deltaTime * camAnimSpeed);
			menuCam.UpdateBaseRot();

			if ((Input.anyKeyDown) && Time.time - waitStartTime > 0.25f)
			{
				menuObjects.SetActive(false);
				waitingForPlayerConfirm = false;
				menuCam.gameObject.SetActive(false);
				currentPlayer.SetControllable(true);
				introUI.gameObject.SetActive(false);
				gameHud.SetActive(true);
				timerText.gameObject.SetActive(true);
				gameActive = true;
			}
		}

		if (gameActive)
		{
			audioListener.transform.position = currentPlayer.cam.transform.position;
			// playback prev players
			for (int i = 0; i < playerIndex; i++)
			{
				FirstPersonController playbackPlayer = players[i];
				playbackPlayer.playbackKeyframes = PlaybackData.playbacks[playbackPlayer.playerType];
				playbackPlayer.PlaybackUpdate(playerTimer); //

				foreach (Task task in playbackPlayer.tasks)
				{
					task.Playback(playerTimer);
				}
			}

			// Update timer
			playerTimer += Time.deltaTime;
			float timeRemainingSecs = Mathf.Max(0, numSecs - playerTimer);
			int minutes = (int)(timeRemainingSecs / 60);
			int seconds = (int)(timeRemainingSecs % 60);
			string formatted = $"{minutes}:{seconds:D2}";
			timerText.text = formatted;
			if (timeRemainingSecs < 5)
			{
				timerText.color = Color.Lerp(timerText.color, Color.red, Time.deltaTime * 0.3f);
				timerText.transform.localScale = Vector3.one * Mathf.Lerp(1.3f, 1, Maths.EaseQuadInOut(timeRemainingSecs / 5f));
			}

			if (timeRemainingSecs <= 0 && !ignoreTimer)
			{
				gameActive = false;
				OnFailTimeRanOut();
			}
		}
	

		if (!mainMenu.gameObject.activeSelf && !settingsMenu.gameObject.activeSelf && !isPaused)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				SetPaused(true);
			}
		}

		DevMode();
	}

	void DevMode()
	{
		if (Application.isEditor)
		{
			
			if (Input.GetKeyDown(KeyCode.Q))
			{
				Time.timeScale = 1 - Time.timeScale;
				gameActive = !gameActive;
				ShowCursor(true);
			}

			
		}
	}
}