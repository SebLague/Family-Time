using Seb.Helpers;
using UnityEngine;

public class IntroUI : MonoBehaviour
{
	public TMPro.TMP_Text header;
	public TMPro.TMP_Text goals;
	public CanvasGroup group;

	const string babyGoalA = "Defeat the tower puzzle";
	const string babyGoalB = "Make an awesome artwork";
	const string babyGoalC = "Play some funky tunes";
	float time;
	
	
	void Update()
	{
		group.alpha = Maths.EaseQuadInOut(time);
		time += Time.deltaTime;
	}

	public void Set(GameManager.Players playerType)
	{
		string playerName = playerType.ToString().ToUpper();
		string playerName2 = playerType.ToString().ToUpper();
		if (playerType == GameManager.Players.Mother) playerName2 = "MOM";
		if (playerType == GameManager.Players.Father) playerName2 = "DAD";
		if (playerType == GameManager.Players.Cat) playerName2 = "KITTY";

		string headText = $"YOU ARE {playerName}\n<size=25%>YOU HAVE 3 MINUTES TO ACCOMPLISH YOUR {playerName2} GOALS\n(press tab when you are ready to go!)\n";
		string goalText = playerType switch
		{
			GameManager.Players.Baby => MakeGoalsString(babyGoalA, babyGoalB, babyGoalC),
			GameManager.Players.Cat => "cat",
			GameManager.Players.Teen => "teen",
			_ => "error :("
		};

		header.text = headText;
		goals.text = goalText;
		group.alpha = 0;
	}

	static string MakeGoalsString(params string[] goals)
	{
		string s = "";
		for (int i = 0; i < goals.Length; i++)
		{
			s += $"-- {goals[i]} --";
			if (i != goals.Length - 1)
			{
				s += "\n";
			}
		}

		return s;
	}
}