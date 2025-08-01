using UnityEngine;

public class CatNapTask : Task
{
	bool hasCat;

	void Awake()
	{
		ExitTask();
	}

	public override void EnterTask()
	{
		base.EnterTask();

		owner.SetControllable(false);
		owner.gameObject.SetActive(true);
		owner.animator.SetBool("Snooze", true);

		hasCat = true;
		TaskCompleted();
	}

	void Update()
	{
		if (hasCat)
		{
			owner.transform.position = transform.position;
			owner.transform.rotation = transform.rotation;
		}
	}
}