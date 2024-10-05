using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class LootCrateOpenDialog : WPFMonoBehaviour
{
	private class LootCrateRewardQueueElement
	{
		public LootCrateType lootCrateType;

		public LootCrate.AnalyticData data;

		public bool isRewarded;

		public int xp;

		public LootCrateRewardQueueElement(LootCrateType _lootCrateType, LootCrate.AnalyticData _data, int _xp)
		{
			lootCrateType = _lootCrateType;
			data = _data;
			isRewarded = false;
			xp = _xp;
		}

		public void SetRewarded()
		{
			isRewarded = true;
			GameProgress.RemoveLootcrate(lootCrateType);
		}
	}

	public class LootCrateDelivered : EventManager.Event
	{
		private LootCrateType type;

		public LootCrateType Type => type;

		public LootCrateDelivered(LootCrateType type)
		{
			this.type = type;
		}
	}

	private static bool s_dialogOpen;

	private static LootCrateOpenDialog instance;

	private Queue<LootCrateRewardQueueElement> lootcrateRewardQueue;

	[SerializeField]
	private GameObject lootCrateAnimation;

	[SerializeField]
	private SkeletonAnimation closeBtnAnimation;

	[SerializeField]
	private GameObject closeButton;

	[SerializeField]
	private GameObject closeButtonGfx;

	[SerializeField]
	private GameObject[] powerUpIcons;

	[SerializeField]
	private GameObject[] scrapIcons;

	[SerializeField]
	private GameObject[] dessertIcons;

	[SerializeField]
	private GameObject[] coinIcons;

	private GameObject lootCrateAnimationInstance;

	private bool crateOpened;

	private bool levelLootcrateOpened;

	public static bool DialogOpen => s_dialogOpen;

	public event Dialog.OnClose onClose;

	public static LootCrateOpenDialog CreateLootCrateOpenDialog()
	{
		if (instance == null && WPFMonoBehaviour.gameData.m_lootcrateOpenDialog != null)
		{
			instance = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_lootcrateOpenDialog, Vector3.zero, Quaternion.identity).GetComponent<LootCrateOpenDialog>();
		}
		return instance;
	}

	private void Awake()
	{
		lootCrateAnimationInstance = UnityEngine.Object.Instantiate(lootCrateAnimation, Vector3.up * 1000f, Quaternion.identity);
		lootCrateAnimationInstance.SetActive(value: false);
	}

	public void PrepareOpening()
	{
		if ((bool)WPFMonoBehaviour.levelManager)
		{
			levelLootcrateOpened = true;
			WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.LootCrateOpening);
		}
	}

	public void AddLootCrate(LootCrateType lootCrateType, int amount, LootCrate.AnalyticData data, bool fromQueue = false, int xp = 0)
	{
		if (lootcrateRewardQueue == null)
		{
			lootcrateRewardQueue = new Queue<LootCrateRewardQueueElement>();
		}
		if (!(lootCrateAnimation != null))
		{
			return;
		}
		if (lootCrateAnimationInstance == null)
		{
			lootCrateAnimationInstance = UnityEngine.Object.Instantiate(lootCrateAnimation, Vector3.up * 1000f, Quaternion.identity);
		}
		else
		{
			lootCrateAnimationInstance.SetActive(value: true);
		}
		lootCrateAnimationInstance.transform.parent = base.transform;
		LootCrateRewardQueueElement lootCrateRewardQueueElement = null;
		if (!fromQueue)
		{
			bool flag = lootcrateRewardQueue.Count == 0;
			for (int i = 0; i < amount; i++)
			{
				lootCrateRewardQueueElement = new LootCrateRewardQueueElement(lootCrateType, data, xp);
				lootcrateRewardQueue.Enqueue(lootCrateRewardQueueElement);
			}
			if (!flag)
			{
				return;
			}
		}
		else
		{
			lootCrateRewardQueueElement = lootcrateRewardQueue.Peek();
		}
		closeButton.SetActive(value: false);
		closeButtonGfx.SetActive(value: false);
		crateOpened = false;
		LootCrateRewards.SlotRewards[] randomRewards = LootCrateRewards.GetRandomRewards(lootCrateType);
		if (randomRewards == null)
		{
			return;
		}
		int num = 0;
		LootCrateButton componentInChildren = lootCrateAnimationInstance.GetComponentInChildren<LootCrateButton>();
		componentInChildren.GainedXP = lootCrateRewardQueueElement.xp;
		componentInChildren.Init(lootCrateType);
		componentInChildren.onOpeningDone = (Action)Delegate.Combine(componentInChildren.onOpeningDone, (Action)delegate
		{
			StartCoroutine(CrateOpened());
		});
		StartCoroutine(DelayIntro(componentInChildren, Vector3.forward * -1f));
		List<BasePart> list = new List<BasePart>();
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		List<int> list2 = new List<int>();
		for (int j = 0; j < randomRewards.Length; j++)
		{
			int num8 = UnityEngine.Random.Range(0, randomRewards.Length);
			int num9 = randomRewards.Length;
			while (list2.Contains(num8) && num9 >= 0)
			{
				if (++num8 >= randomRewards.Length)
				{
					num8 = 0;
				}
				num9--;
			}
			if (num9 < 0)
			{
				continue;
			}
			list2.Add(num8);
			LootCrateRewards.SlotRewards reward = randomRewards[num8];
			switch (reward.Type)
			{
			case LootCrateRewards.Reward.Part:
			{
				bool isDuplicatePart = false;
				int num10 = 0;
				int num11 = 10;
				BasePart randomLootCrateRewardPartFromTier;
				do
				{
					randomLootCrateRewardPartFromTier = CustomizationManager.GetRandomLootCrateRewardPartFromTier(reward.PartTier);
					num11--;
				}
				while (list.Contains(randomLootCrateRewardPartFromTier) && num11 > 0);
				list.Add(randomLootCrateRewardPartFromTier);
				if (CustomizationManager.IsPartUnlocked(randomLootCrateRewardPartFromTier))
				{
					num10 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("part_salvage_rewards", randomLootCrateRewardPartFromTier.m_partTier.ToString());
					GameProgress.AddScrap(num10);
					num2 += num10;
					num7++;
					isDuplicatePart = true;
				}
				else
				{
					CustomizationManager.UnlockPart(randomLootCrateRewardPartFromTier, lootCrateType.ToString() + "_crate");
				}
				componentInChildren.SetIcon(num, randomLootCrateRewardPartFromTier.m_constructionIconSprite.gameObject, string.Empty, (int)reward.PartTier, isDuplicatePart);
				componentInChildren.SetScrapIcon(num, scrapIcons[0], num10.ToString());
				switch (reward.PartTier)
				{
				case BasePart.PartTier.Epic:
					num6++;
					break;
				case BasePart.PartTier.Rare:
					num5++;
					break;
				case BasePart.PartTier.Common:
					num4++;
					break;
				}
				break;
			}
			case LootCrateRewards.Reward.Powerup:
			{
				GameObject iconPrefab = powerUpIcons[(int)(reward.Powerup - 1)];
				string customTypeOfGain = lootCrateType.ToString() + " crate";
				switch (reward.Powerup)
				{
				case LootCrateRewards.Powerup.Magnet:
					GameProgress.AddSuperMagnet(1);
					if (Singleton<IapManager>.Instance != null)
					{
						Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.SuperMagnetSingle, 1, customTypeOfGain);
					}
					break;
				case LootCrateRewards.Powerup.Superglue:
					GameProgress.AddSuperGlue(1);
					if (Singleton<IapManager>.Instance != null)
					{
						Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.SuperGlueSingle, 1, customTypeOfGain);
					}
					break;
				case LootCrateRewards.Powerup.Turbo:
					GameProgress.AddTurboCharge(1);
					if (Singleton<IapManager>.Instance != null)
					{
						Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.TurboChargeSingle, 1, customTypeOfGain);
					}
					break;
				case LootCrateRewards.Powerup.Supermechanic:
					GameProgress.AddBluePrints(1);
					if (Singleton<IapManager>.Instance != null)
					{
						Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.BlueprintSingle, 1, customTypeOfGain);
					}
					break;
				case LootCrateRewards.Powerup.NightVision:
					GameProgress.AddNightVision(1);
					if (Singleton<IapManager>.Instance != null)
					{
						Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.NightVisionSingle, 1, customTypeOfGain);
					}
					break;
				}
				componentInChildren.SetIcon(num, iconPrefab, string.Empty);
				break;
			}
			case LootCrateRewards.Reward.Dessert:
				if (reward.GoldenCupcake)
				{
					GameProgress.AddDesserts(WPFMonoBehaviour.gameData.m_desserts[WPFMonoBehaviour.gameData.m_desserts.Count - 1].GetComponent<Dessert>().saveId, 1);
					componentInChildren.SetIcon(num, dessertIcons[1], string.Empty);
				}
				else
				{
					GameProgress.AddDesserts(WPFMonoBehaviour.gameData.m_desserts[UnityEngine.Random.Range(0, WPFMonoBehaviour.gameData.m_desserts.Count - 1)].GetComponent<Dessert>().saveId, reward.Desserts);
					componentInChildren.SetIcon(num, dessertIcons[0], reward.Desserts.ToString());
				}
				num3 += reward.Desserts;
				break;
			case LootCrateRewards.Reward.Scrap:
			{
				GameProgress.AddScrap(reward.Scrap);
				num2 += reward.Scrap;
				GameObject scrapRewardContainer = componentInChildren.SetIcon(num, scrapIcons[0], reward.Scrap.ToString());
				LootRewardElement component2 = scrapRewardContainer.GetComponent<LootRewardElement>();
				if (component2 != null)
				{
					component2.onRewardOpened = (Action)Delegate.Combine(component2.onRewardOpened, (Action)delegate
					{
						ScrapButton.Instance.AddParticles(scrapRewardContainer, reward.Scrap, 0f, reward.Scrap);
					});
				}
				break;
			}
			case LootCrateRewards.Reward.Coin:
			{
				GameProgress.AddSnoutCoins(reward.Coins);
				GameObject coinRewardContainer = componentInChildren.SetIcon(num, coinIcons[0], reward.Coins.ToString());
				LootRewardElement component = coinRewardContainer.GetComponent<LootRewardElement>();
				if (component != null)
				{
					component.onRewardOpened = (Action)Delegate.Combine(component.onRewardOpened, (Action)delegate
					{
						SnoutButton.Instance.AddParticles(coinRewardContainer, reward.Scrap, 0f, reward.Scrap);
					});
				}
				break;
			}
			}
			num++;
		}
		if (lootCrateRewardQueueElement != null && num > 0)
		{
			lootCrateRewardQueueElement.SetRewarded();
			int @int = GameProgress.GetInt(lootCrateType.ToString() + "_crates_collected");
			GameProgress.SetInt(lootCrateType.ToString() + "_crates_collected", @int + 1);
		}
		EventManager.Send(new LootCrateDelivered(lootCrateType));
		int value = GameProgress.GetInt("Total_parts_scrapped") + num7;
		GameProgress.SetInt("Total_parts_scrapped", value);
		int value2 = GameProgress.GetInt("Total_parts_received") + num4 + num5 + num6;
		GameProgress.SetInt("Total_parts_received", value2);
	}

	private void ReportCustomPartsGainedEvent(string itemName, string typeOfGain, int amount)
	{
		if (Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.Level)
		{
			_ = Singleton<GameManager>.Instance.CurrentLevelIdentifier;
		}
	}

	private IEnumerator DelayIntro(LootCrateButton button, Vector3 position)
	{
		yield return null;
		button.PlayIntro();
		yield return null;
		button.transform.localPosition = position;
	}

	private IEnumerator CrateOpened()
	{
		closeButton.SetActive(value: true);
		closeBtnAnimation.state.AddAnimation(0, "Intro1", loop: false, 0f);
		crateOpened = true;
		yield return null;
		closeButtonGfx.SetActive(value: true);
	}

	public void Close()
	{
		if (lootCrateAnimationInstance != null)
		{
			UnityEngine.Object.Destroy(lootCrateAnimationInstance);
			lootCrateAnimationInstance = null;
		}
		while (lootcrateRewardQueue.Count > 0)
		{
			LootCrateRewardQueueElement lootCrateRewardQueueElement = lootcrateRewardQueue.Peek();
			if (!lootCrateRewardQueueElement.isRewarded)
			{
				AddLootCrate(lootCrateRewardQueueElement.lootCrateType, 1, lootCrateRewardQueueElement.data, fromQueue: true);
				return;
			}
			lootcrateRewardQueue.Dequeue();
		}
		if (levelLootcrateOpened && WPFMonoBehaviour.levelManager != null)
		{
			WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Continue);
		}
		levelLootcrateOpened = false;
		base.gameObject.SetActive(value: false);
		if (this.onClose != null)
		{
			this.onClose();
		}
	}

	private void OnEnable()
	{
		closeButton.SetActive(value: false);
		closeButtonGfx.SetActive(value: false);
		if (Singleton<GuiManager>.IsInstantiated())
		{
			Singleton<GuiManager>.Instance.GrabPointer(this);
		}
		if (Singleton<KeyListener>.IsInstantiated())
		{
			Singleton<KeyListener>.Instance.GrabFocus(this);
		}
		KeyListener.keyReleased += HandleKeyReleased;
		EventManager.Send(new UIEvent(UIEvent.Type.OpenedLootCrateDialog));
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			ResourceBar.Instance.LockItem(ResourceBar.Item.SnoutCoin, showItem: true, enableItem: false, revertable: true);
		}
		ResourceBar.Instance.LockItem(ResourceBar.Item.Scrap, showItem: true, enableItem: false, revertable: true);
		s_dialogOpen = true;
	}

	private void OnDisable()
	{
		if (Singleton<GuiManager>.IsInstantiated())
		{
			Singleton<GuiManager>.Instance.ReleasePointer(this);
		}
		if (Singleton<KeyListener>.IsInstantiated())
		{
			Singleton<KeyListener>.Instance.ReleaseFocus(this);
		}
		KeyListener.keyReleased -= HandleKeyReleased;
		EventManager.Send(new UIEvent(UIEvent.Type.ClosedLootCrateDialog));
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			ResourceBar.Instance.ReleaseItem(ResourceBar.Item.SnoutCoin);
		}
		ResourceBar.Instance.ReleaseItem(ResourceBar.Item.Scrap);
		s_dialogOpen = false;
	}

	private void HandleKeyReleased(KeyCode obj)
	{
		if (crateOpened && obj == KeyCode.Escape)
		{
			Close();
		}
	}
}
