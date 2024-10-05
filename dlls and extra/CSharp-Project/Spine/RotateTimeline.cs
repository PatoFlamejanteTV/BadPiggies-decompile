namespace Spine;

public class RotateTimeline : CurveTimeline
{
	public const int ENTRIES = 2;

	internal const int PREV_TIME = -2;

	internal const int PREV_ROTATION = -1;

	internal const int ROTATION = 1;

	internal int boneIndex;

	internal float[] frames;

	public int BoneIndex
	{
		get
		{
			return boneIndex;
		}
		set
		{
			boneIndex = value;
		}
	}

	public float[] Frames
	{
		get
		{
			return frames;
		}
		set
		{
			frames = value;
		}
	}

	public RotateTimeline(int frameCount)
		: base(frameCount)
	{
		frames = new float[frameCount << 1];
	}

	public void SetFrame(int frameIndex, float time, float degrees)
	{
		frameIndex <<= 1;
		frames[frameIndex] = time;
		frames[frameIndex + 1] = degrees;
	}

	public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha)
	{
		float[] array = frames;
		if (time < array[0])
		{
			return;
		}
		Bone bone = skeleton.bones.Items[boneIndex];
		float num;
		if (time >= array[^2])
		{
			for (num = bone.data.rotation + array[array.Length + -1] - bone.rotation; num > 180f; num -= 360f)
			{
			}
			for (; num < -180f; num += 360f)
			{
			}
			bone.rotation += num * alpha;
			return;
		}
		int num2 = Animation.binarySearch(array, time, 2);
		float num3 = array[num2 + -1];
		float num4 = array[num2];
		float curvePercent = GetCurvePercent((num2 >> 1) - 1, 1f - (time - num4) / (array[num2 + -2] - num4));
		for (num = array[num2 + 1] - num3; num > 180f; num -= 360f)
		{
		}
		for (; num < -180f; num += 360f)
		{
		}
		for (num = bone.data.rotation + (num3 + num * curvePercent) - bone.rotation; num > 180f; num -= 360f)
		{
		}
		for (; num < -180f; num += 360f)
		{
		}
		bone.rotation += num * alpha;
	}
}
