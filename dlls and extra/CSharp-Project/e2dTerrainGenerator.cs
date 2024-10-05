using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(e2dTerrain))]
public class e2dTerrainGenerator : MonoBehaviour
{
	private class HeightComparer : IComparer<KeyValuePair<float, int>>
	{
		public int Compare(KeyValuePair<float, int> x, KeyValuePair<float, int> y)
		{
			if (!(x.Key >= y.Key))
			{
				return -1;
			}
			if (!(x.Key <= y.Key))
			{
				return 1;
			}
			return 0;
		}
	}

	public bool FullRebuild = e2dConstants.INIT_FULL_REBUILD;

	public float NodeStepSize = e2dConstants.INIT_NODE_STEP_SIZE;

	public Vector2 TargetPosition = e2dConstants.INIT_TARGET_POSITION;

	public Vector2 TargetSize = e2dConstants.INIT_TARGET_SIZE;

	public float TargetAngle = e2dConstants.INIT_TARGET_ANGLE;

	public e2dCurvePerlinPreset Perlin = (e2dCurvePerlinPreset)e2dPresets.GetDefault(e2dGeneratorCurveMethod.PERLIN);

	public e2dCurveVoronoiPreset Voronoi = (e2dCurveVoronoiPreset)e2dPresets.GetDefault(e2dGeneratorCurveMethod.VORONOI);

	public e2dCurveMidpointPreset Midpoint = (e2dCurveMidpointPreset)e2dPresets.GetDefault(e2dGeneratorCurveMethod.MIDPOINT);

	public e2dCurveWalkPreset Walk = (e2dCurveWalkPreset)e2dPresets.GetDefault(e2dGeneratorCurveMethod.WALK);

	public List<e2dGeneratorPeak> Peaks = new List<e2dGeneratorPeak>();

	public float[] CurveBlendingWeights = new float[4];

	public int SmoothIterations;

	public int CliffTextureIndex;

	public float CliffStartAngle;

	public List<float> TextureHeights;

	private e2dTerrain mTerrain;

	[NonSerialized]
	public UnityEngine.Object EditorReference;

	public e2dTerrain Terrain
	{
		get
		{
			if (mTerrain == null)
			{
				mTerrain = GetComponent<e2dTerrain>();
			}
			return mTerrain;
		}
	}

	public int NodeCount => 1 + Mathf.RoundToInt(TargetSize.x / NodeStepSize);

	private void OnEnable()
	{
		EditorReference = null;
	}

	private void SetPointsToCurve(Vector2[] points, bool fullRebuild)
	{
		if (fullRebuild || Terrain.TerrainCurve.Count == 0)
		{
			Terrain.AddCurvePoints(points, 0, Terrain.TerrainCurve.Count - 1);
			return;
		}
		int[] curveIndicesInTargetArea = GetCurveIndicesInTargetArea();
		if (curveIndicesInTargetArea[0] == -2)
		{
			float sqrMagnitude = (Terrain.TerrainCurve[0].position - points[^1]).sqrMagnitude;
			float sqrMagnitude2 = (Terrain.TerrainCurve[Terrain.TerrainCurve.Count - 1].position - points[0]).sqrMagnitude;
			if (sqrMagnitude < sqrMagnitude2)
			{
				curveIndicesInTargetArea[0] = 0;
				curveIndicesInTargetArea[1] = -1;
			}
			else
			{
				curveIndicesInTargetArea[0] = Terrain.TerrainCurve.Count;
				curveIndicesInTargetArea[1] = Terrain.TerrainCurve.Count - 1;
			}
		}
		Terrain.AddCurvePoints(points, curveIndicesInTargetArea[0], curveIndicesInTargetArea[1]);
	}

	private int[] GetCurveIndicesInTargetArea()
	{
		Vector2[] targetAreaBoundary = GetTargetAreaBoundary();
		int num = -2;
		for (int i = 1; i < Terrain.TerrainCurve.Count; i++)
		{
			if (e2dUtils.SegmentIntersectsPolygon(Terrain.TerrainCurve[i - 1].position, Terrain.TerrainCurve[i].position, targetAreaBoundary))
			{
				num = i;
				break;
			}
		}
		if (e2dUtils.PointInConvexPolygon(Terrain.TerrainCurve[0].position, targetAreaBoundary))
		{
			num = 0;
		}
		int num2 = -2;
		for (int num3 = Terrain.TerrainCurve.Count - 1; num3 >= 1; num3--)
		{
			if (e2dUtils.SegmentIntersectsPolygon(Terrain.TerrainCurve[num3 - 1].position, Terrain.TerrainCurve[num3].position, targetAreaBoundary))
			{
				num2 = num3 - 1;
				break;
			}
		}
		if (e2dUtils.PointInConvexPolygon(Terrain.TerrainCurve[Terrain.TerrainCurve.Count - 1].position, targetAreaBoundary))
		{
			num2 = Terrain.TerrainCurve.Count - 1;
		}
		return new int[2] { num, num2 };
	}

	public Rect GetTargetAreaBoundingBox()
	{
		Rect result = default(Rect);
		result.xMin = float.MaxValue;
		result.yMin = float.MaxValue;
		result.xMax = float.MinValue;
		result.yMax = float.MinValue;
		Vector2[] targetAreaBoundary = GetTargetAreaBoundary();
		for (int i = 0; i < targetAreaBoundary.Length; i++)
		{
			Vector2 vector = targetAreaBoundary[i];
			if (vector.x < result.xMin)
			{
				result.xMin = vector.x;
			}
			if (vector.y < result.yMin)
			{
				result.yMin = vector.y;
			}
			if (vector.x > result.xMax)
			{
				result.xMax = vector.x;
			}
			if (vector.y > result.yMax)
			{
				result.yMax = vector.y;
			}
		}
		return result;
	}

	public Vector2[] GetTargetAreaBoundary()
	{
		return new Vector2[4]
		{
			TransformPointFromTargetArea(new Vector3(-0.5f * TargetSize.x, -0.5f * TargetSize.y)),
			TransformPointFromTargetArea(new Vector3(-0.5f * TargetSize.x, 0.5f * TargetSize.y)),
			TransformPointFromTargetArea(new Vector3(0.5f * TargetSize.x, 0.5f * TargetSize.y)),
			TransformPointFromTargetArea(new Vector3(0.5f * TargetSize.x, -0.5f * TargetSize.y))
		};
	}

	public Rect GetTargetAreaLocalBox()
	{
		return new Rect(-0.5f * TargetSize.x, -0.5f * TargetSize.y, TargetSize.x, TargetSize.y);
	}

	public Vector3 TransformPointFromTargetArea(Vector3 point)
	{
		point = Quaternion.Euler(0f, 0f, TargetAngle) * point;
		point.x += TargetPosition.x;
		point.y += TargetPosition.y;
		return point;
	}

	public Vector3 TransformPointIntoTargetArea(Vector3 point)
	{
		point.x -= TargetPosition.x;
		point.y -= TargetPosition.y;
		point = Quaternion.Euler(0f, 0f, 0f - TargetAngle) * point;
		return point;
	}

	public void FixTargetArea()
	{
		TargetSize.x = Mathf.Max(TargetSize.x, float.Epsilon);
		TargetSize.y = Mathf.Max(TargetSize.y, float.Epsilon);
		Rect targetAreaLocalBox = GetTargetAreaLocalBox();
		for (int i = 0; i < Peaks.Count; i++)
		{
			Vector3 vector = Peaks[i].position;
			if (vector.x < targetAreaLocalBox.xMin)
			{
				vector.x = targetAreaLocalBox.xMin;
			}
			if (vector.y < targetAreaLocalBox.yMin)
			{
				vector.y = targetAreaLocalBox.yMin;
			}
			if (vector.x > targetAreaLocalBox.xMax)
			{
				vector.x = targetAreaLocalBox.xMax;
			}
			if (vector.y > targetAreaLocalBox.yMax)
			{
				vector.y = targetAreaLocalBox.yMax;
			}
			Peaks[i].position = vector;
		}
	}

	public void GenerateCurve()
	{
		float[] debugHeightmap = null;
		GenerateCurve(ref debugHeightmap);
	}

	public void GenerateCurve(ref float[] debugHeightmap)
	{
		if (NodeCount < 2)
		{
			return;
		}
		float num = 0f;
		float[] curveBlendingWeights = CurveBlendingWeights;
		foreach (float num2 in curveBlendingWeights)
		{
			num += num2;
		}
		float[] array = new float[NodeCount];
		float[] array2 = new float[NodeCount];
		for (int j = 0; j < NodeCount; j++)
		{
			array[j] = 0.5f;
		}
		bool flag = true;
		for (int k = 0; k < 4; k++)
		{
			if (num == 0f || CurveBlendingWeights[k] == 0f)
			{
				continue;
			}
			switch (k)
			{
			case 0:
			{
				bool num4 = GenerateCurvePerlin(array2, GetTargetAreaLocalBox(), ref debugHeightmap);
				if (num4)
				{
					NormalizeHeightmap(array2);
				}
				if (!num4)
				{
					flag = false;
				}
				break;
			}
			case 1:
			{
				bool num5 = GenerateCurveMidpoint(array2, GetTargetAreaLocalBox(), ref debugHeightmap);
				if (num5)
				{
					NormalizeHeightmap(array2);
				}
				if (!num5)
				{
					flag = false;
				}
				break;
			}
			case 2:
				if (!GenerateCurveVoronoi(array2, GetTargetAreaLocalBox(), ref debugHeightmap))
				{
					flag = false;
				}
				break;
			case 3:
			{
				bool num3 = GenerateCurveWalk(array2, GetTargetAreaLocalBox(), ref debugHeightmap);
				if (num3)
				{
					NormalizeHeightmap(array2);
				}
				if (!num3)
				{
					flag = false;
				}
				break;
			}
			}
			for (int l = 0; l < array.Length; l++)
			{
				array[l] += (array2[l] - 0.5f) * CurveBlendingWeights[k] / num;
			}
		}
		if (flag)
		{
			NormalizeHeightmap(array);
		}
		Vector2[] targetAreaBoundary = GetTargetAreaBoundary();
		Vector2[] array3 = new Vector2[NodeCount];
		for (int m = 0; m < NodeCount; m++)
		{
			float num6 = (float)m / (float)(NodeCount - 1);
			array3[m] = (1f - num6) * targetAreaBoundary[0] + num6 * targetAreaBoundary[3];
			array3[m] += array[m] * (targetAreaBoundary[1] - targetAreaBoundary[0]);
		}
		SetPointsToCurve(array3, FullRebuild);
		Terrain.CurveClosed = false;
		Terrain.FixCurve();
		Terrain.FixBoundary();
		Terrain.RebuildAllMaterials();
		Terrain.RebuildAllMeshes();
	}

	private void NormalizeHeightmap(float[] heightMap)
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		foreach (float num3 in heightMap)
		{
			if (num3 < num)
			{
				num = num3;
			}
			if (num3 > num2)
			{
				num2 = num3;
			}
		}
		if (!(num2 - num <= float.Epsilon))
		{
			for (int j = 0; j < heightMap.Length; j++)
			{
				heightMap[j] = (heightMap[j] - num) / (num2 - num);
			}
		}
	}

	private List<Vector2> PreparePeaks(int totalPeakCount, float peakRatio, bool includeCustomPeaks, Rect targetRect)
	{
		List<Vector2> list = new List<Vector2>();
		totalPeakCount = Mathf.Max(totalPeakCount, 2);
		bool flag = true;
		if (includeCustomPeaks)
		{
			foreach (e2dGeneratorPeak peak in Peaks)
			{
				float x = (peak.position.x - targetRect.xMin) / targetRect.width;
				float num = (peak.position.y - targetRect.yMin) / targetRect.height;
				list.Add(new Vector2(x, num));
				if (Mathf.Approximately(num, 1f))
				{
					flag = false;
				}
			}
		}
		while (list.Count < totalPeakCount)
		{
			float y = 0f;
			if (UnityEngine.Random.value <= peakRatio)
			{
				y = UnityEngine.Random.value;
			}
			list.Add(new Vector2(UnityEngine.Random.value, y));
		}
		if (flag)
		{
			float num2 = float.MinValue;
			int num3 = (includeCustomPeaks ? Peaks.Count : 0);
			for (int i = num3; i < list.Count; i++)
			{
				if (list[i].y > num2)
				{
					num2 = list[i].y;
				}
			}
			if (num2 > float.Epsilon)
			{
				for (int j = num3; j < list.Count; j++)
				{
					Vector2 value = list[j];
					value.y /= num2;
					list[j] = value;
				}
			}
		}
		return list;
	}

	private bool GenerateCurvePerlin(float[] heightMap, Rect targetRect, ref float[] debugHeightmap)
	{
		int a = 1 + Mathf.RoundToInt(Perlin.frequencyPerUnit * targetRect.width);
		a = Mathf.Max(a, 2);
		e2dPerlinNoise e2dPerlinNoise2 = new e2dPerlinNoise(Perlin.octaves, 1f, a, Perlin.persistence);
		e2dPerlinNoise2.Regenerate();
		for (int i = 0; i < heightMap.Length; i++)
		{
			float x = (float)i / (float)(heightMap.Length - 1);
			heightMap[i] = e2dPerlinNoise2.GetValue(x);
		}
		if (e2dUtils.DEBUG_GENERATOR_CURVE)
		{
			debugHeightmap = new float[10 * heightMap.Length];
			for (int j = 0; j < debugHeightmap.Length; j++)
			{
				float x2 = (float)j / (float)(debugHeightmap.Length - 1);
				debugHeightmap[j] = e2dPerlinNoise2.GetValue(x2) * targetRect.height;
			}
		}
		return true;
	}

	private bool GenerateCurveVoronoi(float[] heightMap, Rect targetRect, ref float[] debugHeightmap)
	{
		int totalPeakCount = Mathf.RoundToInt(Voronoi.frequencyPerUnit * targetRect.width);
		e2dVoronoi e2dVoronoi2 = new e2dVoronoi(PreparePeaks(totalPeakCount, Voronoi.peakRatio, Voronoi.usePeaks, targetRect), Voronoi.peakType, Voronoi.peakWidth);
		for (int i = 0; i < heightMap.Length; i++)
		{
			float x = (float)i / (float)(heightMap.Length - 1);
			heightMap[i] = e2dVoronoi2.GetValue(x);
		}
		if (e2dUtils.DEBUG_GENERATOR_CURVE)
		{
			debugHeightmap = new float[10 * heightMap.Length];
			for (int j = 0; j < debugHeightmap.Length; j++)
			{
				float x2 = (float)j / (float)(debugHeightmap.Length - 1);
				debugHeightmap[j] = e2dVoronoi2.GetValue(x2) * targetRect.height;
			}
		}
		return !Voronoi.usePeaks;
	}

	private bool GenerateCurveMidpoint(float[] heightMap, Rect targetRect, ref float[] debugHeightmap)
	{
		List<Vector2> peaks = null;
		if (Midpoint.usePeaks)
		{
			peaks = PreparePeaks(Peaks.Count, 0f, includeCustomPeaks: true, targetRect);
		}
		int initialStep = Mathf.RoundToInt((float)heightMap.Length / (Midpoint.frequencyPerUnit * targetRect.width));
		e2dMidpoint e2dMidpoint2 = new e2dMidpoint(heightMap.Length, initialStep, Midpoint.roughness, peaks);
		e2dMidpoint2.Regenerate();
		for (int i = 0; i < heightMap.Length; i++)
		{
			heightMap[i] = e2dMidpoint2.GetValueAt(i);
			if (Midpoint.usePeaks)
			{
				heightMap[i] = Mathf.Clamp01(heightMap[i]);
			}
		}
		if (e2dUtils.DEBUG_GENERATOR_CURVE)
		{
			debugHeightmap = new float[heightMap.Length];
			for (int j = 0; j < heightMap.Length; j++)
			{
				debugHeightmap[j] = e2dMidpoint2.GetValueAt(j) * targetRect.height;
			}
		}
		return !Midpoint.usePeaks;
	}

	private bool GenerateCurveWalk(float[] heightMap, Rect targetRect, ref float[] debugHeightmap)
	{
		float num = 1f / (float)(heightMap.Length - 1);
		float num2 = targetRect.width / (float)(heightMap.Length - 1);
		float value = 1f / (Walk.frequencyPerUnit * targetRect.width);
		value = Mathf.Clamp01(value);
		float num3 = 0.5f;
		float num4 = 0f;
		float num5 = num3;
		float num6 = num3;
		heightMap[0] = num3;
		for (int i = 1; i < heightMap.Length; i++)
		{
			float num7 = (float)i / (float)(heightMap.Length - 1);
			float num8 = Mathf.Min(num7 % value, value - num7 % value);
			num8 = 1f - num8 * 2f / value;
			num8 = Mathf.Pow(num8, 10f);
			float num9 = num8 * Walk.angleChangePerUnit * num2 * (2f * UnityEngine.Random.value - 1f);
			num4 = Mathf.Repeat(num4 + num9 + 180f, 360f) - 180f;
			num4 = Mathf.Clamp(num4, -80f, 80f);
			float num10 = Mathf.Tan(num4 * ((float)Math.PI / 180f)) * num;
			num3 += num10;
			float num11 = (e2dConstants.WALK_COHESION_MAX - Walk.cohesionPerUnit) * (float)i * num * 0.5f;
			float num12 = Mathf.Clamp(num3, num6 - num11, num5 + num11);
			if (num3 != num12)
			{
				num4 = 0f;
			}
			num3 = num12;
			num5 = Mathf.Min(num5, num3);
			num6 = Mathf.Max(num6, num3);
			heightMap[i] = num3;
		}
		if (e2dUtils.DEBUG_GENERATOR_CURVE)
		{
			debugHeightmap = new float[heightMap.Length];
			for (int j = 0; j < heightMap.Length; j++)
			{
				debugHeightmap[j] = heightMap[j] * targetRect.height;
			}
		}
		return true;
	}

	public void SmoothCurve()
	{
		int[] curveIndicesInTargetArea = GetCurveIndicesInTargetArea();
		int num = curveIndicesInTargetArea[1] - curveIndicesInTargetArea[0] + 1;
		if (curveIndicesInTargetArea[0] == -2 || num < 2)
		{
			return;
		}
		float[] array = new float[num];
		float[] array2 = new float[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = TransformPointIntoTargetArea(Terrain.TerrainCurve[curveIndicesInTargetArea[0] + i].position).y;
		}
		for (int j = 0; j < SmoothIterations; j++)
		{
			for (int k = 1; k < num - 1; k++)
			{
				float num2 = 0.5f * (array[k - 1] + array[k + 1]);
				num2 = 0.5f * (num2 + array[k]);
				array2[k] = num2;
			}
			for (int l = 1; l < num - 1; l++)
			{
				array[l] = array2[l];
			}
		}
		for (int m = 0; m < num; m++)
		{
			Vector2 vector = TransformPointIntoTargetArea(Terrain.TerrainCurve[curveIndicesInTargetArea[0] + m].position);
			vector.y = array[m];
			Terrain.TerrainCurve[curveIndicesInTargetArea[0] + m].position = TransformPointFromTargetArea(vector);
		}
		Terrain.FixCurve();
		Terrain.FixBoundary();
		Terrain.RebuildAllMaterials();
		Terrain.RebuildAllMeshes();
	}

	public void TextureTerrain()
	{
		if (TextureHeights.Count != Terrain.CurveTextures.Count)
		{
			return;
		}
		List<KeyValuePair<float, int>> list = new List<KeyValuePair<float, int>>();
		float num = float.MinValue;
		foreach (float textureHeight in TextureHeights)
		{
			num = Mathf.Max(num, textureHeight);
		}
		if (num <= 0f)
		{
			return;
		}
		for (int i = 0; i < TextureHeights.Count; i++)
		{
			if (TextureHeights[i] > 0f)
			{
				list.Add(new KeyValuePair<float, int>(TextureHeights[i] / num, i));
			}
		}
		list.Sort(new HeightComparer());
		int[] curveIndicesInTargetArea = GetCurveIndicesInTargetArea();
		int num2 = curveIndicesInTargetArea[1] - curveIndicesInTargetArea[0] + 1;
		if (curveIndicesInTargetArea[0] == -2 || num2 < 1)
		{
			return;
		}
		Rect targetAreaLocalBox = GetTargetAreaLocalBox();
		for (int j = 0; j < num2; j++)
		{
			Vector2 vector = TransformPointIntoTargetArea(Terrain.TerrainCurve[curveIndicesInTargetArea[0] + j].position);
			float num3 = (vector.y + 0.5f * targetAreaLocalBox.height) / targetAreaLocalBox.height;
			foreach (KeyValuePair<float, int> item in list)
			{
				if (num3 <= item.Key)
				{
					Terrain.TerrainCurve[curveIndicesInTargetArea[0] + j].texture = item.Value;
					break;
				}
			}
			if (j > 0 && j < num2 - 1)
			{
				Vector2 vector2 = TransformPointIntoTargetArea(Terrain.TerrainCurve[curveIndicesInTargetArea[0] + j - 1].position);
				Vector2 vector3 = TransformPointIntoTargetArea(Terrain.TerrainCurve[curveIndicesInTargetArea[0] + j + 1].position);
				float num4 = Mathf.Atan2(vector.y - vector2.y, vector.x - vector2.x) * 57.29578f;
				float num5 = Mathf.Atan2(vector3.y - vector.y, vector3.x - vector.x) * 57.29578f;
				if ((num4 >= CliffStartAngle && num5 >= CliffStartAngle) || (num4 <= 0f - CliffStartAngle && num5 <= 0f - CliffStartAngle))
				{
					Terrain.TerrainCurve[curveIndicesInTargetArea[0] + j].texture = CliffTextureIndex;
				}
			}
		}
		Terrain.RebuildAllMaterials();
		Terrain.RebuildAllMeshes();
	}
}
