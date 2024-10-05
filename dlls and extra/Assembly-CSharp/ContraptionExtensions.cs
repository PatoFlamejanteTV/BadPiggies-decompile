using System;
using System.Collections.Generic;
using UnityEngine;

public static class ContraptionExtensions
{
	private static ContraptionExtensionData Data => INContraption.Instance.ExtensionData;

	public static void Initialize()
	{
		OnConnectedComponentsChanged();
		Contraption.Instance.ConnectedComponentsChangedEvent += OnConnectedComponentsChanged;
	}

	public static void CreateAndSaveContraption(this Contraption contraption, string currentContraptionName)
	{
		ContraptionDataset contraptionDataset = new ContraptionDataset();
		foreach (BasePart part in contraption.Parts)
		{
			contraptionDataset.AddPart(part.m_coordX, part.m_coordY, (int)part.m_partType, part.customPartIndex, part.m_gridRotation, part.m_flipped);
		}
		WPFPrefs.SaveContraptionDataset(currentContraptionName, contraptionDataset);
	}

	public static List<BasePart> GetConnectedParts(this Contraption contraption, BasePart part)
	{
		return Data.ConnectedParts[part.ConnectedComponent];
	}

	public static List<BasePart> GetConnectedParts(this Contraption contraption, int index)
	{
		return Data.ConnectedParts[index];
	}

	public static List<Joint> FindPartJointsFast(this Contraption contraption, BasePart part)
	{
		return FindPartJointsInternal(part, (Joint joint) => true);
	}

	public static List<Joint> FindPartFixedJointsFast(this Contraption contraption, BasePart part)
	{
		return FindPartJointsInternal(part, (Joint joint) => joint is FixedJoint || joint is HingeJoint);
	}

	public static bool IsGearBoxEnabled(this Contraption contraption, int connectedComponent)
	{
		Contraption.ConnectedComponent connectedComponent2 = contraption.ConnectedComponents[connectedComponent];
		if (connectedComponent2.hasGearbox)
		{
			return connectedComponent2.gearbox.IsEnabled();
		}
		return false;
	}

	public static bool IsGearBoxEnabled(this Contraption contraption, BasePart part)
	{
		return contraption.IsGearBoxEnabled(part.ConnectedComponent);
	}

	public static void AddJointEdge(this Contraption contraption, BasePart partA, BasePart partB, Joint joint)
	{
		Dictionary<BasePart, List<(BasePart, Joint)>> jointGraph = Data.JointGraph;
		if (!jointGraph.TryGetValue(partA, out var value))
		{
			value = new List<(BasePart, Joint)>();
			jointGraph.Add(partA, value);
		}
		value.Add((partB, joint));
		if (!jointGraph.TryGetValue(partB, out value))
		{
			value = new List<(BasePart, Joint)>();
			jointGraph.Add(partB, value);
		}
		value.Add((partA, joint));
	}

	public static void UpdateConnectedComponents(this Contraption contraption)
	{
		contraption.m_jointDetached = true;
	}

	private static List<Joint> FindPartJointsInternal(BasePart part, Func<Joint, bool> match)
	{
		List<Joint> list = new List<Joint>();
		BasePart basePart = ((!(part.enclosedInto == null)) ? part.enclosedInto : ((part.enclosedPart == null) ? null : part.enclosedPart));
		ContraptionExtensionData data = Data;
		for (int i = 0; i < 2; i++)
		{
			BasePart basePart2 = ((i == 0) ? part : basePart);
			if (!(basePart2 != null) || !data.JointGraph.TryGetValue(basePart2, out var value))
			{
				continue;
			}
			foreach (var item2 in value)
			{
				Joint item = item2.Item2;
				if (item != null && match(item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	private static void OnConnectedComponentsChanged()
	{
	}
}
