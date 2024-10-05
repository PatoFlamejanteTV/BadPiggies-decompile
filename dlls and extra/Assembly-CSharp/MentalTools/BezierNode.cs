using System;
using UnityEngine;

namespace MentalTools;

[Serializable]
public class BezierNode
{
	public enum TangentType
	{
		None,
		Free,
		Mirrored
	}

	[SerializeField]
	private Vector3 position;

	[SerializeField]
	private Vector3 tangent0;

	[SerializeField]
	private Vector3 tangent1;

	[SerializeField]
	private TangentType tangetType0 = TangentType.Free;

	[SerializeField]
	private TangentType tangetType1 = TangentType.Free;

	public Vector3 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public Vector3 ForwardTangent
	{
		get
		{
			return tangent0;
		}
		set
		{
			tangent0 = value;
		}
	}

	public Vector3 BackwardTangent
	{
		get
		{
			return tangent1;
		}
		set
		{
			tangent1 = value;
		}
	}

	public TangentType ForwardTangentType
	{
		get
		{
			return tangetType0;
		}
		set
		{
			tangetType0 = value;
		}
	}

	public TangentType BackwardTangentType
	{
		get
		{
			return tangetType1;
		}
		set
		{
			tangetType1 = value;
		}
	}

	public BezierNode()
	{
	}

	public BezierNode(Vector3 _position, Vector3 _tangent0, Vector3 _tangent1)
	{
		Set(_position, _tangent0, _tangent1);
	}

	public void Set(Vector3 _position, Vector3 _tangent0, Vector3 _tangent1)
	{
		position = _position;
		tangent0 = _tangent0;
		tangent1 = _tangent1;
	}

	public void SetTangentType(int _index, TangentType _type)
	{
		switch (_index)
		{
		case 0:
			tangetType0 = _type;
			break;
		case 1:
			tangetType1 = _type;
			break;
		}
	}
}
