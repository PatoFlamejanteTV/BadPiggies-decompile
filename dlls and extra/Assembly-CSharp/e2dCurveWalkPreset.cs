using System;

[Serializable]
public class e2dCurveWalkPreset : e2dPreset
{
	public float angleChangePerUnit;

	public float frequencyPerUnit;

	public float cohesionPerUnit;

	public override void Copy(e2dPreset other)
	{
		e2dCurveWalkPreset e2dCurveWalkPreset2 = (e2dCurveWalkPreset)other;
		angleChangePerUnit = e2dCurveWalkPreset2.angleChangePerUnit;
		frequencyPerUnit = e2dCurveWalkPreset2.frequencyPerUnit;
		cohesionPerUnit = e2dCurveWalkPreset2.cohesionPerUnit;
	}

	public override void UpdateValues(e2dTerrainGenerator generator)
	{
		generator.Walk.Copy(this);
	}

	public override e2dPreset Clone()
	{
		e2dCurveWalkPreset obj = new e2dCurveWalkPreset();
		obj.Copy(this);
		return obj;
	}
}
