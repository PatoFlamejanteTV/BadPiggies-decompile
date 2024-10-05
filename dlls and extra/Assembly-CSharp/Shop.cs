using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Shop : WPFMonoBehaviour
{
	public enum PriceIndicator
	{
		Price,
		Loading,
		NotConnected,
		Purchased
	}

	private class SaleEvent
	{
		public string identifier;

		public DateTime startTime;

		public SaleEvent(string identifier, DateTime startTime)
		{
			this.identifier = identifier;
			this.startTime = startTime;
		}
	}

	private const string SUPER_GLUE_KEY = "IAP_SUPER_GLUE";

	private const string SUPER_MAGNET_KEY = "IAP_SUPER_MAGNET";

	private const string TURBO_CHARGE_KEY = "IAP_TURBO_CHARGE";

	private const string NIGHTVISION_KEY = "IAP_NIGHTVISION";

	private const string LOOTCRATE_KEY = "IAP_WOODEN_LOOTCRATE";

	private const string BLUEPRINT_KEY = "IAP_BLUEPRINT";

	private const string BUNDLE_KEY = "IAP_BUNDLE";

	public GameObject m_SelectionBack;

	public GameObject m_BestValueRibbon;

	public GameObject m_MostPopularRibbon;

	public GameObject m_ShopItems;

	public GameObject m_ShopToolBar;

	public GameObject m_scrollPivot;

	public GameObject m_loadingIndicator;

	public GameObject m_notConnectedIndicator;

	public GameObject m_purchasedIndicator;

	public GameObject m_confirmRestorePopup;

	public GameObject m_LockOverlay;

	public GameObject m_snoutCoinShopPrefab;

	public GameObject m_crateCrazePopupPrefab;

	public GameObject m_coinCrazePopupPrefab;

	public GameObject m_offerBanner;

	public GameObject m_restoreButton;

	private SnoutCoinShopPopup m_snoutCoinShop;

	private TextDialog starterPackDialog;

	private ShopRibbon[] m_ribbons;

	public GameObject[] m_pages;

	private int m_currentPage = -1;

	private PageScroller pageScroller;

	private Action m_onClose;

	private bool isProductListHandlerAttached;

	private bool isStarted;

	private bool showFetchError;

	private bool ducking;

	private bool subscribedToPurchaseEvents;

	private bool waitingSalePopupAnswer;

	private PurchaseProductConfirmDialog confirmDialog;

	public SnoutCoinShopPopup SnoutCoinShop => m_snoutCoinShop;

	private void Awake()
	{
		ShowOfferBanner(show: false);
		pageScroller = GetComponent<PageScroller>();
		pageScroller.PageCount = m_pages.Length;
		pageScroller.OnPageChanged += OnPageChanged;
		m_currentPage = 0;
		pageScroller.SetPage(m_currentPage);
		if (m_currentPage == 0)
		{
			OnPageChanged(-1, m_currentPage);
		}
		EnsureSnoutShop();
		m_BestValueRibbon.SetActive(value: false);
		m_MostPopularRibbon.SetActive(value: false);
		EventManager.Connect<IapManager.PurchaseEvent>(OnPurchaseItem);
		EventManager.Connect<LevelLoadedEvent>(OnLevelLoaded);
		EventManager.Connect<UIEvent>(OnUIEvent);
		EventManager.Connect<PlayFabEvent>(OnPlayFabEvent);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<IapManager.PurchaseEvent>(OnPurchaseItem);
		EventManager.Disconnect<LevelLoadedEvent>(OnLevelLoaded);
		EventManager.Disconnect<UIEvent>(OnUIEvent);
		EventManager.Disconnect<PlayFabEvent>(OnPlayFabEvent);
		if (subscribedToPurchaseEvents)
		{
			IapManager.onPurchaseSucceeded -= OnPurchaseSucceeded;
			IapManager.onPurchaseSucceeded -= UnlockScreen;
			IapManager.onPurchaseFailed -= UnlockScreen;
			IapManager.onRestorePurchaseComplete -= OnRestorePurchaseComplete;
			subscribedToPurchaseEvents = false;
		}
		if (m_LockOverlay != null)
		{
			UnityEngine.Object.Destroy(m_LockOverlay);
		}
	}

	private void OnLevelLoaded(LevelLoadedEvent data)
	{
		if (data.currentGameState != GameManager.GameState.MainMenu)
		{
			return;
		}
		bool isGoldenLootCrateSale = IsSaleOn("GoldenLootCrateSale");
		bool flag = IsSaleOn("GoldenLootCratePack");
		bool isSnoutCoinPackMediumSale = IsSaleOn("SnoutCoinPackMediumSale");
		bool flag2 = IsSaleOn("SnoutCoinPackHugeSale");
		if (isSnoutCoinPackMediumSale || flag2)
		{
			if (!DateTime.TryParse(GameProgress.GetString("CoinCrazeSale_lastShown", DateTime.MinValue.ToShortDateString()), out var result))
			{
				result = DateTime.MinValue;
			}
			if (DateTime.Today.Subtract(result).TotalHours > 12.0 && m_coinCrazePopupPrefab != null)
			{
				TextDialog component = UnityEngine.Object.Instantiate(m_coinCrazePopupPrefab, WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 2f, Quaternion.identity).GetComponent<TextDialog>();
				if (component != null)
				{
					waitingSalePopupAnswer = true;
					component.onClose += delegate
					{
						SendSalePopupFlurryEvent((!isSnoutCoinPackMediumSale) ? "SnoutCoinPackHugeSale" : "SnoutCoinPackMediumSale", "dismissed");
					};
					component.onShopPageOpened += delegate
					{
						SendSalePopupFlurryEvent((!isSnoutCoinPackMediumSale) ? "SnoutCoinPackHugeSale" : "SnoutCoinPackMediumSale", "went_to_shop");
					};
				}
				GameProgress.SetString("CoinCrazeSale_lastShown", DateTime.Today.ToShortDateString());
			}
		}
		if (!(isGoldenLootCrateSale || flag))
		{
			return;
		}
		if (!DateTime.TryParse(GameProgress.GetString("CrateCrazeSale_lastShown", DateTime.MinValue.ToShortDateString()), out var result2))
		{
			result2 = DateTime.MinValue;
		}
		if (result2.Subtract(DateTime.Now).TotalHours > 12.0 && m_crateCrazePopupPrefab != null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(m_crateCrazePopupPrefab, WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 5f, Quaternion.identity);
			TextDialog component2 = obj.GetComponent<TextDialog>();
			if (component2 != null)
			{
				waitingSalePopupAnswer = true;
				component2.onClose += delegate
				{
					SendSalePopupFlurryEvent((!isGoldenLootCrateSale) ? "GoldenLootCratePack" : "GoldenLootCrateSale", "dismissed");
				};
				component2.onShopPageOpened += delegate
				{
					SendSalePopupFlurryEvent((!isGoldenLootCrateSale) ? "GoldenLootCratePack" : "GoldenLootCrateSale", "went_to_shop");
				};
			}
			GameProgress.SetString("CrateCrazeSale_lastShown", DateTime.Now.ToShortDateString());
			Transform transform = obj.transform.Find("Text_2");
			if (transform != null)
			{
				TextMesh component3 = transform.GetComponent<TextMesh>();
				TextMeshLocale component4 = transform.GetComponent<TextMeshLocale>();
				if (component3 != null && component4 != null)
				{
					if (flag)
					{
						component3.text = "CRATE_CRAZE_MESSAGE_02";
					}
					else if (isGoldenLootCrateSale)
					{
						component3.text = "CRATE_CRAZE_MESSAGE";
					}
					component4.RefreshTranslation();
				}
			}
		}
		ShowOfferBanner(show: true);
	}

	private void SendSalePopupFlurryEvent(string offerName, string action)
	{
	}

	private void HandleOnNotificationStatusChanged(bool enabled)
	{
	}

	private bool IsSaleOn(string identifier)
	{
		if (string.IsNullOrEmpty(identifier))
		{
			return false;
		}
		ConfigData config = Singleton<GameConfigurationManager>.Instance.GetConfig("iap_sale_items");
		if (config == null)
		{
			return false;
		}
		for (int i = 0; i < config.Count; i++)
		{
			if (config.Keys[i].StartsWith(identifier))
			{
				string text = config.Values[i];
				if (string.IsNullOrEmpty(text))
				{
					return false;
				}
				string[] array = text.Split('-');
				if (array != null && array.Length != 0 && GetSaleTimeLeft(array[0], (array.Length <= 1) ? string.Empty : array[1]) > 0 && !GameProgress.GetBool(config.Keys[i] + "_used"))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void ShowOfferBanner(bool show)
	{
		if (m_offerBanner != null)
		{
			m_offerBanner.SetActive(show);
		}
	}

	public static DateTime ConvertStringToDate(string rawDate)
	{
		string[] formats = new string[8] { "d.M.yy", "d.MM.yy", "dd.M.yy", "dd.MM.yy", "d.M.yyyy", "d.MM.yyyy", "dd.M.yyyy", "dd.MM.yyyy" };
		if (!string.IsNullOrEmpty(rawDate) && DateTime.TryParseExact(rawDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
		{
			return result;
		}
		return DateTime.MinValue;
	}

	public static int GetSaleTimeLeft(string saleStart, string saleEnd)
	{
		DateTime date = ConvertStringToDate(saleStart);
		DateTime date2 = ConvertStringToDate(saleEnd).AddDays(1.0);
		int num = TimeManager.ConvertDateTime2Seconds(date);
		int num2 = TimeManager.ConvertDateTime2Seconds(date2);
		if (num > 0 && num2 <= num)
		{
			num2 = num + 86400;
		}
		int currentEpochTime = Singleton<TimeManager>.Instance.CurrentEpochTime;
		if (num < currentEpochTime && num2 > currentEpochTime)
		{
			return num2 - currentEpochTime;
		}
		return -1;
	}

	private void Start()
	{
		if (Singleton<IapManager>.IsInstantiated() && !Singleton<IapManager>.Instance.UserInitiatedRestore)
		{
			base.transform.Find("RestoreButton").gameObject.SetActive(value: false);
		}
		isStarted = true;
		LayoutItems();
		m_confirmRestorePopup.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 5f;
		UpdateAllData();
	}

	private void EnsureSnoutShop()
	{
		if (m_snoutCoinShop == null && m_snoutCoinShopPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_snoutCoinShopPrefab);
			gameObject.transform.parent = Singleton<IapManager>.Instance.transform;
			gameObject.transform.localPosition = Vector3.back * 6f;
			gameObject.name = m_snoutCoinShopPrefab.name;
			m_snoutCoinShop = gameObject.GetComponent<SnoutCoinShopPopup>();
			m_snoutCoinShop.UpdatePrices(this);
			gameObject.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		if (isStarted)
		{
			UpdateAllData();
		}
		if (!subscribedToPurchaseEvents)
		{
			IapManager.onPurchaseSucceeded += OnPurchaseSucceeded;
			IapManager.onPurchaseSucceeded += UnlockScreen;
			IapManager.onPurchaseFailed += UnlockScreen;
			IapManager.onRestorePurchaseComplete += OnRestorePurchaseComplete;
			subscribedToPurchaseEvents = true;
		}
		KeyListener.keyReleased += HandleKeyReleased;
		Singleton<KeyListener>.Instance.GrabFocus(this);
		UpdateLowerButtons(GameProgress.GetInt("LastShopPage"));
		UpdateBestPrices();
	}

	private void OnPlayFabEvent(PlayFabEvent data)
	{
		if (data.type == PlayFabEvent.Type.LocalDataUpdated)
		{
			UpdatePurchasedCounters();
		}
	}

	private void OnUIEvent(UIEvent data)
	{
		switch (data.type)
		{
		case UIEvent.Type.ClosedSnoutCoinShop:
			if (ducking)
			{
				base.gameObject.SetActive(value: true);
				ducking = false;
			}
			break;
		case UIEvent.Type.OpenedSnoutCoinShop:
			if (base.gameObject.activeInHierarchy)
			{
				ducking = base.gameObject.activeInHierarchy;
				base.gameObject.SetActive(value: false);
			}
			break;
		}
	}

	private void LockScreen()
	{
		m_LockOverlay.transform.parent = base.transform.parent;
		m_LockOverlay.SetActive(value: true);
		EventManager.Send(new UIEvent(UIEvent.Type.ShopLockedScreen));
	}

	public void UnlockScreen(IapManager.InAppPurchaseItemType type = IapManager.InAppPurchaseItemType.Undefined)
	{
		m_LockOverlay.SetActive(value: false);
		EventManager.Send(new UIEvent(UIEvent.Type.ShopUnlockedScreen));
	}

	private void UpdateAllData()
	{
		if (!isProductListHandlerAttached)
		{
			IapManager.onProductListReceived += OnProductListReceived;
			VirtualCatalogManager.onVirtualProductListParsed += OnVirtualProductListParsed;
			isProductListHandlerAttached = true;
		}
		if (Singleton<IapManager>.Instance.ProductList == null && Singleton<IapManager>.Instance.Status != IapManager.InAppPurchaseStatus.FetchingItems)
		{
			Singleton<IapManager>.Instance.FetchPurchasableItemList();
		}
		UpdatePrices(IapManager.CurrencyType.RealMoney, updateAll: true);
		UpdatePurchasedCounters();
	}

	private void HandleKeyReleased(KeyCode keyCode)
	{
		if (keyCode == KeyCode.Escape && !m_LockOverlay.activeSelf)
		{
			Close();
		}
	}

	private void OnDisable()
	{
		if (isProductListHandlerAttached)
		{
			IapManager.onProductListReceived -= OnProductListReceived;
			VirtualCatalogManager.onVirtualProductListParsed -= OnVirtualProductListParsed;
			isProductListHandlerAttached = false;
		}
		if (subscribedToPurchaseEvents)
		{
			IapManager.onPurchaseSucceeded -= OnPurchaseSucceeded;
			IapManager.onPurchaseSucceeded -= UnlockScreen;
			IapManager.onPurchaseFailed -= UnlockScreen;
			IapManager.onRestorePurchaseComplete -= OnRestorePurchaseComplete;
			subscribedToPurchaseEvents = false;
		}
		KeyListener.keyReleased -= HandleKeyReleased;
		if (Singleton<KeyListener>.IsInstantiated())
		{
			Singleton<KeyListener>.Instance.ReleaseFocus(this);
		}
	}

	private void OnProductListReceived(List<IAPProductInfo> products, string error)
	{
		if (showFetchError && (products == null || products.Count == 0))
		{
			Singleton<IapManager>.Instance.ShowErrorPopup("IN_APP_PURCHASE_NOT_READY");
		}
		showFetchError = false;
		UpdatePrices();
		UpdatePurchasedCounters();
	}

	private void OnVirtualProductListParsed()
	{
		showFetchError = false;
		UpdatePrices(IapManager.CurrencyType.SnoutCoin);
		UpdatePurchasedCounters();
	}

	private void UpdateBestPrices()
	{
		List<ShopRibbon> list = new List<ShopRibbon>();
		Hashtable values = Singleton<GameConfigurationManager>.Instance.GetValues("shop_ribbons");
		if (values == null)
		{
			return;
		}
		foreach (DictionaryEntry item in values)
		{
			string[] array = item.Key.ToString().Split('/');
			if (array.Length < 2)
			{
				continue;
			}
			ShopRibbon shopRibbon = new ShopRibbon();
			if (!array[0].Equals("Android"))
			{
				continue;
			}
			shopRibbon.platform = RuntimePlatform.Android;
			shopRibbon.itemId = string.Empty;
			for (int i = 1; i < array.Length; i++)
			{
				if (i > 1)
				{
					shopRibbon.itemId += "/";
				}
				shopRibbon.itemId += array[i];
			}
			if (int.TryParse(item.Value.ToString(), out var result) && Enum.IsDefined(typeof(ShopRibbon.Ribbon), result))
			{
				shopRibbon.ribbonType = (ShopRibbon.Ribbon)result;
				list.Add(shopRibbon);
			}
		}
		m_ribbons = list.ToArray();
		UpdateRibbons();
	}

	private void OnRestorePurchaseComplete(bool isSucceeded)
	{
		if (!isSucceeded)
		{
			Singleton<IapManager>.Instance.ShowErrorPopup("IN_APP_PURCHASE_NOT_READY");
		}
	}

	public string GetFormattedPrice(IapManager.InAppPurchaseItemType purchaseType)
	{
		if (!Singleton<IapManager>.Instance.SnoutCoinPurchasable(purchaseType))
		{
			if (Singleton<IapManager>.Instance.ProductList != null)
			{
				string productIdByItem = Singleton<IapManager>.Instance.GetProductIdByItem(purchaseType);
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
		int productPrice = Singleton<VirtualCatalogManager>.Instance.GetProductPrice(purchaseType);
		if (productPrice == int.MaxValue || productPrice < 0)
		{
			return null;
		}
		return $"[snout] {productPrice}";
	}

	private void UpdatePrices(IapManager.CurrencyType updateType = IapManager.CurrencyType.RealMoney, bool updateAll = false)
	{
		if (!updateAll && (updateType != 0 || !Singleton<IapManager>.IsInstantiated()) && (updateType == IapManager.CurrencyType.RealMoney || !Singleton<VirtualCatalogManager>.IsInstantiated()))
		{
			return;
		}
		m_snoutCoinShop.UpdatePrices(this);
		PurchaseInfo[] componentsInChildren = base.gameObject.GetComponentsInChildren<PurchaseInfo>();
		foreach (PurchaseInfo purchaseInfo in componentsInChildren)
		{
			if (updateAll || purchaseInfo.currencyType == updateType)
			{
				purchaseInfo.CheckOnSale();
				string formattedSalePrice = ((!purchaseInfo.isSaleItem) ? null : GetFormattedPrice(purchaseInfo.saleItem));
				SetPriceIndicator(purchaseInfo, GetFormattedPrice(purchaseInfo.purchaseItem), formattedSalePrice);
			}
		}
	}

	public void SetPriceIndicator(PurchaseInfo infoData, string formattedPrice = null, string formattedSalePrice = null)
	{
		if (infoData.purchaseItem == IapManager.InAppPurchaseItemType.Undefined && !infoData.isSaleItem)
		{
			infoData.Show(show: false);
			return;
		}
		bool num = Singleton<IapManager>.Instance.SnoutCoinPurchasable(infoData.purchaseItem);
		Transform transform = infoData.transform.FindChildRecursively("Price");
		Transform transform2 = infoData.transform.Find("Sale");
		if ((bool)transform2)
		{
			transform2.gameObject.SetActive(value: false);
		}
		Vector3 vector = ((!num) ? Vector3.zero : (-Vector3.right * transform.transform.localPosition.x));
		Transform transform3 = transform.FindChildOrInstantiate("LoadingIndicator", m_loadingIndicator, transform.transform.position + vector, transform.transform.rotation);
		Transform transform4 = transform.FindChildOrInstantiate("NotConnectedIndicator", m_notConnectedIndicator, transform.transform.position + vector, transform.transform.rotation);
		Transform transform5 = transform.FindChildOrInstantiate("PurchasedIndicator", m_purchasedIndicator, transform.transform.position + vector, transform.transform.rotation);
		Transform transform6 = infoData.transform.Find("HideIfPurchased");
		Renderer component = infoData.GetComponent<Renderer>();
		if (component != null)
		{
			Renderer[] componentsInChildren = transform3.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].sortingLayerID = component.sortingLayerID;
				}
			}
			componentsInChildren = transform4.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].sortingLayerID = component.sortingLayerID;
				}
			}
			componentsInChildren = transform5.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					componentsInChildren[k].sortingLayerID = component.sortingLayerID;
				}
			}
		}
		transform3.gameObject.SetActive(value: false);
		transform4.gameObject.SetActive(value: false);
		transform5.gameObject.SetActive(value: false);
		bool flag = Singleton<IapManager>.Instance.IsItemPurchased(infoData.purchaseItem);
		Renderer[] componentsInChildren2 = transform.GetComponentsInChildren<Renderer>();
		for (int l = 0; l < 2; l++)
		{
			if ((infoData.doUpdatePrice || flag) && l < componentsInChildren2.Length && componentsInChildren2[l] != null)
			{
				componentsInChildren2[l].enabled = false;
			}
		}
		if (Singleton<IapManager>.Instance.Status == IapManager.InAppPurchaseStatus.FetchingItems)
		{
			transform3.gameObject.SetActive(value: true);
			return;
		}
		if (flag)
		{
			infoData.EnableCollider(enable: false);
			transform5.gameObject.SetActive(value: true);
			if (transform6 != null)
			{
				transform6.gameObject.SetActive(value: false);
			}
			Button component2 = infoData.GetComponent<Button>();
			if ((bool)component2)
			{
				UnityEngine.Object.Destroy(component2);
			}
			return;
		}
		if (!string.IsNullOrEmpty(formattedPrice) || (infoData.isSaleItem && !string.IsNullOrEmpty(formattedSalePrice)))
		{
			infoData.EnableCollider();
			bool doUpdatePrice = infoData.doUpdatePrice;
			TextMesh[] componentsInChildren3 = transform.GetComponentsInChildren<TextMesh>();
			if (!string.IsNullOrEmpty(formattedPrice))
			{
				for (int m = 0; m < componentsInChildren3.Length; m++)
				{
					if (componentsInChildren2 != null && m < componentsInChildren2.Length && componentsInChildren2[m] != null)
					{
						componentsInChildren2[m].enabled = doUpdatePrice || !flag;
					}
					if (doUpdatePrice && m < componentsInChildren3.Length && componentsInChildren3[m] != null)
					{
						componentsInChildren3[m].text = formattedPrice;
						TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren3[m]);
					}
				}
			}
			if (!infoData.isSaleItem || !(transform2 != null) || string.IsNullOrEmpty(formattedSalePrice))
			{
				return;
			}
			if (Singleton<IapManager>.Instance.IsCurrencyPack(infoData.saleItem))
			{
				Transform transform7 = transform2.Find("OldPrice");
				if (!transform7)
				{
					return;
				}
				Renderer[] componentsInChildren4 = transform7.GetComponentsInChildren<Renderer>();
				TextMesh[] componentsInChildren5 = transform7.GetComponentsInChildren<TextMesh>();
				for (int n = 0; n < componentsInChildren5.Length; n++)
				{
					if (componentsInChildren4 != null && n < componentsInChildren4.Length && componentsInChildren4[n] != null)
					{
						componentsInChildren4[n].enabled = doUpdatePrice || !flag;
					}
					if (doUpdatePrice && n < componentsInChildren5.Length && componentsInChildren5[n] != null)
					{
						if (!string.IsNullOrEmpty(formattedPrice))
						{
							componentsInChildren5[n].text = formattedPrice;
						}
						TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren5[n]);
					}
				}
				for (int num2 = 0; num2 < componentsInChildren3.Length; num2++)
				{
					if (doUpdatePrice && num2 < componentsInChildren3.Length && componentsInChildren3[num2] != null)
					{
						componentsInChildren3[num2].text = formattedSalePrice;
						TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren3[num2]);
					}
				}
				transform2.gameObject.SetActive(value: true);
				return;
			}
			Transform transform8 = transform2.Find("Tag_pivot");
			if (!transform8)
			{
				return;
			}
			Renderer[] componentsInChildren6 = transform8.GetComponentsInChildren<Renderer>();
			TextMesh[] componentsInChildren7 = transform8.GetComponentsInChildren<TextMesh>();
			for (int num3 = 0; num3 < componentsInChildren7.Length; num3++)
			{
				if (componentsInChildren6 != null && num3 < componentsInChildren6.Length && componentsInChildren6[num3] != null)
				{
					componentsInChildren6[num3].enabled = doUpdatePrice || !flag;
				}
				if (doUpdatePrice && num3 < componentsInChildren7.Length && componentsInChildren7[num3] != null)
				{
					componentsInChildren7[num3].text = formattedSalePrice;
					TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren7[num3]);
				}
			}
			transform2.gameObject.SetActive(value: true);
			return;
		}
		infoData.EnableCollider(enable: false);
		transform4.gameObject.SetActive(value: true);
		for (int num4 = 0; num4 < 2; num4++)
		{
			if (num4 < componentsInChildren2.Length && componentsInChildren2[num4] != null)
			{
				componentsInChildren2[num4].enabled = false;
			}
		}
	}

	private bool IsPowerUpPage(string pageName)
	{
		if (!pageName.Equals("SuperGlue") && !pageName.Equals("TurboCharge") && !pageName.Equals("SuperMagnet") && !pageName.Equals("SuperBluePrints"))
		{
			return pageName.Equals("NightVisions");
		}
		return true;
	}

	public void ChangeShopPage(string pageName, bool immediatly = false)
	{
		bool flag = pageName.Equals("PowerUps");
		bool num = IsPowerUpPage(pageName);
		bool flag2 = IsPowerUpPage(m_pages[m_currentPage].name);
		if ((num || flag) && flag && !flag2)
		{
			pageName = "SuperGlue";
		}
		for (int i = 0; i < m_pages.Length; i++)
		{
			if (m_pages[i].name == pageName)
			{
				if (immediatly)
				{
					GetComponent<PageScroller>().SetPage(i);
				}
				else
				{
					GetComponent<PageScroller>().ScrollToPage(i);
				}
				UpdateLowerButtons(i);
				break;
			}
		}
	}

	private void UpdateLowerButtons(int page)
	{
		m_restoreButton.SetActive(value: false);
		m_currentPage = page;
	}

	private void UpdateRibbons()
	{
		if (m_ribbons == null || m_ribbons.Length == 0)
		{
			return;
		}
		for (int i = 0; i < m_ribbons.Length; i++)
		{
			m_ribbons[i].ribbon = AddRibbon(m_ribbons[i]);
			if (m_ribbons[i].ribbon == null)
			{
				m_ribbons[i].ribbon = m_snoutCoinShop.AddRibbon(m_ribbons[i]);
			}
		}
	}

	private GameObject AddRibbon(ShopRibbon ribbon)
	{
		Transform transform = m_ShopItems.transform.Find("Pages/" + ribbon.itemId);
		if (!transform || !transform.gameObject.activeInHierarchy)
		{
			return null;
		}
		string text = string.Empty;
		GameObject original = null;
		switch (ribbon.ribbonType)
		{
		case ShopRibbon.Ribbon.MostPopular:
			text = "MostPopularRibbon";
			original = m_MostPopularRibbon;
			break;
		case ShopRibbon.Ribbon.BestValue:
			text = "BestValueRibbon";
			original = m_BestValueRibbon;
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		Transform transform2 = transform.Find(text);
		GameObject gameObject;
		if (transform2 == null)
		{
			gameObject = UnityEngine.Object.Instantiate(original);
			gameObject.name = text;
		}
		else
		{
			gameObject = transform2.gameObject;
		}
		Vector3 position = transform.position;
		gameObject.transform.position = position + Vector3.back;
		gameObject.transform.parent = transform;
		gameObject.SetActive(value: true);
		return gameObject;
	}

	public void UpdatePurchasedCounters()
	{
		if (m_ShopToolBar == null)
		{
			return;
		}
		Transform transform = m_ShopToolBar.transform.Find("Pages");
		int num = transform.childCount;
		while (--num >= 0)
		{
			Transform child = transform.GetChild(num);
			Transform transform2 = child.Find("Counter/Text");
			if ((bool)transform2)
			{
				if (child.name == "SuperGlue")
				{
					transform2.GetComponent<TextMesh>().text = GameProgress.SuperGlueCount().ToString();
				}
				else if (child.name == "SuperMagnet")
				{
					transform2.GetComponent<TextMesh>().text = GameProgress.SuperMagnetCount().ToString();
				}
				else if (child.name == "TurboCharge")
				{
					transform2.GetComponent<TextMesh>().text = GameProgress.TurboChargeCount().ToString();
				}
				else if (child.name == "SuperBluePrints")
				{
					transform2.GetComponent<TextMesh>().text = GameProgress.BluePrintCount().ToString();
				}
				else if (child.name == "NightVisions")
				{
					transform2.GetComponent<TextMesh>().text = GameProgress.NightVisionCount().ToString();
				}
			}
		}
	}

	private void LayoutItems()
	{
		float num = 2f * WPFMonoBehaviour.hudCamera.orthographicSize * (float)Screen.width / (float)Screen.height;
		for (int i = 0; i < m_pages.Length; i++)
		{
			Vector3 localPosition = m_pages[i].transform.localPosition;
			localPosition.x = (float)i * num;
			m_pages[i].transform.localPosition = localPosition;
			m_pages[i].SetActive(value: true);
		}
	}

	private void OnPageChanged(int oldPage, int newPage)
	{
		Transform transform = m_ShopToolBar.transform.Find("Pages");
		if (oldPage >= 0)
		{
			Transform child = transform.GetChild(oldPage);
			Vector3 localPosition = child.localPosition;
			localPosition.y = 0f;
			StartCoroutine(MoveTo(child, child.localPosition, localPosition));
		}
		if (newPage >= 0)
		{
			Transform child2 = transform.GetChild(newPage);
			Vector3 localPosition2 = child2.localPosition;
			localPosition2.y = 0.4f;
			StartCoroutine(MoveTo(child2, child2.localPosition, localPosition2));
		}
		UpdateLowerButtons(newPage);
	}

	private void OnPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		UpdatePurchasedCounters();
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(WaitAndUpdate());
		}
	}

	private IEnumerator WaitAndUpdate()
	{
		yield return null;
		UpdatePrices();
	}

	public void Open(Action onClose, string pageName = null)
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			Close();
			return;
		}
		Singleton<IapManager>.Instance.UpdatePosition();
		if (m_snoutCoinShopPrefab != null && m_snoutCoinShopPrefab.name.Equals(pageName))
		{
			if (!m_snoutCoinShop.gameObject.activeInHierarchy)
			{
				m_snoutCoinShop.transform.localPosition = Vector3.back * 6f;
				m_snoutCoinShop.Open(onClose);
				UpdateLowerButtons(0);
			}
			return;
		}
		if (onClose != null)
		{
			m_onClose = (Action)Delegate.Combine(m_onClose, onClose);
		}
		if (!base.gameObject.activeSelf)
		{
			EventManager.Send(new UIEvent(UIEvent.Type.OpenIapMenu));
			base.gameObject.SetActive(value: true);
		}
		LayoutItems();
		if (!string.IsNullOrEmpty(pageName))
		{
			ChangeShopPage(pageName, immediatly: true);
		}
		else
		{
			ChangeShopPage("LootCrates", immediatly: true);
		}
	}

	public void Close()
	{
		if (base.gameObject.activeSelf)
		{
			EventManager.Send(new UIEvent(UIEvent.Type.CloseIapMenu));
			base.gameObject.SetActive(value: false);
		}
		if (m_onClose != null)
		{
			m_onClose();
		}
		m_onClose = null;
	}

	public void ConfirmSinglePurchase(string inAppPurchaseItemTypeAsString, string spriteID, string effectID, int itemCount, Action onClose)
	{
		if (Enum.IsDefined(typeof(IapManager.InAppPurchaseItemType), inAppPurchaseItemTypeAsString))
		{
			IapManager.InAppPurchaseItemType inAppPurchaseItemType = (IapManager.InAppPurchaseItemType)Enum.Parse(typeof(IapManager.InAppPurchaseItemType), inAppPurchaseItemTypeAsString);
			int price = Singleton<VirtualCatalogManager>.Instance.GetProductPrice(inAppPurchaseItemType);
			string itemKey = ItemLocalizationKey(inAppPurchaseItemType);
			string productLocalizationKey = Singleton<VirtualCatalogManager>.Instance.GetProductLocalizationKey(inAppPurchaseItemType);
			ShowConfirmationPopup(Singleton<GameManager>.Instance.gameData.m_purchaseProductConfirmDialog, inAppPurchaseItemTypeAsString, spriteID, effectID, itemKey, productLocalizationKey, price, itemCount, () => GameProgress.SnoutCoinCount() >= price, onClose);
		}
	}

	public void ConfirmPurchase(string inAppPurchaseItemTypeAsString, string spriteID, string effectID, int itemCount)
	{
		if (Enum.IsDefined(typeof(IapManager.InAppPurchaseItemType), inAppPurchaseItemTypeAsString))
		{
			IapManager.InAppPurchaseItemType inAppPurchaseItemType = (IapManager.InAppPurchaseItemType)Enum.Parse(typeof(IapManager.InAppPurchaseItemType), inAppPurchaseItemTypeAsString);
			int price = Singleton<VirtualCatalogManager>.Instance.GetProductPrice(inAppPurchaseItemType);
			string itemKey = ItemLocalizationKey(inAppPurchaseItemType);
			string productLocalizationKey = Singleton<VirtualCatalogManager>.Instance.GetProductLocalizationKey(inAppPurchaseItemType);
			ShowConfirmationPopup(Singleton<GameManager>.Instance.gameData.m_purchaseProductConfirmDialog, inAppPurchaseItemTypeAsString, spriteID, effectID, itemKey, productLocalizationKey, price, itemCount, () => GameProgress.SnoutCoinCount() >= price);
		}
	}

	public void ConfirmLootCratePurchase(string inAppPurchaseItemTypeAsString)
	{
		if (Enum.IsDefined(typeof(IapManager.InAppPurchaseItemType), inAppPurchaseItemTypeAsString))
		{
			IapManager.InAppPurchaseItemType inAppPurchaseItemType = (IapManager.InAppPurchaseItemType)Enum.Parse(typeof(IapManager.InAppPurchaseItemType), inAppPurchaseItemTypeAsString);
			int price = Singleton<VirtualCatalogManager>.Instance.GetProductPrice(inAppPurchaseItemType);
			string itemKey = ItemLocalizationKey(inAppPurchaseItemType);
			string productLocalizationKey = Singleton<VirtualCatalogManager>.Instance.GetProductLocalizationKey(inAppPurchaseItemType);
			ShowConfirmationPopup(Singleton<GameManager>.Instance.gameData.m_purchaseLootcrateConfirmDialog, inAppPurchaseItemTypeAsString, string.Empty, string.Empty, itemKey, productLocalizationKey, price, 0, () => GameProgress.SnoutCoinCount() >= price);
		}
	}

	private string ItemLocalizationKey(IapManager.InAppPurchaseItemType type)
	{
		switch (type)
		{
		case IapManager.InAppPurchaseItemType.BlueprintSmall:
		case IapManager.InAppPurchaseItemType.BlueprintMedium:
		case IapManager.InAppPurchaseItemType.BlueprintLarge:
		case IapManager.InAppPurchaseItemType.BlueprintHuge:
		case IapManager.InAppPurchaseItemType.BlueprintUltimate:
		case IapManager.InAppPurchaseItemType.BlueprintSingle:
			return "IAP_BLUEPRINT";
		case IapManager.InAppPurchaseItemType.SuperGlueSmall:
		case IapManager.InAppPurchaseItemType.SuperGlueMedium:
		case IapManager.InAppPurchaseItemType.SuperGlueLarge:
		case IapManager.InAppPurchaseItemType.SuperGlueHuge:
		case IapManager.InAppPurchaseItemType.SuperGlueUltimate:
		case IapManager.InAppPurchaseItemType.SuperGlueSingle:
			return "IAP_SUPER_GLUE";
		case IapManager.InAppPurchaseItemType.SuperMagnetSmall:
		case IapManager.InAppPurchaseItemType.SuperMagnetMedium:
		case IapManager.InAppPurchaseItemType.SuperMagnetLarge:
		case IapManager.InAppPurchaseItemType.SuperMagnetHuge:
		case IapManager.InAppPurchaseItemType.SuperMagnetUltimate:
		case IapManager.InAppPurchaseItemType.SuperMagnetSingle:
			return "IAP_SUPER_MAGNET";
		case IapManager.InAppPurchaseItemType.TurboChargeSmall:
		case IapManager.InAppPurchaseItemType.TurboChargeMedium:
		case IapManager.InAppPurchaseItemType.TurboChargeLarge:
		case IapManager.InAppPurchaseItemType.TurboChargeHuge:
		case IapManager.InAppPurchaseItemType.TurboChargeUltimate:
		case IapManager.InAppPurchaseItemType.TurboChargeSingle:
			return "IAP_TURBO_CHARGE";
		case IapManager.InAppPurchaseItemType.BundleStarterPack:
		case IapManager.InAppPurchaseItemType.BundleMediumPack:
		case IapManager.InAppPurchaseItemType.BundleBigPack:
		case IapManager.InAppPurchaseItemType.BundleHugePack:
			return "IAP_BUNDLE";
		case IapManager.InAppPurchaseItemType.NightVisionSmall:
		case IapManager.InAppPurchaseItemType.NightVisionMedium:
		case IapManager.InAppPurchaseItemType.NightVisionLarge:
		case IapManager.InAppPurchaseItemType.NightVisionHuge:
		case IapManager.InAppPurchaseItemType.NightVisionUltimate:
		case IapManager.InAppPurchaseItemType.NightVisionSingle:
			return "IAP_NIGHTVISION";
		case IapManager.InAppPurchaseItemType.WoodenLootCrate:
		case IapManager.InAppPurchaseItemType.WoodenLootCrateSale:
			return "IAP_WOODEN_LOOTCRATE";
		default:
			return string.Empty;
		}
	}

	private void ShowConfirmationPopup(GameObject dialogPrefab, string inAppPurchaseItemTypeAsString, string spriteID, string effectID, string itemKey, string descriptionKey, int cost, int itemCount, Func<bool> requirements, Action onClose = null)
	{
		if (dialogPrefab != null && confirmDialog == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(dialogPrefab);
			gameObject.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 5f;
			confirmDialog = gameObject.GetComponent<PurchaseProductConfirmDialog>();
			confirmDialog.Close();
		}
		if (!(confirmDialog != null))
		{
			return;
		}
		confirmDialog.ItemCount = itemCount;
		confirmDialog.ItemSpriteID = spriteID;
		confirmDialog.EffectSpriteID = effectID;
		confirmDialog.Cost = cost;
		confirmDialog.ShowConfirmEnabled = null;
		confirmDialog.ItemLocalizationKey = itemKey;
		confirmDialog.ItemDescriptionKey = descriptionKey;
		confirmDialog.Open();
		bool shopWasOpen = base.gameObject.activeSelf;
		confirmDialog.SetOnConfirm(delegate
		{
			if (requirements())
			{
				PurchaseItem(inAppPurchaseItemTypeAsString);
			}
			else if (Singleton<IapManager>.IsInstantiated())
			{
				Singleton<IapManager>.Instance.OpenShopPage(delegate
				{
					if (shopWasOpen)
					{
						base.gameObject.SetActive(value: true);
					}
					confirmDialog.Open();
				}, "SnoutCoinShop");
				if (shopWasOpen)
				{
					base.gameObject.SetActive(value: false);
				}
				confirmDialog.Close();
			}
			else
			{
				confirmDialog.Close();
			}
		});
		confirmDialog.onClose += delegate
		{
			if (onClose != null)
			{
				onClose();
			}
			if (m_snoutCoinShop == null || !m_snoutCoinShop.gameObject.activeInHierarchy)
			{
				UnityEngine.Object.Destroy(confirmDialog.gameObject);
			}
			else if (!Singleton<IapManager>.IsInstantiated())
			{
				UnityEngine.Object.Destroy(confirmDialog.gameObject);
			}
		};
	}

	private void OnPurchaseItem(IapManager.PurchaseEvent data)
	{
		PurchaseItem(data.itemType.ToString());
	}

	public void PurchaseItem(string inAppPurchaseItemTypeAsString)
	{
		if (!subscribedToPurchaseEvents)
		{
			IapManager.onPurchaseSucceeded += OnPurchaseSucceeded;
			IapManager.onPurchaseSucceeded += UnlockScreen;
			IapManager.onPurchaseFailed += UnlockScreen;
			IapManager.onRestorePurchaseComplete += OnRestorePurchaseComplete;
			subscribedToPurchaseEvents = true;
		}
		if (!Enum.IsDefined(typeof(IapManager.InAppPurchaseItemType), inAppPurchaseItemTypeAsString))
		{
			return;
		}
		IapManager.InAppPurchaseItemType purchaseId = (IapManager.InAppPurchaseItemType)Enum.Parse(typeof(IapManager.InAppPurchaseItemType), inAppPurchaseItemTypeAsString);
		if (purchaseId == IapManager.InAppPurchaseItemType.StarterPack)
		{
			if (starterPackDialog == null)
			{
				if (!(WPFMonoBehaviour.gameData.m_starterPackDialog != null))
				{
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_starterPackDialog);
				gameObject.transform.parent = base.transform.parent;
				gameObject.transform.localPosition = -Vector3.forward * 8f;
				starterPackDialog = gameObject.GetComponent<TextDialog>();
				if (starterPackDialog != null)
				{
					Transform transform = starterPackDialog.transform.Find("CoinCount");
					if (transform != null)
					{
						TextMeshFormattedLocale[] componentsInChildren = transform.GetComponentsInChildren<TextMeshFormattedLocale>();
						if (componentsInChildren != null)
						{
							int purchaseItemTypeCount = Singleton<IapManager>.Instance.GetPurchaseItemTypeCount(IapManager.InAppPurchaseItemType.StarterPack);
							for (int i = 0; i < componentsInChildren.Length; i++)
							{
								componentsInChildren[i].SetText("+{0} {1}", "SNOUT_COIN_PLURAL", purchaseItemTypeCount);
							}
						}
					}
					starterPackDialog.ConfirmButtonText = GetFormattedPrice(purchaseId);
					starterPackDialog.SetOnConfirm(delegate
					{
						LockScreen();
						Singleton<IapManager>.Instance.PurchaseItem(purchaseId);
						starterPackDialog.Close();
					});
				}
				starterPackDialog.Open();
			}
			else
			{
				starterPackDialog.Open();
			}
		}
		else if (string.IsNullOrEmpty(GetFormattedPrice(purchaseId)))
		{
			showFetchError = true;
			Singleton<IapManager>.Instance.FetchPurchasableItemList();
			UpdatePrices();
		}
		else
		{
			LockScreen();
			Singleton<IapManager>.Instance.PurchaseItem(purchaseId);
		}
	}

	public void RestorePurchasedItems()
	{
		Singleton<IapManager>.Instance.RestorePurchasedItems();
		Dialog component = m_confirmRestorePopup.GetComponent<Dialog>();
		if (component != null)
		{
			component.Close();
		}
		GameObject gameObject = GameObject.Find("ConfirmationPopupRestore");
		if (gameObject != null)
		{
			gameObject.GetComponent<Dialog>().Close();
		}
	}

	public void ConfirmRestorePurchasedItems()
	{
		Dialog component = m_confirmRestorePopup.GetComponent<Dialog>();
		if (component != null)
		{
			component.Open();
		}
	}

	private IEnumerator MoveTo(Transform target, Vector3 from, Vector3 to)
	{
		float current = 0f;
		while (target.gameObject.activeInHierarchy && current < 0.15f)
		{
			target.localPosition = Vector3.Lerp(from, to, current / 0.15f);
			yield return null;
			current += Time.unscaledDeltaTime;
		}
		target.localPosition = to;
	}
}
