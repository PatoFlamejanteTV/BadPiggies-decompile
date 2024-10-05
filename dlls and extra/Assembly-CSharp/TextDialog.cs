using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class TextDialog : WPFMonoBehaviour
{
	public delegate void OnClose();

	public delegate void OnOpen();

	public delegate void OnConfirm();

	[Serializable]
	protected struct TextKeyPair
	{
		[SerializeField]
		public string localizationKey;

		[SerializeField]
		public TextMesh textMesh;
	}

	[Serializable]
	protected class LocalizeSprite
	{
		[SerializeField]
		public string spriteId;

		[SerializeField]
		private string localizationId;

		public static string GetLocalizedSprite(LocalizeSprite[] data, string localeId)
		{
			if (data.Length == 0)
			{
				return string.Empty;
			}
			string empty = string.Empty;
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i].localizationId.Equals(localeId))
				{
					return data[i].spriteId;
				}
			}
			if (string.IsNullOrEmpty(empty) && !localeId.Equals("en-EN"))
			{
				return GetLocalizedSprite(data, "en-EN");
			}
			return data[0].localizationId;
		}
	}

	[Serializable]
	protected class SpriteScale
	{
		[SerializeField]
		public string spriteId;

		[SerializeField]
		private Vector2 scale;

		public static bool GetCustomScale(SpriteScale[] data, string spriteId, out Vector2 scale)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i].spriteId.Equals(spriteId))
				{
					scale = data[i].scale;
					return true;
				}
			}
			scale = Vector2.zero;
			return false;
		}
	}

	private const string APPEAR_ANIMATION_NAME = "PopUp";

	[SerializeField]
	protected int maxCharactersInLine;

	[SerializeField]
	protected int maxKanjiCharacterInLine;

	[SerializeField]
	protected TextKeyPair[] texts;

	[SerializeField]
	private GameObject enabledConfirmButton;

	[SerializeField]
	private GameObject disabledConfirmButton;

	[SerializeField]
	private bool positionSoftCurrencyButton;

	[SerializeField]
	private SoftCurrencyButton.Position softCurrencyPosition;

	private SkeletonAnimation appearAnimation;

	private TextMesh[] enabledConfirmButtonTxt;

	private TextMesh[] disabledConfirmButtonTxt;

	private Func<bool> showConfirmEnabled;

	private List<Renderer> activeRenderers;

	private List<Collider> activeColliders;

	private bool hidden;

	protected GameObject dialogRoot;

	protected bool isOpened;

	protected static bool s_dialogOpen;

	public Func<bool> ShowConfirmEnabled
	{
		get
		{
			return showConfirmEnabled;
		}
		set
		{
			showConfirmEnabled = value;
		}
	}

	public string ConfirmButtonText
	{
		get
		{
			if (enabledConfirmButtonTxt != null && enabledConfirmButtonTxt.Length != 0)
			{
				return enabledConfirmButtonTxt[0].text;
			}
			return string.Empty;
		}
		set
		{
			if (enabledConfirmButtonTxt != null)
			{
				for (int i = 0; i < enabledConfirmButtonTxt.Length; i++)
				{
					enabledConfirmButtonTxt[i].text = value;
					enabledConfirmButtonTxt[i].SendMessage("TextUpdated", SendMessageOptions.DontRequireReceiver);
				}
			}
			if (disabledConfirmButtonTxt != null)
			{
				for (int j = 0; j < disabledConfirmButtonTxt.Length; j++)
				{
					disabledConfirmButtonTxt[j].text = value;
					disabledConfirmButtonTxt[j].SendMessage("TextUpdated", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public static bool DialogOpen => s_dialogOpen;

	public bool IsOpen => isOpened;

	public event OnClose onClose;

	public event OnOpen onOpen;

	private event OnConfirm onConfirm;

	public event OnOpen onShopPageOpened;

	protected virtual void Awake()
	{
		if ((bool)enabledConfirmButton)
		{
			enabledConfirmButtonTxt = enabledConfirmButton.GetComponentsInChildren<TextMesh>();
			enabledConfirmButton.SetActive(value: true);
		}
		if ((bool)disabledConfirmButton)
		{
			disabledConfirmButtonTxt = disabledConfirmButton.GetComponentsInChildren<TextMesh>();
			disabledConfirmButton.SetActive(value: false);
		}
		hidden = false;
		appearAnimation = GetComponent<SkeletonAnimation>();
		Transform transform = base.transform.Find("SkeletonUtility-Root/root/BN_Shake");
		if ((bool)transform)
		{
			dialogRoot = transform.gameObject;
		}
	}

	protected virtual void Start()
	{
		for (int i = 0; i < texts.Length; i++)
		{
			texts[i].textMesh.text = texts[i].localizationKey;
			TextMeshLocale component = texts[i].textMesh.gameObject.GetComponent<TextMeshLocale>();
			if (component != null)
			{
				component.RefreshTranslation();
				component.enabled = false;
				TextMeshHelper.Wrap(texts[i].textMesh, (!TextMeshHelper.UsesKanjiCharacters()) ? maxCharactersInLine : maxKanjiCharacterInLine);
			}
		}
	}

	public virtual void Open()
	{
		if (!isOpened)
		{
			isOpened = true;
			base.gameObject.SetActive(value: true);
			UpdateTextMeshSpriteIcons();
			PlayAppearAnimation();
			if (this.onOpen != null)
			{
				this.onOpen();
			}
		}
	}

	public virtual void Close()
	{
		isOpened = false;
		base.gameObject.SetActive(value: false);
		if (this.onClose != null)
		{
			this.onClose();
		}
	}

	public void Confirm()
	{
		if (this.onConfirm != null)
		{
			this.onConfirm();
		}
	}

	public void OpenShop()
	{
		if (Singleton<IapManager>.IsInstantiated())
		{
			Singleton<IapManager>.Instance.OpenShopPage(EnableConfirmButton, "SnoutCoinShop");
		}
	}

	public void OpenShopPageAndClose(string shopPage)
	{
		if (Singleton<IapManager>.IsInstantiated())
		{
			MainMenu mainMenu = UnityEngine.Object.FindObjectOfType<MainMenu>();
			if (string.IsNullOrEmpty(shopPage) && mainMenu != null)
			{
				mainMenu.OpenShop();
			}
			else
			{
				Singleton<IapManager>.Instance.OpenShopPage(null, shopPage);
			}
			if (this.onShopPageOpened != null)
			{
				this.onShopPageOpened();
			}
		}
		Close();
	}

	public void SetOnConfirm(OnConfirm onConfirm)
	{
		this.onConfirm = onConfirm;
	}

	protected void EnableConfirmButton()
	{
		if (showConfirmEnabled != null)
		{
			EnableConfirmButton(showConfirmEnabled());
		}
	}

	protected void EnableConfirmButton(bool enable)
	{
		if ((bool)enabledConfirmButton && (bool)disabledConfirmButton)
		{
			enabledConfirmButton.SetActive(enable);
			disabledConfirmButton.SetActive(!enable);
		}
	}

	protected virtual void OnEnable()
	{
		isOpened = true;
		Singleton<GuiManager>.Instance.GrabPointer(this);
		Singleton<KeyListener>.Instance.GrabFocus(this);
		KeyListener.keyReleased += HandleKeyReleased;
		EnableConfirmButton();
		s_dialogOpen = true;
	}

	protected virtual void OnDisable()
	{
		isOpened = false;
		if (Singleton<GuiManager>.IsInstantiated())
		{
			Singleton<GuiManager>.Instance.ReleasePointer(this);
		}
		if (Singleton<KeyListener>.IsInstantiated())
		{
			Singleton<KeyListener>.Instance.ReleaseFocus(this);
		}
		KeyListener.keyReleased -= HandleKeyReleased;
		s_dialogOpen = false;
	}

	protected void HandleKeyReleased(KeyCode key)
	{
		if (key == KeyCode.Escape)
		{
			Close();
		}
	}

	protected void PlayAppearAnimation()
	{
		if (!(appearAnimation != null) || !(dialogRoot != null))
		{
			return;
		}
		List<Renderer> renderers = GetActiveComponents<Renderer>();
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].enabled = false;
		}
		if (appearAnimation.state == null)
		{
			appearAnimation.Initialize(overwrite: true);
		}
		appearAnimation.state.ClearTracks();
		appearAnimation.state.SetAnimation(0, "PopUp", loop: false);
		StartCoroutine(CoroutineRunner.DelayFrames(delegate
		{
			for (int j = 0; j < renderers.Count; j++)
			{
				if (renderers[j] != null)
				{
					renderers[j].enabled = true;
				}
			}
		}, 1));
	}

	protected void Hide()
	{
		if (!hidden)
		{
			activeColliders = GetActiveComponents<Collider>();
			activeRenderers = GetActiveComponents<Renderer>();
			for (int i = 0; i < activeColliders.Count; i++)
			{
				activeColliders[i].enabled = false;
			}
			for (int j = 0; j < activeRenderers.Count; j++)
			{
				activeRenderers[j].enabled = false;
			}
			hidden = true;
		}
	}

	protected void Show()
	{
		if (activeColliders != null)
		{
			_ = activeRenderers;
		}
		for (int i = 0; i < activeColliders.Count; i++)
		{
			activeColliders[i].enabled = true;
		}
		for (int j = 0; j < activeRenderers.Count; j++)
		{
			activeRenderers[j].enabled = true;
		}
		hidden = false;
	}

	public void UpdateTextMeshSpriteIcons()
	{
		TextMesh[] componentsInChildren = base.gameObject.GetComponentsInChildren<TextMesh>(includeInactive: true);
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren[i]);
			}
		}
	}
}
