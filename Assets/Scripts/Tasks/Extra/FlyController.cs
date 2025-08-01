using PathCreation;
using UnityEngine;

public class FlyController : MonoBehaviour
{
	public float speedMax;
	public float speedMin;
	public float speedMulti;

	public Transform[] wings;
	public float flapSpeed;
	public float flapMag;
	public PathCreator pathCreator;
	VertexPath path;
	float dst = 0;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		path = pathCreator.path;
	}

	// Update is called once per frame
	void Update()
	{
		if (!GameManager.Instance.gameActive) return;

		foreach (Transform t in wings)
		{
			Vector3 e = t.localEulerAngles;
			e.x = (Mathf.Sin(Time.time * flapSpeed) * flapMag);
			t.localEulerAngles = e;
		}


		transform.rotation = path.GetRotationAtDistance(dst, EndOfPathInstruction.Loop);
		transform.position = path.GetPointAtDistance(dst, EndOfPathInstruction.Loop);

		float speedT = 1 - Mathf.InverseLerp(-0.3f, 0.3f, transform.forward.y);
		float speed = Mathf.Lerp(speedMin, speedMax, speedT) * speedMulti;
		dst += speed * Time.deltaTime;
	}

	public void Catch()
	{
		FindFirstObjectByType<CatCatchTask>().Caught(this);
		gameObject.SetActive(false);
	}
}