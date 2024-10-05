public class Pumpkin : BasePart
{
	private bool m_enabled;

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override Direction EffectDirection()
	{
		if (INSettings.GetBool(INFeature.RotatablePumpkin))
		{
			return BasePart.Rotate(Direction.Right, m_gridRotation);
		}
		return Direction.Right;
	}

	public override bool IsEnabled()
	{
		if (INSettings.GetBool(INFeature.FixedPumpkin))
		{
			return m_enabled;
		}
		return false;
	}

	protected override void OnTouch()
	{
		if (INSettings.GetBool(INFeature.FixedPumpkin))
		{
			SetEnabled(!m_enabled);
		}
	}

	public override void SetEnabled(bool enabled)
	{
		if (INSettings.GetBool(INFeature.FixedPumpkin) && m_enabled != enabled)
		{
			m_enabled = enabled;
			FixedPumpkinManager.Instance.NeedsUpdate = true;
		}
	}

	public override void PrePlaced()
	{
		base.PrePlaced();
		if (INSettings.GetBool(INFeature.RotatablePumpkin))
		{
			m_autoAlign = AutoAlignType.Rotate;
		}
	}

	public override void PostInitialize()
	{
		if (INSettings.GetBool(INFeature.FixedPumpkin))
		{
			base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), 1);
			m_enabled = true;
		}
	}
}
