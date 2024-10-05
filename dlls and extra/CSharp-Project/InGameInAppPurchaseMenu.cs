using UnityEngine;

public class InGameInAppPurchaseMenu : MonoBehaviour
{
	public delegate void OnClose();

	private GameObject m_loader;

	private Dialog m_dialog;

	public event OnClose onClose;

	private void Awake()
	{
		m_dialog = base.transform.Find("Dialog").GetComponent<Dialog>();
		m_loader = base.transform.Find("Dialog").Find("PurchaseLoader").gameObject;
		m_loader.SetActive(value: false);
		if ((bool)m_dialog)
		{
			m_dialog.Close();
		}
	}

	private void OnCloseFbLikeDialog()
	{
		if (this.onClose != null)
		{
			this.onClose();
			this.onClose = null;
		}
	}

	private void OnCloseDialog()
	{
		if (this.onClose != null)
		{
			this.onClose();
			this.onClose = null;
		}
	}

	private void OnEnable()
	{
		IapManager.onPurchaseSucceeded += HandleIapManageronPurchaseSucceeded;
		IapManager.onPurchaseFailed += HandleIapManageronPurchaseFailed;
		if ((bool)m_dialog)
		{
			m_dialog.onClose += OnCloseDialog;
		}
	}

	private void OnDisable()
	{
		IapManager.onPurchaseSucceeded -= HandleIapManageronPurchaseSucceeded;
		IapManager.onPurchaseFailed -= HandleIapManageronPurchaseFailed;
		if ((bool)m_dialog)
		{
			m_dialog.onClose -= OnCloseDialog;
		}
	}

	public void PurchaseBlueprintPackSmall()
	{
		Purchase(IapManager.InAppPurchaseItemType.BlueprintSmall);
	}

	public void PurchaseBlueprintPackMedium()
	{
		Purchase(IapManager.InAppPurchaseItemType.BlueprintMedium);
	}

	public void PurchaseBlueprintPackLarge()
	{
		Purchase(IapManager.InAppPurchaseItemType.BlueprintLarge);
	}

	public void PurchaseUnlockFullVersion()
	{
		Purchase(IapManager.InAppPurchaseItemType.UnlockFullVersion);
	}

	public void PurchaseSpecialSandbox()
	{
		Purchase(IapManager.InAppPurchaseItemType.UnlockSpecialSandbox);
	}

	private void Purchase(IapManager.InAppPurchaseItemType type)
	{
		m_loader.SetActive(value: true);
		Singleton<IapManager>.Instance.PurchaseItem(type);
	}

	public void OpenDialog()
	{
		SetVisible(visible: true);
	}

	private void HandleIapManageronPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		SetVisible(visible: false);
	}

	private void HandleIapManageronPurchaseFailed(IapManager.InAppPurchaseItemType type)
	{
		m_loader.SetActive(value: false);
	}

	public void SetVisible(bool visible)
	{
		if (visible)
		{
			if ((bool)m_dialog)
			{
				m_dialog.Open();
			}
			m_loader.SetActive(!visible);
		}
		else if ((bool)m_dialog)
		{
			m_dialog.Close();
		}
		if (!visible && this.onClose != null)
		{
			this.onClose();
			this.onClose = null;
		}
	}
}
