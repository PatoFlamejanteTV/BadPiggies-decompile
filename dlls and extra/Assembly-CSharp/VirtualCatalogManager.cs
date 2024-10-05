using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class VirtualCatalogManager : Singleton<VirtualCatalogManager>
{
	private Dictionary<string, VirtualProductInfo> virtualCatalogDictionary;

	private bool hasCatalog;

	private SecureJsonManager secureJson;

	public bool HasCatalog => hasCatalog;

	public static event Action onVirtualProductListParsed;

	private void Awake()
	{
		SetAsPersistant();
		hasCatalog = false;
		secureJson = new SecureJsonManager("virtualcatalog");
		secureJson.Initialize(OnDataLoaded);
	}

	private void OnDataLoaded(string rawData)
	{
		virtualCatalogDictionary = new Dictionary<string, VirtualProductInfo>();
		if (!(MiniJSON.jsonDecode(rawData) is Hashtable hashtable) || !hashtable.ContainsKey("catalog"))
		{
			return;
		}
		foreach (Hashtable item in (ArrayList)hashtable["catalog"])
		{
			if (item.ContainsKey("productID"))
			{
				string key = (string)item["productID"];
				if (!virtualCatalogDictionary.ContainsKey(key))
				{
					VirtualProductInfo value = new VirtualProductInfo(item);
					virtualCatalogDictionary.Add(key, value);
				}
			}
		}
		hasCatalog = true;
		if (VirtualCatalogManager.onVirtualProductListParsed != null)
		{
			VirtualCatalogManager.onVirtualProductListParsed();
		}
	}

	public int GetProductPrice(IapManager.InAppPurchaseItemType itemType)
	{
		string productIdByItem = Singleton<IapManager>.Instance.GetProductIdByItem(itemType);
		if (string.IsNullOrEmpty(productIdByItem))
		{
			return -1;
		}
		string key = productIdByItem.Substring(productIdByItem.LastIndexOf(".") + 1);
		if (virtualCatalogDictionary != null && virtualCatalogDictionary.ContainsKey(key))
		{
			return virtualCatalogDictionary[key].price;
		}
		return -1;
	}

	public string GetProductLocalizationKey(IapManager.InAppPurchaseItemType itemType)
	{
		string productIdByItem = Singleton<IapManager>.Instance.GetProductIdByItem(itemType);
		if (string.IsNullOrEmpty(productIdByItem))
		{
			return string.Empty;
		}
		productIdByItem = productIdByItem.Substring(productIdByItem.LastIndexOf(".") + 1);
		if (virtualCatalogDictionary != null && virtualCatalogDictionary.ContainsKey(productIdByItem))
		{
			return virtualCatalogDictionary[productIdByItem].localizationKey;
		}
		return string.Empty;
	}

	public int GetProductPrice(string id)
	{
		if (virtualCatalogDictionary != null && virtualCatalogDictionary.ContainsKey(id))
		{
			return virtualCatalogDictionary[id].price;
		}
		return -1;
	}

	public Hashtable GetProductRewards(IapManager.InAppPurchaseItemType itemType)
	{
		string productIdByItem = Singleton<IapManager>.Instance.GetProductIdByItem(itemType);
		if (string.IsNullOrEmpty(productIdByItem))
		{
			return null;
		}
		string key = productIdByItem.Substring(productIdByItem.LastIndexOf(".") + 1);
		if (virtualCatalogDictionary != null && virtualCatalogDictionary.ContainsKey(key))
		{
			return virtualCatalogDictionary[key].rewards;
		}
		return null;
	}

	public int GetCustomRewardValue(string itemName, string customKey)
	{
		Hashtable productRewards = GetProductRewards(itemName);
		if (productRewards != null && productRewards.ContainsKey(customKey) && int.TryParse((string)productRewards[customKey], out var result))
		{
			return result;
		}
		return -1;
	}

	public int GetProductRewardCount(IapManager.InAppPurchaseItemType itemType)
	{
		return GetProductRewards(itemType)?.Count ?? 0;
	}

	public IapManager.BundleItem[] GetProductRewardsAsBundleItems(IapManager.InAppPurchaseItemType itemType)
	{
		Hashtable productRewards = GetProductRewards(itemType);
		List<IapManager.BundleItem> list = new List<IapManager.BundleItem>();
		foreach (DictionaryEntry item in productRewards)
		{
			if (Enum.IsDefined(typeof(IapManager.BundleItem.BundleItemType), (string)item.Key))
			{
				IapManager.BundleItem.BundleItemType type = (IapManager.BundleItem.BundleItemType)Enum.Parse(typeof(IapManager.BundleItem.BundleItemType), (string)item.Key);
				int count = int.Parse((string)item.Value);
				list.Add(new IapManager.BundleItem(type, count));
			}
		}
		return list.ToArray();
	}

	public Hashtable GetProductRewards(string id)
	{
		if (virtualCatalogDictionary != null && virtualCatalogDictionary.ContainsKey(id))
		{
			return virtualCatalogDictionary[id].rewards;
		}
		return null;
	}

	private void PrintHashtable(Hashtable hash, int indentation, ref StringBuilder contents)
	{
		foreach (DictionaryEntry item in hash)
		{
			if (indentation > 0)
			{
				contents.Append(' ', indentation * 3);
			}
			contents.Append(item.Key.ToString() + " : ");
			if (item.Value is Hashtable)
			{
				contents.Append('\n');
				PrintHashtable((Hashtable)item.Value, indentation + 1, ref contents);
			}
			else if (item.Value is ArrayList)
			{
				contents.Append('\n');
				foreach (object item2 in (ArrayList)item.Value)
				{
					if (item2 is Hashtable)
					{
						PrintHashtable((Hashtable)item2, indentation + 1, ref contents);
					}
					contents.Append('\n');
				}
			}
			else
			{
				contents.Append(item.Value.ToString());
				contents.Append('\n');
			}
		}
	}
}
