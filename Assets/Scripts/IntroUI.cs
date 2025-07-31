using UnityEngine;

public class IntroUI : MonoBehaviour
{
	public TMPro.TMP_Text header;
	public TMPro.TMP_Text goals;

	const string babyGoalA = "Defeat the tower puzzle";
	const string babyGoalB = "Make an awesome artwork";
	const string babyGoalC = "Play some funky tunes";

	public void Set(GameManager.Players playerType)
	{
		string playerName = playerType.ToString().ToUpper();

		string headText = $"YOU ARE {playerName}\n<size=25%>YOU HAVE 3 MINUTES TO ACCOMPLISH YOUR {playerName} GOALS\n(press tab when you are ready to goo goo gaga!)\n";
		string goalText = playerType switch
		{
			GameManager.Players.Baby => MakeGoalsString(babyGoalA, babyGoalB, babyGoalC),
			GameManager.Players.Cat => "cat",
			GameManager.Players.Teen => "teen",
			_ => "error :("
		};

		header.text = headText;
		goals.text = goalText;
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