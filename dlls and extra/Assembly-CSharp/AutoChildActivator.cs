using System.Collections.Generic;
using UnityEngine;

public class AutoChildActivator : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> children;

	[SerializeField]
	private bool persist;

	[SerializeField]
	[HideInInspector]
	private int activatedChild = -1;

	private void Awake()
	{
		ActivateChild((activatedChild >= 0) ? activatedChild : Random.Range(0, children.Count));
	}

	private void ActivateChild(int childIndex)
	{
		if (children == null || children.Count == 0)
		{
			return;
		}
		for (int i = 0; i < children.Count; i++)
		{
			if (i == childIndex)
			{
				children[i].SetActive(value: true);
			}
			else
			{
				children[i].SetActive(value: false);
			}
		}
		activatedChild = childIndex;
	}

	private void LateUpdate()
	{
		if (persist)
		{
			ActivateChild(activatedChild);
		}
	}
}
