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

	void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(KeyCode.PageUp))
		{
			Debug.Log("Dev equip extinguisher");
			Equip();
		}

		if (equipped && Input.GetMouseButton(0))
		{
			Shoot();
		}
	}

	void Shoot()
	{
		if (Time.time - lastShootTime >= timeBetween)
		{
			Foam foam = Instantiate(foamPrefab, nozzle.position, nozzle.rotation);
			foam.Init(mom.controller.velocity);
			lastShootTime = Time.time;
		}
	}

	public void Equip()
	{
		if (equipped) return;
		
		task.Fetched();
		equipped = true;
		transform.parent = equipPos;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		GetComponent<SphereCollider>().enabled = false;
	}
}