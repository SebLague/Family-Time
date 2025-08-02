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

	RenderTexture rt;

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
		if (!taskActive || !GameManager.Instance.gameActive) return;

		if (gameController.hasWon)
		{
			TaskCompleted();
		}

		// -------- Exit
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ExitTask();
		}
	}

	public override void EnterTask()
	{
		base.EnterTask();

		gameController.StartGame(controllerFetch.taskCompleted);
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