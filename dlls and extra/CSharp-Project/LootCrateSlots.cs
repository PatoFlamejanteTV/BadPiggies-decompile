using System;
using System.Collections.Generic;
using UnityEngine;

public class LootCrateSlots : MonoBehaviour
{
	[SerializeField]
	private GameObject lootCrateSlotPrefab;

	[SerializeField]
	private int slotCount = 4;

	[SerializeField]
	private GameObject unlockCrateSlotDialogPrefab;

	[SerializeField]
	private GameObject slotsFullBubble;

	private UnlockLootCrateSlotDialog unlockCrateSlotDialog;

	private LootCrateSlot[] slots;

	private static LootCrateType overflowCrateType = LootCrateType.None;

	private static LootCrateSlots instance;

	private const string CONFIG_LOOTCRATE_OPENING_TIME_KEY = "lootcrate_open_times";

	private const string CURRENT_OPENING_SLOT_IDENTIFIER = "LootCrateSlotOpening";

	public static int SlotsAvailable { get; private set; }

	private void Awake()
	{
		instance = this;
		List<LootCrateSlot> list = new List<LootCrateSlot>();
		for (int i = 0; i < slotCount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(lootCrateSlotPrefab);
			if (gameObject != null)
			{
				gameObject.name = $"Slot{i + 1}";
				gameObject.transform.parent = base.transform;
				LootCrateSlot component = gameObject.GetComponent<LootCrateSlot>();
				component.Initialize(isNew: false, i, LootCrateType.None);
				list.Add(component);
			}
		}
		slots = list.ToArray();
		SlotsAvailable = slots.Length;
		GetComponent<GridLayout>().UpdateLayout();
		if (slotsFullBubble != null)
		{
			slotsFullBubble.transform.position = slots[slotCount - 1].transform.position;
		}
		ShowFullBubble(AreSlotsFull());
		if (overflowCrateType == LootCrateType.None)
		{
			return;
		}
		LootCrateType crateType = overflowCrateType;
		unlockCrateSlotDialog = ShowNoFreeSlotsDialog(crateType, delegate
		{
			if (unlockCrateSlotDialog != null && GameProgress.UseSnoutCoins(unlockCrateSlotDialog.SnoutCoinPrice))
			{
				Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
				LootCrate.SpawnLootCrateOpeningDialog(crateType, 1, WPFMonoBehaviour.hudCamera, null, new LootCrate.AnalyticData("CakeRaceOverflowUnlock", unlockCrateSlotDialog.SnoutCoinPrice.ToString(), LootCrate.AdWatched.NotApplicaple));
				SendLootCrateUnlockedFlurryEvent(crateType, unlockCrateSlotDialog.SnoutCoinPrice, "overflow");
			}
		});
		overflowCrateType = LootCrateType.None;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public bool IsPopUpOpen()
	{
		return unlockCrateSlotDialog != null;
	}

	public void ShowFullBubble(bool show)
	{
		if (slotsFullBubble != null)
		{
			slotsFullBubble.SetActive(show);
		}
	}

	public static GameObject GetCratePrefab(LootCrateType crateType)
	{
		GameObject[] array = WPFMonoBehaviour.gameData.m_lootCrateLargeIcons.ToArray();
		if (crateType >= LootCrateType.Wood && (int)crateType < array.Length)
		{
			return array[(int)crateType];
		}
		return null;
	}

	public void ShowUnlockDialog(LootCrateType crateType, int price, int timeLeft, TextDialog.OnConfirm onConfirm, UnlockLootCrateSlotDialog.UnlockType unlockType)
	{
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey && !(unlockCrateSlotDialogPrefab == null))
		{
			if (unlockCrateSlotDialog == null)
			{
				unlockCrateSlotDialog = UnityEngine.Object.Instantiate(unlockCrateSlotDialogPrefab).GetComponent<UnlockLootCrateSlotDialog>();
			}
			unlockCrateSlotDialog.Open();
			unlockCrateSlotDialog.SetInfoLabel(unlockType);
			unlockCrateSlotDialog.SetOnConfirm(onConfirm);
			unlockCrateSlotDialog.InitPopup(price, timeLeft, GetCratePrefab(crateType), crateType);
		}
	}

	public UnlockLootCrateSlotDialog ShowNoFreeSlotsDialog(LootCrateType crateType, TextDialog.OnConfirm onConfirm)
	{
		if (WPFMonoBehaviour.gameData.m_noFreeCrateSlotsPopup == null)
		{
			return null;
		}
		UnlockLootCrateSlotDialog component = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_noFreeCrateSlotsPopup).GetComponent<UnlockLootCrateSlotDialog>();
		component.Open();
		component.SetOnConfirm(onConfirm);
		int openTimeForCrate = GetOpenTimeForCrate(crateType);
		component.InitPopup(LootCrateSlot.GetSnoutCoinPrice(crateType, openTimeForCrate), openTimeForCrate, GetCratePrefab(crateType), crateType);
		return component;
	}

	public static bool AreSlotsFull()
	{
		for (int i = 0; i < SlotsAvailable; i++)
		{
			if (!IsSlotOccupied(i))
			{
				return false;
			}
		}
		return true;
	}

	public static void AddLootCrateToFreeSlot(LootCrateType crateType)
	{
		for (int i = 0; i < SlotsAvailable; i++)
		{
			if (!IsSlotOccupied(i))
			{
				AddLootCrateToSlot(i, crateType);
				int @int = GameProgress.GetInt("loot_crates_added_to_slots");
				GameProgress.SetInt("loot_crates_added_to_slots", ++@int);
				if (instance != null)
				{
					instance.ShowFullBubble(AreSlotsFull());
				}
				return;
			}
		}
		overflowCrateType = crateType;
	}

	public static void AddLootCrateToSlot(int index, LootCrateType crateType)
	{
		GameProgress.SetString(LootCrateSlot.GetSlotIdentifier(index), string.Format("{0},{1},{2}", LootCrateSlot.State.Inactive.ToString(), crateType.ToString(), "new"));
	}

	public static int TryToActivateLootCrateAtSlot(int index, LootCrateType crateType, OnTimedOut onCrateUnlocked)
	{
		if (IsUnlockingInProgress())
		{
			return -1;
		}
		string slotIdentifier = LootCrateSlot.GetSlotIdentifier(index);
		GameProgress.SetString(slotIdentifier, $"{LootCrateSlot.State.Locked.ToString()},{crateType.ToString()}");
		if (!Singleton<TimeManager>.Instance.HasTimer(slotIdentifier))
		{
			DateTime time = Singleton<TimeManager>.Instance.CurrentTime.AddSeconds(GetOpenTimeForCrate(crateType));
			Singleton<TimeManager>.Instance.CreateTimer(slotIdentifier, time, onCrateUnlocked);
			GameProgress.SetString("LootCrateSlotOpening", slotIdentifier);
			if (instance != null)
			{
				instance.ShowFullBubble(AreSlotsFull());
			}
			return 0;
		}
		return 1;
	}

	public static bool IsUnlockingInProgress()
	{
		return GameProgress.HasKey("LootCrateSlotOpening");
	}

	public static void InformCrateUnlocked(string identifier, LootCrateType crateType, int snoutCoinCost = 0)
	{
		if (GameProgress.GetString("LootCrateSlotOpening", string.Empty).Equals(identifier))
		{
			GameProgress.DeleteKey("LootCrateSlotOpening");
		}
		if (instance != null)
		{
			instance.SendLootCrateUnlockedFlurryEvent(crateType, snoutCoinCost, "default");
		}
	}

	private void SendLootCrateUnlockedFlurryEvent(LootCrateType crateType, int snoutCoinCost, string unlockType)
	{
		int @int = GameProgress.GetInt("loot_crates_unlocked_from_slots");
		GameProgress.SetInt("loot_crates_unlocked_from_slots", ++@int);
	}

	public static void InformCrateOpened(string identifier, LootCrateType crateType)
	{
		if (instance != null)
		{
			instance.ShowFullBubble(AreSlotsFull());
		}
	}

	public static int GetOpenTimeForCrate(LootCrateType crateType)
	{
		string valueKey = crateType.ToString();
		if (Singleton<GameConfigurationManager>.Instance.HasValue("lootcrate_open_times", valueKey))
		{
			int value = Singleton<GameConfigurationManager>.Instance.GetValue<int>("lootcrate_open_times", valueKey);
			if (value > 0)
			{
				return value;
			}
		}
		return crateType switch
		{
			LootCrateType.Wood => HoursToSeconds(8f), 
			LootCrateType.Metal => HoursToSeconds(24f), 
			LootCrateType.Gold => HoursToSeconds(48f), 
			LootCrateType.Cardboard => HoursToSeconds(2f), 
			LootCrateType.Glass => HoursToSeconds(4f), 
			LootCrateType.Bronze => HoursToSeconds(16f), 
			LootCrateType.Marble => HoursToSeconds(28f), 
			_ => HoursToSeconds(72f), 
		};
	}

	private static int HoursToSeconds(float hours)
	{
		return Mathf.FloorToInt(3600f * hours);
	}

	private static bool IsSlotOccupied(int index)
	{
		return GameProgress.HasKey(LootCrateSlot.GetSlotIdentifier(index));
	}
}
