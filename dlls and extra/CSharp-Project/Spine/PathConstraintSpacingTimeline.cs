namespace Spine;

public class PathConstraintSpacingTimeline : PathConstraintPositionTimeline
{
	public PathConstraintSpacingTimeline(int frameCount)
		: base(frameCount)
	{
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
				pathConstraint.spacing += (array[num + -1] - pathConstraint.spacing) * alpha;
				return;
			}
			int num2 = Animation.binarySearch(array, time, 2);
			float num3 = array[num2 + -1];
			float num4 = array[num2];
			float curvePercent = GetCurvePercent(num2 / 2 - 1, 1f - (time - num4) / (array[num2 + -2] - num4));
			pathConstraint.spacing += (num3 + (array[num2 + 1] - num3) * curvePercent - pathConstraint.spacing) * alpha;
		}
	}
}
