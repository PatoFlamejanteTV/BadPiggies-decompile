using System;
using System.Collections;

public class VirtualProductInfo
{
	public enum RewardType
	{
		Magnet,
		Turbo,
		Glue,
		Nightvision,
		Blueprint
	}

	public string productID;

	public string localizationKey;

	public IapManager.CurrencyType currencyType;

	public int price;

	public Hashtable rewards;

	public VirtualProductInfo(string newProductID, string newLocalizationKey, IapManager.CurrencyType newCurrencyType, int newPrice, Hashtable newRewards)
	{
		productID = newProductID;
		localizationKey = newLocalizationKey;
		currencyType = newCurrencyType;
		price = newPrice;
		rewards = newRewards;
	}

	public VirtualProductInfo(Hashtable hash)
	{
		productID = (string)hash["productID"];
		localizationKey = (string)hash["localizationKey"];
		price = int.Parse((string)hash["price"]);
		rewards = (Hashtable)hash["rewards"];
		if (Enum.IsDefined(typeof(IapManager.CurrencyType), (string)hash["currencyType"]))
		{
			currencyType = (IapManager.CurrencyType)Enum.Parse(typeof(IapManager.CurrencyType), (string)hash["currencyType"]);
		}
	}

	public Hashtable ToHashtable()
	{
		return new Hashtable
		{
			{ "productID", productID },
			{ "localizationKey", localizationKey },
			{
				"currencyType",
				currencyType.ToString()
			},
			{
				"price",
				price.ToString()
			},
			{ "rewards", rewards }
		};
	}

	public override string ToString()
	{
		return string.Format("[VirtualProductInfo] " + productID + ", " + localizationKey + ", " + currencyType.ToString() + ", " + price);
	}
}
