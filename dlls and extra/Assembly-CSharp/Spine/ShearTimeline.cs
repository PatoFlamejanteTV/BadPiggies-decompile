namespace Spine;

public class ShearTimeline : TranslateTimeline
{
	public ShearTimeline(int frameCount)
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
				bone.shearX += (bone.data.shearX + array[array.Length + -2] - bone.shearX) * alpha;
				bone.shearY += (bone.data.shearY + array[array.Length + -1] - bone.shearY) * alpha;
				return;
			}
			int num = Animation.binarySearch(array, time, 3);
			float num2 = array[num + -2];
			float num3 = array[num + -1];
			float num4 = array[num];
			float curvePercent = GetCurvePercent(num / 3 - 1, 1f - (time - num4) / (array[num + -3] - num4));
			bone.shearX += (bone.data.shearX + (num2 + (array[num + 1] - num2) * curvePercent) - bone.shearX) * alpha;
			bone.shearY += (bone.data.shearY + (num3 + (array[num + 2] - num3) * curvePercent) - bone.shearY) * alpha;
		}
	}
}
