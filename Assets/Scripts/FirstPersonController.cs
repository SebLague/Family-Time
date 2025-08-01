using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
	public GameManager.Players playerType;
	public Task[] tasks;


	public float smoothVelTAir;
	public float smoothVelTGround;
	public float gravity = 12f;
	public float jumpForce = 20;
	public float moveSpeed = 5;
	public float sprintSpeed = 5;
	public Vector2 mouseSensitivity;
	public Vector2 verticalLookMinMax;
	public Camera cam;
	CharacterController controller;
	public Animator animator;
	public SkinnedMeshRenderer skinnedMesh;
	public float fov = 60;
	public float fovSprint = 70;

	float fovCur;
	float fovSmoothRef;
	float pitch;
	float velocityY;
	Vector3 velocity;
	Vector3 velSmoothRef;
	bool isControllable;

	public TMP_Text infoUI;
	Task potentialTask;
	float animSpeed;
	float animTimeScale = 1;
	bool lockCursor = true;
	float timeSinceLastGrounded;
	GameManager manager;


	[HideInInspector] public List<PlaybackKeyframe> playbackKeyframes = new();


	void Start()
	{
		fovCur = fov;
		controller = GetComponent<CharacterController>();

		infoUI.text = "";
		manager = FindFirstObjectByType<GameManager>();
	}

	public void NotifyTaskCompleted(bool forceAllDone = false)
	{
		bool done = tasks.All(t => t.taskCompleted);
		done |= forceAllDone;
		UpdateGoalHud();

		if (done)
		{
			Debug.Log("All Tasks Completed");
			manager.NotifyAllTasksCompleted();
		}
	}

	public void SetControllable(bool state)
	{
		manager = FindFirstObjectByType<GameManager>();
		isControllable = state;

		if (isControllable)
		{
			manager.playerGoalUI.gameObject.SetActive(true);
			UpdateGoalHud();
		}
		else
		{
			manager.playerGoalUI.gameObject.SetActive(false);
		}
	}

	public void NotifyTaskProgress()
	{
		UpdateGoalHud();
	}

	void UpdateGoalHud()
	{
		string goalText = "Goals:\n";
		foreach (Task task in tasks)
		{
			goalText += task.GetGoalString() + "\n";
		}

		manager.playerGoalUI.text = goalText;
	}


	void Update()
	{
		cam.gameObject.SetActive(isControllable);
		if (!isControllable || !manager.gameActive) return;

		UpdateController();
		if (Application.isEditor && Input.GetKeyDown(KeyCode.Delete)) NotifyTaskCompleted(true);

		if (Input.GetKeyDown(KeyCode.Tab) && potentialTask != null)
		{
			potentialTask.EnterTask();
			infoUI.text = "";
		}

		float timeBetweenKeyframes = 1 / 30f;
		if (playbackKeyframes.Count == 0 || GameManager.Instance.playerTimer - playbackKeyframes[^1].time > timeBetweenKeyframes)
		{
			PlaybackKeyframe frame = new PlaybackKeyframe()
			{
				time = GameManager.Instance.playerTimer,
				pos = transform.position,
				rot = transform.rotation,
				animSpeed = animSpeed,
				animTimescale = animTimeScale,
			};

			playbackKeyframes.Add(frame);
		}
	}

	public static string TextCol(string s, Color col, bool setCol)
	{
		if (!setCol) return s;
		return $"<color=#{ColorUtility.ToHtmlStringRGB(col)}>{s}</color>";
	}

	void OnTriggerEnter(Collider other)
	{
		if (!isControllable || !GameManager.Instance.gameActive) return;
		if (other.gameObject.GetComponent<Task>())
		{
			Task task = other.gameObject.GetComponent<Task>();
			if (task.owner == this)
			{
				potentialTask = task;
				infoUI.text = task.infoString;
			}
		}

		if (playerType == GameManager.Players.Cat && other.gameObject.GetComponent<Candle>())
		{
			other.gameObject.GetComponent<Candle>().ApplyForce(cam.transform.forward);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!isControllable || !GameManager.Instance.gameActive) return;

		if (other.gameObject.GetComponent<Task>())
		{
			Task task = other.gameObject.GetComponent<Task>();
			if (task.owner == this)
			{
				infoUI.text = "";
				potentialTask = null;
			}
		}
	}

	void UpdateController()
	{
		Vector3 moveDirLocal = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		if (controller.isGrounded)
		{
			timeSinceLastGrounded = 0;
			velocityY = -0.5f;
		}
		else
		{
			timeSinceLastGrounded += Time.deltaTime;
		}

		if (controller.isGrounded || timeSinceLastGrounded < 0.1f)
		{
			if (Input.GetKeyDown(KeyCode.Space) && velocity.y <= 0)
			{
				velocityY = jumpForce;
			}
		}

		transform.Rotate(Vector3.up * mouseInput.x * mouseSensitivity.x);
		pitch += mouseInput.y * mouseSensitivity.y;
		pitch = ClampAngle(pitch, verticalLookMinMax.x, verticalLookMinMax.y);
		Quaternion yQuaternion = Quaternion.AngleAxis(pitch, Vector3.left);
		cam.transform.localRotation = yQuaternion;

		velocityY -= gravity * Time.deltaTime;
		Vector3 moveDirWorld = transform.TransformDirection(moveDirLocal);
		bool sprint = Input.GetKey(KeyCode.LeftShift);
		Vector3 targetVelocity = moveDirWorld * (sprint ? sprintSpeed : moveSpeed);
		velocity = Vector3.SmoothDamp(new Vector3(velocity.x, 0, velocity.z), targetVelocity, ref velSmoothRef, controller.isGrounded ? smoothVelTGround : smoothVelTAir);
		velocity.y = velocityY;

		controller.Move(velocity * Time.deltaTime);

		animTimeScale = 1.3f;
		animSpeed = 0;

		if (animator)
		{
			float fwdSpeedParam = Mathf.Abs(Vector3.Dot(moveDirWorld, transform.forward) * 1.5f);
			float sideSpeedParam = Mathf.Abs(Vector3.Dot(moveDirWorld, transform.right) * 1);
			animSpeed = fwdSpeedParam;

			if (fwdSpeedParam < 0.1f)
			{
				animTimeScale = 1 + sideSpeedParam * 2.5f;
				if (sprint) animTimeScale *= 1.7f;
			}
			else
			{
				if (sprint) animTimeScale = 1.7f;
			}

			// Set anim params
			animator.SetFloat("Speed", animSpeed);
			animator.speed = animTimeScale;
		}

		if (skinnedMesh != null)
		{
			skinnedMesh.shadowCastingMode = cam.transform.forward.y > 0 ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On;
		}

		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		float fovTarget = moveDirLocal.z > 0 && sprint ? fovSprint : fov;
		fovCur = Mathf.SmoothDamp(fovCur, fovTarget, ref fovSmoothRef, 0.2f);
		cam.fieldOfView = fovCur;
	}

	public void PlaybackUpdate(float playTime)
	{
		if (playbackKeyframes.Count <= 1) return;

		int prevIndex = 0;
		int nextIndex = playbackKeyframes.Count - 1;
		int i = (nextIndex) / 2;
		int safety = 10000;

		while (true)
		{
			// t lies to left
			if (playTime <= playbackKeyframes[i].time)
			{
				nextIndex = i;
			}
			// t lies to right
			else
			{
				prevIndex = i;
			}

			i = (nextIndex + prevIndex) / 2;

			if (nextIndex - prevIndex <= 1)
			{
				break;
			}

			safety--;
			if (safety <= 0)
			{
				Debug.Log("Fix me!");
				return;
			}
		}


		PlaybackKeyframe frameA = playbackKeyframes[prevIndex];
		PlaybackKeyframe frameB = playbackKeyframes[nextIndex];
		float abPercent = Mathf.InverseLerp(frameA.time, frameB.time, playTime);

		transform.position = Vector3.Lerp(frameA.pos, frameB.pos, abPercent);
		transform.rotation = Quaternion.Slerp(frameA.rot, frameB.rot, abPercent);

		if (animator)
		{
			animator.speed = Mathf.Lerp(frameA.animTimescale, frameB.animTimescale, abPercent);
			animator.SetFloat("Speed", Mathf.Lerp(frameA.animSpeed, frameB.animSpeed, abPercent));
		}
	}

	static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
			angle += 360f;
		if (angle > 360f)
			angle -= 360f;
		return Mathf.Clamp(angle, min, max);
	}

	public struct PlaybackKeyframe
	{
		public float time;
		public Vector3 pos;
		public Quaternion rot;
		public float animSpeed;
		public float animTimescale;
	}
}