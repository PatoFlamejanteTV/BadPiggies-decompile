using System.Collections.Generic;
using UnityEngine;

public class SeparatedFrameManager : PartManager
{
	protected override void Initialize()
	{
		base.Initialize();
		m_status = StatusCode.Running;
	}

	public override void Start()
	{
		List<Joint> list = new List<Joint>();
		foreach (Contraption.JointConnection item in Contraption.Instance.JointMap)
		{
			if (item.partA == null || item.partB == null || item.joint == null)
			{
				continue;
			}
			for (int i = 0; i < 2; i++)
			{
				Joint joint = item.joint;
				BasePart basePart = ((i == 0) ? item.partA : item.partB);
				BasePart basePart2 = ((i == 0) ? item.partB : item.partA);
				basePart = ((basePart.enclosedInto == null) ? basePart : basePart.enclosedInto);
				basePart2 = ((basePart2.enclosedInto == null) ? basePart2 : basePart2.enclosedInto);
				if (basePart.IsSeparatedFrame())
				{
					BasePart enclosedPart = basePart.enclosedPart;
					BasePart partB = (basePart2.CanEncloseParts() ? basePart2.enclosedPart : basePart2);
					if (!CanConnectTo(enclosedPart, partB))
					{
						list.Add(joint);
						break;
					}
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (Joint item2 in list)
		{
			Object.Destroy(item2);
		}
		Contraption.Instance.UpdateConnectedComponents();
	}

	private static bool CanConnectTo(BasePart partA, BasePart partB)
	{
		if (partA == null || partB == null)
		{
			return false;
		}
		int num = partB.m_coordX - partA.m_coordX;
		int num2 = partB.m_coordY - partA.m_coordY;
		if (num == 0 && num2 == 0)
		{
			return true;
		}
		BasePart.Direction direction;
		if (num == 1 && num2 == 0)
		{
			direction = BasePart.Direction.Right;
		}
		else if (num == 0 && num2 == 1)
		{
			direction = BasePart.Direction.Up;
		}
		else if (num == -1 && num2 == 0)
		{
			direction = BasePart.Direction.Left;
		}
		else
		{
			if (num != 0 || num2 != -1)
			{
				return false;
			}
			direction = BasePart.Direction.Down;
		}
		if (partA.CanConnectTo(direction))
		{
			return partB.CanConnectTo(BasePart.InverseDirection(direction));
		}
		return false;
	}
}
