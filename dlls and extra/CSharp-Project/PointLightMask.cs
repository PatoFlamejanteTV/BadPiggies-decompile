using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PointLightMask : MonoBehaviour
{
	public enum LightType
	{
		PointLight,
		BeamLight
	}

	public LightType lightType;

	public Transform lightSource;

	public GameObject border;

	public float radius = 1f;

	public int vertexCount = 100;

	public float borderWidth = 0.3f;

	[HideInInspector]
	public MeshFilter meshFilter;

	[HideInInspector]
	public float colliderSize;

	protected float lastRadius;

	protected Mesh mesh;

	protected Mesh borderMesh;

	protected Vector3[] newVertices;

	protected Vector2[] newUV;

	protected int[] newTriangles;

	protected Vector3[] borderVertices;

	protected Vector2[] borderUV;

	protected int[] borderTriangles;

	protected void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		mesh = meshFilter.mesh;
	}

	public virtual void UpdateMesh()
	{
		radius = 1f;
		if (border != null)
		{
			borderMesh = border.GetComponent<MeshFilter>().mesh;
		}
		if (mesh.vertices.Length == vertexCount + 1 && radius == lastRadius)
		{
			return;
		}
		mesh.Clear();
		newVertices = new Vector3[vertexCount + 1];
		newVertices[0] = Vector3.zero;
		newTriangles = new int[(vertexCount - 1) * 3 + 3];
		int num = 1;
		for (int i = 0; i < newTriangles.Length; i += 3)
		{
			newTriangles[i] = 0;
			newTriangles[i + 1] = num + 1;
			newTriangles[i + 2] = num;
			if (i >= (vertexCount - 1) * 3)
			{
				newTriangles[i] = 0;
				newTriangles[i + 1] = 1;
				newTriangles[i + 2] = num;
			}
			else
			{
				num++;
			}
		}
		for (int j = 0; j < vertexCount; j++)
		{
			float f = (float)Math.PI * 2f / (float)vertexCount * (float)j;
			float x = newVertices[0].x + radius * Mathf.Cos(f);
			float y = newVertices[0].y + radius * Mathf.Sin(f);
			newVertices[j + 1] = new Vector3(x, y);
		}
		newUV = new Vector2[newVertices.Length];
		for (int k = 0; k < newUV.Length; k++)
		{
			newUV[k] = new Vector2(newVertices[k].x, newVertices[k].y);
		}
		if (border != null)
		{
			borderVertices = new Vector3[vertexCount + 1];
			borderTriangles = new int[(vertexCount - 1) * 3 + 3];
			num = 1;
			for (int l = 0; l < borderTriangles.Length; l += 3)
			{
				borderTriangles[l] = 0;
				borderTriangles[l + 1] = num + 1;
				borderTriangles[l + 2] = num;
				if (l >= (vertexCount - 1) * 3)
				{
					borderTriangles[l] = 0;
					borderTriangles[l + 1] = 1;
					borderTriangles[l + 2] = num;
				}
				else
				{
					num++;
				}
			}
			for (int m = 0; m < borderVertices.Length; m++)
			{
				borderVertices[m] = newVertices[m];
			}
			borderUV = new Vector2[borderVertices.Length];
			for (int n = 0; n < borderUV.Length; n++)
			{
				borderUV[n] = new Vector2(borderVertices[n].x, borderVertices[n].y);
			}
			borderMesh.vertices = borderVertices;
			borderMesh.uv = borderUV;
			borderMesh.triangles = borderTriangles;
			borderMesh.RecalculateNormals();
			borderMesh.RecalculateBounds();
		}
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		lastRadius = radius;
	}
}
