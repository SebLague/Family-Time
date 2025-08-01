using UnityEngine;

public abstract class Task : MonoBehaviour
{
	[Header("Task")]
	public bool isEnterableTask = true;
	public string infoString;
	public string goalString;

	public bool taskCompleted { get; private set; }
	public Camera cam;
	public FirstPersonController owner;
	protected bool taskActive;
	bool taskCompleteQuiet;

	public string GetGoalString()
	{
		return FirstPersonController.TextCol(CustomizeGoalString(), GameManager.Instance.goalSuccessCol, taskCompleted || taskCompleteQuiet);
	}

	protected virtual string CustomizeGoalString()
	{
		return goalString;
	}

	protected void TaskCompleted()
	{
		Debug.Log("Task Completed");
		if (!taskCompleted)
		{
			taskCompleted = true;
			owner.NotifyTaskCompleted();
		}
	}

	protected void TaskCompletedButDontNotify()
	{
		taskCompleteQuiet = true;
	}


	public virtual void EnterTask()
	{
		if (isEnterableTask)
		{
			taskActive = true;
			owner.SetControllable(false);
			owner.gameObject.SetActive(false);
			cam.gameObject.SetActive(true);

			GameManager.ShowCursor(true);
		}
	}

	public virtual void ExitTask()
	{
		if (isEnterableTask)
		{
			owner.gameObject.SetActive(true);
			owner.SetControllable(true);
			cam.gameObject.SetActive(false);
			taskActive = false;
		}
	}

	public virtual void Playback(float playTime)
	{
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