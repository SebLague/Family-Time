using UnityEngine;

public class GameHud : MonoBehaviour
{
	public TMPro.TMP_Text infoUI;

	void Start()
	{
		ClearInfo();
	}

	public void SetInfoText(string info)
	{
		infoUI.text = info;
	}

	public void ClearInfo()
	{
		infoUI.text = "";
	}
}