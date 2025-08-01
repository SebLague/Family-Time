using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
	public RawImage[] photos;


	void Start()
	{
		var task = FindFirstObjectByType<PicTask>();
		for (int i = 0; i < Mathf.Min(photos.Length, task.pics.Length); i++)
		{
			photos[i].texture = task.pics[i];
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			GameManager.Instance.RestartGame();
		}
	}
}