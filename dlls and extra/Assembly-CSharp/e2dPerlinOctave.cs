using System;
using UnityEngine;

public class e2dPerlinOctave
{
	private float mAmplitude;

	private int mFrequency;

	private float[] mNoise;

	public e2dPerlinOctave(float amplitude, int frequency)
	{
		mAmplitude = amplitude;
		mFrequency = frequency;
		mNoise = new float[mFrequency];
	}

	public void Regenerate()
	{
		for (int i = 0; i < mFrequency; i++)
		{
			mNoise[i] = mAmplitude * UnityEngine.Random.value;
		}
	}

	public float GetValue(float x01)
	{
		float num = x01 * (float)(mFrequency - 1);
		int num2 = Mathf.FloorToInt(num);
		int num3 = Mathf.CeilToInt(num);
		float x2 = num - (float)num2;
		return InterpolateCosine(mNoise[num2], mNoise[num3], x2);
	}

	private float InterpolateCosine(float a, float b, float x)
	{
		float f = x * (float)Math.PI;
		float num = (1f - Mathf.Cos(f)) * 0.5f;
		return a * (1f - num) + b * num;
	}

	private float InterpolateCubic(float v0, float v1, float v2, float v3, float x)
	{
		float num = v3 - v2 - (v0 - v1);
		float num2 = v0 - v1 - num;
		float num3 = v2 - v0;
		return num * x * x * x + num2 * x * x + num3 * x + v1;
	}
}
