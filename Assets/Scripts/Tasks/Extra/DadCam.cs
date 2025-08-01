using UnityEngine;

public class DadCam : MonoBehaviour
{
	bool equipped;


	public void Equip()
	{
		equipped = true;
		GetComponent<SphereCollider>().enabled = false;
		gameObject.SetActive(false);
	}
}