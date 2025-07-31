using UnityEngine;

public abstract class Task : MonoBehaviour
{
	[Header("Task")]
	public string infoString;

	public bool taskActive;
	public bool taskCompleted;
	public Camera cam;
	protected FirstPersonController controller;

	public virtual void EnterTask(FirstPersonController controller)
	{
		taskActive = true;
		this.controller = controller;
		controller.gameObject.SetActive(false);
		cam.gameObject.SetActive(true);

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public virtual void ExitTask()
	{
		if (controller) controller.gameObject.SetActive(true);
		cam.gameObject.SetActive(false);
		taskActive = false;
	}
}