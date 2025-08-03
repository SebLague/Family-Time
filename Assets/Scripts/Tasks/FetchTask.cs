using UnityEngine;

public class FetchTask : Task
{
	public bool selfFetch;
	public GameObject hideOnSelfFetch;
	public Sfx completedSfx;

	void Update()
	{
		if (selfFetch && ownerInRegion && Input.GetKeyDown(GameManager.PickupKey))
		{
			hideOnSelfFetch?.SetActive(false);
			Fetched();
		}
	}

	public void Fetched()
	{
		if (taskCompleted) return;
		if (completedSfx.clip) AudioSource.PlayClipAtPoint(completedSfx.clip, owner.cam.transform.position, completedSfx.volumeT);
		if (GetComponent<SphereCollider>()) GetComponent<SphereCollider>().enabled = false;
		TaskCompleted();
	}
}