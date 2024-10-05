using System.Collections.Generic;
using UnityEngine;

public class e2dMidpoint
{
	private float[] mCells;

	private int mInitialCellCount;

	private int mInitialStep;

	private float mRoughness;

	private List<Vector2> mPeaks;

	public e2dMidpoint(int cellCount, int initialStep, float roughness, List<Vector2> peaks)
	{
		mRoughness = roughness;
		mInitialCellCount = cellCount;
		cellCount = Mathf.NextPowerOfTwo(cellCount - 1) + 1;
		mCells = new float[cellCount];
		mInitialStep = Mathf.ClosestPowerOfTwo(initialStep);
		mInitialStep = Mathf.Clamp(mInitialStep, 1, cellCount - 1);
		mPeaks = peaks;
	}

	public void Regenerate()
	{
		int num = mInitialStep;
		float num2 = 0.9f;
		float num3 = Mathf.Pow(2f, Mathf.Lerp(e2dConstants.MIDPOINT_ROUGHNESS_POWER_MIN, e2dConstants.MIDPOINT_ROUGHNESS_POWER_MAX, mRoughness));
		for (int i = 0; i < mCells.Length; i++)
		{
			mCells[i] = -1f;
		}
		for (int j = 0; j < mCells.Length; j += mInitialStep)
		{
			mCells[j] = 0.5f - 0.5f * num2 + Random.value * num2;
		}
		num2 *= num3;
		if (mPeaks != null)
		{
			foreach (Vector2 mPeak in mPeaks)
			{
				int num4 = Mathf.RoundToInt(mPeak.x * (float)(mInitialCellCount - 1) / (float)mInitialStep);
				num4 *= mInitialStep;
				mCells[num4] = mPeak.y;
			}
		}
		while (num > 1)
		{
			for (int k = 0; k + num < mCells.Length; k += num)
			{
				int num5 = k + (num >> 1);
				int num6 = k + num;
				float num7 = 0.5f * (mCells[k] + mCells[num6]);
				num7 += -0.5f * num2 + Random.value * num2;
				mCells[num5] = num7;
			}
			num >>= 1;
			num2 *= num3;
		}
	}

	public float GetValueAt(int i)
	{
		return mCells[i];
	}
}
