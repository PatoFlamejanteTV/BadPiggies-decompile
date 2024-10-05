using System;
using System.Collections.Generic;

public class PlayerProgress : Singleton<PlayerProgress>
{
	[Flags]
	public enum ExperienceType
	{
		UnknownSource = -1,
		LevelComplete1Star = 0,
		LevelComplete2Star = 1,
		LevelComplete3Star = 2,
		JokerLevelComplete1Star = 3,
		JokerLevelComplete2Star = 4,
		JokerLevelComplete3Star = 5,
		StarBoxCollectedSandbox = 6,
		PartBoxCollectedSandbox = 7,
		KingBurp = 8,
		HiddenSkullFound = 9,
		AllHiddenSkullsFound = 0xA,
		HiddenStatueFound = 0xB,
		AllHiddenStatuesFound = 0xC,
		CommonPartCrafted = 0xD,
		RarePartCrafted = 0xE,
		EpicPartCrafted = 0xF,
		NewCommonPartViewed = 0x10,
		NewRarePartViewed = 0x11,
		NewEpicPartViewed = 0x12,
		EveryplayVideoShared = 0x13,
		WinCakeRace = 0x14,
		LoseCakeRace = 0x15,
		DailyLootCrateCollected1st = 0x16,
		DailyLootCrateCollected2nd = 0x17,
		DailyLootCrateCollected3rd = 0x18,
		LegendaryPartCrafted = 0x19,
		NewLegendaryPartViewed = 0x1A
	}

	private Dictionary<int, int> experienceTable;

	public const string EXPERIENCE_KEY = "player_experience";

	public const string PENDING_EXPERIENCE_KEY = "player_pending_experience";

	public const string LEVEL_KEY = "player_level";

	private const string EXPERIENCE_TABLE_KEY = "player_experience_table";

	private const string EXPERIENCE_TYPES_KEY = "player_experience_types";

	private static Dictionary<ExperienceType, int> pendingExperienceTypes;

	public bool Initialized { get; private set; }

	public int Level { get; private set; }

	public int Experience { get; private set; }

	public int PendingExperience { get; private set; }

	public int MaxLevel { get; private set; }

	public bool LevelUpPending => PendingExperience >= 0;

	public int ExperienceToNextLevel
	{
		get
		{
			if (experienceTable == null)
			{
				return Experience;
			}
			if (experienceTable.ContainsKey(Level + 1))
			{
				return experienceTable[Level + 1];
			}
			if (experienceTable.ContainsKey(MaxLevel))
			{
				return experienceTable[MaxLevel];
			}
			return Experience;
		}
	}

	private void Awake()
	{
		SetAsPersistant();
		LoadData();
		EventManager.Connect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnPlayerChanged(PlayerChangedEvent data)
	{
		LoadData();
	}

	private void Start()
	{
		if (Singleton<GameConfigurationManager>.Instance.HasData)
		{
			UpdateExperienceTable();
			return;
		}
		GameConfigurationManager gameConfigurationManager = Singleton<GameConfigurationManager>.Instance;
		gameConfigurationManager.OnHasData = (Action)Delegate.Combine(gameConfigurationManager.OnHasData, new Action(UpdateExperienceTable));
	}

	public void Initialize(int level, int experience, int pendingExperience)
	{
		Level = level;
		Experience = experience;
		PendingExperience = pendingExperience;
		SaveData();
		UpdateExperienceTable();
		FirePlayerProgressEvent();
	}

	private void LoadData()
	{
		PendingExperience = GameProgress.GetInt("player_pending_experience", -1);
		Experience = GameProgress.GetInt("player_experience");
		Level = GameProgress.GetInt("player_level", 1);
		FirePlayerProgressEvent();
	}

	private void SaveData()
	{
		GameProgress.SetInt("player_level", Level);
		GameProgress.SetInt("player_experience", Experience);
		GameProgress.SetInt("player_pending_experience", PendingExperience);
	}

	private void UpdateExperienceTable()
	{
		GameConfigurationManager gameConfigurationManager = Singleton<GameConfigurationManager>.Instance;
		gameConfigurationManager.OnHasData = (Action)Delegate.Remove(gameConfigurationManager.OnHasData, new Action(UpdateExperienceTable));
		ConfigData config = Singleton<GameConfigurationManager>.Instance.GetConfig("player_experience_table");
		if (config == null || config.Count == 0)
		{
			return;
		}
		MaxLevel = 0;
		experienceTable = new Dictionary<int, int>();
		for (int i = 0; i < config.Count; i++)
		{
			if (int.TryParse(config.Keys[i], out var result) && int.TryParse(config.Values[i], out var result2))
			{
				if (!experienceTable.ContainsKey(result))
				{
					experienceTable.Add(result, result2);
				}
				if (result > MaxLevel)
				{
					MaxLevel = result;
				}
			}
		}
		if (pendingExperienceTypes != null && pendingExperienceTypes.Count > 0)
		{
			foreach (KeyValuePair<ExperienceType, int> pendingExperienceType in pendingExperienceTypes)
			{
				for (int j = 0; j < pendingExperienceType.Value; j++)
				{
					AddExperience(pendingExperienceType.Key);
				}
			}
		}
		FirePlayerProgressEvent();
	}

	public void FirePlayerProgressEvent()
	{
		AddExperience(ExperienceType.UnknownSource);
	}

	public int AddExperience(ExperienceType experienceType)
	{
		int num = ExperienceCount(experienceType);
		AddExperience(num);
		SendFlurryExperienceGained(num, experienceType);
		return num;
	}

	public int AddExperience(ExperienceType[] experienceType)
	{
		int num = 0;
		for (int i = 0; i < experienceType.Length; i++)
		{
			num += AddExperience(experienceType[i]);
		}
		return num;
	}

	private int ExperienceCount(ExperienceType experienceType)
	{
		int num = 0;
		if (experienceType == ExperienceType.UnknownSource)
		{
			return num;
		}
		if (Singleton<GameConfigurationManager>.Instance.HasData && Singleton<GameConfigurationManager>.Instance.HasConfig("player_experience_types"))
		{
			string key = experienceType.ToString();
			ConfigData config = Singleton<GameConfigurationManager>.Instance.GetConfig("player_experience_types");
			if (config.HasKey(key) && int.TryParse(config[key], out var result))
			{
				num = result;
			}
		}
		if (Singleton<DoubleRewardManager>.IsInstantiated() && Singleton<DoubleRewardManager>.Instance.CurrentStatus == DoubleRewardManager.Status.Initialized && Singleton<DoubleRewardManager>.Instance.HasDoubleReward)
		{
			return num * 2;
		}
		return num;
	}

	public void CheckLevelUp()
	{
		if (LevelUpPending)
		{
			int pendingExperience = PendingExperience;
			PendingExperience = -1;
			Level++;
			EventManager.Send(new LevelUpEvent(Level));
			AddExperience(pendingExperience);
			SendFlurryLevelUpGained();
		}
	}

	private void AddExperience(int amount)
	{
		int experienceToNextLevel = ExperienceToNextLevel;
		if (LevelUpPending)
		{
			PendingExperience += amount;
		}
		else
		{
			Experience += amount;
			if (experienceTable != null && Experience >= experienceToNextLevel)
			{
				PendingExperience = Experience - experienceToNextLevel;
				Experience = 0;
			}
		}
		SaveData();
		int experience = Experience;
		if (LevelUpPending)
		{
			experience = experienceToNextLevel;
		}
		EventManager.Send(new PlayerProgressEvent(Level, experience, experienceToNextLevel, PendingExperience));
	}

	public static void AddPendingExperience(Dictionary<ExperienceType, int> experiences)
	{
		if (pendingExperienceTypes == null)
		{
			pendingExperienceTypes = new Dictionary<ExperienceType, int>();
		}
		foreach (KeyValuePair<ExperienceType, int> experience in experiences)
		{
			if (experience.Value > 0)
			{
				if (pendingExperienceTypes.ContainsKey(experience.Key))
				{
					pendingExperienceTypes[experience.Key] += experience.Value;
				}
				else
				{
					pendingExperienceTypes.Add(experience.Key, experience.Value);
				}
			}
		}
	}

	private void SendFlurryExperienceGained(int amount, ExperienceType typeOfGain)
	{
	}

	private void SendFlurryLevelUpGained()
	{
	}
}
