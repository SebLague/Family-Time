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

	public int GetNearestMouseOverIndex(BoxCollider[] elements)
	{
		RaycastHit hitInfo;
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);

		int nearestIndex = -1;
		float nearestDst = float.MaxValue;

		for (int i = 0; i < elements.Length; i++)
		{
			elements[i].Raycast(ray, out hitInfo, 100);
			if (hitInfo.collider != null)
			{
				if (hitInfo.distance < nearestDst)
				{
					nearestIndex = i;
					nearestDst = hitInfo.distance;
				}
			}
		}

		return nearestIndex;
	}
}