using UnityEngine;

public class CatCatchTask : Task
{
	int numFlies;
	int numCaught;

	void Awake()
	{
		numFlies = FindObjectsByType<FlyController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
	}

	public void Caught()
	{
		numCaught++;

		if (numCaught >= numFlies)
		{
			TaskCompleted();
		}
	}

	protected override string CustomizeGoalString()
	{
		return $"catch flies {numCaught} / {numFlies}";
	}
}