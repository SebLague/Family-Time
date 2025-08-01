using UnityEngine;

public class VideoGameTask : Task
{

	void Update()
	{
		if (!taskActive || !GameManager.Instance.gameActive) return;
		
		// -------- Exit
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ExitTask();
		} 
	}
}