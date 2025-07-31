using System.Collections.Generic;
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

	public static List<XyloSoundKeyframe> keyframes = new();
	float playbackTimePrev;

	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		notePlayCounts = new int[notes.Length];

		ExitTask();
	}


	void Update()
	{
		if (!taskActive || !GameManager.Instance.gameActive) return;

		// ---------- Play notes
		int selectedNoteIndex = GetNearestMouseOverIndex(notes);

		if (Input.GetMouseButtonDown(0) && selectedNoteIndex != -1)
		{
			PlayNoteSound(selectedNoteIndex);
			notePlayCounts[selectedNoteIndex]++;

			// Record
			XyloSoundKeyframe keyframe = new()
			{
				time = GameManager.Instance.playerTimer,
				noteIndex = selectedNoteIndex
			};
			keyframes.Add(keyframe);
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
		if (Application.isEditor && Input.GetKeyDown(KeyCode.Q)) scoreInt = 100;

		if (!taskCompleted)
		{
			scoreUI.text = $"music score: {scoreInt} %";
		}

		if (scoreInt >= 100 && !taskCompleted)
		{
			scoreUI.color = textColDone;
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
		scoreUI.gameObject.SetActive(true);
	}

	public override void ExitTask()
	{
		base.ExitTask();
		scoreUI.gameObject.SetActive(false);
	}

	public override void Playback(float playTime)
	{
		foreach (var frame in keyframes)
		{
			if (frame.time > playbackTimePrev && frame.time < playTime)
			{
				PlayNoteSound(frame.noteIndex);
			}
		}

		playbackTimePrev = playTime;
	}

	void PlayNoteSound(int noteIndex)
	{
		audioSource.PlayOneShot(noteSounds[noteIndex], Mathf.Lerp(0.6f, 1, Random.value));
	}

	public struct XyloSoundKeyframe
	{
		public float time;
		public int noteIndex;
	}
}