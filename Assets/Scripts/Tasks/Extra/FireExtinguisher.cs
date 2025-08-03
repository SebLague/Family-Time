using System.Collections.Generic;
using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
	public FetchTask task;
	public FirstPersonController mom;
	public Transform equipPos;
	public Transform nozzle;
	public Foam foamPrefab;
	public float timeBetween;
	float lastShootTime;
	bool equipped;
	
	AudioSource audioSource;

	public static List<float> shootTimes = new();
	float playbackTimePrev;
	int playbackIndexPrev;
	float targetVol;
	public float volumeMax;
	public float volDecay;
	public float volResponseSpeed;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(KeyCode.PageUp) && mom.isControllable)
		{
			Debug.Log("Dev equip extinguisher");
			Equip();
		}

		if (equipped && Input.GetMouseButton(0))
		{
			Shoot();
		}
		
		// vol
		targetVol -= Time.deltaTime * volDecay;
		targetVol = Mathf.Clamp01(targetVol);
		audioSource.volume = Mathf.Lerp(audioSource.volume, targetVol*volumeMax, Time.deltaTime * volResponseSpeed);
		
	}

	void LateUpdate()
	{
		if (equipped)
		{
			transform.position = equipPos.position;
			transform.rotation = equipPos.rotation;
		}
	}

	void Shoot(bool isPlayback = false)
	{
		if (Time.time - lastShootTime >= timeBetween || isPlayback)
		{
			Foam foam = Instantiate(foamPrefab, nozzle.position, nozzle.rotation);
			foam.Init(mom.controller.velocity);
			lastShootTime = Time.time;
			targetVol = 1;

			if (!isPlayback) shootTimes.Add(GameManager.Instance.playerTimer);
		}
	}

	public void Playback(float playTime)
	{
		int s = playbackIndexPrev;
		for (int i = s; i < shootTimes.Count; i++)
		{
			if (shootTimes[i] > playbackTimePrev && shootTimes[i] < playTime)
			{
				playbackIndexPrev = i;
				Shoot(true);
			}
			else if (shootTimes[i] >= playTime) break;
		}

		playbackTimePrev = playTime;
	}


	public void Equip(bool isPlayback = false)
	{
		if (equipped) return;


		equipped = true;
		//transform.parent = equipPos;
		//transform.localPosition = Vector3.zero;
		//transform.localRotation = Quaternion.identity;
		GetComponent<SphereCollider>().enabled = false;

		if (!isPlayback)
		{
			task.Fetched();
			FindFirstObjectByType<PutOutFiresTask>().OnExtinguisherEquipped();
		}
	}
}