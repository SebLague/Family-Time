using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum StartupMode
	{
		Game,
		DevTask,
	}

	public enum Players
	{
		Cat,
		Baby,
		Teen
	}

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
	float playerTimer;


	bool playbackTest;
	float playbackTime;

	int playerIndex;

	bool waitingForPlayerConfirm;
	FirstPersonController currentPlayer;
	bool gameActive;

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
	}

	public void NotifyTimeRanOut()
	{
		Debug.Log("Time Ran Out");
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
			playerTimer += Time.deltaTime;
			float timeRemainingSecs = Mathf.Max(0, 60 * 3 - playerTimer);
			int minutes = (int)(timeRemainingSecs / 60);
			int seconds = (int)(timeRemainingSecs % 60);
			string formatted = $"{minutes}:{seconds:D2}";
			timerText.text = formatted;

			if (timeRemainingSecs <= 0)
			{
				gameActive = false;
				NotifyTimeRanOut();
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