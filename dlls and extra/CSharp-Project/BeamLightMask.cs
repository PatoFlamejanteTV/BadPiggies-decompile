using System;
using UnityEngine;

public class BeamLightMask : PointLightMask
{
	public float angle = 90f;

	public float cutHeight = 1f;

	public Vector3 arcCenter;

	private float lastAngle;

	private float lastCutHeight;

	private void Update()
	{
		UpdateMesh();
	}

	public override void UpdateMesh()
	{
		if (borderMesh == null && border != null)
		{
			borderMesh = border.GetComponent<MeshFilter>().mesh;
		}
		if (mesh.vertices.Length == vertexCount * 2 && radius == lastRadius && angle == lastAngle && cutHeight == lastCutHeight)
		{
			return;
		}
		if (vertexCount % 2 == 0)
		{
			vertexCount++;
		}
		mesh.Clear();
		newVertices = new Vector3[vertexCount * 2];
		newTriangles = new int[(vertexCount - 1) * 6];
		int num = vertexCount;
		int num2 = 0;
		for (int i = 0; i < newTriangles.Length / 2; i += 3)
		{
			if (num2 < vertexCount / 2)
			{
				newTriangles[i] = num2;
				newTriangles[i + 1] = num;
				newTriangles[i + 2] = num2 + 1;
			}
			else
			{
				newTriangles[i] = num2;
				newTriangles[i + 1] = num + 1;
				newTriangles[i + 2] = num2 + 1;
			}
			num2++;
			num++;
		}
		num = vertexCount;
		num2 = 0;
		for (int j = newTriangles.Length / 2; j < newTriangles.Length; j += 3)
		{
			if (num2 < vertexCount / 2)
			{
				newTriangles[j] = num;
				newTriangles[j + 1] = num + 1;
				newTriangles[j + 2] = num2 + 1;
			}
			else
			{
				newTriangles[j] = num;
				newTriangles[j + 1] = num + 1;
				newTriangles[j + 2] = num2;
			}
			num2++;
			num++;
		}
		float num3 = angle * ((float)Math.PI / 180f);
		float f = (float)Math.PI / 2f - num3 / 2f;
		float f2 = (float)Math.PI / 2f + num3 / 2f;
		float num4 = cutHeight / Mathf.Cos(num3 / 2f);
		Vector3 vector = new Vector3(radius * Mathf.Cos(f2), radius * Mathf.Sin(f2));
		Vector3 vector2 = new Vector3(radius * Mathf.Cos(f), radius * Mathf.Sin(f));
		Vector3 vector3 = vector.normalized * num4;
		Vector3 vector4 = vector2.normalized * num4;
		Vector3 vector5 = vector4 - vector3;
		float num5 = Vector3.Distance(vector4, vector3) / (float)(vertexCount - 1);
		int num6 = 0;
		newVertices[newVertices.Length / 2 - 1] = vector4;
		for (int k = 0; k < newVertices.Length / 2 - 1; k++)
		{
			newVertices[k] = vector3 + vector5.normalized * num6 * num5;
			num6++;
		}
		float[] array = new float[vertexCount];
		float num7 = num3 / 2f;
		array[0] = 0f - num7;
		array[^1] = (float)Math.PI + num7;
		float num8 = ((float)Math.PI + 2f * num7) / (float)(vertexCount - 1);
		for (int l = 1; l < array.Length - 1; l++)
		{
			array[l] = array[0] + (float)l * num8;
		}
		float num9 = num3 / 2f;
		float f3 = (float)Math.PI / 2f - num9;
		Vector3 vector6 = new Vector3((vector.x + vector2.x) / 2f, (vector.y + vector2.y) / 2f);
		float num10 = (vector2 - vector6).magnitude / Mathf.Tan(f3);
		arcCenter = new Vector3(vector6.x, vector6.y + num10);
		float num11 = num10 / Mathf.Cos(f3);
		num6 = array.Length - 1;
		for (int m = vertexCount; m < newVertices.Length; m++)
		{
			float x = arcCenter.x + num11 * Mathf.Cos(array[num6]);
			float y = arcCenter.y + num11 * Mathf.Sin(array[num6]);
			newVertices[m] = new Vector3(x, y);
			num6--;
		}
		newUV = new Vector2[newVertices.Length];
		for (int n = 0; n < newUV.Length; n++)
		{
			newUV[n] = new Vector2(newVertices[n].x, newVertices[n].y);
		}
		if (border != null)
		{
			vector3 = new Vector3(vector3.x - borderWidth * Mathf.Sin((float)Math.PI / 4f), vector3.y - borderWidth * Mathf.Cos((float)Math.PI / 4f));
			vector4 = new Vector3(vector4.x + borderWidth * Mathf.Sin((float)Math.PI / 4f), vector4.y - borderWidth * Mathf.Cos((float)Math.PI / 4f));
			vector5 = vector4 - vector3;
			num5 = Vector3.Distance(vector4, vector3) / (float)(vertexCount - 1);
			num6 = 0;
			Vector3[] array2 = new Vector3[newVertices.Length];
			array2[array2.Length / 2 - 1] = vector4;
			for (int num12 = 0; num12 < array2.Length / 2 - 1; num12++)
			{
				array2[num12] = vector3 + vector5.normalized * num6 * num5;
				num6++;
			}
			array = new float[vertexCount];
			num7 = num3 / 2f;
			array[0] = 0f - num7;
			array[^1] = (float)Math.PI + num7;
			num8 = ((float)Math.PI + 2f * num7) / (float)(vertexCount - 1);
			for (int num13 = 1; num13 < array.Length - 1; num13++)
			{
				array[num13] = array[0] + (float)num13 * num8;
			}
			num11 = (colliderSize = num11 + borderWidth);
			num6 = array.Length - 1;
			for (int num14 = vertexCount; num14 < array2.Length; num14++)
			{
				float x2 = arcCenter.x + num11 * Mathf.Cos(array[num6]);
				float y2 = arcCenter.y + num11 * Mathf.Sin(array[num6]);
				array2[num14] = new Vector3(x2, y2);
				num6--;
			}
			borderMesh.vertices = array2;
			borderMesh.uv = newUV;
			borderMesh.triangles = newTriangles;
			borderMesh.RecalculateNormals();
			borderMesh.RecalculateBounds();
		}
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		lastRadius = radius;
		lastAngle = angle;
		lastCutHeight = cutHeight;
	}
}
