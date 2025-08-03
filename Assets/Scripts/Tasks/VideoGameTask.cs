using Seb.Helpers;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class VideoGameTask : Task
{
	[Header("Videogame")]
	public MeshRenderer screen;

	public Camera gameCam;
	public Aircraft gameController;
	public FetchTask controllerFetch;
	public Transform controllerGraphic;

	RenderTexture rt;
	public float maxVol;
	public Sfx winSfx;
	public AudioSource audioSource;
	public float controllerAngleT;

	protected override void Awake()
	{
		base.Awake();

		rt = ComputeHelper.CreateRenderTexture(960, 540, FilterMode.Point, GraphicsFormat.R8G8B8A8_UNorm);
		gameCam.targetTexture = rt;
		screen.material.mainTexture = rt;
		gameController.isplaying = false;
	}

	void Update()
	{
		float targVol = (taskActive && controllerFetch.taskCompleted) ? maxVol : 0;
		audioSource.volume = Mathf.Lerp(audioSource.volume, targVol, Time.deltaTime * 6);

		if (!taskActive || !GameManager.Instance.gameActive) return;

		if (gameController.hasWon && !taskCompleted)
		{
			GameManager.Instance.audioSource2D.PlayOneShot(winSfx.clip, winSfx.volumeT);
			TaskCompleted();
		}

		// -------- Exit
		if (Input.GetKeyDown(GameManager.TaskEnterKey) && Time.frameCount > enterFrame)
		{
			ExitTask();
		}

		Vector3 a = controllerGraphic.localEulerAngles;
		a.z = gameController.currentRollAngle * controllerAngleT;
		controllerGraphic.localEulerAngles = a;
	}

	public override void EnterTask()
	{
		base.EnterTask();

		controllerGraphic.gameObject.SetActive(controllerFetch.taskCompleted);
		gameController.StartGame(controllerFetch.taskCompleted);
		GameManager.ShowCursor(false);
		//GameManager.Instance.playerGoalUI.gameObject.SetActive(true);
	}

	public override void Playback(float playTime)
	{
		gameController.PlaybackUpdate(playTime);
	}

	public override void ExitTask()
	{
		base.ExitTask();

		gameController.StopGame();
	}

	void OnDestroy()
	{
		ComputeHelper.Release(rt);
	}
}