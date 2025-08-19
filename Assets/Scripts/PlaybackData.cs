using System.Collections.Generic;
using UnityEngine;

public static class PlaybackData
{
	public static Dictionary<GameManager.Players, List<FirstPersonController.PlaybackKeyframe>> playbacks = new();
	public static int activePlayerIndex;
	public static bool skipMenu;


	public static void Wipe()
	{
		Debug.Log("Clear all playbacks");
		playbacks = new();
		activePlayerIndex = 0;
		skipMenu = false;

		WipeCat();
		WipeBaby();
		WipeTeen();
		WipeMother();

	}

	static void WipeCat()
	{
		CandleTask.toppleEvents.Clear();
		CatCatchTask.events.Clear();
	}

	static void WipeBaby()
	{
		ScribbleTask.keyframes.Clear();
		TowerTask.keyframes.Clear();
		XyloTask.keyframes.Clear();
	}

	static void WipeTeen()
	{
		Aircraft.frames.Clear();
	}

	static void WipeMother()
	{
		FireExtinguisher.shootTimes.Clear();
	}

	public static void WipeCurrentOnly()
	{
		if (activePlayerIndex == (int)GameManager.Players.Cat) WipeCat();
		if (activePlayerIndex == (int)GameManager.Players.Baby) WipeBaby();
		if (activePlayerIndex == (int)GameManager.Players.Teenager) WipeTeen();
		if (activePlayerIndex == (int)GameManager.Players.Mother) WipeMother();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void Init()
	{
		Wipe();
	}
}