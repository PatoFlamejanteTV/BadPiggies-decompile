using System;
using UnityEngine;

public class ConfirmationPopup : MonoBehaviour
{
	public event Action PopupClosed;

	public void DismissDialog()
	{
		base.gameObject.SetActive(value: false);
		if (this.PopupClosed != null)
		{
			this.PopupClosed();
		}
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
