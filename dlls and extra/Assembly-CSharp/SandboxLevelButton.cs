using System;
using UnityEngine;

public class SandboxLevelButton : MonoBehaviour
{
	public string m_sandboxIdentifier;

	[SerializeField]
	private SandboxSelector m_sandboxSelector;

	[SerializeField]
	private TextMesh m_starsText;

	[SerializeField]
	public bool isBigSandboxButton;

	[SerializeField]
	private GameObject[] hideOnContentLocked;

	[SerializeField]
	private GameObject[] hideOnContentUnlocked;

	private Action onOpenUnlockDialog;

	private Transform starSet;

	private GameObject episodeSelector;

	private static SandboxUnlockDialog sandboxUnlockDialog;

	private Button setTooltipButton;

	private TextMesh[] priceLabels;

	private void OnEnable()
	{
		if (m_sandboxSelector != null && m_sandboxIdentifier.Equals("S-F") && GameProgress.GetSandboxUnlocked(m_sandboxIdentifier))
		{
			UnlockSandboxSequence component = GetComponent<UnlockSandboxSequence>();
			GameProgress.ButtonUnlockState buttonUnlockState = GameProgress.GetButtonUnlockState("SandboxLevelButton_" + m_sandboxIdentifier);
			if (component == null && buttonUnlockState == GameProgress.ButtonUnlockState.Locked)
			{
				base.gameObject.AddComponent<UnlockSandboxSequence>();
			}
			GetComponent<Button>().MethodToCall.SetMethod(m_sandboxSelector, "LoadSandboxLevel", m_sandboxIdentifier);
			string text = WPFMonoBehaviour.gameData.m_sandboxLevels.GetLevelData(m_sandboxIdentifier).m_starBoxCount.ToString();
			m_starsText.text = GameProgress.SandboxStarCount(m_sandboxSelector.FindLevelFile(m_sandboxIdentifier)) + "/" + text;
			for (int i = 0; i < hideOnContentUnlocked.Length; i++)
			{
				hideOnContentUnlocked[i].SetActive(value: false);
			}
			for (int j = 0; j < hideOnContentLocked.Length; j++)
			{
				hideOnContentLocked[j].SetActive(value: true);
			}
		}
	}

	private void Awake()
	{
		if (!m_sandboxIdentifier.Equals("S-M"))
		{
			return;
		}
		Transform transform = base.transform.Find("Price");
		if (!(transform != null))
		{
			return;
		}
		priceLabels = transform.GetComponentsInChildren<TextMesh>();
		if (priceLabels != null && priceLabels.Length != 0)
		{
			for (int i = 0; i < priceLabels.Length; i++)
			{
				priceLabels[i].text = string.Empty;
			}
		}
	}

	private void Start()
	{
		if (isBigSandboxButton && base.transform.parent != null)
		{
			m_sandboxSelector = base.transform.parent.GetComponent<SandboxSelector>();
		}
		if (m_sandboxSelector == null && base.transform.parent != null && base.transform.parent.parent != null)
		{
			m_sandboxSelector = base.transform.parent.parent.GetComponent<SandboxSelector>();
		}
		starSet = base.transform.Find("StarSet");
		if (starSet != null && !isBigSandboxButton)
		{
			starSet.parent = base.transform.parent;
		}
		UnlockSandboxSequence component = GetComponent<UnlockSandboxSequence>();
		bool flag = GameProgress.GetSandboxUnlocked(m_sandboxIdentifier);
		bool isOdyssey = Singleton<BuildCustomizationLoader>.Instance.IsOdyssey;
		if (isOdyssey && (m_sandboxIdentifier.Equals("S-M") || m_sandboxIdentifier.Equals("S-F")))
		{
			flag = true;
		}
		if (!flag && m_sandboxIdentifier.Equals("S-M"))
		{
			Debug.LogError("SandboxLevelButton S-M");
			int cost = GetUnlockPrice(m_sandboxIdentifier);
			AddSMSandboxUnlockDialog(GetComponent<Button>(), cost, m_sandboxIdentifier, () => GameProgress.SnoutCoinCount() >= cost);
		}
		else if (!flag && m_sandboxIdentifier.Equals("S-F"))
		{
			AddBuyFieldOfDreamsButton(GetComponent<Button>());
		}
		else if (!flag && !isOdyssey)
		{
			if (sandboxUnlockDialog == null)
			{
				GameObject obj = UnityEngine.Object.Instantiate(Singleton<GameManager>.Instance.gameData.m_sandboxUnlockDialog);
				obj.transform.position = new Vector3(0f, 0f, -10f);
				sandboxUnlockDialog = obj.GetComponent<SandboxUnlockDialog>();
				sandboxUnlockDialog.Close();
			}
			int cost = GetUnlockPrice(m_sandboxIdentifier);
			AddSandboxUnlockDialog(GetComponent<Button>(), sandboxUnlockDialog, m_sandboxIdentifier, cost, () => GameProgress.SnoutCoinCount() >= cost);
		}
		else if (!flag && isOdyssey)
		{
			TooltipInfo component2 = GetComponent<TooltipInfo>();
			if (component2 != null)
			{
				setTooltipButton = GetComponent<Button>();
				setTooltipButton.MethodToCall.SetMethod(component2, "Show");
			}
		}
		GameProgress.ButtonUnlockState buttonUnlockState = GameProgress.GetButtonUnlockState("SandboxLevelButton_" + m_sandboxIdentifier);
		if (flag)
		{
			if (component == null && buttonUnlockState == GameProgress.ButtonUnlockState.Locked)
			{
				base.gameObject.AddComponent<UnlockSandboxSequence>();
			}
			GetComponent<Button>().MethodToCall.SetMethod(m_sandboxSelector, "LoadSandboxLevel", m_sandboxIdentifier);
			string text = WPFMonoBehaviour.gameData.m_sandboxLevels.GetLevelData(m_sandboxIdentifier).m_starBoxCount.ToString();
			m_starsText.text = GameProgress.SandboxStarCount(m_sandboxSelector.FindLevelFile(m_sandboxIdentifier)) + "/" + text;
			for (int i = 0; i < hideOnContentUnlocked.Length; i++)
			{
				hideOnContentUnlocked[i].SetActive(value: false);
			}
		}
		else if (!flag && m_starsText != null)
		{
			string arg = WPFMonoBehaviour.gameData.m_sandboxLevels.GetLevelData(m_sandboxIdentifier).m_starBoxCount.ToString();
			m_starsText.text = $"{0}/{arg}";
			for (int j = 0; j < hideOnContentLocked.Length; j++)
			{
				hideOnContentLocked[j].SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		if (setTooltipButton != null)
		{
			setTooltipButton.Lock(lockState: false);
		}
	}

	private void AddSMSandboxUnlockDialog(Button button, int cost, string levelIdentifier, Func<bool> requirements)
	{
		GameData gameData = Singleton<GameManager>.Instance.gameData;
		if (gameData.m_lpaUnlockDialog != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gameData.m_lpaUnlockDialog);
			TextDialog dialog = gameObject.GetComponent<TextDialog>();
			gameObject.transform.position = new Vector3(0f, 0f, -10f);
			button.MethodToCall.SetMethod(dialog, "Open");
			dialog.ConfirmButtonText = $"[snout] {cost}";
			dialog.Close();
			dialog.ShowConfirmEnabled = () => true;
			dialog.SetOnConfirm(delegate
			{
				if (!GameProgress.GetSandboxUnlocked(levelIdentifier) && requirements() && GameProgress.UseSnoutCoins(cost))
				{
					GameProgress.SetSandboxUnlocked(levelIdentifier, unlocked: true);
					GameProgress.SetButtonUnlockState("SandboxLevelButton_" + levelIdentifier, GameProgress.ButtonUnlockState.Locked);
					Singleton<GameManager>.Instance.ReloadCurrentLevel(showLoadingScreen: true);
					ReportUnlockSandbox(cost, levelIdentifier);
					UnityEngine.Object.DontDestroyOnLoad(Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse));
				}
				else if (!requirements() && Singleton<IapManager>.IsInstantiated())
				{
					dialog.Close();
					Singleton<IapManager>.Instance.OpenShopPage(dialog.Open, "SnoutCoinShop");
				}
				else
				{
					dialog.Close();
				}
			});
		}
		if (priceLabels != null && priceLabels.Length != 0)
		{
			string text = $"[snout] {cost}";
			for (int i = 0; i < priceLabels.Length; i++)
			{
				priceLabels[i].text = text;
				TextMeshSpriteIcons.EnsureSpriteIcon(priceLabels[i]);
			}
		}
	}

	private void AddSandboxUnlockDialog(Button button, SandboxUnlockDialog dialog, string levelIdentifier, int price, Func<bool> requirements)
	{
		button.MethodToCall.SetMethod(this, "OpenSandboxUnlockDialog");
		onOpenUnlockDialog = delegate
		{
			dialog.SandboxIdentifier = levelIdentifier;
			dialog.Cost = price;
			dialog.ShowConfirmEnabled = requirements;
			dialog.SetOnConfirm(delegate
			{
				if (!GameProgress.GetSandboxUnlocked(levelIdentifier) && requirements() && GameProgress.UseSnoutCoins(price))
				{
					GameProgress.SetSandboxUnlocked(levelIdentifier, unlocked: true);
					GameProgress.SetButtonUnlockState("SandboxLevelButton_" + levelIdentifier, GameProgress.ButtonUnlockState.Locked);
					Singleton<GameManager>.Instance.ReloadCurrentLevel(showLoadingScreen: true);
					ReportUnlockSandbox(price, levelIdentifier);
					EventManager.Connect<LevelLoadedEvent>(DelayedPurchaseSound);
				}
				else if (!requirements() && Singleton<IapManager>.IsInstantiated())
				{
					dialog.Close();
					Singleton<IapManager>.Instance.OpenShopPage(dialog.Open, "SnoutCoinShop");
				}
				else
				{
					dialog.Close();
				}
			});
		};
	}

	private void DelayedPurchaseSound(LevelLoadedEvent data)
	{
		EventManager.Disconnect<LevelLoadedEvent>(DelayedPurchaseSound);
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
	}

	private void ReportUnlockSandbox(int cost, string levelIdentifier)
	{
	}

	private void AddBuyFieldOfDreamsButton(Button button)
	{
		episodeSelector = GameObject.Find("EpisodeSelector");
		button.MethodToCall.SetMethod(this, "OpenFieldOfDreamsShopPage");
		button.Lock(lockState: false);
	}

	public void OpenSandboxUnlockDialog()
	{
		if (onOpenUnlockDialog != null)
		{
			onOpenUnlockDialog();
		}
		if (sandboxUnlockDialog != null)
		{
			sandboxUnlockDialog.Open();
		}
	}

	public void OpenFieldOfDreamsShopPage()
	{
		if (Singleton<IapManager>.Instance != null)
		{
			episodeSelector.SetActive(value: false);
			Singleton<IapManager>.Instance.OpenShopPage(delegate
			{
				episodeSelector.SetActive(value: true);
			}, "FieldOfDreams");
		}
	}

	private int GetUnlockPrice(string identifier)
	{
		if (Singleton<IapManager>.Instance == null)
		{
			return int.MaxValue;
		}
		switch (identifier)
		{
		case "S-1":
		case "S-3":
		case "S-5":
		case "S-7":
		case "S-9":
			return Singleton<VirtualCatalogManager>.Instance.GetProductPrice("sandbox_normal_first_unlock");
		case "S-2":
		case "S-4":
		case "S-6":
		case "S-8":
		case "S-10":
			return Singleton<VirtualCatalogManager>.Instance.GetProductPrice("sandbox_normal_second_unlock");
		case "S-M":
			return Singleton<VirtualCatalogManager>.Instance.GetProductPrice("sandbox_lpa_unlock");
		default:
			return int.MaxValue;
		}
	}
}
