using UnityEngine;

public class CandleTask : Task
{
	Candle[] candles;
	int toppledCount;

	void Start()
	{
		candles = FindObjectsByType<Candle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
	}

	void Update()
	{
	}

	public void OnTopple()
	{
		toppledCount++;
		owner.NotifyTaskProgress();
		if (toppledCount >= candles.Length)
		{
			TaskCompleted();
		}
	}

	protected override string CustomizeGoalString()
	{
		return $"topple candles {toppledCount} / {candles.Length}";
	}
}