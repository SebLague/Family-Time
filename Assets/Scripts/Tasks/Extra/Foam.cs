using Seb.Helpers;
using UnityEngine;

public class Foam : MonoBehaviour
{
	public float speed;
	public float gravity;
	public float upBoost;
	Vector3 vel;
	float lifeTime;
	public LayerMask layerMask;
	bool hasHit;
	float fadeTimer;
	Vector3 hitPointLocal;
	Transform hitTransform;
	float localTimeScale = 1;

	public void Init(Vector3 v)
	{
		transform.eulerAngles += new Vector3(Random.value, Random.value, Random.value) * 3;
		transform.localScale = Vector3.zero;
		vel = transform.forward * speed + Vector3.up * upBoost + v;
	}

	public void OnFireSlurp()
	{
		GetComponent<SphereCollider>().enabled = false;
		localTimeScale = 2;
	}

	void Update()
	{
		lifeTime += Time.deltaTime * localTimeScale;
		float lifetimeT = Maths.EaseQuadInOut(lifeTime * 6);
		transform.localScale = Vector3.one * (lifetimeT * 1);

		if (hasHit)
		{
			if (lifetimeT >= 1)
			{
				transform.position = hitTransform.TransformPoint(hitPointLocal);
				fadeTimer += Time.deltaTime * localTimeScale;
				float fadeT = Maths.EaseQuadIn(fadeTimer / 3);
				transform.localScale *= (1 - fadeT);
				if (fadeT >= 1)
				{
					Destroy(gameObject);
				}
			}
		}
		else
		{
			vel -= Vector3.up * (gravity * Time.deltaTime);
			transform.position += vel * Time.deltaTime;

			RaycastHit hitInfo;
			if (Physics.Linecast(transform.position - vel * (Time.deltaTime * 3), transform.position + vel * (Time.deltaTime * 2), out hitInfo, layerMask, QueryTriggerInteraction.Ignore))
			{
				hitTransform = hitInfo.transform;
				hitPointLocal = hitTransform.InverseTransformPoint(hitInfo.point);
				hasHit = true;
			}
		}

		if (transform.position.y < -10)
		{
			Destroy(gameObject);
		}
	}
}