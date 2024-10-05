using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Draggable : Widget
{
	public enum Direction
	{
		None,
		Horizontal,
		Vertical
	}

	private Vector3 _offset = Vector3.zero;

	private Vector3 _velocity = Vector3.zero;

	private Vector3 _initialPosition;

	private Vector3 _moveTarget = Vector3.zero;

	private bool _dragging;

	private Camera _hudCamera;

	private List<Vector3> _achors = new List<Vector3>();

	public Direction LimitToAxis;

	public float DampingTime = 1.5f;

	public float MaxVelocity = 2f;

	public Vector3 MinPosition;

	public Vector3 MaxPosition;

	private void Awake()
	{
		_hudCamera = GameObject.Find("HUDCamera").GetComponent<Camera>();
		_initialPosition = base.transform.localPosition;
		MinPosition = _initialPosition;
		MaxPosition = _initialPosition;
		_moveTarget = _initialPosition;
	}

	private Vector3 ScreenToWorldToLocalPosition(Vector3 point)
	{
		Vector4 vector = base.transform.localToWorldMatrix * base.transform.localPosition;
		Vector3 vector2 = _hudCamera.ScreenToWorldPoint(point);
		vector2.z = vector.z;
		return base.transform.localToWorldMatrix * vector2;
	}

	private Vector3 ClampToLimitedAxis(Vector3 point)
	{
		if (LimitToAxis == Direction.Horizontal)
		{
			point.y = base.transform.localPosition.y;
		}
		else if (LimitToAxis == Direction.Vertical)
		{
			point.x = base.transform.localPosition.x;
		}
		return point;
	}

	private Vector3 ClampBetweenLimits(Vector3 point)
	{
		if (LimitToAxis == Direction.Horizontal)
		{
			if (point.x < MinPosition.x)
			{
				point.x = MinPosition.x;
			}
			else if (point.x > MaxPosition.x)
			{
				point.x = MaxPosition.x;
			}
		}
		else if (LimitToAxis == Direction.Vertical)
		{
			if (point.y < MinPosition.y)
			{
				point.y = MinPosition.y;
			}
			else if (point.y > MaxPosition.y)
			{
				point.y = MaxPosition.y;
			}
		}
		return ClampToLimitedAxis(point);
	}

	protected override void OnInput(InputEvent input)
	{
		base.OnInput(input);
		if (input.type == InputEvent.EventType.Press)
		{
			_velocity = Vector3.zero;
			_dragging = true;
			_offset = ClampToLimitedAxis(base.transform.localPosition - ScreenToWorldToLocalPosition(input.position));
		}
		else if (input.type == InputEvent.EventType.Release)
		{
			_dragging = false;
			_offset = ClampBetweenLimits(base.transform.localPosition);
		}
		else if (input.type == InputEvent.EventType.Drag && _dragging)
		{
			Vector3 point = ScreenToWorldToLocalPosition(input.position);
			_moveTarget = ClampToLimitedAxis(point) + _offset;
		}
	}

	private void FixedUpdate()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition = Vector3.SmoothDamp(localPosition, _moveTarget, ref _velocity, Time.smoothDeltaTime * 3.5f);
		localPosition = ClampBetweenLimits(localPosition);
		base.transform.localPosition = localPosition;
	}

	public void Reset()
	{
		base.transform.localPosition = _initialPosition;
		MinPosition = _initialPosition;
		MaxPosition = _initialPosition;
		_moveTarget = Vector3.zero;
	}

	private void OnDisable()
	{
		Reset();
	}

	public void AddAnchor(Vector3 anchor)
	{
		_achors.Add(anchor);
	}
}
