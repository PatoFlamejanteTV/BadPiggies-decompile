using UnityEngine;

public class PurchaseProductConfirmDialog : TextDialog
{
	[SerializeField]
	private SpriteScale[] customScales;

	[SerializeField]
	private GameObject description;

	[SerializeField]
	private SpriteText countText;

	[SerializeField]
	private Sprite itemIcon;

	[SerializeField]
	private Transform itemCountTf;

	[SerializeField]
	private GameObject costText;

	[SerializeField]
	private GameObject productName;

	private string itemSpriteID;

	private string effectSpriteID;

	private string itemLocalizationKey;

	private string itemDescriptionKey;

	private int itemCount;

	private int cost;

	private Sprite buttonBackground;

	private Sprite disabledButtonBackground;

	private Vector2 defaultScale;

	public string ItemSpriteID
	{
		get
		{
			return itemSpriteID;
		}
		set
		{
			itemSpriteID = value;
		}
	}

	public string EffectSpriteID
	{
		get
		{
			return effectSpriteID;
		}
		set
		{
			effectSpriteID = value;
		}
	}

	public string ItemLocalizationKey
	{
		get
		{
			return itemLocalizationKey;
		}
		set
		{
			itemLocalizationKey = value;
		}
	}

	public string ItemDescriptionKey
	{
		get
		{
			return itemDescriptionKey;
		}
		set
		{
			itemDescriptionKey = value;
		}
	}

	public int ItemCount
	{
		get
		{
			return itemCount;
		}
		set
		{
			itemCount = value;
		}
	}

	public int Cost
	{
		get
		{
			return cost;
		}
		set
		{
			cost = value;
		}
	}

	protected override void Awake()
	{
		defaultScale = new Vector2(itemIcon.m_scaleX, itemIcon.m_scaleY);
		if (WPFMonoBehaviour.levelManager != null)
		{
			WPFMonoBehaviour.levelManager.ConstructionUI.DisableFunctionality = true;
		}
		base.Awake();
	}

	protected virtual void OnDestroy()
	{
		if (WPFMonoBehaviour.levelManager != null)
		{
			WPFMonoBehaviour.levelManager.ConstructionUI.DisableFunctionality = false;
		}
	}

	protected override void Start()
	{
		RebuildIcons();
		RebuildTexts();
	}

	public new void Open()
	{
		base.Open();
		RebuildIcons();
		RebuildTexts();
		EventManager.Send(new UIEvent(UIEvent.Type.OpenedPurchaseConfirmation));
	}

	public new void Close()
	{
		base.Close();
		EventManager.Send(new UIEvent(UIEvent.Type.ClosedPurchaseConfirmation));
	}

	private new void HandleKeyReleased(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			Close();
		}
	}

	public new void Confirm()
	{
		base.Confirm();
		Close();
	}

	public void OpenSnoutCoinPopup()
	{
		if (Singleton<IapManager>.Instance != null)
		{
			Singleton<IapManager>.Instance.OpenShopPage(null, "SnoutCoinShop");
		}
	}

	public void RebuildIcons()
	{
		if (Singleton<RuntimeSpriteDatabase>.Instance != null)
		{
			if (SpriteScale.GetCustomScale(customScales, itemSpriteID, out var scale))
			{
				itemIcon.m_scaleX = scale.x;
				itemIcon.m_scaleY = scale.y;
			}
			else
			{
				itemIcon.m_scaleX = defaultScale.x;
				itemIcon.m_scaleY = defaultScale.y;
			}
			SpriteData spriteData = Singleton<RuntimeSpriteDatabase>.Instance.Find(itemSpriteID);
			if (spriteData != null)
			{
				itemIcon.SelectSprite(spriteData, forceResetMesh: true);
			}
		}
	}

	public void RebuildTexts()
	{
		if (itemCountTf != null && countText != null)
		{
			string text = ((itemCount <= 0) ? string.Empty : $"x{itemCount}");
			countText.Text = text;
		}
		RefreshTexts(costText, $"[snout] {cost}", updateLocale: false, updateSprites: true);
		RefreshTexts(productName, itemLocalizationKey, updateLocale: true, updateSprites: false);
		RefreshTexts(description, itemDescriptionKey, updateLocale: true, updateSprites: false);
		EnableConfirmButton(cost > 0);
	}

	private void RefreshTexts(GameObject target, string text, bool updateLocale, bool updateSprites)
	{
		TextMesh[] componentsInChildren = target.GetComponentsInChildren<TextMesh>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].text = text;
			if (updateLocale)
			{
				TextMeshLocale textMeshLocale = componentsInChildren[i].GetComponent<TextMeshLocale>();
				if (textMeshLocale == null)
				{
					textMeshLocale = componentsInChildren[i].gameObject.AddComponent<TextMeshLocale>();
				}
				textMeshLocale.RefreshTranslation();
				textMeshLocale.enabled = false;
			}
			if (updateSprites)
			{
				TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren[i]);
			}
		}
	}
}
