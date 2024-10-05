using System.Collections.Generic;
using UnityEngine;

public class e2dTerrainFillMesh : e2dTerrainMesh
{
	public MeshRenderer renderer
	{
		get
		{
			EnsureMeshComponentsExist();
			return base.transform.Find(e2dConstants.FILL_MESH_NAME).GetComponent<MeshRenderer>();
		}
	}

	public GameObject gameObject => base.transform.Find(e2dConstants.FILL_MESH_NAME).gameObject;

	public e2dTerrainFillMesh(e2dTerrain terrain)
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
		ResetMeshObjectsTransforms();
		List<Vector2> shapePolygon = GetShapePolygon();
		List<int> list = new e2dTriangulator(shapePolygon.ToArray()).Triangulate();
		list.Reverse();
		int[] triangles = list.ToArray();
		Vector3[] array = new Vector3[shapePolygon.Count];
		Vector3[] array2 = new Vector3[array.Length];
		Vector2[] array3 = new Vector2[array.Length];
		for (int i = 0; i < shapePolygon.Count; i++)
		{
			array[i] = shapePolygon[i];
			array2[i] = Vector3.back;
			array3[i] = GetPointFillUV(shapePolygon[i]);
		}
		MeshFilter component = base.transform.Find(e2dConstants.FILL_MESH_NAME).GetComponent<MeshFilter>();
		component.sharedMesh.Clear();
		component.sharedMesh.vertices = array;
		component.sharedMesh.normals = array2;
		component.sharedMesh.uv = array3;
		component.sharedMesh.triangles = triangles;
		if (SomeMaterialsMissing())
		{
			RebuildMaterial();
		}
	}

	public bool IsMeshValid()
	{
		Transform transform = base.transform.Find(e2dConstants.FILL_MESH_NAME);
		if (transform == null)
		{
			return false;
		}
		MeshFilter component = transform.GetComponent<MeshFilter>();
		if (!(component == null) && !(component.sharedMesh == null))
		{
			return component.sharedMesh.vertexCount != 0;
		}
		return false;
	}

	public List<Vector2> GetShapePolygon()
	{
		List<Vector2> list = new List<Vector2>(base.TerrainCurve.Count + 3);
		Vector2[] boundaryRect = base.Boundary.GetBoundaryRect();
		Vector2 point = base.TerrainCurve[0].position;
		Vector2 point2 = base.TerrainCurve[base.TerrainCurve.Count - 1].position;
		int num = base.Boundary.ProjectStartPointToBoundary(ref point);
		int num2 = base.Boundary.ProjectEndPointToBoundary(ref point2);
		if (!base.Terrain.CurveClosed && point2 != base.TerrainCurve[base.TerrainCurve.Count - 1].position)
		{
			list.Add(point2);
		}
		if (!base.Terrain.CurveClosed)
		{
			bool flag = (point2 - boundaryRect[num2 + 1]).sqrMagnitude <= (point - boundaryRect[num2 + 1]).sqrMagnitude;
			int num3 = num2;
			while (flag || num3 != num)
			{
				flag = false;
				list.Add(boundaryRect[num3 + 1]);
				num3 = (num3 + 1) % 4;
			}
		}
		if (!base.Terrain.CurveClosed && point != base.TerrainCurve[0].position)
		{
			list.Add(point);
		}
		foreach (e2dCurveNode item in base.TerrainCurve)
		{
			list.Add(item.position);
		}
		if (list[list.Count - 1] == list[0])
		{
			list.RemoveAt(list.Count - 1);
		}
		return list;
	}

	private Vector2 GetPointFillUV(Vector2 curvePoint)
	{
		float x = (curvePoint.x - base.Terrain.FillTextureTileOffsetX) / base.Terrain.FillTextureTileWidth;
		float y = (curvePoint.y - base.Terrain.FillTextureTileOffsetY) / base.Terrain.FillTextureTileHeight;
		return new Vector2(x, y);
	}

	public void DestroyMesh()
	{
		EnsureMeshObjectsExist();
		MeshFilter component = base.transform.Find(e2dConstants.FILL_MESH_NAME).GetComponent<MeshFilter>();
		if ((bool)component && component.sharedMesh != null)
		{
			Object.DestroyImmediate(component.sharedMesh);
		}
		MeshRenderer component2 = base.transform.Find(e2dConstants.FILL_MESH_NAME).GetComponent<MeshRenderer>();
		if ((bool)component2 && component2.sharedMaterials != null)
		{
			Material[] sharedMaterials = component2.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				Object.DestroyImmediate(sharedMaterials[i]);
			}
		}
	}

	public void RebuildMaterial()
	{
		EnsureMeshComponentsExist();
		MeshRenderer component = base.transform.Find(e2dConstants.FILL_MESH_NAME).GetComponent<MeshRenderer>();
		Material[] sharedMaterials = component.sharedMaterials;
		if (sharedMaterials != null)
		{
			Material[] array = sharedMaterials;
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i], allowDestroyingAssets: true);
			}
		}
		sharedMaterials = new Material[1]
		{
			new Material(Shader.Find("_Custom/Unlit_Color_Geometry"))
		};
		if (!base.Terrain.FillTexture)
		{
			base.Terrain.FillTexture = (Texture)Resources.Load("defaultFillTexture", typeof(Texture));
		}
		sharedMaterials[0].mainTexture = base.Terrain.FillTexture;
		component.materials = sharedMaterials;
	}

	public bool SomeMaterialsMissing()
	{
		return base.transform.Find(e2dConstants.FILL_MESH_NAME).GetComponent<MeshRenderer>().sharedMaterial == null;
	}
}
