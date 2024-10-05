using UnityEngine;

public class PointLight : BasePart
{
	public GameObject activeSprite;

	public GameObject inactiveSprite;

	protected bool activated;

	protected PointLightSource lightSource;

	public EntityLight EntityLight { get; set; }

	public override void Awake()
	{
		base.Awake();
		activeSprite.SetActive(activated);
		inactiveSprite.SetActive(!activated);
		lightSource = GetComponent<PointLightSource>();
	}

	public override bool CanBeEnabled()
	{
		return true;
	}

	public override bool HasOnOffToggle()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return activated;
	}

	public override bool IsIntegralPart()
	{
		return false;
	}

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override void OnDetach()
	{
		base.OnDetach();
	}

	protected override void OnTouch()
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

	public override void PrePlaced()
	{
		base.PrePlaced();
		if (INSettings.GetBool(INFeature.LightSystem) && this.IsEntityLight())
		{
			m_autoAlign = AutoAlignType.Rotate;
			EntityLight entityLight = base.gameObject.AddComponent<EntityLight>();
			entityLight.Width = INSettings.GetFloat(INFeature.CircleLightWidth) * 0.5f;
			entityLight.Length = INSettings.GetFloat(INFeature.CircleLightRadius);
			if (m_partTier == PartTier.Regular)
			{
				entityLight.Type = 2;
				entityLight.Angle = INSettings.GetFloat(INFeature.CircleLightAngle2);
			}
			else if (m_partTier == PartTier.Common)
			{
				entityLight.Type = 2;
				entityLight.Angle = INSettings.GetFloat(INFeature.CircleLightAngle1);
			}
			else if (m_partTier == PartTier.Rare)
			{
				entityLight.Type = 3;
				entityLight.Angle = INSettings.GetFloat(INFeature.CircleLightAngle2);
			}
			else if (m_partTier == PartTier.Epic)
			{
				entityLight.Type = 3;
				entityLight.Angle = INSettings.GetFloat(INFeature.CircleLightAngle1);
			}
			else if (m_partTier == PartTier.Legendary)
			{
				entityLight.Type = 4;
				entityLight.Angle = 360f;
			}
		}
	}

	public override void Initialize()
	{
		if (INSettings.GetBool(INFeature.LightSystem) && this.IsEntityLight())
		{
			EntityLight = GetComponent<EntityLight>();
		}
	}
}
