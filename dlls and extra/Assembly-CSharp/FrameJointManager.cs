using System.Collections.Generic;
using UnityEngine;

public class FrameJointManager : PartManager
{
	private int m_jointCount;

	private bool m_initialized;

	public static FrameJointManager Instance { get; private set; }

	public int JointCount => m_jointCount;

	protected override void Initialize()
	{
		base.Initialize();
		m_status = StatusCode.Running;
		Instance = this;
	}

	public override void FixedUpdate()
	{
		if (!m_initialized)
		{
			AddJoints(Contraption.Instance.Parts);
			m_initialized = true;
		}
	}

	public override void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void AddJoints(List<BasePart> parts)
	{
		List<BasePart> list = new List<BasePart>();
		foreach (BasePart part in Contraption.Instance.Parts)
		{
			if (part.IsBracketFrame())
			{
				list.Add(part);
			}
		}
		int count = list.Count;
		DisjointSet disjointSet = new DisjointSet(count);
		for (int i = 0; i < count; i++)
		{
			for (int j = i + 1; j < count; j++)
			{
				BasePart basePart = list[i];
				BasePart basePart2 = list[j];
				int num = basePart2.m_coordX - basePart.m_coordX;
				int num2 = basePart2.m_coordY - basePart.m_coordY;
				if (num * num + num2 * num2 == 1)
				{
					disjointSet.Union(i, j);
				}
			}
		}
		int componentCount;
		int[] componentIndexes = disjointSet.GetComponentIndexes(out componentCount);
		Dictionary<BasePart, int> dictionary = new Dictionary<BasePart, int>();
		for (int k = 0; k < count; k++)
		{
			int value = componentIndexes[k];
			dictionary[list[k]] = value;
		}
		List<(BasePart, byte)> list2 = new List<(BasePart, byte)>();
		float breakForce = (Contraption.Instance.HasSuperGlue ? float.PositiveInfinity : (WPFMonoBehaviour.gameData.m_jointConnectionStrengthHigh * INSettings.GetFloat(INFeature.ConnectionStrength)));
		foreach (BasePart part2 in parts)
		{
			byte b = 0;
			BasePart enclosedPart = part2.m_enclosedPart;
			bool flag = enclosedPart != null && enclosedPart.m_partType == BasePart.PartType.SpringBoxingGlove && enclosedPart.customPartIndex == 4;
			if (part2.m_partType == BasePart.PartType.MetalFrame && flag)
			{
				b = (byte)(b | 1u);
			}
			if (part2.m_partType == BasePart.PartType.WoodenFrame && flag)
			{
				b = (byte)(b | 2u);
			}
			if (part2.IsLightFrame())
			{
				b = (byte)(b | 4u);
			}
			if (part2.IsBracketFrame())
			{
				b = (byte)(b | 8u);
			}
			if (b != 0)
			{
				list2.Add((part2, b));
			}
		}
		for (int l = 0; l < list2.Count; l++)
		{
			for (int m = l + 1; m < list2.Count; m++)
			{
				(BasePart, byte) tuple = list2[l];
				(BasePart, byte) tuple2 = list2[m];
				BasePart item = tuple.Item1;
				BasePart item2 = tuple2.Item1;
				byte num3 = (byte)(tuple.Item2 & tuple2.Item2);
				byte b2 = (byte)(num3 & 1u);
				byte b3 = (byte)(num3 & 2u);
				byte b4 = (byte)(num3 & 8u);
				if (num3 != 0 && ((b4 > 0) ? (dictionary[item] == dictionary[item2]) : (item.StrictConnectedComponent == item2.StrictConnectedComponent)))
				{
					Vector3 position = item.transform.position;
					Vector3 position2 = item2.transform.position;
					float num4 = position.x - position2.x;
					float num5 = position.y - position2.y;
					float num6 = ((b2 > 0) ? 32f : ((b3 > 0) ? 16f : 8f));
					if (num4 * num4 + num5 * num5 < num6 * num6)
					{
						FixedJoint fixedJoint = item.gameObject.AddComponent<FixedJoint>();
						fixedJoint.connectedBody = item2.rigidbody;
						fixedJoint.breakForce = breakForce;
						FixedJoint fixedJoint2 = item2.gameObject.AddComponent<FixedJoint>();
						fixedJoint2.connectedBody = item.rigidbody;
						fixedJoint2.breakForce = breakForce;
						m_jointCount += 2;
					}
				}
			}
		}
	}
}
