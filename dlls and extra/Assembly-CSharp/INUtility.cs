using System;
using UnityEngine;

public static class INUtility
{
	public static void CreateSphereMesh(this MeshFilter meshFilter, float radius, float halfWidth, float angle, int count)
	{
		Mesh mesh = meshFilter.sharedMesh;
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		float num = (float)Math.PI / 180f * angle / (float)count;
		Vector3[] array = new Vector3[(count + 1) * 2];
		int[] array2 = new int[count * 6];
		for (int i = 0; i <= count; i++)
		{
			float num2 = Mathf.Cos((float)(i - count / 2) * num);
			float num3 = Mathf.Sin((float)(i - count / 2) * num);
			array[i] = new Vector3((radius - halfWidth) * num2, (radius - halfWidth) * num3);
			array[i + count + 1] = new Vector3((radius + halfWidth) * num2, (radius + halfWidth) * num3);
		}
		for (int j = 0; j < count; j++)
		{
			array2[j * 6] = j;
			array2[j * 6 + 1] = j + 1;
			array2[j * 6 + 2] = j + count + 1;
			array2[j * 6 + 3] = j + 1;
			array2[j * 6 + 4] = j + count + 1;
			array2[j * 6 + 5] = j + count + 2;
		}
		mesh.vertices = array;
		mesh.triangles = array2;
		meshFilter.sharedMesh = mesh;
	}

	public static void CreateBoxMesh(this MeshFilter meshFilter, float length, float halfWidth)
	{
		Mesh mesh = meshFilter.sharedMesh;
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		else
		{
			mesh.Clear();
		}
		mesh.vertices = new Vector3[4]
		{
			new Vector3(length, halfWidth),
			new Vector3(0f, halfWidth),
			new Vector3(0f, 0f - halfWidth),
			new Vector3(length, 0f - halfWidth)
		};
		mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		meshFilter.sharedMesh = mesh;
	}
}
