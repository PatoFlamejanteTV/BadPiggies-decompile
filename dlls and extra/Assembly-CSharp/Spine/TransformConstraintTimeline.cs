namespace Spine;

public class TransformConstraintTimeline : CurveTimeline
{
	public const int ENTRIES = 5;

	private const int PREV_TIME = -5;

	private const int PREV_ROTATE = -4;

	private const int PREV_TRANSLATE = -3;

	private const int PREV_SCALE = -2;

	private const int PREV_SHEAR = -1;

	private const int ROTATE = 1;

	private const int TRANSLATE = 2;

	private const int SCALE = 3;

	private const int SHEAR = 4;

	internal int transformConstraintIndex;

	internal float[] frames;

	public int TransformConstraintIndex
	{
		get
		{
			return transformConstraintIndex;
		}
		set
		{
			transformConstraintIndex = value;
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

	public TransformConstraintTimeline(int frameCount)
		: base(frameCount)
	{
		frames = new float[frameCount * 5];
	}

	public void SetFrame(int frameIndex, float time, float rotateMix, float translateMix, float scaleMix, float shearMix)
	{
		frameIndex *= 5;
		frames[frameIndex] = time;
		frames[frameIndex + 1] = rotateMix;
		frames[frameIndex + 2] = translateMix;
		frames[frameIndex + 3] = scaleMix;
		frames[frameIndex + 4] = shearMix;
	}

	public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha)
	{
		float[] array = frames;
		if (!(time < array[0]))
		{
			TransformConstraint transformConstraint = skeleton.transformConstraints.Items[transformConstraintIndex];
			if (time >= array[^5])
			{
				int num = array.Length;
				transformConstraint.rotateMix += (array[num + -4] - transformConstraint.rotateMix) * alpha;
				transformConstraint.translateMix += (array[num + -3] - transformConstraint.translateMix) * alpha;
				transformConstraint.scaleMix += (array[num + -2] - transformConstraint.scaleMix) * alpha;
				transformConstraint.shearMix += (array[num + -1] - transformConstraint.shearMix) * alpha;
				return;
			}
			int num2 = Animation.binarySearch(array, time, 5);
			float num3 = array[num2];
			float curvePercent = GetCurvePercent(num2 / 5 - 1, 1f - (time - num3) / (array[num2 + -5] - num3));
			float num4 = array[num2 + -4];
			float num5 = array[num2 + -3];
			float num6 = array[num2 + -2];
			float num7 = array[num2 + -1];
			transformConstraint.rotateMix += (num4 + (array[num2 + 1] - num4) * curvePercent - transformConstraint.rotateMix) * alpha;
			transformConstraint.translateMix += (num5 + (array[num2 + 2] - num5) * curvePercent - transformConstraint.translateMix) * alpha;
			transformConstraint.scaleMix += (num6 + (array[num2 + 3] - num6) * curvePercent - transformConstraint.scaleMix) * alpha;
			transformConstraint.shearMix += (num7 + (array[num2 + 4] - num7) * curvePercent - transformConstraint.shearMix) * alpha;
		}
	}
}
