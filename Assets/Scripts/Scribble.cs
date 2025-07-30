using Seb.Helpers;
using UnityEngine;

public class Scribble : Task
{
	[Header("Scribble")]
	public int texRes = 512;

	public MeshRenderer paper;
	public BoxCollider paperBoxCollider;
	public ComputeShader scribbleCompute;
	public Camera cam;
	public BoxCollider[] crayons;
	RenderTexture tex;
	FirstPersonController controller;

	GameObject activeCrayon;

	void Start()
	{
		float pageWidth = paper.transform.localScale.z;
		float pageHeight = paper.transform.localScale.x;
		tex = ComputeHelper.CreateRenderTexture(Mathf.CeilToInt(texRes * (pageWidth / pageHeight)), texRes);
		paper.sharedMaterial.mainTexture = tex;
		scribbleCompute.SetTexture(0, "Tex", tex);
		scribbleCompute.SetTexture(1, "Tex", tex);
		scribbleCompute.SetInts("Res", tex.width, tex.height);
		ComputeHelper.Dispatch(scribbleCompute, tex.width, tex.height, kernelIndex: 0);
		
		if (!taskActive) cam.gameObject.SetActive(false);
		if (taskActive)
		{
			Baby baby = FindFirstObjectByType<Baby>(FindObjectsInactive.Include);
			EnterTask(baby.gameObject.GetComponent<FirstPersonController>());
		}
	}

	void Update()
	{
		if (!taskActive) return;


		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		// --------- Crayon selection
		int nearestCrayonIndex = -1;
		float nearestCrayonDistance = float.MaxValue;

		for (int i = 0; i < crayons.Length; i++)
		{
			crayons[i].Raycast(ray, out hitInfo, 100);
			if (hitInfo.collider != null)
			{
				if (hitInfo.distance < nearestCrayonDistance)
				{
					nearestCrayonIndex = i;
					nearestCrayonDistance = hitInfo.distance;
				}
			}
		}

		if (Input.GetMouseButtonDown(0) && nearestCrayonIndex != -1)
		{
			if (activeCrayon) activeCrayon.gameObject.SetActive(true);
			activeCrayon = crayons[nearestCrayonIndex].gameObject;
			activeCrayon.SetActive(false);
			scribbleCompute.SetVector("drawCol", activeCrayon.GetComponent<MeshRenderer>().sharedMaterial.color);
		}

		// ------------- Draw
		paperBoxCollider.Raycast(ray, out hitInfo, 100);
		if (hitInfo.collider != null && activeCrayon != null)
		{
			Vector3 pLocal = paper.transform.InverseTransformPoint(hitInfo.point);
			Vector2 pLocal2D = new Vector2(pLocal.z, pLocal.x);
			Vector2 uv = pLocal2D + Vector2.one * 0.5f;

			if (Input.GetMouseButton(0))
			{
				scribbleCompute.SetVector("drawUV", uv);
				ComputeHelper.Dispatch(scribbleCompute, tex.width, tex.height, kernelIndex: 1);
			}
		}
		
		// -------- Exit
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ExitTask();
		}
	}

	public override void EnterTask(FirstPersonController controller)
	{
		this.controller = controller;
		controller.gameObject.SetActive(false);
		taskActive = true;
		cam.gameObject.SetActive(true);
		
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	void ExitTask()
	{
		cam.gameObject.SetActive(false);
		taskActive = false;
		controller.gameObject.SetActive(true);
	}
}