using System;
using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;

public class SnoutCoinShopPopup : MonoBehaviour
{
	[SerializeField]
	private GameObject m_bestValueRibbon;

	[SerializeField]
	private GameObject m_mostPopularRibbon;

	[SerializeField]
	private GameObject m_offerBanner;

	[SerializeField]
	private SkeletonAnimation m_ultimatePackAnimation;

	[SerializeField]
	private SkeletonAnimation m_hugePackAnimation;

	[SerializeField]
	private SkeletonAnimation m_largePackAnimation;

	[SerializeField]
	private SkeletonAnimation m_mediumPackAnimation;

	[SerializeField]
	private SkeletonAnimation m_smallPackAnimation;

	private bool m_playingAnimation;

	private static bool s_dialogOpen;

	private Shop shop;

	private Action OnClose;

	public GameObject BestValueRibbon => m_bestValueRibbon;

	public GameObject MostPopularRibbon => m_mostPopularRibbon;

	public static bool DialogOpen => s_dialogOpen;

	private void Start()
	{
		m_playingAnimation = false;
		m_hugePackAnimation.timeScale = float.MaxValue;
		m_hugePackAnimation.AnimationState.AddAnimation(0, "SafeIdle", loop: false, 0f);
		m_hugePackAnimation.AnimationState.End += OnAnimationEnd;
		m_largePackAnimation.timeScale = float.MaxValue;
		m_largePackAnimation.AnimationState.AddAnimation(0, "BarrelIdle", loop: false, 0f);
		m_largePackAnimation.AnimationState.End += OnAnimationEnd;
		m_mediumPackAnimation.timeScale = float.MaxValue;
		m_mediumPackAnimation.AnimationState.AddAnimation(0, "CoinPillar_Idle", loop: false, 0f);
		m_mediumPackAnimation.AnimationState.End += OnAnimationEnd;
		m_smallPackAnimation.timeScale = float.MaxValue;
		m_smallPackAnimation.AnimationState.AddAnimation(0, "PiggieBankIdle", loop: false, 0f);
		m_smallPackAnimation.AnimationState.End += OnAnimationEnd;
		m_ultimatePackAnimation.AnimationState.AddAnimation(0, "Shower_Idle", loop: true, 0f);
		StartCoroutine(PlayRandomAnimation(2f));
	}

	private void OnEnable()
	{
		m_bestValueRibbon.SetActive(value: false);
		m_mostPopularRibbon.SetActive(value: false);
		ShowOfferBanner(show: false);
		UpdatePrices();
		s_dialogOpen = true;
		EventManager.Send(new UIEvent(UIEvent.Type.OpenedSnoutCoinShop));
	}

	private void OnDisable()
	{
		s_dialogOpen = false;
		EventManager.Send(new UIEvent(UIEvent.Type.ClosedSnoutCoinShop));
	}

	private void OnDestroy()
	{
		m_hugePackAnimation.AnimationState.End -= OnAnimationEnd;
		m_largePackAnimation.AnimationState.End -= OnAnimationEnd;
		m_mediumPackAnimation.AnimationState.End -= OnAnimationEnd;
		m_smallPackAnimation.AnimationState.End -= OnAnimationEnd;
	}

	public void ShowOfferBanner(bool show)
	{
		if (m_offerBanner != null)
		{
			m_offerBanner.SetActive(show);
		}
	}

	public GameObject AddRibbon(ShopRibbon ribbon)
	{
		Transform transform = base.transform.Find("Items/" + ribbon.itemId);
		if (!transform)
		{
			return null;
		}
		string text = string.Empty;
		GameObject original = null;
		switch (ribbon.ribbonType)
		{
		case ShopRibbon.Ribbon.MostPopular:
			text = "MostPopularRibbon";
			original = m_mostPopularRibbon;
			break;
		case ShopRibbon.Ribbon.BestValue:
			text = "BestValueRibbon";
			original = m_bestValueRibbon;
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		Transform transform2 = transform.Find(text);
		GameObject gameObject = null;
		if (transform2 != null)
		{
			gameObject = UnityEngine.Object.Instantiate(original);
			gameObject.name = text;
			gameObject.transform.parent = transform2;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetActive(value: true);
		}
		return gameObject;
	}

	public void UpdatePrices(Shop _shop = null)
	{
		if (_shop != null)
		{
			shop = _shop;
		}
		if (shop == null)
		{
			return;
		}
		bool show = false;
		PurchaseInfo[] componentsInChildren = base.gameObject.GetComponentsInChildren<PurchaseInfo>();
		foreach (PurchaseInfo purchaseInfo in componentsInChildren)
		{
			Button component = purchaseInfo.GetComponent<Button>();
			if (component != null)
			{
				if (purchaseInfo.purchaseItem == IapManager.InAppPurchaseItemType.StarterPack)
				{
					component.MethodToCall.SetMethod(shop, "PurchaseItem");
				}
				else
				{
					component.MethodToCall.SetMethod(purchaseInfo, "Purchase");
				}
			}
			purchaseInfo.CheckOnSale();
			string text = ((!purchaseInfo.isSaleItem) ? null : shop.GetFormattedPrice(purchaseInfo.saleItem));
			if (!string.IsNullOrEmpty(text))
			{
				show = true;
			}
			shop.SetPriceIndicator(purchaseInfo, shop.GetFormattedPrice(purchaseInfo.purchaseItem), text);
		}
		ShowOfferBanner(show);
	}

	public void Open(Action onClose)
	{
		Singleton<GuiManager>.Instance.GrabPointer(this);
		Singleton<KeyListener>.Instance.GrabFocus(this);
		KeyListener.keyReleased += HandleKeyReleased;
		OnClose = onClose;
		base.gameObject.SetActive(value: true);
		ResourceBar.Instance.ShowItem(ResourceBar.Item.SnoutCoin, showItem: true, enableItem: false);
		DoubleRewardIcon.Instance.SetSortingLayer("Default");
	}

	public void Close()
	{
		Singleton<GuiManager>.Instance.ReleasePointer(this);
		Singleton<KeyListener>.Instance.ReleaseFocus(this);
		KeyListener.keyReleased -= HandleKeyReleased;
		if (OnClose != null)
		{
			OnClose();
		}
		base.gameObject.SetActive(value: false);
		if (SnoutButton.Instance != null)
		{
			SnoutButton.Instance.EnableButton(enable: true);
		}
		if (DoubleRewardIcon.Instance != null)
		{
			DoubleRewardIcon.Instance.SetSortingLayer("Popup");
		}
	}

	private void HandleKeyReleased(KeyCode key)
	{
		if (key == KeyCode.Escape)
		{
			Close();
		}
	}

	private IEnumerator PlayRandomAnimation(float interval)
	{
		float counter = 0f;
		while (base.gameObject.activeInHierarchy)
		{
			if (counter > interval)
			{
				counter = 0f;
				m_playingAnimation = true;
				PlayRandomAnimation();
			}
			else if (!m_playingAnimation)
			{
				counter += Time.deltaTime;
			}
			yield return null;
		}
	}

	private void PlayRandomAnimation()
	{
		switch (UnityEngine.Random.Range(0, 4))
		{
		default:
			m_hugePackAnimation.timeScale = 1f;
			m_hugePackAnimation.AnimationState.AddAnimation(0, "SafeIdle", loop: false, 0f);
			break;
		case 1:
			m_largePackAnimation.timeScale = 1f;
			m_largePackAnimation.AnimationState.AddAnimation(0, "BarrelIdle", loop: false, 0f);
			break;
		case 2:
			m_mediumPackAnimation.timeScale = 1f;
			m_mediumPackAnimation.AnimationState.AddAnimation(0, "CoinPillar_Idle", loop: false, 0f);
			break;
		case 3:
			m_smallPackAnimation.timeScale = 1f;
			m_smallPackAnimation.AnimationState.AddAnimation(0, "PiggieBankIdle", loop: false, 0f);
			break;
		}
	}

	private void OnAnimationEnd(Spine.AnimationState state, int trackIndex)
	{
		m_playingAnimation = false;
	}
}
