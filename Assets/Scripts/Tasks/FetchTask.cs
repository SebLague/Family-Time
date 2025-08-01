using UnityEngine;

public class FetchTask : Task
{
	public bool selfFetch;


	void Update()
	{
		if (selfFetch && ownerInRegion && Input.GetKeyDown(GameManager.PickupKey))
		{
			Fetched();
		}
	}

	public void Fetched()
	{
		TaskCompleted();
	}
}