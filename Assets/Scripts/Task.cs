using UnityEngine;

public abstract class Task : MonoBehaviour
{
	[Header("Task")]
	public string infoString;

	public bool taskCompleted;
	public Camera cam;
	public FirstPersonController owner;
	protected bool taskActive;

	public virtual void EnterTask()
	{
		taskActive = true;
		owner.gameObject.SetActive(false);
		cam.gameObject.SetActive(true);

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public virtual void ExitTask()
	{
		owner.gameObject.SetActive(true);
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