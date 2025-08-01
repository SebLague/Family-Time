using UnityEngine;

public class FlyController : MonoBehaviour
{
	public Transform[] wings;
	public float flapSpeed;
	public float flapMag;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		foreach (Transform t in wings)
		{
			Vector3 e = t.localEulerAngles;
			e.x = (Mathf.Sin(Time.time * flapSpeed) * flapMag);
			t.localEulerAngles = e;
		}
	}
}