using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public const KeyCode CatchFlyKey = KeyCode.Space;
	public const KeyCode TaskEnterKey = KeyCode.Tab;

	public enum StartupMode
	{
		Game,
		DevTask,
	}

	public enum Players
	{
		Cat,
		Baby,
		Teen,
		Mother,
		Father
	}

	public int numSecs = 60 * 3;
	public StartupMode startupMode;
	public Task startTask;
	public FirstPersonController[] players;
	public Camera menuCam;
	public GameObject mainMenu;
	public GameObject gameHud;
	public IntroUI introUI;
	public Color goalSuccessCol;
	public TMPro.TMP_Text timerText;
	public TMPro.TMP_Text playerGoalUI;
	public FailUI failUI;
	public float playerTimer { get; private set; }


	bool playbackTest;
	float playbackTime;

	int playerIndex;

	bool waitingForPlayerConfirm;
	FirstPersonController currentPlayer;
	public bool gameActive { get; private set; }
	[HideInInspector] public bool ignoreTimer;

	static GameManager instance;

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
		timerText.gameObject.SetActive(false);
		PlaybackData.playbacks[currentPlayer.playerType] = currentPlayer.playbackKeyframes;

		playerIndex++;
		RestartTimeLoop();
	}

	public void OnFailTimeRanOut()
	{
		OnFail("YOU RAN OUT OF TIME!");
	}

	public void OnFailCatNapTooSoon()
	{
		OnFail("Oh no!\n<size=50%>You fell asleep before accomplishing your goals!");
	}

	void OnFail(string reason)
	{
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
		PlaybackData.skipMenu = true;
		PlaybackData.activePlayerIndex = playerIndex;
		SceneManager.LoadScene(0);
	}

	public void RestartGame()
	{
		PlaybackData.Wipe();
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
	}

	void Update()
	{
		if (waitingForPlayerConfirm)
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
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

			if (timeRemainingSecs <= 0 && !ignoreTimer)
			{
				gameActive = false;
				OnFailTimeRanOut();
			}
		}

		DevMode();
	}

	void DevMode()
	{
		if (Application.isEditor)
		{
			/*
			if (Input.GetKeyDown(KeyCode.P))
			{
				Debug.Log("TEST");
				baby.isControllable = false;
				playbackTest = true;
				playbackTime = 0;
			}

			if (playbackTest)
			{
				playbackTime += Time.deltaTime;
				baby.PlaybackUpdate(playbackTime);
			}
			*/
		}
	}
}