using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomizePartWidget : Widget
{
	[HideInInspector]
	public Button closeButton;

	[SerializeField]
	private GameObject newTag;

	private PartListing partListing;

	private ConstructionUI constructionUI;

	private CustomizePartUI customizeMenu;

	private BasePart.PartType centerPart;

	private List<BasePart.PartType> scope;

	private Action<BasePart.PartType> onPartListClose;

	private void Start()
	{
		constructionUI = UnityEngine.Object.FindObjectOfType<ConstructionUI>();
		EventManager.Connect<PartSelectedEvent>(OnPartSelected);
		EventManager.Connect<LootCrateOpenDialog.LootCrateDelivered>(OnLootCrateDelivered);
		CreatePartListing();
		UpdateNewTagState();
		if (constructionUI != null)
		{
			ConstructionUI obj = constructionUI;
			obj.OnPartsUnlocked = (Action)Delegate.Combine(obj.OnPartsUnlocked, new Action(OnPartsUnlocked));
		}
	}

	private void OnDestroy()
	{
		if (constructionUI != null)
		{
			ConstructionUI obj = constructionUI;
			obj.OnPartsUnlocked = (Action)Delegate.Remove(obj.OnPartsUnlocked, new Action(OnPartsUnlocked));
		}
		EventManager.Disconnect<PartSelectedEvent>(OnPartSelected);
		EventManager.Disconnect<LootCrateOpenDialog.LootCrateDelivered>(OnLootCrateDelivered);
	}

	private void OnPartsUnlocked()
	{
		CreatePartScope(UnityEngine.Object.FindObjectOfType<LevelManager>());
		partListing.SetPartScope(scope);
	}

	private void OnPartSelected(PartSelectedEvent evnt)
	{
		centerPart = evnt.type;
	}

	private void OnLootCrateDelivered(LootCrateOpenDialog.LootCrateDelivered data)
	{
		UpdateNewTagState();
	}

	public void OpenCustomizationForPart(BasePart part, Action<BasePart.PartType> onClose = null)
	{
		onPartListClose = onClose;
		if ((bool)part)
		{
			centerPart = part.m_partType;
		}
		OpenPartList();
	}

	public void ClosePastList()
	{
		onPartListClose = null;
		if (closeButton != null)
		{
			closeButton.Activate();
		}
	}

	public void OpenPartList()
	{
		if (constructionUI != null)
		{
			constructionUI.SetEnabled(enableUI: false, enableGrid: false);
		}
		if (partListing == null)
		{
			CreatePartListing();
		}
		LevelManager levelManager = UnityEngine.Object.FindObjectOfType<LevelManager>();
		LevelManager.GameState previousState = LevelManager.GameState.Building;
		if (levelManager != null)
		{
			previousState = levelManager.gameState;
			levelManager.SetGameState(LevelManager.GameState.CustomizingPart);
		}
		partListing.CenterOnPart(centerPart);
		partListing.Open(delegate
		{
			if (constructionUI != null)
			{
				constructionUI.SetEnabled(enableUI: true, enableGrid: true);
				levelManager.SetGameState(previousState);
			}
			UpdateNewTagState();
		});
		if (customizeMenu == null)
		{
			customizeMenu = base.gameObject.AddComponent<CustomizePartUI>();
		}
		customizeMenu.customPartWidget = this;
		customizeMenu.InitButtons(partListing, onPartListClose);
		centerPart = BasePart.PartType.Unknown;
	}

	private void CreatePartListing()
	{
		if (!(partListing != null))
		{
			partListing = PartListing.Create();
			partListing.transform.parent = base.transform.parent.parent;
			partListing.Close();
			if (scope == null)
			{
				CreatePartScope(UnityEngine.Object.FindObjectOfType<LevelManager>());
			}
			partListing.SetPartScope(scope);
		}
	}

	private void CreatePartScope(LevelManager levelManager)
	{
		if (levelManager == null)
		{
			return;
		}
		scope = new List<BasePart.PartType>();
		for (int i = 0; i < levelManager.ConstructionUI.PartDescriptors.Count; i++)
		{
			scope.Add(levelManager.ConstructionUI.PartDescriptors[i].part.m_partType);
		}
		for (int j = 0; j < levelManager.ConstructionUI.UnlockedParts.Count; j++)
		{
			if (!scope.Contains(levelManager.ConstructionUI.UnlockedParts[j].part.m_partType))
			{
				scope.Add(levelManager.ConstructionUI.UnlockedParts[j].part.m_partType);
			}
		}
	}

	private bool IsRaceLevelUnlocked(BasePart.PartType type)
	{
		int num = GameProgress.GetAllStars() + GameProgress.GetRaceLevelUnlockedStars();
		string currentLevelIdentifier = Singleton<GameManager>.Instance.CurrentLevelIdentifier;
		foreach (RaceLevels.UnlockableTier tier in WPFMonoBehaviour.gameData.m_raceLevels.GetLevelUnlockableData(currentLevelIdentifier).m_tiers)
		{
			if (tier.m_part == type)
			{
				return num >= tier.m_starLimit;
			}
		}
		return true;
	}

	public void UpdateNewTagState()
	{
		newTag.SetActive(NewPartsInScope());
	}

	private bool NewPartsInScope()
	{
		if (scope == null)
		{
			return CustomizationManager.HasNewParts();
		}
		for (int i = 0; i < scope.Count; i++)
		{
			List<BasePart> customParts = CustomizationManager.GetCustomParts(scope[i]);
			for (int j = 0; j < customParts.Count; j++)
			{
				if (CustomizationManager.IsPartNew(customParts[j]))
				{
					return true;
				}
			}
		}
		return false;
	}
}
