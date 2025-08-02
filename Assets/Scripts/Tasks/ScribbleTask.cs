using System.Collections.Generic;
using Seb.Helpers;
using UnityEngine;

public class ScribbleTask : Task
{
	[Header("Scribble")]
	public int texRes = 512;

	public float upDst;
	public float downDel;
	public MeshRenderer paper;
	public BoxCollider paperBoxCollider;
	public ComputeShader scribbleCompute;
	public BoxCollider[] crayons;
	public Color[] crayonCols;
	public TMPro.TMP_Text scoreUI;
	public TMPro.TMP_Text instructUI;
	public Color textColDone;
	RenderTexture tex;

	GameObject activeCrayon;
	int activeCrayonIndex = -1;
	Vector4[,] imageScoreMap = new Vector4[32, 32];
	float currScoreT;
	Color activeCol;
	Vector2 uvCurr;
	Vector3[] posRestore;
	Quaternion[] rotRestore;

	public static List<ScribbleKeyframe> keyframes = new();

	protected override void Awake()
	{
		base.Awake();

		float pageWidth = paper.transform.localScale.z;
		float pageHeight = paper.transform.localScale.x;
		tex = ComputeHelper.CreateRenderTexture(Mathf.CeilToInt(texRes * (pageWidth / pageHeight)), texRes);
		paper.sharedMaterial.mainTexture = tex;
		scribbleCompute.SetTexture(0, "Tex", tex);
		scribbleCompute.SetTexture(1, "Tex", tex);
		scribbleCompute.SetInts("Res", tex.width, tex.height);
		ComputeHelper.Dispatch(scribbleCompute, tex.width, tex.height, kernelIndex: 0);

		posRestore = new Vector3[crayons.Length];
		rotRestore = new Quaternion[crayons.Length];
		for (int i = 0; i < crayons.Length; i++)
		{
			posRestore[i] = crayons[i].transform.position;
			rotRestore[i] = crayons[i].transform.rotation;
		}
	}

	void Update()
	{
		if (!taskActive || !GameManager.Instance.gameActive) return;


		// --------- Crayon selection
		int nearestCrayonIndex = GetNearestMouseOverIndex(crayons);

		if (Input.GetMouseButtonDown(0) && nearestCrayonIndex != -1)
		{
			ReturnCrayon();
			TakeCrayon(nearestCrayonIndex);
		}

		// ------------- Draw
		bool mouseIsDown = Input.GetMouseButton(0);
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);

		if (activeCrayon != null)
		{
			RaycastHit hitAny;
			Physics.Raycast(ray, out hitAny);
			if (hitAny.collider != null)
			{
				activeCrayon.transform.localEulerAngles = new Vector3(150, 0, 0);
				activeCrayon.transform.position = hitAny.point + activeCrayon.transform.up * (upDst + (mouseIsDown ? downDel : 0));
			}
		}

		paperBoxCollider.Raycast(ray, out RaycastHit hitInfo, 100);
		if (hitInfo.collider != null && activeCrayon != null)
		{
			Vector3 pLocal = paper.transform.InverseTransformPoint(hitInfo.point);
			Vector2 pLocal2D = new Vector2(pLocal.z, pLocal.x);
			uvCurr = pLocal2D + Vector2.one * 0.5f;

			if (mouseIsDown)
			{
				scribbleCompute.SetVector("drawUV", uvCurr);
				ComputeHelper.Dispatch(scribbleCompute, tex.width, tex.height, kernelIndex: 1);

				// ------ Scoring
				int px = Mathf.Clamp((int)(uvCurr.x * imageScoreMap.GetLength(0)), 0, imageScoreMap.GetLength(0) - 1);
				int py = Mathf.Clamp((int)(uvCurr.y * imageScoreMap.GetLength(1)), 0, imageScoreMap.GetLength(1) - 1);
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

				float fillScoreT = Maths.EaseQuadIn(Mathf.Clamp01(fillCount / 200f));
				float secondMostUsedCol = GetSecondHighest(colCounts);
				fillScoreT *= Mathf.Lerp(0.3f, 1, secondMostUsedCol / 80f);
				const int colScoreMax = 40;
				float colScoreT = Vector4.Dot(Vector4.Min(Vector4.one * colScoreMax, colCounts), Vector4.one * (1f / colScoreMax)) / 4f;
				currScoreT = fillScoreT * 0.3f + colScoreT * 0.7f;
			}
		}

		int scoreInt = Mathf.CeilToInt(currScoreT * 100);
		if (Application.isEditor && Input.GetKeyDown(KeyCode.Q)) scoreInt = 100;

		if (!taskCompleted)
		{
			scoreUI.text = $"scribble satisfaction: {scoreInt}%";
		}

		if (scoreInt >= 100 && !taskCompleted)
		{
			scoreUI.color = textColDone;
			instructUI.text = "F to exit";
			TaskCompleted();
		}


		// -------- Exit
		if (Input.GetKeyDown(GameManager.TaskEnterKey) && Time.frameCount > enterFrame)
		{
			ExitTask();
		}

		// Record
		float timeBetweenKeyframes = 1 / 30f;
		if (keyframes.Count == 0 || GameManager.Instance.playerTimer - keyframes[^1].time > timeBetweenKeyframes)
		{
			ScribbleKeyframe frame = new()
			{
				time = GameManager.Instance.playerTimer,
				col = activeCol,
				uv = uvCurr,
				mouseIsDown = mouseIsDown
			};

			keyframes.Add(frame);
		}
	}

	void ReturnCrayon()
	{
		if (activeCrayon != null)
		{
			activeCrayon.SetActive(true);
			crayons[activeCrayonIndex].enabled = true;
			activeCrayon.transform.position =  posRestore[activeCrayonIndex];
			activeCrayon.transform.rotation = rotRestore[activeCrayonIndex];
		}

		activeCrayonIndex = -1;
		activeCrayon = null;
	}

	void TakeCrayon(int index)
	{
		crayons[index].enabled = false;
		activeCrayonIndex = index;
		activeCrayon = crayons[index].gameObject;
		activeCol = crayonCols[index];
		scribbleCompute.SetVector("drawCol", activeCol);
		//activeCrayon.gameObject.SetActive(false);
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

	public override void Playback(float playTime)
	{
		if (keyframes.Count <= 1) return;

		int prevIndex = 0;
		int nextIndex = keyframes.Count - 1;
		int i = (nextIndex) / 2;
		int safety = 10000;

		while (true)
		{
			// t lies to left
			if (playTime <= keyframes[i].time)
			{
				nextIndex = i;
			}
			// t lies to right
			else
			{
				prevIndex = i;
			}

			i = (nextIndex + prevIndex) / 2;

			if (nextIndex - prevIndex <= 1)
			{
				break;
			}

			safety--;
			if (safety <= 0)
			{
				Debug.Log("Fix me!");
				return;
			}
		}


		ScribbleKeyframe frameA = keyframes[prevIndex];
		ScribbleKeyframe frameB = keyframes[nextIndex];
		float abPercent = Mathf.InverseLerp(frameA.time, frameB.time, playTime);

		if (frameA.mouseIsDown && frameB.mouseIsDown)
		{
			scribbleCompute.SetVector("drawCol", frameA.col);
			scribbleCompute.SetVector("drawUV", Vector2.Lerp(frameA.uv, frameB.uv, abPercent));
			ComputeHelper.Dispatch(scribbleCompute, tex.width, tex.height, kernelIndex: 1);
		}
	}

	public struct ScribbleKeyframe
	{
		public float time;
		public Vector2 uv;
		public Color col;
		public bool mouseIsDown;
	}
}