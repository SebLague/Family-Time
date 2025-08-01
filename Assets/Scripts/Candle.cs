using System;
using UnityEngine;

public class Candle : MonoBehaviour
{
	public Rigidbody rb;
	public float force;
	public float forceUp;
	bool hasApplied;
	public Transform[] directions;
	public GameObject firePrefab;

	float fireStartDelay = 2;
	float fireStartTime;
	bool hasStartedFire;
	public LayerMask foamLayer;
	float nextFoamTestTime;
	ParticleSystem fireParticles;
	int extinguishCounter;
	bool extinguished;
	bool notifiedExtinguish;
	float extinguishTime;
	readonly Collider[] foamBuffer = new Collider[32];

	void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(KeyCode.PageDown))
		{
			ApplyForceDir(directions[0].forward);
		}

		if (hasApplied && Time.time > fireStartTime && !hasStartedFire)
		{
			hasStartedFire = true;
			Vector3 v = rb.ClosestPointOnBounds(rb.position + Vector3.down * 10);
			GameObject fireInst = Instantiate(firePrefab, v, Quaternion.Euler(-90, 0, 0));
			fireParticles = fireInst.GetComponent<ParticleSystem>();
			GetComponent<SphereCollider>().radius *= 2;
		}

		// ---------- put out fire task (mother)
		if (hasStartedFire && Time.time > nextFoamTestTime && !extinguished)
		{
			int n = Physics.OverlapSphereNonAlloc(fireParticles.transform.position, 0.7f, foamBuffer, foamLayer, QueryTriggerInteraction.Collide);
			for (int i = 0; i < n; i++)
			{
				extinguishCounter++;
				foamBuffer[i].gameObject.GetComponent<Foam>().OnFireSlurp();
			}

			nextFoamTestTime = Time.time + 0.5f;
		}

		if (hasStartedFire && !extinguished && extinguishCounter > 8)
		{
			extinguished = true;
			fireParticles.Stop();
			extinguishTime = Time.time;
		}

		if (extinguished && !notifiedExtinguish && Time.time > extinguishTime + 4)
		{
			notifiedExtinguish = true;
			if (GameManager.Instance.currentPlayer.playerType == GameManager.Players.Mother)
			{
				FindFirstObjectByType<PutOutFiresTask>().NotifyFireOut();
			}
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

	public void ApplyForceDir(Vector3 dir, bool notify = true)
	{
		hasApplied = true;
		fireStartTime = Time.time + fireStartDelay;
		rb.AddForce(dir * force + Vector3.up * forceUp, ForceMode.Impulse);

		if (notify) FindFirstObjectByType<CandleTask>().OnTopple(this, dir);
	}
}