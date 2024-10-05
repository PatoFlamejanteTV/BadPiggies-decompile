public class DecelerationLight : PointLight
{
	public override bool HasOnOffToggle()
	{
		return false;
	}

	protected override void OnTouch()
	{
		if (activated || !this.IsSinglePart())
		{
			base.OnTouch();
		}
	}

	public override void SetEnabled(bool enabled)
	{
		if (activated != enabled)
		{
			OnTouch();
		}
	}
}
