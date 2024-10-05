using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using UnityEngine;

public class IapManager : Singleton<IapManager>
{
	public delegate void PurchaseSucceeded(InAppPurchaseItemType type);

	public delegate void PurchaseFailed(InAppPurchaseItemType type);

	public delegate void RestorePurchaseComplete(bool isSucceeded);

	public delegate void ProductListReceived(List<IAPProductInfo> products, string error);

	public struct PurchaseEvent : EventManager.Event
	{
		public InAppPurchaseItemType itemType;

		public PurchaseEvent(InAppPurchaseItemType itemType)
		{
			this.itemType = itemType;
		}
	}

	public struct BundleItem
	{
		public enum BundleItemType
		{
			Undefined,
			Blueprint,
			SuperGlue,
			SuperMagnet,
			TurboCharge,
			NightVision
		}

		public readonly BundleItemType type;

		public readonly int count;

		public BundleItem(BundleItemType type, int count)
		{
			this.type = type;
			this.count = count;
		}
	}

	public enum InAppPurchaseStatus
	{
		Init,
		Idle,
		FetchingItems,
		PurchasingItem
	}

	public enum CurrencyType
	{
		RealMoney,
		SnoutCoin,
		Scrap
	}

	public enum InAppPurchaseItemType
	{
		Undefined,
		UnlockFullVersion,
		UnlockSpecialSandbox,
		BlueprintSmall,
		BlueprintMedium,
		BlueprintLarge,
		BlueprintHuge,
		BlueprintUltimate,
		SuperGlueSmall,
		SuperGlueMedium,
		SuperGlueLarge,
		SuperGlueHuge,
		SuperGlueUltimate,
		SuperMagnetSmall,
		SuperMagnetMedium,
		SuperMagnetLarge,
		SuperMagnetHuge,
		SuperMagnetUltimate,
		TurboChargeSmall,
		TurboChargeMedium,
		TurboChargeLarge,
		TurboChargeHuge,
		TurboChargeUltimate,
		PermanentBlueprint,
		BundleStarterPack,
		BundleMediumPack,
		BundleBigPack,
		UnlockTenLevels,
		UnlockEpisode,
		AddTenDesserts,
		UnlockNineLevels,
		NightVisionSmall,
		NightVisionMedium,
		NightVisionLarge,
		NightVisionHuge,
		NightVisionUltimate,
		SnoutCoinPackSmall,
		SnoutCoinPackMedium,
		SnoutCoinPackLarge,
		SnoutCoinPackHuge,
		SnoutCoinPackUltimate,
		BundleHugePack,
		StarterPack,
		BlueprintSingle,
		SuperGlueSingle,
		SuperMagnetSingle,
		TurboChargeSingle,
		NightVisionSingle,
		WoodenLootCrate,
		MetalLootCrate,
		GoldenLootCrate,
		WoodenLootCrateSale,
		MetalLootCrateSale,
		GoldenLootCrateSale,
		GoldenLootCratePack,
		SnoutCoinPackSmallSale,
		SnoutCoinPackMediumSale,
		SnoutCoinPackLargeSale,
		SnoutCoinPackHugeSale,
		SnoutCoinPackUltimateSale
	}

	public enum CodeRedeemError
	{
		AlreadyUsed,
		AlreadyOwned,
		Invalid
	}

	[SerializeField]
	private GameObject m_iapUnlockFullVersionPage;

	private GameObject m_iapUnlockFullVersionPageInstance;

	[SerializeField]
	private GameObject m_errorPopup;

	private GameObject m_errorPopupInstance;

	[SerializeField]
	private GameObject m_ShopPage;

	private GameObject m_ShopPageInstance;

	private Shop m_Shop;

	public string m_priceAllowedChars;

	public GameObject m_reward;

	[SerializeField]
	private TextAsset m_catalog;

	private const float purchaseTimeout = 10f;

	private Dictionary<string, InAppPurchaseItemType> m_idDictionary = new Dictionary<string, InAppPurchaseItemType>();

	private Dictionary<InAppPurchaseItemType, string> m_itemDictionary = new Dictionary<InAppPurchaseItemType, string>();

	private Dictionary<InAppPurchaseItemType, BundleItem[]> m_bundlesInfo = new Dictionary<InAppPurchaseItemType, BundleItem[]>();

	private Dictionary<InAppPurchaseItemType, int> m_countDictionary = new Dictionary<InAppPurchaseItemType, int>();

	private Dictionary<InAppPurchaseItemType, int> m_priceDictionary = new Dictionary<InAppPurchaseItemType, int>();

	private InAppPurchaseStatus m_state;

	private InAppPurchaseItemType m_activePurchase;

	private const string NORMAL_COUNT = "normalCount";

	private const string CHINA_COUNT = "chinaCount";

	private const string BUNDLE_ITEMS = "bundleItems";

	private const string ITEM_TYPE = "itemType";

	private const string SNOUT_COIN_PRICE = "snoutCoinPrice";

	private const string SNOUT_COIN_COUNT = "snoutCoinCount";

	private List<IAPProductInfo> productList;

	private IAPInterface m_iap;

	public InAppPurchaseStatus Status => m_state;

	public bool ReadyForTransaction
	{
		get
		{
			if (m_iap != null && m_iap.readyForTransactions())
			{
				return m_state == InAppPurchaseStatus.Idle;
			}
			return false;
		}
	}

	public bool UserInitiatedRestore
	{
		get
		{
			if (m_iap != null)
			{
				return m_iap.UserInitiatedRestore;
			}
			return false;
		}
	}

	public bool PurchaseListInited => m_countDictionary.Count > 0;

	public IEnumerable<IAPProductInfo> ProductList => productList;

	public string LocalCatalogJson
	{
		get
		{
			if (m_catalog != null)
			{
				return m_catalog.text;
			}
			return string.Empty;
		}
	}

	public static event PurchaseSucceeded onPurchaseSucceeded;

	public static event PurchaseFailed onPurchaseFailed;

	public static event ProductListReceived onProductListReceived;

	public static event RestorePurchaseComplete onRestorePurchaseComplete;

	public static event Action<object, CodeRedeemError> onCodeRedeemFailed;

	public static event Action<object, bool> onCodeVerificationCompleted;

	public static event Action onProductListParsed;

	public IAPInterface createIAP()
	{
		return new NullIAP();
	}

	private void Awake()
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IAPEnabled)
		{
			_ = m_iap;
			SetAsPersistant();
			InitDictionaries();
			m_iap = createIAP();
			m_iap.readyForTransactionsEvent += HandleReadyForTransactionsEvent;
			m_iap.purchaseSucceededEvent += HandlePurchaseSucceededEvent;
			m_iap.purchaseFailedEvent += HandlePurchaseFailedEvent;
			m_iap.purchaseCancelledEvent += HandlePurchaseCancelledEvent;
			m_iap.transactionsRestoredEvent += HandleTransactionsRestoredEvent;
			m_iap.transactionRestoreFailedEvent += HandleTransactionsRestoreFailedEvent;
			m_iap.productListReceivedEvent += HandleProductListReceivedEvent;
			m_iap.productListRequestFailedEvent += HandleProductListRequestFailedEvent;
			m_iap.codeRedeemFailedEvent += HandleCodeRedeemFailedEvent;
			m_iap.codeVerificationEvent += HandleCodeVerificationEvent;
			m_iap.deliverItem += HandleDeliverItem;
			m_state = InAppPurchaseStatus.Init;
			m_iap.init();
			EventManager.Connect<PlayerChangedEvent>(OnPlayerChanged);
			EventManager.Connect<LevelLoadedEvent>(OnLevelLoadedEvent);
		}
	}

	private void OnPlayerChanged(PlayerChangedEvent data)
	{
		CreatePages();
	}

	private void Start()
	{
		CreatePages();
	}

	private void CreatePages()
	{
		if (m_iapUnlockFullVersionPageInstance != null)
		{
			UnityEngine.Object.Destroy(m_iapUnlockFullVersionPageInstance);
		}
		m_iapUnlockFullVersionPageInstance = UnityEngine.Object.Instantiate(m_iapUnlockFullVersionPage);
		m_iapUnlockFullVersionPageInstance.transform.parent = base.transform;
		m_iapUnlockFullVersionPageInstance.SetActive(value: false);
		if (m_errorPopupInstance != null)
		{
			UnityEngine.Object.Destroy(m_errorPopupInstance);
		}
		m_errorPopupInstance = UnityEngine.Object.Instantiate(m_errorPopup);
		m_errorPopupInstance.transform.parent = base.transform;
		m_errorPopupInstance.SetActive(value: false);
		if (m_ShopPageInstance != null)
		{
			m_Shop = null;
			UnityEngine.Object.Destroy(m_ShopPageInstance);
		}
		m_ShopPageInstance = UnityEngine.Object.Instantiate(m_ShopPage);
		m_ShopPageInstance.transform.parent = base.transform;
		m_ShopPageInstance.transform.localPosition = Vector3.zero;
		m_ShopPageInstance.SetActive(value: false);
		m_Shop = GetShop();
		MainMenu mainMenu = UnityEngine.Object.FindObjectOfType<MainMenu>();
		if (!(mainMenu == null))
		{
			mainMenu.ConnectShopToRestoreConfirmButton(m_Shop);
		}
	}

	private void OnLevelLoadedEvent(LevelLoadedEvent data)
	{
		if (m_iap != null)
		{
			m_iap.OnLevelWasLoaded();
		}
		if (data.currentGameState != GameManager.GameState.MainMenu)
		{
			return;
		}
		MainMenu mainMenu = UnityEngine.Object.FindObjectOfType<MainMenu>();
		if (mainMenu == null)
		{
			return;
		}
		mainMenu.ConnectShopToRestoreConfirmButton(m_Shop);
		string[] names = Enum.GetNames(typeof(LootCrateType));
		for (int i = 0; i < names.Length - 1; i++)
		{
			int lootcrateAmount = GameProgress.GetLootcrateAmount((LootCrateType)i);
			if (lootcrateAmount > 0)
			{
				WorkshopMenu.AnyLootCrateCollected = true;
				Camera hudCamera = Singleton<GuiManager>.Instance.FindCamera();
				LootCrate.SpawnLootCrateOpeningDialog((LootCrateType)i, lootcrateAmount, hudCamera, null, new LootCrate.AnalyticData("restored", "0", LootCrate.AdWatched.NotApplicaple));
			}
		}
	}

	private void OnDestroy()
	{
		if (m_iap != null)
		{
			m_iap.deInit();
		}
		EventManager.Disconnect<PlayerChangedEvent>(OnPlayerChanged);
		EventManager.Disconnect<LevelLoadedEvent>(OnLevelLoadedEvent);
	}

	public void UpdatePosition()
	{
		base.transform.position = Singleton<GuiManager>.Instance.FindCamera().transform.position + Vector3.forward * 10f;
	}

	public string GetProductIdByItem(InAppPurchaseItemType itemId)
	{
		string value = null;
		if (m_itemDictionary.TryGetValue(itemId, out value))
		{
			return value;
		}
		return string.Empty;
	}

	public InAppPurchaseItemType GetItemByProductId(string productId)
	{
		if (!string.IsNullOrEmpty(productId) && m_idDictionary.ContainsKey(productId))
		{
			return m_idDictionary[productId];
		}
		return InAppPurchaseItemType.Undefined;
	}

	public bool IsBundleProduct(InAppPurchaseItemType itemId)
	{
		return m_bundlesInfo.ContainsKey(itemId);
	}

	private bool IsLootCrateIAP(InAppPurchaseItemType itemId)
	{
		if ((uint)(itemId - 49) <= 1u || (uint)(itemId - 52) <= 2u)
		{
			return true;
		}
		return false;
	}

	public bool SnoutCoinPurchasable(InAppPurchaseItemType itemId)
	{
		switch (itemId)
		{
		default:
			return true;
		case InAppPurchaseItemType.UnlockSpecialSandbox:
		case InAppPurchaseItemType.PermanentBlueprint:
		case InAppPurchaseItemType.SnoutCoinPackSmall:
		case InAppPurchaseItemType.SnoutCoinPackMedium:
		case InAppPurchaseItemType.SnoutCoinPackLarge:
		case InAppPurchaseItemType.SnoutCoinPackHuge:
		case InAppPurchaseItemType.SnoutCoinPackUltimate:
		case InAppPurchaseItemType.StarterPack:
		case InAppPurchaseItemType.MetalLootCrate:
		case InAppPurchaseItemType.GoldenLootCrate:
		case InAppPurchaseItemType.MetalLootCrateSale:
		case InAppPurchaseItemType.GoldenLootCrateSale:
		case InAppPurchaseItemType.GoldenLootCratePack:
		case InAppPurchaseItemType.SnoutCoinPackSmallSale:
		case InAppPurchaseItemType.SnoutCoinPackMediumSale:
		case InAppPurchaseItemType.SnoutCoinPackLargeSale:
		case InAppPurchaseItemType.SnoutCoinPackHugeSale:
		case InAppPurchaseItemType.SnoutCoinPackUltimateSale:
			return false;
		}
	}

	public int SnoutCoinPrice(InAppPurchaseItemType type)
	{
		if (m_priceDictionary.ContainsKey(type))
		{
			return m_priceDictionary[type];
		}
		return int.MaxValue;
	}

	public bool HasBundleInfo(InAppPurchaseItemType itemId)
	{
		return m_bundlesInfo[itemId] != null;
	}

	public IEnumerable<BundleItem> GetBundleInfo(InAppPurchaseItemType itemId)
	{
		return m_bundlesInfo[itemId];
	}

	private void HandleReadyForTransactionsEvent(bool isReady)
	{
		m_state = InAppPurchaseStatus.Idle;
		if (isReady)
		{
			FetchPurchasableItemList();
		}
		if (!m_iap.UserInitiatedRestore && !GameProgress.GetBool("IAPRestored"))
		{
			RestorePurchasedItems();
		}
	}

	private void HandlePurchaseFailedEvent(string error)
	{
		if (m_state == InAppPurchaseStatus.PurchasingItem)
		{
			m_state = InAppPurchaseStatus.Idle;
			ShowErrorPopup("IN_APP_PURCHASE_NOT_READY");
		}
		IapManager.onPurchaseFailed(m_activePurchase);
	}

	private void HandlePurchaseCancelledEvent(string obj)
	{
		m_state = InAppPurchaseStatus.Idle;
		IapManager.onPurchaseFailed(m_activePurchase);
	}

	private bool HandleDeliverItem(string productId)
	{
		return DeliverItem(GetItemByProductId(productId));
	}

	private void HandlePurchaseSucceededEvent(string productId)
	{
		m_state = InAppPurchaseStatus.Idle;
		InAppPurchaseItemType itemByProductId = GetItemByProductId(productId);
		if (itemByProductId != 0)
		{
			if (IapManager.onPurchaseSucceeded != null)
			{
				IapManager.onPurchaseSucceeded(itemByProductId);
			}
		}
		else if (IapManager.onPurchaseFailed != null)
		{
			IapManager.onPurchaseFailed(itemByProductId);
		}
		if (m_Shop != null)
		{
			m_Shop.UnlockScreen();
		}
	}

	private void HandleTransactionsRestoredEvent()
	{
		GameProgress.SetBool("IAPRestored", value: true);
		IapManager.onRestorePurchaseComplete(isSucceeded: true);
	}

	private void HandleTransactionsRestoreFailedEvent(string error)
	{
		IapManager.onRestorePurchaseComplete(isSucceeded: false);
	}

	private void HandleProductListReceivedEvent(List<IAPProductInfo> products)
	{
		m_state = InAppPurchaseStatus.Idle;
		if (products != null && products.Count > 0)
		{
			if (productList == null)
			{
				productList = new List<IAPProductInfo>();
			}
			productList.Clear();
			productList.AddRange(products);
			foreach (IAPProductInfo product in products)
			{
				ParseProductInfo(product);
			}
			if (IapManager.onProductListParsed != null)
			{
				IapManager.onProductListParsed();
			}
		}
		IapManager.onProductListReceived(productList, null);
	}

	private void ParseProductInfo(IAPProductInfo product)
	{
		if (!m_idDictionary.ContainsKey(product.productId))
		{
			return;
		}
		InAppPurchaseItemType inAppPurchaseItemType = m_idDictionary[product.productId];
		if (!m_bundlesInfo.ContainsKey(inAppPurchaseItemType) && !SnoutCoinPurchasable(inAppPurchaseItemType))
		{
			string key = ((!Singleton<BuildCustomizationLoader>.Instance.IsChina) ? "normalCount" : "chinaCount");
			if (!m_countDictionary.ContainsKey(inAppPurchaseItemType) && product.clientData.ContainsKey(key) && int.TryParse(product.clientData[key], out var result))
			{
				m_countDictionary.Add(inAppPurchaseItemType, result);
			}
		}
	}

	private BundleItem[] ParseBundleData(string json)
	{
		object[] obj = JsonReader.Deserialize(json) as object[];
		List<BundleItem> list = new List<BundleItem>();
		object[] array = obj;
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] is Dictionary<string, object> dictionary))
			{
				continue;
			}
			string key = "snoutCoinCount";
			if (dictionary.ContainsKey("itemType") && dictionary.ContainsKey(key))
			{
				int result = 0;
				BundleItem.BundleItemType bundleItemType = (Enum.IsDefined(typeof(BundleItem.BundleItemType), dictionary["itemType"] as string) ? ((BundleItem.BundleItemType)Enum.Parse(typeof(BundleItem.BundleItemType), dictionary["itemType"] as string)) : BundleItem.BundleItemType.Undefined);
				if (int.TryParse(dictionary[key] as string, out result) && bundleItemType != 0)
				{
					list.Add(new BundleItem(bundleItemType, result));
				}
			}
		}
		return list.ToArray();
	}

	private void HandleProductListRequestFailedEvent(string error)
	{
		m_state = InAppPurchaseStatus.Idle;
		IapManager.onProductListReceived(null, error);
	}

	private void HandleCodeRedeemFailedEvent(CodeRedeemError error)
	{
		ShowErrorPopup("REDEEM_CODE_FAILURE_MESSAGE");
		if (IapManager.onCodeRedeemFailed != null)
		{
			IapManager.onCodeRedeemFailed(this, error);
		}
	}

	private void HandleCodeVerificationEvent(bool succeeded)
	{
		if (IapManager.onCodeVerificationCompleted != null)
		{
			IapManager.onCodeVerificationCompleted(this, succeeded);
		}
	}

	public void PurchaseItem(InAppPurchaseItemType type)
	{
		if (SnoutCoinPurchasable(type))
		{
			PurchaseSnoutCoinItem(type);
			return;
		}
		if (!ReadyForTransaction)
		{
			IapManager.onPurchaseFailed(m_activePurchase);
			ShowErrorPopup("IN_APP_PURCHASE_NOT_READY");
			return;
		}
		m_state = InAppPurchaseStatus.PurchasingItem;
		m_activePurchase = type;
		m_iap.purchaseProduct(m_itemDictionary[type]);
		SendStartPurchaseFlurryEvent(type);
	}

	private void PurchaseSnoutCoinItem(InAppPurchaseItemType type)
	{
		int productPrice = Singleton<VirtualCatalogManager>.Instance.GetProductPrice(type);
		if (productPrice <= 0)
		{
			return;
		}
		if (GameProgress.UseSnoutCoins(productPrice))
		{
			if (DeliverItem(type))
			{
				HandlePurchaseSucceededEvent(m_itemDictionary[type]);
			}
			else
			{
				HandlePurchaseFailedEvent("DELIVER_ERROR");
			}
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
		}
		else
		{
			HandlePurchaseFailedEvent("NOT_ENOUGH_SNOUT_COINS");
		}
	}

	private IEnumerator PurchaseTimeout()
	{
		float pollTime = 0.5f;
		float waitTime = 10f;
		while (waitTime > 0f && m_state == InAppPurchaseStatus.PurchasingItem)
		{
			yield return new WaitForSeconds(pollTime);
			waitTime -= pollTime;
		}
		if (m_state == InAppPurchaseStatus.PurchasingItem)
		{
			m_state = InAppPurchaseStatus.Idle;
			IapManager.onPurchaseFailed(m_activePurchase);
			ShowErrorPopup("IN_APP_PURCHASE_TIMEOUT");
		}
	}

	public void FetchPurchasableItemList()
	{
		if (m_state != InAppPurchaseStatus.FetchingItems)
		{
			if (!ReadyForTransaction)
			{
				IapManager.onProductListReceived(null, "IN_APP_PURCHASE_NOT_READY");
				return;
			}
			productList = null;
			m_state = InAppPurchaseStatus.FetchingItems;
			m_iap.fetchAvailableProducts(GetPurchasableItemIdentifiers());
		}
	}

	public void RestorePurchasedItems()
	{
		if (!m_iap.readyForTransactions())
		{
			IapManager.onRestorePurchaseComplete(isSucceeded: false);
		}
		else
		{
			m_iap.restoreTransactions();
		}
	}

	public static string GetProductName()
	{
		string text = "badpiggies";
		if (Singleton<BuildCustomizationLoader>.Instance.IsHDVersion)
		{
			text += "hd";
		}
		return text;
	}

	private void InitDictionaries()
	{
		string productName = GetProductName();
		m_idDictionary.Add("com.rovio." + productName + ".supermechanic_single", InAppPurchaseItemType.BlueprintSingle);
		m_idDictionary.Add("com.rovio." + productName + ".supermechanic_small", InAppPurchaseItemType.BlueprintSmall);
		m_idDictionary.Add("com.rovio." + productName + ".supermechanic_medium", InAppPurchaseItemType.BlueprintMedium);
		m_idDictionary.Add("com.rovio." + productName + ".supermechanic_large", InAppPurchaseItemType.BlueprintLarge);
		m_idDictionary.Add("com.rovio." + productName + ".supermechanic_huge", InAppPurchaseItemType.BlueprintHuge);
		m_idDictionary.Add("com.rovio." + productName + ".superglue_single", InAppPurchaseItemType.SuperGlueSingle);
		m_idDictionary.Add("com.rovio." + productName + ".superglue_small", InAppPurchaseItemType.SuperGlueSmall);
		m_idDictionary.Add("com.rovio." + productName + ".superglue_medium", InAppPurchaseItemType.SuperGlueMedium);
		m_idDictionary.Add("com.rovio." + productName + ".superglue_large", InAppPurchaseItemType.SuperGlueLarge);
		m_idDictionary.Add("com.rovio." + productName + ".superglue_huge", InAppPurchaseItemType.SuperGlueHuge);
		m_idDictionary.Add("com.rovio." + productName + ".supermagnet_single", InAppPurchaseItemType.SuperMagnetSingle);
		m_idDictionary.Add("com.rovio." + productName + ".supermagnet_small", InAppPurchaseItemType.SuperMagnetSmall);
		m_idDictionary.Add("com.rovio." + productName + ".supermagnet_medium", InAppPurchaseItemType.SuperMagnetMedium);
		m_idDictionary.Add("com.rovio." + productName + ".supermagnet_large", InAppPurchaseItemType.SuperMagnetLarge);
		m_idDictionary.Add("com.rovio." + productName + ".supermagnet_huge", InAppPurchaseItemType.SuperMagnetHuge);
		m_idDictionary.Add("com.rovio." + productName + ".turbocharge_single", InAppPurchaseItemType.TurboChargeSingle);
		m_idDictionary.Add("com.rovio." + productName + ".turbocharge_small", InAppPurchaseItemType.TurboChargeSmall);
		m_idDictionary.Add("com.rovio." + productName + ".turbocharge_medium", InAppPurchaseItemType.TurboChargeMedium);
		m_idDictionary.Add("com.rovio." + productName + ".turbocharge_large", InAppPurchaseItemType.TurboChargeLarge);
		m_idDictionary.Add("com.rovio." + productName + ".turbocharge_huge", InAppPurchaseItemType.TurboChargeHuge);
		m_idDictionary.Add("com.rovio." + productName + ".nightvision_single", InAppPurchaseItemType.NightVisionSingle);
		m_idDictionary.Add("com.rovio." + productName + ".nightvision_small", InAppPurchaseItemType.NightVisionSmall);
		m_idDictionary.Add("com.rovio." + productName + ".nightvision_medium", InAppPurchaseItemType.NightVisionMedium);
		m_idDictionary.Add("com.rovio." + productName + ".nightvision_large", InAppPurchaseItemType.NightVisionLarge);
		m_idDictionary.Add("com.rovio." + productName + ".nightvision_huge", InAppPurchaseItemType.NightVisionHuge);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_small", InAppPurchaseItemType.SnoutCoinPackSmall);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_medium", InAppPurchaseItemType.SnoutCoinPackMedium);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_large", InAppPurchaseItemType.SnoutCoinPackLarge);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_huge", InAppPurchaseItemType.SnoutCoinPackHuge);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_ultimate", InAppPurchaseItemType.SnoutCoinPackUltimate);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_small_sale", InAppPurchaseItemType.SnoutCoinPackSmallSale);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_medium_sale", InAppPurchaseItemType.SnoutCoinPackMediumSale);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_large_sale", InAppPurchaseItemType.SnoutCoinPackLargeSale);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_huge_sale", InAppPurchaseItemType.SnoutCoinPackHugeSale);
		m_idDictionary.Add("com.rovio." + productName + ".snout_pack_ultimate_sale", InAppPurchaseItemType.SnoutCoinPackUltimateSale);
		m_idDictionary.Add("com.rovio." + productName + ".bundle_small", InAppPurchaseItemType.BundleStarterPack);
		m_idDictionary.Add("com.rovio." + productName + ".bundle_medium", InAppPurchaseItemType.BundleMediumPack);
		m_idDictionary.Add("com.rovio." + productName + ".bundle_big", InAppPurchaseItemType.BundleBigPack);
		m_idDictionary.Add("com.rovio." + productName + ".bundle_huge", InAppPurchaseItemType.BundleHugePack);
		m_idDictionary.Add("com.rovio." + productName + ".lootcrate_wooden", InAppPurchaseItemType.WoodenLootCrate);
		m_idDictionary.Add("com.rovio." + productName + ".lootcrate_metal", InAppPurchaseItemType.MetalLootCrate);
		m_idDictionary.Add("com.rovio." + productName + ".lootcrate_golden", InAppPurchaseItemType.GoldenLootCrate);
		m_idDictionary.Add("com.rovio." + productName + ".lootcrate_wooden_sale", InAppPurchaseItemType.WoodenLootCrateSale);
		m_idDictionary.Add("com.rovio." + productName + ".lootcrate_metal_sale", InAppPurchaseItemType.MetalLootCrateSale);
		m_idDictionary.Add("com.rovio." + productName + ".lootcrate_golden_sale", InAppPurchaseItemType.GoldenLootCrateSale);
		m_idDictionary.Add("com.rovio." + productName + ".lootcrate_golden_pack", InAppPurchaseItemType.GoldenLootCratePack);
		if (Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			m_idDictionary.Add("com.rovio." + productName + ".unlocktenlevels", InAppPurchaseItemType.UnlockTenLevels);
			m_idDictionary.Add("com.rovio." + productName + ".unlockninelevels", InAppPurchaseItemType.UnlockNineLevels);
			m_idDictionary.Add("com.rovio." + productName + ".unlockepisode", InAppPurchaseItemType.UnlockEpisode);
			m_idDictionary.Add("com.rovio." + productName + ".tendesserts", InAppPurchaseItemType.AddTenDesserts);
		}
		m_idDictionary.Add("com.rovio." + productName + ".special_sandbox", InAppPurchaseItemType.UnlockSpecialSandbox);
		m_idDictionary.Add("com.rovio." + productName + ".mechanic", InAppPurchaseItemType.PermanentBlueprint);
		m_idDictionary.Add("com.rovio." + productName + ".piggy_pack", InAppPurchaseItemType.StarterPack);
		m_itemDictionary.Add(InAppPurchaseItemType.BlueprintSingle, "com.rovio." + productName + ".supermechanic_single");
		m_itemDictionary.Add(InAppPurchaseItemType.BlueprintSmall, "com.rovio." + productName + ".supermechanic_small");
		m_itemDictionary.Add(InAppPurchaseItemType.BlueprintMedium, "com.rovio." + productName + ".supermechanic_medium");
		m_itemDictionary.Add(InAppPurchaseItemType.BlueprintLarge, "com.rovio." + productName + ".supermechanic_large");
		m_itemDictionary.Add(InAppPurchaseItemType.BlueprintHuge, "com.rovio." + productName + ".supermechanic_huge");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperGlueSingle, "com.rovio." + productName + ".superglue_single");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperGlueSmall, "com.rovio." + productName + ".superglue_small");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperGlueMedium, "com.rovio." + productName + ".superglue_medium");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperGlueLarge, "com.rovio." + productName + ".superglue_large");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperGlueHuge, "com.rovio." + productName + ".superglue_huge");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperMagnetSingle, "com.rovio." + productName + ".supermagnet_single");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperMagnetSmall, "com.rovio." + productName + ".supermagnet_small");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperMagnetMedium, "com.rovio." + productName + ".supermagnet_medium");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperMagnetLarge, "com.rovio." + productName + ".supermagnet_large");
		m_itemDictionary.Add(InAppPurchaseItemType.SuperMagnetHuge, "com.rovio." + productName + ".supermagnet_huge");
		m_itemDictionary.Add(InAppPurchaseItemType.TurboChargeSingle, "com.rovio." + productName + ".turbocharge_single");
		m_itemDictionary.Add(InAppPurchaseItemType.TurboChargeSmall, "com.rovio." + productName + ".turbocharge_small");
		m_itemDictionary.Add(InAppPurchaseItemType.TurboChargeMedium, "com.rovio." + productName + ".turbocharge_medium");
		m_itemDictionary.Add(InAppPurchaseItemType.TurboChargeLarge, "com.rovio." + productName + ".turbocharge_large");
		m_itemDictionary.Add(InAppPurchaseItemType.TurboChargeHuge, "com.rovio." + productName + ".turbocharge_huge");
		m_itemDictionary.Add(InAppPurchaseItemType.NightVisionSingle, "com.rovio." + productName + ".nightvision_single");
		m_itemDictionary.Add(InAppPurchaseItemType.NightVisionSmall, "com.rovio." + productName + ".nightvision_small");
		m_itemDictionary.Add(InAppPurchaseItemType.NightVisionMedium, "com.rovio." + productName + ".nightvision_medium");
		m_itemDictionary.Add(InAppPurchaseItemType.NightVisionLarge, "com.rovio." + productName + ".nightvision_large");
		m_itemDictionary.Add(InAppPurchaseItemType.NightVisionHuge, "com.rovio." + productName + ".nightvision_huge");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackSmall, "com.rovio." + productName + ".snout_pack_small");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackMedium, "com.rovio." + productName + ".snout_pack_medium");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackLarge, "com.rovio." + productName + ".snout_pack_large");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackHuge, "com.rovio." + productName + ".snout_pack_huge");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackUltimate, "com.rovio." + productName + ".snout_pack_ultimate");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackSmallSale, "com.rovio." + productName + ".snout_pack_small_sale");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackMediumSale, "com.rovio." + productName + ".snout_pack_medium_sale");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackLargeSale, "com.rovio." + productName + ".snout_pack_large_sale");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackHugeSale, "com.rovio." + productName + ".snout_pack_huge_sale");
		m_itemDictionary.Add(InAppPurchaseItemType.SnoutCoinPackUltimateSale, "com.rovio." + productName + ".snout_pack_ultimate_sale");
		m_itemDictionary.Add(InAppPurchaseItemType.BundleStarterPack, "com.rovio." + productName + ".bundle_small");
		m_itemDictionary.Add(InAppPurchaseItemType.BundleMediumPack, "com.rovio." + productName + ".bundle_medium");
		m_itemDictionary.Add(InAppPurchaseItemType.BundleBigPack, "com.rovio." + productName + ".bundle_big");
		m_itemDictionary.Add(InAppPurchaseItemType.BundleHugePack, "com.rovio." + productName + ".bundle_huge");
		m_itemDictionary.Add(InAppPurchaseItemType.WoodenLootCrate, "com.rovio." + productName + ".lootcrate_wooden");
		m_itemDictionary.Add(InAppPurchaseItemType.MetalLootCrate, "com.rovio." + productName + ".lootcrate_metal");
		m_itemDictionary.Add(InAppPurchaseItemType.GoldenLootCrate, "com.rovio." + productName + ".lootcrate_golden");
		m_itemDictionary.Add(InAppPurchaseItemType.WoodenLootCrateSale, "com.rovio." + productName + ".lootcrate_wooden_sale");
		m_itemDictionary.Add(InAppPurchaseItemType.MetalLootCrateSale, "com.rovio." + productName + ".lootcrate_metal_sale");
		m_itemDictionary.Add(InAppPurchaseItemType.GoldenLootCrateSale, "com.rovio." + productName + ".lootcrate_golden_sale");
		m_itemDictionary.Add(InAppPurchaseItemType.GoldenLootCratePack, "com.rovio." + productName + ".lootcrate_golden_pack");
		if (Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			m_itemDictionary.Add(InAppPurchaseItemType.UnlockTenLevels, "com.rovio." + productName + ".unlocktenlevels");
			m_itemDictionary.Add(InAppPurchaseItemType.UnlockNineLevels, "com.rovio." + productName + ".unlockninelevels");
			m_itemDictionary.Add(InAppPurchaseItemType.UnlockEpisode, "com.rovio." + productName + ".unlockepisode");
			m_itemDictionary.Add(InAppPurchaseItemType.AddTenDesserts, "com.rovio." + productName + ".tendesserts");
		}
		m_itemDictionary.Add(InAppPurchaseItemType.UnlockSpecialSandbox, "com.rovio." + productName + ".special_sandbox");
		m_itemDictionary.Add(InAppPurchaseItemType.PermanentBlueprint, "com.rovio." + productName + ".mechanic");
		m_itemDictionary.Add(InAppPurchaseItemType.StarterPack, "com.rovio." + productName + ".piggy_pack");
		m_bundlesInfo.Add(InAppPurchaseItemType.BundleStarterPack, null);
		m_bundlesInfo.Add(InAppPurchaseItemType.BundleMediumPack, null);
		m_bundlesInfo.Add(InAppPurchaseItemType.BundleBigPack, null);
		m_bundlesInfo.Add(InAppPurchaseItemType.BundleHugePack, null);
	}

	private string[] GetPurchasableItemIdentifiers()
	{
		string productName = GetProductName();
		List<string> list = new List<string>();
		foreach (KeyValuePair<InAppPurchaseItemType, string> item in m_itemDictionary)
		{
			if ((item.Key == InAppPurchaseItemType.SnoutCoinPackMediumSale && Singleton<BuildCustomizationLoader>.Instance.CustomerID.ToLower().Equals("amazon") && productName.ToLower().Equals("badpiggieshdfree")) || (item.Key == InAppPurchaseItemType.SnoutCoinPackHugeSale && Singleton<BuildCustomizationLoader>.Instance.CustomerID.ToLower().Equals("apple")))
			{
				list.Add(item.Value + "1");
			}
			else
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public void EnableUnlockFullVersionPurchasePage()
	{
		m_iapUnlockFullVersionPageInstance.transform.position = GameObject.FindGameObjectWithTag("HUDCamera").transform.position + Vector3.forward * 5f;
		m_iapUnlockFullVersionPageInstance.SetActive(base.enabled);
		m_iapUnlockFullVersionPageInstance.GetComponent<InGameInAppPurchaseMenu>().OpenDialog();
	}

	public void EnableUnlockFullVersionPurchasePage(InGameInAppPurchaseMenu.OnClose onClose)
	{
		m_iapUnlockFullVersionPageInstance.transform.position = GameObject.FindGameObjectWithTag("HUDCamera").transform.position + Vector3.forward * 5f;
		m_iapUnlockFullVersionPageInstance.SetActive(base.enabled);
		m_iapUnlockFullVersionPageInstance.GetComponent<InGameInAppPurchaseMenu>().onClose += onClose;
	}

	public bool IsShopPageOpened()
	{
		return m_ShopPageInstance.activeInHierarchy;
	}

	public void OpenShopPage(Action onClose, string pageName = null)
	{
		m_ShopPageInstance.GetComponent<Shop>().Open(onClose, pageName);
	}

	public Shop GetShop()
	{
		if (m_Shop == null)
		{
			m_Shop = m_ShopPageInstance.GetComponent<Shop>();
		}
		return m_Shop;
	}

	public void ShowErrorPopup(string message)
	{
		m_errorPopupInstance.transform.position = GameObject.FindGameObjectWithTag("HUDCamera").transform.position + Vector3.forward * 1.5f;
		m_errorPopupInstance.SetActive(value: true);
		NotificationPopup componentInChildren = m_errorPopupInstance.GetComponentInChildren<NotificationPopup>();
		componentInChildren.SetText(message);
		componentInChildren.onClose += delegate
		{
			m_ShopPageInstance.GetComponent<Shop>().UnlockScreen();
		};
		componentInChildren.Open();
	}

	private void AddBundleItem(BundleItem item)
	{
		switch (item.type)
		{
		case BundleItem.BundleItemType.Blueprint:
			GameProgress.AddBluePrints(item.count);
			break;
		case BundleItem.BundleItemType.SuperGlue:
			GameProgress.AddSuperGlue(item.count);
			break;
		case BundleItem.BundleItemType.SuperMagnet:
			GameProgress.AddSuperMagnet(item.count);
			break;
		case BundleItem.BundleItemType.TurboCharge:
			GameProgress.AddTurboCharge(item.count);
			break;
		case BundleItem.BundleItemType.NightVision:
			GameProgress.AddNightVision(item.count);
			break;
		}
	}

	private bool DeliverItem(InAppPurchaseItemType product)
	{
		int purchaseItemTypeCount = GetPurchaseItemTypeCount(product);
		switch (product)
		{
		case InAppPurchaseItemType.Undefined:
			return false;
		case InAppPurchaseItemType.UnlockFullVersion:
			if (GameProgress.GetFullVersionUnlocked())
			{
				return false;
			}
			GameProgress.SetFullVersionUnlocked(unlock: true);
			break;
		case InAppPurchaseItemType.UnlockSpecialSandbox:
			TryGiveFreeCrate("S-F_GoldenCrate", "fod");
			if (GameProgress.GetSandboxUnlocked("S-F"))
			{
				return false;
			}
			GameProgress.SetSandboxUnlocked("S-F", unlocked: true);
			GameProgress.UnlockButton("EpisodeButtonSandbox");
			break;
		case InAppPurchaseItemType.BlueprintSmall:
		case InAppPurchaseItemType.BlueprintMedium:
		case InAppPurchaseItemType.BlueprintLarge:
		case InAppPurchaseItemType.BlueprintHuge:
		case InAppPurchaseItemType.BlueprintSingle:
			GameProgress.AddBluePrints(purchaseItemTypeCount);
			SendFlurryInventoryGainEvent(product, purchaseItemTypeCount, string.Empty);
			break;
		case InAppPurchaseItemType.SuperGlueSmall:
		case InAppPurchaseItemType.SuperGlueMedium:
		case InAppPurchaseItemType.SuperGlueLarge:
		case InAppPurchaseItemType.SuperGlueHuge:
		case InAppPurchaseItemType.SuperGlueSingle:
			GameProgress.AddSuperGlue(purchaseItemTypeCount);
			SendFlurryInventoryGainEvent(product, purchaseItemTypeCount, string.Empty);
			break;
		case InAppPurchaseItemType.SuperMagnetSmall:
		case InAppPurchaseItemType.SuperMagnetMedium:
		case InAppPurchaseItemType.SuperMagnetLarge:
		case InAppPurchaseItemType.SuperMagnetHuge:
		case InAppPurchaseItemType.SuperMagnetSingle:
			GameProgress.AddSuperMagnet(purchaseItemTypeCount);
			SendFlurryInventoryGainEvent(product, purchaseItemTypeCount, string.Empty);
			break;
		case InAppPurchaseItemType.TurboChargeSmall:
		case InAppPurchaseItemType.TurboChargeMedium:
		case InAppPurchaseItemType.TurboChargeLarge:
		case InAppPurchaseItemType.TurboChargeHuge:
		case InAppPurchaseItemType.TurboChargeSingle:
			GameProgress.AddTurboCharge(purchaseItemTypeCount);
			SendFlurryInventoryGainEvent(product, purchaseItemTypeCount, string.Empty);
			break;
		case InAppPurchaseItemType.PermanentBlueprint:
			if (GameProgress.GetPermanentBlueprint())
			{
				return false;
			}
			GameProgress.SetPermanentBlueprint(unlock: true);
			break;
		case InAppPurchaseItemType.BundleStarterPack:
		case InAppPurchaseItemType.BundleMediumPack:
		case InAppPurchaseItemType.BundleBigPack:
		case InAppPurchaseItemType.BundleHugePack:
		{
			BundleItem[] productRewardsAsBundleItems = Singleton<VirtualCatalogManager>.Instance.GetProductRewardsAsBundleItems(product);
			for (int j = 0; j < productRewardsAsBundleItems.Length; j++)
			{
				BundleItem item = productRewardsAsBundleItems[j];
				AddBundleItem(item);
				SendFlurryInventoryGainEvent(item.type, item.count);
			}
			break;
		}
		case InAppPurchaseItemType.UnlockTenLevels:
			GameProgress.SetMinimumLockedLevel(Singleton<GameManager>.Instance.CurrentEpisodeIndex, GameProgress.GetMinimumLockedLevel(Singleton<GameManager>.Instance.CurrentEpisodeIndex) + 10);
			break;
		case InAppPurchaseItemType.UnlockEpisode:
			GameProgress.SetMinimumLockedLevel(Singleton<GameManager>.Instance.CurrentEpisodeIndex, GameProgress.GetMinimumLockedLevel(Singleton<GameManager>.Instance.CurrentEpisodeIndex) + 100);
			break;
		case InAppPurchaseItemType.AddTenDesserts:
		{
			GameProgress.AddDesserts("GoldenCake", 2);
			int max = Singleton<GameManager>.Instance.gameData.m_desserts.Count - 1;
			int num = 8;
			for (int i = 0; i < num; i++)
			{
				GameProgress.AddDesserts(Singleton<GameManager>.Instance.gameData.m_desserts[UnityEngine.Random.Range(0, max)].name, 1);
			}
			Singleton<GameManager>.Instance.LoadKingPigFeed(showLoadingScreen: false);
			break;
		}
		case InAppPurchaseItemType.UnlockNineLevels:
			GameProgress.SetMinimumLockedLevel(Singleton<GameManager>.Instance.CurrentEpisodeIndex, GameProgress.GetMinimumLockedLevel(Singleton<GameManager>.Instance.CurrentEpisodeIndex) + 10);
			break;
		case InAppPurchaseItemType.NightVisionSmall:
		case InAppPurchaseItemType.NightVisionMedium:
		case InAppPurchaseItemType.NightVisionLarge:
		case InAppPurchaseItemType.NightVisionHuge:
		case InAppPurchaseItemType.NightVisionSingle:
			GameProgress.AddNightVision(purchaseItemTypeCount);
			SendFlurryInventoryGainEvent(product, purchaseItemTypeCount, string.Empty);
			break;
		case InAppPurchaseItemType.SnoutCoinPackSmall:
		case InAppPurchaseItemType.SnoutCoinPackMedium:
		case InAppPurchaseItemType.SnoutCoinPackLarge:
		case InAppPurchaseItemType.SnoutCoinPackHuge:
		case InAppPurchaseItemType.SnoutCoinPackUltimate:
		case InAppPurchaseItemType.SnoutCoinPackSmallSale:
		case InAppPurchaseItemType.SnoutCoinPackMediumSale:
		case InAppPurchaseItemType.SnoutCoinPackLargeSale:
		case InAppPurchaseItemType.SnoutCoinPackHugeSale:
		case InAppPurchaseItemType.SnoutCoinPackUltimateSale:
			GameProgress.AddSnoutCoins(purchaseItemTypeCount);
			break;
		case InAppPurchaseItemType.StarterPack:
			TryGiveFreeCrate("Starter_GoldenCrate", "starter");
			if (GameProgress.HasStarterPack())
			{
				return false;
			}
			GameProgress.SetStarterPack(unlock: true);
			GameProgress.AddSnoutCoins(purchaseItemTypeCount);
			break;
		case InAppPurchaseItemType.WoodenLootCrate:
		case InAppPurchaseItemType.MetalLootCrate:
		case InAppPurchaseItemType.GoldenLootCrate:
		case InAppPurchaseItemType.WoodenLootCrateSale:
		case InAppPurchaseItemType.MetalLootCrateSale:
		case InAppPurchaseItemType.GoldenLootCrateSale:
		case InAppPurchaseItemType.GoldenLootCratePack:
		{
			LootCrateType crateType = LootCrateType.Wood;
			switch (product)
			{
			case InAppPurchaseItemType.MetalLootCrate:
			case InAppPurchaseItemType.MetalLootCrateSale:
				crateType = LootCrateType.Metal;
				break;
			case InAppPurchaseItemType.GoldenLootCrate:
			case InAppPurchaseItemType.GoldenLootCrateSale:
			case InAppPurchaseItemType.GoldenLootCratePack:
				crateType = LootCrateType.Gold;
				break;
			}
			string gainType = "shop";
			string price = "0";
			switch (product)
			{
			case InAppPurchaseItemType.WoodenLootCrate:
				price = Singleton<VirtualCatalogManager>.Instance.GetProductPrice("lootcrate_wooden").ToString();
				break;
			case InAppPurchaseItemType.MetalLootCrate:
				price = CentsToDecimal(Singleton<VirtualCatalogManager>.Instance.GetProductPrice("lootcrate_metal"));
				break;
			case InAppPurchaseItemType.GoldenLootCrate:
				price = CentsToDecimal(Singleton<VirtualCatalogManager>.Instance.GetProductPrice("lootcrate_gold"));
				break;
			case InAppPurchaseItemType.WoodenLootCrateSale:
				price = Singleton<VirtualCatalogManager>.Instance.GetProductPrice("lootcrate_wooden_sale").ToString();
				break;
			case InAppPurchaseItemType.MetalLootCrateSale:
				price = CentsToDecimal(Singleton<VirtualCatalogManager>.Instance.GetProductPrice("lootcrate_metal_sale"));
				break;
			case InAppPurchaseItemType.GoldenLootCrateSale:
				price = CentsToDecimal(Singleton<VirtualCatalogManager>.Instance.GetProductPrice("lootcrate_gold_sale"));
				break;
			case InAppPurchaseItemType.GoldenLootCratePack:
				price = CentsToDecimal(Singleton<VirtualCatalogManager>.Instance.GetProductPrice("lootcrate_gold_pack"));
				break;
			}
			int amount = 1;
			if (product == InAppPurchaseItemType.GoldenLootCratePack)
			{
				amount = 3;
			}
			GiveLootCrate(crateType, amount, price, gainType);
			break;
		}
		}
		SendCompletedPurchaseFlurryEvent(product);
		return true;
	}

	private void TryGiveFreeCrate(string key, string analyticName)
	{
		if (!GameProgress.GetBool(key))
		{
			GiveLootCrate(LootCrateType.Gold, 1, "free", analyticName);
			GameProgress.SetBool(key, value: true);
		}
	}

	private void GiveLootCrate(LootCrateType crateType, int amount, string price, string gainType)
	{
		GameProgress.AddLootcrate(crateType, amount);
		WorkshopMenu.AnyLootCrateCollected = true;
		Camera hudCamera = Singleton<GuiManager>.Instance.FindCamera();
		LootCrate.SpawnLootCrateOpeningDialog(crateType, amount, hudCamera, null, new LootCrate.AnalyticData(gainType, price, LootCrate.AdWatched.NotApplicaple));
	}

	private string CentsToDecimal(int cents)
	{
		return $"{Mathf.FloorToInt((float)cents / 100f)}.{cents % 100}";
	}

	private void SendFlurryInventoryGainEvent(BundleItem.BundleItemType type, int amount)
	{
		switch (type)
		{
		case BundleItem.BundleItemType.Blueprint:
			SendFlurryInventoryGainEvent(InAppPurchaseItemType.BlueprintSingle, amount, string.Empty);
			break;
		case BundleItem.BundleItemType.SuperGlue:
			SendFlurryInventoryGainEvent(InAppPurchaseItemType.SuperGlueSingle, amount, string.Empty);
			break;
		case BundleItem.BundleItemType.SuperMagnet:
			SendFlurryInventoryGainEvent(InAppPurchaseItemType.SuperMagnetSingle, amount, string.Empty);
			break;
		case BundleItem.BundleItemType.TurboCharge:
			SendFlurryInventoryGainEvent(InAppPurchaseItemType.TurboChargeSingle, amount, string.Empty);
			break;
		case BundleItem.BundleItemType.NightVision:
			SendFlurryInventoryGainEvent(InAppPurchaseItemType.NightVisionSingle, amount, string.Empty);
			break;
		}
	}

	public void SendFlurryInventoryGainEvent(InAppPurchaseItemType type, int amount, string customTypeOfGain = "")
	{
	}

	private void SendStartPurchaseFlurryEvent(InAppPurchaseItemType type)
	{
	}

	public bool IsItemPurchased(InAppPurchaseItemType type)
	{
		return type switch
		{
			InAppPurchaseItemType.UnlockFullVersion => GameProgress.GetFullVersionUnlocked(), 
			InAppPurchaseItemType.UnlockSpecialSandbox => GameProgress.GetSandboxUnlocked("S-F"), 
			InAppPurchaseItemType.StarterPack => GameProgress.HasStarterPack(), 
			InAppPurchaseItemType.PermanentBlueprint => GameProgress.GetPermanentBlueprint(), 
			_ => false, 
		};
	}

	public bool IsCurrencyPack(InAppPurchaseItemType type)
	{
		if ((uint)(type - 36) > 4u && type != InAppPurchaseItemType.StarterPack && (uint)(type - 55) > 4u)
		{
			return false;
		}
		return true;
	}

	public bool IsPowerUp(InAppPurchaseItemType type)
	{
		switch (type)
		{
		case InAppPurchaseItemType.BlueprintSmall:
		case InAppPurchaseItemType.BlueprintMedium:
		case InAppPurchaseItemType.BlueprintLarge:
		case InAppPurchaseItemType.BlueprintHuge:
		case InAppPurchaseItemType.SuperGlueSmall:
		case InAppPurchaseItemType.SuperGlueMedium:
		case InAppPurchaseItemType.SuperGlueLarge:
		case InAppPurchaseItemType.SuperGlueHuge:
		case InAppPurchaseItemType.SuperMagnetSmall:
		case InAppPurchaseItemType.SuperMagnetMedium:
		case InAppPurchaseItemType.SuperMagnetLarge:
		case InAppPurchaseItemType.SuperMagnetHuge:
		case InAppPurchaseItemType.TurboChargeSmall:
		case InAppPurchaseItemType.TurboChargeMedium:
		case InAppPurchaseItemType.TurboChargeLarge:
		case InAppPurchaseItemType.TurboChargeHuge:
		case InAppPurchaseItemType.NightVisionSmall:
		case InAppPurchaseItemType.NightVisionMedium:
		case InAppPurchaseItemType.NightVisionLarge:
		case InAppPurchaseItemType.NightVisionHuge:
		case InAppPurchaseItemType.BlueprintSingle:
		case InAppPurchaseItemType.SuperGlueSingle:
		case InAppPurchaseItemType.SuperMagnetSingle:
		case InAppPurchaseItemType.TurboChargeSingle:
		case InAppPurchaseItemType.NightVisionSingle:
			return true;
		default:
			return false;
		}
	}

	public int GetPurchaseItemTypeCount(InAppPurchaseItemType type)
	{
		if (SnoutCoinPurchasable(type))
		{
			Hashtable productRewards = Singleton<VirtualCatalogManager>.Instance.GetProductRewards(type);
			if (productRewards != null)
			{
				foreach (DictionaryEntry item in productRewards)
				{
					if (Enum.IsDefined(typeof(BundleItem.BundleItemType), (string)item.Key))
					{
						return int.Parse((string)item.Value);
					}
				}
				return 0;
			}
			return 0;
		}
		if (m_countDictionary.ContainsKey(type))
		{
			return m_countDictionary[type];
		}
		if (IsBundleProduct(type))
		{
			return 1;
		}
		if (IsLootCrateIAP(type))
		{
			return 1;
		}
		return 0;
	}

	public string PreparePrice(string formattedPrice)
	{
		return formattedPrice;
	}

	public bool HasProduct(InAppPurchaseItemType productType)
	{
		if (Singleton<IapManager>.Instance.ProductList != null)
		{
			string productIdByItem = Singleton<IapManager>.Instance.GetProductIdByItem(productType);
			if (!string.IsNullOrEmpty(productIdByItem))
			{
				foreach (IAPProductInfo product in Singleton<IapManager>.Instance.ProductList)
				{
					if (product.productId == productIdByItem)
					{
						return true;
					}
				}
				return false;
			}
		}
		return false;
	}

	public string GetFlurryItemName(InAppPurchaseItemType type)
	{
		switch (type)
		{
		case InAppPurchaseItemType.BlueprintSmall:
		case InAppPurchaseItemType.BlueprintMedium:
		case InAppPurchaseItemType.BlueprintLarge:
		case InAppPurchaseItemType.BlueprintHuge:
		case InAppPurchaseItemType.BlueprintSingle:
			return "Blueprint";
		case InAppPurchaseItemType.SuperGlueSmall:
		case InAppPurchaseItemType.SuperGlueMedium:
		case InAppPurchaseItemType.SuperGlueLarge:
		case InAppPurchaseItemType.SuperGlueHuge:
		case InAppPurchaseItemType.SuperGlueSingle:
			return "Super Glue";
		case InAppPurchaseItemType.SuperMagnetSmall:
		case InAppPurchaseItemType.SuperMagnetMedium:
		case InAppPurchaseItemType.SuperMagnetLarge:
		case InAppPurchaseItemType.SuperMagnetHuge:
		case InAppPurchaseItemType.SuperMagnetSingle:
			return "Super Magnet";
		case InAppPurchaseItemType.TurboChargeSmall:
		case InAppPurchaseItemType.TurboChargeMedium:
		case InAppPurchaseItemType.TurboChargeLarge:
		case InAppPurchaseItemType.TurboChargeHuge:
		case InAppPurchaseItemType.TurboChargeSingle:
			return "Turbo Charge";
		case InAppPurchaseItemType.BundleStarterPack:
		case InAppPurchaseItemType.BundleMediumPack:
		case InAppPurchaseItemType.BundleBigPack:
		case InAppPurchaseItemType.BundleHugePack:
			return "Powerup Bundle";
		case InAppPurchaseItemType.NightVisionSmall:
		case InAppPurchaseItemType.NightVisionMedium:
		case InAppPurchaseItemType.NightVisionLarge:
		case InAppPurchaseItemType.NightVisionHuge:
		case InAppPurchaseItemType.NightVisionSingle:
			return "Nightvision";
		case InAppPurchaseItemType.SnoutCoinPackSmall:
		case InAppPurchaseItemType.SnoutCoinPackMedium:
		case InAppPurchaseItemType.SnoutCoinPackLarge:
		case InAppPurchaseItemType.SnoutCoinPackHuge:
		case InAppPurchaseItemType.SnoutCoinPackUltimate:
			return "Snout Coin";
		case InAppPurchaseItemType.SnoutCoinPackSmallSale:
		case InAppPurchaseItemType.SnoutCoinPackMediumSale:
		case InAppPurchaseItemType.SnoutCoinPackLargeSale:
		case InAppPurchaseItemType.SnoutCoinPackHugeSale:
		case InAppPurchaseItemType.SnoutCoinPackUltimateSale:
			return "Snout Coin Sale";
		default:
			return type.ToString();
		}
	}

	private void SendCompletedPurchaseFlurryEvent(InAppPurchaseItemType type)
	{
	}

	static IapManager()
	{
		IapManager.onPurchaseSucceeded = delegate
		{
		};
		IapManager.onPurchaseFailed = delegate
		{
		};
		IapManager.onProductListReceived = delegate
		{
		};
		IapManager.onRestorePurchaseComplete = delegate
		{
		};
	}
}
