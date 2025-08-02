using UnityEngine;

public class FetchTask : Task
{
	public bool selfFetch;
	public GameObject hideOnSelfFetch;


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
		TaskCompleted();
	}
}