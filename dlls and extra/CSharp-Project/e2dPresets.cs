public class e2dPresets
{
	private static e2dPreset[][] sCurvePresets;

	public static e2dPreset[] GetCurvePresets(e2dGeneratorCurveMethod method)
	{
		if (sCurvePresets == null)
		{
			sCurvePresets = new e2dPreset[5][];
			sCurvePresets[0] = new e2dPreset[3]
			{
				new e2dCurvePerlinPreset
				{
					name = "Rolling Hills",
					octaves = 3,
					frequencyPerUnit = 0.02f,
					persistence = 0.3f
				},
				new e2dCurvePerlinPreset
				{
					name = "Jagged Plains",
					octaves = 11,
					frequencyPerUnit = 0.001f,
					persistence = 0.51f
				},
				new e2dCurvePerlinPreset
				{
					name = "Spiky Mountains",
					octaves = 2,
					frequencyPerUnit = 0.117f,
					persistence = 0.155f
				}
			};
			sCurvePresets[2] = new e2dPreset[3]
			{
				new e2dCurveVoronoiPreset
				{
					name = "Rolling Hills",
					frequencyPerUnit = 0.025f,
					peakRatio = 0.755f,
					peakWidth = 0.21f,
					peakType = e2dVoronoiPeakType.SINE
				},
				new e2dCurveVoronoiPreset
				{
					name = "Highland Plateaus",
					frequencyPerUnit = 0.012f,
					peakRatio = 0.664f,
					peakWidth = 0.21f,
					peakType = e2dVoronoiPeakType.QUADRATIC
				},
				new e2dCurveVoronoiPreset
				{
					name = "Scattered Mountains",
					frequencyPerUnit = 0.037f,
					peakRatio = 0.237f,
					peakWidth = 0.21f,
					peakType = e2dVoronoiPeakType.SINE
				}
			};
			sCurvePresets[1] = new e2dPreset[3]
			{
				new e2dCurveMidpointPreset
				{
					name = "Spiky Hills",
					frequencyPerUnit = 0.025f,
					roughness = 0.082f
				},
				new e2dCurveMidpointPreset
				{
					name = "Jagged Plains",
					frequencyPerUnit = 0.001f,
					roughness = 0.709f
				},
				new e2dCurveMidpointPreset
				{
					name = "Jagged Mountains",
					frequencyPerUnit = 0.084f,
					roughness = 0.327f
				}
			};
			sCurvePresets[3] = new e2dPreset[2]
			{
				new e2dCurveWalkPreset
				{
					name = "Rolling Hills",
					frequencyPerUnit = 0.5f,
					angleChangePerUnit = 56f,
					cohesionPerUnit = 0.855f
				},
				new e2dCurveWalkPreset
				{
					name = "Large Plains",
					frequencyPerUnit = 0.269f,
					angleChangePerUnit = 39f,
					cohesionPerUnit = 2f
				}
			};
		}
		return sCurvePresets[(int)method];
	}

	public static e2dPreset GetDefault(e2dGeneratorCurveMethod method)
	{
		return GetCurvePresets(method)[0].Clone();
	}
}
