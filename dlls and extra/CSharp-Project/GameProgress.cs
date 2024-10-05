using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GameProgress : MonoBehaviour
{
	[Flags]
	public enum Location
	{
		Local = 1,
		Hatch = 2
	}

	public enum ButtonUnlockState
	{
		Locked,
		Unlocked,
		UnlockNow
	}

	public static Action OnScrapAmountChanged;

	public static Action OnProgressSaved;

	private const Location DEFAULT_SAVE_LOCATION = Location.Local;

	private const Location DEFAULT_LOAD_LOCATION = Location.Local;

	private const string NULL_VALUE = "NULL";

	[SerializeField]
	private GameData m_gameData;

	private static GameProgress instance;

	private static SettingsData m_data;

	private static string m_currentPlayer;

	private static bool m_lastSaveOk = true;

	private static bool EnableAutoSave { get; set; }

	public static bool Initialized
	{
		get
		{
			if (instance != null)
			{
				return instance.m_gameData != null;
			}
			return false;
		}
	}

	private void Awake()
	{
		m_gameData = Singleton<GameManager>.Instance.gameData;
		instance = this;
		m_currentPlayer = null;
		ChangePlayer(string.Empty);
		UnityEngine.Object.DontDestroyOnLoad(this);
		EventManager.Connect<LoadLevelEvent>(ReceiveLoadingLevelEvent);
		HatchManager.onLogout = (Action)Delegate.Combine(HatchManager.onLogout, new Action(OnLogoutActions));
		EnableAutoSave = true;
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<LoadLevelEvent>(ReceiveLoadingLevelEvent);
		HatchManager.onLogout = (Action)Delegate.Remove(HatchManager.onLogout, new Action(OnLogoutActions));
	}

	private void Update()
	{
		if (EnableAutoSave && m_data.ChangedSinceLastSave && m_lastSaveOk)
		{
			m_lastSaveOk = m_data.Save();
		}
	}

	private void OnLogoutActions()
	{
		EnableAutoSave = false;
	}

	public static void ChangePlayer(string identifier = "")
	{
		if (m_currentPlayer == null || !(m_currentPlayer == identifier))
		{
			bool num = m_currentPlayer == null || !m_currentPlayer.Equals(identifier);
			m_currentPlayer = identifier;
			string path = "Progress.dat";
			string key = "56SA%FG42Dv5#4aG67f2";
			if (!string.IsNullOrEmpty(m_currentPlayer))
			{
				path = $"Progress_{m_currentPlayer}.dat";
				key = string.Format("{0}{1}", m_currentPlayer, "z9dD2wS2,h");
			}
			m_data = new SettingsData(Path.Combine(Application.persistentDataPath, path), useEncryption: true, key);
			m_data.Load();
			bool isNewGameProgress = false;
			if (!m_data.GetBool("GameProgress_initialized", defaultValue: false))
			{
				isNewGameProgress = true;
				InitializeGameProgressData();
			}
			instance.UpgradeDataFormatVersion(isNewGameProgress);
			if (num)
			{
				Save();
				EventManager.Send(new PlayerChangedEvent(m_currentPlayer));
			}
			EnableAutoSave = true;
		}
	}

	public static void InitializeGameProgressData()
	{
		if (!GetRaceLevelUnlocked("R-1"))
		{
			SetRaceLevelUnlocked("R-1", unlocked: true);
			SetButtonUnlockState("RaceLevelButton_R-1", ButtonUnlockState.Unlocked);
		}
		m_data.SetBool("GameProgress_initialized", value: true);
		m_data.SetString("InstallVersion", Singleton<BuildCustomizationLoader>.Instance.ApplicationVersion);
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			m_lastSaveOk = m_data.Save();
		}
	}

	public static string GetInstallVersionString()
	{
		return GetString("InstallVersion", "1.8.0");
	}

	public static void SetFullVersionUnlocked(bool unlock)
	{
		m_data.SetBool("FullVersionUnlocked", unlock);
	}

	public static bool HasStarterPack()
	{
		return m_data.GetBool("StarterPack", defaultValue: false);
	}

	public static void SetStarterPack(bool unlock)
	{
		if (!GetPermanentBlueprint())
		{
			SetPermanentBlueprint(unlock);
		}
		if (unlock && !GetSandboxUnlocked("S-F"))
		{
			SetSandboxUnlocked("S-F", unlock);
			UnlockButton("EpisodeButtonSandbox");
		}
		m_data.SetBool("StarterPack", unlock);
	}

	public static bool GetFullVersionUnlocked()
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IsContentLimited)
		{
			return m_data.GetBool("FullVersionUnlocked", defaultValue: false);
		}
		return true;
	}

	public static void SetLevelCompleted(string levelName)
	{
		m_data.SetInt(levelName + "_completed", 1);
	}

	public static bool IsLevelCompleted(string levelName)
	{
		return m_data.GetInt(levelName + "_completed", 0) == 1;
	}

	[Obsolete("Obsolete in 2.2.0")]
	public static void SetLevelAdUnlocked(string levelName)
	{
		m_data.SetInt(levelName + "_ad_unlocked", 1);
	}

	public static bool IsLevelAdUnlocked(string levelName)
	{
		return m_data.GetInt(levelName + "_ad_unlocked", 0) == 1;
	}

	public static void SetShowRowUnlockStarEffect(int episodeIndex, int row, bool show)
	{
		m_data.SetBool("star_effect_ep_" + episodeIndex + "_row_" + row, show);
	}

	public static bool GetShowRowUnlockStarEffect(int episodeIndex, int row)
	{
		return m_data.GetBool("star_effect_ep_" + episodeIndex + "_row_" + row, defaultValue: false);
	}

	public static bool IsChallengeCompleted(string levelName, int challengeNumber)
	{
		return m_data.GetInt(levelName + "_challenge_" + challengeNumber, 0) > 0;
	}

	public static void SetChallengeCompletedWithoutCoins(string levelName, int challengeNumber)
	{
		m_data.SetInt(levelName + "_challenge_" + challengeNumber, 1);
	}

	public static void SetChallengeCompleted(string levelName, int challengeNumber, bool completed, bool snoutCoinsCollected = true)
	{
		bool isOdyssey = Singleton<BuildCustomizationLoader>.Instance.IsOdyssey;
		if (completed && !isOdyssey)
		{
			m_data.SetInt(levelName + "_challenge_" + challengeNumber, (!snoutCoinsCollected) ? 1 : 2);
		}
		else if (completed && isOdyssey)
		{
			m_data.SetInt(levelName + "_challenge_" + challengeNumber, 1);
		}
		else
		{
			m_data.SetInt(levelName + "_challenge_" + challengeNumber, 0);
		}
	}

	public static void SetDoubleRewardStartTime(int serverTime)
	{
		m_data.SetInt("double_reward_start_time", serverTime);
	}

	public static int GetDoubleRewardStartTime()
	{
		return m_data.GetInt("double_reward_start_time", -1);
	}

	public static bool HasCollectedSnoutCoins(string levelName, int challengeNumber)
	{
		return m_data.GetInt(levelName + "_challenge_" + challengeNumber, 0) >= 2;
	}

	public static bool IsEpisodeThreeStarred(string episode)
	{
		return m_data.GetBool(episode + "_3starred", defaultValue: false);
	}

	public static void SetEpisodeThreeStarred(string episode, bool completed)
	{
		m_data.SetBool(episode + "_3starred", completed);
	}

	public static void SetSandboxUnlocked(string sandboxIdentifier, bool unlocked)
	{
		m_data.SetBool("sandbox_unlocked_" + sandboxIdentifier, unlocked);
	}

	public static bool GetSandboxUnlocked(string sandboxIdentifier)
	{
		if (INSettings.GetBool(INFeature.UnlockSandBoxes))
		{
			return true;
		}
		return m_data.GetBool("sandbox_unlocked_" + sandboxIdentifier, defaultValue: false);
	}

	public static void SetRaceLevelUnlocked(string levelIdentifier, bool unlocked)
	{
		m_data.SetBool("race_level_unlocked_" + levelIdentifier, unlocked);
	}

	public static bool GetRaceLevelUnlocked(string levelIdentifier)
	{
		if (INSettings.GetBool(INFeature.UnlockLevels))
		{
			return true;
		}
		return m_data.GetBool("race_level_unlocked_" + levelIdentifier, defaultValue: false);
	}

	public static void SetRaceLevelPartUnlocked(string levelIdentifier, string partName, bool unlocked)
	{
		m_data.SetBool("race_level_part_unlocked_" + levelIdentifier + "_" + partName, unlocked);
	}

	public static bool GetRaceLevelPartUnlocked(string levelIdentifier, string partName)
	{
		if (INSettings.GetBool(INFeature.UnlockLevels))
		{
			return true;
		}
		return m_data.GetBool("race_level_part_unlocked_" + levelIdentifier + "_" + partName, defaultValue: false);
	}

	public static void UnlockButton(string buttonId)
	{
		if (GetButtonUnlockState(buttonId) == ButtonUnlockState.Locked)
		{
			SetButtonUnlockState(buttonId, ButtonUnlockState.UnlockNow);
		}
	}

	public static void SetButtonUnlockState(string buttonId, ButtonUnlockState state)
	{
		m_data.SetInt("button_unlock_" + buttonId, (int)state);
	}

	public static ButtonUnlockState GetButtonUnlockState(string buttonId)
	{
		return (ButtonUnlockState)m_data.GetInt("button_unlock_" + buttonId, 0);
	}

	public static int GetTutorialBookOpenCount()
	{
		return m_data.GetInt("TutorialBookOpenCount", 0);
	}

	public static void IncreaseTutorialBookOpenCount()
	{
		int tutorialBookOpenCount = GetTutorialBookOpenCount();
		tutorialBookOpenCount++;
		m_data.SetInt("TutorialBookOpenCount", tutorialBookOpenCount);
	}

	public static int GetTutorialBookLastOpenedPage()
	{
		return m_data.GetInt("TutorialBookLastOpenedPage", 0);
	}

	public static void SetTutorialBookLastOpenedPage(int page)
	{
		m_data.SetInt("TutorialBookLastOpenedPage", page);
	}

	public static int GetTutorialBookFirstOpenedPage()
	{
		return m_data.GetInt("TutorialBookFirstOpenedPage", 1000000);
	}

	public static void SetTutorialBookFirstOpenedPage(int page)
	{
		m_data.SetInt("TutorialBookFirstOpenedPage", page);
	}

	public static void AddSandboxStar(string levelName, string starName, bool snoutCoinsCollected = false)
	{
		string key = levelName + "_star_" + starName;
		int @int = m_data.GetInt(key, 0);
		if (@int == 0)
		{
			m_data.SetInt(key, (!snoutCoinsCollected) ? 1 : 2);
			string key2 = levelName + "_stars";
			int int2 = m_data.GetInt(key2, 0);
			int2++;
			m_data.SetInt(key2, int2);
		}
		else if (@int == 1 && snoutCoinsCollected)
		{
			m_data.SetInt(key, 2);
		}
	}

	public static int GetSandboxStarCollectCount(string levelName, string starName)
	{
		string key = levelName + "_star_" + starName;
		return m_data.GetInt(key, 0);
	}

	public static void AddPartBox(string levelName, string partBoxName)
	{
		string key = levelName + "_part_" + partBoxName;
		if (m_data.GetInt(key, 0) == 0)
		{
			m_data.SetInt(key, 1);
			string key2 = levelName + "_parts";
			int @int = m_data.GetInt(key2, 0);
			m_data.SetInt(key2, @int + 1);
		}
	}

	public static bool HasSandboxStar(string levelName, string starName)
	{
		string key = levelName + "_star_" + starName;
		return m_data.GetInt(key, 0) > 0;
	}

	public static bool HasPartBox(string levelName, string partBoxName)
	{
		string key = levelName + "_part_" + partBoxName;
		return m_data.GetInt(key, 0) > 0;
	}

	public static int SandboxStarCount(string levelName)
	{
		string key = levelName + "_stars";
		return m_data.GetInt(key, 0);
	}

	public static bool HasBestTime(string levelName)
	{
		return m_data.HasKey(levelName + "_time");
	}

	public static void SetBestTime(string levelName, float time)
	{
		m_data.SetFloat(levelName + "_time", time);
	}

	public static float GetBestTime(string levelName)
	{
		return m_data.GetFloat(levelName + "_time", 10000f);
	}

	public static void AddSecretSkull()
	{
		string key = "SkullsCollected";
		int @int = m_data.GetInt(key, 0);
		@int++;
		m_data.SetInt(key, @int);
	}

	public static int MaxSkullCount()
	{
		return 45;
	}

	public static int SecretSkullCount()
	{
		string key = "SkullsCollected";
		return m_data.GetInt(key, 0);
	}

	public static void AddSecretStatue()
	{
		string key = "StatuesFound";
		int @int = m_data.GetInt(key, 0);
		@int++;
		m_data.SetInt(key, @int);
	}

	public static int MaxStatueCount()
	{
		return 15;
	}

	public static int SecretStatueCount()
	{
		string key = "StatuesFound";
		return m_data.GetInt(key, 0);
	}

	public static int BluePrintCount()
	{
		if (INSettings.GetBool(INFeature.InfiniteTools))
		{
			return 1061109567;
		}
		return m_data.GetInt("Blueprints_Available", 0);
	}

	public static void SetBluePrintCount(int count)
	{
		m_data.SetInt("Blueprints_Available", count);
	}

	public static void AddBluePrints(int count)
	{
		int @int = m_data.GetInt("Blueprints_Available", 0);
		@int += count;
		m_data.SetInt("Blueprints_Available", @int);
	}

	public static int NightVisionCount()
	{
		if (INSettings.GetBool(INFeature.InfiniteTools))
		{
			return 1061109567;
		}
		return m_data.GetInt("NightVisions_Available", 0);
	}

	public static void SetNightVisionCount(int count)
	{
		m_data.SetInt("NightVisions_Available", count);
	}

	public static void AddNightVision(int count)
	{
		int @int = m_data.GetInt("NightVisions_Available", 0);
		@int += count;
		m_data.SetInt("NightVisions_Available", @int);
	}

	public static void SetPermanentBlueprint(bool unlock)
	{
		m_data.SetBool("PermanentBlueprint", unlock);
	}

	public static bool GetPermanentBlueprint()
	{
		return m_data.GetBool("PermanentBlueprint", defaultValue: false);
	}

	public static int SuperGlueCount()
	{
		if (INSettings.GetBool(INFeature.InfiniteTools))
		{
			return 1061109567;
		}
		return m_data.GetInt("SuperGlue_Available", 0);
	}

	public static void SetSuperGlueCount(int count)
	{
		m_data.SetInt("SuperGlue_Available", count);
	}

	public static void AddSuperGlue(int count)
	{
		int @int = m_data.GetInt("SuperGlue_Available", 0);
		@int += count;
		m_data.SetInt("SuperGlue_Available", @int);
	}

	public static int SuperMagnetCount()
	{
		if (INSettings.GetBool(INFeature.InfiniteTools))
		{
			return 1061109567;
		}
		return m_data.GetInt("SuperMagnet_Available", 0);
	}

	public static void SetSuperMagnetCount(int count)
	{
		m_data.SetInt("SuperMagnet_Available", count);
	}

	public static void AddSuperMagnet(int count)
	{
		int @int = m_data.GetInt("SuperMagnet_Available", 0);
		@int += count;
		m_data.SetInt("SuperMagnet_Available", @int);
	}

	public static int TurboChargeCount()
	{
		if (INSettings.GetBool(INFeature.InfiniteTools))
		{
			return 1061109567;
		}
		return m_data.GetInt("TurboCharge_Available", 0);
	}

	public static void SetTurboChargeCount(int count)
	{
		m_data.SetInt("TurboCharge_Available", count);
	}

	public static void AddTurboCharge(int count)
	{
		int @int = m_data.GetInt("TurboCharge_Available", 0);
		@int += count;
		m_data.SetInt("TurboCharge_Available", @int);
	}

	public static int SnoutCoinCount()
	{
		if (INSettings.GetBool(INFeature.InfiniteSnoutCoins))
		{
			return 1061109567;
		}
		return m_data.GetInt("SnoutCoins", 0);
	}

	public static void SetSnoutCoinCount(int count)
	{
		m_data.SetInt("SnoutCoins", count);
	}

	public static void AddSnoutCoins(int count)
	{
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			int @int = m_data.GetInt("SnoutCoins", 0);
			@int += count;
			m_data.SetInt("SnoutCoins", @int);
		}
	}

	public static bool UseSnoutCoins(int amount)
	{
		int @int = m_data.GetInt("SnoutCoins", 0);
		if (@int >= amount)
		{
			@int -= amount;
			m_data.SetInt("SnoutCoins", @int);
			return true;
		}
		return false;
	}

	public static int ScrapCount()
	{
		return m_data.GetInt("Scrap", 0);
	}

	public static void SetScrapCount(int count)
	{
		m_data.SetInt("Scrap", count);
		if (OnScrapAmountChanged != null)
		{
			OnScrapAmountChanged();
		}
	}

	public static void AddScrap(int count)
	{
		int @int = m_data.GetInt("Scrap", 0);
		@int += count;
		m_data.SetInt("Scrap", @int);
		if (OnScrapAmountChanged != null)
		{
			OnScrapAmountChanged();
		}
	}

	public static bool UseScrap(int amount)
	{
		int @int = m_data.GetInt("Scrap", 0);
		if (@int >= amount)
		{
			@int -= amount;
			m_data.SetInt("Scrap", @int);
			if (OnScrapAmountChanged != null)
			{
				OnScrapAmountChanged();
			}
			return true;
		}
		return false;
	}

	public static int AncientPiggiesRevealed()
	{
		return m_data.GetInt("Ancient_Pigs_Revealed", 0);
	}

	public static void AddAncientPiggiesRevealed(int count)
	{
		int @int = m_data.GetInt("Ancient_Pigs_Revealed", 0);
		@int += count;
		m_data.SetInt("Ancient_Pigs_Revealed", @int);
	}

	public static int DessertCount(string dessertName)
	{
		return m_data.GetInt(dessertName, 0);
	}

	public static void SetDessertCount(string dessertName, int count)
	{
		m_data.SetInt(dessertName, count);
	}

	public static void EatDesserts(string dessertName, int count)
	{
		if (count > 0)
		{
			int num = DessertCount(dessertName);
			int num2 = num - count;
			if (num2 < 0)
			{
				num2 = 0;
				AddEatenDesserts(num);
			}
			else
			{
				AddEatenDesserts(count);
			}
			SetDessertCount(dessertName, num2);
		}
	}

	public static void AddDesserts(string dessertName, int count)
	{
		if (count > 0)
		{
			if (!m_data.GetBool("ChiefPigExploded", defaultValue: false))
			{
				m_data.SetBool("ChiefPigExploded", value: true);
			}
			int num = DessertCount(dessertName);
			SetDessertCount(dessertName, num + count);
			m_data.SetInt("TotalDessertCount", m_data.GetInt("TotalDessertCount", 0) + count);
		}
	}

	public static int EatenDessertsCount()
	{
		return m_data.GetInt("EatenDessertsCount", 0);
	}

	private static void SetEatenDessertsCount(int count)
	{
		m_data.SetInt("EatenDessertsCount", count);
	}

	private static void AddEatenDesserts(int count)
	{
		int num = EatenDessertsCount();
		SetEatenDessertsCount(num + count);
		num = m_data.GetInt("TotalDessertCount", 0);
		num -= count;
		m_data.SetInt("TotalDessertCount", (num >= 0) ? num : 0);
	}

	public static int TotalDessertCount()
	{
		return m_data.GetInt("TotalDessertCount", 0);
	}

	public static int AddSandboxParts(BasePart.PartType part, int count, bool showUnlockAnimations = true)
	{
		if (showUnlockAnimations)
		{
			m_data.AddToInt("part_unlocked_" + part, count);
		}
		return m_data.AddToInt("part_" + part, count);
	}

	public static int AddSandboxParts(string levelName, BasePart.PartType part, int count, bool showUnlockAnimations = true)
	{
		if (showUnlockAnimations)
		{
			m_data.AddToInt(levelName + "_part_unlocked_" + part, count);
		}
		return m_data.AddToInt(levelName + "_part_" + part, count);
	}

	public static int GetSandboxPartCount(BasePart.PartType part)
	{
		return m_data.GetInt("part_" + part, 0);
	}

	public static int GetSandboxPartCount(string levelName, BasePart.PartType part)
	{
		string key = levelName + "_part_" + part;
		return m_data.GetInt(key, 0);
	}

	public static int GetUnlockedSandboxPartCount(BasePart.PartType part)
	{
		return m_data.GetInt("part_unlocked_" + part, 0);
	}

	public static int GetUnlockedSandboxPartCount(string levelName, BasePart.PartType part)
	{
		string key = levelName + "_part_unlocked_" + part;
		return m_data.GetInt(key, 0);
	}

	public static void SetUnlockedSandboxPartCount(BasePart.PartType part, int count)
	{
		m_data.SetInt("part_unlocked_" + part, count);
	}

	public static void SetUnlockedSandboxPartCount(string levelName, BasePart.PartType part, int count)
	{
		string key = levelName + "_part_unlocked_" + part;
		m_data.SetInt(key, count);
	}

	public static int GetRaceLevelUnlockedStars()
	{
		string key = "race_level_unlocked_stars";
		return m_data.GetInt(key, 0);
	}

	public static void SetRaceLevelUnlockedStars(int value)
	{
		string key = "race_level_unlocked_stars";
		m_data.SetInt(key, value);
	}

	public static void AddRaceLevelUnlockedStars(int value)
	{
		string key = "race_level_unlocked_stars";
		int @int = m_data.GetInt(key, 0);
		m_data.SetInt(key, @int + value);
	}

	public static bool AllLevelsUnlocked()
	{
		if (INSettings.GetBool(INFeature.UnlockLevels))
		{
			return true;
		}
		return m_data.GetBool("UnlockAllLevels", defaultValue: false);
	}

	public static bool AllFreeLevelsUnlocked()
	{
		return m_data.GetBool("UnlockAllFreeLevels", defaultValue: false);
	}

	public static string[] GetTimerIds()
	{
		string @string = m_data.GetString("TimerIds", string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			return new string[0];
		}
		return @string.Split(',');
	}

	public static void SetTimerIds(string[] ids)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < ids.Length; i++)
		{
			stringBuilder.Append(ids[i]);
			if (i < ids.Length - 1)
			{
				stringBuilder.Append(',');
			}
		}
		m_data.SetString("TimerIds", stringBuilder.ToString());
	}

	public static T GetTimerData<T>(string timerId, string timerValue)
	{
		return m_data.Get($"{timerId}_{timerValue}", default(T));
	}

	public static void SetTimerData<T>(string timerId, string timerValue, T value)
	{
		m_data.Set($"{timerId}_{timerValue}", value);
	}

	public static void RemoveTimerData(string timerId, string timerValue)
	{
		m_data.DeleteKey($"{timerId}_{timerValue}");
	}

	public static void SetInt(string key, int value, Location location = Location.Local)
	{
		if ((location & Location.Local) == Location.Local)
		{
			m_data.SetInt(key, value);
		}
	}

	public static int GetInt(string key, int defaultValue = 0, Location location = Location.Local, Action<int> response = null)
	{
		int @int = m_data.GetInt(key, defaultValue);
		response?.Invoke(@int);
		return @int;
	}

	public static void SetFloat(string key, float value, Location location = Location.Local)
	{
		m_data.SetFloat(key, value);
	}

	public static float GetFloat(string key, float defaultValue = 0f, Location location = Location.Local, Action<float> response = null)
	{
		float @float = m_data.GetFloat(key, defaultValue);
		response?.Invoke(@float);
		return @float;
	}

	public static void SetString(string key, string value, Location location = Location.Local)
	{
		m_data.SetString(key, value);
	}

	public static string GetString(string key, string defaultValue = "", Location location = Location.Local, Action<string> response = null)
	{
		string @string = m_data.GetString(key, defaultValue);
		response?.Invoke(@string);
		return @string;
	}

	public static void SetBool(string key, bool value, Location location = Location.Local)
	{
		m_data.SetBool(key, value);
	}

	public static bool GetBool(string key, bool defaultValue = false, Location location = Location.Local, Action<bool> response = null)
	{
		bool @bool = m_data.GetBool(key, defaultValue);
		response?.Invoke(@bool);
		return @bool;
	}

	public static bool HasKey(string key, Location location = Location.Local, Action<bool> response = null)
	{
		bool flag = m_data.HasKey(key);
		response?.Invoke(flag);
		return flag;
	}

	public static void DeleteKey(string key, Location location = Location.Local)
	{
		m_data.DeleteKey(key);
	}

	public static void DeleteAll()
	{
		m_data.DeleteAll();
	}

	public static void Save()
	{
		m_lastSaveOk = m_data.Save();
	}

	public static void Load()
	{
		m_data.Load();
	}

	private void UpgradeDataFormatVersion(bool isNewGameProgress = false)
	{
		int num = GetInt("DataFormatVersion", 1);
		if (num == 1)
		{
			for (int i = 1; i <= 4; i++)
			{
				if (m_data.GetBool("sandbox_unlocked_" + i, defaultValue: false))
				{
					m_data.SetBool("sandbox_unlocked_S-" + i, value: true);
					m_data.DeleteKey("sandbox_unlocked_" + i);
				}
				if (GetButtonUnlockState("SandboxLevelButton_" + i) == ButtonUnlockState.Unlocked)
				{
					SetButtonUnlockState("SandboxLevelButton_S-" + i, ButtonUnlockState.Unlocked);
					m_data.DeleteKey("button_unlock_SandboxLevelButton_" + i);
				}
			}
			num = 2;
			SetInt("DataFormatVersion", num);
		}
		if (num == 2)
		{
			UserSettings.SetInt("Episode2LevelSelection_active_page", 0);
			num = 3;
			SetInt("DataFormatVersion", num);
		}
		if (num == 3)
		{
			num = 4;
			SetInt("DataFormatVersion", num);
		}
		if (num == 4)
		{
			try
			{
				for (int j = 0; j < 6; j++)
				{
					List<string> levelNames = LevelInfo.GetLevelNames(j);
					for (int k = 0; k < levelNames.Count; k++)
					{
						if (m_data.GetBool(levelNames[k] + "_challenge_" + 1, defaultValue: false))
						{
							m_data.DeleteKey(levelNames[k] + "_challenge_" + 1);
							m_data.SetInt(levelNames[k] + "_challenge_" + 1, 1);
						}
						if (m_data.GetBool(levelNames[k] + "_challenge_" + 2, defaultValue: false))
						{
							m_data.DeleteKey(levelNames[k] + "_challenge_" + 2);
							m_data.SetInt(levelNames[k] + "_challenge_" + 2, 1);
						}
					}
				}
				foreach (RaceLevels.LevelData level in Singleton<GameManager>.Instance.gameData.m_raceLevels.Levels)
				{
					string sceneName = level.SceneName;
					if (m_data.GetBool(sceneName + "_challenge_" + 1, defaultValue: false))
					{
						m_data.DeleteKey(sceneName + "_challenge_" + 1);
						m_data.SetInt(sceneName + "_challenge_" + 1, 1);
					}
					if (m_data.GetBool(sceneName + "_challenge_" + 2, defaultValue: false))
					{
						m_data.DeleteKey(sceneName + "_challenge_" + 2);
						m_data.SetInt(sceneName + "_challenge_" + 2, 1);
					}
				}
			}
			catch
			{
			}
			num = 5;
			SetInt("DataFormatVersion", num);
		}
		if (num == 5)
		{
			int[] array = new int[3];
			int[] array2 = new int[3];
			int num2 = 0;
			int num3 = 0;
			int num4 = SecretSkullCount();
			int num5 = SecretStatueCount();
			foreach (Episode episodeLevel in instance.m_gameData.m_episodeLevels)
			{
				int num6 = 0;
				foreach (EpisodeLevelInfo levelInfo in episodeLevel.LevelInfos)
				{
					int @int = GetInt(levelInfo.sceneName + "_stars");
					bool flag = LevelInfo.IsStarLevel(num3, num6);
					for (int l = 0; l < Mathf.Clamp(@int, 0, 3); l++)
					{
						if (flag)
						{
							array2[l]++;
						}
						else
						{
							array[l]++;
						}
					}
					num6++;
				}
				num3++;
			}
			foreach (SandboxLevels.LevelData level2 in instance.m_gameData.m_sandboxLevels.Levels)
			{
				num2 += GetInt(level2.SceneName + "_stars");
			}
			foreach (RaceLevels.LevelData level3 in instance.m_gameData.m_raceLevels.Levels)
			{
				int int2 = GetInt(level3.SceneName + "_stars");
				for (int m = 0; m < Mathf.Clamp(int2, 0, 3); m++)
				{
					array[m]++;
				}
			}
			Dictionary<PlayerProgress.ExperienceType, int> dictionary = new Dictionary<PlayerProgress.ExperienceType, int>();
			int num7 = 0;
			int num8 = 3;
			for (int n = 0; n < 3; n++)
			{
				if (array[n] > 0)
				{
					dictionary.Add((PlayerProgress.ExperienceType)(num7 + n), array[n]);
				}
				if (array2[n] > 0)
				{
					dictionary.Add((PlayerProgress.ExperienceType)(num8 + n), array2[n]);
				}
			}
			if (num2 > 0)
			{
				dictionary.Add(PlayerProgress.ExperienceType.StarBoxCollectedSandbox, num2);
			}
			if (num4 > 0)
			{
				dictionary.Add(PlayerProgress.ExperienceType.HiddenSkullFound, num4);
				if (num4 >= MaxSkullCount())
				{
					dictionary.Add(PlayerProgress.ExperienceType.AllHiddenSkullsFound, 1);
				}
			}
			if (num5 > 0)
			{
				dictionary.Add(PlayerProgress.ExperienceType.HiddenStatueFound, num5);
				if (num4 >= MaxStatueCount())
				{
					dictionary.Add(PlayerProgress.ExperienceType.AllHiddenStatuesFound, 1);
				}
			}
			PlayerProgress.AddPendingExperience(dictionary);
			num = 6;
			SetInt("DataFormatVersion", num);
		}
		SetString("LastKnownVersion", Singleton<BuildCustomizationLoader>.Instance.ApplicationVersion);
	}

	public static string GetLastKnownVersionString()
	{
		return GetString("LastKnownVersion", "1.8.0");
	}

	public static int GetAllStars()
	{
		int num = 0;
		foreach (Episode episodeLevel in instance.m_gameData.m_episodeLevels)
		{
			foreach (EpisodeLevelInfo levelInfo in episodeLevel.LevelInfos)
			{
				num += m_data.GetInt(levelInfo.sceneName + "_stars", 0);
			}
		}
		foreach (SandboxLevels.LevelData level in instance.m_gameData.m_sandboxLevels.Levels)
		{
			num += m_data.GetInt(level.SceneName + "_stars", 0);
		}
		foreach (RaceLevels.LevelData level2 in instance.m_gameData.m_raceLevels.Levels)
		{
			num += m_data.GetInt(level2.SceneName + "_stars", 0);
		}
		return num;
	}

	private void ReceiveLoadingLevelEvent(LoadLevelEvent data)
	{
		if (data.nextGameState == GameManager.GameState.Level)
		{
			m_data.BackupData();
		}
	}

	public static int ExperienceGiven(PlayerProgress.ExperienceType expType, string identifier = "")
	{
		return m_data.GetInt($"exp_{expType.ToString()}{identifier}", 0);
	}

	public static void ReportExperienceGiven(PlayerProgress.ExperienceType expType, string identifier = "")
	{
		string key = $"exp_{expType.ToString()}{identifier}";
		int @int = m_data.GetInt(key, 0);
		m_data.SetInt(key, @int + 1);
	}

	public static void SetMinimumLockedLevel(int episode, int level)
	{
		m_data.SetInt("MinimumLockedLevel" + episode, level);
	}

	public static int GetMinimumLockedLevel(int episode)
	{
		return m_data.GetInt("MinimumLockedLevel" + episode, 6);
	}

	public static int GetLootcrateAmount(LootCrateType crateType)
	{
		return m_data.GetInt($"Crate_amount_{crateType.ToString()}", 0);
	}

	public static void AddLootcrate(LootCrateType crateType, int amount = 1)
	{
		m_data.AddToInt($"Crate_amount_{crateType.ToString()}", amount);
	}

	public static void RemoveLootcrate(LootCrateType crateType)
	{
		m_data.AddToInt($"Crate_amount_{crateType.ToString()}", -1, 0);
	}
}
