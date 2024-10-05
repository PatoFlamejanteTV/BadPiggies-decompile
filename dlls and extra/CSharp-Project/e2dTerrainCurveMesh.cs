using System.Collections.Generic;
using UnityEngine;

public class e2dTerrainCurveMesh : e2dTerrainMesh
{
	public List<Texture2D> ControlTextures;

	public List<Vector3> StripeVertices;

	public MeshRenderer renderer
	{
		get
		{
			EnsureMeshComponentsExist();
			return base.transform.Find(e2dConstants.CURVE_MESH_NAME).GetComponent<MeshRenderer>();
		}
	}

	public GameObject gameObject => base.transform.Find(e2dConstants.CURVE_MESH_NAME).gameObject;

	public e2dTerrainCurveMesh(e2dTerrain terrain)
		: base(terrain)
	{
		ControlTextures = new List<Texture2D>();
		StripeVertices = new List<Vector3>();
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
		Vector3[] array = new Vector3[base.TerrainCurve.Count * 2];
		Vector2[] array2 = new Vector2[array.Length];
		Color[] array3 = new Color[array.Length];
		int[] array4 = new int[(base.TerrainCurve.Count - 1) * 2 * 3];
		StripeVertices = ComputeStripeVertices();
		array[0] = base.TerrainCurve[0].position;
		array[1] = StripeVertices[0];
		array2[0] = new Vector2(0f, 0f);
		array2[1] = new Vector2(0f, 0f);
		array3[0] = new Color(1f, 0f, 0f, 0f);
		array3[1] = new Color(0f, 0f, 0f, 0f);
		float num = 0f;
		for (int i = 1; i < base.TerrainCurve.Count; i++)
		{
			int num2 = i;
			int num3 = 2 * num2;
			int num4 = 6 * (i - 1);
			float magnitude = (base.TerrainCurve[i].position - base.TerrainCurve[i - 1].position).magnitude;
			num += magnitude;
			array[num3] = base.TerrainCurve[num2].position;
			array[num3] -= Vector3.forward * 0.01f;
			array2[num3] = new Vector2(num, num2);
			array3[num3] = new Color(1f, 0f, 0f, 0f);
			array[num3 + 1] = StripeVertices[i];
			array2[num3 + 1] = new Vector2(num, num2);
			array3[num3 + 1] = new Color(0f, 0f, 0f, 0f);
			if (e2dUtils.PointInTriangle(array[num3 + 1], array[num3 - 2], array[num3], array[num3 - 1]))
			{
				array4[num4] = num3 - 2;
				array4[num4 + 1] = num3 + 1;
				array4[num4 + 2] = num3 - 1;
				array4[num4 + 3] = num3 - 2;
				array4[num4 + 4] = num3;
				array4[num4 + 5] = num3 + 1;
			}
			else
			{
				array4[num4] = num3 - 2;
				array4[num4 + 1] = num3;
				array4[num4 + 2] = num3 - 1;
				array4[num4 + 3] = num3 - 1;
				array4[num4 + 4] = num3;
				array4[num4 + 5] = num3 + 1;
			}
		}
		MeshFilter component = base.transform.Find(e2dConstants.CURVE_MESH_NAME).GetComponent<MeshFilter>();
		component.sharedMesh.Clear();
		component.sharedMesh.vertices = array;
		component.sharedMesh.uv = array2;
		component.sharedMesh.colors = array3;
		component.sharedMesh.triangles = array4;
		if (SomeMaterialsMissing())
		{
			RebuildMaterial();
		}
	}

	private List<Vector3> ComputeStripeVertices()
	{
		List<Vector3> list = new List<Vector3>(base.TerrainCurve.Count);
		list.Add(ComputeFirstStripeVertex());
		for (int i = 1; i < base.TerrainCurve.Count - 1; i++)
		{
			list.Add(ComputeStripeVertex(i));
		}
		list.Add(ComputeLastStripeVertex());
		for (int j = 0; j < list.Count - 1; j++)
		{
			for (int k = j + 2; k < list.Count - 1; k++)
			{
				if (e2dUtils.SegmentsIntersect(list[j], list[j + 1], list[k], list[k + 1], out var intersection))
				{
					for (int l = j + 1; l <= k; l++)
					{
						list[l] = intersection;
					}
					break;
				}
			}
		}
		return list;
	}

	private Vector3 ComputeFirstStripeVertex()
	{
		Vector2 point = base.TerrainCurve[0].position;
		base.Boundary.ProjectStartPointToBoundary(ref point);
		Vector3 point2 = point;
		if (point != base.TerrainCurve[0].position)
		{
			Vector2 vector = base.TerrainCurve[1].position - base.TerrainCurve[0].position;
			Vector2 vector2 = GetNodeStripeSize(0) * new Vector2(vector.y, 0f - vector.x).normalized;
			Vector2 b = point - base.TerrainCurve[0].position;
			if (!e2dUtils.HalfLineAndLineIntersect(Vector2.zero, b, vector2, vector2 + vector, out var result))
			{
				result = Vector2.zero;
			}
			point2 = base.TerrainCurve[0].position + result;
			base.Boundary.EnsurePointIsInBoundary(ref point2);
		}
		return point2;
	}

	private Vector3 ComputeLastStripeVertex()
	{
		Vector2 point = base.TerrainCurve[base.TerrainCurve.Count - 1].position;
		base.Boundary.ProjectEndPointToBoundary(ref point);
		Vector3 point2 = point;
		if (point != base.TerrainCurve[base.TerrainCurve.Count - 1].position)
		{
			Vector2 vector = base.TerrainCurve[base.TerrainCurve.Count - 1].position - base.TerrainCurve[base.TerrainCurve.Count - 2].position;
			Vector2 vector2 = GetNodeStripeSize(base.TerrainCurve.Count - 1) * new Vector2(vector.y, 0f - vector.x).normalized;
			Vector2 b = point - base.TerrainCurve[base.TerrainCurve.Count - 1].position;
			if (!e2dUtils.HalfLineAndLineIntersect(Vector2.zero, b, vector2, vector2 + vector, out var result))
			{
				result = Vector2.zero;
			}
			point2 = base.TerrainCurve[base.TerrainCurve.Count - 1].position + result;
			base.Boundary.EnsurePointIsInBoundary(ref point2);
		}
		return point2;
	}

	private Vector3 ComputeStripeVertex(int nodeIndex)
	{
		Vector2 vector = base.TerrainCurve[nodeIndex].position - base.TerrainCurve[nodeIndex - 1].position;
		Vector2 vector2 = base.TerrainCurve[nodeIndex + 1].position - base.TerrainCurve[nodeIndex].position;
		Vector2 normalized = new Vector2(vector.y, 0f - vector.x).normalized;
		Vector2 normalized2 = new Vector2(vector2.y, 0f - vector2.x).normalized;
		Vector2 vector3 = GetNodeStripeSize(nodeIndex) * (normalized + normalized2).normalized;
		Vector3 point = base.TerrainCurve[nodeIndex].position + vector3;
		base.Boundary.EnsurePointIsInBoundary(ref point);
		return point;
	}

	private float GetNodeStripeSize(int nodeIndex)
	{
		return base.CurveTextures[base.TerrainCurve[nodeIndex].texture].size.y;
	}

	public void DestroyMesh()
	{
		EnsureMeshObjectsExist();
		DestroyTemporaryAssets();
		MeshFilter component = base.transform.Find(e2dConstants.CURVE_MESH_NAME).GetComponent<MeshFilter>();
		if ((bool)component && component.sharedMesh != null)
		{
			Object.DestroyImmediate(component.sharedMesh);
		}
		MeshRenderer component2 = base.transform.Find(e2dConstants.CURVE_MESH_NAME).GetComponent<MeshRenderer>();
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
		MeshRenderer component = base.transform.Find(e2dConstants.CURVE_MESH_NAME).GetComponent<MeshRenderer>();
		Material[] sharedMaterials = component.sharedMaterials;
		if (sharedMaterials != null)
		{
			Material[] array = sharedMaterials;
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i], allowDestroyingAssets: true);
			}
		}
		EnsureTexturesInited();
		int materialsNeededCount = GetMaterialsNeededCount();
		sharedMaterials = new Material[materialsNeededCount];
		if (ControlTextures.Count != materialsNeededCount)
		{
			UpdateControlTextures();
		}
		int num = 0;
		for (int j = 0; j < materialsNeededCount; j++)
		{
			sharedMaterials[j] = new Material(Shader.Find("e2d/Curve"));
			sharedMaterials[j].SetFloat("_ControlSize", ControlTextures[j].width);
			sharedMaterials[j].SetFloat("_InvControlSize", 1f / (float)ControlTextures[j].width);
			sharedMaterials[j].SetFloat("_InvControlSizeHalf", 0.5f / (float)ControlTextures[j].width);
			sharedMaterials[j].SetTexture("_Control", ControlTextures[j]);
			int num2 = 0;
			while (num2 < e2dConstants.NUM_TEXTURES_PER_STRIPE_SHADER && num < base.CurveTextures.Count)
			{
				sharedMaterials[j].SetTexture("_Splat" + num2, base.CurveTextures[num].texture);
				Vector4 value = new Vector4(1f / base.CurveTextures[num].size.x, base.CurveTextures[num].size.y, base.CurveTextures[num].fixedAngle ? 1 : 0, base.CurveTextures[num].fadeThreshold);
				sharedMaterials[j].SetVector("_SplatParams" + num2, value);
				num2++;
				num++;
			}
		}
		component.materials = sharedMaterials;
	}

	private void EnsureTexturesInited()
	{
		if (base.CurveTextures.Count != 0)
		{
			return;
		}
		base.CurveTextures.Clear();
		base.CurveTextures.Add(GetDefaultCurveTexture());
		foreach (Texture2D controlTexture in ControlTextures)
		{
			Object.DestroyImmediate(controlTexture, allowDestroyingAssets: true);
		}
		ControlTextures.Clear();
		ControlTextures.Add(CreateControlTexture(new Color(1f, 0f, 0f, 0f)));
	}

	public void UpdateControlTextures()
	{
		UpdateControlTextures(forceRecreate: false);
	}

	public void UpdateControlTextures(bool forceRecreate)
	{
		while (ControlTextures.Count > GetMaterialsNeededCount())
		{
			Object.DestroyImmediate(ControlTextures[ControlTextures.Count - 1], allowDestroyingAssets: true);
			ControlTextures.RemoveAt(ControlTextures.Count - 1);
		}
		while (ControlTextures.Count < GetMaterialsNeededCount())
		{
			ControlTextures.Add(CreateControlTexture(new Color(0f, 0f, 0f, 0f)));
		}
		for (int i = 0; i < ControlTextures.Count; i++)
		{
			if (forceRecreate || ControlTextures[i] == null || ControlTextures[i].width != GetControlTextureSize())
			{
				Object.DestroyImmediate(ControlTextures[i], allowDestroyingAssets: true);
				ControlTextures[i] = CreateControlTexture(new Color(0f, 0f, 0f, 0f));
				EnsureMeshComponentsExist();
				MeshRenderer component = base.transform.Find(e2dConstants.CURVE_MESH_NAME).GetComponent<MeshRenderer>();
				if (component.sharedMaterials != null && i == component.sharedMaterials.Length - 1 && (bool)component.sharedMaterials[i])
				{
					component.sharedMaterials[i].SetFloat("_ControlSize", ControlTextures[i].width);
					component.sharedMaterials[i].SetFloat("_InvControlSize", 1f / (float)ControlTextures[i].width);
					component.sharedMaterials[i].SetFloat("_InvControlSizeHalf", 0.5f / (float)ControlTextures[i].width);
					component.sharedMaterials[i].SetTexture("_Control", ControlTextures[i]);
				}
			}
			if (base.TerrainCurve.Count == 0)
			{
				break;
			}
			Color[] array = new Color[base.TerrainCurve.Count];
			for (int j = 0; j < base.TerrainCurve.Count; j++)
			{
				array[j] = new Color(0f, 0f, 0f, 0f);
				if (base.TerrainCurve[j].texture / e2dConstants.NUM_TEXTURES_PER_STRIPE_SHADER == i)
				{
					switch (base.TerrainCurve[j].texture % e2dConstants.NUM_TEXTURES_PER_STRIPE_SHADER)
					{
					case 0:
						array[j].r = 1f;
						break;
					case 1:
						array[j].g = 1f;
						break;
					case 2:
						array[j].b = 1f;
						break;
					case 3:
						array[j].a = 1f;
						break;
					}
				}
			}
			ControlTextures[i].SetPixels(0, 0, array.Length, 1, array);
			ControlTextures[i].Apply();
		}
	}

	private int GetMaterialsNeededCount()
	{
		int num = base.CurveTextures.Count / e2dConstants.NUM_TEXTURES_PER_STRIPE_SHADER;
		if (base.CurveTextures.Count % e2dConstants.NUM_TEXTURES_PER_STRIPE_SHADER != 0)
		{
			num++;
		}
		return num;
	}

	private int GetControlTextureSize()
	{
		int num = Mathf.NextPowerOfTwo(base.TerrainCurve.Count);
		if (num == 0)
		{
			num = 1;
		}
		return num;
	}

	private Texture2D CreateControlTexture(Color color)
	{
		int controlTextureSize = GetControlTextureSize();
		Texture2D texture2D = new Texture2D(controlTextureSize, 1, TextureFormat.ARGB32, mipChain: false);
		texture2D.filterMode = FilterMode.Bilinear;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.anisoLevel = 1;
		Color[] array = new Color[controlTextureSize];
		for (int i = 0; i < controlTextureSize; i++)
		{
			array[i] = color;
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	public e2dCurveTexture GetDefaultCurveTexture()
	{
		return new e2dCurveTexture((Texture)Resources.Load("defaultCurveTexture", typeof(Texture)));
	}

	public bool SomeMaterialsMissing()
	{
		return base.transform.Find(e2dConstants.CURVE_MESH_NAME).GetComponent<MeshRenderer>().sharedMaterial == null;
	}

	public void AppendCurveTexture()
	{
		base.Terrain.CurveTextures.Add(base.Terrain.CurveMesh.GetDefaultCurveTexture());
		UpdateControlTextures();
	}

	public void RemoveCurveTexture(int index)
	{
		base.Terrain.CurveTextures.RemoveAt(index);
		foreach (e2dCurveNode item in base.TerrainCurve)
		{
			if (item.texture == index)
			{
				item.texture = 0;
			}
			else if (item.texture > index)
			{
				item.texture--;
			}
		}
		UpdateControlTextures();
	}

	public void DestroyTemporaryAssets()
	{
		foreach (Texture2D controlTexture in ControlTextures)
		{
			if ((bool)controlTexture)
			{
				Object.DestroyImmediate(controlTexture, allowDestroyingAssets: true);
			}
		}
		ControlTextures.Clear();
	}
}
