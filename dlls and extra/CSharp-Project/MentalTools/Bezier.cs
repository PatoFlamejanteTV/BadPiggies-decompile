using System;
using System.Collections.Generic;
using UnityEngine;

namespace MentalTools;

[Serializable]
public class Bezier
{
	[SerializeField]
	private List<BezierNode> nodes;

	public int Count
	{
		get
		{
			if (nodes != null)
			{
				return nodes.Count;
			}
			return 0;
		}
	}

	public BezierNode this[int index]
	{
		get
		{
			if (nodes != null && index >= 0 && index < nodes.Count)
			{
				return nodes[index];
			}
			return null;
		}
	}

	public Bezier()
	{
		nodes = new List<BezierNode>();
	}

	public BezierNode AddNode(Vector3 _pos, Vector3 _dir)
	{
		BezierNode bezierNode = new BezierNode(_pos, _dir, -_dir);
		nodes.Add(bezierNode);
		return bezierNode;
	}

	public BezierNode AddNode(Vector3 _pos, Vector3 _dir0, Vector3 _dir1)
	{
		BezierNode bezierNode = new BezierNode(_pos, _dir0, _dir1);
		nodes.Add(bezierNode);
		return bezierNode;
	}

	public void RemoveNode(int index)
	{
		nodes.RemoveAt(index);
	}

	public void ConfineToAxisSpace(MentalMath.AxisSpace space)
	{
		switch (space)
		{
		case MentalMath.AxisSpace.XY:
		{
			for (int j = 0; j < nodes.Count; j++)
			{
				nodes[j].Position = new Vector3(nodes[j].Position.x, nodes[j].Position.y, 0f);
				nodes[j].BackwardTangent = new Vector3(nodes[j].BackwardTangent.x, nodes[j].BackwardTangent.y, 0f);
				nodes[j].ForwardTangent = new Vector3(nodes[j].ForwardTangent.x, nodes[j].ForwardTangent.y, 0f);
			}
			break;
		}
		case MentalMath.AxisSpace.XZ:
		{
			for (int k = 0; k < nodes.Count; k++)
			{
				nodes[k].Position = new Vector3(nodes[k].Position.x, 0f, nodes[k].Position.z);
				nodes[k].BackwardTangent = new Vector3(nodes[k].BackwardTangent.x, 0f, nodes[k].Position.z);
				nodes[k].ForwardTangent = new Vector3(nodes[k].ForwardTangent.x, 0f, nodes[k].Position.z);
			}
			break;
		}
		case MentalMath.AxisSpace.YZ:
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].Position = new Vector3(0f, nodes[i].Position.y, nodes[i].Position.z);
				nodes[i].BackwardTangent = new Vector3(0f, nodes[i].BackwardTangent.y, nodes[i].Position.z);
				nodes[i].ForwardTangent = new Vector3(0f, nodes[i].ForwardTangent.y, nodes[i].Position.z);
			}
			break;
		}
		}
	}

	public Vector3 GetPoint(float ct, bool loop)
	{
		return GetPoint(ct, loop, Vector3.zero);
	}

	public Vector3 GetPoint(float ct, bool loop, Vector3 root)
	{
		if (nodes == null || nodes.Count < 2)
		{
			return Vector3.zero;
		}
		int count = nodes.Count;
		ct = Mathf.Clamp01(ct);
		float num = ct / (1f / (float)((!loop) ? (count - 1) : count));
		int num2 = ((!Mathf.Approximately(ct, 1f)) ? Mathf.Clamp(Mathf.FloorToInt(num), 0, (!loop) ? (count - 2) : (count - 1)) : ((!loop) ? (count - 2) : (count - 1)));
		float num3 = ((!Mathf.Approximately(ct, 1f)) ? (num % 1f) : 1f);
		Vector3 position = nodes[num2].Position;
		Vector3 vector = position + nodes[num2].ForwardTangent;
		Vector3 position2 = nodes[(num2 + 1 < count) ? (num2 + 1) : 0].Position;
		Vector3 vector2 = position2 + nodes[(num2 + 1 < count) ? (num2 + 1) : 0].BackwardTangent;
		return root + (Mathf.Pow(1f - num3, 3f) * position + 3f * Mathf.Pow(1f - num3, 2f) * num3 * vector + 3f * (1f - num3) * Mathf.Pow(num3, 2f) * vector2 + Mathf.Pow(num3, 3f) * position2);
	}
}
