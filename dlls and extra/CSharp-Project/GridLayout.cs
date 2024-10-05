using System;
using UnityEngine;

[ExecuteInEditMode]
public class GridLayout : MonoBehaviour
{
	private enum GridType
	{
		Horizontal,
		Vertical
	}

	private enum GridAlign
	{
		Left,
		Right,
		Center
	}

	[SerializeField]
	private GridType gridType;

	[SerializeField]
	private GridAlign gridAlign;

	[SerializeField]
	private int items = 5;

	[SerializeField]
	private float horizontalGap = 2f;

	[SerializeField]
	private float verticalGap = 2f;

	public Action onUpdateLayout;

	public float HorizontalGap => horizontalGap;

	public float VerticalGap => verticalGap;

	private void Awake()
	{
		UpdateLayout();
	}

	public void UpdateLayout()
	{
		if (items <= 0)
		{
			items = 1;
		}
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeInHierarchy)
			{
				num++;
			}
		}
		float num2 = 0f;
		switch (gridAlign)
		{
		case GridAlign.Right:
			num2 = (float)Mathf.Clamp(num - 1, 0, items - 1) * horizontalGap;
			break;
		case GridAlign.Center:
			num2 = (float)Mathf.Clamp(num - 1, 0, items - 1) * horizontalGap * 0.5f;
			break;
		}
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		for (int j = 0; j < base.transform.childCount; j++)
		{
			Transform child = base.transform.GetChild(j);
			if (child.gameObject.activeInHierarchy)
			{
				if (num5 > 0 && num5 % items == 0)
				{
					num3++;
					num4 = 0;
				}
				switch (gridType)
				{
				case GridType.Vertical:
					child.localPosition = -Vector3.up * (verticalGap * (float)num4) + Vector3.right * (horizontalGap * (float)num3);
					break;
				case GridType.Horizontal:
					child.localPosition = Vector3.right * (horizontalGap * (float)num4 - num2) - Vector3.up * (verticalGap * (float)num3);
					break;
				}
				num4++;
				num5++;
			}
		}
		if (onUpdateLayout != null)
		{
			onUpdateLayout();
		}
	}
}
