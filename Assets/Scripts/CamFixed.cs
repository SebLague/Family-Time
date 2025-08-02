using UnityEngine;

public class CamFixed : MonoBehaviour
{
	public float maxAngle;
	public float smoothTime;
	Quaternion baseRot;
	public bool active = true;

	void Start()
	{
		baseRot = transform.rotation;
	}

	void Update()
	{
		if (!active) return;
		
		Vector2 mousePos = Input.mousePosition;
		Vector2 screenCentre = new Vector2(Screen.width, Screen.height);
		float tx = Mathf.InverseLerp(0, screenCentre.x, mousePos.x);
		float ty = Mathf.InverseLerp(0, screenCentre.y, mousePos.y);

		Vector2 offset = new Vector2(tx - 0.5f, ty - 0.5f);
		
		if (mousePos.x < 0 ||  mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height) offset = Vector2.zero;

		float rotX = Mathf.Clamp(-offset.y * maxAngle, -maxAngle, maxAngle);
		float rotY = Mathf.Clamp(offset.x * maxAngle, -maxAngle, maxAngle);
		Quaternion targetRot = baseRot * Quaternion.Euler(rotX, rotY, 0f);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smoothTime);
	}

	public void UpdateBaseRot()
	{
		baseRot = transform.rotation;
	}
}