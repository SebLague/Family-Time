using System;
using Seb.Helpers;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class PicTask : Task
{
	bool picMode;
	int numPicsTaken;
	const int picsTotalToTake = 5;
	public GameObject camHud;
	public GameObject camHudInner;
	public RawImage camDisplay;
	public Camera picCaptureCam;

	public readonly RenderTexture[] pics = new RenderTexture[picsTotalToTake];
	const float picPreviewDuration = 1;
	float nextPicableTime;

	public void NotifyPicMode()
	{
		picMode = true;
		camHud.SetActive(true);
	}

	void Update()
	{
		if (!picMode || !GameManager.Instance.gameActive) return;

		if (Time.time > nextPicableTime && !camHudInner.activeSelf)
		{
			if (numPicsTaken >= picsTotalToTake)
			{
				TaskCompleted();
			}

			camDisplay.color = Color.clear;
			camHudInner.SetActive(true);
		}

		if (Input.GetMouseButtonDown(0) && numPicsTaken < picsTotalToTake && Time.time > nextPicableTime)
		{
			pics[numPicsTaken] = ComputeHelper.CreateRenderTexture(Screen.width / 4, Screen.height / 4, FilterMode.Point, GraphicsFormat.R8G8B8A8_UNorm);
			picCaptureCam.targetTexture = pics[numPicsTaken];
			picCaptureCam.Render();

			camDisplay.texture = pics[numPicsTaken];
			camDisplay.color = Color.white;
			numPicsTaken++;
			nextPicableTime = Time.time + picPreviewDuration;
			camHudInner.SetActive(false);
			if (numPicsTaken >= picsTotalToTake) TaskCompletedButDontNotify();
			owner.NotifyTaskProgress();
			Debug.Log("take pic");
		}
	}

	protected override string CustomizeGoalString()
	{
		return $"snap pics {numPicsTaken} / {picsTotalToTake}";
	}

	void OnDestroy()
	{
		foreach (RenderTexture pic in pics)
		{
			ComputeHelper.Release(pic);
		}
	}
}