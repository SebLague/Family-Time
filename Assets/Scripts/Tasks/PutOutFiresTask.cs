using UnityEngine;

public class PutOutFiresTask : Task
{
	FireExtinguisher fireExtinguisher;
	int numFires;
	int numDone;

	public static float equipTime;

	void Start()
	{
		fireExtinguisher = FindFirstObjectByType<FireExtinguisher>();
		numFires = FindObjectsByType<Candle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
	}

	public void NotifyFireOut()
	{
		numDone++;
		if (numDone >= numFires)
		{
			TaskCompleted();
		}
		else
		{
			owner.NotifyTaskProgress();
		}
	}

	public override void Playback(float playTime)
	{
		if (playTime > equipTime)
		{
			fireExtinguisher.Equip(true);
		}

		fireExtinguisher.Playback(playTime);
	}

	protected override string CustomizeGoalString()
	{
		return $"fight fires {numDone}/{numFires}";
	}

	public void OnExtinguisherEquipped()
	{
		equipTime = GameManager.Instance.playerTimer;
	}
}