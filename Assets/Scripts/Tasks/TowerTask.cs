using System.Collections.Generic;
using UnityEngine;

public class TowerTask : Task
{
	public BoxCollider[] poles;
	public BoxCollider[] disks;
	
	public List<int> poleStart = new();
	public List<int> poleEnd = new();
	public List<int> poleMiddle = new();

	void Start()
	{
		poleStart.AddRange(new int[] { 0, 1, 2, 3 });
		
		// ----- Test state
		if (!taskActive) ExitTask();
		if (taskActive)
		{
			Baby baby = FindFirstObjectByType<Baby>(FindObjectsInactive.Include);
			EnterTask(baby.gameObject.GetComponent<FirstPersonController>());
		}
	}


	void Update()
	{
	}

	public override void EnterTask(FirstPersonController controller)
	{
		base.EnterTask(controller);
	}

	public override void ExitTask()
	{
		base.ExitTask();
	}
}