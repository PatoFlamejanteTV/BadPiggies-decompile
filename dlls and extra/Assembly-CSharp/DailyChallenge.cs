using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyChallenge : Singleton<DailyChallenge>
{
	public struct ChallengeInfo
	{
		private static readonly string[] jokerLevelNames = new string[9] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

		private static readonly int[] episodes = new int[6] { 1, 3, 4, 2, 5, 6 };

		public string levelName;

		public int episodeIndex;

		public int levelIndex;

		public int positionIndex;

		public bool collected;

		public bool revealed;

		public bool adRevealed;

		public static ChallengeInfo Empty => new ChallengeInfo(null, -1, -1);

		public string DailyKey => $"{episodeIndex}-{levelIndex}";

		public string ImageKey => $"{episodes[episodeIndex]}-{LevelCode}-{positionIndex}.png";

		public string Location => $"{episodes[episodeIndex]}-{LevelCode}";

		private string LevelCode
		{
			get
			{
				if ((levelIndex + 1) % 5 == 0)
				{
					return jokerLevelNames[levelIndex / 5];
				}
				return (levelIndex + 1 - levelIndex / 5).ToString();
			}
		}

		public ChallengeInfo(string levelName, int episodeIndex, int levelIndex)
		{
			this.levelName = levelName;
			this.episodeIndex = episodeIndex;
			this.levelIndex = levelIndex;
			positionIndex = 0;
			collected = false;
			revealed = Singleton<BuildCustomizationLoader>.instance.IsOdyssey;
			adRevealed = false;
		}

		public ChallengeInfo(string levelName, int episodeIndex, int levelIndex, int positionIndex, bool collected, bool revealed, bool adRevealed)
		{
			this.levelName = levelName;
			this.episodeIndex = episodeIndex;
			this.levelIndex = levelIndex;
			this.positionIndex = positionIndex;
			this.collected = collected;
			this.revealed = revealed;
			this.adRevealed = adRevealed;
		}

		public override string ToString()
		{
			return $"LevelName: '{levelName}', EpisodeIndex: '{episodeIndex}', LevelIndex: '{levelIndex}', PositionIndex: '{positionIndex}'";
		}

		public override bool Equals(object obj)
		{
			if (obj is ChallengeInfo challengeInfo)
			{
				if (challengeInfo.episodeIndex == episodeIndex && challengeInfo.levelIndex == levelIndex)
				{
					return challengeInfo.positionIndex == positionIndex;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public struct ChallengeConfigs
	{
		private const string CFG_LOOT_CRATE_INTERVAL = "loot_crate_interval";

		private const string CFG_UNLOCKED_LEVEL_WOOD = "unlocked_chance_wood";

		private const string CFG_1_LOCKED_LEVEL_WOOD = "1_locked_chance_wood";

		private const string CFG_2_LOCKED_LEVEL_WOOD = "2_locked_chance_wood";

		private const string CFG_UNLOCKED_JOKER_LEVEL_WOOD = "unlocked_joker_level_wood";

		private const string CFG_LOCKED_JOKER_LEVEL_WOOD = "locked_joker_level_wood";

		private const string CFG_1_MAX_UNLOCK_DISTANCE_WOOD = "1_max_unlock_distance_wood";

		private const string CFG_2_MAX_UNLOCK_DISTANCE_WOOD = "2_max_unlock_distance_wood";

		private const string CFG_UNLOCKED_LEVEL_METAL = "unlocked_chance_metal";

		private const string CFG_1_LOCKED_LEVEL_METAL = "1_locked_chance_metal";

		private const string CFG_2_LOCKED_LEVEL_METAL = "2_locked_chance_metal";

		private const string CFG_1_MAX_UNLOCK_DISTANCE_METAL = "1_max_unlock_distance_metal";

		private const string CFG_2_MAX_UNLOCK_DISTANCE_METAL = "2_max_unlock_distance_metal";

		private const string CFG_UNLOCKED_JOKER_LEVEL_METAL = "unlocked_joker_level_metal";

		private const string CFG_LOCKED_JOKER_LEVEL_METAL = "locked_joker_level_metal";

		private const string CFG_UNLOCKED_LEVEL_GOLD = "unlocked_chance_gold";

		private const string CFG_1_LOCKED_LEVEL_GOLD = "1_locked_chance_gold";

		private const string CFG_2_LOCKED_LEVEL_GOLD = "2_locked_chance_gold";

		private const string CFG_1_MAX_UNLOCK_DISTANCE_GOLD = "1_max_unlock_distance_gold";

		private const string CFG_2_MAX_UNLOCK_DISTANCE_GOLD = "2_max_unlock_distance_gold";

		private const string CFG_UNLOCKED_JOKER_LEVEL_GOLD = "unlocked_joker_level_gold";

		private const string CFG_LOCKED_JOKER_LEVEL_GOLD = "locked_joker_level_gold";

		private Dictionary<LootCrateType, float> unlockedLevelChance;

		private Dictionary<LootCrateType, float> lockedLevelChance1;

		private Dictionary<LootCrateType, float> lockedLevelChance2;

		private Dictionary<LootCrateType, float> unlockedJokerLevelChance;

		private Dictionary<LootCrateType, float> lockedJokerLevelChance;

		private Dictionary<LootCrateType, int> maxUnlockDistance1;

		private Dictionary<LootCrateType, int> maxUnlockDistance2;

		public ChallengeConfigs(Hashtable data)
		{
			unlockedLevelChance = new Dictionary<LootCrateType, float>();
			unlockedLevelChance.Add(LootCrateType.Wood, (!data.ContainsKey("unlocked_chance_wood")) ? 0f : float.Parse(data["unlocked_chance_wood"] as string));
			unlockedLevelChance.Add(LootCrateType.Metal, (!data.ContainsKey("unlocked_chance_metal")) ? 0f : float.Parse(data["unlocked_chance_metal"] as string));
			unlockedLevelChance.Add(LootCrateType.Gold, (!data.ContainsKey("unlocked_chance_gold")) ? 0f : float.Parse(data["unlocked_chance_gold"] as string));
			lockedLevelChance1 = new Dictionary<LootCrateType, float>();
			lockedLevelChance1.Add(LootCrateType.Wood, (!data.ContainsKey("1_locked_chance_wood")) ? 0f : float.Parse(data["1_locked_chance_wood"] as string));
			lockedLevelChance1.Add(LootCrateType.Metal, (!data.ContainsKey("1_locked_chance_metal")) ? 0f : float.Parse(data["1_locked_chance_metal"] as string));
			lockedLevelChance1.Add(LootCrateType.Gold, (!data.ContainsKey("1_locked_chance_gold")) ? 0f : float.Parse(data["1_locked_chance_gold"] as string));
			lockedLevelChance2 = new Dictionary<LootCrateType, float>();
			lockedLevelChance2.Add(LootCrateType.Wood, (!data.ContainsKey("2_locked_chance_wood")) ? 0f : float.Parse(data["2_locked_chance_wood"] as string));
			lockedLevelChance2.Add(LootCrateType.Metal, (!data.ContainsKey("2_locked_chance_metal")) ? 0f : float.Parse(data["2_locked_chance_metal"] as string));
			lockedLevelChance2.Add(LootCrateType.Gold, (!data.ContainsKey("2_locked_chance_gold")) ? 0f : float.Parse(data["2_locked_chance_gold"] as string));
			unlockedJokerLevelChance = new Dictionary<LootCrateType, float>();
			unlockedJokerLevelChance.Add(LootCrateType.Wood, (!data.ContainsKey("unlocked_joker_level_wood")) ? 0f : float.Parse(data["unlocked_joker_level_wood"] as string));
			unlockedJokerLevelChance.Add(LootCrateType.Metal, (!data.ContainsKey("unlocked_joker_level_metal")) ? 0f : float.Parse(data["unlocked_joker_level_metal"] as string));
			unlockedJokerLevelChance.Add(LootCrateType.Gold, (!data.ContainsKey("unlocked_joker_level_gold")) ? 0f : float.Parse(data["unlocked_joker_level_gold"] as string));
			lockedJokerLevelChance = new Dictionary<LootCrateType, float>();
			lockedJokerLevelChance.Add(LootCrateType.Wood, (!data.ContainsKey("locked_joker_level_wood")) ? 0f : float.Parse(data["locked_joker_level_wood"] as string));
			lockedJokerLevelChance.Add(LootCrateType.Metal, (!data.ContainsKey("locked_joker_level_metal")) ? 0f : float.Parse(data["locked_joker_level_metal"] as string));
			lockedJokerLevelChance.Add(LootCrateType.Gold, (!data.ContainsKey("locked_joker_level_gold")) ? 0f : float.Parse(data["locked_joker_level_gold"] as string));
			maxUnlockDistance1 = new Dictionary<LootCrateType, int>();
			maxUnlockDistance1.Add(LootCrateType.Wood, data.ContainsKey("1_max_unlock_distance_wood") ? int.Parse(data["1_max_unlock_distance_wood"] as string) : 0);
			maxUnlockDistance1.Add(LootCrateType.Metal, data.ContainsKey("1_max_unlock_distance_metal") ? int.Parse(data["1_max_unlock_distance_metal"] as string) : 0);
			maxUnlockDistance1.Add(LootCrateType.Gold, data.ContainsKey("1_max_unlock_distance_gold") ? int.Parse(data["1_max_unlock_distance_gold"] as string) : 0);
			maxUnlockDistance2 = new Dictionary<LootCrateType, int>();
			maxUnlockDistance2.Add(LootCrateType.Wood, data.ContainsKey("2_max_unlock_distance_wood") ? int.Parse(data["2_max_unlock_distance_wood"] as string) : 0);
			maxUnlockDistance2.Add(LootCrateType.Metal, data.ContainsKey("2_max_unlock_distance_metal") ? int.Parse(data["2_max_unlock_distance_metal"] as string) : 0);
			maxUnlockDistance2.Add(LootCrateType.Gold, data.ContainsKey("2_max_unlock_distance_gold") ? int.Parse(data["2_max_unlock_distance_gold"] as string) : 0);
		}

		public float GetUnlockLevelChance(LootCrateType type)
		{
			switch (type)
			{
			case LootCrateType.Wood:
			case LootCrateType.Bronze:
				return unlockedLevelChance[LootCrateType.Metal];
			case LootCrateType.Metal:
			case LootCrateType.Gold:
			case LootCrateType.Marble:
				return unlockedLevelChance[LootCrateType.Gold];
			default:
				return unlockedLevelChance[LootCrateType.Wood];
			}
		}

		public float GetLockedLevelChance1(LootCrateType type)
		{
			switch (type)
			{
			case LootCrateType.Wood:
			case LootCrateType.Bronze:
				return lockedLevelChance1[LootCrateType.Metal];
			case LootCrateType.Metal:
			case LootCrateType.Gold:
			case LootCrateType.Marble:
				return lockedLevelChance1[LootCrateType.Gold];
			default:
				return lockedLevelChance1[LootCrateType.Wood];
			}
		}

		public float GetLockedLevelChance2(LootCrateType type)
		{
			switch (type)
			{
			case LootCrateType.Wood:
			case LootCrateType.Bronze:
				return lockedLevelChance2[LootCrateType.Metal];
			case LootCrateType.Metal:
			case LootCrateType.Gold:
			case LootCrateType.Marble:
				return lockedLevelChance2[LootCrateType.Gold];
			default:
				return lockedLevelChance2[LootCrateType.Wood];
			}
		}

		public float GetUnlockedJokerLevelChance(LootCrateType type)
		{
			switch (type)
			{
			case LootCrateType.Wood:
			case LootCrateType.Bronze:
				return unlockedJokerLevelChance[LootCrateType.Metal];
			case LootCrateType.Metal:
			case LootCrateType.Gold:
			case LootCrateType.Marble:
				return unlockedJokerLevelChance[LootCrateType.Gold];
			default:
				return unlockedJokerLevelChance[LootCrateType.Wood];
			}
		}

		public float GetLockedJokerLevelChanche(LootCrateType type)
		{
			switch (type)
			{
			case LootCrateType.Wood:
			case LootCrateType.Bronze:
				return lockedJokerLevelChance[LootCrateType.Metal];
			case LootCrateType.Metal:
			case LootCrateType.Gold:
			case LootCrateType.Marble:
				return lockedJokerLevelChance[LootCrateType.Gold];
			default:
				return lockedJokerLevelChance[LootCrateType.Wood];
			}
		}

		public int MaxUnlockDistance1(LootCrateType type)
		{
			switch (type)
			{
			case LootCrateType.Wood:
			case LootCrateType.Bronze:
				return maxUnlockDistance1[LootCrateType.Metal];
			case LootCrateType.Metal:
			case LootCrateType.Gold:
			case LootCrateType.Marble:
				return maxUnlockDistance1[LootCrateType.Gold];
			default:
				return maxUnlockDistance1[LootCrateType.Wood];
			}
		}

		public int MaxUnlockDistance2(LootCrateType type)
		{
			switch (type)
			{
			case LootCrateType.Wood:
			case LootCrateType.Bronze:
				return maxUnlockDistance2[LootCrateType.Metal];
			case LootCrateType.Metal:
			case LootCrateType.Gold:
			case LootCrateType.Marble:
				return maxUnlockDistance2[LootCrateType.Gold];
			default:
				return maxUnlockDistance2[LootCrateType.Wood];
			}
		}
	}

	public Action OnInitialize;

	public Action OnDailyChallengeChanged;

	private const int CHALLENGE_COUNT = 3;

	private const string TIMER_ID = "daily_challenge_timer";

	private const string CONFIG_ID = "daily_challenge";

	private bool initialized;

	private ChallengeInfo[] dailyChallenges;

	private DailyChallengeProgram dailyChallengeProgram;

	private ChallengeConfigs challengeConfigs;

	private bool overrideLoot;

	private LootCrateType lootCrate;

	public TimeSpan DailyChallengeTimeLeft
	{
		get
		{
			if (initialized)
			{
				return new TimeSpan(0, 0, 0, Mathf.RoundToInt(Singleton<TimeManager>.Instance.TimeLeft("daily_challenge_timer")));
			}
			return default(TimeSpan);
		}
	}

	public int Count
	{
		get
		{
			if (dailyChallenges == null)
			{
				return 0;
			}
			return dailyChallenges.Length;
		}
	}

	public ChallengeInfo[] Challenges => dailyChallenges;

	public bool HasChallenge => dailyChallenges != null;

	public bool Initialized => initialized;

	public int Left
	{
		get
		{
			if (!HasChallenge)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < dailyChallenges.Length; i++)
			{
				if (!dailyChallenges[i].collected)
				{
					num++;
				}
			}
			return num;
		}
	}

	private bool FirstChallengeCollected
	{
		get
		{
			return GameProgress.GetBool("FirstChallengeCollected");
		}
		set
		{
			GameProgress.SetBool("FirstChallengeCollected", value);
		}
	}

	private void Awake()
	{
		SetAsPersistant();
		dailyChallengeProgram = new DailyChallengeProgram();
		StartCoroutine(WaitFor(CanInitialize, Initialize));
		EventManager.Connect<GameLevelLoaded>(OnLevelLoaded);
		EventManager.Connect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameLevelLoaded>(OnLevelLoaded);
		EventManager.Disconnect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnLevelLoaded(GameLevelLoaded data)
	{
		LevelManager levelManager = WPFMonoBehaviour.levelManager;
		if (!(levelManager != null) || !(levelManager.CurrentGameMode is BaseGameMode) || levelManager.m_sandbox || levelManager.m_raceLevel)
		{
			return;
		}
		int index = 0;
		if (!IsDailyChallenge(data.episodeIndex, data.levelIndex, out index) || dailyChallenges[index].collected)
		{
			return;
		}
		DailyLevel daily = Singleton<GameManager>.instance.gameData.m_dailyChallengeData.GetDaily(dailyChallenges[index].DailyKey);
		if (daily != null && daily.GetPosition(dailyChallenges[index].positionIndex, out var position))
		{
			GameObject gameObject = WPFMonoBehaviour.gameData.m_lootCrates[(int)TodaysLootCrate(index)];
			LootCrate component = UnityEngine.Object.Instantiate(gameObject, position, Quaternion.identity).GetComponent<LootCrate>();
			component.OnCollect = (Action)Delegate.Combine(component.OnCollect, (Action)delegate
			{
				OnDailyRewardCollected(index);
			});
			component.RewardExperience = (Func<int>)Delegate.Combine(component.RewardExperience, new Func<int>(RewardExperience));
			component.SetAnalyticData(index, dailyChallenges[index].adRevealed);
			dailyChallenges[index].revealed = true;
			Debug.LogWarning("Instantiated " + gameObject.name + " at position " + position.ToString());
		}
	}

	private void OnPlayerChanged(PlayerChangedEvent data)
	{
		if (initialized)
		{
			initialized = false;
			StartCoroutine(WaitFor(CanInitialize, Initialize));
		}
	}

	private bool CanInitialize()
	{
		if (LootCrateRewards.Initialized && Singleton<TimeManager>.Instance.Initialized && Singleton<GameConfigurationManager>.Instance.HasData && dailyChallengeProgram.Initialized && (bool)Singleton<PredefinedRewards>.Instance)
		{
			return Singleton<PredefinedRewards>.Instance.Initialized;
		}
		return false;
	}

	private void Initialize()
	{
		if (!initialized)
		{
			Hashtable values = Singleton<GameConfigurationManager>.Instance.GetValues("daily_challenge");
			if (values != null)
			{
				challengeConfigs = new ChallengeConfigs(values);
			}
			if (Singleton<TimeManager>.Instance.HasTimer("daily_challenge_timer"))
			{
				Singleton<TimeManager>.Instance.Subscribe("daily_challenge_timer", OnDailyRewardEnded);
				dailyChallenges = LoadDailyChallenges();
			}
			else
			{
				CreateNewChallenges();
			}
			initialized = true;
			if (OnInitialize != null)
			{
				OnInitialize();
				OnInitialize = null;
			}
			StartCoroutine(WaitForTimeManager());
		}
	}

	private IEnumerator WaitForTimeManager()
	{
		while (!Singleton<TimeManager>.IsInstantiated())
		{
			yield return null;
		}
		if (Singleton<TimeManager>.Instance.Initialized)
		{
			AddNotifications();
		}
		else
		{
			Singleton<TimeManager>.Instance.OnInitialize += AddNotifications;
		}
	}

	private void AddNotifications()
	{
		Singleton<TimeManager>.Instance.OnInitialize -= AddNotifications;
	}

	private void OnDailyRewardEnded(int secondsAgo)
	{
		CreateNewChallenges();
		if (OnDailyChallengeChanged != null)
		{
			OnDailyChallengeChanged();
		}
	}

	private void OnDailyRewardCollected(int index)
	{
		if (dailyChallenges != null && index >= 0 && index < dailyChallenges.Length)
		{
			if (!FirstChallengeCollected && index == 0)
			{
				FirstChallengeCollected = true;
			}
			dailyChallenges[index].collected = true;
			SaveDailyChallenge(dailyChallenges[index], index);
		}
	}

	private int RewardExperience()
	{
		ResourceBar.Instance.ShowItem(ResourceBar.Item.PlayerProgress, showItem: true);
		int num = 0;
		for (int i = 0; i < dailyChallenges.Length; i++)
		{
			if (dailyChallenges[i].collected)
			{
				num++;
			}
		}
		PlayerProgress.ExperienceType experienceType = PlayerProgress.ExperienceType.DailyLootCrateCollected1st;
		switch (num)
		{
		case 2:
			experienceType = PlayerProgress.ExperienceType.DailyLootCrateCollected2nd;
			break;
		case 3:
			experienceType = PlayerProgress.ExperienceType.DailyLootCrateCollected3rd;
			break;
		}
		return Singleton<PlayerProgress>.Instance.AddExperience(experienceType);
	}

	private void CreateNewChallenges()
	{
		dailyChallenges = new ChallengeInfo[3];
		for (int i = 0; i < 3; i++)
		{
			dailyChallenges[i] = ChallengeInfo.Empty;
		}
		for (int j = 0; j < 3; j++)
		{
			dailyChallenges[j] = CreateNewChallenge(TodaysLootCrate(j));
		}
		SaveDailyChallenges(dailyChallenges);
		CreateNewTimer();
	}

	private ChallengeInfo CreateNewChallenge(LootCrateType type, int tryCount = 0)
	{
		if (tryCount > 100)
		{
			return default(ChallengeInfo);
		}
		float num = UnityEngine.Random.Range(0f, 100f);
		float unlockLevelChance = challengeConfigs.GetUnlockLevelChance(type);
		float num2 = unlockLevelChance + challengeConfigs.GetLockedLevelChance1(type);
		float num3 = num2 + challengeConfigs.GetLockedLevelChance2(type);
		float num4 = num3 + challengeConfigs.GetUnlockedJokerLevelChance(type);
		float num5 = num4 + challengeConfigs.GetLockedJokerLevelChanche(type);
		GetPossibleLevels(type, out var unlockedLevels, out var lockedLevels, out var lockedLevels2, out var unlockedJokerLevels, out var lockedJokerLevels);
		ChallengeInfo challengeInfo = ((num < unlockLevelChance && unlockedLevels.Count > 0) ? unlockedLevels[UnityEngine.Random.Range(0, unlockedLevels.Count)] : ((num < num2 && lockedLevels.Count > 0) ? lockedLevels[UnityEngine.Random.Range(0, lockedLevels.Count)] : ((num < num3 && lockedLevels2.Count > 0) ? lockedLevels2[UnityEngine.Random.Range(0, lockedLevels2.Count)] : ((num < num3 && lockedLevels2.Count <= 0 && lockedLevels.Count > 0) ? lockedLevels[UnityEngine.Random.Range(0, lockedLevels.Count)] : ((num < num3 && lockedLevels2.Count <= 0 && unlockedLevels.Count > 0) ? unlockedLevels[UnityEngine.Random.Range(0, unlockedLevels.Count)] : ((num < num4 && unlockedJokerLevels.Count > 0) ? unlockedJokerLevels[UnityEngine.Random.Range(0, unlockedJokerLevels.Count)] : ((num <= num5 && lockedJokerLevels.Count > 0) ? lockedJokerLevels[UnityEngine.Random.Range(0, lockedJokerLevels.Count)] : ((!(num <= num5) || lockedJokerLevels.Count > 0 || unlockedJokerLevels.Count <= 0) ? CreateNewChallenge(type, tryCount + 1) : unlockedJokerLevels[UnityEngine.Random.Range(0, unlockedJokerLevels.Count)]))))))));
		if (IsDailyChallenge(challengeInfo) && !string.IsNullOrEmpty(challengeInfo.levelName))
		{
			challengeInfo = CreateNewChallenge(type, tryCount);
		}
		DailyLevel daily = Singleton<GameManager>.instance.gameData.m_dailyChallengeData.GetDaily(challengeInfo.DailyKey);
		if (daily != null)
		{
			do
			{
				challengeInfo.positionIndex = UnityEngine.Random.Range(0, daily.Count);
			}
			while (!daily.ValidPositionIndex(challengeInfo.positionIndex));
		}
		return challengeInfo;
	}

	private void CreateNewTimer()
	{
		DateTime currentTime = Singleton<TimeManager>.Instance.CurrentTime;
		DateTime time = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day).AddDays(1.0);
		if (Singleton<TimeManager>.Instance.HasTimer("daily_challenge_timer"))
		{
			Singleton<TimeManager>.Instance.RemoveTimer("daily_challenge_timer");
		}
		Singleton<TimeManager>.Instance.CreateTimer("daily_challenge_timer", time, OnDailyRewardEnded);
	}

	private void GetPossibleLevels(LootCrateType type, out List<ChallengeInfo> unlockedLevels, out List<ChallengeInfo> lockedLevels1, out List<ChallengeInfo> lockedLevels2, out List<ChallengeInfo> unlockedJokerLevels, out List<ChallengeInfo> lockedJokerLevels)
	{
		int num = challengeConfigs.MaxUnlockDistance1(type);
		int num2 = challengeConfigs.MaxUnlockDistance2(type);
		unlockedLevels = new List<ChallengeInfo>();
		lockedLevels1 = new List<ChallengeInfo>();
		lockedLevels2 = new List<ChallengeInfo>();
		unlockedJokerLevels = new List<ChallengeInfo>();
		lockedJokerLevels = new List<ChallengeInfo>();
		if (!FirstChallengeCollected && !IsDailyChallenge(0, 0, 0))
		{
			unlockedLevels.Add(new ChallengeInfo(LevelInfo.GetLevelNames(0)[0], 0, 0));
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			List<string> levelNames = LevelInfo.GetLevelNames(i);
			int num3 = 0;
			for (int j = 0; j < levelNames.Count; j++)
			{
				if (LevelInfo.IsStarLevel(i, j) && LevelInfo.IsLevelUnlocked(i, j))
				{
					unlockedJokerLevels.Add(new ChallengeInfo(levelNames[j], i, j));
				}
				else if (LevelInfo.IsStarLevel(i, j) && !LevelInfo.IsLevelUnlocked(i, j))
				{
					num3++;
					lockedJokerLevels.Add(new ChallengeInfo(levelNames[j], i, j));
				}
				else if (LevelInfo.IsLevelUnlocked(i, j))
				{
					unlockedLevels.Add(new ChallengeInfo(levelNames[j], i, j));
				}
				else if (num3 < num)
				{
					num3++;
					lockedLevels1.Add(new ChallengeInfo(levelNames[j], i, j));
				}
				else
				{
					num3++;
					lockedLevels2.Add(new ChallengeInfo(levelNames[j], i, j));
				}
				if (num3 >= num && num3 >= num2)
				{
					break;
				}
			}
		}
	}

	public LootCrateType TodaysLootCrate(int index)
	{
		DateTime date = DateTime.Today;
		if (Singleton<TimeManager>.IsInstantiated())
		{
			date = Singleton<TimeManager>.Instance.CurrentTime;
		}
		if (overrideLoot)
		{
			return lootCrate;
		}
		return dailyChallengeProgram.GetLootCrateType(date, index);
	}

	public LootCrateType TomorrowsLootCrate(int index)
	{
		DateTime dateTime = DateTime.Today;
		if (Singleton<TimeManager>.IsInstantiated())
		{
			dateTime = Singleton<TimeManager>.Instance.CurrentTime;
		}
		return dailyChallengeProgram.GetLootCrateType(dateTime.AddDays(1.0), index);
	}

	public bool DailyChallengeCollected(int index)
	{
		return dailyChallenges[index].collected;
	}

	public bool AllLocationsRevealed()
	{
		for (int i = 0; i < dailyChallenges.Length; i++)
		{
			if (!dailyChallenges[i].revealed)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsLocationRevealed(int index)
	{
		return dailyChallenges[index].revealed;
	}

	public void SetLocationsRevealed()
	{
		for (int i = 0; i < dailyChallenges.Length; i++)
		{
			SetLocationRevealed(i);
		}
	}

	public void SetLocationRevealed(int index)
	{
		dailyChallenges[index].revealed = true;
		SaveDailyChallenge(dailyChallenges[index], index);
	}

	public void SetLocationAdRevealed(int index)
	{
		dailyChallenges[index].adRevealed = true;
		SaveDailyChallenge(dailyChallenges[index], index);
	}

	public bool IsDailyChallenge(ChallengeInfo info)
	{
		if (dailyChallenges == null)
		{
			return false;
		}
		for (int i = 0; i < dailyChallenges.Length; i++)
		{
			if (dailyChallenges[i].Equals(info))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsDailyChallenge(int epIndex, int lvlIndex, int posIndex)
	{
		if (dailyChallenges == null)
		{
			return false;
		}
		for (int i = 0; i < dailyChallenges.Length; i++)
		{
			if (dailyChallenges[i].episodeIndex == epIndex && dailyChallenges[i].levelIndex == lvlIndex && dailyChallenges[i].positionIndex == posIndex)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsDailyChallenge(string levelName, out int index)
	{
		index = -1;
		if (dailyChallenges == null)
		{
			return false;
		}
		for (int i = 0; i < dailyChallenges.Length; i++)
		{
			if (dailyChallenges[i].levelName == levelName)
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	public bool IsDailyChallenge(int episodeIndex, int levelIndex, out int index)
	{
		index = -1;
		if (dailyChallenges == null)
		{
			return false;
		}
		for (int i = 0; i < dailyChallenges.Length; i++)
		{
			if (dailyChallenges[i].episodeIndex == episodeIndex && dailyChallenges[i].levelIndex == levelIndex)
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	private void SaveDailyChallenges(ChallengeInfo[] infos)
	{
		if (infos != null)
		{
			for (int i = 0; i < infos.Length; i++)
			{
				SaveDailyChallenge(infos[i], i);
			}
		}
	}

	private void SaveDailyChallenge(ChallengeInfo info, int index)
	{
		GameProgress.SetString($"DailyChallenge_{index}_LevelName", info.levelName);
		GameProgress.SetInt($"DailyChallenge_{index}_EpisodeIndex", info.episodeIndex);
		GameProgress.SetInt($"DailyChallenge_{index}_LevelIndex", info.levelIndex);
		GameProgress.SetInt($"DailyChallenge_{index}_PositionIndex", info.positionIndex);
		GameProgress.SetBool($"DailyChallenge_{index}_Collected", info.collected);
		GameProgress.SetBool($"DailyChallenge_{index}_Revealed", info.revealed);
		GameProgress.SetBool($"DailyChallenge_{index}_AdRevealed", info.adRevealed);
	}

	private ChallengeInfo[] LoadDailyChallenges()
	{
		ChallengeInfo[] array = new ChallengeInfo[3];
		for (int i = 0; i < 3; i++)
		{
			array[i] = LoadDailyChallenge(i);
		}
		return array;
	}

	private ChallengeInfo LoadDailyChallenge(int index)
	{
		return new ChallengeInfo(GameProgress.GetString($"DailyChallenge_{index}_LevelName", string.Empty), GameProgress.GetInt($"DailyChallenge_{index}_EpisodeIndex", -1), GameProgress.GetInt($"DailyChallenge_{index}_LevelIndex", -1), GameProgress.GetInt($"DailyChallenge_{index}_PositionIndex", -1), GameProgress.GetBool($"DailyChallenge_{index}_Collected"), GameProgress.GetBool($"DailyChallenge_{index}_Revealed", Singleton<BuildCustomizationLoader>.Instance.IsOdyssey), GameProgress.GetBool($"DailyChallenge_{index}_AdRevealed"));
	}

	private void DeleteDailyChallenges()
	{
		for (int i = 0; i < 3; i++)
		{
			DeleteDailyChallenge(i);
		}
	}

	private void DeleteDailyChallenge(int index)
	{
		GameProgress.DeleteKey($"DailyChallenge_{index}_LevelName");
		GameProgress.DeleteKey($"DailyChallenge_{index}_EpisodeIndex");
		GameProgress.DeleteKey($"DailyChallenge_{index}_LevelIndex");
		GameProgress.DeleteKey($"DailyChallenge_{index}_PositionIndex");
		GameProgress.DeleteKey($"DailyChallenge_{index}_Collected");
		GameProgress.DeleteKey($"DailyChallenge_{index}_Revealed");
		GameProgress.DeleteKey($"DailyChallenge_{index}_AdRevealed");
	}

	public void SetDailyChallenge(int challengeIndex, int episodeIndex, int levelIndex)
	{
	}

	public void SetDailyLootCrate(LootCrateType type)
	{
	}

	public void ForceNewChallenge()
	{
	}

	private IEnumerator WaitFor(Func<bool> function, Action OnFinish)
	{
		while (!function())
		{
			yield return null;
		}
		OnFinish?.Invoke();
	}
}
