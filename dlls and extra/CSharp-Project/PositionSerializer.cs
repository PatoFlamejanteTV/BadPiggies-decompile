using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PositionSerializer : ExportAction
{
	public GameObject prefab;

	[HideInInspector]
	[SerializeField]
	private GameObject prefabCache;

	[SerializeField]
	private List<Vector3> childLocalPositions;

	private void OnDataLoaded()
	{
		if (base.transform.childCount == prefab.transform.childCount)
		{
			return;
		}
		for (int i = 0; i < prefab.transform.childCount; i++)
		{
			GameObject gameObject = Object.Instantiate(prefab.transform.GetChild(i).gameObject);
			gameObject.transform.parent = base.transform;
			if (childLocalPositions.Count >= prefab.transform.childCount)
			{
				gameObject.transform.localPosition = childLocalPositions[i];
			}
		}
	}

	public void DestroyChildren()
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Object.DestroyImmediate(base.transform.GetChild(num).gameObject);
		}
	}

	public override void StartActions()
	{
		if (base.transform.childCount != 0)
		{
			DestroyChildren();
		}
	}

	public override void EndActions()
	{
		OnDataLoaded();
	}

	public void SavePositions()
	{
		childLocalPositions = new List<Vector3>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Vector3 localPosition = base.transform.GetChild(i).localPosition;
			childLocalPositions.Add(localPosition);
		}
	}
}
