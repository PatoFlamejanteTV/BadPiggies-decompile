using System;

[Serializable]
public class e2dCurveMidpointPreset : e2dPreset
{
	public float frequencyPerUnit;

	public float roughness;

	public bool usePeaks;

	public override void Copy(e2dPreset other)
	{
		e2dCurveMidpointPreset e2dCurveMidpointPreset2 = (e2dCurveMidpointPreset)other;
		frequencyPerUnit = e2dCurveMidpointPreset2.frequencyPerUnit;
		roughness = e2dCurveMidpointPreset2.roughness;
		usePeaks = e2dCurveMidpointPreset2.usePeaks;
	}

	public override void UpdateValues(e2dTerrainGenerator generator)
	{
		generator.Midpoint.Copy(this);
	}

	public override e2dPreset Clone()
	{
		e2dCurveMidpointPreset obj = new e2dCurveMidpointPreset();
		obj.Copy(this);
		return obj;
	}
}
