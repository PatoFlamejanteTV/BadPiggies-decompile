using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class e2dTerrain : MonoBehaviour
{
	public List<e2dCurveNode> TerrainCurve = new List<e2dCurveNode>();

	public Rect TerrainBoundary = new Rect(0f, 0f, 0f, 0f);

	public Texture FillTexture;

	public float FillTextureTileWidth = e2dConstants.INIT_FILL_TEXTURE_WIDTH;

	public float FillTextureTileHeight = e2dConstants.INIT_FILL_TEXTURE_HEIGHT;

	public float FillTextureTileOffsetX = e2dConstants.INIT_FILL_TEXTURE_OFFSET_X;

	public float FillTextureTileOffsetY = e2dConstants.INIT_FILL_TEXTURE_OFFSET_Y;

	public bool CurveClosed = e2dConstants.INIT_CURVE_CLOSED;

	public bool NoCollider = e2dConstants.INIT_NO_COLLIDER;

	public List<e2dCurveTexture> CurveTextures = new List<e2dCurveTexture>();

	public bool PlasticEdges = true;

	public bool AllowRebuildMaterial = true;

	private e2dTerrainFillMesh mFillMesh;

	private e2dTerrainCurveMesh mCurveMesh;

	private e2dTerrainColliderMesh mColliderMesh;

	private e2dTerrainBoundary mBoundary;

	private bool mCurveIntercrossing;

	[NonSerialized]
	public UnityEngine.Object EditorReference;

	public bool IsEditable
	{
		get
		{
			if (TerrainCurve != null)
			{
				return TerrainCurve.Count >= 2;
			}
			return false;
		}
	}

	public e2dTerrainBoundary Boundary => mBoundary;

	public e2dTerrainCurveMesh CurveMesh => mCurveMesh;

	public e2dTerrainFillMesh FillMesh => mFillMesh;

	public e2dTerrainColliderMesh ColliderMesh => mColliderMesh;

	public bool CurveIntercrossing => mCurveIntercrossing;

	private void OnEnable()
	{
		EditorReference = null;
		mBoundary = new e2dTerrainBoundary(this);
		mFillMesh = new e2dTerrainFillMesh(this);
		mCurveMesh = new e2dTerrainCurveMesh(this);
		mColliderMesh = new e2dTerrainColliderMesh(this);
		if (!mFillMesh.IsMeshValid())
		{
			FixCurve();
			FixBoundary();
			RebuildAllMaterials();
			RebuildAllMeshes();
		}
		else
		{
			CurveMesh.UpdateControlTextures(forceRecreate: true);
		}
	}

	private void OnDisable()
	{
		mCurveMesh.DestroyTemporaryAssets();
	}

	public void Reset()
	{
		TerrainCurve.Clear();
		TerrainBoundary = new Rect(0f, 0f, 0f, 0f);
		mFillMesh.DestroyMesh();
		mCurveMesh.DestroyMesh();
		mColliderMesh.DestroyMesh();
	}

	public int GetMaxNodesCount()
	{
		return 8192;
	}

	public void AddPointOnCurve(int beforeWhichIndex, Vector2 toAdd)
	{
		e2dCurveNode e2dCurveNode2 = new e2dCurveNode(toAdd);
		if (beforeWhichIndex > 0)
		{
			e2dCurveNode2.texture = TerrainCurve[beforeWhichIndex - 1].texture;
		}
		else if (beforeWhichIndex < TerrainCurve.Count)
		{
			e2dCurveNode2.texture = TerrainCurve[beforeWhichIndex].texture;
		}
		TerrainCurve.Insert(beforeWhichIndex, e2dCurveNode2);
		mCurveMesh.UpdateControlTextures();
	}

	public void RemovePointOnCurve(int index, bool moveTheRest)
	{
		Vector2 vector = Vector2.zero;
		if (index < TerrainCurve.Count - 1)
		{
			vector = TerrainCurve[index + 1].position - TerrainCurve[index].position;
		}
		TerrainCurve.RemoveAt(index);
		if (moveTheRest)
		{
			for (int i = index; i < TerrainCurve.Count; i++)
			{
				TerrainCurve[i].position = TerrainCurve[i].position - vector;
			}
		}
		mCurveMesh.UpdateControlTextures();
	}

	public void AddCurvePoints(Vector2[] points, int firstToReplace, int lastToReplace)
	{
		if (TerrainCurve.Count + points.Length - (lastToReplace - firstToReplace + 1) <= GetMaxNodesCount())
		{
			TerrainCurve.RemoveRange(firstToReplace, lastToReplace - firstToReplace + 1);
			TerrainCurve.Capacity = TerrainCurve.Count + points.Length;
			int num = firstToReplace;
			foreach (Vector2 position in points)
			{
				TerrainCurve.Insert(num++, new e2dCurveNode(position));
			}
			mCurveMesh.UpdateControlTextures();
		}
	}

	public void AlignPointsOnCurve(int index, int referenceIndex, bool horizontally)
	{
		if (index >= 0 && index <= TerrainCurve.Count - 1 && referenceIndex >= 0 && referenceIndex <= TerrainCurve.Count - 1)
		{
			if (horizontally)
			{
				TerrainCurve[index].position.y = TerrainCurve[referenceIndex].position.y;
			}
			else
			{
				TerrainCurve[index].position.x = TerrainCurve[referenceIndex].position.x;
			}
		}
	}

	public void FixCurve()
	{
		for (int i = 0; i < TerrainCurve.Count; i++)
		{
			if (float.IsNaN(TerrainCurve[i].position.x))
			{
				TerrainCurve[i].position.x = 0f;
			}
			if (float.IsNaN(TerrainCurve[i].position.y))
			{
				TerrainCurve[i].position.y = 0f;
			}
		}
		if (TerrainCurve.Count >= 3 && !CurveClosed && TerrainCurve[TerrainCurve.Count - 1] == TerrainCurve[0])
		{
			Vector2 vector = TerrainCurve[TerrainCurve.Count - 1].position - TerrainCurve[TerrainCurve.Count - 2].position;
			TerrainCurve[TerrainCurve.Count - 1].position -= 0.5f * vector;
		}
		if (TerrainCurve.Count >= 3 && CurveClosed)
		{
			TerrainCurve[TerrainCurve.Count - 1].Copy(TerrainCurve[0]);
		}
		if (!e2dConstants.CHECK_CURVE_INTERCROSSING)
		{
			return;
		}
		mCurveIntercrossing = false;
		int num = TerrainCurve.Count;
		if (CurveClosed)
		{
			num--;
		}
		for (int j = 3; j < num; j++)
		{
			if (IntersectsCurve(0, j - 2, TerrainCurve[j - 1].position, TerrainCurve[j].position))
			{
				mCurveIntercrossing = true;
			}
		}
	}

	public void FixBoundary()
	{
		Boundary.FixBoundary();
	}

	public bool IntersectsCurve(int startIndex, int endIndex, Vector2 a, Vector2 b)
	{
		for (int i = startIndex; i < endIndex; i++)
		{
			if (e2dUtils.SegmentsIntersect(a, b, TerrainCurve[i].position, TerrainCurve[i + 1].position))
			{
				return true;
			}
		}
		return false;
	}

	public void RebuildAllMeshes()
	{
		mFillMesh.RebuildMesh();
		mCurveMesh.RebuildMesh();
		if (NoCollider)
		{
			mColliderMesh.ResetMesh();
		}
		else
		{
			mColliderMesh.RebuildMesh();
		}
	}

	public void RebuildAllMaterials()
	{
		mFillMesh.RebuildMaterial();
		mCurveMesh.UpdateControlTextures(forceRecreate: true);
		mCurveMesh.RebuildMaterial();
	}
}
