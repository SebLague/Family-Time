using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
	bool lockCursor = true;
	public float smoothVelTAir;
	public float smoothVelTGround;
	public float gravity = 12f;
	public float jumpForce = 20;
	public float moveSpeed = 5;
	public Vector2 mouseSensitivity;
	public Vector2 verticalLookMinMax;
	public Transform cam;
	CharacterController controller;
	public Animator animator;
	float pitch;
	float velocityY;
	Vector3 velocity;
	Vector3 velSmoothRef;

	public TMP_Text infoUI;
	Task potentialTask;

	void Start()
	{
		controller = GetComponent<CharacterController>();

		infoUI.text = "";
	}


	void Update()
	{
		UpdateController();

		if (Input.GetKeyDown(KeyCode.Tab) && potentialTask != null)
		{
			potentialTask.EnterTask(this);
			infoUI.text = "";
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<Task>())
		{
			Task task = other.gameObject.GetComponent<Task>();
			potentialTask = task;
			infoUI.text = task.infoString;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.GetComponent<Task>())
		{
			infoUI.text = "";
			potentialTask = null;
		}
	}

	void UpdateController()
	{
		Vector3 moveDirLocal = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		if (controller.isGrounded)
		{
			velocityY = 0;

			if (Input.GetKeyDown(KeyCode.Space))
			{
				velocityY = jumpForce;
			}
		}

		transform.Rotate(Vector3.up * mouseInput.x * mouseSensitivity.x);
		pitch += mouseInput.y * mouseSensitivity.y;
		pitch = ClampAngle(pitch, verticalLookMinMax.x, verticalLookMinMax.y);
		Quaternion yQuaternion = Quaternion.AngleAxis(pitch, Vector3.left);
		cam.localRotation = yQuaternion;

		velocityY -= gravity * Time.deltaTime;
		Vector3 moveDirWorld = transform.TransformDirection(moveDirLocal);
		Vector3 targetVelocity = moveDirWorld * moveSpeed;
		velocity = Vector3.SmoothDamp(new Vector3(velocity.x, 0, velocity.z), targetVelocity, ref velSmoothRef, controller.isGrounded ? smoothVelTGround : smoothVelTAir);
		velocity.y = velocityY;

		controller.Move(velocity * Time.deltaTime);
		if (animator)
		{
			float fwdSpeedParam = Mathf.Abs(Vector3.Dot(moveDirWorld, transform.forward) * 1.5f);
			float sideSpeedParam = Mathf.Abs(Vector3.Dot(moveDirWorld, transform.right) * 1);
			animator.SetFloat("Speed",fwdSpeedParam);
			animator.speed = 1;

			if (fwdSpeedParam < 0.1f)
			{
				animator.speed = 1 + sideSpeedParam*2.5f;
			}
		}

		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
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
}