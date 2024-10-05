using UnityEngine;

public class ShopTabIndicator : MonoBehaviour
{
	public IapManager.InAppPurchaseItemType m_purchaseItem;

	private void OnEnable()
	{
		UpdateInidicator(IapManager.InAppPurchaseItemType.Undefined);
		IapManager.onPurchaseSucceeded += UpdateInidicator;
	}

	private void OnDisable()
	{
		IapManager.onPurchaseSucceeded -= UpdateInidicator;
	}

	private void UpdateInidicator(IapManager.InAppPurchaseItemType type)
	{
		if (Singleton<IapManager>.IsInstantiated())
		{
			GetComponent<Renderer>().enabled = Singleton<IapManager>.Instance.IsItemPurchased(m_purchaseItem);
		}
	}
}
