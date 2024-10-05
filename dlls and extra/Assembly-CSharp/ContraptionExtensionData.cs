using System;
using System.Collections.Generic;
using UnityEngine;

public class ContraptionExtensionData
{
	public class ComponentsData
	{
		public Component[] Components { get; set; }

		public float Time { get; set; }

		public ComponentsData(Component[] components, float time)
		{
			Components = components;
			Time = time;
		}
	}

	public List<BasePart>[] ConnectedParts { get; set; }

	public Dictionary<Type, ComponentsData> Components { get; set; }

	public Dictionary<(int, int), (int, bool)> PartRotations { get; set; }

	public Dictionary<BasePart, List<(BasePart, Joint)>> JointGraph { get; set; }

	public ContraptionExtensionData()
	{
		ConnectedParts = Array.Empty<List<BasePart>>();
		Components = new Dictionary<Type, ComponentsData>();
		PartRotations = new Dictionary<(int, int), (int, bool)>();
		JointGraph = new Dictionary<BasePart, List<(BasePart, Joint)>>();
	}
}
