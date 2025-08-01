using Seb.Helpers;
using UnityEngine;
using UnityEngine.UI;

public class CatNapTask : Task
{
	bool hasCat;
	float animStartedTime;
	public GameObject zs;
	public Image snoozeToBlack;

	void Awake()
	{
		ExitTask();
	}

	public override void EnterTask()
	{
		base.EnterTask();

		owner.gameObject.SetActive(true);
		owner.SetControllable(false);
		owner.animator.SetBool("Snooze", true);

		hasCat = true;
		animStartedTime = Time.time;
		GameManager.Instance.ignoreTimer = true;
		zs.SetActive(true);
	}

	void Update()
	{
		if (hasCat)
		{
			owner.transform.position = transform.position;
			owner.transform.rotation = transform.rotation;

			const float duration = 50;
			
			
			float animTime = Time.time - animStartedTime;
			float t = animTime / duration;
			snoozeToBlack.color = new Color(0, 0, 0, Maths.EaseQuadInOut(t));
			//zs.transform.forward = Vector3.up;
			if (animTime >= duration)
			{
				TaskCompleted();
			}
		}
	}
}