using System;
using UnityEngine;

public class GridAnchor : MonoBehaviour
{
	public enum SnapSide
	{
		Top,
		Bottom,
		Left,
		Right
	}

	[SerializeField]
	private GridLayout grid;

	[SerializeField]
	private SnapSide snapToSide = SnapSide.Bottom;

	private void Awake()
	{
		OnUpdateLayout();
		if (grid != null)
		{
			GridLayout gridLayout = grid;
			gridLayout.onUpdateLayout = (Action)Delegate.Combine(gridLayout.onUpdateLayout, new Action(OnUpdateLayout));
		}
	}

	private void OnDestroy()
	{
		if (grid != null)
		{
			GridLayout gridLayout = grid;
			gridLayout.onUpdateLayout = (Action)Delegate.Remove(gridLayout.onUpdateLayout, new Action(OnUpdateLayout));
		}
	}

	private void OnUpdateLayout()
	{
		if (snapToSide == SnapSide.Bottom)
		{
			base.transform.position = grid.transform.position + Vector3.down * (grid.VerticalGap * (float)grid.transform.childCount);
		}
	}
}
