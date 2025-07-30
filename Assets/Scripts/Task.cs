using UnityEngine;

public abstract class Task : MonoBehaviour
{
	[Header("Task")]
	public string infoString;

	public bool taskActive;

	public abstract void EnterTask(FirstPersonController controller);
}