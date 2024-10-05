namespace Spine;

public class ScaleTimeline : TranslateTimeline
{
	public ScaleTimeline(int frameCount)
		: base(frameCount)
	{
	}

	public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha)
	{
		float[] array = frames;
		if (!(time < array[0]))
		{
			Bone bone = skeleton.bones.Items[boneIndex];
			if (time >= array[^3])
			{
				bone.scaleX += (bone.data.scaleX * array[array.Length + -2] - bone.scaleX) * alpha;
				bone.scaleY += (bone.data.scaleY * array[array.Length + -1] - bone.scaleY) * alpha;
				return;
			}
			int num = Animation.binarySearch(array, time, 3);
			float num2 = array[num + -2];
			float num3 = array[num + -1];
			float num4 = array[num];
			float curvePercent = GetCurvePercent(num / 3 - 1, 1f - (time - num4) / (array[num + -3] - num4));
			bone.scaleX += (bone.data.scaleX * (num2 + (array[num + 1] - num2) * curvePercent) - bone.scaleX) * alpha;
			bone.scaleY += (bone.data.scaleY * (num3 + (array[num + 2] - num3) * curvePercent) - bone.scaleY) * alpha;
		}
	}
}
