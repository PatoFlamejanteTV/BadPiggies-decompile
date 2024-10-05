using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UnparentExportAction : ExportAction
{
	private List<Transform> childObjects;

	private void OnEnable()
	{
		base.gameObject.hideFlags = HideFlags.None;
	}

	public override void StartActions()
	{
		if (childObjects == null)
		{
			childObjects = new List<Transform>();
		}
		if (base.transform.parent == null)
		{
			childObjects = new List<Transform>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				childObjects.Add(base.transform.GetChild(i));
			}
			for (int j = 0; j < childObjects.Count; j++)
			{
				childObjects[j].parent = null;
			}
		}
		base.gameObject.hideFlags = HideFlags.HideAndDontSave;
	}

	public override void EndActions()
	{
		base.gameObject.hideFlags = HideFlags.None;
		if (childObjects != null)
		{
			for (int i = 0; i < childObjects.Count; i++)
			{
				childObjects[i].transform.parent = base.transform;
			}
		}
	}
}
