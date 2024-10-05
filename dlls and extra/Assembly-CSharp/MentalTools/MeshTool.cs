using System.Collections.Generic;
using UnityEngine;

namespace MentalTools;

public class MeshTool
{
	private struct MeshObject
	{
		public GameObject parent;

		public MeshFilter mf;

		public MeshRenderer mr;

		public MeshObject(GameObject _parent, MeshFilter _mf, MeshRenderer _mr)
		{
			parent = _parent;
			mf = _mf;
			mr = _mr;
		}
	}

	private static MeshObject InitTransformForMesh(Transform transform)
	{
		GameObject gameObject = new GameObject("Polygon");
		gameObject.transform.parent = transform;
		gameObject.transform.localPosition = Vector3.zero;
		MeshFilter mf = MentalHelper.EnsureComponent<MeshFilter>(gameObject);
		MeshRenderer mr = MentalHelper.EnsureComponent<MeshRenderer>(gameObject);
		return new MeshObject(gameObject, mf, mr);
	}

	public static GameObject CreateMeshFromBezier(BezierCurve bezierCurve, Transform transform, MentalMath.AxisSpace space, bool debugMode = false)
	{
		if (bezierCurve == null || bezierCurve.Curve == null || bezierCurve.Curve.Count == 0)
		{
			return null;
		}
		List<Vector3> list = new List<Vector3>();
		int bezierPointCount = bezierCurve.bezierPointCount;
		for (int i = 1; i <= bezierPointCount; i++)
		{
			list.Add(bezierCurve.Curve.GetPoint((float)i / (float)bezierPointCount, loop: true, Vector3.zero));
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return CreateMeshFromPolygon(list, transform, space, debugMode);
	}

	public static GameObject CreateMeshFromPolygon(List<Vector3> vertices, Transform transform, MentalMath.AxisSpace space, bool debugMode = false)
	{
		MeshObject meshObject = InitTransformForMesh(transform);
		if (debugMode)
		{
			for (int i = 0; i < vertices.Count; i++)
			{
			}
		}
		List<Vector2> list = new List<Vector2>();
		for (int j = 0; j < vertices.Count; j++)
		{
			list.Add(new Vector2(vertices[j].x, vertices[j].y));
		}
		List<int> list2 = new List<int>();
		bool[] array = new bool[vertices.Count];
		for (int k = 0; k < array.Length; k++)
		{
			array[k] = true;
		}
		int num = 0;
		int num2 = 1;
		int num3 = 2;
		int num4 = vertices.Count;
		int num5 = num4 * 10;
		while (num4 > 0)
		{
			num5--;
			if (num5 < 0)
			{
				break;
			}
			bool flag = true;
			for (int l = 0; l < vertices.Count; l++)
			{
				if (num != l && num2 != l && num3 != l && MentalMath.PointInTriangle(vertices[l], vertices[num], vertices[num2], vertices[num3], MentalMath.AxisSpace.XY))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Vector3 vector = vertices[num2] - vertices[num];
				flag = Vector3.Angle(to: MentalMath.GetCounterClockwiseNormal(vector + (vertices[num3] - vertices[num2]), space), from: -vector) > 90f;
			}
			if (!flag)
			{
				num = num2;
				num2 = num3;
			}
			else
			{
				if (debugMode)
				{
					_ = transform.position + Vector3.up * 3f;
				}
				list2.Add(num);
				list2.Add(num2);
				list2.Add(num3);
				num4--;
				if (num4 <= 2)
				{
					if (!debugMode)
					{
					}
					break;
				}
				array[num2] = false;
				num2 = num3;
			}
			int num6 = num2;
			int num7 = vertices.Count;
			while (num2 == num3)
			{
				num7--;
				if (num7 < 0)
				{
					break;
				}
				num6++;
				if (num6 >= vertices.Count)
				{
					num6 = 0;
				}
				if (array[num6] && num != num6 && num2 != num6)
				{
					num3 = num6;
					break;
				}
			}
		}
		if (!debugMode)
		{
			Mesh mesh = new Mesh();
			mesh.name = transform.name;
			mesh.vertices = vertices.ToArray();
			mesh.uv = list.ToArray();
			mesh.triangles = list2.ToArray();
			mesh.RecalculateNormals();
			meshObject.mf.sharedMesh = mesh;
		}
		return meshObject.parent;
	}

	public static GameObject CreateMeshStripFromBezier(BezierCurve bezierCurve, Transform transform, MentalMath.AxisSpace space, float stripWidth, bool debugMode = false)
	{
		List<Vector3> list = new List<Vector3>();
		int bezierPointCount = bezierCurve.bezierPointCount;
		Vector3 vector = bezierCurve.Curve.GetPoint(0f, loop: false);
		Vector3 vector2 = bezierCurve.Curve.GetPoint(1f / (float)bezierPointCount, bezierCurve.loop);
		Vector3 vector3 = vector2;
		Vector3 counterClockwiseNormal = MentalMath.GetCounterClockwiseNormal(vector2 - vector + (vector3 - vector2), space);
		list.Add(vector + counterClockwiseNormal * stripWidth);
		list.Add(vector - counterClockwiseNormal * stripWidth);
		for (int i = 1; i <= bezierPointCount; i++)
		{
			if (i == bezierPointCount && bezierCurve.loop)
			{
				list.Add(list[0]);
				list.Add(list[1]);
				continue;
			}
			float ct = (float)(i + 1) / (float)bezierPointCount;
			vector3 = bezierCurve.Curve.GetPoint(ct, bezierCurve.loop);
			counterClockwiseNormal = MentalMath.GetCounterClockwiseNormal(vector2 - vector + (vector3 - vector2), space);
			list.Add(vector2 + counterClockwiseNormal * stripWidth);
			list.Add(vector2 - counterClockwiseNormal * stripWidth);
			vector = vector2;
			vector2 = vector3;
		}
		return CreateMeshStrip(list, transform, debugMode);
	}

	public static GameObject CreateMeshStrip(List<Vector3> vertices, Transform transform, bool debugMode = false)
	{
		MeshObject meshObject = InitTransformForMesh(transform);
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < vertices.Count; i++)
		{
			list.Add(new Vector2(vertices[i].x, vertices[i].y));
		}
		List<int> list2 = new List<int>();
		for (int j = 0; j < vertices.Count - 2; j++)
		{
			list2.Add(j);
			if (j % 2 == 0)
			{
				list2.Add(j + 2);
				list2.Add(j + 1);
			}
			else
			{
				list2.Add(j + 1);
				list2.Add(j + 2);
			}
		}
		if (!debugMode)
		{
			Mesh mesh = new Mesh();
			mesh.name = transform.name;
			mesh.vertices = vertices.ToArray();
			mesh.uv = list.ToArray();
			mesh.triangles = list2.ToArray();
			mesh.RecalculateNormals();
			meshObject.mf.sharedMesh = mesh;
		}
		return meshObject.parent;
	}
}
