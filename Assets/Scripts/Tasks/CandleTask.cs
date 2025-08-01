using System;
using System.Collections.Generic;
using UnityEngine;

public class CandleTask : Task
{
	List<Candle> candles;
	int toppledCount;
	float playbackTimePrev;

	public static List<ToppleEvent> toppleEvents = new();

	void Awake()
	{
		candles = new (FindObjectsByType<Candle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
		candles.Sort((a,b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
	}

	void Update()
	{
	}

	public void OnTopple(Candle candle, Vector3 dir)
	{
		ToppleEvent e = new()
		{
			time = GameManager.Instance.playerTimer,
			index = candles.IndexOf(candle),
			dir = dir
		};
		toppleEvents.Add(e);

		toppledCount++;
		owner.NotifyTaskProgress();
		if (toppledCount >= candles.Count)
		{
			TaskCompleted();
		}
	}

	protected override string CustomizeGoalString()
	{
		if (candles == null) return "topple candles";
		return $"topple candles {toppledCount} / {candles.Count}";
	}

	public override void Playback(float playTime)
	{
		foreach (var frame in toppleEvents)
		{
			if (frame.time > playbackTimePrev && frame.time < playTime)
			{
				candles[frame.index].ApplyForceDir(frame.dir, false);
			}
		}

		playbackTimePrev = playTime;
	}

	public struct ToppleEvent
	{
		public float time;
		public int index;
		public Vector3 dir;
	}
}