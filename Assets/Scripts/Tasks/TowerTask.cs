using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerTask : Task
{
	public BoxCollider[] poles;
	public BoxCollider[] disks;

	public List<int> poleStart = new();
	public List<int> poleEnd = new();
	public List<int> poleMiddle = new();

	Color[] diskCols;

	void Start()
	{
		poleStart.AddRange(new int[] { 0, 1, 2, 3 });
		diskCols = disks.Select(d => d.gameObject.GetComponent<MeshRenderer>().material.color).ToArray();

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
		if (!taskActive) return;

		if (Input.GetMouseButtonDown(0))
		{
			int selectedDiskIndex = GetNearestMouseOverIndex(disks);

			if (selectedDiskIndex != -1)
			{
				Debug.Log(selectedDiskIndex);
				for (int i = 0; i < disks.Length; i++)
				{
					Color c = diskCols[i];
					float f = 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;
					disks[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(f, f, f);
				}

				disks[selectedDiskIndex].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = diskCols[selectedDiskIndex];
			}
			else
			{
				int selectedPoleIndex = GetNearestMouseOverIndex(poles);
			}
		}
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