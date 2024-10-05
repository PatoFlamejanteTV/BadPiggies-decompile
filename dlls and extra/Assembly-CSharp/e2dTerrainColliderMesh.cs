using System.Collections.Generic;
using UnityEngine;

public class e2dTerrainColliderMesh : e2dTerrainMesh
{
	public MeshCollider collider
	{
		get
		{
			EnsureMeshComponentsExist();
			return base.transform.Find(e2dConstants.COLLIDER_MESH_NAME).GetComponent<MeshCollider>();
		}
	}

	public GameObject gameObject => base.transform.Find(e2dConstants.COLLIDER_MESH_NAME).gameObject;

	public e2dTerrainColliderMesh(e2dTerrain terrain)
		: base(terrain)
	{
	}

	public void RebuildMesh()
	{
		if (base.TerrainCurve.Count < 2 || base.Terrain.CurveIntercrossing)
		{
			DestroyMesh();
			return;
		}
		EnsureMeshComponentsExist();
		List<Vector2> shapePolygon = base.Terrain.FillMesh.GetShapePolygon();
		Vector3[] array = new Vector3[2 * shapePolygon.Count];
		Vector3[] array2 = new Vector3[array.Length];
		int[] array3 = new int[6 * shapePolygon.Count];
		for (int i = 0; i < shapePolygon.Count; i++)
		{
			int num = 2 * i;
			array[num] = new Vector3(shapePolygon[i].x, shapePolygon[i].y, -0.5f * e2dConstants.COLLISION_MESH_Z_DEPTH);
			array[num + 1] = new Vector3(shapePolygon[i].x, shapePolygon[i].y, 0.5f * e2dConstants.COLLISION_MESH_Z_DEPTH);
			int num2 = i - 1;
			if (num2 < 0)
			{
				num2 += shapePolygon.Count;
			}
			int num3 = i + 1;
			if (num3 >= shapePolygon.Count)
			{
				num3 -= shapePolygon.Count;
			}
			Vector2 vector = new Vector2(shapePolygon[i].y - shapePolygon[num2].y, shapePolygon[num2].x - shapePolygon[num2].x);
			Vector2 vector2 = new Vector2(shapePolygon[num3].y - shapePolygon[i].y, shapePolygon[i].x - shapePolygon[num3].x);
			Vector3 vector3 = 0.5f * (vector + vector2);
			vector3.Normalize();
			array2[num] = vector3;
			array2[num + 1] = vector3;
			int num4 = 6 * i;
			array3[num4] = num % array.Length;
			array3[num4 + 1] = (num + 1) % array.Length;
			array3[num4 + 2] = (num + 2) % array.Length;
			array3[num4 + 3] = (num + 2) % array.Length;
			array3[num4 + 4] = (num + 1) % array.Length;
			array3[num4 + 5] = (num + 3) % array.Length;
		}
		MeshCollider component = base.transform.Find(e2dConstants.COLLIDER_MESH_NAME).GetComponent<MeshCollider>();
		component.sharedMesh.Clear();
		component.sharedMesh.vertices = array;
		component.sharedMesh.triangles = array3;
		ResetMeshObjectsTransforms();
	}

	public void ResetMesh()
	{
		base.transform.Find(e2dConstants.COLLIDER_MESH_NAME).GetComponent<MeshCollider>().sharedMesh.Clear();
		ResetMeshObjectsTransforms();
	}

	public void DestroyMesh()
	{
		EnsureMeshObjectsExist();
		MeshCollider component = base.transform.Find(e2dConstants.COLLIDER_MESH_NAME).GetComponent<MeshCollider>();
		if ((bool)component && component.sharedMesh != null)
		{
			Object.DestroyImmediate(component.sharedMesh);
		}
	}
}
