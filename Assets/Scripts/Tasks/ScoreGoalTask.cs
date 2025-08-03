using System;
using UnityEngine;

public class ScoreGoalTask : Task
{
	public GameObject[] balls;

	public void OnTriggerEnter(Collider other)
	{
		if (taskCompleted) return;
		foreach (var b in balls)
		{
			if (other.gameObject == b)
			{
				TaskCompleted();
				GetComponent<AudioSource>().Play();
				break;
			}
		}
	}
}