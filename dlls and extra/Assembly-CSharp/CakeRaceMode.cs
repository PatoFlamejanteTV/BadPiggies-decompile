using System;
using System.Collections;
using System.Collections.Generic;
using CakeRace;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CakeRaceMode : GameMode
{
	private List<Cake> cakes;

	private int retries;

	private int gainedXP;

	private bool openTutorial;

	private bool openMechanicInfo;

	private bool timeRunning;

	private bool showBombParticles;

	private List<LevelManager.PartCount> parts;

	public Action<int> ScoreUpdated;

	public Action<int> CakeCollected;

	public Action<int> TrackChanged;

	private const string MECHANIC_PROMOTION_COUNT = "Mechanic_Promotion_Count";

	private const string MECHANIC_PROMOTION_TIME = "Last_Mechanic_Promotion_Time_Binary";

	private const string TUTORIAL_PROMOTION_COUNT = "Tutorial_Promotion_Count";

	private const string BRANDED_REWARD_GIFT_COUNT = "branded_reward_gifts_today";

	private const string BRANDED_REWARD_GIFT_TIME = "branded_reward_gift_time";

	private const string BRANDED_REWARD_COOLDOWN = "branded_reward_cooldown";

	private bool openMechanicGift;

	private static int currentRaceTrackIndex;

	private static CakeRaceInfo? cakeRaceInfo;

	public bool IsRaceOn { get; private set; }

	public bool LocalPlayerIsWinner => CurrentScore >= OpponentScore;

	public int CollectedCakes { get; private set; }

	public float RaceTimeLeft { get; private set; }

	public int CurrentScore { get; private set; }

	public int OpponentScore { get; set; }

	private float CollectMultiplier { get; set; }

	private float ExplodeMultiplier { get; set; }

	public int GainedXP => gainedXP;

	private string TutorialPromotionCount => string.Format("{0}_{1}", "Tutorial_Promotion_Count", base.gameManager.CurrentSceneName);

	private string MechanicPromotionCount => string.Format("{0} {1}", "Mechanic_Promotion_Count", base.gameManager.CurrentSceneName);

	public static CakeRaceReplay OpponentReplay { get; set; }

	public static CakeRaceReplay CurrentReplay { get; set; }

	public static CakeRaceInfo CurrentCakeRaceInfo
	{
		get
		{
			return cakeRaceInfo.Value;
		}
		set
		{
			cakeRaceInfo = value;
		}
	}

	public static LootCrateType CurrentRewardCrate { get; private set; }

	public int CurrentTrackIndex => currentRaceTrackIndex;

	public static bool IsPreviewMode => false;

	public override void InitGameMode()
	{
		CakeRaceInfo? cakeRaceInfo = CakeRaceMode.cakeRaceInfo;
		if (!cakeRaceInfo.HasValue)
		{
			FindCakeRaceInfo(currentRaceTrackIndex);
		}
		else
		{
			currentRaceTrackIndex = CakeRaceMode.cakeRaceInfo.Value.TrackIndex;
		}
		gainedXP = 0;
		IsRaceOn = false;
		InitScoreVariables();
		CreateCakes();
		CreateProps();
		base.Preview = CreatePreview();
		base.CurrentConstructionGridRows = CakeRaceMode.cakeRaceInfo.Value.Start.GridData;
		base.CameraLimits = CakeRaceMode.cakeRaceInfo.Value.CameraLimits;
		base.GridCellPrefab = CakeRaceMode.cakeRaceInfo.Value.GridCellPrefab;
		base.TutorialPage = CakeRaceMode.cakeRaceInfo.Value.TutorialBookPrefab;
		int num = 1;
		int num2 = 1;
		for (int i = 0; i < base.CurrentConstructionGridRows.Count; i++)
		{
			if (base.CurrentConstructionGridRows[i] != 0)
			{
				int numberOfHighestBit = WPFMonoBehaviour.GetNumberOfHighestBit(base.CurrentConstructionGridRows[i]);
				if (numberOfHighestBit + 1 > num)
				{
					num = numberOfHighestBit + 1;
				}
				num2 = i + 1;
			}
		}
		num *= INSettings.GetInt(INFeature.GridSize);
		num2 *= INSettings.GetInt(INFeature.GridSize);
		int newGridXMin = -(num - 1) / 2;
		int newGridXMax = num / 2;
		levelManager.CreateGrid(num, num2, newGridXMin, newGridXMax, CakeRaceMode.cakeRaceInfo.Value.Start.Position);
		base.ContraptionProto.transform.position = CakeRaceMode.cakeRaceInfo.Value.Start.Position;
		InitParts();
		base.CameraOffset = new Vector3(0f, 15f, 0f);
		base.PreviewOffset = new Vector3(0f, 15f, 0f);
		base.ConstructionOffset = new Vector3(0f, 0f, 0f);
		timeRunning = false;
		if ((bool)levelManager.ConstructionUI)
		{
			if (GameProgress.HasKey(SchematicButton.LastLoadedSlotKey))
			{
				base.CurrentContraptionIndex = GameProgress.GetInt(SchematicButton.LastLoadedSlotKey);
			}
			BuildContraption(WPFPrefs.LoadContraptionDataset(GetCurrentContraptionName()));
			foreach (ConstructionUI.PartDesc partDescriptor in levelManager.ConstructionUI.PartDescriptors)
			{
				EventManager.Send(new PartCountChanged(partDescriptor.part.m_partType, partDescriptor.CurrentCount));
			}
		}
		EventManager.Connect<TimeBomb.BombOutOfBounds>(OnBombOutOfBounds);
	}

	private void InitScoreVariables()
	{
		CollectMultiplier = 1f;
		ExplodeMultiplier = 0.1f;
	}

	private void InitParts()
	{
		if (parts != null)
		{
			return;
		}
		parts = new List<LevelManager.PartCount>(cakeRaceInfo.Value.CustomParts);
		BasePart currentFavorite = Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite;
		bool flag = false;
		bool flag2 = false;
		foreach (LevelManager.PartCount part in parts)
		{
			if (part.type == BasePart.PartType.TimeBomb)
			{
				flag = true;
				part.count = 1;
			}
			if (!flag2 && currentFavorite != null && part.type == currentFavorite.m_partType)
			{
				flag2 = true;
			}
		}
		if (!flag)
		{
			LevelManager.PartCount partCount = new LevelManager.PartCount();
			partCount.type = BasePart.PartType.TimeBomb;
			partCount.count = 1;
			parts.Add(partCount);
		}
		if (!flag2 && currentFavorite != null)
		{
			LevelManager.PartCount partCount2 = new LevelManager.PartCount();
			partCount2.type = currentFavorite.m_partType;
			partCount2.count = 1;
			parts.Add(partCount2);
		}
	}

	public override void OnDataLoadedDone()
	{
		PartBox[] array = UnityEngine.Object.FindObjectsOfType<PartBox>();
		if (array != null && array.Length != 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
		}
		for (int j = 0; j < StarBox.StarBoxes.Count; j++)
		{
			StarBox.StarBoxes[j].gameObject.SetActive(value: false);
		}
		for (int k = 0; k < Challenge.Challenges.Count; k++)
		{
			Challenge.Challenges[k].gameObject.SetActive(value: false);
		}
		if (levelManager.GoalPosition != null)
		{
			levelManager.GoalPosition.gameObject.SetActive(value: false);
		}
		levelManager.m_sandbox = false;
		levelManager.m_raceLevel = false;
		Singleton<GuiManager>.Instance.enabled = true;
	}

	public override void OnDataLoadedStart()
	{
	}

	public override void CleanUp()
	{
		base.CleanUp();
		EventManager.Disconnect<TimeBomb.BombOutOfBounds>(OnBombOutOfBounds);
		cakeRaceInfo = null;
	}

	public override void Update()
	{
		if (levelManager.gameState == LevelManager.GameState.Running && IsRaceOn)
		{
			if (timeRunning)
			{
				RaceTimeLeft -= Time.deltaTime;
				if (RaceTimeLeft <= 0f)
				{
					EndRace(-1);
				}
			}
			else
			{
				timeRunning = (base.ContraptionRunning.FindPig().transform.position - levelManager.PigStartPosition).magnitude >= 1f;
				if (timeRunning)
				{
					EventManager.Send(new UIEvent(UIEvent.Type.CakeRaceTimerStarted));
				}
			}
		}
		base.Update();
	}

	public override LevelManager.GameState SetGameState(LevelManager.GameState currentState, LevelManager.GameState newState)
	{
		LevelManager.GameState gameState = currentState;
		switch (newState)
		{
		case LevelManager.GameState.Building:
		{
			if (GameTime.IsPaused())
			{
				GameTime.Pause(pause: false);
			}
			if (currentState == LevelManager.GameState.Running || currentState == LevelManager.GameState.PausedWhileRunning)
			{
				StopRunningContraption();
				retries++;
				if (retries == 3 && !levelManager.m_sandbox && !tutorialBookOpened)
				{
					int @int = GameProgress.GetInt("Tutorial_Promotion_Count");
					if (@int < 3 && !GameProgress.IsLevelCompleted(base.gameManager.CurrentSceneName) && GameProgress.GetInt(TutorialPromotionCount) == 0)
					{
						openTutorial = true;
						@int++;
						GameProgress.SetInt("Tutorial_Promotion_Count", @int);
						GameProgress.SetInt(TutorialPromotionCount, 1);
					}
				}
				bool @bool = GameProgress.GetBool(base.gameManager.CurrentSceneName + "_autobuild_available");
				bool bool2 = GameProgress.GetBool("PermanentBlueprint");
				if (!levelManager.m_sandbox && retries % 5 == 0 && AdvertisementHandler.GetRewardNativeTexture() != null)
				{
					int int2 = GameProgress.GetInt("branded_reward_gifts_today");
					int num = 2;
					if (Singleton<GameConfigurationManager>.IsInstantiated() && Singleton<GameConfigurationManager>.Instance.HasValue("branded_reward_gift_count", "count"))
					{
						num = Singleton<GameConfigurationManager>.Instance.GetValue<int>("branded_reward_gift_count", "count");
					}
					if (int2 < num)
					{
						if (!GameProgress.HasKey("branded_reward_gift_time"))
						{
							GameProgress.SetInt("branded_reward_gift_time", Singleton<TimeManager>.Instance.CurrentEpochTime);
						}
						GameProgress.SetInt("branded_reward_gifts_today", int2 + 1);
						openMechanicGift = true;
					}
				}
				else if (!levelManager.m_sandbox && retries == 5 && ((!bool2 && !levelManager.SuperBluePrintsAllowed) || (!@bool && levelManager.SuperBluePrintsAllowed)) && Singleton<BuildCustomizationLoader>.Instance.IAPEnabled)
				{
					int int3 = GameProgress.GetInt("Mechanic_Promotion_Count");
					long num2 = Convert.ToInt64(GameProgress.GetString("Last_Mechanic_Promotion_Time_Binary", "0"));
					DateTime value = DateTime.Now;
					if (num2 != 0L)
					{
						value = DateTime.FromBinary(num2);
					}
					else
					{
						GameProgress.SetString("Last_Mechanic_Promotion_Time_Binary", value.ToBinary().ToString());
					}
					if ((DateTime.Now.Subtract(value).TotalMinutes > 1440.0 || int3 < 3) && !GameProgress.IsLevelCompleted(base.gameManager.CurrentSceneName) && GameProgress.GetInt(MechanicPromotionCount) == 0)
					{
						if (int3 < 1)
						{
							GameProgress.AddBluePrints(1);
						}
						openMechanicInfo = true;
						int3++;
						GameProgress.SetInt("Mechanic_Promotion_Count", int3);
						GameProgress.SetInt(MechanicPromotionCount, 1);
						GameProgress.SetString("Last_Mechanic_Promotion_Time_Binary", DateTime.Now.ToBinary().ToString());
					}
				}
			}
			if (levelManager.m_toolboxOpenUponShopActivation)
			{
				levelManager.InGameGUI.BuildMenu.ToolboxButton.OnPressed();
			}
			levelManager.SetupDynamicObjects();
			base.ContraptionProto.SetVisible(visible: true);
			if ((bool)levelManager.ConstructionUI)
			{
				levelManager.ConstructionUI.SetEnabled(enableUI: true, enableGrid: true);
			}
			ResetCakes();
			if (!(GameProgress.GetString("REPLAY_LEVEL", string.Empty) == SceneManager.GetActiveScene().name) || LightManager.enabledLightPositions == null || LightManager.enabledLightPositions.Count <= 0)
			{
				break;
			}
			PointLightSource[] array = UnityEngine.Object.FindObjectsOfType<PointLightSource>();
			for (int i = 0; i < array.Length; i++)
			{
				if (LightManager.enabledLightPositions.Contains(array[i].transform.position))
				{
					array[i].isEnabled = true;
				}
			}
			GameProgress.SetString("REPLAY_LEVEL", string.Empty);
			break;
		}
		case LevelManager.GameState.Preview:
			if (GameTime.IsPaused())
			{
				GameTime.Pause(pause: false);
			}
			levelManager.m_previewSpeed = 1f;
			levelManager.m_previewTime = 0f;
			base.ContraptionProto.SetVisible(visible: false);
			if ((bool)levelManager.ConstructionUI)
			{
				levelManager.ConstructionUI.SetEnabled(enableUI: false, enableGrid: true);
			}
			break;
		case LevelManager.GameState.PreviewMoving:
			levelManager.m_previewTime = 0f;
			base.ContraptionProto.SetVisible(visible: false);
			if ((bool)levelManager.ConstructionUI)
			{
				levelManager.ConstructionUI.SetEnabled(enableUI: false, enableGrid: true);
			}
			levelManager.SetupDynamicObjects();
			break;
		case LevelManager.GameState.PreviewWhileBuilding:
			if (levelManager.EggRequired)
			{
				levelManager.InGameGUI.PreviewMenu.SetGoal(base.gameData.m_eggTransportGoal);
			}
			else if (levelManager.PumpkinRequired)
			{
				levelManager.InGameGUI.PreviewMenu.SetGoal(base.gameData.m_pumpkinTransportGoal);
			}
			else
			{
				levelManager.InGameGUI.PreviewMenu.SetGoal(base.gameData.m_basicGoal);
			}
			levelManager.InGameGUI.PreviewMenu.SetChallenges(levelManager.Challenges);
			if ((bool)levelManager.ConstructionUI)
			{
				levelManager.ConstructionUI.SetEnabled(enableUI: false, enableGrid: true);
			}
			levelManager.PreviewCenter = base.ContraptionProto.transform.position;
			levelManager.m_previewDragging = false;
			break;
		case LevelManager.GameState.PreviewWhileRunning:
			levelManager.PreviewCenter = base.ContraptionRunning.transform.position;
			GameTime.Pause(pause: true);
			levelManager.m_previewDragging = false;
			break;
		case LevelManager.GameState.Running:
			if (GameTime.IsPaused())
			{
				GameTime.Pause(pause: false);
			}
			levelManager.TimeElapsed = 0f;
			levelManager.PartsInGoal.Clear();
			levelManager.TimeStarted = false;
			levelManager.PigStartPosition = base.ContraptionProto.FindPig().transform.position;
			if ((bool)levelManager.ConstructionUI)
			{
				levelManager.ConstructionUI.SetEnabled(enableUI: false, enableGrid: false);
			}
			base.ContraptionRunning = base.ContraptionProto.Clone();
			base.ContraptionProto.SetVisible(visible: false);
			if (base.ContraptionProto.HasRegularGlue)
			{
				if (!base.ContraptionProto.HasGluedParts)
				{
					GameProgress.AddSuperGlue(1);
				}
				base.ContraptionProto.RemoveSuperGlue();
			}
			if (base.ContraptionProto.HasSuperMagnet)
			{
				base.ContraptionProto.HasSuperMagnet = false;
			}
			if (base.ContraptionProto.HasNightVision)
			{
				levelManager.LightManager.ToggleNightVision();
				base.ContraptionProto.HasNightVision = false;
			}
			base.ContraptionRunning.StartContraption();
			if (base.ContraptionProto.HasTurboCharge)
			{
				if (base.ContraptionRunning.PowerConsumption == 0f)
				{
					base.ContraptionRunning.HasTurboCharge = false;
					GameProgress.AddTurboCharge(1);
				}
				base.ContraptionProto.HasTurboCharge = false;
			}
			if (gameState == LevelManager.GameState.Building)
			{
				StartRace();
			}
			base.ContraptionRunning.SaveContraption(GetCurrentContraptionName());
			break;
		case LevelManager.GameState.Continue:
			if (GameTime.IsPaused())
			{
				GameTime.Pause(pause: false);
			}
			switch (gameState)
			{
			case LevelManager.GameState.Building:
			case LevelManager.GameState.PausedWhileBuilding:
				newState = LevelManager.GameState.Building;
				break;
			case LevelManager.GameState.CustomizingPart:
				newState = LevelManager.GameState.CustomizingPart;
				break;
			default:
				newState = ((levelManager.HasCompleted || gameState != LevelManager.GameState.LootCrateOpening) ? ((!levelManager.HasCompleted) ? LevelManager.GameState.Running : LevelManager.GameState.Completed) : LevelManager.GameState.Running);
				break;
			}
			break;
		case LevelManager.GameState.PausedWhileRunning:
			GameTime.Pause(pause: true);
			break;
		case LevelManager.GameState.PausedWhileBuilding:
			GameTime.Pause(pause: true);
			break;
		case LevelManager.GameState.AutoBuilding:
			levelManager.StartAutoBuild(levelManager.m_oneStarContraption);
			break;
		case LevelManager.GameState.ShowingUnlockedParts:
			GameTime.Pause(pause: false);
			levelManager.UnlockedPartIndex = -1;
			levelManager.PartShowTimer = 0f;
			break;
		case LevelManager.GameState.Snapshot:
			GameTime.Pause(pause: true);
			levelManager.InGameGUI.ShowCurrentMenu(show: false);
			WPFMonoBehaviour.ingameCamera.TakeSnapshot(levelManager.HandleSnapshotFinished);
			break;
		case LevelManager.GameState.SuperAutoBuilding:
			levelManager.StartAutoBuild(levelManager.m_threeStarContraption[levelManager.CurrentSuperBluePrint]);
			if (Singleton<SocialGameManager>.IsInstantiated())
			{
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.CHIPS_FOR_WHIPS", 100.0);
			}
			break;
		}
		currentState = newState;
		return currentState;
	}

	protected override bool HandleUIEvent(UIEvent data)
	{
		switch (data.type)
		{
		case UIEvent.Type.ReplayLevel:
			StopRunningContraption();
			levelManager.SetGameState(LevelManager.GameState.Building);
			return true;
		default:
			return false;
		case UIEvent.Type.LevelSelection:
		case UIEvent.Type.NextLevel:
			if (levelManager.gameState != LevelManager.GameState.PausedWhileRunning)
			{
				if (levelManager.gameState == LevelManager.GameState.PausedWhileBuilding)
				{
					if (levelManager.ContraptionProto.HasSuperGlue)
					{
						GameProgress.AddSuperGlue(1);
					}
					if (levelManager.ContraptionProto.HasSuperMagnet)
					{
						GameProgress.AddSuperMagnet(1);
					}
					if (levelManager.ContraptionProto.HasTurboCharge)
					{
						GameProgress.AddTurboCharge(1);
					}
					if (levelManager.ContraptionProto.HasNightVision)
					{
						GameProgress.AddNightVision(1);
					}
				}
				else
				{
					_ = levelManager.gameState;
					_ = 8;
				}
			}
			if (GameTime.IsPaused())
			{
				GameTime.Pause(pause: false);
			}
			if (data.type == UIEvent.Type.NextLevel)
			{
				CakeRaceMenu.FindNewPlayer = true;
			}
			levelManager.SetGameState(LevelManager.GameState.Undefined);
			Singleton<GameManager>.Instance.LoadCakeRaceMenu(showLoadingScreen: true);
			return true;
		}
	}

	protected void OnBombOutOfBounds(TimeBomb.BombOutOfBounds data)
	{
		showBombParticles = true;
	}

	protected override void OnPigOutOfBounds(Pig.PigOutOfBounds data)
	{
		EndRace(-1);
	}

	private void StartRace()
	{
		IsRaceOn = true;
		CurrentReplay = new CakeRaceReplay(cakeRaceInfo.Value.UniqueIdentifier, HatchManager.CurrentPlayer.PlayFabDisplayName, Singleton<PlayerProgress>.Instance.Level, HasKingsFavoritePart(), null);
		CurrentScore = 0;
		if (ScoreUpdated != null)
		{
			ScoreUpdated(CurrentScore);
		}
		RaceTimeLeft = cakeRaceInfo.Value.TimeLimit;
		CollectedCakes = 0;
		EventManager.Connect<TimeBombExplodeEvent>(OnTimeBombExplode);
	}

	private bool HasKingsFavoritePart()
	{
		BasePart currentFavorite = Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite;
		if (currentFavorite == null)
		{
			return false;
		}
		foreach (BasePart part in base.ContraptionRunning.Parts)
		{
			if (part != null && part.m_partType == currentFavorite.m_partType && part.customPartIndex == currentFavorite.customPartIndex)
			{
				return true;
			}
		}
		return false;
	}

	private void OnTimeBombExplode(TimeBombExplodeEvent data)
	{
		EndRace(-1, waitToExplode: false);
	}

	private void EndRace(int cakeIndex, bool waitToExplode = true)
	{
		EventManager.Disconnect<TimeBombExplodeEvent>(OnTimeBombExplode);
		if (IsRaceOn && levelManager.gameState != LevelManager.GameState.CakeRaceCompleted)
		{
			int collectTime = RaceTimeLeftInHundrethOfSeconds();
			CurrentReplay.SetCollectedCake(cakeIndex, collectTime);
			AddScore(CakeRaceReplay.CalculateCakeScore(cakeIndex < 0, collectTime, Singleton<PlayerProgress>.Instance.Level, HasKingsFavoritePart()));
			IsRaceOn = false;
			OpponentScore = 0;
			if (OpponentReplay != null)
			{
				OpponentScore = CakeRaceReplay.TotalScore(OpponentReplay);
			}
			PlayerProgressBar.Instance.DelayUpdate();
			gainedXP = Singleton<PlayerProgress>.Instance.AddExperience((!LocalPlayerIsWinner) ? PlayerProgress.ExperienceType.LoseCakeRace : PlayerProgress.ExperienceType.WinCakeRace);
			int @int = GameProgress.GetInt("cake_race_total_wins");
			if (!IsPreviewMode && LocalPlayerIsWinner)
			{
				RewardCrate(@int);
				Singleton<PlayFabManager>.Instance.Leaderboard.AddScore(PlayFabLeaderboard.Leaderboard.CakeRaceWins, 1);
				CakeRaceMenu.WinCount++;
			}
			else
			{
				CurrentRewardCrate = LootCrateType.None;
			}
			int trackIndex = CakeRaceMenu.GetTrackIndex(cakeRaceInfo.Value.UniqueIdentifier);
			if (IsPersonalBest(trackIndex, CurrentScore) && trackIndex >= 0 && trackIndex < 7)
			{
				string key = $"replay_track_{trackIndex}";
				string text = CurrentReplay.TrimmedString();
				Singleton<PlayFabManager>.Instance.UpdateUserData(new Dictionary<string, string> { { key, text } }, UserDataPermission.Public);
				SavePersonalBest(trackIndex, text);
				ReportCupScore(GameProgress.GetInt("cake_race_current_cup", (int)PlayFabLeaderboard.LowestCup()));
			}
			CoroutineRunner.Instance.StartCoroutine(EndingSequence(waitToExplode));
			int int2 = GameProgress.GetInt("cake_race_total_losses");
			string currentSeasonID = GetCurrentSeasonID();
			string key2 = $"Season_{currentSeasonID}_wins";
			string key3 = $"Season_{currentSeasonID}_losses";
			int int3 = GameProgress.GetInt(key2);
			int int4 = GameProgress.GetInt(key3);
			if (LocalPlayerIsWinner)
			{
				GameProgress.SetInt("cake_race_total_wins", ++@int);
				GameProgress.SetInt(key2, ++int3);
			}
			else
			{
				GameProgress.SetInt("cake_race_total_losses", ++int2);
				GameProgress.SetInt(key3, ++int4);
			}
		}
	}

	private bool IsPersonalBest(int trackIndex, int score)
	{
		string key = $"cake_race_track_{trackIndex}_pb_replay";
		int num = 0;
		if (GameProgress.HasKey(key))
		{
			num = CakeRaceReplay.TotalScore(new CakeRaceReplay(GameProgress.GetString(key, string.Empty)));
		}
		return num < score;
	}

	public CakeRaceReplay PersonalBest()
	{
		CakeRaceInfo? cakeRaceInfo = CakeRaceMode.cakeRaceInfo;
		if (!cakeRaceInfo.HasValue)
		{
			return null;
		}
		int trackIndex = CakeRaceMenu.GetTrackIndex(CakeRaceMode.cakeRaceInfo.Value.UniqueIdentifier);
		string key = $"cake_race_track_{trackIndex}_pb_replay";
		if (GameProgress.HasKey(key))
		{
			return new CakeRaceReplay(GameProgress.GetString(key, string.Empty));
		}
		return null;
	}

	private void SavePersonalBest(int trackIndex, string replay)
	{
		GameProgress.SetString($"cake_race_track_{trackIndex}_pb_replay", replay);
	}

	public static void ClearPersonalBestData()
	{
		for (int i = 0; i < 7; i++)
		{
			GameProgress.DeleteKey($"cake_race_track_{i}_pb_replay");
		}
	}

	private void ReportCupScore(int cupIndex)
	{
		int num = 0;
		for (int i = 0; i < 7; i++)
		{
			string text = $"cake_race_track_{i}_pb_replay";
			if (GameProgress.HasKey(text))
			{
				CakeRaceReplay cakeRaceReplay = new CakeRaceReplay(GameProgress.GetString(text, string.Empty));
				if (cakeRaceReplay.IsValid)
				{
					int num2 = CakeRaceReplay.TotalScore(cakeRaceReplay);
					num += num2;
					Debug.LogWarning("[CakeRaceMode] Track (" + text + ") score " + num2);
				}
			}
		}
		PlayFabLeaderboard.Leaderboard board = (PlayFabLeaderboard.Leaderboard)cupIndex;
		Debug.LogWarning("[CakeRaceMode] ReportCupScore [" + board.ToString() + "] " + num);
		Singleton<PlayFabManager>.Instance.Leaderboard.AddScore(board, num, OnCupScoreReported, OnCupScoreError);
	}

	private void OnCupScoreReported(UpdatePlayerStatisticsResult result)
	{
		Debug.LogWarning("[CakeRaceMode] OnCupScoreReported");
	}

	private void OnCupScoreError(PlayFabError error)
	{
		Debug.LogWarning("[CakeRaceMode] OnCupScoreError: " + error.ErrorMessage);
	}

	public static string GetCurrentSeasonID()
	{
		int @int = GameProgress.GetInt("cake_race_current_week");
		return $"{@int:0000}_{Singleton<TimeManager>.Instance.ServerTime.Year:0000}";
	}

	private IEnumerator EndingSequence(bool waitToExplode)
	{
		GadgetButtonList buttonList = levelManager.InGameGUI.FlightMenu.ButtonList;
		for (int i = 0; i < buttonList.Buttons.Count; i++)
		{
			buttonList.Buttons[i].Lock(lockState: true);
		}
		CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.MoveObject(buttonList.transform, buttonList.transform.position + Vector3.down * 4f, 1.5f));
		(levelManager.ContraptionRunning.FindPig() as Pig).SetExpression(Pig.Expressions.Fear);
		levelManager.InGameGUI.CakeRaceHUD.SetTimeBombMode(CakeRaceHUD.TimerMode.TimesUp, clearAnimations: true, loopAnimation: false);
		if (waitToExplode)
		{
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(base.gameData.commonAudioCollection.timeBombAlarm[0]);
		}
		levelManager.SetGameState(LevelManager.GameState.CakeRaceExploding);
		if (waitToExplode)
		{
			yield return new WaitForSeconds(3f);
		}
		if (showBombParticles)
		{
			ShowTimeBombParticles();
		}
		levelManager.SetGameState(LevelManager.GameState.CakeRaceCompleted);
	}

	private void ShowTimeBombParticles()
	{
		BasePart basePart = base.ContraptionRunning.FindPart(BasePart.PartType.TimeBomb);
		bool num = WPFMonoBehaviour.mainCamera.transform.position.x < basePart.transform.position.x;
		Vector3 vector = ((!num) ? new Vector3(-19f, 0f, 5f) : new Vector3(19f, 0f, 5f));
		Vector3 euler = ((!num) ? new Vector3(0f, 0f, -45f) : new Vector3(0f, 0f, 45f));
		GameObject gameObject = UnityEngine.Object.Instantiate(base.gameData.m_cakeRaceBombParticles);
		gameObject.transform.position = WPFMonoBehaviour.hudCamera.transform.position + vector;
		gameObject.transform.rotation = Quaternion.Euler(euler);
	}

	private void RewardCrate(int rewardIndex)
	{
		LootCrateType lootCrateType = LootCrateType.Cardboard;
		if (Singleton<GameConfigurationManager>.Instance.HasValue("cake_race", "loot_crates"))
		{
			string[] array = Singleton<GameConfigurationManager>.Instance.GetValue<string>("cake_race", "loot_crates").Split(',');
			int num = rewardIndex % array.Length;
			if (array != null && array.Length != 0 && num >= 0 && int.TryParse(array[num], out var result))
			{
				lootCrateType = (LootCrateType)result;
			}
		}
		CurrentRewardCrate = lootCrateType;
		if (lootCrateType != LootCrateType.None)
		{
			LootCrateSlots.AddLootCrateToFreeSlot(lootCrateType);
		}
	}

	private void AddScore(int amount)
	{
		CurrentScore += amount;
		if (ScoreUpdated != null)
		{
			ScoreUpdated(CurrentScore);
		}
	}

	private void CreateCakes()
	{
		CakeRaceInfo? cakeRaceInfo = CakeRaceMode.cakeRaceInfo;
		if (cakeRaceInfo.HasValue && cakes == null)
		{
			cakes = new List<Cake>();
			for (int i = 0; i < CakeRaceMode.cakeRaceInfo.Value.CakeLocations.Length; i++)
			{
				ObjectLocation objectLocation = CakeRaceMode.cakeRaceInfo.Value.CakeLocations[i];
				Cake component = UnityEngine.Object.Instantiate(objectLocation.Prefab, objectLocation.Position, objectLocation.Rotation).GetComponent<Cake>();
				Debug.LogWarning("Add cake collected listener " + i);
				component.OnCakeCollected += OnCakeCollected;
				cakes.Add(component);
			}
		}
	}

	private void CreateProps()
	{
		CakeRaceInfo? cakeRaceInfo = CakeRaceMode.cakeRaceInfo;
		if (cakeRaceInfo.HasValue)
		{
			for (int i = 0; i < CakeRaceMode.cakeRaceInfo.Value.Props.Length; i++)
			{
				ObjectLocation objectLocation = CakeRaceMode.cakeRaceInfo.Value.Props[i];
				UnityEngine.Object.Instantiate(objectLocation.Prefab, objectLocation.Position, objectLocation.Rotation);
			}
		}
	}

	private void InstantiateObjects(ObjectLocation[] locationData)
	{
	}

	private void ResetCakes()
	{
		for (int i = 0; i < cakes.Count; i++)
		{
			if (cakes[i] != null)
			{
				cakes[i].Reset();
			}
		}
	}

	private bool FindCakeRaceInfo(int trackIndex)
	{
		switch (base.gameManager.CurrentEpisodeType)
		{
		case GameManager.EpisodeType.Normal:
		{
			int currentEpisodeIndex = base.gameManager.CurrentEpisodeIndex;
			int currentLevel = base.gameManager.CurrentLevel;
			int trackCount2 = base.gameData.m_cakeRaceData.GetTrackCount(currentEpisodeIndex, currentLevel);
			if (trackIndex >= trackCount2)
			{
				trackIndex = 0;
			}
			else if (trackIndex < 0)
			{
				trackIndex = trackCount2 - 1;
			}
			if (!base.gameData.m_cakeRaceData.GetInfo(currentEpisodeIndex, currentLevel, trackIndex, out cakeRaceInfo))
			{
				return false;
			}
			break;
		}
		case GameManager.EpisodeType.Sandbox:
		case GameManager.EpisodeType.Race:
		{
			string currentLevelIdentifier = base.gameManager.CurrentLevelIdentifier;
			int trackCount = base.gameData.m_cakeRaceData.GetTrackCount(currentLevelIdentifier);
			Debug.LogWarning("[CakeRaceMode] track index " + trackIndex + ", track count " + trackCount);
			if (trackIndex >= trackCount)
			{
				trackIndex = 0;
			}
			else if (trackIndex < 0)
			{
				trackIndex = trackCount - 1;
			}
			if (!base.gameData.m_cakeRaceData.GetInfo(currentLevelIdentifier, trackIndex, out cakeRaceInfo))
			{
				return false;
			}
			break;
		}
		default:
			return false;
		}
		currentRaceTrackIndex = trackIndex;
		return true;
	}

	private void SetNextTrack()
	{
		FindCakeRaceInfo(currentRaceTrackIndex + 1);
	}

	private void SetPreviousTrack()
	{
		FindCakeRaceInfo(currentRaceTrackIndex - 1);
	}

	private List<CameraPreview.CameraControlPoint> CreatePreview()
	{
		List<CameraPreview.CameraControlPoint> list = new List<CameraPreview.CameraControlPoint>();
		for (int num = cakes.Count - 1; num >= 0; num--)
		{
			list.Add(new CameraPreview.CameraControlPoint
			{
				easing = CameraPreview.EasingAnimation.EasingInOut,
				position = cakes[num].transform.position,
				zoom = ((num >= cakeRaceInfo.Value.CakeZoomLevels.Length) ? 6f : cakeRaceInfo.Value.CakeZoomLevels[num])
			});
		}
		CameraPreview.CameraControlPoint cameraControlPoint = new CameraPreview.CameraControlPoint();
		cameraControlPoint.easing = CameraPreview.EasingAnimation.EasingInOut;
		cameraControlPoint.position = cakeRaceInfo.Value.Start.Position;
		cakeRaceInfo.Value.Start.GetGridSize(out var columns, out var rows);
		cameraControlPoint.zoom = (float)Mathf.Max(columns, rows) * 1f;
		cameraControlPoint.position += Vector2.up * rows * 0.45f;
		list.Add(cameraControlPoint);
		return list;
	}

	private void OnCakeCollected(Cake cake)
	{
		if (CakeCollected != null)
		{
			CakeCollected(0);
		}
		if (!cake.CollectedByOtherPlayer)
		{
			CollectedCakes++;
			if (CollectedCakes >= cakes.Count)
			{
				EndRace(cake.CakeIndex);
				return;
			}
			int collectTime = RaceTimeLeftInHundrethOfSeconds();
			CurrentReplay.SetCollectedCake(cake.CakeIndex, collectTime);
			AddScore(CakeRaceReplay.CalculateCakeScore(explosion: false, collectTime, Singleton<PlayerProgress>.Instance.Level, HasKingsFavoritePart()));
		}
	}

	public int RaceTimeLeftInHundrethOfSeconds()
	{
		return Mathf.FloorToInt(RaceTimeLeft * 100f);
	}

	private void OnTimeOut()
	{
	}

	public override int GetPartCount(BasePart.PartType type)
	{
		if (parts == null)
		{
			InitParts();
		}
		for (int i = 0; i < parts.Count; i++)
		{
			if (parts[i].type == type)
			{
				return parts[i].count;
			}
		}
		return 0;
	}
}
