using System;
using Spine.Unity;
using UnityEngine;

public class LootCrateSlot : WPFMonoBehaviour
{
	public enum State
	{
		Empty,
		Inactive,
		Locked,
		Unlocked
	}

	[SerializeField]
	private string openNowLocalization;

	[SerializeField]
	private Transform activeTf;

	[SerializeField]
	private Transform emptyTf;

	[SerializeField]
	private Transform unlockedTf;

	[SerializeField]
	private Transform animationTf;

	[SerializeField]
	private SkeletonAnimation skeletonAnimation;

	private Transform priceTf;

	private Transform timeTf;

	private Transform crateHolder;

	private Renderer lockIcon;

	private Renderer clockIcon;

	private TextMesh[] timeLabel;

	private TextMesh[] priceLabel;

	private TextMesh[] activeInfoLabel;

	private State state;

	private LootCrateType crateType = LootCrateType.None;

	private int index;

	private string identifier;

	private int unlockPrice;

	private LootCrateSlots lootCrateSlots;

	private float nextLabelUpdateTime;

	private bool areComponentsSet;

	private bool hasTimer;

	private const string CONFIG_LOOTCRATE_OPENING_RATES = "lootcrate_open_hour_rates";

	private void FindComponents()
	{
		if (!areComponentsSet)
		{
			lootCrateSlots = base.transform.parent.GetComponent<LootCrateSlots>();
			priceTf = activeTf.Find("Price");
			timeTf = activeTf.Find("Time");
			crateHolder = activeTf.Find("Crate");
			Transform transform = priceTf.Find("Label");
			if (transform != null)
			{
				priceLabel = transform.GetComponentsInChildren<TextMesh>();
			}
			transform = timeTf.Find("Label");
			if (transform != null)
			{
				timeLabel = transform.GetComponentsInChildren<TextMesh>();
			}
			transform = activeTf.Find("Info");
			if (transform != null)
			{
				activeInfoLabel = transform.GetComponentsInChildren<TextMesh>();
			}
			transform = timeTf.Find("Lock");
			if (transform != null)
			{
				lockIcon = transform.GetComponent<Renderer>();
			}
			transform = timeTf.Find("Clock");
			if (transform != null)
			{
				clockIcon = transform.GetComponent<Renderer>();
			}
			areComponentsSet = true;
			ChangeState(State.Empty);
		}
	}

	private void OnDestroy()
	{
		if (Singleton<TimeManager>.IsInstantiated() && Singleton<TimeManager>.Instance.HasTimer(identifier))
		{
			Singleton<TimeManager>.Instance.Unsubscribe(identifier, OnCrateUnlocked);
		}
	}

	private void ChangeState(State newState)
	{
		state = newState;
		if (state == State.Empty)
		{
			crateType = LootCrateType.None;
		}
		activeTf.gameObject.SetActive(state != State.Empty);
		emptyTf.gameObject.SetActive(state == State.Empty);
		unlockedTf.gameObject.SetActive(state == State.Unlocked);
		priceTf.gameObject.SetActive(state == State.Locked);
		timeTf.gameObject.SetActive(state != State.Unlocked);
		lockIcon.enabled = state == State.Inactive;
		clockIcon.enabled = state == State.Locked;
		bool flag = state == State.Unlocked || state == State.Inactive;
		crateHolder.transform.localScale = Vector3.one * ((!flag) ? 1f : 1.2f);
		crateHolder.transform.localPosition = Vector3.up * ((!flag) ? 0.1f : 0f);
		if (crateType != LootCrateType.None && crateHolder.childCount == 0 && state != 0)
		{
			GameObject obj = UnityEngine.Object.Instantiate(LootCrateSlots.GetCratePrefab(crateType));
			obj.transform.parent = crateHolder;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			obj.transform.localRotation = Quaternion.identity;
		}
		else if (crateHolder.childCount > 0 && state == State.Empty)
		{
			for (int i = 0; i < crateHolder.childCount; i++)
			{
				Transform child = crateHolder.GetChild(i);
				if ((bool)child)
				{
					UnityEngine.Object.Destroy(child.gameObject);
				}
			}
		}
		if (state == State.Locked)
		{
			Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve(openNowLocalization);
			TextMeshHelper.UpdateTextMeshes(activeInfoLabel, localeParameters.translation);
			TextMeshHelper.Wrap(activeInfoLabel, (!TextMeshHelper.UsesKanjiCharacters()) ? 8 : 4);
		}
		else
		{
			TextMeshHelper.UpdateTextMeshes(activeInfoLabel, string.Empty);
		}
		UpdateLabels();
	}

	private void Update()
	{
		if (nextLabelUpdateTime <= Time.realtimeSinceStartup)
		{
			nextLabelUpdateTime = Time.realtimeSinceStartup + 60f;
			UpdateLabels();
		}
	}

	private void UpdateLabels()
	{
		string formattedTimeFromSeconds = GetFormattedTimeFromSeconds((int)TimeLeftInSeconds());
		nextLabelUpdateTime = Time.realtimeSinceStartup + 1f;
		TextMeshHelper.UpdateTextMeshes(timeLabel, formattedTimeFromSeconds);
		TextMeshHelper.UpdateTextMeshes(priceLabel, $"{GetSnoutCoinPrice(crateType, TimeLeftInSeconds())} [snout]");
		TextMeshSpriteIcons[] componentsInChildren = priceTf.GetComponentsInChildren<TextMeshSpriteIcons>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].UpdateIcons();
			}
		}
	}

	public static string GetFormattedTimeFromSeconds(int timeLeft)
	{
		int num = Mathf.FloorToInt((float)timeLeft / 3600f);
		int num2 = Mathf.FloorToInt((float)timeLeft % 3600f / 60f);
		int num3 = Mathf.FloorToInt((float)timeLeft % 60f);
		string empty = string.Empty;
		if (num > 0 && num2 > 0)
		{
			return $"{num}h {num2}m";
		}
		if (num > 0)
		{
			return $"{num}h";
		}
		if (num2 > 0 && num3 > 0)
		{
			return $"{num2}m {num3}s";
		}
		if (num2 > 0)
		{
			return $"{num2}m";
		}
		return $"{num3}s";
	}

	private float TimeLeftInSeconds()
	{
		if (!string.IsNullOrEmpty(identifier) && Singleton<TimeManager>.Instance.HasTimer(identifier))
		{
			return Mathf.Clamp(Singleton<TimeManager>.Instance.TimeLeft(identifier), 0f, float.MaxValue);
		}
		if (state == State.Inactive)
		{
			return LootCrateSlots.GetOpenTimeForCrate(crateType);
		}
		return 0f;
	}

	public bool IsOccupied()
	{
		return state != State.Empty;
	}

	public void Initialize(bool isNew, int index, LootCrateType crateType)
	{
		FindComponents();
		this.index = index;
		this.crateType = crateType;
		identifier = GetSlotIdentifier(index);
		hasTimer = false;
		if (isNew)
		{
			LootCrateSlots.AddLootCrateToSlot(index, crateType);
			ChangeState(State.Inactive);
		}
		else
		{
			TryRecover();
		}
	}

	private void TryRecover()
	{
		string[] array = GameProgress.GetString(identifier, string.Empty).Split(',');
		if (array != null && array.Length >= 2 && UpdateSlotFromString(array) && state == State.Empty)
		{
			GameProgress.DeleteKey(identifier);
		}
		else if (!Singleton<TimeManager>.Instance.Initialized)
		{
			Singleton<TimeManager>.Instance.OnInitialize += UpdateTimer;
		}
		else
		{
			UpdateTimer();
		}
	}

	private void UpdateTimer()
	{
		Singleton<TimeManager>.Instance.OnInitialize -= UpdateTimer;
		hasTimer = Singleton<TimeManager>.Instance.HasTimer(identifier);
		if (hasTimer)
		{
			Singleton<TimeManager>.Instance.Subscribe(identifier, OnCrateUnlocked);
			GameProgress.SetString(identifier, $"{State.Locked.ToString()},{crateType.ToString()}");
			ChangeState(State.Locked);
		}
	}

	public void OpenCrate()
	{
		if (Singleton<TimeManager>.Instance.HasTimer(identifier))
		{
			Singleton<TimeManager>.Instance.RemoveTimer(identifier);
		}
		GameProgress.DeleteKey(identifier);
		LootCrate.SpawnLootCrateOpeningDialog(crateType, 1, WPFMonoBehaviour.s_hudCamera, null, new LootCrate.AnalyticData("Slot", unlockPrice.ToString(), LootCrate.AdWatched.NotApplicaple));
		ChangeState(State.Empty);
		LootCrateSlots.InformCrateOpened(identifier, crateType);
	}

	private void OnCrateUnlocked(int secondsSinceDone)
	{
		if (GameProgress.HasKey(identifier))
		{
			GameProgress.SetString(identifier, $"{State.Unlocked.ToString()},{crateType.ToString()}");
			ChangeState(State.Unlocked);
		}
		LootCrateSlots.InformCrateUnlocked(identifier, crateType, unlockPrice);
	}

	private bool UpdateSlotFromString(string[] data)
	{
		try
		{
			crateType = (LootCrateType)Enum.Parse(typeof(LootCrateType), data[1], ignoreCase: true);
		}
		catch
		{
			crateType = LootCrateType.None;
		}
		State state = State.Empty;
		try
		{
			state = (State)Enum.Parse(typeof(State), data[0], ignoreCase: true);
		}
		catch
		{
			state = State.Empty;
		}
		if (data.Length > 2 && data[2] == "new")
		{
			AppearAnimation();
			GameProgress.SetString(identifier, $"{data[0]},{data[1]}");
		}
		if (crateType == LootCrateType.None)
		{
			state = State.Empty;
		}
		ChangeState(state);
		return true;
	}

	public static int GetSnoutCoinPrice(LootCrateType crateType, float timeLeft)
	{
		string valueKey = crateType.ToString();
		int num = 0;
		if (Singleton<GameConfigurationManager>.Instance.HasValue("lootcrate_open_hour_rates", valueKey))
		{
			num = Singleton<GameConfigurationManager>.Instance.GetValue<int>("lootcrate_open_hour_rates", valueKey);
		}
		if (num <= 0)
		{
			num = 15;
		}
		return (int)Mathf.Clamp((float)num * (timeLeft / 3600f), 2f, 9999f);
	}

	public void OnSlotPressed()
	{
		if (Singleton<TimeManager>.Instance.CurrentEpochTime == 0)
		{
			return;
		}
		Debug.LogWarning("[OnSlotPressed] " + index + ": state(" + state.ToString() + ")");
		if (state == State.Inactive)
		{
			if (LootCrateSlots.IsUnlockingInProgress())
			{
				ShowPurchasePopup(UnlockLootCrateSlotDialog.UnlockType.PurchaseInactiveCrate);
			}
			else
			{
				ShowPurchasePopup(UnlockLootCrateSlotDialog.UnlockType.StartUnlocking);
			}
		}
		else if (state == State.Locked)
		{
			ShowPurchasePopup(UnlockLootCrateSlotDialog.UnlockType.PurchaseLockedCrate);
		}
		else if (state == State.Unlocked)
		{
			OpenCrate();
		}
	}

	private void ShowPurchasePopup(UnlockLootCrateSlotDialog.UnlockType unlockType)
	{
		unlockPrice = 0;
		int snoutPrice = GetSnoutCoinPrice(crateType, TimeLeftInSeconds());
		lootCrateSlots.ShowUnlockDialog(crateType, snoutPrice, (int)TimeLeftInSeconds(), delegate
		{
			if (unlockType == UnlockLootCrateSlotDialog.UnlockType.StartUnlocking)
			{
				ActivateLootCrateSlot();
			}
			else if (GameProgress.UseSnoutCoins(snoutPrice))
			{
				unlockPrice = snoutPrice;
				Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
				SnoutButton.Instance.UpdateAmount();
				OnCrateUnlocked(0);
				OpenCrate();
			}
		}, unlockType);
	}

	private void ActivateLootCrateSlot()
	{
		switch (LootCrateSlots.TryToActivateLootCrateAtSlot(index, crateType, OnCrateUnlocked))
		{
		case 1:
			UpdateTimer();
			break;
		case 0:
			ChangeState(State.Locked);
			break;
		}
	}

	public static string GetSlotIdentifier(int index)
	{
		return $"LootCrateSlot_{index}";
	}

	private void AppearAnimation()
	{
		skeletonAnimation.transform.parent = base.transform.parent;
		base.transform.parent = animationTf;
		skeletonAnimation.state.AddAnimation(0, "Intro1", loop: false, 0f);
		skeletonAnimation.state.Update(Time.deltaTime);
	}
}
