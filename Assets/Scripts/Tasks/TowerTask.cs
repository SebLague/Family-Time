using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerTask : Task
{
	public BoxCollider[] poles;
	public BoxCollider[] disks;

	List<int> poleStart = new();
	List<int> poleEnd = new();
	List<int> poleMiddle = new();

	float startY;
	float spacingY;

	Color[] diskCols;
	int selectedDiskIndex;

	void Awake()
	{
		poleStart.AddRange(new int[] { 0, 1, 2, 3 });
		diskCols = disks.Select(d => d.gameObject.GetComponent<MeshRenderer>().material.color).ToArray();
		startY = disks[0].transform.localPosition.z;
		spacingY = disks[1].transform.localPosition.z - startY;

		ExitTask();
	}


	void Update()
	{
		if (!taskActive || !GameManager.Instance.gameActive) return;

		if (Input.GetMouseButtonDown(0))
		{
			int newSelectedDiskIndex = GetNearestMouseOverIndex(disks);
			int selectedPoleIndex = GetNearestMouseOverIndex(poles);

			if (newSelectedDiskIndex != -1)
			{
				selectedDiskIndex = newSelectedDiskIndex;

				for (int i = 0; i < disks.Length; i++)
				{
					Color c = diskCols[i];
					float f = 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;
					disks[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(f, f, f);
				}

				disks[selectedDiskIndex].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = diskCols[selectedDiskIndex];
			}
			else if (selectedDiskIndex != -1 && selectedPoleIndex != -1)
			{
				int currMaxOnPole = GetPoleValue(selectedPoleIndex);
				//Debug.Log($"Selected pole: {selectedPoleIndex}  with topDiskIndex = {currMaxOnPole}");

				// can move to pole
				if (selectedDiskIndex > currMaxOnPole)
				{
					//Debug.Log($"Can move to pole: {selectedDiskIndex} > {currMaxOnPole}");
					int currPoleIndex = GetPoleIndexFromDiskIndex(selectedDiskIndex);
					// is at top of curr pole (can move from this pole)
					if (GetPoleValue(currPoleIndex) == selectedDiskIndex)
					{
						disks[selectedDiskIndex].transform.localPosition = new Vector3(0, poles[selectedPoleIndex].transform.localPosition.y, startY + GetPoleFromPoleIndex(selectedPoleIndex).Count * spacingY);
						PopFromPole(currPoleIndex);
						AddToPole(selectedPoleIndex, selectedDiskIndex);
						selectedDiskIndex = -1;
						ResetCols();
						//Debug.Log($"Can move from pole (is at top): {GetPoleValue(currPoleIndex)} == {selectedDiskIndex}  pos = {poles[selectedPoleIndex].transform.localPosition.y}");
					}
				}
			}
		}

		// Task completion
		if (poleEnd.Count == 4 || (Application.isEditor && Input.GetKeyDown(KeyCode.Q)))
		{
			Debug.Log("Tower completed");
			TaskCompleted();
		}

		// -------- Exit
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ExitTask();
		}
	}

	List<int> GetPoleFromPoleIndex(int p)
	{
		if (p == 0) return poleStart;
		if (p == 1) return poleMiddle;
		return poleEnd;
	}

	int GetPoleIndexFromDiskIndex(int diskIndex)
	{
		if (poleStart.Contains(diskIndex)) return 0;
		if (poleMiddle.Contains(diskIndex)) return 1;
		return 2;
	}

	int GetPoleValue(int poleIndex)
	{
		List<int> pole = GetPoleFromPoleIndex(poleIndex);
		if (pole.Count == 0) return -1;

		return pole[^1];
	}

	void PopFromPole(int poleIndex)
	{
		List<int> pole = GetPoleFromPoleIndex(poleIndex);
		if (pole.Count > 0) pole.RemoveAt(pole.Count - 1);
	}

	void AddToPole(int poleIndex, int value)
	{
		List<int> pole = GetPoleFromPoleIndex(poleIndex);
		pole.Add(value);
	}

	public override void EnterTask()
	{
		base.EnterTask();
		selectedDiskIndex = -1;
	}

	public override void ExitTask()
	{
		base.ExitTask();
		ResetCols();
	}

	void ResetCols()
	{
		for (int i = 0; i < disks.Length; i++)
		{
			disks[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = diskCols[i];
		}
	}
}