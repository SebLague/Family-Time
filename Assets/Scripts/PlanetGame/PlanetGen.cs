using System;
using NoiseTest;
using SebStuff;
using UnityEngine;

public class PlanetGen : MonoBehaviour
{
	public int res;
	public MeshFilter display;
	int resOld = -1;
	public float radius;
	public int noiseSeed;
	public float noiseScale;
	public float noiseStrength;
	public float oceanHeight;

	Vector3[] sphereVerts;
	int[] sphereIndices;

	Vector3[] planetVerts;
	Mesh planetMesh;
	bool needsUpdate;
	OpenSimplexNoise noiseGen;
	public Transform ocean;

	void Start()
	{
		Regenerate();
	}


	void Update()
	{
		if (needsUpdate)
		{
			needsUpdate = false;
			Regenerate();
		}
	}

	void Regenerate()
	{
		noiseGen = new OpenSimplexNoise(noiseSeed);

		if (resOld != res)
		{
			Mesh sphereMesh = SphereGenerator.GenerateSphereMesh(res);
			sphereVerts = sphereMesh.vertices;
			sphereIndices = sphereMesh.triangles;
			planetMesh = new();
			planetMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

			planetVerts = new Vector3[sphereVerts.Length];
			resOld = res;
		}

		for (int i = 0; i < sphereVerts.Length; i++)
		{
			Vector3 spherePos = sphereVerts[i];
			float h = GetNoise(spherePos);
			planetVerts[i] = sphereVerts[i] * (radius + h);
		}

		planetMesh.vertices = planetVerts;
		planetMesh.triangles = sphereIndices;
		planetMesh.RecalculateNormals();
		planetMesh.RecalculateBounds();

		display.sharedMesh = planetMesh;
		ocean.localScale = Vector3.one * (radius + oceanHeight);
	}

	float GetNoise(Vector3 p)
	{
		float h = 0;

		p *= noiseScale;
		h = (float)noiseGen.Evaluate(p.x, p.y, p.z);

		return h * noiseStrength;
	}

	void OnValidate()
	{
		needsUpdate = true;
	}
}