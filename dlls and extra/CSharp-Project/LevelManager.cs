using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LevelManager : WPFMonoBehaviour
{
	[Serializable]
	public class CameraLimits
	{
		public Vector2 topLeft;

		public Vector2 size;
	}

	[Serializable]
	public class ScoreLimits
	{
		public int silverScore;

		public int goldScore;
	}

	[Serializable]
	public class PartCount
	{
		public BasePart.PartType type;

		public int count;
	}

	public enum GameState
	{
		Undefined,
		Building,
		Preview,
		PreviewMoving,
		PreviewWhileBuilding,
		PreviewWhileRunning,
		Running,
		Continue,
		Completed,
		PausedWhileRunning,
		PausedWhileBuilding,
		IngamePurchase,
		AutoBuilding,
		TutorialBook,
		ShowingUnlockedParts,
		Purchasing,
		Snapshot,
		MechanicInfoScreen,
		MechanicGiftScreen,
		SuperAutoBuilding,
		CustomizingPart,
		LootCrateOpening,
		CakeRaceExploding,
		CakeRaceCompleted
	}

	private struct DessertPlacePair
	{
		public GameObject dessert;

		public Transform place;
	}

	[HideInInspector]
	public int m_constructionUiRows = 1;

	[HideInInspector]
	public List<int> m_constructionGridRows = new List<int>();

	[HideInInspector]
	public List<int> m_secondStarConstructionGridRows = new List<int>();

	[HideInInspector]
	public List<PartCount> m_partTypeCounts = new List<PartCount>();

	[HideInInspector]
	public bool m_newHighscore;

	[HideInInspector]
	public GameObject m_levelCompleteTutorialBookPagePrefab;

	[HideInInspector]
	public bool m_gridForSecondStar;

	[HideInInspector]
	public int m_totalAvailableParts;

	[HideInInspector]
	public int m_totalDestroyedParts;

	public GameObject m_inGameGuiPrefab;

	public Vector3 m_cameraOffset = new Vector3(0f, 0f, -10f);

	public Vector3 m_constructionOffset = new Vector3(0f, 0f, -7f);

	public Vector3 m_previewOffset = new Vector3(0f, 0f, -10f);

	public float m_predictionOffset = 2f;

	public CameraLimits m_cameraLimits;

	public float m_previewZoomOut = 15f;

	public float m_previewMoveTime = 5f;

	public float m_previewWaitTime = 5f;

	public bool m_showPowerupTutorial;

	public Texture2D m_blueprintTexture;

	public ScoreLimits m_scoreLimits;

	public GameObject m_gridCellPrefab;

	public bool m_SuperGlueAllowed = true;

	public bool m_SuperMagnetAllowed = true;

	public bool m_TurboChargeAllowed = true;

	public float m_tutorialDelayBeforeHint = 3f;

	public float m_tutorialDelayAfterHint = 1.5f;

	public GameData m_gameData;

	public GUIStyle m_buttonStyle;

	public float m_previewSpeed;

	public float m_previewTime;

	public bool m_previewDragging;

	public Vector2 m_previewLastMousePos;

	public GameObject m_tutorialBookPagePrefab;

	public TextAsset m_prebuiltContraption;

	public TextAsset m_oneStarContraption;

	public List<TextAsset> m_threeStarContraption = new List<TextAsset>();

	public List<PartCount> m_partsToUnlockOnCompletion = new List<PartCount>();

	public bool m_showOnlyEngineButton;

	public bool m_disablePigCollisions;

	public List<PartCount> m_extraPartsForSecondStar = new List<PartCount>();

	public bool m_sandbox;

	public bool m_raceLevel;

	public bool m_collectPartBoxesSandbox;

	public bool m_darkLevel;

	public bool m_autoBuildUnlocked;

	public int m_CollectedDessertsCount;

	[SerializeField]
	private int m_DessertsCount = -1;

	[SerializeField]
	private float animationTimerOverride = 0.03f;

	[SerializeField]
	private float mechanicAnimationTimerOverride = 0.2f;

	private static float nextInterstitialTime = -1f;

	private const string INTERSTITIAL_COOLDOWN = "interstitial_cooldown";

	private bool m_levelStartAdDidPause;

	private bool m_requireConnectedContraption;

	private GameState m_gameState;

	private GameState m_stateBeforeTutorial;

	private List<BasePart.PartType> m_partsInGoal = new List<BasePart.PartType>();

	private int m_starCollected;

	private float m_completionTime;

	private bool m_completedLevel;

	private ConstructionUI m_constructionUI;

	private bool m_timeStarted;

	private AudioManager audioManager;

	private LightManager lightManager;

	private int m_gridHeight;

	private int m_gridWidth;

	private int m_gridXmin;

	private int m_gridXmax;

	private Vector3 m_previewCenter;

	private float m_lastTimePlayedCollisionSound;

	private InGameGUI m_inGameGui;

	private List<Challenge> m_challenges = new List<Challenge>();

	private float m_timeLimit;

	private List<float> m_timeLimits = new List<float>();

	private bool m_useSecondStarParts;

	private float m_autoBuildTimer;

	private float m_mechanicDustTimer;

	private int m_autoBuildIndex;

	private int m_autoBuildPhase;

	private float m_partShowTimer;

	private int m_unlockedPartIndex = -1;

	private List<ConstructionUI.PartDesc> m_unlockedParts;

	private GameObject m_unlockedPartBackground;

	private ContraptionDataset m_autoBuildData;

	private ConstructionUI.PartDesc m_autoBuildPart;

	private GameObject m_dragIcon;

	private int m_retries;

	private bool m_openTutorial;

	private bool m_openMechanicInfo;

	private bool m_openMechanicGift;

	private bool m_useBlueprint;

	private bool m_useSuperBlueprint;

	private bool m_tutorialBookOpened;

	private List<GameObject> m_dynamicObjects = new List<GameObject>();

	private List<GameObject> m_temporaryDynamicObjects = new List<GameObject>();

	private List<GameObject> m_dynamicObjectClones = new List<GameObject>();

	private Dictionary<string, string> m_UsedDessertPlaces = new Dictionary<string, string>();

	private Vector3 m_levelStart;

	private bool m_hasIceGround;

	private GameObject m_ambientSource;

	private bool fastBuilding;

	private bool m_firstTime = true;

	private GameMode m_gameMode;

	public bool m_toolboxOpenUponShopActivation;

	private static bool isConfectionerReported;

	public static int GameModeIndex { get; set; }

	public int LevelDessertsCount
	{
		get
		{
			if (m_DessertsCount >= 0)
			{
				return m_DessertsCount;
			}
			return WPFMonoBehaviour.gameData.m_LevelDessertsCount;
		}
	}

	public GameState gameState
	{
		get
		{
			return m_gameState;
		}
		private set
		{
			m_gameState = value;
		}
	}

	public bool RequireConnectedContraption => m_requireConnectedContraption;

	public Contraption ContraptionProto => m_gameMode.ContraptionProto;

	public Contraption ContraptionRunning => m_gameMode.ContraptionRunning;

	public ConstructionUI ConstructionUI => m_constructionUI;

	public LightManager LightManager => lightManager;

	public bool TimeStarted
	{
		get
		{
			return m_timeStarted;
		}
		set
		{
			m_timeStarted = value;
		}
	}

	public float TimeReward { get; set; }

	public float OriginalTimeLimit { get; set; }

	public float TimeLimit
	{
		get
		{
			return m_timeLimit;
		}
		set
		{
			m_timeLimit = value;
		}
	}

	public List<float> TimeLimits
	{
		get
		{
			return m_timeLimits;
		}
		set
		{
			m_timeLimits = value;
		}
	}

	public float CompletionTime
	{
		get
		{
			return m_completionTime;
		}
		set
		{
			m_completionTime = value;
		}
	}

	public bool HasCompleted
	{
		get
		{
			return m_completedLevel;
		}
		set
		{
			m_completedLevel = value;
		}
	}

	public int GridHeight => m_gridHeight;

	public int GridWidth => m_gridWidth;

	public int GridXMin => m_gridXmin;

	public int GridXMax => m_gridXmax;

	public Vector3 PreviewCenter
	{
		get
		{
			return m_previewCenter;
		}
		set
		{
			m_previewCenter = value;
		}
	}

	public Vector3 StartingPosition => m_levelStart;

	public bool HasGroundIce => m_hasIceGround;

	public bool EggRequired
	{
		get
		{
			if (!m_sandbox)
			{
				return GetPartTypeCount(BasePart.PartType.Egg) > 0;
			}
			return false;
		}
	}

	public bool PumpkinRequired
	{
		get
		{
			if (!m_sandbox)
			{
				return GetPartTypeCount(BasePart.PartType.Pumpkin) > 0;
			}
			return false;
		}
	}

	public List<Challenge> Challenges
	{
		get
		{
			return m_challenges;
		}
		set
		{
			m_challenges = value;
		}
	}

	public InGameGUI InGameGUI => m_inGameGui;

	public Dictionary<string, string> UsedDessertPlaces => m_UsedDessertPlaces;

	public List<int> CurrentConstructionGridRows => m_gameMode.CurrentConstructionGridRows;

	public List<BasePart.PartType> PartsInGoal => m_partsInGoal;

	public List<ConstructionUI.PartDesc> UnlockedParts
	{
		get
		{
			return m_unlockedParts;
		}
		set
		{
			m_unlockedParts = value;
		}
	}

	public float PartShowTimer
	{
		get
		{
			return m_partShowTimer;
		}
		set
		{
			m_partShowTimer = value;
		}
	}

	public int UnlockedPartIndex
	{
		get
		{
			return m_unlockedPartIndex;
		}
		set
		{
			m_unlockedPartIndex = value;
		}
	}

	public bool FastBuilding
	{
		get
		{
			return fastBuilding;
		}
		set
		{
			fastBuilding = value;
		}
	}

	public bool FirstTime
	{
		get
		{
			return m_firstTime;
		}
		set
		{
			m_firstTime = value;
		}
	}

	public GameState StateBeforeTutorial
	{
		get
		{
			return m_stateBeforeTutorial;
		}
		set
		{
			m_stateBeforeTutorial = value;
		}
	}

	public bool UseBlueprint
	{
		get
		{
			return m_useBlueprint;
		}
		set
		{
			m_useBlueprint = value;
		}
	}

	public bool UseSuperBlueprint
	{
		get
		{
			return m_useSuperBlueprint;
		}
		set
		{
			m_useSuperBlueprint = value;
		}
	}

	public int CurrentSuperBluePrint { get; set; }

	public Vector3 PigStartPosition { get; set; }

	public GameMode CurrentGameMode => m_gameMode;

	public Vector3 PreviewOffset => m_gameMode.PreviewOffset;

	public Vector3 CameraOffset => m_gameMode.CameraOffset;

	public Vector3 ConstructionOffset => m_gameMode.ConstructionOffset;

	public CameraLimits CurrentCameraLimits
	{
		get
		{
			if (CurrentGameMode.CameraLimits == null)
			{
				return m_cameraLimits;
			}
			return CurrentGameMode.CameraLimits;
		}
	}

	public GameObject GridCellPrefab
	{
		get
		{
			if (CurrentGameMode.GridCellPrefab == null)
			{
				return m_gridCellPrefab;
			}
			return CurrentGameMode.GridCellPrefab;
		}
	}

	public GameObject TutorialBookPage
	{
		get
		{
			if (CurrentGameMode.TutorialPage == null)
			{
				return m_tutorialBookPagePrefab;
			}
			return CurrentGameMode.TutorialPage;
		}
	}

	public bool SuperGlueAllowed
	{
		get
		{
			if (!m_SuperGlueAllowed)
			{
				return CurrentGameMode is CakeRaceMode;
			}
			return true;
		}
	}

	public bool SuperMagnetAllowed
	{
		get
		{
			if (!m_SuperMagnetAllowed)
			{
				return CurrentGameMode is CakeRaceMode;
			}
			return true;
		}
	}

	public bool TurboChargeAllowed
	{
		get
		{
			if (!m_TurboChargeAllowed)
			{
				return CurrentGameMode is CakeRaceMode;
			}
			return true;
		}
	}

	public bool SuperBluePrintsAllowed
	{
		get
		{
			if ((Singleton<GameManager>.Instance.CurrentEpisodeIndex != 0 || Singleton<GameManager>.Instance.CurrentLevel != 0) && Singleton<GameManager>.Instance.CurrentEpisodeIndex != -1 && !m_sandbox)
			{
				return !(CurrentGameMode is CakeRaceMode);
			}
			return false;
		}
	}

	public Transform GoalPosition
	{
		get
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Goal");
			if ((bool)gameObject)
			{
				return gameObject.transform;
			}
			return null;
		}
	}

	public List<CameraPreview.CameraControlPoint> CustomPreview
	{
		get
		{
			if (m_gameMode == null)
			{
				return null;
			}
			return m_gameMode.Preview;
		}
	}

	public float TimeElapsed { get; set; }

	public void AddTemporaryDynamicObject(GameObject obj)
	{
		m_temporaryDynamicObjects.Add(obj);
	}

	private void Awake()
	{
		if (!SingletonSpawner.SpawnDone)
		{
			UnityEngine.Object.Instantiate(m_gameData.singletonSpawner);
			StartCoroutine(DelayOnDataLoaded());
		}
		else if (!LevelLoader.IsLoadingLevel())
		{
			OnDataLoaded();
		}
	}

	private void OnDestroy()
	{
		SetGameState(GameState.Undefined);
		if (m_gameMode != null)
		{
			m_gameMode.CleanUp();
		}
	}

	public static void IncentiveVideoShown()
	{
		float num = 240f;
		if (Singleton<GameConfigurationManager>.IsInstantiated() && Singleton<GameConfigurationManager>.Instance.HasValue("interstitial_cooldown", "time"))
		{
			num = Singleton<GameConfigurationManager>.Instance.GetValue<float>("interstitial_cooldown", "time");
		}
		nextInterstitialTime = Time.realtimeSinceStartup + num;
	}

	private IEnumerator DelayOnDataLoaded()
	{
		while ((!HatchManager.IsInitialized || !Bundle.initialized) && Bundle.checkingBundles)
		{
			yield return null;
		}
		OnDataLoaded();
	}

	private GameMode SetupGameMode()
	{
		int gameModeIndex = GameModeIndex;
		GameManager.EpisodeType episodeType = Singleton<GameManager>.Instance.CurrentEpisodeType;
		if (episodeType == GameManager.EpisodeType.Undefined)
		{
			episodeType = (m_raceLevel ? GameManager.EpisodeType.Race : ((!m_sandbox) ? GameManager.EpisodeType.Normal : GameManager.EpisodeType.Sandbox));
		}
		bool flag = false;
		switch (episodeType)
		{
		case GameManager.EpisodeType.Sandbox:
		case GameManager.EpisodeType.Race:
		{
			string currentLevelIdentifier = Singleton<GameManager>.Instance.CurrentLevelIdentifier;
			flag = WPFMonoBehaviour.gameData.m_cakeRaceData.GetTrackCount(currentLevelIdentifier) > 0;
			break;
		}
		case GameManager.EpisodeType.Normal:
		{
			int currentEpisodeIndex = Singleton<GameManager>.Instance.CurrentEpisodeIndex;
			int currentLevel = Singleton<GameManager>.Instance.CurrentLevel;
			flag = WPFMonoBehaviour.gameData.m_cakeRaceData.GetTrackCount(currentEpisodeIndex, currentLevel) > 0;
			break;
		}
		}
		if (gameModeIndex != 1)
		{
			return new BaseGameMode();
		}
		if (flag)
		{
			return new CakeRaceMode();
		}
		return new BaseGameMode();
	}

	public void OnDataLoaded()
	{
		float @float = INSettings.GetFloat(INFeature.TerrainScale);
		m_cameraLimits.topLeft *= @float;
		m_cameraLimits.size *= @float;
		m_gameMode = SetupGameMode();
		if (!GameObject.Find("LevelStub"))
		{
			Singleton<GameManager>.Instance.InitializeTestLevelState();
		}
		m_gameMode.Initialize(this);
		m_gameMode.OnDataLoadedStart();
		UnityEngine.Object.Instantiate(m_gameData.effectManager);
		if ((bool)m_inGameGuiPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_inGameGuiPrefab);
			gameObject.name = m_inGameGuiPrefab.name;
			m_inGameGui = gameObject.GetComponent<InGameGUI>();
			Vector3 position = WPFMonoBehaviour.hudCamera.transform.position;
			m_inGameGui.transform.position = new Vector3(position.x, position.y, m_inGameGui.transform.position.z);
		}
		m_gameMode.InitGameMode();
		if (m_constructionUI != null)
		{
			m_constructionUI.SetMoveButtonStates();
		}
		DynamicObject[] array = UnityEngine.Object.FindObjectsOfType<DynamicObject>();
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject2 = array[i].gameObject;
			gameObject2.SetActive(value: false);
			m_dynamicObjects.Add(gameObject2);
		}
		if (INSettings.GetBool(INFeature.CancelPreview))
		{
			SetGameState(GameState.Building);
		}
		else
		{
			SetGameState(GameState.Preview);
		}
		if (GetComponent<AudioSource>() == null)
		{
			base.gameObject.AddComponent<AudioSource>();
		}
		GameObject gameObject3 = GameObject.FindGameObjectWithTag("World");
		string empty = string.Empty;
		PositionSerializer component = gameObject3.GetComponent<PositionSerializer>();
		empty = ((!component || !component.prefab) ? gameObject3.name : component.prefab.name);
		GameProgress.SetString("MenuBackground", empty);
		KeyListener.keyReleased += HandleKeyListenerkeyReleased;
		GameObject gameObject4 = null;
		if ((bool)GameObject.Find("Background_Jungle_01_SET"))
		{
			gameObject4 = WPFMonoBehaviour.gameData.commonAudioCollection.AmbientJungle;
		}
		else if ((bool)GameObject.Find("Background_Plateau_01_SET"))
		{
			gameObject4 = WPFMonoBehaviour.gameData.commonAudioCollection.AmbientPlateau;
		}
		else if (m_darkLevel || (bool)GameObject.Find("Background_Cave_01_SET 1"))
		{
			gameObject4 = WPFMonoBehaviour.gameData.commonAudioCollection.AmbientCave;
		}
		else if ((bool)GameObject.Find("Background_Night_01_SET 1"))
		{
			gameObject4 = WPFMonoBehaviour.gameData.commonAudioCollection.AmbientNight;
		}
		else if ((bool)GameObject.Find("Background_Forest_01_SET 1"))
		{
			gameObject4 = WPFMonoBehaviour.gameData.commonAudioCollection.AmbientMorning;
		}
		else if ((bool)GameObject.Find("Background_Halloween"))
		{
			gameObject4 = WPFMonoBehaviour.gameData.commonAudioCollection.AmbientHalloween;
		}
		else if ((bool)GameObject.Find("Background_MM_01_SET"))
		{
			gameObject4 = WPFMonoBehaviour.gameData.commonAudioCollection.AmbientMaya;
		}
		else if ((bool)GameObject.Find("Background_MM_Temple_01_SET_01"))
		{
			gameObject4 = WPFMonoBehaviour.gameData.commonAudioCollection.AmbientMaya;
		}
		AudioSource audioSource = ((!gameObject4) ? null : gameObject4.GetComponent<AudioSource>());
		if ((bool)audioSource)
		{
			m_ambientSource = Singleton<AudioManager>.Instance.SpawnLoopingEffect(audioSource, base.transform);
		}
		if (m_levelStartAdDidPause)
		{
			GameTime.Pause(pause: true);
		}
		TimeReward = 0f;
		if (m_darkLevel)
		{
			GameObject gameObject5 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Lights/LightManager"));
			lightManager = gameObject5.GetComponent<LightManager>();
			lightManager.Init(this);
		}
		m_gameMode.OnDataLoadedDone();
		EventManager.Send(new GameLevelLoaded(Singleton<GameManager>.Instance.CurrentLevel, Singleton<GameManager>.Instance.CurrentEpisodeIndex));
	}

	private void Start()
	{
		audioManager = Singleton<AudioManager>.Instance;
		if (m_oneStarContraption != null)
		{
			m_threeStarContraption.Add(m_oneStarContraption);
		}
	}

	private void OnEnable()
	{
		EventManager.Connect<GadgetControlEvent>(ReceiveGadgetControlEvent);
		EventManager.Connect<Dessert.DessertCollectedEvent>(ReceiveDessertCollected);
	}

	private void OnDisable()
	{
		EventManager.Disconnect<GadgetControlEvent>(ReceiveGadgetControlEvent);
		EventManager.Disconnect<Dessert.DessertCollectedEvent>(ReceiveDessertCollected);
		KeyListener.keyReleased -= HandleKeyListenerkeyReleased;
	}

	private void HandleKeyListenerkeyReleased(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			if (gameState == GameState.Building || gameState == GameState.Running)
			{
				EventManager.Send(new UIEvent(UIEvent.Type.Pause));
			}
			else if (gameState == GameState.PausedWhileBuilding || gameState == GameState.PausedWhileRunning)
			{
				EventManager.Send(new UIEvent(UIEvent.Type.ContinueFromPause));
			}
			else if (gameState == GameState.TutorialBook)
			{
				EventManager.Send(new UIEvent(UIEvent.Type.CloseTutorial));
			}
			else if (gameState == GameState.PreviewWhileBuilding)
			{
				SetGameState(GameState.Building);
			}
		}
	}

	public void CameraPreviewDone()
	{
		if (m_sandbox && m_constructionUI.UnlockedParts.Count > 0 && !INSettings.GetBool(INFeature.PartCounter))
		{
			SetGameState(GameState.ShowingUnlockedParts);
		}
		else
		{
			SetGameState(GameState.Building);
		}
	}

	public void ReceiveGadgetControlEvent(GadgetControlEvent data)
	{
		ContraptionRunning.ActivatePartType(data.partType, data.direction);
	}

	public void CheckForLevelStartAchievements()
	{
		if (!Singleton<SocialGameManager>.IsInstantiated())
		{
			return;
		}
		Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.COMPLEX_COMPLEX", 100.0, (int limit) => ContraptionProto.Parts.Count >= limit);
		List<BasePart.PartType> pinkParts = new List<BasePart.PartType>();
		List<BasePart.PartType> boneParts = new List<BasePart.PartType>();
		List<BasePart.PartType> goldenParts = new List<BasePart.PartType>();
		List<BasePart.PartType> blackParts = new List<BasePart.PartType>();
		foreach (BasePart part in ContraptionProto.Parts)
		{
			if (part.tags != null && part.tags.Count > 0)
			{
				if (!pinkParts.Contains(part.m_partType) && part.tags.Contains("Pink"))
				{
					pinkParts.Add(part.m_partType);
				}
				if (!boneParts.Contains(part.m_partType) && part.tags.Contains("Bone"))
				{
					boneParts.Add(part.m_partType);
				}
				if (!goldenParts.Contains(part.m_partType) && part.tags.Contains("Gold"))
				{
					goldenParts.Add(part.m_partType);
				}
				if (!blackParts.Contains(part.m_partType) && part.tags.Contains("Black"))
				{
					blackParts.Add(part.m_partType);
				}
			}
		}
		if (pinkParts.Count > 0)
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.PINK_BUILDER", 100.0, (int limit) => pinkParts.Count >= limit);
		}
		if (boneParts.Count > 0)
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.BONE_BUILDER", 100.0, (int limit) => boneParts.Count >= limit);
		}
		if (goldenParts.Count > 0)
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.GOLDEN_BUILDER", 100.0, (int limit) => goldenParts.Count >= limit);
		}
		if (blackParts.Count > 0)
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.BLACK_BUILDER", 100.0, (int limit) => blackParts.Count >= limit);
		}
	}

	public void PlaceBuildArea()
	{
		float x = m_gameMode.ContraptionProto.FindPig().transform.localPosition.x;
		Vector3 position = m_gameMode.ContraptionRunning.FindPig().transform.position;
		Vector3 vector = position;
		int layerMask = 1 << LayerMask.NameToLayer("Ground");
		if (Physics.Raycast(new Ray(position, new Vector3(0f, -1f, 0f)), out var hitInfo, 100f, layerMask))
		{
			vector.y = position.y - hitInfo.distance + 1.1f;
		}
		vector.x = position.x - x;
		int num = 0;
		int num2 = 0;
		for (int i = GridXMin; i <= GridXMax; i++)
		{
			int num3 = 0;
			for (int j = 0; j < GridHeight; j++)
			{
				for (int k = GridXMin - 1; k <= GridXMax + 1; k++)
				{
					if (!Physics.CheckSphere(vector + new Vector3(k + i, j, 0f), 0.55f, layerMask))
					{
						num3++;
					}
				}
			}
			int num4 = num3;
			if (i == 0)
			{
				num4++;
			}
			if (num4 > num)
			{
				num = num4;
				num2 = i;
			}
		}
		vector.x += num2;
		vector.z = 0f;
		m_constructionUI.transform.position = vector;
		m_gameMode.ContraptionProto.transform.position = vector;
	}

	public void ShowPurchaseDialog(IapManager.InAppPurchaseItemType iapType)
	{
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			string spriteID = string.Empty;
			switch (iapType)
			{
			case IapManager.InAppPurchaseItemType.BlueprintSingle:
				spriteID = "ce80c724-b7f4-4df0-9f5b-46c4bc5a8599";
				break;
			case IapManager.InAppPurchaseItemType.SuperGlueSingle:
				spriteID = "5a3b2e58-b8c9-444e-a315-e7d76c5bbac0";
				break;
			case IapManager.InAppPurchaseItemType.SuperMagnetSingle:
				spriteID = "ac695667-b01a-4f46-b346-9225a78f6baf";
				break;
			case IapManager.InAppPurchaseItemType.TurboChargeSingle:
				spriteID = "67151809-a646-4a30-9a4e-5241ab0da385";
				break;
			case IapManager.InAppPurchaseItemType.NightVisionSingle:
				spriteID = "33e4b4c2-4626-4e65-8b5e-a1e9b0df563d";
				break;
			}
			m_inGameGui.Hide();
			Singleton<IapManager>.Instance.GetShop().ConfirmSinglePurchase(iapType.ToString(), spriteID, string.Empty, 1, delegate
			{
				m_inGameGui.Show();
				ResourceBar.Instance.ShowItem(ResourceBar.Item.SnoutCoin, showItem: false);
			});
		}
	}

	public void OpenShop(string pageName)
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IAPEnabled)
		{
			m_toolboxOpenUponShopActivation = m_inGameGui.BuildMenu.ToolboxButton.ToolboxOpen;
			m_inGameGui.Hide();
			Singleton<IapManager>.Instance.OpenShopPage(delegate
			{
				m_inGameGui.Show();
			}, pageName);
		}
	}

	private IEnumerator OpenBluePrint()
	{
		yield return 0;
		EventManager.Send(new UIEvent(UIEvent.Type.Blueprint));
	}

	public void StartAutoBuild(TextAsset contraptionQuality)
	{
		Singleton<GuiManager>.Instance.IsEnabled = false;
		ConstructionUI.ClearContraption();
		m_autoBuildData = WPFPrefs.LoadContraptionDataset(contraptionQuality);
		SetAutoBuildOrder(m_autoBuildData);
		m_autoBuildTimer = 0f;
		m_mechanicDustTimer = 0f;
		m_autoBuildIndex = 0;
		m_autoBuildPhase = 0;
		m_inGameGui.BuildMenu.PigMechanic.SetTime(0.6f);
		m_inGameGui.BuildMenu.PigMechanic.Play();
	}

	public void SetGameState(GameState newState)
	{
		GameState prevState = gameState;
		gameState = m_gameMode.SetGameState(gameState, newState);
		EventManager.Send(new GameStateChanged(gameState, prevState));
	}

	public void HandleSnapshotFinished()
	{
		m_inGameGui.ShowCurrentMenu();
		SetGameState(GameState.Continue);
		if (Singleton<SocialGameManager>.IsInstantiated())
		{
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.SHOW_OFF", 100.0);
		}
	}

	public void SetupDynamicObjects()
	{
		foreach (GameObject dynamicObjectClone in m_dynamicObjectClones)
		{
			dynamicObjectClone.SetActive(value: false);
			UnityEngine.Object.Destroy(dynamicObjectClone);
		}
		m_dynamicObjectClones.Clear();
		foreach (GameObject temporaryDynamicObject in m_temporaryDynamicObjects)
		{
			temporaryDynamicObject.SetActive(value: false);
			UnityEngine.Object.Destroy(temporaryDynamicObject);
		}
		m_temporaryDynamicObjects.Clear();
		foreach (GameObject dynamicObject in m_dynamicObjects)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(dynamicObject);
			gameObject.SetActive(value: true);
			m_dynamicObjectClones.Add(gameObject);
		}
	}

	private void ReceiveDessertCollected(Dessert.DessertCollectedEvent eventData)
	{
		GameProgress.AddDesserts(eventData.dessert.saveId, 1);
		m_CollectedDessertsCount++;
		string key = Singleton<GameManager>.Instance.CurrentSceneName + "_dessert_placement";
		if (m_UsedDessertPlaces.Remove(eventData.dessert.place.name))
		{
			string value = string.Empty;
			if (m_UsedDessertPlaces.Count > 0)
			{
				int num = 0;
				string[] array = new string[m_UsedDessertPlaces.Count];
				foreach (KeyValuePair<string, string> usedDessertPlace in m_UsedDessertPlaces)
				{
					array[num] = usedDessertPlace.Key + ":" + usedDessertPlace.Value;
					num++;
				}
				value = string.Join(";", array);
			}
			GameProgress.SetString(key, value);
		}
		if (Singleton<SocialGameManager>.IsInstantiated() && !isConfectionerReported && Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.CONFECTIONER", 100.0, (int limit) => GameProgress.TotalDessertCount() > limit))
		{
			isConfectionerReported = true;
		}
	}

	public bool LoadDessertsPlacement(GameObject dessertPlacesRoot)
	{
		if (dessertPlacesRoot == null)
		{
			return false;
		}
		string key = Singleton<GameManager>.Instance.CurrentSceneName + "_dessert_placement";
		m_UsedDessertPlaces.Clear();
		string @string = GameProgress.GetString(key, null);
		if (string.IsNullOrEmpty(@string))
		{
			return false;
		}
		string[] array = @string.Split(';');
		if (array == null || array.Length == 0)
		{
			return false;
		}
		DessertPlacePair[] array2 = new DessertPlacePair[array.Length];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string[] placeDessert = array[i].Split(new char[1] { ':' }, 2);
			if (placeDessert == null || placeDessert.Length < 2 || string.IsNullOrEmpty(placeDessert[0]) || string.IsNullOrEmpty(placeDessert[1]))
			{
				m_UsedDessertPlaces.Clear();
				return false;
			}
			Transform transform = dessertPlacesRoot.transform.Find(placeDessert[0]);
			GameObject gameObject = WPFMonoBehaviour.gameData.m_desserts.Find((GameObject dessert) => dessert != null && dessert.GetComponent<Dessert>().saveId == placeDessert[1]);
			if (!(transform != null) || !(gameObject != null))
			{
				m_UsedDessertPlaces.Clear();
				return false;
			}
			array2[num].dessert = gameObject;
			array2[num].place = transform;
			num++;
			m_UsedDessertPlaces.Add(placeDessert[0], placeDessert[1]);
		}
		for (int j = 0; j < num; j++)
		{
			GameObject dessert2 = array2[j].dessert;
			Transform place = array2[j].place;
			GameObject obj = UnityEngine.Object.Instantiate(dessert2, place.position, place.rotation);
			obj.name = dessert2.name;
			obj.GetComponent<Dessert>().place = place.GetComponent<DessertPlace>();
		}
		return true;
	}

	public void NotifyGoalReachedByPart(BasePart.PartType partType)
	{
		m_gameMode.NotifyGoalReachedByPart(partType);
	}

	public bool PlayerHasRequiredObjects()
	{
		return m_gameMode.PlayerHasRequiredObjects();
	}

	public void NotifyGoalReached()
	{
		m_gameMode.NotifyGoalReached();
	}

	public bool IsPartTransported(BasePart.PartType partType)
	{
		BasePart basePart = m_gameMode.ContraptionRunning.FindPart(partType);
		if (basePart == null)
		{
			return false;
		}
		if (m_gameMode.ContraptionRunning.IsConnectedToPig(basePart))
		{
			return true;
		}
		if ((bool)basePart)
		{
			int connectedComponent = m_gameMode.ContraptionRunning.FindPig().ConnectedComponent;
			for (int i = 0; i < m_gameMode.ContraptionRunning.Parts.Count; i++)
			{
				if (m_gameMode.ContraptionRunning.Parts[i] != null && m_gameMode.ContraptionRunning.Parts[i].ConnectedComponent == connectedComponent && Vector3.Distance(basePart.Position, m_gameMode.ContraptionRunning.Parts[i].Position) < 2.5f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void NotifyStarCollected()
	{
		m_starCollected++;
	}

	public void CreateGrid(int newGridWidth, int newGridHeight, int newGridXMin, int newGridXMax, Vector3 position)
	{
		m_gridWidth = newGridWidth;
		m_gridHeight = newGridHeight;
		m_gridXmin = newGridXMin;
		m_gridXmax = newGridXMax;
		m_levelStart = position;
		if ((bool)WPFMonoBehaviour.gameData.m_constructionUIPrefab)
		{
			Transform transform = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_constructionUIPrefab);
			transform.gameObject.name = WPFMonoBehaviour.gameData.m_constructionUIPrefab.name;
			if ((bool)transform)
			{
				m_constructionUI = transform.GetComponent<ConstructionUI>();
				transform.position = position;
			}
		}
	}

	public bool CanPlacePartAtGridCell(int x, int y)
	{
		if (INSettings.GetBool(INFeature.InfiniteGrid))
		{
			return true;
		}
		if (INSettings.GetInt(INFeature.GridSize) != 1)
		{
			if (x >= m_gridXmin && x <= m_gridXmax && y >= 0)
			{
				return y < m_gridHeight;
			}
			return false;
		}
		if (x < m_gridXmin || x > m_gridXmax || y < 0 || y >= m_gridHeight)
		{
			return false;
		}
		int index = m_gridHeight - y - 1;
		int num = x - m_gridXmin;
		return (CurrentConstructionGridRows[index] & (1 << num)) != 0;
	}

	public BasePart BuildPart(ContraptionDataset.ContraptionDatasetUnit cdu, BasePart partPrefab)
	{
		BasePart basePart = WPFMonoBehaviour.levelManager.ConstructionUI.SetPartAt(cdu.x, cdu.y, partPrefab, autoalign: false);
		if (cdu.flipped)
		{
			basePart.SetFlipped(flipped: true);
			basePart.SetRotation((BasePart.GridRotation)cdu.rot);
		}
		else
		{
			basePart.SetRotation((BasePart.GridRotation)cdu.rot);
		}
		return basePart;
	}

	public int GetPartTypeCount(BasePart.PartType type)
	{
		if (Application.isPlaying)
		{
			return m_gameMode.GetPartCount(type);
		}
		int num = 0;
		foreach (PartCount partTypeCount in m_partTypeCounts)
		{
			if (partTypeCount.type == type)
			{
				num += partTypeCount.count;
				break;
			}
		}
		if (m_useSecondStarParts)
		{
			foreach (PartCount item in m_extraPartsForSecondStar)
			{
				if (item.type == type)
				{
					num += item.count;
					break;
				}
			}
		}
		if (m_sandbox && !(CurrentGameMode is CakeRaceMode))
		{
			if (!m_collectPartBoxesSandbox)
			{
				num += GameProgress.GetSandboxPartCount(type);
			}
			num += GameProgress.GetSandboxPartCount(Singleton<GameManager>.Instance.CurrentSceneName, type);
		}
		return num;
	}

	public void OnDrawGizmosSelected()
	{
		LevelStart levelStart = WPFMonoBehaviour.FindSceneObjectOfType<LevelStart>();
		Transform goalPosition = GoalPosition;
		if ((bool)levelStart && (bool)goalPosition)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(levelStart.transform.position, goalPosition.transform.position);
		}
		if ((bool)levelStart)
		{
			Gizmos.color = Color.green;
			Vector3 constructionOffset = m_constructionOffset;
			constructionOffset.z = 0f;
			Vector3 center = levelStart.transform.position + constructionOffset;
			float num = 1.3333334f;
			float num2 = Mathf.Tan(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView * ((float)Math.PI / 180f)) * Mathf.Abs(m_constructionOffset.z);
			Vector3 size = new Vector3(num2, num2 / num, 0f);
			Gizmos.DrawWireCube(center, size);
		}
		if ((bool)goalPosition)
		{
			Gizmos.color = Color.green;
			Vector3 previewOffset = m_previewOffset;
			previewOffset.z = 0f;
			Vector3 center2 = goalPosition.transform.position + previewOffset;
			float num3 = 1.3333334f;
			float num4 = Mathf.Tan(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView * ((float)Math.PI / 180f)) * Mathf.Abs(m_previewOffset.z);
			Vector3 size2 = new Vector3(num4, num4 / num3, 0f);
			Gizmos.DrawWireCube(center2, size2);
		}
	}

	public void Update()
	{
		switch (gameState)
		{
		case GameState.Preview:
			UpdatePreview();
			break;
		case GameState.PreviewMoving:
			UpdatePreviewMoving();
			break;
		case GameState.ShowingUnlockedParts:
			UpdateShowUnlockedParts();
			break;
		case GameState.AutoBuilding:
		case GameState.SuperAutoBuilding:
			UpdateAutoBuild();
			break;
		case GameState.Running:
			UpdateRunning();
			break;
		}
		if (GuiManager.GetPointer().down && GuiManager.GetPointer().onWidget)
		{
			EventManager.Send(default(UserInputEvent));
		}
		m_gameMode.Update();
	}

	private void UpdatePreview()
	{
		m_previewTime += Time.deltaTime * m_previewSpeed;
		bool flag = Input.touchCount > 0 || Input.GetMouseButtonDown(0);
		if (m_previewTime > m_previewWaitTime || flag)
		{
			SetGameState(GameState.PreviewMoving);
		}
	}

	private void UpdatePreviewMoving()
	{
		m_previewTime += Time.deltaTime * m_previewSpeed;
	}

	private void UpdateRunning()
	{
		if (m_timeStarted)
		{
			TimeElapsed += Time.deltaTime;
		}
		else if (Vector3.Distance(ContraptionRunning.FindPig().transform.position, PigStartPosition) >= 1f)
		{
			m_timeStarted = true;
		}
	}

	private void UpdateShowUnlockedParts()
	{
		if (!m_unlockedPartBackground)
		{
			m_unlockedPartBackground = new GameObject();
			m_unlockedPartBackground.transform.position = new Vector3(WPFMonoBehaviour.hudCamera.transform.position.x, WPFMonoBehaviour.hudCamera.transform.position.y, WPFMonoBehaviour.hudCamera.transform.position.z + 2f);
			float num = WPFMonoBehaviour.hudCamera.orthographicSize * 0.15f;
			m_unlockedPartBackground.transform.localScale = new Vector3(num, num, num);
			GameObject obj = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_partAppearBackground);
			obj.transform.parent = m_unlockedPartBackground.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			obj.GetComponent<Animation>().Play("BringInPartBox");
		}
		m_partShowTimer += Time.deltaTime;
		if (m_unlockedPartIndex == -1)
		{
			if (m_partShowTimer > 0.6f)
			{
				m_partShowTimer = 0f;
				m_unlockedPartIndex = 0;
			}
		}
		else if (m_partShowTimer < 1.25f && m_unlockedPartIndex < m_unlockedParts.Count)
		{
			ConstructionUI.PartDesc partDesc = m_unlockedParts[m_unlockedPartIndex];
			if (!m_dragIcon)
			{
				m_dragIcon = new GameObject();
				float num2 = Vector3.Distance(m_constructionUI.GridPositionToGuiPosition(0, 0), m_constructionUI.GridPositionToGuiPosition(1, 0));
				m_dragIcon.transform.localScale = new Vector3(num2, num2, num2);
				m_dragIcon.transform.position = new Vector3(WPFMonoBehaviour.hudCamera.transform.position.x, WPFMonoBehaviour.hudCamera.transform.position.y, WPFMonoBehaviour.hudCamera.transform.position.z + 1f);
				GameObject obj2 = UnityEngine.Object.Instantiate(partDesc.part.m_constructionIconSprite.gameObject, new Vector3(1000f, 0f, 0f), Quaternion.identity);
				obj2.transform.parent = m_dragIcon.transform;
				obj2.transform.localScale = Vector3.one;
				obj2.transform.localPosition = Vector3.zero;
				obj2.AddComponent<Animation>();
				obj2.GetComponent<Animation>().AddClip(WPFMonoBehaviour.gameData.m_partAppearAnimation, "PartAppear");
				obj2.GetComponent<Animation>().Play("PartAppear");
			}
			if (m_partShowTimer > 0.75f)
			{
				Vector3 item = new Vector3(WPFMonoBehaviour.hudCamera.transform.position.x, WPFMonoBehaviour.hudCamera.transform.position.y, WPFMonoBehaviour.hudCamera.transform.position.z + 1f);
				GameObject gameObject = m_constructionUI.FindPartButton(partDesc.part.m_partType);
				if (!(gameObject == null))
				{
					Vector3 position = gameObject.transform.position;
					position.z = WPFMonoBehaviour.hudCamera.transform.position.z + 1f;
					position.z = WPFMonoBehaviour.hudCamera.transform.position.z + 1f;
					List<Vector3> list = new List<Vector3>();
					list.Add(item);
					list.Add(item);
					list.Add(new Vector3(0.4f * item.x + 0.6f * position.x, 0.2f * item.y + 0.8f * position.y, item.z));
					list.Add(position);
					float t = MathsUtil.EaseInOutQuad(m_partShowTimer - 0.75f, 0f, 1f, 0.5f);
					m_dragIcon.transform.position = Tutorial.PositionOnSpline(list, t);
				}
			}
		}
		else
		{
			if (m_unlockedPartIndex < m_unlockedParts.Count)
			{
				ConstructionUI.PartDesc partDesc2 = m_unlockedParts[m_unlockedPartIndex];
				m_constructionUI.AddUnlockedPart(partDesc2.part.m_partType, partDesc2.maxCount);
			}
			m_partShowTimer = 0f;
			m_unlockedPartIndex++;
			UnityEngine.Object.Destroy(m_dragIcon);
			m_dragIcon = null;
			if (m_unlockedPartIndex >= m_unlockedParts.Count)
			{
				m_unlockedPartBackground.transform.GetChild(0).GetComponent<Animation>().Play("BringOutPartBox");
				UnityEngine.Object.Destroy(m_unlockedPartBackground, 1f);
				m_unlockedPartBackground = null;
				SetGameState(GameState.Building);
			}
		}
	}

	private void UpdateAutoBuild()
	{
		m_autoBuildTimer += Time.deltaTime;
		if (m_autoBuildPhase == 0)
		{
			Vector3 b = m_constructionUI.RelativeLevelPositionToHudPosition(new Vector3((float)WPFMonoBehaviour.levelManager.GridXMax + 0.5f, -0.5f, 0f));
			Vector3 a = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
			a.x += 2.5f;
			a.y = b.y;
			a.z = b.z;
			if (m_autoBuildTimer <= ((!fastBuilding) ? 0.7f : mechanicAnimationTimerOverride))
			{
				float t = MathsUtil.EaseInOutQuad(m_autoBuildTimer, 0f, 1f, (!fastBuilding) ? 0.7f : mechanicAnimationTimerOverride);
				GameObject gameObject = GameObject.Find("PigMechanic");
				gameObject.transform.position = Vector3.Slerp(a, b, t);
				m_mechanicDustTimer += Time.deltaTime;
				if (m_mechanicDustTimer > 0.125f)
				{
					m_mechanicDustTimer = 0f;
					Vector3 position = WPFMonoBehaviour.hudCamera.WorldToScreenPoint(gameObject.transform.position + Vector3.down);
					Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
					WPFMonoBehaviour.effectManager.CreateParticles(WPFMonoBehaviour.gameData.m_dustParticles, position2);
				}
			}
			else if (m_autoBuildTimer > ((!fastBuilding) ? 1f : mechanicAnimationTimerOverride))
			{
				m_autoBuildPhase = 1;
			}
		}
		else if (m_autoBuildPhase == 1)
		{
			if (m_autoBuildPart == null)
			{
				if (!(m_autoBuildTimer > ((!fastBuilding) ? 0.2f : animationTimerOverride)))
				{
					return;
				}
				m_autoBuildTimer = 0f;
				if (m_autoBuildIndex < m_autoBuildData.ContraptionDatasetList.Count)
				{
					ContraptionDataset.ContraptionDatasetUnit contraptionDatasetUnit = m_autoBuildData.ContraptionDatasetList[m_autoBuildIndex];
					ConstructionUI.PartDesc partDesc = m_constructionUI.FindPartDesc((BasePart.PartType)contraptionDatasetUnit.partType);
					if (partDesc != null)
					{
						partDesc.useCount++;
						EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
					}
					m_autoBuildPart = partDesc;
					m_dragIcon = UnityEngine.Object.Instantiate(partDesc.part.m_constructionIconSprite.gameObject, new Vector3(1000f, 0f, 0f), Quaternion.identity);
					float num = Vector3.Distance(m_constructionUI.GridPositionToGuiPosition(0, 0), m_constructionUI.GridPositionToGuiPosition(1, 0));
					m_dragIcon.transform.localScale = new Vector3(num, num, num);
				}
				else
				{
					m_autoBuildPhase = 2;
				}
			}
			else if (m_autoBuildTimer < ((!fastBuilding) ? 0.2f : animationTimerOverride))
			{
				ContraptionDataset.ContraptionDatasetUnit contraptionDatasetUnit2 = m_autoBuildData.ContraptionDatasetList[m_autoBuildIndex];
				Vector3 position3 = GameObject.Find("PartSelector").GetComponent<PartSelector>().FindPartButton(m_autoBuildPart)
					.transform.position;
				position3.z = WPFMonoBehaviour.hudCamera.transform.position.z + 1f;
				Vector3 item = m_constructionUI.RelativeLevelPositionToHudPosition(new Vector3(contraptionDatasetUnit2.x, contraptionDatasetUnit2.y, 0f));
				item.z = WPFMonoBehaviour.hudCamera.transform.position.z + 1f;
				List<Vector3> list = new List<Vector3>();
				list.Add(position3);
				list.Add(new Vector3(0.4f * position3.x + 0.6f * item.x, 0.2f * position3.y + 0.8f * item.y, position3.z));
				list.Add(item);
				float t2 = MathsUtil.EaseInOutQuad(m_autoBuildTimer, 0f, 1f, 0.2f);
				m_dragIcon.transform.position = Tutorial.PositionOnSpline(list, t2);
			}
			else if (m_autoBuildTimer > ((!fastBuilding) ? 0.2f : animationTimerOverride))
			{
				UnityEngine.Object.Destroy(m_dragIcon);
				ContraptionDataset.ContraptionDatasetUnit contraptionDatasetUnit3 = m_autoBuildData.ContraptionDatasetList[m_autoBuildIndex];
				BasePart basePart = BuildPart(contraptionDatasetUnit3, m_autoBuildPart.part);
				basePart.GetComponent<BasePart>().ChangeVisualConnections();
				ContraptionProto.RefreshNeighbours(basePart.m_coordX, basePart.m_coordY);
				Vector3 position4 = m_constructionUI.GridPositionToWorldPosition(contraptionDatasetUnit3.x, contraptionDatasetUnit3.y);
				position4.z += -1f;
				WPFMonoBehaviour.effectManager.CreateParticles(WPFMonoBehaviour.gameData.m_constructionParticles, position4);
				m_autoBuildIndex++;
				m_autoBuildPart = null;
				m_autoBuildTimer = 0f;
			}
		}
		else
		{
			if (m_autoBuildPhase != 2)
			{
				return;
			}
			Vector3 a2 = m_constructionUI.RelativeLevelPositionToHudPosition(new Vector3((float)WPFMonoBehaviour.levelManager.GridXMax + 0.5f, -0.5f, 0f));
			Vector3 b2 = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
			b2.x += 2.5f;
			b2.y = a2.y;
			b2.z = a2.z;
			if (m_autoBuildTimer <= ((!fastBuilding) ? 0.7f : mechanicAnimationTimerOverride))
			{
				float t3 = MathsUtil.EaseInOutQuad(m_autoBuildTimer, 0f, 1f, (!fastBuilding) ? 0.7f : mechanicAnimationTimerOverride);
				GameObject gameObject2 = GameObject.Find("PigMechanic");
				gameObject2.transform.position = Vector3.Slerp(a2, b2, t3);
				m_mechanicDustTimer += Time.deltaTime;
				if (m_mechanicDustTimer > 0.125f)
				{
					m_mechanicDustTimer = 0f;
					Vector3 position5 = WPFMonoBehaviour.hudCamera.WorldToScreenPoint(gameObject2.transform.position + Vector3.down);
					Vector3 position6 = Camera.main.ScreenToWorldPoint(position5);
					WPFMonoBehaviour.effectManager.CreateParticles(WPFMonoBehaviour.gameData.m_dustParticles, position6);
				}
			}
			else
			{
				SetGameState(GameState.Building);
				m_constructionUI.SetMoveButtonStates();
				Singleton<GuiManager>.Instance.IsEnabled = true;
			}
		}
	}

	public void SetAutoBuildOrder(ContraptionDataset data)
	{
		int num = -1;
		List<ContraptionDataset.ContraptionDatasetUnit> contraptionDatasetList = data.ContraptionDatasetList;
		for (int i = 0; i < contraptionDatasetList.Count; i++)
		{
			if (contraptionDatasetList[i].partType == 10)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			ContraptionDataset.ContraptionDatasetUnit value = contraptionDatasetList[contraptionDatasetList.Count - 1];
			contraptionDatasetList[contraptionDatasetList.Count - 1] = contraptionDatasetList[num];
			contraptionDatasetList[num] = value;
		}
	}

	public void PlayVictorySound()
	{
		audioManager.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.clearBuildGrid);
		audioManager.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.victory);
	}

	private void OnApplicationFocus(bool focus)
	{
		Shop shop = Singleton<IapManager>.Instance.GetShop();
		if (!focus && (!(shop != null) || (!shop.gameObject.activeInHierarchy && !shop.SnoutCoinShop.gameObject.activeInHierarchy)) && !Application.isEditor && (gameState == GameState.Running || gameState == GameState.Building))
		{
			EventManager.Send(new UIEvent(UIEvent.Type.Pause));
		}
	}

	public void StopAmbient()
	{
		if (m_ambientSource != null)
		{
			Singleton<AudioManager>.Instance.StopLoopingEffect(m_ambientSource.GetComponent<AudioSource>());
		}
	}

	public void AddToTimeLimit(float time)
	{
		TimeReward = time;
		for (int i = 0; i < m_timeLimits.Count; i++)
		{
			m_timeLimits[i] += time;
			m_timeLimit += time;
		}
		for (int j = 0; j < m_challenges.Count; j++)
		{
			if (m_challenges[j].Type == Challenge.ChallengeType.Time)
			{
				(m_challenges[j] as TimeChallenge).m_targetTime += time;
			}
		}
	}

	public bool IsTimeChallengesCompleted()
	{
		for (int i = 0; i < m_challenges.Count; i++)
		{
			if (m_challenges[i].Type == Challenge.ChallengeType.Time && GameProgress.IsChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[i].ChallengeNumber) && m_challenges[i].IsCompleted())
			{
				return true;
			}
		}
		return false;
	}
}
