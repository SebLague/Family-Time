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
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void Init()
	{
		Wipe();
	}
}