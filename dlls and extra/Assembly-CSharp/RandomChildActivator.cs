using System.Collections.Generic;
using UnityEngine;

public class RandomChildActivator : MonoBehaviour
{
	public List<GameObject> children;

	public static int indexToActivate;

	public GameObject ActivateChild()
	{
		if (children == null || children.Count == 0 || indexToActivate > children.Count - 1)
		{
			return null;
		}
		GameObject result = null;
		for (int i = 0; i < children.Count; i++)
		{
			if (i == indexToActivate)
			{
				children[i].SetActive(value: true);
				result = children[i];
			}
			else
			{
				children[i].SetActive(value: false);
			}
		}
		return result;
	}

	[ContextMenu("Random")]
	private void Rand()
	{
		indexToActivate = Random.Range(0, children.Count);
	}
}
