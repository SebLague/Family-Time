using UnityEngine;

public class CandleTask : Task
{
	Candle[] candles;
	int toppledCount;

	void Awake()
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
		if (candles == null) return "topple candles";
		return $"topple candles {toppledCount} / {candles.Length}";
	}
}