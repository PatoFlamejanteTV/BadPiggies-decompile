using System;
using System.Collections.Generic;
using UnityEngine;

public class e2dVoronoi
{
	private class Vector2XComparer : IComparer<Vector2>
	{
		public int Compare(Vector2 x, Vector2 y)
		{
			if (!(x.x >= y.x))
			{
				return -1;
			}
			if (!(x.x <= y.x))
			{
				return 1;
			}
			return 0;
		}
	}

	private List<Vector2> mPeaks;

	private List<Vector2> mValleys;

	private e2dVoronoiPeakType mPeakType;

	private float mPeakWidth;

	public e2dVoronoi(List<Vector2> peaks, e2dVoronoiPeakType peakType, float peakWidth)
	{
		mPeaks = peaks;
		mPeakType = peakType;
		mPeakWidth = peakWidth;
		mPeaks.Sort(new Vector2XComparer());
		mValleys = new List<Vector2>(mPeaks.Count + 1);
		mValleys.Add(new Vector2(0f, 0f - mPeaks[0].x));
		for (int i = 1; i < mPeaks.Count; i++)
		{
			float num = Mathf.Abs(mPeaks[i].x - mPeaks[i - 1].x);
			float x = 0.5f * (mPeaks[i - 1].x + mPeaks[i].x);
			mValleys.Add(new Vector2(x, -0.5f * num));
		}
		mValleys.Add(new Vector2(1f, 0f - (1f - mPeaks[mPeaks.Count - 1].x)));
		float num2 = float.MaxValue;
		foreach (Vector2 mValley in mValleys)
		{
			if (mValley.y < num2)
			{
				num2 = mValley.y;
			}
		}
		for (int j = 0; j < mValleys.Count; j++)
		{
			Vector2 value = mValleys[j];
			value.y = (value.y - num2) / (0f - num2);
			if (j == 0)
			{
				value.y *= mPeaks[j].y;
			}
			else if (j == mValleys.Count - 1)
			{
				value.y *= mPeaks[j - 1].y;
			}
			else
			{
				value.y *= Mathf.Min(mPeaks[j - 1].y, mPeaks[j].y);
			}
			if (mPeakType == e2dVoronoiPeakType.LINEAR)
			{
				if (mPeakWidth <= 0.5f)
				{
					value.y *= 2f * mPeakWidth;
				}
				else if (j > 0 && j < mValleys.Count - 1)
				{
					float t = 2f * (mPeakWidth - 0.5f);
					value.y = Mathf.Lerp(value.y, Mathf.Min(mPeaks[j - 1].y, mPeaks[j].y), t);
				}
			}
			mValleys[j] = value;
		}
	}

	public float GetValue(float x)
	{
		int index = mPeaks.Count - 1;
		int index2 = mPeaks.Count;
		for (int i = 0; i < mPeaks.Count; i++)
		{
			if (x < mPeaks[i].x)
			{
				if (x < mValleys[i].x)
				{
					index = i - 1;
					index2 = i;
				}
				else
				{
					index = i;
					index2 = i;
				}
				break;
			}
		}
		float num = (x - mValleys[index2].x) / (mPeaks[index].x - mValleys[index2].x);
		if (float.IsNaN(num))
		{
			num = 0f;
		}
		float result = 0f;
		switch (mPeakType)
		{
		case e2dVoronoiPeakType.QUADRATIC:
		{
			num *= Mathf.Lerp(1f, e2dConstants.VORONOI_QUADRATIC_PEAK_WIDTH_RATIO, mPeakWidth);
			if (num > 1f)
			{
				result = mPeaks[index].y;
				break;
			}
			float num4 = mPeaks[index].y - mValleys[index2].y;
			result = mValleys[index2].y + num * num * num4;
			break;
		}
		case e2dVoronoiPeakType.SINE:
		{
			num = 1f - Mathf.Pow(1f - num, Mathf.Lerp(e2dConstants.VORONOI_SIN_POWER_MIN, e2dConstants.VORONOI_SIN_POWER_MAX, mPeakWidth));
			float f = -(float)Math.PI / 2f + num * (float)Math.PI;
			float num2 = 0.5f * (mPeaks[index].y - mValleys[index2].y);
			float num3 = 1f;
			result = mValleys[index2].y + (Mathf.Sin(f) + num3) * num2;
			break;
		}
		case e2dVoronoiPeakType.LINEAR:
			result = Mathf.Lerp(mValleys[index2].y, mPeaks[index].y, num);
			break;
		}
		return result;
	}
}
