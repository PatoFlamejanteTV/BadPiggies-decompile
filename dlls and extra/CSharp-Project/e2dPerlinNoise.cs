public class e2dPerlinNoise
{
	private int mOctavesCount;

	private e2dPerlinOctave[] mOctaves;

	public e2dPerlinNoise(int octaves, float amplitude, int frequency, float persistence)
	{
		mOctavesCount = octaves;
		if (frequency < 2)
		{
			frequency = 2;
		}
		if (amplitude <= 0f)
		{
			amplitude = 0.1f;
		}
		if (mOctavesCount < 1)
		{
			mOctavesCount = 1;
		}
		int num = frequency;
		float num2 = amplitude;
		mOctaves = new e2dPerlinOctave[mOctavesCount];
		for (int i = 0; i < mOctavesCount; i++)
		{
			mOctaves[i] = new e2dPerlinOctave(num2, num);
			num2 *= persistence;
			num *= 2;
		}
	}

	public void Regenerate()
	{
		for (int i = 0; i < mOctavesCount; i++)
		{
			mOctaves[i].Regenerate();
		}
	}

	public float GetValue(float x)
	{
		float num = 0f;
		for (int i = 0; i < mOctavesCount; i++)
		{
			num += mOctaves[i].GetValue(x);
		}
		return num;
	}
}
