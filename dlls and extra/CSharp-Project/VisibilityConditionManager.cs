using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VisibilityConditionManager : Singleton<VisibilityConditionManager>
{
	private delegate bool ConditionCheck();

	public delegate void ConditionChange(VisibilityCondition.Condition condition, bool state);

	private LevelManager m_levelManager;

	private int conditionCount;

	private bool[] previousConditions;

	private ConditionCheck[] check;

	private ConditionChange[] events;

	private void Awake()
	{
		SetAsPersistant();
		conditionCount = Enum.GetNames(typeof(VisibilityCondition.Condition)).Length;
		previousConditions = new bool[conditionCount];
		InitializeConditions();
		InitializeEvents();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void Update()
	{
		for (int i = 1; i < conditionCount; i++)
		{
			if (events[i] != null && previousConditions[i] != check[i]())
			{
				previousConditions[i] = !previousConditions[i];
				events[i]((VisibilityCondition.Condition)i, previousConditions[i]);
			}
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (m_levelManager == null)
		{
			m_levelManager = UnityEngine.Object.FindObjectOfType<LevelManager>();
		}
		InitializeConditions();
	}

	public void SubscribeToConditionChange(ConditionChange onChange, VisibilityCondition.Condition condition)
	{
		if (condition == VisibilityCondition.Condition.None)
		{
			throw new ArgumentException($"Invalid Condition '{condition}'");
		}
		if (m_levelManager == null)
		{
			m_levelManager = UnityEngine.Object.FindObjectOfType<LevelManager>();
		}
		InitializeConditions();
		ConditionChange[] array = events;
		array[(int)condition] = (ConditionChange)Delegate.Combine(array[(int)condition], onChange);
		previousConditions[(int)condition] = check[(int)condition]();
		onChange(condition, previousConditions[(int)condition]);
	}

	public void UnsubscribeToConditionChange(ConditionChange onChange, VisibilityCondition.Condition condition)
	{
		ConditionChange[] array = events;
		array[(int)condition] = (ConditionChange)Delegate.Remove(array[(int)condition], onChange);
	}

	private void InitializeConditions()
	{
		check = new ConditionCheck[conditionCount];
		string autoBuild = Singleton<GameManager>.Instance.CurrentSceneName + "_autobuild_available";
		bool gameCenterAvailable = DeviceInfo.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios;
		bool chiefPigExploded = GameProgress.GetBool("ChiefPigExploded") || INSettings.GetBool(INFeature.EnableDesserts);
		bool iapEnabled = Singleton<BuildCustomizationLoader>.Instance.IAPEnabled;
		bool isCheat = Singleton<BuildCustomizationLoader>.Instance.CheatsEnabled;
		bool isDebug = Singleton<BuildCustomizationLoader>.Instance.IsDevelopmentBuild;
		bool lessCheats = Singleton<BuildCustomizationLoader>.Instance.LessCheats;
		bool boughtFoD = GameProgress.GetSandboxUnlocked("S-F");
		check[0] = () => false;
		check[1] = () => m_levelManager != null && m_levelManager.ContraptionProto != null && m_levelManager.ContraptionProto.ValidateContraption() && m_levelManager.gameState != LevelManager.GameState.AutoBuilding && m_levelManager.gameState != LevelManager.GameState.SuperAutoBuilding && m_levelManager.gameState != LevelManager.GameState.ShowingUnlockedParts;
		check[2] = () => m_levelManager != null && m_levelManager.ContraptionRunning != null && m_levelManager.ContraptionRunning.HasEngine && m_levelManager.ContraptionRunning.EnginePoweredPartTypeCount() > 1;
		check[3] = () => true;
		check[4] = () => m_levelManager != null && m_levelManager.gameState == LevelManager.GameState.PausedWhileRunning;
		check[5] = () => m_levelManager != null && m_levelManager.ContraptionProto.DynamicPartCount() > 0;
		check[6] = () => false;
		check[7] = () => true;
		check[8] = () => m_levelManager.gameState == LevelManager.GameState.PausedWhileRunning;
		check[9] = () => true;
		check[10] = () => true;
		check[11] = () => true;
		check[12] = () => true;
		check[13] = () => false;
		check[14] = () => m_levelManager != null && m_levelManager.m_autoBuildUnlocked && iapEnabled && m_levelManager.gameState != LevelManager.GameState.AutoBuilding && m_levelManager.gameState != LevelManager.GameState.SuperAutoBuilding && m_levelManager.gameState != LevelManager.GameState.ShowingUnlockedParts && !GameProgress.GetBool(autoBuild) && GameProgress.GetBool("PermanentBlueprint");
		check[15] = () => m_levelManager != null && m_levelManager.TutorialBookPage != null && m_levelManager.gameState != LevelManager.GameState.AutoBuilding && m_levelManager.gameState != LevelManager.GameState.SuperAutoBuilding && m_levelManager.gameState != LevelManager.GameState.ShowingUnlockedParts;
		check[16] = () => (m_levelManager != null && m_levelManager.gameState == LevelManager.GameState.AutoBuilding) || m_levelManager.gameState == LevelManager.GameState.SuperAutoBuilding;
		check[17] = () => m_levelManager != null && m_levelManager.ContraptionProto != null && m_levelManager.ContraptionProto.DynamicPartCount() > 0 && m_levelManager.gameState != LevelManager.GameState.AutoBuilding && m_levelManager.gameState != LevelManager.GameState.SuperAutoBuilding && m_levelManager.gameState != LevelManager.GameState.ShowingUnlockedParts;
		check[18] = () => m_levelManager != null && m_levelManager.gameState != LevelManager.GameState.AutoBuilding && m_levelManager.gameState != LevelManager.GameState.SuperAutoBuilding;
		check[19] = () => m_levelManager != null && m_levelManager.gameState != LevelManager.GameState.AutoBuilding && m_levelManager.gameState != LevelManager.GameState.SuperAutoBuilding && m_levelManager.gameState != LevelManager.GameState.ShowingUnlockedParts;
		check[20] = () => iapEnabled;
		check[21] = () => chiefPigExploded;
		check[22] = () => m_levelManager != null && GameProgress.GetBool(autoBuild) && m_levelManager.gameState != LevelManager.GameState.AutoBuilding && m_levelManager.gameState != LevelManager.GameState.SuperAutoBuilding;
		check[23] = () => m_levelManager != null && m_levelManager.m_sandbox;
		check[24] = () => m_levelManager != null && m_levelManager.m_sandbox && m_levelManager.gameState != LevelManager.GameState.AutoBuilding && m_levelManager.gameState != LevelManager.GameState.SuperAutoBuilding && m_levelManager.gameState != LevelManager.GameState.ShowingUnlockedParts;
		check[28] = () => gameCenterAvailable;
		check[29] = () => Singleton<BuildCustomizationLoader>.Instance.IsContentLimited;
		check[30] = () => Singleton<BuildCustomizationLoader>.Instance.IsHDVersion;
		check[31] = () => Singleton<BuildCustomizationLoader>.Instance.IsOdyssey;
		check[32] = () => DeviceInfo.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios;
		check[33] = () => isCheat;
		check[34] = () => isDebug;
		check[35] = () => FreeLootCrate.FreeShopLootCrateCollected;
		check[36] = CustomizationManager.HasNewParts;
		check[37] = () => lessCheats;
		check[38] = () => Singleton<NetworkManager>.IsInstantiated() && Singleton<NetworkManager>.Instance.HasNetworkAccess;
		check[39] = () => boughtFoD;
		check[40] = () => Singleton<DailyChallenge>.Instance.HasChallenge && Singleton<DailyChallenge>.Instance.Left == 0;
		check[41] = () => m_levelManager != null && m_levelManager.CurrentGameMode is CakeRaceMode;
		check[42] = () => Singleton<TimeManager>.IsInstantiated() && Singleton<TimeManager>.Instance.CurrentTime.Month == 12;
	}

	private void InitializeEvents()
	{
		events = new ConditionChange[conditionCount];
		for (int i = 1; i < conditionCount; i++)
		{
			events[i] = null;
		}
	}

	public bool GetState(VisibilityCondition.Condition condition)
	{
		return check[(int)condition]();
	}
}
