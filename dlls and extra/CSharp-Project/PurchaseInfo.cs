using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseInfo : MonoBehaviour
{
	public IapManager.InAppPurchaseItemType purchaseItem;

	public IapManager.InAppPurchaseItemType saleItem;

	public IapManager.CurrencyType currencyType = IapManager.CurrencyType.SnoutCoin;

	public bool doUpdatePrice = true;

	public bool isSaleItem;

	public float xOffsetOnDisable;

	private int currentCount;

	private string currentSpriteID = string.Empty;

	private string currentEffectID = string.Empty;

	private Transform countTf;

	private bool colliderEnabled = true;

	private RefreshLocalizer offerTimeLocalizer;

	private string currentSaleKey = string.Empty;

	private void Start()
	{
		Sprite sprite = null;
		Sprite sprite2 = null;
		Transform transform = base.transform.Find("Icon");
		Transform transform2 = base.transform.Find("Deco");
		if (transform != null)
		{
			sprite = transform.GetComponent<Sprite>();
		}
		if (transform2 != null)
		{
			sprite2 = transform2.GetComponent<Sprite>();
		}
		if (sprite != null)
		{
			currentSpriteID = sprite.Id;
		}
		if (sprite2 != null)
		{
			currentEffectID = sprite2.Id;
		}
		countTf = base.transform.Find("Count");
		GetComponent<Collider>().enabled = false;
		SetCount(0, countTf);
		UpdateParameters();
		if (currencyType == IapManager.CurrencyType.RealMoney)
		{
			if (Singleton<IapManager>.IsInstantiated() && Singleton<IapManager>.Instance.PurchaseListInited)
			{
				UpdatePurchaseCount();
			}
			else
			{
				IapManager.onProductListParsed += UpdatePurchaseCount;
			}
		}
		else if (Singleton<VirtualCatalogManager>.IsInstantiated() && Singleton<VirtualCatalogManager>.Instance.HasCatalog)
		{
			UpdatePurchaseCount();
		}
		else
		{
			VirtualCatalogManager.onVirtualProductListParsed += UpdatePurchaseCount;
		}
		IapManager.onPurchaseSucceeded += OnPurchaseSucceeded;
	}

	private void OnDestroy()
	{
		IapManager.onProductListParsed -= UpdatePurchaseCount;
		VirtualCatalogManager.onVirtualProductListParsed -= UpdatePurchaseCount;
		if (offerTimeLocalizer != null)
		{
			offerTimeLocalizer.Dispose();
		}
		IapManager.onPurchaseSucceeded -= OnPurchaseSucceeded;
	}

	private void UpdateParameters()
	{
		Button component = GetComponent<Button>();
		if ((bool)component)
		{
			string text = ((!isSaleItem) ? purchaseItem.ToString() : saleItem.ToString());
			List<MethodCaller.Parameter> parametersForInspector = component.MethodToCall.GetParametersForInspector();
			int count = parametersForInspector.Count;
			if (count == 4 && parametersForInspector[3].type != null)
			{
				component.MethodToCall.SetParameters(new object[4] { text, currentSpriteID, currentEffectID, currentCount });
			}
			else if (count >= 3 && parametersForInspector[2].type != null)
			{
				component.MethodToCall.SetParameters(new object[3] { text, currentSpriteID, currentCount });
			}
			else if (count >= 2 && parametersForInspector[1].type != null)
			{
				MethodCaller methodToCall = component.MethodToCall;
				object[] parameters = new string[2] { text, currentSpriteID };
				methodToCall.SetParameters(parameters);
			}
			else if (count >= 1 && parametersForInspector[0].type != null)
			{
				component.MethodToCall.SetParameter(text);
			}
		}
	}

	private void UpdatePurchaseCount()
	{
		if (currencyType == IapManager.CurrencyType.RealMoney)
		{
			if (Singleton<IapManager>.Instance.IsBundleProduct(purchaseItem))
			{
				if (Singleton<IapManager>.Instance.HasBundleInfo(purchaseItem))
				{
					using IEnumerator<IapManager.BundleItem> enumerator = Singleton<IapManager>.Instance.GetBundleInfo(purchaseItem).GetEnumerator();
					if (enumerator.MoveNext())
					{
						SetCount(enumerator.Current.count, countTf);
					}
				}
				else
				{
					int num = 0;
					string[] names = Enum.GetNames(typeof(IapManager.BundleItem.BundleItemType));
					for (int i = 0; i < names.Length; i++)
					{
						num++;
					}
					if (num > 0)
					{
						SetCount(0, countTf);
					}
				}
			}
			else
			{
				currentCount = Singleton<IapManager>.Instance.GetPurchaseItemTypeCount(purchaseItem);
				SetCount(currentCount, countTf);
			}
		}
		else
		{
			Hashtable productRewards = Singleton<VirtualCatalogManager>.Instance.GetProductRewards(purchaseItem);
			if (productRewards != null)
			{
				foreach (DictionaryEntry item in productRewards)
				{
					if (Enum.IsDefined(typeof(IapManager.BundleItem.BundleItemType), (string)item.Key))
					{
						currentCount = int.Parse((string)item.Value);
						SetCount(currentCount, countTf);
						break;
					}
					if (item.Key.Equals("WoodenCrate"))
					{
						SetCount(1, countTf);
						break;
					}
				}
			}
			else
			{
				SetCount(0, countTf);
			}
		}
		UpdateParameters();
	}

	private void SetCount(int count, Transform countTrans)
	{
		if (count > 0 || currencyType != IapManager.CurrencyType.SnoutCoin)
		{
			GetComponent<Collider>().enabled = colliderEnabled;
			if ((bool)countTrans)
			{
				countTrans.GetComponent<SpriteText>().Text = ((count != 0) ? ("x" + count) : string.Empty);
			}
		}
		else
		{
			GetComponent<Collider>().enabled = false;
			if ((bool)countTrans)
			{
				countTrans.GetComponent<SpriteText>().Text = string.Empty;
			}
		}
	}

	public void EnableCollider(bool enable = true)
	{
		colliderEnabled = enable;
	}

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
		if (base.transform.parent != null)
		{
			base.transform.parent.position += Vector3.right * ((!show) ? xOffsetOnDisable : (0f - xOffsetOnDisable));
		}
	}

	public void Purchase()
	{
		EventManager.Send(new IapManager.PurchaseEvent((!isSaleItem) ? purchaseItem : saleItem));
	}

	private void OnPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		if (!string.IsNullOrEmpty(currentSaleKey) && type == saleItem)
		{
			GameProgress.SetBool(currentSaleKey + "_used", value: true);
		}
	}

	public void CheckOnSale()
	{
		isSaleItem = IsSaleOn(saleItem.ToString(), out var saleLeft);
		if (!isSaleItem || !(base.transform.parent != null) || !(base.transform.parent.parent != null))
		{
			return;
		}
		Transform transform = base.transform.parent.parent.Find("OfferBanner/OfferText");
		TextMesh textMesh = null;
		if (transform != null)
		{
			textMesh = transform.GetComponent<TextMesh>();
		}
		if (textMesh != null)
		{
			if (offerTimeLocalizer == null)
			{
				offerTimeLocalizer = new RefreshLocalizer(textMesh);
			}
			int days = saleLeft / 86400;
			int hours = saleLeft % 86400 / 3600;
			offerTimeLocalizer.Update = () => (days > 0) ? $"{days}d {hours}h" : $"{hours}h";
			offerTimeLocalizer.Refresh();
		}
	}

	private bool IsSaleOn(string saleItemKey, out int saleLeft)
	{
		ConfigData config = Singleton<GameConfigurationManager>.Instance.GetConfig("iap_sale_items");
		if (!Singleton<IapManager>.Instance.HasProduct(saleItem))
		{
			saleLeft = -1;
			return false;
		}
		if (config != null)
		{
			for (int i = 0; i < config.Keys.Length; i++)
			{
				if (config.Keys[i].StartsWith(saleItemKey))
				{
					string[] array = config[config.Keys[i]].Split('-');
					int saleTimeLeft = Shop.GetSaleTimeLeft(array[0], (array.Length <= 1) ? string.Empty : array[1]);
					if (saleTimeLeft > 0 && !GameProgress.GetBool(config.Keys[i] + "_used"))
					{
						saleLeft = saleTimeLeft;
						currentSaleKey = config.Keys[i];
						return true;
					}
				}
			}
		}
		saleLeft = -1;
		return false;
	}
}
