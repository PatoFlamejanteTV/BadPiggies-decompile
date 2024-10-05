public static class PartExtensions
{
	public static bool IsSinglePart(this BasePart part)
	{
		return part.contraption.ComponentPartCount(part.ConnectedComponent) == 1;
	}

	public static bool HasMultipleRigidbodies(this BasePart part)
	{
		return part.m_partType == BasePart.PartType.Rope;
	}

	public static bool IsSeparatedFrame(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.MetalFrame)
		{
			return part.customPartIndex == 8;
		}
		return false;
	}

	public static bool IsLightFrame(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.MetalFrame)
		{
			return part.customPartIndex == 10;
		}
		return false;
	}

	public static bool IsAlienMetalFrame(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.MetalFrame)
		{
			return part.customPartIndex == 11;
		}
		return false;
	}

	public static bool IsColoredrame(this BasePart part)
	{
		if (part.m_partType != BasePart.PartType.MetalFrame || 12 > part.customPartIndex || part.customPartIndex > 129)
		{
			return part.IsTransparentFrame();
		}
		return true;
	}

	public static bool IsTransparentFrame(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.MetalFrame && 132 <= part.customPartIndex)
		{
			return part.customPartIndex <= 133;
		}
		return false;
	}

	public static bool IsBracketFrame(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.MetalFrame)
		{
			return part.customPartIndex == 131;
		}
		return false;
	}

	public static bool IsWoodenBox(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.WoodenFrame)
		{
			return part.customPartIndex == 10;
		}
		return false;
	}

	public static bool IsMetalBox(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.MetalFrame)
		{
			return part.customPartIndex == 130;
		}
		return false;
	}

	public static bool IsAvoidanceRocket(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.Rocket)
		{
			if (part.customPartIndex != 1)
			{
				return part.customPartIndex == 3;
			}
			return true;
		}
		return false;
	}

	public static bool IsTrackingRocket(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.RedRocket)
		{
			if (part.customPartIndex != 1)
			{
				return part.customPartIndex == 3;
			}
			return true;
		}
		return false;
	}

	public static bool IsHingePlate(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.Rope && 4 <= part.customPartIndex)
		{
			return part.customPartIndex <= 7;
		}
		return false;
	}

	public static bool IsMultipartGenerator(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.GrapplingHook && 8 <= part.customPartIndex)
		{
			return part.customPartIndex <= 10;
		}
		return false;
	}

	public static bool IsAutoGun(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.GrapplingHook)
		{
			return part.customPartIndex == 6;
		}
		return false;
	}

	public static bool IsElasticConnector(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.Kicker)
		{
			if (part.customPartIndex != 2)
			{
				return part.customPartIndex == 4;
			}
			return true;
		}
		return false;
	}

	public static bool IsAutoConnector(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.Kicker)
		{
			return part.customPartIndex == 1;
		}
		return false;
	}

	public static bool IsMarker(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.Kicker)
		{
			return part.customPartIndex == 3;
		}
		return false;
	}

	public static bool IsEntityLight(this BasePart part)
	{
		if (part.m_partType != BasePart.PartType.PointLight || 0 > part.customPartIndex || part.customPartIndex > 4)
		{
			if (part.m_partType == BasePart.PartType.SpotLight && 0 <= part.customPartIndex)
			{
				return part.customPartIndex <= 3;
			}
			return false;
		}
		return true;
	}

	public static bool IsDecelerationLight(this BasePart part)
	{
		if (part.m_partType == BasePart.PartType.PointLight)
		{
			return part.customPartIndex == 5;
		}
		return false;
	}
}
