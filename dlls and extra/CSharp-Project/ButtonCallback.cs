using System;
using UnityEngine;

public class ButtonCallback : MonoBehaviour
{
	public Action onOkCallback;

	public Action onCancelCallback;

	public void OkButtonPressed()
	{
		if (onOkCallback != null)
		{
			onOkCallback();
		}
	}

	public void CancelButtonPressed()
	{
		if (onCancelCallback != null)
		{
			onCancelCallback();
		}
	}
}
