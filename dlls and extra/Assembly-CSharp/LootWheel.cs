using System;
using Spine;
using Spine.Unity;
using UnityEngine;

public class LootWheel : WPFMonoBehaviour
{
	public enum WheelSlotType
	{
		Part,
		Scrap1,
		Scrap2,
		Dessert1,
		Dessert2,
		Dessert3,
		Powerup
	}

	[Serializable]
	public class WheelSlot
	{
		[SerializeField]
		private float m_rotationBegin;

		[HideInInspector]
		[SerializeField]
		private float m_rotationEnd;

		[SerializeField]
		private WheelSlotType m_slotType;

		[SerializeField]
		private TextMesh m_countIndicator;

		[SerializeField]
		private GameObject m_collectedIndicator;

		[SerializeField]
		private GameObject m_collectableIcon;

		private LootWheelRewards.LootWheelReward[] m_rewards;

		private float[] m_probabilities;

		private int m_rewardIndex;

		public float RotationBegin => m_rotationBegin;

		public float RotationEnd => m_rotationEnd;

		public WheelSlotType SlotType => m_slotType;

		public int RewardIndex => m_rewardIndex;

		public bool RewardsCollected => m_rewardIndex >= m_rewards.Length;

		public int RewardsLeft => m_rewards.Length - m_rewardIndex;

		public int TotalRewards => m_rewards.Length;

		public GameObject CollectableIcon => m_collectableIcon;

		public float Probability
		{
			get
			{
				if (m_rewardIndex >= m_rewards.Length)
				{
					return 0f;
				}
				float num = 0f;
				for (int i = 0; i < m_probabilities.Length; i++)
				{
					num += m_probabilities[i];
				}
				return num;
			}
		}

		public LootWheelRewards.LootWheelReward[] InitReward(LootWheelRewards rewards)
		{
			m_rewardIndex = 0;
			switch (m_slotType)
			{
			case WheelSlotType.Part:
				m_rewards = new LootWheelRewards.LootWheelReward[3]
				{
					rewards.GetReward(LootWheelRewards.WheelReward.CommonPart),
					rewards.GetReward(LootWheelRewards.WheelReward.RarePart),
					rewards.GetReward(LootWheelRewards.WheelReward.EpicPart)
				};
				break;
			case WheelSlotType.Scrap1:
				m_rewards = new LootWheelRewards.LootWheelReward[1] { rewards.GetReward(LootWheelRewards.WheelReward.Scrap1) };
				break;
			case WheelSlotType.Scrap2:
				m_rewards = new LootWheelRewards.LootWheelReward[1] { rewards.GetReward(LootWheelRewards.WheelReward.Scrap2) };
				break;
			case WheelSlotType.Dessert1:
				m_rewards = new LootWheelRewards.LootWheelReward[1] { rewards.GetReward(LootWheelRewards.WheelReward.Dessert1) };
				break;
			case WheelSlotType.Dessert2:
				m_rewards = new LootWheelRewards.LootWheelReward[1] { rewards.GetReward(LootWheelRewards.WheelReward.Dessert2) };
				break;
			case WheelSlotType.Dessert3:
				m_rewards = new LootWheelRewards.LootWheelReward[1] { rewards.GetReward(LootWheelRewards.WheelReward.Dessert3) };
				break;
			case WheelSlotType.Powerup:
				m_rewards = new LootWheelRewards.LootWheelReward[1] { rewards.GetReward(LootWheelRewards.WheelReward.Powerup) };
				break;
			}
			m_probabilities = new float[m_rewards.Length];
			for (int i = 0; i < m_probabilities.Length; i++)
			{
				float num = (float)rewards.TotalRewardValues / (float)m_rewards[i].TotalValue;
				m_probabilities[i] = num / rewards.TotalRewardInverseValues;
			}
			if (m_countIndicator != null && m_rewards.Length != 0)
			{
				TextMesh[] componentsInChildren = m_countIndicator.gameObject.GetComponentsInChildren<TextMesh>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].text = m_rewards[0].Amount.ToString();
				}
			}
			if (m_slotType == WheelSlotType.Part)
			{
				CheckDuplicateParts();
			}
			if (m_slotType == WheelSlotType.Powerup && m_rewards.Length != 0)
			{
				CreatePowerupIcon(m_rewards[0].PowerupReward);
			}
			m_collectedIndicator.SetActive(value: false);
			return m_rewards;
		}

		public void SetCollectIndicator(bool set)
		{
			m_collectedIndicator.SetActive(set);
		}

		public bool PeekReward(out LootWheelRewards.LootWheelReward reward)
		{
			if (m_rewardIndex < m_rewards.Length && m_rewardIndex >= 0)
			{
				reward = m_rewards[m_rewardIndex];
				return true;
			}
			reward = LootWheelRewards.LootWheelReward.Empty;
			return false;
		}

		public bool GetReward(out LootWheelRewards.LootWheelReward reward)
		{
			if (m_rewardIndex < m_rewards.Length && m_rewardIndex >= 0)
			{
				reward = m_rewards[m_rewardIndex++];
				return true;
			}
			reward = LootWheelRewards.LootWheelReward.Empty;
			return false;
		}

		private void CheckDuplicateParts()
		{
			ConfigData config = Singleton<GameConfigurationManager>.Instance.GetConfig("part_salvage_rewards");
			for (int i = 0; i < m_rewards.Length; i++)
			{
				if (CustomizationManager.IsPartUnlocked(m_rewards[i].PartReward))
				{
					int amount = int.Parse(config[m_rewards[i].PartReward.m_partTier.ToString()]);
					m_rewards[i] = new LootWheelRewards.LootWheelReward(amount, m_rewards[i].SingleValue, LootWheelRewards.RewardType.Scrap);
				}
			}
		}

		private void CreatePowerupIcon(LootCrateRewards.Powerup powerup)
		{
			if (m_collectableIcon == null)
			{
				return;
			}
			Sprite component = m_collectableIcon.GetComponent<Sprite>();
			if (!(component == null))
			{
				string text = string.Empty;
				switch (m_rewards[0].PowerupReward)
				{
				case LootCrateRewards.Powerup.Magnet:
					text = WPFMonoBehaviour.gameData.m_superMagnetIcon.GetComponent<Sprite>().Id;
					break;
				case LootCrateRewards.Powerup.Superglue:
					text = WPFMonoBehaviour.gameData.m_superGlueIcon.GetComponent<Sprite>().Id;
					break;
				case LootCrateRewards.Powerup.Turbo:
					text = WPFMonoBehaviour.gameData.m_turboChargeIcon.GetComponent<Sprite>().Id;
					break;
				case LootCrateRewards.Powerup.Supermechanic:
					text = WPFMonoBehaviour.gameData.m_superMechanicIcon.GetComponent<Sprite>().Id;
					break;
				case LootCrateRewards.Powerup.NightVision:
					text = WPFMonoBehaviour.gameData.m_nightVisionIcon.GetComponent<Sprite>().Id;
					break;
				}
				if (!string.IsNullOrEmpty(text))
				{
					RuntimeSpriteDatabase instance = Singleton<RuntimeSpriteDatabase>.Instance;
					component.SelectSprite(instance.Find(text), forceResetMesh: true);
				}
			}
		}
	}

	[SerializeField]
	private Transform needleTransform;

	[SerializeField]
	private Rigidbody wheelRigidbody;

	[SerializeField]
	private WheelSlot[] wheelSlots;

	[SerializeField]
	private LootWheelRewardingRoutine rewardRoutine;

	[SerializeField]
	private SkeletonAnimation shakeAnimation;

	[SerializeField]
	private string starAnimationName;

	[SerializeField]
	private string shakeAnimationName;

	[SerializeField]
	private Transform epicPartRoot;

	[SerializeField]
	private Transform rarePartRoot;

	[SerializeField]
	private Transform commonPartRoot;

	[SerializeField]
	private Transform epicPartStarsRoot;

	[SerializeField]
	private Transform rarePartStarsRoot;

	[SerializeField]
	private Transform commonPartStarsRoot;

	[SerializeField]
	private LootWheelPopup popup;

	[SerializeField]
	private GameObject spinText;

	[SerializeField]
	private GameObject spinAgainText;

	[SerializeField]
	private GameObject scrapIconPrefab;

	[SerializeField]
	private GameObject genericTextPrefab;

	[SerializeField]
	private float initialSpinTime = 0.75f;

	[SerializeField]
	private float decelerationRate = 0.15f;

	[SerializeField]
	private float spinVelocity = 30f;

	[SerializeField]
	private float tickSoundRate = 5f;

	[SerializeField]
	private int odysseySpinCount;

	private LootWheelSpinner spinner;

	private LootWheelRewards rewards;

	private bool initialized;

	private bool subscribed;

	private GameObject epicPart;

	private GameObject rarePart;

	private GameObject commonPart;

	private int currentSpin;

	private int currentPrice;

	private Action OnStarAnimationEnd;

	private SkeletonAnimation epicStarAnim;

	private SkeletonAnimation rareStarAnim;

	private SkeletonAnimation commonStarAnim;

	private int TotalRewards
	{
		get
		{
			int num = 0;
			for (int i = 0; i < wheelSlots.Length; i++)
			{
				num += wheelSlots[i].TotalRewards;
			}
			return num;
		}
	}

	private int RewardsLeft
	{
		get
		{
			int num = 0;
			for (int i = 0; i < wheelSlots.Length; i++)
			{
				num += wheelSlots[i].RewardsLeft;
			}
			return num;
		}
	}

	public bool Initialized => initialized;

	public bool Dirty => currentSpin > 0;

	private void Awake()
	{
		initialized = false;
		subscribed = false;
		spinner = new LootWheelSpinner(wheelRigidbody, needleTransform, wheelSlots);
		rewards = new LootWheelRewards();
		epicStarAnim = epicPartStarsRoot.gameObject.GetComponentInChildren<SkeletonAnimation>(includeInactive: true);
		rareStarAnim = rarePartStarsRoot.gameObject.GetComponentInChildren<SkeletonAnimation>(includeInactive: true);
		commonStarAnim = commonPartStarsRoot.gameObject.GetComponentInChildren<SkeletonAnimation>(includeInactive: true);
	}

	private void OnDestroy()
	{
		if (epicStarAnim != null && epicStarAnim.state != null)
		{
			epicStarAnim.state.Complete -= OnStarAnimationComplete;
		}
		if (rareStarAnim != null && rareStarAnim.state != null)
		{
			rareStarAnim.state.Complete -= OnStarAnimationComplete;
		}
		if (commonStarAnim != null && commonStarAnim.state != null)
		{
			commonStarAnim.state.Complete -= OnStarAnimationComplete;
		}
		if (rewards != null)
		{
			LootWheelRewards lootWheelRewards = rewards;
			lootWheelRewards.OnInitialized = (Action)Delegate.Remove(lootWheelRewards.OnInitialized, new Action(Initialize));
		}
	}

	public void ForceReInit()
	{
		initialized = false;
		if (rewards.Initialized)
		{
			Initialize();
			return;
		}
		LootWheelRewards lootWheelRewards = rewards;
		lootWheelRewards.OnInitialized = (Action)Delegate.Combine(lootWheelRewards.OnInitialized, new Action(Initialize));
	}

	private void Initialize()
	{
		for (int i = 0; i < wheelSlots.Length; i++)
		{
			LootWheelRewards.LootWheelReward[] array = wheelSlots[i].InitReward(rewards);
			if (wheelSlots[i].SlotType == WheelSlotType.Part)
			{
				if (commonPart != null)
				{
					UnityEngine.Object.Destroy(commonPart);
				}
				commonPart = InstantiateRewardImage(commonPartRoot, array[0]);
				if (rarePart != null)
				{
					UnityEngine.Object.Destroy(rarePart);
				}
				rarePart = InstantiateRewardImage(rarePartRoot, array[1]);
				if (epicPart != null)
				{
					UnityEngine.Object.Destroy(epicPart);
				}
				epicPart = InstantiateRewardImage(epicPartRoot, array[2]);
			}
		}
		if (!subscribed)
		{
			if (epicStarAnim != null && epicStarAnim.state != null)
			{
				epicStarAnim.state.Complete += OnStarAnimationComplete;
			}
			if (rareStarAnim != null && rareStarAnim.state != null)
			{
				rareStarAnim.state.Complete += OnStarAnimationComplete;
			}
			if (commonStarAnim != null && commonStarAnim.state != null)
			{
				commonStarAnim.state.Complete += OnStarAnimationComplete;
			}
			subscribed = true;
		}
		SetStarsEnabled(BasePart.PartTier.Epic, enabled: false, null);
		SetStarsEnabled(BasePart.PartTier.Rare, enabled: false, null);
		SetStarsEnabled(BasePart.PartTier.Common, enabled: false, null);
		SetCheckMark(BasePart.PartTier.Epic, enabled: false);
		SetCheckMark(BasePart.PartTier.Rare, enabled: false);
		SetCheckMark(BasePart.PartTier.Common, enabled: false);
		currentSpin = 0;
		currentPrice = CalculateSpinPrice(currentSpin);
		SetPriceToButton(currentPrice);
		SetSpinText(currentSpin);
		popup.DoneButtonHidden = true;
		popup.SpinButtonEnabled = true;
		initialized = true;
	}

	private GameObject InstantiateRewardImage(Transform root, LootWheelRewards.LootWheelReward reward)
	{
		GameObject gameObject = root.parent.Find("Label").gameObject;
		GameObject gameObject2;
		switch (reward.Type)
		{
		default:
			return null;
		case LootWheelRewards.RewardType.Scrap:
		{
			gameObject.SetActive(value: true);
			gameObject2 = UnityEngine.Object.Instantiate(scrapIconPrefab);
			GameObject gameObject3 = UnityEngine.Object.Instantiate(genericTextPrefab);
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.position += new Vector3(0f, -0.05f, -0.1f);
			gameObject3.transform.localScale = new Vector3(0.08f, 0.08f, 1f);
			TextMesh[] componentsInChildren = gameObject3.GetComponentsInChildren<TextMesh>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].alignment = TextAlignment.Center;
				componentsInChildren[i].anchor = TextAnchor.MiddleCenter;
				componentsInChildren[i].text = reward.Amount.ToString();
			}
			SetLayer(gameObject3, base.gameObject.layer);
			SetSortingLayer(gameObject3, "Popup");
			SetOrderInLayer(gameObject3, 0);
			break;
		}
		case LootWheelRewards.RewardType.Part:
			gameObject2 = UnityEngine.Object.Instantiate(reward.PartReward.m_constructionIconSprite.gameObject);
			gameObject.SetActive(value: false);
			break;
		}
		gameObject2.transform.parent = root;
		gameObject2.transform.localPosition = Vector3.zero;
		SetSortingLayer(gameObject2, "Popup");
		SetLayer(gameObject2, base.gameObject.layer);
		SetOrderInLayer(gameObject2, 0);
		return gameObject2;
	}

	public void Spin()
	{
		if (!initialized || spinner.IsSpinning)
		{
			return;
		}
		WheelSlot target = GetRewardSlot();
		if (target != null && GameProgress.UseSnoutCoins(currentPrice))
		{
			if (currentPrice > 0)
			{
				Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
			}
			SnoutButton.Instance.UpdateAmount();
			popup.SpinButtonEnabled = false;
			popup.DoneButtonEnabled = false;
			spinner.Spin(target, initialSpinTime, spinVelocity, decelerationRate, tickSoundRate, delegate
			{
				OnSpinEnd(target);
			});
			SendFlurryLootWheelSpinEvent(target, currentPrice);
		}
		else
		{
			Singleton<IapManager>.Instance.OpenShopPage(delegate
			{
				popup.gameObject.SetActive(value: true);
			}, "SnoutCoinShop");
			popup.gameObject.SetActive(value: false);
		}
	}

	private void OnSpinEnd(WheelSlot reward)
	{
		currentSpin++;
		GiveReward(reward);
	}

	private WheelSlot GetRewardSlot(int counter = 0)
	{
		if (counter > 10)
		{
			return null;
		}
		float num = 0f;
		for (int i = 0; i < wheelSlots.Length; i++)
		{
			num += wheelSlots[i].Probability;
		}
		if (num <= 0f)
		{
			return null;
		}
		float num2 = UnityEngine.Random.Range(0f, 1f) * num;
		float num3 = 0f;
		for (int j = 0; j < wheelSlots.Length; j++)
		{
			num3 += wheelSlots[j].Probability;
			if (num2 <= num3)
			{
				return wheelSlots[j];
			}
		}
		return GetRewardSlot(++counter);
	}

	private void GiveReward(WheelSlot slot)
	{
		if (!slot.GetReward(out var reward))
		{
			return;
		}
		switch (reward.Type)
		{
		case LootWheelRewards.RewardType.Dessert:
			RewardDesserts(reward.Amount);
			break;
		case LootWheelRewards.RewardType.Scrap:
			RewardScrap(reward.Amount);
			ScrapButton.Instance.UpdateAmount();
			break;
		case LootWheelRewards.RewardType.Powerup:
			RewardPowerup(reward.PowerupReward);
			break;
		case LootWheelRewards.RewardType.Part:
			RewardPart(reward.PartReward);
			break;
		}
		if (slot.SlotType == WheelSlotType.Part)
		{
			SetStarsEnabled((BasePart.PartTier)slot.RewardIndex, enabled: true, delegate
			{
				shakeAnimation.state.AddAnimation(0, shakeAnimationName, loop: false, 0f);
				CoroutineRunner.Instance.DelayAction(delegate
				{
					ShowRewardRoutine(slot, reward.Amount, reward.Type, delegate
					{
						SetCheckMark((BasePart.PartTier)slot.RewardIndex, enabled: true);
					});
				}, 0.3f, realTime: false);
			});
		}
		else
		{
			ShowRewardRoutine(slot, reward.Amount, reward.Type, delegate
			{
				slot.SetCollectIndicator(set: true);
			});
		}
	}

	private void RewardDesserts(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			int index = UnityEngine.Random.Range(0, WPFMonoBehaviour.gameData.m_desserts.Count - 1);
			GameProgress.AddDesserts(WPFMonoBehaviour.gameData.m_desserts[index].name, 1);
		}
	}

	private void RewardPart(BasePart part)
	{
		CustomizationManager.UnlockPart(part, "_LootWheel");
	}

	private void RewardScrap(int amount)
	{
		GameProgress.AddScrap(amount);
	}

	private void RewardPowerup(LootCrateRewards.Powerup powerup)
	{
		switch (powerup)
		{
		case LootCrateRewards.Powerup.Magnet:
			GameProgress.AddSuperMagnet(1);
			break;
		case LootCrateRewards.Powerup.Superglue:
			GameProgress.AddSuperGlue(1);
			break;
		case LootCrateRewards.Powerup.Turbo:
			GameProgress.AddTurboCharge(1);
			break;
		case LootCrateRewards.Powerup.Supermechanic:
			GameProgress.AddBluePrints(1);
			break;
		case LootCrateRewards.Powerup.NightVision:
			GameProgress.AddNightVision(1);
			break;
		}
	}

	private void ShowRewardRoutine(WheelSlot slot, int rewardAmount, LootWheelRewards.RewardType reward, Action OnEnd)
	{
		GameObject copy;
		LootWheelRewardingRoutine.BackgroundType bgType;
		bool showHorns;
		if (slot.SlotType == WheelSlotType.Part)
		{
			if (slot.RewardIndex == 1)
			{
				copy = commonPart.transform.parent.gameObject;
				bgType = LootWheelRewardingRoutine.BackgroundType.Common;
			}
			else if (slot.RewardIndex == 2)
			{
				copy = rarePart.transform.parent.gameObject;
				bgType = LootWheelRewardingRoutine.BackgroundType.Rare;
			}
			else
			{
				copy = epicPart.transform.parent.gameObject;
				bgType = LootWheelRewardingRoutine.BackgroundType.Epic;
			}
			showHorns = true;
			bgType = ((reward != LootWheelRewards.RewardType.Part) ? LootWheelRewardingRoutine.BackgroundType.Regular : bgType);
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.lootWheelJackPot);
		}
		else
		{
			copy = slot.CollectableIcon;
			bgType = LootWheelRewardingRoutine.BackgroundType.Regular;
			showHorns = false;
		}
		CoroutineRunner.Instance.DelayAction(delegate
		{
			rewardRoutine.ShowRewarding(showHorns, rewardAmount, UnityEngine.Object.Instantiate(copy), bgType, OnEnd);
			popup.DoneButtonEnabled = true;
			if ((!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey && currentSpin < TotalRewards) || (Singleton<BuildCustomizationLoader>.Instance.IsOdyssey && currentSpin < odysseySpinCount))
			{
				popup.SpinButtonEnabled = true;
				currentPrice = CalculateSpinPrice(currentSpin);
				SetPriceToButton(currentPrice);
				SetSpinText(currentSpin);
			}
		}, 0.5f, realTime: false);
	}

	private void SetSpinText(int currentSpins)
	{
		bool flag = currentSpins <= 0;
		spinText.SetActive(flag);
		spinAgainText.SetActive(!flag);
	}

	private int CalculateSpinPrice(int currentSpin)
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			return 0;
		}
		if (currentSpin <= 0)
		{
			return 0;
		}
		float num = rewards.RewardValueAvg * 2f * rewards.SpinPriceVariation / (float)TotalRewards;
		return Mathf.RoundToInt((rewards.RewardValueAvg * (1f - rewards.SpinPriceVariation) + (float)currentSpin * num) * rewards.SpinPriceMultiplier / 10f) * 10;
	}

	private void SetPriceToButton(int price)
	{
		if (price <= 0)
		{
			popup.ResetSpinButtonTextMaterials();
			popup.SpinButtonText = "TEXT_FREE";
			popup.RefreshSpinButtonTranslation();
		}
		else
		{
			popup.SpinButtonText = $"[snout] {price}";
		}
	}

	private void SetStarsEnabled(BasePart.PartTier tier, bool enabled, Action OnAnimationEnd)
	{
		switch (tier)
		{
		case BasePart.PartTier.Common:
			SetStarEnabled(commonPartStarsRoot, enabled, OnAnimationEnd);
			break;
		case BasePart.PartTier.Rare:
			SetStarEnabled(rarePartStarsRoot, enabled, OnAnimationEnd);
			break;
		case BasePart.PartTier.Epic:
			SetStarEnabled(epicPartStarsRoot, enabled, OnAnimationEnd);
			break;
		}
	}

	private void SetCheckMark(BasePart.PartTier tier, bool enabled)
	{
		switch (tier)
		{
		case BasePart.PartTier.Common:
			SetCheckMark(commonPartStarsRoot.parent, enabled);
			break;
		case BasePart.PartTier.Rare:
			SetCheckMark(rarePartStarsRoot.parent, enabled);
			break;
		case BasePart.PartTier.Epic:
			SetCheckMark(epicPartStarsRoot.parent, enabled);
			break;
		}
	}

	private void SetStarEnabled(Transform starRoot, bool enabled, Action OnAnimationEnd)
	{
		starRoot.Find("Star").gameObject.SetActive(enabled);
		if (enabled)
		{
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.lootWheelStarExplosion);
			OnStarAnimationEnd = (Action)Delegate.Combine(OnStarAnimationEnd, OnAnimationEnd);
			starRoot.gameObject.GetComponentInChildren<SkeletonAnimation>().state.AddAnimation(0, starAnimationName, loop: false, 0f);
		}
	}

	private void OnStarAnimationComplete(Spine.AnimationState state, int trackIndex, int loopCount)
	{
		if (OnStarAnimationEnd != null)
		{
			OnStarAnimationEnd();
			OnStarAnimationEnd = null;
		}
	}

	private void SetCheckMark(Transform root, bool enabled)
	{
		root.Find("Checked").gameObject.SetActive(enabled);
	}

	private void SetSortingLayer(GameObject go, string layer)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingLayerName = layer;
		}
	}

	private void SetOrderInLayer(GameObject go, int order)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingOrder = order;
		}
	}

	private void SetLayer(GameObject go, int layer)
	{
		go.layer = layer;
		for (int i = 0; i < go.transform.childCount; i++)
		{
			SetLayer(go.transform.GetChild(i).gameObject, layer);
		}
	}

	private void SendFlurryLootWheelSpinEvent(WheelSlot slot, int snoutCost)
	{
	}
}
