using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum StartupMode
	{
		Game,
		DevTask,
		Baby,
		Cat
	}

	public StartupMode startupMode;
	public Task startTask;
	public FirstPersonController baby;
	public FirstPersonController cat;
	public Camera menuCam;
	public GameObject mainMenu;
	public GameObject gameHud;

	bool playbackTest;
	float playbackTime;

	void Start()
	{
		FirstPersonController[] all = new[] { cat, baby };
		foreach (FirstPersonController c in all)
		{
			c.isControllable = false;
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

			if (startupMode == StartupMode.DevTask)
			{
				startTask.EnterTask();
			}
			else if (startupMode == StartupMode.Baby)
			{
				baby.isControllable = true;
			}
			else if (startupMode == StartupMode.Cat)
			{
				cat.isControllable = true;
			}
		}
	}

	void Update()
	{
		if (Application.isEditor)
		{
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
		}
	}
}