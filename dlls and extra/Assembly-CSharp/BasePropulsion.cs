using System.Collections.Generic;

public abstract class BasePropulsion : BasePart
{
	public override bool ValidatePart()
	{
		if (!WPFMonoBehaviour.levelManager.RequireConnectedContraption)
		{
			return true;
		}
		List<BasePart> list = base.contraption.FindNeighbours(m_coordX, m_coordY);
		int num = 0;
		foreach (BasePart item in list)
		{
			if (item.IsPartOfChassis())
			{
				num++;
			}
		}
		return num >= 1;
	}
}
