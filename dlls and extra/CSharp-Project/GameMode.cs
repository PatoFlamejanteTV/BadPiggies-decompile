using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode
{
	protected LevelManager levelManager;

	protected float resourceUnloadTimer;

	protected bool tutorialBookOpened;

	protected LevelStart levelStart;

	protected GameManager gameManager => Singleton<GameManager>.Instance;

	protected GameData gameData => WPFMonoBehaviour.gameData;

	public Contraption ContraptionProto { get; protected set; }

	public Contraption ContraptionRunning { get; protected set; }

	public int CurrentContraptionIndex { get; set; }

	public List<int> CurrentConstructionGridRows { get; protected set; }

	public List<CameraPreview.CameraControlPoint> Preview { get; protected set; }

	public Vector3 CameraOffset { get; protected set; }

	public Vector3 PreviewOffset { get; protected set; }

	public Vector3 ConstructionOffset { get; protected set; }

	public LevelManager.CameraLimits CameraLimits { get; protected set; }

	public GameObject GridCellPrefab { get; protected set; }

	public GameObject TutorialPage { get; protected set; }

	public void Initialize(LevelManager newLevelManager)
	{
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		EventManager.Connect<Pig.PigOutOfBounds>(OnPigOutOfBounds);
		levelManager = newLevelManager;
		levelStart = WPFMonoBehaviour.FindSceneObjectOfType<LevelStart>();
		Vector3 position = ((!levelStart) ? Vector3.zero : levelStart.transform.position);
		if ((bool)gameData.m_contraptionPrefab)
		{
			Transform transform = Object.Instantiate(gameData.m_contraptionPrefab, position, Quaternion.identity);
			ContraptionProto = transform.GetComponent<Contraption>();
		}
		if ((bool)gameData.m_hudPrefab)
		{
			Object.Instantiate(gameData.m_hudPrefab, position, Quaternion.identity).parent = levelManager.transform;
		}
		if (!ContraptionProto)
		{
			ContraptionProto = WPFMonoBehaviour.FindSceneObjectOfType<Contraption>();
		}
	}

	public virtual void CleanUp()
	{
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
		EventManager.Disconnect<Pig.PigOutOfBounds>(OnPigOutOfBounds);
	}

	private void ReceiveUIEvent(UIEvent data)
	{
		if (HandleUIEvent(data))
		{
			return;
		}
		switch (data.type)
		{
		case UIEvent.Type.Building:
			if (resourceUnloadTimer > 600f)
			{
				resourceUnloadTimer = 0f;
				Resources.UnloadUnusedAssets();
			}
			if (levelManager.m_sandbox && levelManager.ConstructionUI.UnlockedParts.Count > 0 && !INSettings.GetBool(INFeature.PartCounter))
			{
				StopRunningContraption();
				levelManager.SetGameState(LevelManager.GameState.ShowingUnlockedParts);
			}
			else
			{
				levelManager.SetGameState(LevelManager.GameState.Building);
			}
			break;
		case UIEvent.Type.Play:
			if (!levelManager.ConstructionUI.IsDragging())
			{
				levelManager.CheckForLevelStartAchievements();
				LevelManager.GameState gameState5 = ((levelManager.gameState != LevelManager.GameState.Building) ? LevelManager.GameState.Continue : LevelManager.GameState.Running);
				if (levelManager.gameState == LevelManager.GameState.Building)
				{
					Singleton<AudioManager>.Instance.Play2dEffect(gameData.commonAudioCollection.buildContraption);
				}
				levelManager.SetGameState(gameState5);
			}
			break;
		case UIEvent.Type.LevelSelection:
			levelManager.SetGameState(LevelManager.GameState.Undefined);
			if (Singleton<GameManager>.Instance.CurrentEpisode != string.Empty)
			{
				Singleton<GameManager>.Instance.LoadLevelSelection(Singleton<GameManager>.Instance.CurrentEpisode, showLoadingScreen: true);
			}
			else
			{
				Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: true);
			}
			break;
		case UIEvent.Type.NextLevel:
			levelManager.SetGameState(LevelManager.GameState.Undefined);
			Singleton<GameManager>.Instance.LoadNextLevel();
			break;
		case UIEvent.Type.Preview:
		{
			LevelManager.GameState gameState2 = ((levelManager.gameState != LevelManager.GameState.Running) ? LevelManager.GameState.PreviewWhileBuilding : LevelManager.GameState.PreviewWhileRunning);
			levelManager.SetGameState(gameState2);
			break;
		}
		case UIEvent.Type.Clear:
			if (INSettings.GetBool(INFeature.AutoSaveWhenClearingGrid))
			{
				ContraptionProto.CreateAndSaveContraption(GetCurrentContraptionName());
			}
			levelManager.ConstructionUI.ClearContraption();
			break;
		case UIEvent.Type.Pause:
		{
			LevelManager.GameState gameState3 = ((levelManager.gameState != LevelManager.GameState.Running) ? LevelManager.GameState.PausedWhileBuilding : LevelManager.GameState.PausedWhileRunning);
			levelManager.SetGameState(gameState3);
			break;
		}
		case UIEvent.Type.ReplayLevel:
			levelManager.SetGameState(LevelManager.GameState.Undefined);
			if (levelManager.m_darkLevel)
			{
				LightManager.enabledLightPositions = new List<Vector3>();
				PointLightSource[] array = Object.FindObjectsOfType<PointLightSource>();
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].isEnabled)
					{
						LightManager.enabledLightPositions.Add(array[j].transform.position);
					}
				}
			}
			Singleton<GameManager>.Instance.ReloadCurrentLevel(showLoadingScreen: true);
			break;
		case UIEvent.Type.ActivateRockets:
		{
			Rocket[] componentsInChildren = ContraptionRunning.GetComponentsInChildren<Rocket>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ProcessTouch();
			}
			break;
		}
		case UIEvent.Type.ActivateEngines:
		{
			Engine[] componentsInChildren2 = ContraptionRunning.GetComponentsInChildren<Engine>();
			if (componentsInChildren2.Length != 0)
			{
				componentsInChildren2[0].ProcessTouch();
			}
			else
			{
				ContraptionRunning.m_pig.ProcessTouch();
			}
			break;
		}
		case UIEvent.Type.BackFromPreview:
		{
			LevelManager.GameState gameState4 = ((levelManager.gameState != LevelManager.GameState.PreviewWhileRunning) ? LevelManager.GameState.Building : LevelManager.GameState.Continue);
			levelManager.SetGameState(gameState4);
			break;
		}
		case UIEvent.Type.ContinueFromPause:
		{
			LevelManager.GameState gameState = ((levelManager.gameState != LevelManager.GameState.PausedWhileRunning) ? LevelManager.GameState.Building : LevelManager.GameState.Continue);
			levelManager.SetGameState(gameState);
			break;
		}
		case UIEvent.Type.ReplayFlight:
			StopRunningContraption();
			levelManager.SetGameState(LevelManager.GameState.Running);
			break;
		case UIEvent.Type.QuestModeBuild:
			levelManager.PlaceBuildArea();
			levelManager.SetGameState(LevelManager.GameState.Building);
			break;
		case UIEvent.Type.OpenTutorial:
			levelManager.StateBeforeTutorial = levelManager.gameState;
			levelManager.SetGameState(LevelManager.GameState.TutorialBook);
			Singleton<AudioManager>.Instance.Play2dEffect(gameData.commonAudioCollection.tutorialIn);
			GameProgress.IncreaseTutorialBookOpenCount();
			tutorialBookOpened = true;
			break;
		case UIEvent.Type.CloseTutorial:
			Singleton<AudioManager>.Instance.Play2dEffect(gameData.commonAudioCollection.tutorialOut);
			if (levelManager.HasCompleted)
			{
				levelManager.SetGameState(LevelManager.GameState.Completed);
				levelManager.InGameGUI.LevelCompleteMenu.ResumeAnimations();
			}
			else
			{
				levelManager.SetGameState(levelManager.StateBeforeTutorial);
			}
			break;
		case UIEvent.Type.Snapshot:
			levelManager.SetGameState(LevelManager.GameState.Snapshot);
			break;
		case UIEvent.Type.EpisodeSelection:
			Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: true);
			break;
		case UIEvent.Type.ApplySuperGlue:
		{
			if (!levelManager.m_SuperGlueAllowed)
			{
				break;
			}
			int num2 = GameProgress.SuperGlueCount();
			if (num2 > 0 || ContraptionProto.HasRegularGlue)
			{
				num2 += (ContraptionProto.HasRegularGlue ? 1 : (-1));
				GameProgress.SetSuperGlueCount(num2);
				GameProgress.Save();
				if (ContraptionProto.HasRegularGlue)
				{
					ContraptionProto.RemoveSuperGlue();
					if (!INSettings.GetBool(INFeature.NewAlienEgg) && ContraptionProto.HasPart(BasePart.PartType.Egg, BasePart.PartTier.Legendary))
					{
						ContraptionProto.ApplySuperGlue(Glue.Type.Alien);
					}
				}
				else
				{
					ContraptionProto.ApplySuperGlue(Glue.Type.Regular);
				}
				EventManager.Send(new InGameBuildMenu.ApplySuperGlueEvent(num2, ContraptionProto.HasRegularGlue));
			}
			else
			{
				levelManager.ShowPurchaseDialog(IapManager.InAppPurchaseItemType.SuperGlueSingle);
			}
			break;
		}
		case UIEvent.Type.ApplySuperMagnet:
			if (levelManager.m_SuperMagnetAllowed)
			{
				int num4 = GameProgress.SuperMagnetCount();
				if (num4 > 0 || ContraptionProto.HasSuperMagnet)
				{
					num4 += (ContraptionProto.HasSuperMagnet ? 1 : (-1));
					GameProgress.SetSuperMagnetCount(num4);
					GameProgress.Save();
					ContraptionProto.HasSuperMagnet = !ContraptionProto.HasSuperMagnet;
					EventManager.Send(new InGameBuildMenu.ApplySuperMagnetEvent(num4, ContraptionProto.HasSuperMagnet));
				}
				else
				{
					levelManager.ShowPurchaseDialog(IapManager.InAppPurchaseItemType.SuperMagnetSingle);
				}
			}
			break;
		case UIEvent.Type.ApplyTurboCharge:
			if (levelManager.m_TurboChargeAllowed)
			{
				int num3 = GameProgress.TurboChargeCount();
				if (num3 > 0 || ContraptionProto.HasTurboCharge)
				{
					num3 += (ContraptionProto.HasTurboCharge ? 1 : (-1));
					GameProgress.SetTurboChargeCount(num3);
					GameProgress.Save();
					ContraptionProto.HasTurboCharge = !ContraptionProto.HasTurboCharge;
					EventManager.Send(new InGameBuildMenu.ApplyTurboChargeEvent(num3, ContraptionProto.HasTurboCharge));
				}
				else
				{
					levelManager.ShowPurchaseDialog(IapManager.InAppPurchaseItemType.TurboChargeSingle);
				}
			}
			break;
		case UIEvent.Type.LoadContraptionSlot1:
			LoadContraptionFromSlot(0);
			break;
		case UIEvent.Type.LoadContraptionSlot2:
			LoadContraptionFromSlot(1);
			break;
		case UIEvent.Type.LoadContraptionSlot3:
			LoadContraptionFromSlot(2);
			break;
		case UIEvent.Type.ApplyNightVision:
			if (levelManager.m_darkLevel && levelManager.LightManager != null)
			{
				int num = GameProgress.NightVisionCount();
				if (num > 0 || ContraptionProto.HasNightVision)
				{
					num += (ContraptionProto.HasNightVision ? 1 : (-1));
					GameProgress.SetNightVisionCount(num);
					GameProgress.Save();
					ContraptionProto.HasNightVision = !ContraptionProto.HasNightVision;
					EventManager.Send(new InGameBuildMenu.ApplyNightVisionEvent(num, ContraptionProto.HasNightVision));
				}
				else if (ContraptionProto.HasNightVision)
				{
					levelManager.ConstructionUI.ApplyNightVision(apply: false);
					num++;
					GameProgress.SetNightVisionCount(num);
					GameProgress.Save();
					EventManager.Send(new InGameBuildMenu.ApplyNightVisionEvent(num, ContraptionProto.HasNightVision));
				}
				else
				{
					levelManager.ShowPurchaseDialog(IapManager.InAppPurchaseItemType.NightVisionSingle);
				}
			}
			break;
		case UIEvent.Type.Home:
		case UIEvent.Type.Blueprint:
		case UIEvent.Type.IapPurchaseCurrency:
		case UIEvent.Type.IapPurchaseRocket:
		case UIEvent.Type.IapPurchaseEngine:
		case UIEvent.Type.OpenIapMenu:
		case UIEvent.Type.CloseIapMenu:
		case UIEvent.Type.MoveContraptionLeft:
		case UIEvent.Type.MoveContraptionRight:
		case UIEvent.Type.MoveContraptionUp:
		case UIEvent.Type.MoveContraptionDown:
		case UIEvent.Type.CloseMechanicInfo:
		case UIEvent.Type.CloseMechanicInfoAndUseMechanic:
		case UIEvent.Type.OpenUnlockFullVersionIapMenu:
		case UIEvent.Type.CloseUnlockFullVersionIapMenu:
		case UIEvent.Type.SuperBlueprint:
		case UIEvent.Type.RotateSuperBluePrints:
		case UIEvent.Type.CloseMechanicInfoAndUseSuperMechanic:
		case UIEvent.Type.UnlockNextLevelWithVideoAd:
			break;
		}
	}

	public virtual void Update()
	{
		resourceUnloadTimer += Time.deltaTime;
	}

	public virtual void NotifyGoalReachedByPart(BasePart.PartType partType)
	{
		if (levelManager.PartsInGoal.Contains(partType))
		{
			return;
		}
		levelManager.PartsInGoal.Add(partType);
		if (partType == BasePart.PartType.Pig)
		{
			if (levelManager.EggRequired && !levelManager.PartsInGoal.Contains(BasePart.PartType.Egg))
			{
				ContraptionRunning.SetCameraTarget(ContraptionRunning.FindPart(BasePart.PartType.Egg));
			}
			if (levelManager.PumpkinRequired && !levelManager.PartsInGoal.Contains(BasePart.PartType.Pumpkin))
			{
				ContraptionRunning.SetCameraTarget(ContraptionRunning.FindPart(BasePart.PartType.Pumpkin));
			}
		}
	}

	public virtual bool PlayerHasRequiredObjects()
	{
		if (levelManager.EggRequired)
		{
			if (levelManager.PartsInGoal.Contains(BasePart.PartType.Pig))
			{
				return levelManager.PartsInGoal.Contains(BasePart.PartType.Egg);
			}
			return false;
		}
		if (levelManager.PumpkinRequired)
		{
			if (levelManager.PartsInGoal.Contains(BasePart.PartType.Pig))
			{
				return levelManager.PartsInGoal.Contains(BasePart.PartType.Pumpkin);
			}
			return false;
		}
		return levelManager.PartsInGoal.Contains(BasePart.PartType.Pig);
	}

	public virtual void NotifyGoalReached()
	{
		levelManager.CompletionTime = levelManager.TimeElapsed;
		levelManager.HasCompleted = true;
		levelManager.SetGameState(LevelManager.GameState.Completed);
	}

	protected virtual void OnPigOutOfBounds(Pig.PigOutOfBounds data)
	{
		levelManager.SetGameState(LevelManager.GameState.Building);
	}

	protected void LoadContraptionFromSlot(int slotIndex)
	{
		if ((bool)ContraptionProto)
		{
			if (ContraptionProto.HasTurboCharge)
			{
				GameProgress.AddTurboCharge(1);
				EventManager.Send(new InGameBuildMenu.ApplyTurboChargeEvent(GameProgress.TurboChargeCount(), usedState: false));
			}
			if (ContraptionProto.HasNightVision)
			{
				GameProgress.AddNightVision(1);
				EventManager.Send(new InGameBuildMenu.ApplyNightVisionEvent(GameProgress.NightVisionCount(), usedState: false));
			}
			if (ContraptionProto.HasSuperGlue)
			{
				GameProgress.AddSuperGlue(1);
				EventManager.Send(new InGameBuildMenu.ApplySuperGlueEvent(GameProgress.SuperGlueCount(), usedState: false));
			}
			if (ContraptionProto.HasSuperMagnet)
			{
				GameProgress.AddSuperMagnet(1);
				EventManager.Send(new InGameBuildMenu.ApplySuperMagnetEvent(GameProgress.SuperMagnetCount(), usedState: false));
			}
			foreach (BasePart part in ContraptionProto.Parts)
			{
				ContraptionProto.DataSet.AddPart(part.m_coordX, part.m_coordY, (int)part.m_partType, part.customPartIndex, part.m_gridRotation, part.m_flipped);
			}
			ContraptionProto.SaveContraption(GetCurrentContraptionName());
			levelManager.ConstructionUI.ClearContraption();
			Destroy(ContraptionProto.gameObject);
			ContraptionProto = null;
		}
		CurrentContraptionIndex = slotIndex;
		Vector3 position = ((!levelStart) ? Vector3.zero : levelStart.transform.position);
		if ((bool)gameData.m_contraptionPrefab)
		{
			Transform transform = Object.Instantiate(gameData.m_contraptionPrefab, position, Quaternion.identity);
			ContraptionProto = transform.GetComponent<Contraption>();
		}
		levelManager.ConstructionUI.SetCurrentContraption();
		BuildContraption(WPFPrefs.LoadContraptionDataset(GetCurrentContraptionName()));
		foreach (ConstructionUI.PartDesc partDescriptor in levelManager.ConstructionUI.PartDescriptors)
		{
			EventManager.Send(new PartCountChanged(partDescriptor.part.m_partType, partDescriptor.CurrentCount));
		}
		levelManager.ConstructionUI.SetMoveButtonStates();
		levelManager.SetGameState(LevelManager.GameState.Building);
	}

	protected void StopRunningContraption()
	{
		if ((bool)ContraptionRunning)
		{
			ContraptionRunning.StopContraption();
			Destroy(ContraptionRunning.gameObject);
			ContraptionRunning = null;
		}
		foreach (GameObject item in new List<GameObject>(GameObject.FindGameObjectsWithTag("ParticleEmitter")))
		{
			Destroy(item);
		}
	}

	public string GetContraptionNameAtSlot(int slotIndex)
	{
		if (levelManager.m_sandbox && CurrentContraptionIndex > 0)
		{
			return $"{Singleton<GameManager>.Instance.CurrentSceneName}_{CurrentContraptionIndex}";
		}
		if (this is CakeRaceMode)
		{
			int currentTrackIndex = (this as CakeRaceMode).CurrentTrackIndex;
			return $"cr_{Singleton<GameManager>.Instance.CurrentSceneName}_{currentTrackIndex}";
		}
		return Singleton<GameManager>.Instance.CurrentSceneName;
	}

	public string GetCurrentContraptionName()
	{
		return GetContraptionNameAtSlot(CurrentContraptionIndex);
	}

	public void BuildContraption(ContraptionDataset cds)
	{
		if (cds == null || cds.ContraptionDatasetList == null)
		{
			return;
		}
		foreach (ContraptionDataset.ContraptionDatasetUnit contraptionDataset in cds.ContraptionDatasetList)
		{
			ConstructionUI.PartDesc partDesc = levelManager.ConstructionUI.FindPartDesc((BasePart.PartType)contraptionDataset.partType);
			if (partDesc != null)
			{
				BasePart customPart = gameData.GetCustomPart(partDesc.part.m_partType, contraptionDataset.customPartIndex);
				if (customPart != null)
				{
					levelManager.BuildPart(contraptionDataset, customPart);
					partDesc.useCount++;
				}
			}
		}
	}

	private void PreBuildContraption(ContraptionDataset cds)
	{
		foreach (ContraptionDataset.ContraptionDatasetUnit contraptionDataset in cds.ContraptionDatasetList)
		{
			GameObject part = gameData.GetPart((BasePart.PartType)contraptionDataset.partType);
			if ((bool)part)
			{
				BasePart component = part.GetComponent<BasePart>();
				levelManager.BuildPart(contraptionDataset, component).m_static = true;
				ContraptionProto.IncreaseStaticPartCount();
			}
		}
	}

	protected void Destroy(GameObject go)
	{
		Object.Destroy(go);
	}

	public abstract void OnDataLoadedStart();

	public abstract void InitGameMode();

	public abstract void OnDataLoadedDone();

	public abstract LevelManager.GameState SetGameState(LevelManager.GameState currentState, LevelManager.GameState newState);

	protected abstract bool HandleUIEvent(UIEvent data);

	public abstract int GetPartCount(BasePart.PartType type);
}
