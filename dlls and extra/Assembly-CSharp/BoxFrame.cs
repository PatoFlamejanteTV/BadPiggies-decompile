public class BoxFrame : Frame
{
	public override bool CanEncloseParts()
	{
		return false;
	}

	public override void EnsureRigidbody()
	{
		base.EnsureRigidbody();
		if (m_material == FrameMaterial.Wood)
		{
			base.rigidbody.mass = INSettings.GetFloat(INFeature.WoodenBoxMass);
		}
		else if (m_material == FrameMaterial.Metal)
		{
			base.rigidbody.mass = INSettings.GetFloat(INFeature.MetalBoxMass);
		}
	}
}
