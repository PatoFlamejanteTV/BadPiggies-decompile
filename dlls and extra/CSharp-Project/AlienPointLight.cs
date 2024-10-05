public class AlienPointLight : PointLight
{
	private void OnDestroy()
	{
		if ((bool)LightManager.Instance && LightManager.Instance.NightVisionOn)
		{
			LightManager.Instance.ToggleNightVision();
		}
	}

	protected override void OnTouch()
	{
		base.OnTouch();
		if (!base.contraption.HasNightVision && LightManager.Instance != null && (activated ^ LightManager.Instance.NightVisionOn))
		{
			LightManager.Instance.ToggleNightVision();
		}
	}
}
