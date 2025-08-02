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
	[HideInInspector] public bool ownerInRegion;
	protected int enterFrame;
	public GameObject bg;

	protected virtual void Awake()
	{
		if (cam) cam.gameObject.SetActive(false);
		if (bg) bg.SetActive(false);
	}

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
		if (!taskCompleted)
		{
			Debug.Log("Task Completed");
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
			if (bg) bg.SetActive(true);
			enterFrame = Time.frameCount;
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
			if (bg) bg.SetActive(false);
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

	public int GetNearestMouseOverIndex(BoxCollider[] elements, out float dst)
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

		dst = nearestDst;
		return nearestIndex;
	}

	public int GetNearestMouseOverIndex(BoxCollider[] elements, out Vector3 pos)
	{
		RaycastHit hitInfo;
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		pos = Vector3.zero;
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
					pos = hitInfo.point;
				}
			}
		}

		return nearestIndex;
	}
}