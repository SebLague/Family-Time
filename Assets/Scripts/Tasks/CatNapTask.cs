using Seb.Helpers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CatNapTask : Task
{
	bool hasCat;
	float animStartedTime;
	public GameObject zs;
	public Image snoozeToBlack;
	public Transform spotHolder;
	Transform closestTransform;
	public float camUp;
	public float camFwd;
	protected override void Awake()
	{
		base.Awake();

		foreach (Transform child in spotHolder)
		{
			var s = gameObject.AddComponent<SphereCollider>();
			s.isTrigger = true;
			s.radius = child.localScale.x;
			s.center = child.localPosition;
		}
	}

	public override void EnterTask()
	{
		base.EnterTask();

		owner.gameObject.SetActive(true);
		owner.SetControllable(false);
		owner.skinnedMesh.shadowCastingMode = ShadowCastingMode.On;
		
		owner.CatSnooze();

		hasCat = true;
		animStartedTime = Time.time;
		GameManager.Instance.ignoreTimer = true;
		zs.SetActive(true);

		
		float closestDst = float.MaxValue;
		closestTransform = spotHolder.GetChild(0);
		foreach (Transform child in spotHolder)
		{
			float dst =  Vector3.Distance(child.position, owner.transform.position);
			if (dst < closestDst)
			{
				closestDst = dst;
				closestTransform = child;
			}
		}
		
		
		
	}

	void Update()
	{
		if (hasCat)
		{
			owner.transform.position = closestTransform.position;
			owner.transform.rotation = closestTransform.rotation;
			cam.transform.position = closestTransform.position + closestTransform.GetChild(0).forward * camFwd + Vector3.up * camUp;
			cam.transform.LookAt(closestTransform.position);
			
			const float duration = 5;


			float animTime = Time.time - animStartedTime;
			float t = animTime / duration;
			snoozeToBlack.color = new Color(0, 0, 0, Maths.EaseQuadInOut(t) * 0.9f);
			//zs.transform.forward = Vector3.up;
			if (animTime >= duration)
			{
				snoozeToBlack.color = Color.clear;
				TaskCompleted();
			}
		}
	}
}