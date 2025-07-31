using Seb.Helpers;
using UnityEngine;

public class ScribbleTask : Task
{
	[Header("Scribble")]
	public int texRes = 512;

	public MeshRenderer paper;
	public BoxCollider paperBoxCollider;
	public ComputeShader scribbleCompute;
	public BoxCollider[] crayons;
	public TMPro.TMP_Text scoreUI;
	public Color textColDone;
	RenderTexture tex;

	GameObject activeCrayon;
	int activeCrayonIndex = -1;
	Vector4[,] imageScoreMap = new Vector4[32, 32];
	float currScoreT;

	void Awake()
	{
		float pageWidth = paper.transform.localScale.z;
		float pageHeight = paper.transform.localScale.x;
		tex = ComputeHelper.CreateRenderTexture(Mathf.CeilToInt(texRes * (pageWidth / pageHeight)), texRes);
		paper.sharedMaterial.mainTexture = tex;
		scribbleCompute.SetTexture(0, "Tex", tex);
		scribbleCompute.SetTexture(1, "Tex", tex);
		scribbleCompute.SetInts("Res", tex.width, tex.height);
		ComputeHelper.Dispatch(scribbleCompute, tex.width, tex.height, kernelIndex: 0);
		
		ExitTask();
	}

	void Update()
	{
		if (!taskActive) return;



		// --------- Crayon selection
		int nearestCrayonIndex = GetNearestMouseOverIndex(crayons);

		if (Input.GetMouseButtonDown(0) && nearestCrayonIndex != -1)
		{
			ReturnCrayon();
			TakeCrayon(nearestCrayonIndex);
		}

		// ------------- Draw
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		paperBoxCollider.Raycast(ray, out RaycastHit hitInfo, 100);
		if (hitInfo.collider != null && activeCrayon != null)
		{
			Vector3 pLocal = paper.transform.InverseTransformPoint(hitInfo.point);
			Vector2 pLocal2D = new Vector2(pLocal.z, pLocal.x);
			Vector2 uv = pLocal2D + Vector2.one * 0.5f;

			if (Input.GetMouseButton(0))
			{
				scribbleCompute.SetVector("drawUV", uv);
				ComputeHelper.Dispatch(scribbleCompute, tex.width, tex.height, kernelIndex: 1);

				// ------ Scoring
				int px = Mathf.Clamp((int)(uv.x * imageScoreMap.GetLength(0)), 0, imageScoreMap.GetLength(0) - 1);
				int py = Mathf.Clamp((int)(uv.y * imageScoreMap.GetLength(1)), 0, imageScoreMap.GetLength(1) - 1);
				int c = activeCrayonIndex;

				Vector4 drawColOnehot = new Vector4(c == 0 ? 1 : 0, c == 1 ? 1 : 0, c == 2 ? 1 : 0, c == 3 ? 1 : 0);
				imageScoreMap[px, py] = drawColOnehot;

				int fillCount = 0;
				Vector4 colCounts = Vector4.zero;

				for (int y = 0; y < imageScoreMap.GetLength(1); y++)
				{
					for (int x = 0; x < imageScoreMap.GetLength(0); x++)
					{
						Vector4 col = imageScoreMap[x, y];
						if (col != Vector4.zero)
						{
							fillCount++;
							colCounts += new Vector4(col.x == 0 ? 0 : 1, col.y == 0 ? 0 : 1, col.z == 0 ? 0 : 1, col.w == 0 ? 0 : 1);
						}
					}
				}

				float fillScoreT = Maths.EaseCubeIn(Mathf.Clamp01(fillCount / 300f));
				float secondMostUsedCol = GetSecondHighest(colCounts);
				fillScoreT *= Mathf.Lerp(0.5f, 1, secondMostUsedCol / 80f);
				const int colScoreMax = 40;
				float colScoreT = Vector4.Dot(Vector4.Min(Vector4.one * colScoreMax, colCounts), Vector4.one * (1f / colScoreMax)) / 4f;
				currScoreT = fillScoreT * 0.3f + colScoreT * 0.7f;
			}
		}

		int scoreInt = Mathf.CeilToInt(currScoreT * 100);
		if (!taskCompleted)
		{
			scoreUI.text = $"scribble score: {scoreInt}%";
		}

		if (scoreInt >= 100 && !taskCompleted)
		{
			taskCompleted = true;
			scoreUI.color = textColDone;
		}


		// -------- Exit
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ExitTask();
		}
	}

	void ReturnCrayon()
	{
		if (activeCrayon != null)
		{
			activeCrayon.SetActive(true);
		}

		activeCrayonIndex = -1;
		activeCrayon = null;
	}

	void TakeCrayon(int index)
	{
		activeCrayonIndex = index;
		activeCrayon = crayons[index].gameObject;
		scribbleCompute.SetVector("drawCol", activeCrayon.GetComponent<MeshRenderer>().sharedMaterial.color);
		activeCrayon.gameObject.SetActive(false);
	}

	public override void EnterTask()
	{
		base.EnterTask();

		scoreUI.gameObject.SetActive(true);
		ReturnCrayon();
	}

	public override void ExitTask()
	{
		base.ExitTask();
		ReturnCrayon();
		scoreUI.gameObject.SetActive(false);
	}

	float GetMax(Vector4 v)
	{
		return Mathf.Max(v.x, v.y, v.z, v.w);
	}

	float GetSecondHighest(Vector4 v)
	{
		float max = GetMax(v);
		for (int i = 0; i < 4; i++)
		{
			if (v[i] == max)
			{
				v[i] = 0;
				break;
			}
		}

		return GetMax(v);
	}
}