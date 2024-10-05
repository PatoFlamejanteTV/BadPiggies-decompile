using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	public enum GameState
	{
		Undefined,
		MainMenu,
		EpisodeSelection,
		LevelSelection,
		Level,
		Cutscene,
		CheatsPanel,
		StarLevelCutscene,
		SandboxLevelSelection,
		RaceLevelSelection,
		KingPigFeeding,
		WorkShop,
		CakeRaceMenu
	}

	public enum EpisodeType
	{
		Undefined,
		Normal,
		Sandbox,
		Race
	}

	[SerializeField]
	private GameData m_gameData;

	[SerializeField]
	private LevelRewardData m_levelRewardData;

	private GameState m_prevGameState;

	private GameState m_gameState;

	private GameState m_loadingLevelGameState;

	private int m_currentLevel;

	private int m_currentEpisodeIndex;

	private int m_pagesComingSoonBitmask;

	private EpisodeType m_currentEpisodeType;

	private string m_currentSandboxIdentifier;

	private string m_currentRaceLevelIdentifier;

	private string m_currentLevelName = string.Empty;

	private string m_currentEpisode = string.Empty;

	private List<EpisodeLevelInfo> m_levels = new List<EpisodeLevelInfo>();

	private SandboxSelector m_sandboxSelector;

	private List<int> m_starlevelLimits = new List<int>();

	private string m_openingCutscene;

	private string m_endingCutscene;

	private string m_midCutscene;

	private bool m_isCutsceneStartedFromLevelSelection;

	private bool m_openLevel;

	private int m_openLevelIndex;

	private const string StubLevelName = "LevelStub";

	public GameData gameData
	{
		get
		{
			if (INSettings.GetBool(INFeature.RuntimeGameData) && INRuntimeGameData.IsInitialized)
			{
				return Singleton<INRuntimeGameData>.Instance.GameData;
			}
			return m_gameData;
		}
	}

	public static bool Initialized
	{
		get
		{
			if (Singleton<GameManager>.instance != null)
			{
				return Singleton<GameManager>.instance.m_gameData != null;
			}
			return false;
		}
	}

	public EpisodeType CurrentEpisodeType => m_currentEpisodeType;

	public string CurrentEpisode => m_currentEpisode;

	public int CurrentEpisodeIndex => m_currentEpisodeIndex;

	public int CurrentLevel => m_currentLevel;

	public string CurrentLevelLabel
	{
		get
		{
			if (m_currentEpisodeType == EpisodeType.Normal)
			{
				return LevelSelector.DifferentiatedLevelLabel(m_currentLevel);
			}
			if (m_currentEpisodeType == EpisodeType.Sandbox)
			{
				return m_currentSandboxIdentifier.Substring(2);
			}
			if (m_currentEpisodeType == EpisodeType.Race)
			{
				return m_currentRaceLevelIdentifier.Substring(2);
			}
			return "X";
		}
	}

	public string CurrentEpisodeLabel
	{
		get
		{
			if (m_currentEpisodeType == EpisodeType.Normal)
			{
				for (int i = 0; i < m_gameData.m_episodeLevels.Count; i++)
				{
					if (m_gameData.m_episodeLevels[i].Name == m_currentEpisode)
					{
						return m_gameData.m_episodeLevels[i].Label;
					}
				}
			}
			else
			{
				if (m_currentEpisodeType == EpisodeType.Sandbox)
				{
					return "S";
				}
				if (m_currentEpisodeType == EpisodeType.Race)
				{
					return "R";
				}
			}
			return "X";
		}
	}

	public string CurrentLevelIdentifier
	{
		get
		{
			string empty = string.Empty;
			try
			{
				empty = ((m_gameState == GameState.CakeRaceMenu) ? "CakeRaceMenu" : ((m_gameState == GameState.WorkShop) ? "WorkShop" : ((m_gameState == GameState.KingPigFeeding) ? "Feed" : ((m_currentEpisodeType == EpisodeType.Normal) ? (CurrentEpisodeLabel + "-" + CurrentLevelLabel) : ((m_currentEpisodeType == EpisodeType.Sandbox) ? ((m_gameState != GameState.Level) ? m_currentEpisode : m_currentSandboxIdentifier) : ((m_currentEpisodeType != EpisodeType.Race) ? SceneManager.GetActiveScene().name : m_currentRaceLevelIdentifier))))));
			}
			catch
			{
				empty = SceneManager.GetActiveScene().name;
			}
			if (string.IsNullOrEmpty(empty))
			{
				return "none";
			}
			return empty;
		}
	}

	public string CurrentSceneName
	{
		get
		{
			if (m_gameState == GameState.Level)
			{
				return m_currentLevelName;
			}
			return SceneManager.GetActiveScene().name;
		}
	}

	public string ScreenFlurryIdentifier
	{
		get
		{
			if (m_gameState == GameState.Level)
			{
				return CurrentLevelIdentifier;
			}
			if (m_gameState == GameState.LevelSelection)
			{
				int num = 0;
				GameObject gameObject = GameObject.Find("LevelSelector");
				if ((bool)gameObject)
				{
					num = gameObject.GetComponent<LevelSelector>().CurrentPage;
				}
				string text = string.Empty;
				if (m_currentEpisodeType == EpisodeType.Normal)
				{
					text = m_gameData.m_episodeLevels[m_currentEpisodeIndex].FlurryID;
				}
				else if (m_currentEpisodeType == EpisodeType.Sandbox)
				{
					text = "SB";
				}
				else if (m_currentEpisodeType == EpisodeType.Race)
				{
					text = "Race";
				}
				return text + " " + num + "/3";
			}
			return GetGameState().ToString();
		}
	}

	public bool OverrideInFlightMusic => m_gameData.m_episodeLevels[m_currentEpisodeIndex].OverrideInFlightMusic;

	public GameObject OverriddenInFlightMusic
	{
		get
		{
			if (m_currentEpisodeType == EpisodeType.Normal)
			{
				return m_gameData.m_episodeLevels[m_currentEpisodeIndex].InFlightMusic;
			}
			return m_gameData.commonAudioCollection.InFlightMusic;
		}
	}

	public bool OverrideBuildMusic => m_gameData.m_episodeLevels[m_currentEpisodeIndex].OverrideBuildMusic;

	public GameObject OverriddenBuildMusic
	{
		get
		{
			if (m_currentEpisodeType == EpisodeType.Normal)
			{
				return m_gameData.m_episodeLevels[m_currentEpisodeIndex].BuildingMusic;
			}
			return m_gameData.commonAudioCollection.BuildMusic;
		}
	}

	public string OpeningCutscene => m_openingCutscene;

	public string EndingCutscene => m_endingCutscene;

	public string MidCutscene => m_midCutscene;

	public bool IsCutsceneStartedFromLevelSelection => m_isCutsceneStartedFromLevelSelection;

	public int LevelCount => m_levels.Count;

	public string SandboxToOpenForCurrentLevel
	{
		get
		{
			foreach (LevelRewardData.SandboxUnlock sandboxUnlock in m_levelRewardData.sandboxUnlocks)
			{
				if (sandboxUnlock.levelIdentifier == CurrentLevelIdentifier)
				{
					return sandboxUnlock.sandboxIdentifier;
				}
			}
			return string.Empty;
		}
	}

	public bool IsInGame()
	{
		return m_gameState == GameState.Level;
	}

	public GameState GetPrevGameState()
	{
		return m_prevGameState;
	}

	public GameState GetGameState()
	{
		return m_gameState;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (m_openLevel)
		{
			LoadLevel(m_openLevelIndex);
			m_openLevel = false;
		}
	}

	public bool IsNextPageComingSoon()
	{
		if (CurrentLevel + 1 != 14 || m_pagesComingSoonBitmask <= 0)
		{
			if (CurrentLevel + 1 == 29)
			{
				return m_pagesComingSoonBitmask > 1;
			}
			return false;
		}
		return true;
	}

	public void SetLoadingLevelGameState(GameState state)
	{
		m_loadingLevelGameState = state;
		new GameObject().AddComponent<LevelUnloadNotifier>();
	}

	public void OpenEpisode(LevelSelector episodeLevels)
	{
		m_currentEpisode = SceneManager.GetActiveScene().name;
		m_currentEpisodeType = EpisodeType.Normal;
		m_currentEpisodeIndex = episodeLevels.EpisodeIndex;
		m_openingCutscene = episodeLevels.OpeningCutscene;
		m_midCutscene = episodeLevels.MidCutscene;
		m_endingCutscene = episodeLevels.EndingCutscene;
		m_levels = new List<EpisodeLevelInfo>(episodeLevels.Levels);
		m_starlevelLimits = new List<int>(episodeLevels.StarLevelLimits);
		m_pagesComingSoonBitmask = Convert.ToInt32(episodeLevels.m_pageTwoComingSoon) + Convert.ToInt32(episodeLevels.m_pageThreeComingSoon);
	}

	public void OpenSandboxEpisode(SandboxSelector sandboxLevels)
	{
		m_currentEpisode = SceneManager.GetActiveScene().name;
		m_sandboxSelector = sandboxLevels;
		m_currentEpisodeType = EpisodeType.Sandbox;
	}

	public void OpenRaceEpisode(RaceLevelSelector raceLevels)
	{
		m_currentEpisode = SceneManager.GetActiveScene().name;
		m_currentEpisodeType = EpisodeType.Race;
	}

	public void CloseEpisode()
	{
		m_currentEpisode = null;
		m_levels.Clear();
	}

	public void OnLevelUnloading()
	{
		if (m_loadingLevelGameState != 0)
		{
			m_prevGameState = m_gameState;
			m_gameState = m_loadingLevelGameState;
			m_loadingLevelGameState = GameState.Undefined;
		}
	}

	public void LoadMainMenu(bool showLoadingScreen)
	{
		Singleton<Loader>.Instance.LoadLevel("MainMenu", GameState.MainMenu, showLoadingScreen);
	}

	public void LoadKingPigFeed(bool showLoadingScreen)
	{
		Singleton<Loader>.Instance.LoadLevel("KingPigFeed", GameState.KingPigFeeding, showLoadingScreen);
	}

	public void LoadWorkshop(bool showLoadingScreen)
	{
		Singleton<Loader>.Instance.LoadLevel("Workshop", GameState.WorkShop, showLoadingScreen);
	}

	public int NextLevel()
	{
		int num = m_currentLevel;
		if (m_currentLevel < m_levels.Count - 1 && m_levels[m_currentLevel + 1] == GetCurrentRowJokerLevel())
		{
			num++;
		}
		if (m_currentLevel < m_levels.Count - 1)
		{
			num++;
		}
		return num;
	}

	public void LoadNextLevel()
	{
		if (m_currentEpisodeType == EpisodeType.Race)
		{
			LoadRaceLevel(NextRaceLevel());
			return;
		}
		int num = NextLevel();
		bool flag = LevelInfo.IsLevelUnlocked(CurrentEpisodeIndex, num);
		bool flag2 = true;
		bool flag3 = false;
		bool num2 = num % 5 == 0;
		int num3 = num / 5;
		bool flag4 = GameProgress.IsLevelAdUnlocked(LevelInfo.GetLevelNames(CurrentEpisodeIndex)[num3 * 5]);
		bool flag5 = LevelInfo.IsLevelUnlocked(CurrentEpisodeIndex, num3 * 5);
		if (num2 && !flag4 && !flag5)
		{
			EventManager.Send(new UIEvent(UIEvent.Type.LevelSelection));
		}
		else if (LevelInfo.IsContentLimited(CurrentEpisodeIndex, num) && Singleton<BuildCustomizationLoader>.Instance.IsChina && GameProgress.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_stars") != 3 && GameProgress.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_stars") != 0)
		{
			LevelInfo.DisplayContentLimitNotification();
			GameProgress.SetBool("show_content_limit_popup", value: true);
			EventManager.Send(new UIEvent(UIEvent.Type.LevelSelection));
		}
		else if (flag3 && !flag2 && flag)
		{
			EventManager.Send(new UIEvent(UIEvent.Type.LevelSelection));
		}
		else
		{
			if (Singleton<BuildCustomizationLoader>.Instance.IsChina && GameProgress.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_stars") == 3 && GameProgress.GetMinimumLockedLevel(CurrentEpisodeIndex) <= CurrentLevel + 1)
			{
				GameProgress.SetMinimumLockedLevel(CurrentEpisodeIndex, GameProgress.GetMinimumLockedLevel(CurrentEpisodeIndex) + 1);
			}
			m_currentLevel = num;
			LoadLevel(CurrentLevel);
		}
		if (m_currentEpisode != string.Empty)
		{
			int num4 = m_currentLevel / 15;
			if (m_currentLevel / 5 % 3 == 2 && m_currentLevel % 5 == 3 && !flag2)
			{
				num4++;
			}
			UserSettings.SetInt(m_currentEpisode + "_active_page", num4);
		}
	}

	public void LoadNextLevelAfterCutScene()
	{
		LoadLevel(NextLevel());
	}

	public LevelLoader CurrentLevelLoader()
	{
		string text;
		if (m_currentEpisodeType == EpisodeType.Sandbox)
		{
			text = ((!(m_sandboxSelector == null)) ? m_sandboxSelector.FindLevel(m_currentSandboxIdentifier).m_levelLoaderPath.Remove(0, "Assets/Resources/".Length) : GetSandboxLevelData(m_currentSandboxIdentifier).m_levelLoaderPath.Remove(0, "Assets/Resources/".Length));
		}
		else if (m_currentEpisodeType == EpisodeType.Race)
		{
			text = gameData.FindRaceLevel(m_currentRaceLevelIdentifier).m_levelLoaderPath.Remove(0, "Assets/Resources/".Length);
		}
		else
		{
			if (m_currentLevel >= m_levels.Count)
			{
				return null;
			}
			text = m_levels[m_currentLevel].levelLoaderPath.Remove(0, "Assets/Resources/".Length);
		}
		int startIndex = text.LastIndexOf('.');
		text = text.Remove(startIndex);
		return ((GameObject)Resources.Load(text, typeof(GameObject))).GetComponent<LevelLoader>();
	}

	public void LoadLevel(int index)
	{
		m_currentLevel = index;
		m_currentLevelName = m_levels[index].sceneName;
		Singleton<Loader>.Instance.LoadLevel("LevelStub", GameState.Level, showLoadingScreen: true);
	}

	public void LoadLevel(string sceneName)
	{
		for (int i = 0; i < m_levels.Count; i++)
		{
			if (m_levels[i].sceneName == sceneName)
			{
				LoadLevel(i);
				break;
			}
		}
	}

	public void LoadSandboxLevel(string sandboxIdentifier, int sandboxBundleIndex = 0)
	{
		m_currentSandboxIdentifier = sandboxIdentifier;
		if (m_sandboxSelector == null)
		{
			m_currentLevelName = GetSandboxLevelData(sandboxIdentifier).SceneName;
			m_currentEpisodeType = EpisodeType.Sandbox;
		}
		else
		{
			m_currentLevelName = m_sandboxSelector.FindLevelFile(sandboxIdentifier);
		}
		Singleton<Loader>.Instance.LoadLevel("LevelStub", GameState.Level, showLoadingScreen: true);
	}

	private SandboxLevels.LevelData GetSandboxLevelData(string identifier)
	{
		return m_gameData.m_sandboxLevels.GetLevelData(identifier);
	}

	public void LoadRaceLevel(string raceLevelIdentifier)
	{
		m_currentRaceLevelIdentifier = raceLevelIdentifier;
		m_currentLevelName = gameData.FindRaceLevel(raceLevelIdentifier).SceneName;
		m_currentEpisodeType = EpisodeType.Race;
		Singleton<Loader>.Instance.LoadLevel("LevelStub", GameState.Level, showLoadingScreen: true);
	}

	public void LoadRaceLevelFromLevelCompleteMenu(string raceLevelIdentifier)
	{
		RaceLevels.LevelData levelData = gameData.m_raceLevels.GetLevelData(raceLevelIdentifier);
		int levelIndex = gameData.m_raceLevels.GetLevelIndex(raceLevelIdentifier);
		if (LevelInfo.IsContentLimited(-1, levelIndex))
		{
			GameProgress.SetBool("show_content_limit_popup", value: true);
			EventManager.Send(new UIEvent(UIEvent.Type.EpisodeSelection));
			return;
		}
		m_currentRaceLevelIdentifier = raceLevelIdentifier;
		m_currentLevelName = levelData.SceneName;
		m_currentEpisode = "RaceLevelSelection";
		m_currentEpisodeType = EpisodeType.Race;
		Singleton<Loader>.Instance.LoadLevel("LevelStub", GameState.Level, showLoadingScreen: true);
	}

	public void LoadUnlockedLevelFromLevelCompleteMenu(string levelName)
	{
		m_currentLevel = GetCurrentRowJokerLevelIndex();
		m_currentLevelName = levelName;
		Singleton<Loader>.Instance.LoadLevel(levelName, GameState.Level, showLoadingScreen: true);
	}

	public void LoadStarLevelTransition(string sceneName)
	{
		for (int i = 0; i < m_levels.Count; i++)
		{
			if (m_levels[i].sceneName == sceneName)
			{
				LoadStarLevelTransition(m_levels[i]);
				break;
			}
		}
	}

	public void LoadStarLevelTransition(EpisodeLevelInfo level)
	{
		if (gameData.m_episodeLevels[m_currentEpisodeIndex].m_showStarLevelTransition)
		{
			m_currentLevel = m_levels.IndexOf(level);
			m_currentLevelName = level.sceneName;
			Singleton<Loader>.Instance.LoadLevel("StarLevelTransition", GameState.StarLevelCutscene, showLoadingScreen: true);
		}
		else
		{
			LoadLevel(level.sceneName);
		}
	}

	public void LoadLevelAfterCutScene(EpisodeLevelInfo level, string cutScene)
	{
		m_currentLevel = m_levels.IndexOf(level);
		m_currentLevelName = level.sceneName;
		Singleton<Loader>.Instance.LoadLevel(cutScene, GameState.Cutscene, showLoadingScreen: true);
	}

	public void LoadStarLevelAfterTransition()
	{
		Singleton<Loader>.Instance.LoadLevel("LevelStub", GameState.Level, showLoadingScreen: true);
	}

	public void LoadSandboxLevelSelection()
	{
		GameTime.Pause(pause: false);
		UserSettings.SetBool(CompactEpisodeSelector.IsEpisodeToggledKey, value: true);
		Singleton<Loader>.Instance.LoadLevel("EpisodeSelection", GameState.EpisodeSelection, showLoadingScreen: true);
	}

	public void LoadRaceLevelSelection()
	{
		GameTime.Pause(pause: false);
		Singleton<Loader>.Instance.LoadLevel("RaceLevelSelection", GameState.RaceLevelSelection, showLoadingScreen: true);
	}

	public void LoadSandboxLevelSelectionAndOpenIapMenu()
	{
		LoadSandboxLevelSelection();
	}

	public void ReloadCurrentLevel(bool showLoadingScreen)
	{
		GameProgress.SetString("REPLAY_LEVEL", SceneManager.GetActiveScene().name);
		Singleton<Loader>.Instance.LoadLevel(SceneManager.GetActiveScene().name, m_gameState, showLoadingScreen);
	}

	public void LoadOpeningCutscene()
	{
		Singleton<Loader>.Instance.LoadLevel(OpeningCutscene, GameState.Cutscene, showLoadingScreen: true);
	}

	public void LoadMidCutscene(bool isStartedFromLevelSelection = false)
	{
		m_isCutsceneStartedFromLevelSelection = isStartedFromLevelSelection;
		Singleton<Loader>.Instance.LoadLevel(MidCutscene, GameState.Cutscene, showLoadingScreen: true);
	}

	public void LoadEndingCutscene()
	{
		if (HasMidCutsceneEnabled())
		{
			LoadMidCutscene();
		}
		else
		{
			Singleton<Loader>.Instance.LoadLevel(EndingCutscene, GameState.Cutscene, showLoadingScreen: true);
		}
	}

	public void LoadEpisodeSelection(bool showLoadingScreen)
	{
		Singleton<Loader>.Instance.LoadLevel("EpisodeSelection", GameState.EpisodeSelection, showLoadingScreen);
	}

	public void LoadCakeRaceMenu(bool showLoadingScreen = false)
	{
		Singleton<Loader>.Instance.LoadLevel("CakeRaceMenu", GameState.CakeRaceMenu, showLoadingScreen);
	}

	public void LoadLevelSelection(string episode, bool showLoadingScreen)
	{
		Singleton<Loader>.Instance.LoadLevel(episode, GameState.LevelSelection, showLoadingScreen);
	}

	public void LoadLevelSelectionAndLevel(string episode, int levelIndex)
	{
		m_openLevel = true;
		m_openLevelIndex = levelIndex;
		Singleton<Loader>.Instance.LoadLevel(episode, GameState.LevelSelection, showLoadingScreen: true, enableGUIAfterLoad: false);
	}

	public void LoadCheatsPanel()
	{
		Singleton<Loader>.Instance.LoadLevel("CheatsPanel", GameState.CheatsPanel, showLoadingScreen: true);
	}

	public bool CurrentStarLevelUnlocked()
	{
		int num = 0;
		int num2 = m_currentLevel / 5 * 5;
		if (m_levels.Count < 5)
		{
			return false;
		}
		for (int i = num2; i < num2 + 4; i++)
		{
			num += GameProgress.GetInt(m_levels[i].sceneName + "_stars");
		}
		return num >= m_starlevelLimits[(num2 + 4) / 5];
	}

	public bool CurrentEpisodeThreeStarred()
	{
		if (m_currentEpisodeType == EpisodeType.Normal)
		{
			bool flag = true;
			for (int i = 0; i < m_levels.Count && flag; i++)
			{
				flag &= GameProgress.GetInt(m_levels[i].sceneName + "_stars") == 3;
			}
			return flag;
		}
		if (m_currentEpisodeType == EpisodeType.Race)
		{
			bool flag2 = true;
			for (int j = 0; j < gameData.m_raceLevels.Levels.Count; j++)
			{
				flag2 &= GameProgress.GetInt(gameData.m_raceLevels.Levels[j].SceneName + "_stars") == 3;
			}
			return flag2;
		}
		return false;
	}

	public bool CurrentEpisodeThreeStarredNormalLevels()
	{
		if (m_currentEpisodeType != EpisodeType.Normal)
		{
			return false;
		}
		if (!IsEpisodeCompletable())
		{
			return false;
		}
		bool flag = true;
		for (int i = 0; i < m_levels.Count && flag; i++)
		{
			flag &= GameProgress.GetInt(m_levels[i].sceneName + "_stars") == 3;
		}
		return flag;
	}

	public bool CurrentEpisodeThreeStarredSpecialLevels()
	{
		if (m_currentEpisodeType != EpisodeType.Normal)
		{
			return false;
		}
		if (!IsEpisodeCompletable())
		{
			return false;
		}
		bool flag = true;
		for (int i = 4; i < m_levels.Count && flag; i += 5)
		{
			flag &= GameProgress.GetInt(m_levels[i].sceneName + "_stars") == 3;
		}
		return flag;
	}

	private bool IsEpisodeCompletable()
	{
		return m_levels.Count == gameData.m_episodeLevels[CurrentEpisodeIndex].TotalLevelCount;
	}

	public EpisodeLevelInfo GetCurrentRowJokerLevel()
	{
		int num = m_currentLevel / 5 * 5;
		if (m_levels.Count >= 5)
		{
			return m_levels[num + 4];
		}
		return null;
	}

	public int GetCurrentRowJokerLevelIndex()
	{
		return m_currentLevel / 5 * 5 + 4;
	}

	public void InitializeTestLevelState()
	{
		m_currentLevelName = SceneManager.GetActiveScene().name;
		LevelLoader[] array = UnityEngine.Object.FindObjectsOfType<LevelLoader>();
		if (array.Length != 0)
		{
			m_currentLevelName = array[0].SceneName;
		}
		LevelManager[] array2 = UnityEngine.Object.FindObjectsOfType<LevelManager>();
		if (array2.Length == 0)
		{
			return;
		}
		m_gameState = GameState.Level;
		m_currentEpisodeType = EpisodeType.Normal;
		LevelManager levelManager = array2[0];
		if (levelManager.m_sandbox)
		{
			m_currentEpisodeType = EpisodeType.Sandbox;
		}
		else if (levelManager.m_raceLevel)
		{
			m_currentEpisodeType = EpisodeType.Race;
		}
		if (m_currentEpisodeType == EpisodeType.Sandbox)
		{
			foreach (SandboxLevels.LevelData level in gameData.m_sandboxLevels.Levels)
			{
				if (level.SceneName == m_currentLevelName)
				{
					m_currentSandboxIdentifier = level.m_identifier;
					break;
				}
			}
			return;
		}
		if (m_currentEpisodeType == EpisodeType.Race)
		{
			foreach (RaceLevels.LevelData level2 in gameData.m_raceLevels.Levels)
			{
				if (level2.SceneName == m_currentLevelName)
				{
					m_currentRaceLevelIdentifier = level2.m_identifier;
					break;
				}
			}
			return;
		}
		for (int i = 0; i < gameData.m_episodeLevels.Count; i++)
		{
			List<EpisodeLevelInfo> levelInfos = gameData.m_episodeLevels[i].LevelInfos;
			for (int j = 0; j < levelInfos.Count; j++)
			{
				if (levelInfos[j].sceneName == m_currentLevelName)
				{
					m_currentEpisodeIndex = i;
					m_currentEpisode = m_gameData.m_episodeLevels[m_currentEpisodeIndex].Name;
					m_currentLevel = j;
					break;
				}
			}
		}
	}

	private void Awake()
	{
		SetAsPersistant();
		if (Bundle.initialized)
		{
			gameData.commonAudioCollection.Initialize();
		}
		else
		{
			Bundle.AssetBundlesLoaded = (Action)Delegate.Combine(Bundle.AssetBundlesLoaded, (Action)delegate
			{
				gameData.commonAudioCollection.Initialize();
			});
		}
		EventManager.Connect<UIEvent>(OnReceivedUIEvent);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<UIEvent>(OnReceivedUIEvent);
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnApplicationFocus(bool focus)
	{
		Resources.UnloadUnusedAssets();
	}

	private IEnumerator Start()
	{
		while (!Bundle.initialized)
		{
			yield return null;
		}
	}

	public void Quit(string message)
	{
		Application.Quit();
	}

	private void OnReceivedUIEvent(UIEvent data)
	{
		if (data.type == UIEvent.Type.ClosedLootWheel && (!Singleton<IapManager>.IsInstantiated() || !Singleton<IapManager>.Instance.IsShopPageOpened()) && !SnoutCoinShopPopup.DialogOpen)
		{
			int requiredLevel = PlayerLevelRequirement.GetRequiredLevel("cake_race");
			if (requiredLevel >= 0 && Singleton<PlayerProgress>.Instance.Level >= requiredLevel && !GameProgress.HasKey("CakeRaceUnlockShown"))
			{
				GameProgress.SetBool("CakeRaceUnlockShown", value: false);
				UnityEngine.Object.Instantiate(gameData.m_cakeRaceUnlockedPopup).transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 2f;
			}
		}
	}

	public void CreateMenuBackground()
	{
		string @string = GameProgress.GetString("MenuBackground", string.Empty);
		GameObject gameObject = null;
		if (@string != string.Empty)
		{
			gameObject = (GameObject)Resources.Load("Environment/Background/" + @string, typeof(GameObject));
		}
		if (!gameObject)
		{
			gameObject = (GameObject)Resources.Load("Environment/Background/Background_Jungle_01_SET", typeof(GameObject));
		}
		UnityEngine.Object.Instantiate(gameObject, Vector3.forward * 10f, Quaternion.identity);
	}

	public string NextRaceLevel()
	{
		for (int i = 0; i < gameData.m_raceLevels.Levels.Count - 1; i++)
		{
			if (gameData.m_raceLevels.Levels[i].m_identifier == m_currentRaceLevelIdentifier)
			{
				return gameData.m_raceLevels.Levels[i + 1].m_identifier;
			}
		}
		return "UndefinedRaceLevel";
	}

	public string CurrentLevelLeaderboard()
	{
		RaceLevels.LevelData levelData = gameData.FindRaceLevel(m_currentRaceLevelIdentifier);
		if (levelData != null)
		{
			return levelData.m_leaderboardId;
		}
		return string.Empty;
	}

	public bool IsLastLevelInEpisode()
	{
		return m_currentEpisodeType switch
		{
			EpisodeType.Race => m_currentRaceLevelIdentifier == gameData.m_raceLevels.Levels[gameData.m_raceLevels.Levels.Count - 1].m_identifier, 
			EpisodeType.Normal => m_currentLevel == m_levels.Count - 2, 
			_ => false, 
		};
	}

	public bool HasMidCutsceneEnabled()
	{
		if (CurrentEpisodeIndex == 4 || CurrentEpisodeIndex == 5)
		{
			return m_currentLevel == 13;
		}
		return false;
	}

	public bool HasNextLevel()
	{
		switch (m_currentEpisodeType)
		{
		case EpisodeType.Race:
			return !IsLastLevelInEpisode();
		default:
			return false;
		case EpisodeType.Normal:
			if ((CurrentLevel + 1) % 5 != 0 && !IsNextPageComingSoon())
			{
				return !IsLastLevelInEpisode();
			}
			return false;
		}
	}

	public bool HasCutScene()
	{
		switch (m_currentEpisodeType)
		{
		default:
			return false;
		case EpisodeType.Sandbox:
		case EpisodeType.Race:
			return false;
		case EpisodeType.Normal:
			if (!IsLastLevelInEpisode() || string.IsNullOrEmpty(m_endingCutscene))
			{
				if (HasMidCutsceneEnabled())
				{
					return !string.IsNullOrEmpty(m_midCutscene);
				}
				return false;
			}
			return true;
		}
	}
}
