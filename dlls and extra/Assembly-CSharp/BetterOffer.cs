using UnityEngine;

public class BetterOffer : MonoBehaviour
{
	[SerializeField]
	private PurchaseInfo purchaseInfo;

	[SerializeField]
	private IapManager.InAppPurchaseItemType compareType = IapManager.InAppPurchaseItemType.SnoutCoinPackSmall;

	private TextMesh[] textMeshes;

	private void Awake()
	{
		textMeshes = GetComponentsInChildren<TextMesh>();
	}

	private void Start()
	{
		string arg = $"{CalculatePercentage():0%}";
		for (int i = 0; i < textMeshes.Length; i++)
		{
			TextMeshLocale component = textMeshes[i].gameObject.GetComponent<TextMeshLocale>();
			component.RefreshTranslation();
			component.enabled = false;
			textMeshes[i].text = string.Format(textMeshes[i].text, arg);
		}
	}

	private float CalculatePercentage()
	{
		purchaseInfo.CheckOnSale();
		IapManager.InAppPurchaseItemType inAppPurchaseItemType = ((!purchaseInfo.isSaleItem) ? purchaseInfo.purchaseItem : purchaseInfo.saleItem);
		float num = Singleton<IapManager>.Instance.GetPurchaseItemTypeCount(inAppPurchaseItemType);
		float num2 = Singleton<IapManager>.Instance.GetPurchaseItemTypeCount(compareType);
		float price = GetPrice(inAppPurchaseItemType);
		float price2 = GetPrice(compareType);
		if (num == 0f || num2 == 0f || price == 0f || price2 == 0f)
		{
			return 0f;
		}
		return num / price / (num2 / price2) - 1f;
	}

	private float GetPrice(IapManager.InAppPurchaseItemType item)
	{
		if (!Singleton<IapManager>.IsInstantiated() || !Singleton<IapManager>.Instance.ReadyForTransaction)
		{
			return 0f;
		}
		string productIdByItem = Singleton<IapManager>.Instance.GetProductIdByItem(item);
		if (!string.IsNullOrEmpty(productIdByItem))
		{
			IAPProductInfo iAPProductInfo = null;
			foreach (IAPProductInfo product in Singleton<IapManager>.Instance.ProductList)
			{
				if (product.productId == productIdByItem)
				{
					iAPProductInfo = product;
					break;
				}
			}
			if (iAPProductInfo == null)
			{
				return 0f;
			}
			string s = ((!string.IsNullOrEmpty(iAPProductInfo.unformattedPrice)) ? iAPProductInfo.unformattedPrice : iAPProductInfo.formattedPrice);
			try
			{
				return float.Parse(s);
			}
			catch
			{
				return 0f;
			}
		}
		return 0f;
	}
}
