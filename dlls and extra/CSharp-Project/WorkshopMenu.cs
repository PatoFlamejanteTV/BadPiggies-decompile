using System;
using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;

public class WorkshopMenu : MonoBehaviour
{
	public enum CraftingMachineAction
	{
		None,
		Idle,
		ResetScrap,
		AddScrap,
		RemoveScrap,
		CraftPart
	}

	public struct CraftingMachineEvent : EventManager.Event
	{
		public CraftingMachineAction action;

		public int scrapAmountInMachine;

		public CraftingMachineEvent(CraftingMachineAction action, int scrapAmountInMachine = 0)
		{
			this.action = action;
			this.scrapAmountInMachine = scrapAmountInMachine;
		}
	}

	public static string CRAFT_PRICE_CONFIG_KEY = "part_crafting_prices";

	public static string SALVAGE_PRICE_CONFIG_KEY = "part_salvage_rewards";

	public static bool isDestroyed = true;

	[SerializeField]
	private SpriteText normalMachineLabel;

	[SerializeField]
	private SpriteText alienMachineLabel;

	[SerializeField]
	private Transform machineArrow;

	[SerializeField]
	private GameObject collectRewardButton;

	[SerializeField]
	private GameObject[] lootRewardBackgrounds;

	[SerializeField]
	private GameObject checkMark;

	[SerializeField]
	private GameObject rewardSpawnRoot;

	[SerializeField]
	private GameObject[] hideObjects;

	[SerializeField]
	private SkeletonAnimation machineAnimation;

	[SerializeField]
	private string introAnimationName;

	[SerializeField]
	private string hideRewardAnimationName;

	[SerializeField]
	private string meterFillAnimationName;

	[SerializeField]
	private string resetAnimationName;

	[SerializeField]
	private string craftAnimationName;

	[SerializeField]
	private string slimeCraftAnimationName;

	[SerializeField]
	private string chainPullAnimationName;

	[SerializeField]
	private string insufficientScrapAnimationName;

	[SerializeField]
	private string[] feedAnimationNames;

	[SerializeField]
	private string[] idleAnimationNames;

	[SerializeField]
	private GameObject[] nutPrefabs;

	[SerializeField]
	private Transform nutRootUpper;

	[SerializeField]
	private Transform nutRootLower;

	[SerializeField]
	private PullButton pullChainButton;

	[SerializeField]
	private PullButton pullLeverButton;

	[SerializeField]
	private PartListing partListing;

	[SerializeField]
	private ParticleSystem machineDropEffect;

	[SerializeField]
	private ParticleSystem machineCraftEmptyEffect;

	[SerializeField]
	private ParticleSystem machineSmokePuffEffect;

	[SerializeField]
	private CustomizationsFullCheck customizationsFullCheck;

	private CustomizePartWidget partListingButton;

	[SerializeField]
	private AlienCraftingMachineConverter alienConverter;

	[SerializeField]
	private Transform liquidFillOverride;

	[SerializeField]
	private float maxFill;

	[SerializeField]
	private CustomShaderSprite nextAlienPartIcon;

	[SerializeField]
	private string alienSkinName;

	private GameData gameData;

	private bool machineIsLocked = true;

	private bool introPlaying = true;

	private string waitingAnimationToEnd = string.Empty;

	private float machineArrowFromAngle;

	private float machineArrowTargetAngle;

	private float machineArrowStutterStrength = 1f;

	private GameObject rewardGameObject;

	private int queuedAddScrapActions;

	private GetMoreScrapDialog getMoreScrapDialog;

	private int commonPrice;

	private int rarePrice;

	private int epicPrice;

	private int targetMachineScrapAmount;

	private int currentMachineScrapAmount;

	protected float nextLabelUpdate;

	private GameObject machineIdleLoop;

	private int partsCraftedWhileInScreen;

	private float arrowAngleFade;

	public static bool FirstLootCrateCollected
	{
		get
		{
			return GameProgress.GetBool("CollectedFirstLootCrate");
		}
		set
		{
			GameProgress.SetBool("CollectedFirstLootCrate", value);
		}
	}

	public static bool AnyLootCrateCollected
	{
		get
		{
			return GameProgress.GetBool("AnyLootCrateCollected");
		}
		set
		{
			GameProgress.SetBool("AnyLootCrateCollected", value);
		}
	}

	private bool IsMachineLocked
	{
		get
		{
			if (!machineIsLocked)
			{
				return introPlaying;
			}
			return true;
		}
	}

	private SpriteText MachineLabel
	{
		get
		{
			if (IsAlienMachine)
			{
				return alienMachineLabel;
			}
			return normalMachineLabel;
		}
	}

	private bool IsAlienMachine => alienConverter.IsAlienMachine;

	private void Awake()
	{
		gameData = WPFMonoBehaviour.gameData;
		AlienCraftingMachineConverter alienCraftingMachineConverter = alienConverter;
		alienCraftingMachineConverter.OnBeginUpgrade = (Action)Delegate.Combine(alienCraftingMachineConverter.OnBeginUpgrade, new Action(OnUpgradeMachineBegin));
		AlienCraftingMachineConverter alienCraftingMachineConverter2 = alienConverter;
		alienCraftingMachineConverter2.OnMachineBehindCurtain = (Action)Delegate.Combine(alienCraftingMachineConverter2.OnMachineBehindCurtain, new Action(OnMachineBehindCurtain));
		AlienCraftingMachineConverter alienCraftingMachineConverter3 = alienConverter;
		alienCraftingMachineConverter3.OnEndUpgrade = (Action)Delegate.Combine(alienCraftingMachineConverter3.OnEndUpgrade, new Action(OnUpgradeMachineEnd));
		Transform transform = base.transform.Find("LowerRightButtons/PartList");
		if (transform != null)
		{
			partListingButton = transform.GetComponent<CustomizePartWidget>();
		}
		commonPrice = Singleton<GameConfigurationManager>.Instance.GetValue<int>(CRAFT_PRICE_CONFIG_KEY, BasePart.PartTier.Common.ToString());
		rarePrice = Singleton<GameConfigurationManager>.Instance.GetValue<int>(CRAFT_PRICE_CONFIG_KEY, BasePart.PartTier.Rare.ToString());
		epicPrice = Singleton<GameConfigurationManager>.Instance.GetValue<int>(CRAFT_PRICE_CONFIG_KEY, BasePart.PartTier.Epic.ToString());
		IapManager.onPurchaseSucceeded += OnItemPurchase;
		KeyListener.keyReleased += HandleKeyReleased;
		isDestroyed = false;
	}

	private void OnUpgradeMachineBegin()
	{
		machineIsLocked = true;
	}

	private void OnMachineBehindCurtain()
	{
		UpdateLiquidTank(currentMachineScrapAmount, AlienCustomizationManager.GetPrice());
		UpdateAlienPartSilhouette();
	}

	private void OnUpgradeMachineEnd()
	{
		machineIsLocked = false;
		machineAnimation.state.End += OnMachineAnimationEnd;
		machineAnimation.state.Start += OnMachineAnimationStart;
		machineAnimation.state.Event += OnAnimationEvent;
	}

	private void HandleKeyReleased(KeyCode obj)
	{
		if (obj == KeyCode.Escape && !machineIsLocked)
		{
			Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: true);
		}
	}

	private void OnItemPurchase(IapManager.InAppPurchaseItemType type)
	{
		if (type == IapManager.InAppPurchaseItemType.WoodenLootCrate || type == IapManager.InAppPurchaseItemType.MetalLootCrate || type == IapManager.InAppPurchaseItemType.GoldenLootCrate)
		{
			partListingButton.UpdateNewTagState();
		}
	}

	private void OnEnable()
	{
		EventManager.Connect<CraftingMachineEvent>(OnCraftingMachineEvent);
		if (machineIdleLoop != null)
		{
			AudioSource component = machineIdleLoop.GetComponent<AudioSource>();
			if (component != null)
			{
				component.Play();
			}
		}
	}

	private void Start()
	{
		CoroutineRunner.Instance.StartCoroutine(Init());
	}

	private IEnumerator Init()
	{
		if (alienConverter.IsAlienMachine && alienConverter.RoutineShown)
		{
			alienConverter.ConvertToAlien();
			UpdateAlienPartSilhouette();
			UpdateLiquidTank(currentMachineScrapAmount, AlienCustomizationManager.GetPrice(), quick: true);
		}
		SetMachineScrapAmount(GameProgress.GetInt("Machine_scrap_amount"));
		currentMachineScrapAmount = targetMachineScrapAmount;
		UpdateMachineScrapLabel();
		pullChainButton.LockDragging(lockDragging: true);
		pullChainButton.SetPositionOffset(Vector3.up * 10f);
		machineAnimation.state.End += OnMachineAnimationEnd;
		machineAnimation.state.Start += OnMachineAnimationStart;
		machineAnimation.state.Event += OnAnimationEvent;
		SetMachineAnimation(introAnimationName);
		machineAnimation.timeScale = 0f;
		yield return new WaitForSeconds(0.1f);
		if (isDestroyed)
		{
			yield break;
		}
		machineAnimation.timeScale = 1f;
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.machineIntro);
		int idleAnimationIndex = (int)GetPartTierFromAmount(GameProgress.GetInt("Machine_scrap_amount"));
		SetMachineAnimation(idleAnimationNames[idleAnimationIndex], loop: true, queue: true);
		if (collectRewardButton != null)
		{
			collectRewardButton.SetActive(value: false);
		}
		yield return new WaitForSeconds(1f);
		if (isDestroyed)
		{
			yield break;
		}
		SetMachineIdleSound(idleAnimationIndex);
		float fade = 1f;
		while (fade > 0f)
		{
			fade -= Time.deltaTime;
			pullChainButton.SetPositionOffset(Vector3.up * fade * 10f);
			yield return null;
			if (isDestroyed)
			{
				yield break;
			}
		}
		pullChainButton.LockDragging(lockDragging: false);
		introPlaying = false;
	}

	private void OnDestroy()
	{
		isDestroyed = true;
		machineAnimation.state.End -= OnMachineAnimationEnd;
		machineAnimation.state.Start -= OnMachineAnimationStart;
		machineAnimation.state.Event -= OnAnimationEvent;
		AlienCraftingMachineConverter alienCraftingMachineConverter = alienConverter;
		alienCraftingMachineConverter.OnBeginUpgrade = (Action)Delegate.Remove(alienCraftingMachineConverter.OnBeginUpgrade, new Action(OnUpgradeMachineBegin));
		AlienCraftingMachineConverter alienCraftingMachineConverter2 = alienConverter;
		alienCraftingMachineConverter2.OnMachineBehindCurtain = (Action)Delegate.Remove(alienCraftingMachineConverter2.OnMachineBehindCurtain, new Action(OnMachineBehindCurtain));
		AlienCraftingMachineConverter alienCraftingMachineConverter3 = alienConverter;
		alienCraftingMachineConverter3.OnEndUpgrade = (Action)Delegate.Remove(alienCraftingMachineConverter3.OnEndUpgrade, new Action(OnUpgradeMachineEnd));
		IapManager.onPurchaseSucceeded -= OnItemPurchase;
		KeyListener.keyReleased -= HandleKeyReleased;
	}

	private void OnDisable()
	{
		EventManager.Disconnect<CraftingMachineEvent>(OnCraftingMachineEvent);
	}

	private void OnCraftingMachineEvent(CraftingMachineEvent data)
	{
		SetMachineScrapAmount(GameProgress.GetInt("Machine_scrap_amount"));
	}

	private void UpdateMachineScrapLabel()
	{
		if (MachineLabel != null)
		{
			MachineLabel.Text = $"{Mathf.Clamp(currentMachineScrapAmount, 0, int.MaxValue)}";
		}
	}

	private void SetMachineScrapAmount(int newAmount)
	{
		if (targetMachineScrapAmount == newAmount)
		{
			return;
		}
		targetMachineScrapAmount = newAmount;
		if (!IsAlienMachine && machineArrow != null)
		{
			float num = 1f;
			machineArrowStutterStrength = 1f;
			if (newAmount > 0)
			{
				if (newAmount < commonPrice)
				{
					num = (float)newAmount / (float)commonPrice * 38f;
					machineArrowStutterStrength = 1f;
				}
				else if (newAmount < rarePrice)
				{
					num = 63f + (float)(newAmount - commonPrice) / (float)(rarePrice - commonPrice) * 22f;
					machineArrowStutterStrength = 2f;
				}
				else if (newAmount < epicPrice)
				{
					num = 116f + (float)(newAmount - rarePrice) / (float)(epicPrice - rarePrice) * 22f;
					machineArrowStutterStrength = 3f;
				}
				else
				{
					num = 164f;
					machineArrowStutterStrength = 4f;
				}
				machineArrowStutterStrength *= 3f;
				num = Mathf.Clamp(num, 1f, 164f);
			}
			machineArrowTargetAngle = 0f - num;
			float num2 = machineArrow.localEulerAngles.z;
			if (Mathf.Approximately(num2, 0f) || num2 < 0f)
			{
				num2 += 360f;
			}
			machineArrowFromAngle = num2;
			arrowAngleFade = 0f;
		}
		else if (IsAlienMachine && liquidFillOverride != null)
		{
			UpdateLiquidTank(targetMachineScrapAmount, AlienCustomizationManager.GetPrice());
		}
	}

	public void OpenShop()
	{
		if (!IsMachineLocked)
		{
			SetActive(active: false);
			Singleton<IapManager>.Instance.OpenShopPage(delegate
			{
				SetActive(active: true);
			}, "LootCrates");
		}
	}

	private void SetActive(bool active)
	{
		GameObject[] array = hideObjects;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(active);
			}
		}
		base.gameObject.SetActive(active);
	}

	public void OpenPartList()
	{
		if (!pullLeverButton.IsActivating() && (!IsMachineLocked || waitingAnimationToEnd.Equals(introAnimationName)) && (bool)partListingButton)
		{
			partListingButton.OpenPartList();
		}
	}

	public void GoBack()
	{
		if (!IsMachineLocked || waitingAnimationToEnd.Equals(introAnimationName))
		{
			if ((Singleton<GameManager>.Instance.GetPrevGameState() == GameManager.GameState.LevelSelection || Singleton<GameManager>.Instance.GetPrevGameState() == GameManager.GameState.Level) && Singleton<GameManager>.Instance.CurrentEpisode != string.Empty)
			{
				Singleton<GameManager>.Instance.LoadLevelSelection(Singleton<GameManager>.Instance.CurrentEpisode, showLoadingScreen: true);
			}
			else
			{
				Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: true);
			}
		}
	}

	private void Update()
	{
		if (machineArrow != null)
		{
			float value = machineArrowTargetAngle + 360f + (Mathf.Sin(Time.time * machineArrowStutterStrength) + Mathf.Sin(Time.time * 2f * machineArrowStutterStrength) * machineArrowStutterStrength * 0.4f);
			Vector3 b = Vector3.forward * Mathf.Clamp(value, 1f, 359f);
			arrowAngleFade += GameTime.DeltaTime;
			machineArrow.localEulerAngles = Vector3.Lerp(Vector3.forward * machineArrowFromAngle, b, arrowAngleFade);
		}
		if (targetMachineScrapAmount != currentMachineScrapAmount && Time.realtimeSinceStartup >= nextLabelUpdate)
		{
			nextLabelUpdate = Time.realtimeSinceStartup + SoftCurrencyButton.updateInterval;
			int deltaAmount = SoftCurrencyButton.GetDeltaAmount(currentMachineScrapAmount, targetMachineScrapAmount);
			if (currentMachineScrapAmount < targetMachineScrapAmount)
			{
				currentMachineScrapAmount += deltaAmount;
			}
			else if (currentMachineScrapAmount > targetMachineScrapAmount)
			{
				currentMachineScrapAmount -= deltaAmount;
			}
			UpdateMachineScrapLabel();
		}
		if (!IsMachineLocked && queuedAddScrapActions > 0)
		{
			queuedAddScrapActions--;
			AddScrap();
		}
	}

	public void ChainPulled()
	{
		if (!pullChainButton.IsActivating())
		{
			AddScrap();
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.pullChain);
		}
	}

	public void AddScrap(int addScrapAmount = 0)
	{
		queuedAddScrapActions++;
		if (IsMachineLocked)
		{
			return;
		}
		queuedAddScrapActions--;
		int num = GameProgress.ScrapCount();
		int @int = GameProgress.GetInt("Machine_scrap_amount");
		int num2 = num - @int;
		int value = Singleton<GameConfigurationManager>.Instance.GetValue<int>(CRAFT_PRICE_CONFIG_KEY, BasePart.PartTier.Common.ToString());
		int value2 = Singleton<GameConfigurationManager>.Instance.GetValue<int>(CRAFT_PRICE_CONFIG_KEY, BasePart.PartTier.Rare.ToString());
		int value3 = Singleton<GameConfigurationManager>.Instance.GetValue<int>(CRAFT_PRICE_CONFIG_KEY, BasePart.PartTier.Epic.ToString());
		int price = AlienCustomizationManager.GetPrice();
		bool flag = CustomizationManager.CustomizationCount(BasePart.PartTier.Common, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable) <= 0;
		bool flag2 = CustomizationManager.CustomizationCount(BasePart.PartTier.Rare, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable) <= 0;
		bool flag3 = CustomizationManager.CustomizationCount(BasePart.PartTier.Epic, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable) <= 0;
		bool flag4 = CustomizationManager.CustomizationCount(BasePart.PartTier.Legendary, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable) <= 0;
		BasePart.PartTier nextTier = BasePart.PartTier.Regular;
		int num3 = 0;
		int num4;
		if (addScrapAmount > 0)
		{
			num3 = addScrapAmount;
			num4 = ((num3 + @int >= value) ? ((num3 + @int < value2) ? 1 : ((num3 + @int < value3) ? 2 : ((!IsAlienMachine || num3 + @int >= price) ? 3 : 2))) : 0);
			if (!IsAlienMachine && num3 + @int > value3)
			{
				num3 = value3 - @int;
			}
			else if (IsAlienMachine && num3 + @int > price)
			{
				num3 = price - @int;
			}
		}
		else if (!flag && @int < value)
		{
			nextTier = BasePart.PartTier.Common;
			num3 = value - @int;
			num4 = 0;
		}
		else if (!flag2 && @int < value2)
		{
			nextTier = BasePart.PartTier.Rare;
			num3 = value2 - @int;
			num4 = 1;
		}
		else if (!flag3 && @int < value3)
		{
			nextTier = BasePart.PartTier.Epic;
			num3 = value3 - @int;
			num4 = 2;
		}
		else if (IsAlienMachine && !flag4 && @int < price)
		{
			nextTier = BasePart.PartTier.Legendary;
			num3 = price - @int;
			num4 = 0;
		}
		else
		{
			num4 = 3;
		}
		if (num3 > 0 && num2 > 0)
		{
			if (num3 > num2)
			{
				ShowGetMoreScrapDialog(num3 - num2, nextTier);
				num3 = num2;
			}
			int partTierFromAmount = (int)GetPartTierFromAmount(@int);
			@int += num3;
			GameProgress.SetInt("Machine_scrap_amount", @int);
			EventManager.Send(new CraftingMachineEvent(CraftingMachineAction.AddScrap, GameProgress.GetInt("Machine_scrap_amount")));
			num4 = (int)GetPartTierFromAmount(@int);
			if (num4 >= 0)
			{
				SetMachineAnimation(chainPullAnimationName, loop: false, queue: true, releaseAfterEnd: false);
				SetMachineAnimation(feedAnimationNames[num4]);
				SetMachineAnimation(idleAnimationNames[num4], loop: true, queue: true);
				if (num4 != partTierFromAmount)
				{
					SetMachineIdleSound(num4);
				}
			}
			StartCoroutine(MoveNuts(nutRootUpper, nutRootLower, num3, 0.2f));
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.insertScrap);
		}
		else
		{
			queuedAddScrapActions = 0;
			SetMachineAnimation(idleAnimationNames[num4], loop: true, queue: true);
			SetMachineIdleSound(num4);
			if (num3 > 0 && num2 <= 0)
			{
				ShowGetMoreScrapDialog(num3, nextTier);
			}
		}
	}

	private void ShowGetMoreScrapDialog(int missingScrapAmount, BasePart.PartTier nextTier)
	{
		GameData gameData = Singleton<GameManager>.Instance.gameData;
		int value = Singleton<GameConfigurationManager>.Instance.GetValue<int>("scrap_to_coin_value", "value");
		if (value <= 0)
		{
			return;
		}
		int price = missingScrapAmount * value;
		if (getMoreScrapDialog == null && gameData.m_getMoreScrapDialog != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gameData.m_getMoreScrapDialog);
			gameObject.transform.position = new Vector3(0f, 0f, -15f);
			getMoreScrapDialog = gameObject.GetComponent<GetMoreScrapDialog>();
		}
		if (!(getMoreScrapDialog != null))
		{
			return;
		}
		getMoreScrapDialog.SetScrapAmount(missingScrapAmount, nextTier);
		getMoreScrapDialog.ConfirmButtonText = $"[snout] {price}";
		getMoreScrapDialog.ShowConfirmEnabled = () => true;
		getMoreScrapDialog.Close();
		getMoreScrapDialog.SetOnConfirm(delegate
		{
			if (GameProgress.UseSnoutCoins(price))
			{
				Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
				GameProgress.AddScrap(missingScrapAmount);
				SnoutButton.Instance.UpdateAmount();
				AddScrap();
				getMoreScrapDialog.Close();
			}
			else if (Singleton<IapManager>.IsInstantiated())
			{
				getMoreScrapDialog.Close();
				Singleton<IapManager>.Instance.OpenShopPage(getMoreScrapDialog.Open, "SnoutCoinShop");
			}
			else
			{
				getMoreScrapDialog.Close();
			}
		});
		getMoreScrapDialog.Open();
	}

	private IEnumerator MoveNuts(Transform spawnTf, Transform targetTf, int amount, float waitTime)
	{
		if (amount <= 0)
		{
			yield break;
		}
		bool goingUp = targetTf.position.y > spawnTf.position.y;
		yield return new WaitForSeconds(waitTime);
		Transform[] nuts = new Transform[10];
		for (int i = 0; i < nuts.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(nutPrefabs[0]);
			nuts[i] = gameObject.transform;
			nuts[i].position = spawnTf.position + Vector3.right * ((float)UnityEngine.Random.Range(0, i) / 10f) - Vector3.right;
			nuts[i].localEulerAngles = Vector3.forward * UnityEngine.Random.Range(0f, 180f);
		}
		int nutCount = nuts.Length;
		float fade = 0f;
		while (nutCount > 0)
		{
			fade += Time.deltaTime;
			for (int j = 0; j < nuts.Length; j++)
			{
				if (!(nuts[j] != null))
				{
					continue;
				}
				float num = targetTf.position.x - nuts[j].position.x;
				if (goingUp)
				{
					nuts[j].position += Vector3.up * Time.deltaTime * ((float)(j * 2) + 30f) + Vector3.right * num * Time.deltaTime * 10f;
				}
				else
				{
					nuts[j].position -= Vector3.up * Time.deltaTime * ((float)(j * 2) + 30f) - Vector3.right * num * Time.deltaTime * 10f;
				}
				if ((goingUp && nuts[j].position.y > targetTf.position.y) || (!goingUp && nuts[j].position.y < targetTf.position.y))
				{
					if (fade >= 0.8f)
					{
						UnityEngine.Object.Destroy(nuts[j].gameObject);
						nutCount--;
					}
					else
					{
						nuts[j].position = spawnTf.position + Vector3.right * 3f * ((float)UnityEngine.Random.Range(0, j) / 10f) - Vector3.right;
					}
				}
			}
			yield return null;
		}
	}

	private void RemoveScrap(int removeAmount)
	{
		int @int = GameProgress.GetInt("Machine_scrap_amount");
		if (@int >= removeAmount)
		{
			GameProgress.SetInt("Machine_scrap_amount", @int - removeAmount);
			EventManager.Send(new CraftingMachineEvent(CraftingMachineAction.RemoveScrap, GameProgress.GetInt("Machine_scrap_amount")));
		}
		else
		{
			ResetScrap(playAnimation: false);
		}
	}

	public void ResetScrap(bool playAnimation = true)
	{
		if (IsMachineLocked)
		{
			return;
		}
		queuedAddScrapActions = 0;
		if (playAnimation)
		{
			int @int = GameProgress.GetInt("Machine_scrap_amount");
			if (@int > 0)
			{
				SetMachineAnimation(resetAnimationName);
				SetMachineAnimation(idleAnimationNames[0], loop: true, queue: true);
				StartCoroutine(MoveNuts(nutRootLower, nutRootUpper, @int, 0.5f));
				Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.ejectScrap);
			}
		}
		GameProgress.SetInt("Machine_scrap_amount", 0);
		EventManager.Send(new CraftingMachineEvent(CraftingMachineAction.ResetScrap));
		SetMachineIdleSound(0);
	}

	private BasePart.PartTier GetPartTierFromAmount(int amount)
	{
		for (int num = 3; num > 0; num--)
		{
			if (amount == AlienCustomizationManager.GetPrice())
			{
				return BasePart.PartTier.Legendary;
			}
			GameConfigurationManager instance = Singleton<GameConfigurationManager>.Instance;
			string cRAFT_PRICE_CONFIG_KEY = CRAFT_PRICE_CONFIG_KEY;
			BasePart.PartTier partTier = (BasePart.PartTier)num;
			if (amount >= instance.GetValue<int>(cRAFT_PRICE_CONFIG_KEY, partTier.ToString()))
			{
				return (BasePart.PartTier)num;
			}
		}
		return BasePart.PartTier.Regular;
	}

	public void CraftSelectedPart()
	{
		if (IsMachineLocked)
		{
			return;
		}
		queuedAddScrapActions = 0;
		BasePart.PartTier partTierFromAmount = GetPartTierFromAmount(GameProgress.GetInt("Machine_scrap_amount"));
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.craftLeverCrank);
		if (partTierFromAmount == BasePart.PartTier.Regular)
		{
			SetMachineAnimation(insufficientScrapAnimationName);
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.craftEmpty);
			MaterialAnimation component = MachineLabel.GetComponent<MaterialAnimation>();
			if (component != null)
			{
				component.PlayAnimation(loop: true, 5);
			}
			return;
		}
		SnoutButton.Instance.EnableButton(enable: false);
		int num = 0;
		BasePart part = null;
		if (partTierFromAmount == BasePart.PartTier.Legendary && AlienCustomizationManager.GetNextUnlockable(out part))
		{
			num = AlienCustomizationManager.GetPrice();
		}
		else if (partTierFromAmount != BasePart.PartTier.Legendary)
		{
			num = Singleton<GameConfigurationManager>.Instance.GetValue<int>(CRAFT_PRICE_CONFIG_KEY, partTierFromAmount.ToString());
			part = CustomizationManager.GetRandomCraftablePartFromTier(partTierFromAmount, onlyLocked: true);
		}
		if ((bool)part && num > 0 && GameProgress.UseScrap(num))
		{
			SetMachineAnimation((!IsAlienMachine) ? craftAnimationName : slimeCraftAnimationName, loop: false, queue: false, releaseAfterEnd: false);
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.craftPart);
			CustomizationManager.UnlockPart(part, "crafted");
			RemoveScrap(num);
			SetMachineIdleSound(0);
			PlayerProgress.ExperienceType experienceType = PlayerProgress.ExperienceType.CommonPartCrafted;
			if (partTierFromAmount == BasePart.PartTier.Rare)
			{
				experienceType = PlayerProgress.ExperienceType.RarePartCrafted;
			}
			if (partTierFromAmount == BasePart.PartTier.Epic)
			{
				experienceType = PlayerProgress.ExperienceType.EpicPartCrafted;
			}
			if (partTierFromAmount == BasePart.PartTier.Legendary)
			{
				experienceType = PlayerProgress.ExperienceType.LegendaryPartCrafted;
			}
			PlayerProgressBar.Instance.DelayUpdate();
			int exp = Singleton<PlayerProgress>.Instance.AddExperience(experienceType);
			ShowReward(part, exp);
			string key = "CraftCount" + part.m_partTier;
			int @int = GameProgress.GetInt(key);
			GameProgress.SetInt(key, @int + 1);
			if (Singleton<SocialGameManager>.IsInstantiated() && part.m_partTier == BasePart.PartTier.Epic)
			{
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.CRAFT_PARTS", 100.0);
			}
			partsCraftedWhileInScreen++;
			EventManager.Send(new CraftingMachineEvent(CraftingMachineAction.CraftPart));
		}
		else
		{
			_ = (bool)part;
		}
	}

	private void ShowReward(BasePart part, int exp)
	{
		if (rewardGameObject != null)
		{
			UnityEngine.Object.Destroy(rewardGameObject);
		}
		rewardGameObject = UnityEngine.Object.Instantiate(lootRewardBackgrounds[(int)part.m_partTier]);
		rewardGameObject.transform.parent = rewardSpawnRoot.transform;
		rewardGameObject.transform.localPosition = Vector3.zero;
		rewardGameObject.transform.localScale = Vector3.one * 2f;
		rewardGameObject.transform.localRotation = Quaternion.identity;
		GameObject gameObject = UnityEngine.Object.Instantiate(part.m_constructionIconSprite.gameObject);
		LootRewardElement component = rewardGameObject.GetComponent<LootRewardElement>();
		if ((bool)component)
		{
			gameObject.transform.parent = component.IconRoot;
		}
		gameObject.transform.localPosition = -Vector3.forward * 0.5f;
		gameObject.transform.localScale = Vector3.one * 1.5f;
		gameObject.transform.localEulerAngles = Vector3.forward * 270f;
		StartCoroutine(WaitForReward(rewardGameObject, exp));
	}

	private IEnumerator WaitForReward(GameObject reward, int exp)
	{
		Transform[] tfs = reward.GetComponentsInChildren<Transform>();
		Transform[] array = tfs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = LayerMask.NameToLayer("Default");
		}
		MeshRenderer[] mrs = reward.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array2 = mrs;
		foreach (MeshRenderer obj in array2)
		{
			obj.sortingLayerName = "Default";
			obj.sortingOrder = 0;
		}
		reward.SetActive(value: false);
		float waitTime = 1f;
		while (waitTime > 0f)
		{
			waitTime -= Time.deltaTime;
			yield return null;
		}
		reward.SetActive(value: true);
		waitTime = 6.18f;
		while (waitTime > 0f)
		{
			waitTime -= Time.deltaTime;
			yield return null;
		}
		if (IsAlienMachine)
		{
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.alienMachineReveal);
		}
		array2 = mrs;
		foreach (MeshRenderer obj2 in array2)
		{
			obj2.sortingLayerName = "Popup";
			obj2.sortingOrder = 0;
		}
		reward.transform.ResetPosition(TransformCategory.Axis.Z);
		BackgroundMask.Show(show: true, this, "Popup", null, Vector3.forward, smoothFade: true);
		ResourceBar.Instance.ShowItem(ResourceBar.Item.PlayerProgress, showItem: true, enableItem: false);
		array = tfs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = LayerMask.NameToLayer("HUD");
		}
		yield return new WaitForSeconds(2f);
		if ((bool)PlayerProgressBar.Instance)
		{
			PlayerProgressBar.Instance.AddParticles(reward, exp, 0f, 0f, delegate(bool active)
			{
				if (active)
				{
					ResourceBar.Instance.ShowItem(ResourceBar.Item.PlayerProgress, showItem: true);
				}
			});
		}
		if (collectRewardButton != null)
		{
			collectRewardButton.SetActive(value: true);
		}
		EventManager.Connect<LevelUpEvent>(OnPlayerLevelUp);
	}

	private void OnPlayerLevelUp(LevelUpEvent data)
	{
		CollectReward(hideProgressBar: false);
	}

	public void CollectReward(bool hideProgressBar = true)
	{
		EventManager.Disconnect<LevelUpEvent>(OnPlayerLevelUp);
		if (collectRewardButton != null)
		{
			collectRewardButton.SetActive(value: false);
		}
		if (rewardGameObject != null)
		{
			UnityEngine.Object.Destroy(rewardGameObject);
		}
		machineIsLocked = false;
		BackgroundMask.Show(show: false, this, "Popup", null, Vector3.back, smoothFade: true);
		if (hideProgressBar)
		{
			ResourceBar.Instance.ShowItem(ResourceBar.Item.PlayerProgress, showItem: false);
		}
		ResourceBar.Instance.ShowItem(ResourceBar.Item.SnoutCoin, showItem: true);
		SetMachineAnimation(hideRewardAnimationName);
		partListingButton.UpdateNewTagState();
		if (!IsAlienMachine)
		{
			customizationsFullCheck.Check();
		}
		else
		{
			UpdateAlienPartSilhouette();
		}
		alienConverter.Check();
	}

	private void SetMachineAnimation(string newAnimation, bool loop = false, bool queue = false, bool releaseAfterEnd = true)
	{
		if (machineAnimation != null)
		{
			if (queue)
			{
				machineAnimation.state.AddAnimation(0, newAnimation, loop, 0f);
				return;
			}
			machineIsLocked = true;
			waitingAnimationToEnd = ((!releaseAfterEnd) ? string.Empty : newAnimation);
			machineAnimation.state.SetAnimation(0, newAnimation, loop);
		}
	}

	private void OnMachineAnimationStart(Spine.AnimationState state, int trackIndex)
	{
		if (idleAnimationNames == null || idleAnimationNames.Length <= 3)
		{
			return;
		}
		bool flag = idleAnimationNames[3].Equals(state.ToString());
		if (machineSmokePuffEffect != null)
		{
			if (flag)
			{
				machineSmokePuffEffect.Play();
			}
			else
			{
				machineSmokePuffEffect.Stop();
			}
		}
	}

	private void OnMachineAnimationEnd(Spine.AnimationState state, int trackIndex)
	{
		if (waitingAnimationToEnd.Equals(state.ToString()))
		{
			machineIsLocked = false;
		}
		if (introAnimationName.Equals(state.ToString()))
		{
			EventManager.Send(new CraftingMachineEvent(CraftingMachineAction.Idle));
		}
	}

	private void SetMachineIdleSound(int index)
	{
		int num = Mathf.Clamp(index, 0, gameData.commonAudioCollection.machineIdles.Length - 1);
		if (machineIdleLoop == null)
		{
			machineIdleLoop = Singleton<AudioManager>.Instance.SpawnLoopingEffect(gameData.commonAudioCollection.machineIdles[num], base.transform);
			return;
		}
		AudioSource component = machineIdleLoop.GetComponent<AudioSource>();
		Singleton<AudioManager>.Instance.StopLoopingEffect(component);
		machineIdleLoop = Singleton<AudioManager>.Instance.SpawnLoopingEffect(gameData.commonAudioCollection.machineIdles[num], base.transform);
	}

	private void OnAnimationEvent(Spine.AnimationState state, int trackIndex, Spine.Event e)
	{
		switch (e.Data.Name)
		{
		case "Item_Drop":
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.partCraftedJingle);
			break;
		case "Item_Extract":
			if (IsAlienMachine)
			{
				Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.craftingSlime);
			}
			break;
		case "LightBulb":
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.lightBulb);
			break;
		case "Reset_Button_Push":
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(gameData.commonAudioCollection.ejectScrapButton);
			break;
		case "InsufficientScrap":
			if ((bool)machineCraftEmptyEffect)
			{
				machineCraftEmptyEffect.Play();
			}
			break;
		case "Intro_MachineDrop":
			if ((bool)machineDropEffect)
			{
				machineDropEffect.Play();
			}
			break;
		}
	}

	private void UpdateLiquidTank(float currentScrap, float requiredScrap, bool quick = false)
	{
		float num = currentScrap / requiredScrap;
		float y = maxFill * num;
		Vector3 localPosition = liquidFillOverride.localPosition;
		localPosition.y = y;
		if (quick)
		{
			liquidFillOverride.localPosition = localPosition;
		}
		else
		{
			StartCoroutine(CoroutineRunner.MoveObject(liquidFillOverride, localPosition, 1f, useLocalPosition: true));
		}
	}

	private void UpdateAlienPartSilhouette()
	{
		if (!AlienCustomizationManager.Initialized)
		{
			return;
		}
		if (!AlienCustomizationManager.HasCraftableItems)
		{
			nextAlienPartIcon.ClearSprite();
			checkMark.SetActive(value: true);
			return;
		}
		checkMark.SetActive(value: false);
		if (AlienCustomizationManager.GetNextUnlockable(out var part))
		{
			string id = part.m_constructionIconSprite.Id;
			Singleton<RuntimeSpriteDatabase>.Instance.Find(id);
			nextAlienPartIcon.SetSprite(part.m_constructionIconSprite.gameObject);
		}
		else
		{
			nextAlienPartIcon.ClearSprite();
		}
	}
}
