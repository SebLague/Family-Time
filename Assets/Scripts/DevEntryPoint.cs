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

	void Start()
	{
		if (startupMode == StartupMode.DevTask)
		{
			startTask.EnterTask();
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
}