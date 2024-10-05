using System;
using UnityEngine;

[Serializable]
public class e2dCurveNode
{
	public Vector2 position;

	public int texture;

	public float grassRatio;

	private bool isSelected;

	public bool Selected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
		}
	}

	public e2dCurveNode(Vector2 _position)
	{
		position = _position;
		texture = 0;
		grassRatio = 0f;
	}

	public void Copy(e2dCurveNode other)
	{
		position = other.position;
		texture = other.texture;
		grassRatio = other.grassRatio;
	}

	public override bool Equals(object obj)
	{
		if (obj is e2dCurveNode)
		{
			return this == (e2dCurveNode)obj;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Mathf.RoundToInt(1000f * position.x + 1000f * position.y + (float)texture + 1000f * grassRatio);
	}

	public static bool operator ==(e2dCurveNode a, e2dCurveNode b)
	{
		return a.position == b.position;
	}

	public static bool operator !=(e2dCurveNode a, e2dCurveNode b)
	{
		return !(a == b);
	}
}
