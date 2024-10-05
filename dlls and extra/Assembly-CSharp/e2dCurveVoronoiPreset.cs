using System;

[Serializable]
public class e2dCurveVoronoiPreset : e2dPreset
{
	public e2dVoronoiPeakType peakType;

	public float frequencyPerUnit;

	public float peakRatio;

	public float peakWidth;

	public bool usePeaks;

	public override void Copy(e2dPreset other)
	{
		e2dCurveVoronoiPreset e2dCurveVoronoiPreset2 = (e2dCurveVoronoiPreset)other;
		peakType = e2dCurveVoronoiPreset2.peakType;
		frequencyPerUnit = e2dCurveVoronoiPreset2.frequencyPerUnit;
		peakRatio = e2dCurveVoronoiPreset2.peakRatio;
		peakWidth = e2dCurveVoronoiPreset2.peakWidth;
		usePeaks = e2dCurveVoronoiPreset2.usePeaks;
	}

	public override void UpdateValues(e2dTerrainGenerator generator)
	{
		generator.Voronoi.Copy(this);
	}

	public override e2dPreset Clone()
	{
		e2dCurveVoronoiPreset obj = new e2dCurveVoronoiPreset();
		obj.Copy(this);
		return obj;
	}
}
