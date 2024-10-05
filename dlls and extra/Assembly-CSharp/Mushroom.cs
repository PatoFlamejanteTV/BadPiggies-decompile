public class Mushroom : PointLight
{
	public override void Awake()
	{
		if (INSettings.GetBool(INFeature.LightSystem))
		{
			base.Awake();
			activeSprite.SetActive(activated);
			inactiveSprite.SetActive(!activated);
			lightSource = GetComponent<PointLightSource>();
		}
		else
		{
			activeSprite.SetActive(activated);
			inactiveSprite.SetActive(!activated);
			lightSource = GetComponent<PointLightSource>();
			lightSource.isEnabled = activated;
		}
	}

	public override bool HasOnOffToggle()
	{
		if (INSettings.GetBool(INFeature.LightSystem))
		{
			return true;
		}
		return false;
	}

	public override bool CanBeEnabled()
	{
		if (INSettings.GetBool(INFeature.LightSystem))
		{
			return true;
		}
		return false;
	}

	public override bool IsEnabled()
	{
		if (INSettings.GetBool(INFeature.LightSystem))
		{
			return activated;
		}
		return false;
	}

	public override void PostInitialize()
	{
		if (INSettings.GetBool(INFeature.LightSystem))
		{
			base.PostInitialize();
			return;
		}
		activated = true;
		activeSprite.SetActive(activated);
		inactiveSprite.SetActive(!activated);
		lightSource.isEnabled = activated;
		base.PostInitialize();
	}

	protected override void OnTouch()
	{
		if (INSettings.GetBool(INFeature.LightSystem))
		{
			activated = !activated;
			activeSprite.SetActive(activated);
			inactiveSprite.SetActive(!activated);
			if ((bool)lightSource)
			{
				lightSource.isEnabled = activated;
			}
			if ((bool)base.rigidbody)
			{
				base.rigidbody.WakeUp();
			}
		}
	}
}
