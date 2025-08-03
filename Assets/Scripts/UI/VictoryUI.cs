using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
	public RawImage[] photos;
	public float rotSpeed;
	public float rotOffset;
	public float rotMag;
	

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
		GameManager.ShowCursor(true);
		for (int i = 0; i < photos.Length; i++)
		{
			photos[i].transform.parent.localEulerAngles = Vector3.forward * (Mathf.Sin(Time.time * rotSpeed + i * rotOffset) * rotMag);
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			GameManager.Instance.RestartGame();
		}
	}
}