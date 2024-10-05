using System.Collections.Generic;
using UnityEngine;

public class IapPriceButton : MonoBehaviour
{
	public GameObject m_loadingIndicator;

	public GameObject m_notConnectedIndicator;

	public GameObject m_purchasedIndicator;

	public GameObject m_priceText;

	public GameObject m_LockOverlay;

	public IapManager.InAppPurchaseItemType m_purchaseType;

	private bool isProductListHandlerAttached;

	private void FetchProductInfo()
	{
		if (Singleton<IapManager>.IsInstantiated())
		{
			if (!isProductListHandlerAttached)
			{
				IapManager.onProductListReceived += OnProductListReceived;
				isProductListHandlerAttached = true;
			}
			SetPriceIndicator(Shop.PriceIndicator.Loading);
			if (Singleton<IapManager>.Instance.Status != IapManager.InAppPurchaseStatus.FetchingItems)
			{
				Singleton<IapManager>.Instance.FetchPurchasableItemList();
			}
		}
	}

	private void OnEnable()
	{
		if (!Singleton<IapManager>.IsInstantiated())
		{
			SetPriceIndicator(Shop.PriceIndicator.NotConnected);
		}
		else if (Singleton<IapManager>.Instance.IsItemPurchased(m_purchaseType))
		{
			SetPriceIndicator(Shop.PriceIndicator.Purchased);
		}
		else if (Singleton<IapManager>.Instance.ProductList == null)
		{
			FetchProductInfo();
		}
		else
		{
			string productIdByItem = Singleton<IapManager>.Instance.GetProductIdByItem(m_purchaseType);
			IAPProductInfo iAPProductInfo = null;
			if (!string.IsNullOrEmpty(productIdByItem))
			{
				foreach (IAPProductInfo product in Singleton<IapManager>.Instance.ProductList)
				{
					if (product.productId == productIdByItem)
					{
						iAPProductInfo = product;
						break;
					}
				}
			}
			if (Singleton<IapManager>.Instance.IsItemPurchased(m_purchaseType))
			{
				SetPriceIndicator(Shop.PriceIndicator.Purchased);
			}
			else if (iAPProductInfo != null && !string.IsNullOrEmpty(iAPProductInfo.formattedPrice))
			{
				SetPriceIndicator(Shop.PriceIndicator.Price, iAPProductInfo.formattedPrice);
			}
			else
			{
				SetPriceIndicator(Shop.PriceIndicator.NotConnected);
			}
		}
		IapManager.onPurchaseSucceeded += UnlockScreen;
		IapManager.onPurchaseFailed += UnlockScreen;
	}

	private void OnDisable()
	{
		if (isProductListHandlerAttached)
		{
			IapManager.onProductListReceived -= OnProductListReceived;
			isProductListHandlerAttached = false;
		}
		IapManager.onPurchaseSucceeded -= UnlockScreen;
		IapManager.onPurchaseFailed -= UnlockScreen;
	}

	private void LockScreen()
	{
		m_LockOverlay.SetActive(value: true);
		Singleton<GuiManager>.Instance.IsEnabled = false;
	}

	private void UnlockScreen(IapManager.InAppPurchaseItemType type = IapManager.InAppPurchaseItemType.Undefined)
	{
		m_LockOverlay.SetActive(value: false);
		Singleton<GuiManager>.Instance.IsEnabled = true;
	}

	private void SetPriceIndicator(Shop.PriceIndicator indicatorType, string formattedPrice = null)
	{
		if (indicatorType == Shop.PriceIndicator.Purchased)
		{
			m_loadingIndicator.gameObject.SetActive(value: false);
			m_notConnectedIndicator.gameObject.SetActive(value: false);
			m_priceText.GetComponent<Renderer>().enabled = false;
			m_purchasedIndicator.gameObject.SetActive(value: true);
			Button component = GetComponent<Button>();
			if ((bool)component)
			{
				Object.Destroy(component);
			}
		}
		else
		{
			m_loadingIndicator.gameObject.SetActive(indicatorType == Shop.PriceIndicator.Loading);
			m_notConnectedIndicator.gameObject.SetActive(indicatorType == Shop.PriceIndicator.NotConnected);
			m_priceText.GetComponent<Renderer>().enabled = indicatorType == Shop.PriceIndicator.Price;
		}
		if (formattedPrice != null)
		{
			m_priceText.GetComponent<TextMesh>().text = formattedPrice;
		}
	}

	private void OnProductListReceived(List<IAPProductInfo> products, string error)
	{
		if (products != null && products.Count > 0)
		{
			string productIdByItem = Singleton<IapManager>.Instance.GetProductIdByItem(m_purchaseType);
			IAPProductInfo iAPProductInfo = null;
			foreach (IAPProductInfo product in Singleton<IapManager>.Instance.ProductList)
			{
				if (product.productId == productIdByItem)
				{
					iAPProductInfo = product;
					break;
				}
			}
			if (iAPProductInfo != null && string.IsNullOrEmpty(iAPProductInfo.formattedPrice))
			{
				SetPriceIndicator(Shop.PriceIndicator.Price, iAPProductInfo.formattedPrice);
			}
			else
			{
				SetPriceIndicator(Shop.PriceIndicator.NotConnected);
			}
		}
		else
		{
			SetPriceIndicator(Shop.PriceIndicator.NotConnected);
		}
	}

	private string GetFormattedPrice()
	{
		if (Singleton<IapManager>.Instance.ProductList != null)
		{
			string productIdByItem = Singleton<IapManager>.Instance.GetProductIdByItem(m_purchaseType);
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
				if (iAPProductInfo != null && iAPProductInfo.formattedPrice != null)
				{
					return Singleton<IapManager>.Instance.PreparePrice(iAPProductInfo.formattedPrice);
				}
				return null;
			}
		}
		return null;
	}

	public void Purchase()
	{
		if (string.IsNullOrEmpty(GetFormattedPrice()))
		{
			FetchProductInfo();
		}
		else if (Singleton<IapManager>.IsInstantiated() && !Singleton<IapManager>.Instance.IsItemPurchased(m_purchaseType))
		{
			LockScreen();
			Singleton<IapManager>.Instance.PurchaseItem(m_purchaseType);
		}
	}
}
