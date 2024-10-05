namespace Spine;

public class PathConstraintPositionTimeline : CurveTimeline
{
	public const int ENTRIES = 2;

	protected const int PREV_TIME = -2;

	protected const int PREV_VALUE = -1;

	protected const int VALUE = 1;

	internal int pathConstraintIndex;

	internal float[] frames;

	public int PathConstraintIndex
	{
		get
		{
			return pathConstraintIndex;
		}
		set
		{
			pathConstraintIndex = value;
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

	public PathConstraintPositionTimeline(int frameCount)
		: base(frameCount)
	{
		frames = new float[frameCount * 2];
	}

	public void SetFrame(int frameIndex, float time, float value)
	{
		frameIndex *= 2;
		frames[frameIndex] = time;
		frames[frameIndex + 1] = value;
	}

	public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> events, float alpha)
	{
		float[] array = frames;
		if (!(time < array[0]))
		{
			PathConstraint pathConstraint = skeleton.pathConstraints.Items[pathConstraintIndex];
			if (time >= array[^2])
			{
				int num = array.Length;
				pathConstraint.position += (array[num + -1] - pathConstraint.position) * alpha;
				return;
			}
			int num2 = Animation.binarySearch(array, time, 2);
			float num3 = array[num2 + -1];
			float num4 = array[num2];
			float curvePercent = GetCurvePercent(num2 / 2 - 1, 1f - (time - num4) / (array[num2 + -2] - num4));
			pathConstraint.position += (num3 + (array[num2 + 1] - num3) * curvePercent - pathConstraint.position) * alpha;
		}
	}
}
