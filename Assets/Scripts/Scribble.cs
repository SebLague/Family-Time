using Seb.Helpers;
using UnityEngine;

public class Scribble : MonoBehaviour
{
	public bool taskActive;
	public int texRes = 512;

	public MeshRenderer paper;
	public BoxCollider paperBoxCollider;
	public ComputeShader scribbleCompute;
	public Camera cam;
	RenderTexture tex;

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
	}

	void Update()
	{
		if (!taskActive) return;

		
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);

		RaycastHit hitInfo;
		paperBoxCollider.Raycast(ray, out hitInfo, 100);
		if (hitInfo.collider != null)
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
	}
}