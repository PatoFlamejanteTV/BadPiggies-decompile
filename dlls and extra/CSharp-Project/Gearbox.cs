using UnityEngine;

public class Gearbox : BasePart
{
	public GameObject activeSprite;

	public GameObject inactiveSprite;

	private bool activated;

	private const string achievementId = "grp.LPA_GEARBOX";

	private bool achievementSent;

	public override void Awake()
	{
		base.Awake();
		activeSprite.SetActive(activated);
		inactiveSprite.SetActive(!activated);
	}

	public override bool CanBeEnabled()
	{
		return true;
	}

	public override Direction EffectDirection()
	{
		if (INSettings.GetBool(INFeature.RotatableGearbox))
		{
			return BasePart.Rotate(Direction.Right, m_gridRotation);
		}
		return Direction.Right;
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
		return true;
	}

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override bool ValidatePart()
	{
		return m_enclosedInto != null;
	}

	public override void OnDetach()
	{
		base.OnDetach();
	}

	protected override void OnTouch()
	{
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.gearboxSwitch, base.transform.position);
		activated = !activated;
		activeSprite.SetActive(activated);
		inactiveSprite.SetActive(!activated);
	}

	public override void PrePlaced()
	{
		base.PrePlaced();
		if (INSettings.GetBool(INFeature.RotatableGearbox))
		{
			m_autoAlign = AutoAlignType.Rotate;
		}
	}
}
