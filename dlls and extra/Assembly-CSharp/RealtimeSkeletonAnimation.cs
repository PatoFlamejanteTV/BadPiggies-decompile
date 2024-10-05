using Spine.Unity;

public class RealtimeSkeletonAnimation : SkeletonAnimation
{
	public override void Update()
	{
		Update(GameTime.RealTimeDelta);
	}
}
