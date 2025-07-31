using UnityEngine;

public class XyloTask : Task
{
	[Header("Xylo")]

	public TMPro.TMP_Text scoreUI;
	public BoxCollider[] notes;
	public AudioClip[] noteSounds;
	public Color textColDone;

	AudioSource audioSource;
	int[] notePlayCounts;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		notePlayCounts = new int[notes.Length];

		// ----- Test state
		if (!taskActive) ExitTask();
		if (taskActive)
		{
			Baby baby = FindFirstObjectByType<Baby>(FindObjectsInactive.Include);
			EnterTask(baby.gameObject.GetComponent<FirstPersonController>());
		}
	}


	void Update()
	{
		if (!taskActive) return;

		// ---------- Play notes
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		int selectedNoteIndex = -1;
		float nearestDst = float.MaxValue;

		for (int i = 0; i < notes.Length; i++)
		{
			notes[i].Raycast(ray, out hitInfo, 100);
			if (hitInfo.collider != null)
			{
				if (hitInfo.distance < nearestDst)
				{
					selectedNoteIndex = i;
					nearestDst = hitInfo.distance;
				}
			}
		}

		if (Input.GetMouseButtonDown(0) && selectedNoteIndex != -1)
		{
			audioSource.PlayOneShot(noteSounds[selectedNoteIndex], Mathf.Lerp(0.6f, 1, Random.value));
			notePlayCounts[selectedNoteIndex]++;
		}

		// ------------ Score
		int totalNotesPlayed = 0;
		int notesPlayedAtLeastOnce = 0;
		int notesPlayedAtLeastTwice = 0;
		int notesPlayedAtLeastThrice = 0;

		for (int i = 0; i < notes.Length; i++)
		{
			totalNotesPlayed += Mathf.Min(4, notePlayCounts[i]);
			if (notePlayCounts[i] > 0) notesPlayedAtLeastOnce++;
			if (notePlayCounts[i] > 1) notesPlayedAtLeastTwice++;
			if (notePlayCounts[i] > 2) notesPlayedAtLeastThrice++;
		}

		float scoreT_A = Mathf.Clamp01(totalNotesPlayed / 24f);
		float scoreT_B = Mathf.Clamp01(notesPlayedAtLeastOnce / 6f);
		float scoreT_C = Mathf.Clamp01(notesPlayedAtLeastTwice / 3f);
		float scoreT_D = Mathf.Clamp01(notesPlayedAtLeastThrice / 2f);
		float scoreT = scoreT_A * 0.5f + scoreT_B * 0.2f + scoreT_C * 0.2f + scoreT_D * 0.1f;
		int scoreInt = Mathf.CeilToInt(scoreT * 100);
		if (!taskCompleted)
		{
			scoreUI.text = $"music score: {scoreInt} %";
		}

		if (scoreInt >= 100 && !taskCompleted)
		{
			scoreUI.color = textColDone;
			taskCompleted = true;
		}

		// -------- Exit
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ExitTask();
		}
	}

	public override void EnterTask(FirstPersonController controller)
	{
		base.EnterTask(controller);
		scoreUI.gameObject.SetActive(true);
	}

	public override void ExitTask()
	{
		base.ExitTask();
		scoreUI.gameObject.SetActive(false);
	}
}