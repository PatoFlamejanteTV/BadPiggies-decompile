using System;

[Serializable]
public class e2dCurvePerlinPreset : e2dPreset
{
	public int octaves;

	public float frequencyPerUnit;

	public float persistence;

	public override void Copy(e2dPreset other)
	{
		e2dCurvePerlinPreset e2dCurvePerlinPreset2 = (e2dCurvePerlinPreset)other;
		octaves = e2dCurvePerlinPreset2.octaves;
		frequencyPerUnit = e2dCurvePerlinPreset2.frequencyPerUnit;
		persistence = e2dCurvePerlinPreset2.persistence;
	}

	public override void UpdateValues(e2dTerrainGenerator generator)
	{
		generator.Perlin.Copy(this);
	}

	public override e2dPreset Clone()
	{
		e2dCurvePerlinPreset obj = new e2dCurvePerlinPreset();
		obj.Copy(this);
		return obj;
	}
}
