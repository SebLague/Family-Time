using UnityEngine;

public abstract class Task : MonoBehaviour
{
	[Header("Task")]
	public string infoString;

	public string goalString;

	public bool taskCompleted { get; private set; }
	public Camera cam;
	public FirstPersonController owner;
	protected bool taskActive;

	protected void TaskCompleted()
	{
		taskCompleted = true;
		owner.NotifyTaskCompleted();
	}

	public virtual void EnterTask()
	{
		taskActive = true;
		owner.SetControllable(false);
		owner.gameObject.SetActive(false);
		cam.gameObject.SetActive(true);

		GameManager.ShowCursor(true);
	}

	public virtual void ExitTask()
	{
		owner.gameObject.SetActive(true);
		owner.SetControllable(true);
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