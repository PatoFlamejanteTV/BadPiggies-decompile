using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseGameMode : GameMode
{
	private int retries;

	private bool openTutorial;

	private bool openMechanicGift;

	private bool useBlueprint;

	private bool useSuperBlueprint;

	private const string MECHANIC_PROMOTION_COUNT = "Mechanic_Promotion_Count";

	private const string MECHANIC_PROMOTION_TIME = "Last_Mechanic_Promotion_Time_Binary";

	private const string TUTORIAL_PROMOTION_COUNT = "Tutorial_Promotion_Count";

	private const string BRANDED_REWARD_GIFT_COUNT = "branded_reward_gifts_today";

	private const string BRANDED_REWARD_GIFT_TIME = "branded_reward_gift_time";

	private const string BRANDED_REWARD_COOLDOWN = "branded_reward_cooldown";

	private string TutorialPromotionCount => string.Format("{0}_{1}", "Tutorial_Promotion_Count", Singleton<GameManager>.Instance.CurrentSceneName);

	private string MechanicPromotionCount => string.Format("{0} {1}", "Mechanic_Promotion_Count", Singleton<GameManager>.Instance.CurrentSceneName);

	public override void OnDataLoadedStart()
	{
		PartListing.Create().Close();
		int @int = GameProgress.GetInt("branded_reward_gift_time");
		int num = 86400;
		if (Singleton<GameConfigurationManager>.IsInstantiated() && Singleton<GameConfigurationManager>.Instance.HasValue("branded_reward_cooldown", "time"))
		{
			num = Singleton<GameConfigurationManager>.Instance.GetValue<int>("branded_reward_cooldown", "time");
		}
		if (@int > 0 && Singleton<TimeManager>.Instance.CurrentEpochTime - @int > num)
		{
			GameProgress.DeleteKey("branded_reward_gift_time");
			GameProgress.DeleteKey("branded_reward_gifts_today");
		}
	}

	public override void InitGameMode()
	{
		base.CurrentConstructionGridRows = levelManager.m_constructionGridRows;
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
		Vector3 position = ((!levelStart) ? Vector3.zero : levelStart.transform.position);
		levelManager.CreateGrid(num, num2, newGridXMin, newGridXMax, position);
		if ((bool)levelManager.ConstructionUI)
		{
			if (GameProgress.HasKey(SchematicButton.LastLoadedSlotKey))
			{
				base.CurrentContraptionIndex = GameProgress.GetInt(SchematicButton.LastLoadedSlotKey);
			}
			BuildContraption(WPFPrefs.LoadContraptionDataset(GetCurrentContraptionName()));
		}
		foreach (ConstructionUI.PartDesc partDescriptor in levelManager.ConstructionUI.PartDescriptors)
		{
			EventManager.Send(new PartCountChanged(partDescriptor.part.m_partType, partDescriptor.CurrentCount));
		}
		new GameObject("CollectibleStash").transform.parent = levelManager.transform;
		FindChallenges();
		levelManager.m_CollectedDessertsCount = 0;
		PlaceDesserts();
		InitializeChallenges();
	}

	public override void OnDataLoadedDone()
	{
		levelManager.m_autoBuildUnlocked = levelManager.m_oneStarContraption != null;
	}

	private void FindChallenges()
	{
		levelManager.Challenges = Challenge.Challenges;
	}

	private void InitializeChallenges()
	{
		for (int i = 0; i < levelManager.Challenges.Count; i++)
		{
			Challenge challenge = levelManager.Challenges[i];
			if (challenge.TimeLimit() > 0f)
			{
				levelManager.TimeLimits.Add(challenge.TimeLimit());
				if (levelManager.TimeLimit == 0f || challenge.TimeLimit() > levelManager.TimeLimit)
				{
					levelManager.TimeLimit = challenge.TimeLimit();
					levelManager.OriginalTimeLimit = levelManager.TimeLimit;
				}
			}
		}
	}

	private void PlaceDesserts(bool forceFillAllPlaces = false)
	{
		if (!GameProgress.GetBool("ChiefPigExploded") && !forceFillAllPlaces && !INSettings.GetBool(INFeature.EnableDesserts))
		{
			return;
		}
		GameObject gameObject = GameObject.Find("DessertPlaces");
		if (gameObject == null || (!forceFillAllPlaces && levelManager.LoadDessertsPlacement(gameObject)))
		{
			return;
		}
		levelManager.UsedDessertPlaces.Clear();
		int max = base.gameData.m_desserts.Count - 1;
		DessertPlace[] array = Object.FindObjectsOfType<DessertPlace>();
		if (array.Length == 0)
		{
			return;
		}
		int num = array.Length - 1;
		while (--num >= 1)
		{
			int num2 = Random.Range(0, num + 1);
			DessertPlace dessertPlace = array[num];
			array[num] = array[num2];
			array[num2] = dessertPlace;
		}
		int levelDessertsCount = levelManager.LevelDessertsCount;
		int num3 = ((levelDessertsCount <= array.Length && !forceFillAllPlaces) ? levelDessertsCount : array.Length);
		int num4 = -1;
		if (Random.Range(0, 100) == 50)
		{
			num4 = Random.Range(0, num3);
		}
		for (int i = 0; i < num3; i++)
		{
			Transform transform = array[i].transform;
			GameObject gameObject2 = base.gameData.m_desserts[Random.Range(0, max)];
			gameObject2 = ((num4 != i) ? base.gameData.m_desserts[Random.Range(0, max)] : base.gameData.m_desserts[base.gameData.m_desserts.Count - 1]);
			GameObject gameObject3 = Object.Instantiate(gameObject2, transform.position, transform.rotation);
			gameObject3.name = gameObject2.name;
			gameObject3.GetComponent<Dessert>().place = transform.GetComponent<DessertPlace>();
			levelManager.UsedDessertPlaces.Add(transform.name, gameObject3.GetComponent<Dessert>().saveId);
		}
		if (levelManager.UsedDessertPlaces.Count <= 0)
		{
			return;
		}
		int num5 = 0;
		string[] array2 = new string[levelManager.UsedDessertPlaces.Count];
		foreach (KeyValuePair<string, string> usedDessertPlace in levelManager.UsedDessertPlaces)
		{
			array2[num5] = usedDessertPlace.Key + ":" + usedDessertPlace.Value;
			num5++;
		}
		string value = string.Join(";", array2);
		GameProgress.SetString(Singleton<GameManager>.Instance.CurrentSceneName + "_dessert_placement", value);
	}

	public override void Update()
	{
		if (openTutorial && levelManager.gameState == LevelManager.GameState.Building)
		{
			openTutorial = false;
			EventManager.Send(new UIEvent(UIEvent.Type.OpenTutorial));
		}
		if (openMechanicGift && levelManager.gameState == LevelManager.GameState.Building)
		{
			openMechanicGift = false;
			levelManager.SetGameState(LevelManager.GameState.MechanicGiftScreen);
		}
		if (useBlueprint)
		{
			if (levelManager.gameState == LevelManager.GameState.Building && WPFMonoBehaviour.ingameCamera.IsShowingBuildGrid(0.1f))
			{
				useBlueprint = false;
				EventManager.Send(new UIEvent(UIEvent.Type.Blueprint));
			}
		}
		else if (useSuperBlueprint && levelManager.gameState == LevelManager.GameState.Building && WPFMonoBehaviour.ingameCamera.IsShowingBuildGrid(0.1f))
		{
			useSuperBlueprint = false;
			EventManager.Send(new UIEvent(UIEvent.Type.SuperBlueprint));
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
					if (@int < 3 && !GameProgress.IsLevelCompleted(Singleton<GameManager>.Instance.CurrentSceneName) && GameProgress.GetInt(TutorialPromotionCount) == 0)
					{
						openTutorial = true;
						@int++;
						GameProgress.SetInt("Tutorial_Promotion_Count", @int);
						GameProgress.SetInt(TutorialPromotionCount, 1);
					}
				}
				GameProgress.GetBool(Singleton<GameManager>.Instance.CurrentSceneName + "_autobuild_available");
				GameProgress.GetBool("PermanentBlueprint");
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
			if (!(GameProgress.GetString("REPLAY_LEVEL", string.Empty) == SceneManager.GetActiveScene().name) || LightManager.enabledLightPositions == null || LightManager.enabledLightPositions.Count <= 0)
			{
				break;
			}
			PointLightSource[] array = Object.FindObjectsOfType<PointLightSource>();
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
				base.ContraptionProto.HasTurboCharge = false;
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
		case LevelManager.GameState.Completed:
			levelManager.InGameGUI.ShowCurrentMenu(show: false);
			base.ContraptionRunning.TurnOffAllPoweredParts();
			levelManager.PlayVictorySound();
			if (levelManager.EggRequired)
			{
				levelManager.InGameGUI.LevelCompleteMenu.SetGoal(base.gameData.m_eggTransportGoal);
			}
			else if (levelManager.PumpkinRequired)
			{
				levelManager.InGameGUI.LevelCompleteMenu.SetGoal(base.gameData.m_pumpkinTransportGoal);
			}
			else
			{
				levelManager.InGameGUI.LevelCompleteMenu.SetGoal(base.gameData.m_basicGoal);
			}
			levelManager.InGameGUI.LevelCompleteMenu.SetChallenges(levelManager.Challenges);
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
			levelManager.UnlockedParts = new List<ConstructionUI.PartDesc>(levelManager.ConstructionUI.UnlockedParts);
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
		case UIEvent.Type.Building:
			levelManager.ConstructionUI.transform.position = levelManager.StartingPosition;
			levelManager.ContraptionProto.transform.position = levelManager.StartingPosition;
			levelManager.ConstructionUI.CheckUnlockedParts();
			break;
		case UIEvent.Type.LevelSelection:
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
			break;
		case UIEvent.Type.Blueprint:
			if (GameProgress.GetBool("PermanentBlueprint"))
			{
				if (levelManager.m_threeStarContraption.Count == 1)
				{
					GameProgress.SetBool(Singleton<GameManager>.Instance.CurrentSceneName + "_autobuild_available", value: true);
				}
				levelManager.SetGameState(LevelManager.GameState.AutoBuilding);
			}
			break;
		case UIEvent.Type.CloseMechanicInfo:
			levelManager.SetGameState(LevelManager.GameState.Building);
			break;
		case UIEvent.Type.CloseMechanicInfoAndUseMechanic:
			levelManager.SetGameState(LevelManager.GameState.Building);
			Singleton<GuiManager>.Instance.IsEnabled = false;
			levelManager.UseBlueprint = true;
			break;
		case UIEvent.Type.SuperBlueprint:
		{
			if (!levelManager.SuperBluePrintsAllowed || levelManager.m_threeStarContraption.Count <= 0 || levelManager.gameState != LevelManager.GameState.Building || !WPFMonoBehaviour.ingameCamera.IsShowingBuildGrid(1f))
			{
				break;
			}
			bool flag = GameProgress.GetBool(Singleton<GameManager>.Instance.CurrentSceneName + "_autobuild_available");
			int num = GameProgress.BluePrintCount();
			if (num == 0 && !flag)
			{
				Singleton<GuiManager>.Instance.IsEnabled = true;
				levelManager.ShowPurchaseDialog(IapManager.InAppPurchaseItemType.BlueprintSingle);
				break;
			}
			if (!flag && num > 0)
			{
				GameProgress.SetBluePrintCount(--num);
				GameProgress.SetBool(Singleton<GameManager>.Instance.CurrentSceneName + "_autobuild_available", value: true);
				flag = true;
				GameProgress.Save();
				EventManager.Send(new InGameBuildMenu.AutoBuildEvent(num, usedState: true));
			}
			GameObject superBuildSelection = levelManager.InGameGUI.BuildMenu.SuperBuildSelection;
			if (flag && !superBuildSelection.gameObject.activeSelf)
			{
				levelManager.InGameGUI.BuildMenu.AutoBuildButton.SetActive(value: false);
				superBuildSelection.SetActive(value: true);
			}
			if (flag)
			{
				EventManager.Send(new UIEvent(UIEvent.Type.RotateSuperBluePrints));
			}
			break;
		}
		case UIEvent.Type.RotateSuperBluePrints:
		{
			int count = levelManager.m_threeStarContraption.Count;
			string text = "ABCDEFGH";
			levelManager.CurrentSuperBluePrint++;
			if (levelManager.CurrentSuperBluePrint >= count)
			{
				levelManager.CurrentSuperBluePrint = 0;
			}
			GameObject superBuildSelection2 = levelManager.InGameGUI.BuildMenu.SuperBuildSelection;
			if (superBuildSelection2 != null)
			{
				Transform transform = superBuildSelection2.transform.Find("AmountText");
				Transform transform2 = superBuildSelection2.transform.Find("AmountTextShadow");
				if (transform.GetComponent<TextMesh>().text == string.Empty)
				{
					levelManager.CurrentSuperBluePrint = 0;
				}
				transform.GetComponent<TextMesh>().text = text[levelManager.CurrentSuperBluePrint].ToString();
				transform2.GetComponent<TextMesh>().text = text[levelManager.CurrentSuperBluePrint].ToString();
			}
			levelManager.SetGameState(LevelManager.GameState.SuperAutoBuilding);
			if (!levelManager.FirstTime)
			{
				levelManager.FastBuilding = true;
			}
			else
			{
				levelManager.FirstTime = false;
			}
			break;
		}
		case UIEvent.Type.CloseMechanicInfoAndUseSuperMechanic:
			levelManager.SetGameState(LevelManager.GameState.Building);
			if (levelManager.SuperBluePrintsAllowed && levelManager.m_threeStarContraption.Count > 0)
			{
				Singleton<GuiManager>.Instance.IsEnabled = false;
				levelManager.UseSuperBlueprint = true;
			}
			else
			{
				Singleton<GuiManager>.Instance.IsEnabled = true;
			}
			break;
		}
		return false;
	}

	public override int GetPartCount(BasePart.PartType type)
	{
		List<LevelManager.PartCount> partTypeCounts = levelManager.m_partTypeCounts;
		int num = 0;
		foreach (LevelManager.PartCount item in partTypeCounts)
		{
			if (item.type == type)
			{
				num += item.count;
				break;
			}
		}
		if (levelManager.m_sandbox)
		{
			if (!levelManager.m_collectPartBoxesSandbox)
			{
				num += GameProgress.GetSandboxPartCount(type);
			}
			num += GameProgress.GetSandboxPartCount(Singleton<GameManager>.Instance.CurrentSceneName, type);
		}
		return num;
	}
}
