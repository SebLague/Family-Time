using UnityEngine;

public class DadCam : MonoBehaviour
{
	public FirstPersonController dad;
	public FetchTask task;
	public PicTask picTask;
	bool equipped;

	void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(KeyCode.PageUp) && dad.isControllable)
		{
			Debug.Log("Dev equip cam");
			Equip();
		}
	}


	public void Equip()
	{
		if (equipped) return;

		task.Fetched();
		equipped = true;
		GetComponent<SphereCollider>().enabled = false;
		gameObject.SetActive(false);
		picTask.NotifyPicMode();
	}
}