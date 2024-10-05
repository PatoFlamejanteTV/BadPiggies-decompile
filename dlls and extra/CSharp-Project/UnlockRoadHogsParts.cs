using UnityEngine;

public class UnlockRoadHogsParts : TextDialog
{
	[SerializeField]
	private TextMesh enabledOkButtonText;

	[SerializeField]
	private TextMesh disabledOkButtonText;

	public string Cost
	{
		set
		{
			enabledOkButtonText.text = value;
			disabledOkButtonText.text = value;
		}
	}

	protected override void OnEnable()
	{
		Singleton<KeyListener>.Instance.GrabFocus(this);
		KeyListener.keyReleased += base.HandleKeyReleased;
		EventManager.Send(new UIEvent(UIEvent.Type.OpenedPurchaseRoadHogsParts));
		EnableConfirmButton();
		TextDialog.s_dialogOpen = true;
	}

	protected override void OnDisable()
	{
		isOpened = false;
		if ((bool)Singleton<KeyListener>.Instance)
		{
			Singleton<KeyListener>.Instance.ReleaseFocus(this);
		}
		KeyListener.keyReleased -= base.HandleKeyReleased;
		EventManager.Send(new UIEvent(UIEvent.Type.ClosedPurchaseRoadHogsParts));
		TextDialog.s_dialogOpen = false;
	}

	public new void Open()
	{
		base.Open();
	}

	public new void Close()
	{
		base.Close();
	}

	public new void Confirm()
	{
		base.Confirm();
	}

	public new void OpenShop()
	{
		base.OpenShop();
	}
}
