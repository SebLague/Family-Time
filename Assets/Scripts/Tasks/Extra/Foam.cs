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
	Vector3 fadeStartScale;
	Vector3 hitPointLocal;
	Transform hitTransform;
	

	public void Init(Vector3 v)
	{
		transform.eulerAngles += new Vector3(Random.value, Random.value, Random.value) * 3;
		transform.localScale = Vector3.zero;
		vel = transform.forward * speed + Vector3.up * upBoost + v;
	}

	void Update()
	{
		if (hasHit)
		{
			transform.position = hitTransform.TransformPoint(hitPointLocal);
			fadeTimer += Time.deltaTime;
			float fadeT = Maths.EaseQuadIn(fadeTimer / 3);
			transform.localScale = fadeStartScale * (1 - fadeT);
			if (fadeT >= 1)
			{
				Destroy(gameObject);
			}
		}
		else
		{
			transform.localScale = Vector3.one * (Maths.EaseQuadInOut(lifeTime * 6) * 1);
			vel -= Vector3.up * (gravity * Time.deltaTime);
			transform.position += vel * Time.deltaTime;
			lifeTime += Time.deltaTime;

			RaycastHit hitInfo;
			if (Physics.Linecast(transform.position - vel * (Time.deltaTime * 3), transform.position + vel * (Time.deltaTime * 2), out hitInfo, layerMask, QueryTriggerInteraction.Ignore))
			{
				hitTransform = hitInfo.transform;
				hitPointLocal = hitTransform.InverseTransformPoint(hitInfo.point);
				hasHit = true;
				fadeStartScale = transform.localScale;
			}
		}
	}
}