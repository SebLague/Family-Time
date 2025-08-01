using System;
using UnityEngine;

public class Candle : MonoBehaviour
{
	public Rigidbody rb;
	public float force;
	public float forceUp;
	bool hasApplied;
	public Transform[] directions;

	void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(KeyCode.PageDown))
		{
			ApplyForceDir(directions[0].forward);
		}
	}

	public void ApplyForce(Vector3 targDir)
	{
		if (hasApplied) return;
		hasApplied = true;

		Vector3 o = targDir;
		int maxInd = 0;
		float maxAlign = -10;

		for (int i = 0; i < directions.Length; i++)
		{
			Transform dir = directions[i];
			float align = Vector3.Dot(o, dir.forward);
			if (align > maxAlign)
			{
				maxAlign = align;
				maxInd = i;
			}
		}

		ApplyForceDir(directions[maxInd].forward);
	}

	void ApplyForceDir(Vector3 dir)
	{
		rb.AddForce(dir * force + Vector3.up * forceUp, ForceMode.Impulse);
	}
}