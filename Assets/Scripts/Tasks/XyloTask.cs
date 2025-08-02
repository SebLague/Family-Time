using System.Collections.Generic;
using Seb.Helpers;
using UnityEngine;

public class XyloTask : Task
{
	[Header("Xylo")]
	public float animSpeed = 2;
	public float yAddMul = 2;
	public TMPro.TMP_Text scoreUI;

	public BoxCollider[] notes;
	public AudioClip[] noteSounds;
	public Color textColDone;
	public Transform stickPreview;
	public Transform stick;

	Queue<AnimState> animQueue = new();
	public Transform hitLineA;
	public Transform hitLineB;
	
	

	AudioSource audioSource;
	int[] notePlayCounts;

	public static List<XyloSoundKeyframe> keyframes = new();
	float playbackTimePrev;
	

	protected override void Awake()
	{
		base.Awake();

		audioSource = GetComponent<AudioSource>();
		notePlayCounts = new int[notes.Length];
		stick.gameObject.SetActive(false);
	}


	void Update()
	{
		if (!taskActive || !GameManager.Instance.gameActive) return;
		
		// Anim
		if (animQueue.Count > 0)
		{
			AnimState state = animQueue.Peek();
			if (state.t == 0)
			{
				state.pStart = stick.transform.position;
				state.rStart = stick.transform.rotation;
			}
			state.t += Time.deltaTime * animSpeed;
			
			float yAdd = 1 - Mathf.Pow(2 * Mathf.Clamp01(state.t) - 1, 2);
			stick.transform.position = Vector3.Lerp(state.pStart, state.p, Maths.EaseQuadIn(state.t));
			stick.transform.position += Vector3.up * (yAdd * yAddMul);

			if (state.t >= 0.7f && !state.playedSound)
			{
				state.playedSound = true;
				PlayNoteSound(state.noteIndex);
				
			}
			
			if (state.t >= 1)
			{
				notePlayCounts[state.noteIndex]++;
				animQueue.Dequeue();
			}
		}

		// ---------- Play notes
		
		Vector3 hitPos;
		int selectedNoteIndex = GetNearestMouseOverIndex(notes, out hitPos);

		if (Input.GetMouseButtonDown(0) && selectedNoteIndex != -1)
		{
			AnimState animState = new()
			{
				noteIndex = selectedNoteIndex,
				p = Maths.ClosestPointOnLineSegment(hitPos, hitLineA.position, hitLineB.position),
			};
			animQueue.Enqueue(animState);
			
			//PlayNoteSound(selectedNoteIndex);
			//notePlayCounts[selectedNoteIndex]++;

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
			scoreUI.text = $"{scoreInt}%";
		}

		if (scoreInt >= 100 && !taskCompleted)
		{
			scoreUI.color = textColDone;
			TaskCompleted();
		}

		// -------- Exit
		if (Input.GetKeyDown(GameManager.TaskEnterKey) && Time.frameCount > enterFrame)
		{
			ExitTask();
		}
	}

	class AnimState
	{
		public float t;
		public Vector3 p;
		public int noteIndex;
		public Vector3 pStart;
		public Quaternion rStart;
		public bool playedSound;
	}

	public override void EnterTask()
	{
		base.EnterTask();
		scoreUI.gameObject.SetActive(true);
		stick.gameObject.SetActive(true);
		stickPreview.gameObject.SetActive(false);
	}

	public override void ExitTask()
	{
		base.ExitTask();
		scoreUI.gameObject.SetActive(false);
		
		stick.gameObject.SetActive(false);
		stickPreview.gameObject.SetActive(true);
	}

	public override void Playback(float playTime)
	{
		foreach (var frame in keyframes)
		{
			if (frame.time > playbackTimePrev && frame.time < playTime)
			{
				Debug.Log("playback note: " + frame.noteIndex + "  " + playTime + "   " + frame.time);
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