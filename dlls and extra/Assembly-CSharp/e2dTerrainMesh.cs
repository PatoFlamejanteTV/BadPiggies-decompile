using System.Collections.Generic;
using UnityEngine;

public abstract class e2dTerrainMesh
{
	private e2dTerrain mTerrain;

	protected e2dTerrain Terrain => mTerrain;

	protected Transform transform => mTerrain.transform;

	protected List<e2dCurveNode> TerrainCurve => mTerrain.TerrainCurve;

	protected List<e2dCurveTexture> CurveTextures => mTerrain.CurveTextures;

	protected List<Texture2D> CurveControlTextures => mTerrain.CurveMesh.CurveControlTextures;

	protected e2dTerrainBoundary Boundary => mTerrain.Boundary;

	public e2dTerrainMesh(e2dTerrain terrain)
	{
		mTerrain = terrain;
	}

	protected void ResetMeshObjectsTransforms()
	{
		transform.Find(e2dConstants.FILL_MESH_NAME).transform.localPosition = Vector3.zero;
		transform.Find(e2dConstants.FILL_MESH_NAME).transform.localRotation = Quaternion.identity;
		transform.Find(e2dConstants.FILL_MESH_NAME).transform.localScale = Vector3.one;
		transform.Find(e2dConstants.CURVE_MESH_NAME).transform.localPosition = Vector3.zero;
		transform.Find(e2dConstants.CURVE_MESH_NAME).transform.localRotation = Quaternion.identity;
		transform.Find(e2dConstants.CURVE_MESH_NAME).transform.localScale = Vector3.one;
		transform.Find(e2dConstants.COLLIDER_MESH_NAME).transform.localPosition = Vector3.zero;
		transform.Find(e2dConstants.COLLIDER_MESH_NAME).transform.localRotation = Quaternion.identity;
		transform.Find(e2dConstants.COLLIDER_MESH_NAME).transform.localScale = Vector3.one;
	}

	protected void EnsureMeshObjectsExist()
	{
		if (transform.Find(e2dConstants.FILL_MESH_NAME) == null)
		{
			new GameObject(e2dConstants.FILL_MESH_NAME).transform.parent = transform;
		}
		if (transform.Find(e2dConstants.CURVE_MESH_NAME) == null)
		{
			new GameObject(e2dConstants.CURVE_MESH_NAME).transform.parent = transform;
		}
		if (transform.Find(e2dConstants.COLLIDER_MESH_NAME) == null)
		{
			new GameObject(e2dConstants.COLLIDER_MESH_NAME).transform.parent = transform;
		}
	}

	protected void EnsureMeshComponentsExist()
	{
		EnsureMeshObjectsExist();
		GameObject gameObject = transform.Find(e2dConstants.FILL_MESH_NAME).gameObject;
		EnsureMeshFilterExists(gameObject);
		EnsureMeshRendererExists(gameObject);
		EnsureScriptsAttached(gameObject);
		gameObject = transform.Find(e2dConstants.CURVE_MESH_NAME).gameObject;
		EnsureMeshFilterExists(gameObject);
		EnsureMeshRendererExists(gameObject);
		EnsureScriptsAttached(gameObject);
		gameObject = transform.Find(e2dConstants.COLLIDER_MESH_NAME).gameObject;
		EnsureMeshColliderExists(gameObject);
		EnsureScriptsAttached(gameObject);
	}

	protected void EnsureScriptsAttached(GameObject meshObject)
	{
		if (meshObject.GetComponent<e2dMeshObject>() == null)
		{
			meshObject.AddComponent<e2dMeshObject>();
		}
	}

	protected void EnsureMeshFilterExists(GameObject meshObject)
	{
		if (meshObject.GetComponent<MeshFilter>() == null)
		{
			Mesh mesh = new Mesh();
			mesh.name = meshObject.name;
			meshObject.AddComponent<MeshFilter>().mesh = mesh;
		}
		else if (meshObject.GetComponent<MeshFilter>().sharedMesh == null)
		{
			Mesh mesh2 = new Mesh();
			mesh2.name = meshObject.name;
			meshObject.GetComponent<MeshFilter>().mesh = mesh2;
		}
	}

	protected void EnsureMeshRendererExists(GameObject meshObject)
	{
		if (meshObject.GetComponent<MeshRenderer>() == null)
		{
			meshObject.AddComponent<MeshRenderer>();
		}
	}

	protected void EnsureMeshColliderExists(GameObject meshObject)
	{
		if (meshObject.GetComponent<MeshCollider>() == null)
		{
			Mesh mesh = new Mesh();
			mesh.name = meshObject.name;
			meshObject.AddComponent<MeshCollider>().sharedMesh = mesh;
		}
		else if (meshObject.GetComponent<MeshCollider>().sharedMesh == null)
		{
			Mesh mesh2 = new Mesh();
			mesh2.name = meshObject.name;
			meshObject.GetComponent<MeshCollider>().sharedMesh = mesh2;
		}
	}

	public void DeleteAllSubobjects()
	{
		EnsureMeshObjectsExist();
		Object.DestroyImmediate(transform.Find(e2dConstants.FILL_MESH_NAME).gameObject);
		Object.DestroyImmediate(transform.Find(e2dConstants.CURVE_MESH_NAME).gameObject);
		Object.DestroyImmediate(transform.Find(e2dConstants.COLLIDER_MESH_NAME).gameObject);
	}
}
