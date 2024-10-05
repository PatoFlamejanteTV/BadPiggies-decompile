using System;
using UnityEngine;

public class GameObjectEvents : MonoBehaviour
{
	public Action<bool> OnVisible;

	public Action<bool> OnEnabled;

	private void OnEnable()
	{
		if (OnEnabled != null)
		{
			OnEnabled(obj: true);
		}
	}

	private void OnDisable()
	{
		if (OnEnabled != null)
		{
			OnEnabled(obj: false);
		}
	}

	private void OnBecameVisible()
	{
		if (OnVisible != null)
		{
			OnVisible(obj: true);
		}
	}

	private void OnBecameInvisible()
	{
		if (OnVisible != null)
		{
			OnVisible(obj: false);
		}
	}
}
