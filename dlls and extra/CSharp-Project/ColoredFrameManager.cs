using System.Collections.Generic;
using UnityEngine;

public class ColoredFrameManager : PartManager
{
	protected override void Initialize()
	{
		base.Initialize();
		m_status = (StatusCode)3;
	}

	public override void FixedUpdate()
	{
		Contraption instance = Contraption.Instance;
		List<ColoredFrame> list = new List<ColoredFrame>();
		List<ColoredFrame> list2 = new List<ColoredFrame>();
		foreach (BasePart part in instance.Parts)
		{
			if (part.IsColoredrame())
			{
				if (part.IsTransparentFrame())
				{
					list2.Add(part as ColoredFrame);
				}
				list.Add(part as ColoredFrame);
			}
		}
		if (!INSettings.GetBool(INFeature.CanColorTransparentFrame))
		{
			return;
		}
		float @float = INSettings.GetFloat(INFeature.TransparentFrameColorDecayRate);
		float float2 = INSettings.GetFloat(INFeature.TransparentFrameAlphaDecayRate);
		for (int i = 0; i < 2; i++)
		{
			foreach (ColoredFrame item in list2)
			{
				int num = 1;
				int num2 = 0;
				Color a = item.Color * item.Color.a;
				float num3 = item.Color.a;
				for (int j = 0; j < 4; j++)
				{
					BasePart basePart = instance.FindPartAt(item.m_coordX + num, item.m_coordY + num2);
					if (basePart != null && basePart is ColoredFrame coloredFrame)
					{
						float a2 = coloredFrame.Color.a;
						a += coloredFrame.Color * a2;
						num3 += a2;
					}
					int num4 = num;
					num = -num2;
					num2 = num4;
				}
				a /= num3;
				a = Color.Lerp(a, item.TransparentColor, @float);
				a.a = a.a * (1f - float2) + item.TransparentColor.a * float2;
				item.Color = a;
			}
		}
		foreach (ColoredFrame item2 in list2)
		{
			item2.UpdateRenderers();
		}
	}
}
