using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Baby : MonoBehaviour
{
	public bool lockCursor;
	public float smoothVelTAir;
	public float smoothVelTGround;
	public float gravity = 12f;
	public float jumpForce = 20;
	public float moveSpeed = 5;
	public Vector2 mouseSensitivity;
	public Vector2 verticalLookMinMax;
	public Transform cam;
	CharacterController controller;
	float pitch;
	float velocityY;
	Vector3 velocity;
	Vector3 velSmoothRef;

	void Start()
	{
		controller = GetComponent<CharacterController>();

		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}


	void Update()
	{
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
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
		Vector3 targetVelocity = transform.TransformDirection(moveDir) * moveSpeed;
		velocity = Vector3.SmoothDamp(new Vector3(velocity.x,0, velocity.z), targetVelocity, ref velSmoothRef, controller.isGrounded ? smoothVelTGround : smoothVelTAir);
		velocity.y = velocityY;
		
		controller.Move(velocity * Time.deltaTime);
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