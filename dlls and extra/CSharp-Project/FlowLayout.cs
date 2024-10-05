using System.Collections.Generic;
using UnityEngine;

public class FlowLayout : MonoBehaviour
{
	public enum Direction
	{
		LeftToRight,
		RightToLeft,
		TopToBottom,
		BottomToTop,
		CenterHorizontal,
		CenterVertical
	}

	[SerializeField]
	private Direction m_direction;

	[SerializeField]
	private float m_gap;

	[SerializeField]
	private List<GameObject> m_order;

	private void Start()
	{
		Layout();
	}

	public void removeButton(GameObject button)
	{
		m_order.Remove(button);
		Object.Destroy(button);
	}

	public void Layout()
	{
		Vector3 vector = GetDirectionVector(m_direction) * m_gap;
		Vector3 zero = Vector3.zero;
		if (m_direction == Direction.CenterHorizontal)
		{
			int num = 0;
			foreach (GameObject item in m_order)
			{
				if (item.activeSelf)
				{
					num++;
				}
			}
			zero.x = (0f - (float)(num - 1)) * m_gap / 2f;
		}
		else if (m_direction == Direction.CenterVertical)
		{
			int num2 = 0;
			foreach (GameObject item2 in m_order)
			{
				if (item2.activeSelf)
				{
					num2++;
				}
			}
			zero.y = (float)(num2 - 1) * m_gap / 2f;
		}
		foreach (GameObject item3 in m_order)
		{
			if (item3.activeSelf)
			{
				item3.transform.localPosition = zero;
				zero += vector;
			}
		}
	}

	private static Vector3 GetDirectionVector(Direction direction)
	{
		switch (direction)
		{
		case Direction.LeftToRight:
		case Direction.CenterHorizontal:
			return Vector3.right;
		case Direction.RightToLeft:
			return Vector3.left;
		case Direction.TopToBottom:
		case Direction.CenterVertical:
			return Vector3.down;
		default:
			return Vector3.up;
		}
	}
}
