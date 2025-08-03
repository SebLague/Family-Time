using System;
using System.Collections.Generic;
using UnityEngine;

public class CatCatchTask : Task
{
	int numFlies;
	int numCaught;
	List<FlyController> flies = new();

	public static List<CatchEvent> events = new();
	float playbackTimePrev;

	protected override void Awake()
	{
		base.Awake();

		flies = new(FindObjectsByType<FlyController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
		flies.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
		numFlies = flies.Count;
	}

	public void Caught(FlyController fly)
	{
		CatchEvent e = new()
		{
			time = GameManager.Instance.playerTimer,
			flyIndex = flies.IndexOf(fly)
		};
		events.Add(e);

		numCaught++;
		owner.CatCatchSfx();

		if (numCaught >= numFlies)
		{
			TaskCompleted();
		}
		else
		{
			owner.NotifyTaskProgress();
		}
	}

	protected override string CustomizeGoalString()
	{
		return $"catch bugs {numCaught}/{numFlies}";
	}

	public override void Playback(float playTime)
	{
		foreach (var frame in events)
		{
			if (frame.time > playbackTimePrev && frame.time < playTime)
			{
				flies[frame.flyIndex].gameObject.SetActive(false);
				owner.CatCatchSfx();
			}
		}

		playbackTimePrev = playTime;
	}

	public struct CatchEvent
	{
		public float time;
		public int flyIndex;
	}
}