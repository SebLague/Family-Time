using UnityEngine;

public class PutOutFiresTask : Task
{
	int numFires;
	int numDone;

	void Start()
	{
		numFires = FindObjectsByType<Candle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
	}

	public void NotifyFireOut()
	{
		numDone++;
		if (numDone >= numFires)
		{
			TaskCompleted();
		}
	}

	protected override string CustomizeGoalString()
	{
		return $"Fight fires {numDone} / {numFires}";
	}
}