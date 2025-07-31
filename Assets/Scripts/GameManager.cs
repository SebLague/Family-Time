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


	bool playbackTest;
	float playbackTime;

	int playerIndex;

	bool waitingForPlayerConfirm;
	FirstPersonController currentPlayer;

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