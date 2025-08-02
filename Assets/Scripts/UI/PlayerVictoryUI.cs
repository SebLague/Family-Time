using Seb.Helpers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVictoryUI : MonoBehaviour
{
	public TMPro.TMP_Text ui;
	public Button continueButton;
	public CanvasGroup group;
	float timer;

	void Start()
	{
		continueButton.onClick.AddListener(Continue);
		string text = $"HOORAY!\n<size=50%>YOU COMPLETED ALL YOUR {IntroUI.GetName2(GameManager.Instance.currentPlayer.playerType)} GOALS";
		ui.text = text;
		GameManager.ShowCursor(true);
		group.alpha = 0;
	}


	void Update()
	{
		timer += Time.deltaTime;
		group.alpha = Maths.EaseQuadOut(timer * 2);

		if (Input.GetKeyDown(KeyCode.Return)) Continue();
	}

	void Continue()
	{
		if (timer < 0.2f) return;
		GameManager.Instance.NotifyContinueAfterIndividualVictory();
	}
}