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
	public RawImage camFlash;
	public Camera picCaptureCam;

	public readonly RenderTexture[] pics = new RenderTexture[picsTotalToTake];
	const float picPreviewDuration = 1;
	float nextPicableTime;
	public Sfx picSfx;
	float prevPicTime=-10;

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
			AudioSource.PlayClipAtPoint(picSfx.clip, owner.cam.transform.position, picSfx.volumeT);
			pics[numPicsTaken] = ComputeHelper.CreateRenderTexture(Screen.width / 4, Screen.height / 4, FilterMode.Point, GraphicsFormat.R8G8B8A8_UNorm);
			picCaptureCam.targetTexture = pics[numPicsTaken];
			picCaptureCam.Render();

			camDisplay.texture = pics[numPicsTaken];
			camDisplay.color = Color.white;
			numPicsTaken++;
			prevPicTime = Time.time;
			nextPicableTime = Time.time + picPreviewDuration;
			camHudInner.SetActive(false);
			if (numPicsTaken >= picsTotalToTake) TaskCompletedButDontNotify();
			owner.NotifyTaskProgress();
			Debug.Log("take pic");
		}

		
		float timeF = Time.time - prevPicTime;
		float flashT = 1 - timeF / picPreviewDuration * 2;
		float flashA = Maths.EaseQuadOut(flashT);
		camFlash.color = new Color(0, 0, 0, flashA);
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