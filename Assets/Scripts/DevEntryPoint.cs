using UnityEngine;

public class DevEntryPoint : MonoBehaviour
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

	bool playbackTest;
	float playbackTime;

	void Start()
	{
		if (startupMode == StartupMode.DevTask)
		{
			startTask.EnterTask();
		}

		FirstPersonController[] all = new[] { cat, baby };
		foreach (FirstPersonController c in all)
		{
			c.isControllable = false;
		}

		if (startupMode == StartupMode.Baby)
		{
			baby.isControllable = true;
		}
		else if (startupMode == StartupMode.Cat)
		{
			cat.isControllable = true;
		}
	}

	void Update()
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