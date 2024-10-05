using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Draggable))]
public class ScrollableList : MonoBehaviour
{
	private List<GameObject> _items = new List<GameObject>();

	private Vector3 _lastPosition = Vector3.zero;

	private Draggable _draggable;

	private BoxCollider _collider;

	public float itemHeight;

	public float itemWidth;

	public float listHeight;

	private void Awake()
	{
		_draggable = GetComponent<Draggable>();
		_collider = GetComponent<BoxCollider>();
	}

	public void Add(GameObject item)
	{
		item.transform.localPosition = _lastPosition;
		_items.Add(item);
		_draggable.AddAnchor(-_lastPosition - new Vector3(0f, itemHeight / 2f));
		Vector3 size = _collider.size;
		Vector3 center = _collider.center;
		if (_draggable.LimitToAxis == Draggable.Direction.Horizontal)
		{
			_lastPosition.x += itemWidth;
			size.x = _lastPosition.x;
			center.x = _lastPosition.x / 2f + itemWidth / 2f;
		}
		if (_draggable.LimitToAxis == Draggable.Direction.Vertical)
		{
			_lastPosition.y -= itemHeight;
			size.y = _lastPosition.y;
			center.y = _lastPosition.y / 2f + itemHeight / 2f;
		}
		else
		{
			_lastPosition += new Vector3(0f - itemHeight, itemWidth, 0f);
		}
		if ((float)_items.Count * itemHeight > listHeight)
		{
			_draggable.MaxPosition.y = (float)_items.Count * itemHeight - listHeight;
		}
		if (_draggable.MaxPosition.y < _draggable.MinPosition.y)
		{
			_draggable.MaxPosition.y = _draggable.MinPosition.y;
		}
		_collider.size = size;
		_collider.center = center;
	}

	public void Clear()
	{
		foreach (GameObject item in _items)
		{
			Object.Destroy(item);
		}
		_items.Clear();
		_lastPosition = Vector3.zero;
		_draggable.Reset();
	}
}
