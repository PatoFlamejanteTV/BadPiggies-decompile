using System;
using System.Collections.Generic;
using UnityEngine;

public class PartSelector : WPFMonoBehaviour, WidgetListener
{
	public class PartDescOrder : IComparer<ConstructionUI.PartDesc>
	{
		private GameData gameData;

		public PartDescOrder(GameData data)
		{
			gameData = data;
		}

		public int Compare(ConstructionUI.PartDesc obj1, ConstructionUI.PartDesc obj2)
		{
			int partStarLimit = GetPartStarLimit(obj1.part.m_partType);
			int partStarLimit2 = GetPartStarLimit(obj2.part.m_partType);
			if (partStarLimit < partStarLimit2)
			{
				return -1;
			}
			if (partStarLimit > partStarLimit2)
			{
				return 1;
			}
			if (obj1.sortKey < obj2.sortKey)
			{
				return -1;
			}
			if (obj1.sortKey > obj2.sortKey)
			{
				return 1;
			}
			return 0;
		}

		private int GetPartStarLimit(BasePart.PartType part)
		{
			string currentLevelIdentifier = Singleton<GameManager>.Instance.CurrentLevelIdentifier;
			RaceLevels.LevelUnlockablePartsData levelUnlockableData = gameData.m_raceLevels.GetLevelUnlockableData(currentLevelIdentifier);
			if (levelUnlockableData != null)
			{
				foreach (RaceLevels.UnlockableTier tier in levelUnlockableData.m_tiers)
				{
					if (tier.m_part == part)
					{
						return tier.m_starLimit;
					}
				}
				return -1;
			}
			return -1;
		}
	}

	public GameObject m_partButtonPrefab;

	public GameObject m_partUnavailableButtonPrefab;

	public GameObject m_partUnlockAnimatedLock;

	public GameData m_gameData;

	public Transform m_toolBox;

	public Transform m_partList;

	private ExtendedScrollList m_scrollList;

	private List<ConstructionUI.PartDesc> m_partDescs;

	private ConstructionUI m_constructionUI;

	private Dictionary<BasePart.PartType, int> m_partOrder = new Dictionary<BasePart.PartType, int>();

	private UnlockRoadHogsParts m_unlockPartTierDialog;

	public int UsedRows => m_scrollList.UsedRows;

	public void SetMaxRows(int rows)
	{
		m_scrollList.SetMaxRows(rows);
	}

	public void SetConstructionUI(ConstructionUI constructionUI)
	{
		m_constructionUI = constructionUI;
	}

	public GameObject FindPartButton(ConstructionUI.PartDesc partDesc)
	{
		return m_scrollList.FindButton(partDesc);
	}

	public void SetSelection(ConstructionUI.PartDesc targetObject)
	{
		if (WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.AutoBuilding)
		{
			m_scrollList.SetSelection(targetObject);
		}
	}

	public void Select(Widget widget, object targetObject)
	{
		if ((bool)m_constructionUI && WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.AutoBuilding && WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.SuperAutoBuilding)
		{
			m_constructionUI.SelectPart((ConstructionUI.PartDesc)targetObject);
		}
	}

	public void StartDrag(Widget widget, object targetObject)
	{
		if ((bool)m_constructionUI && WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.AutoBuilding && WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.SuperAutoBuilding)
		{
			m_constructionUI.StartDrag((ConstructionUI.PartDesc)targetObject);
		}
	}

	public void CancelDrag(Widget widget, object targetObject)
	{
		if ((bool)m_constructionUI)
		{
			m_constructionUI.CancelDrag((ConstructionUI.PartDesc)targetObject);
		}
	}

	public void Drop(Widget widget, Vector3 dropPosition, object targetObject)
	{
	}

	public void SetParts(List<ConstructionUI.PartDesc> partDescs)
	{
		m_partDescs = new List<ConstructionUI.PartDesc>(partDescs);
		foreach (ConstructionUI.PartDesc partDesc in m_partDescs)
		{
			partDesc.sortKey = m_partOrder[partDesc.part.m_partType];
		}
		m_partDescs.Sort(new PartDescOrder(WPFMonoBehaviour.gameData));
		CreatePartList(handleDragIcons: false);
	}

	public void ResetSelection()
	{
		m_scrollList.ResetSelection();
	}

	private void Awake()
	{
		m_scrollList = base.transform.Find("ScrollList").GetComponent<ExtendedScrollList>();
		m_scrollList.SetListener(this);
		if (Singleton<BuildCustomizationLoader>.Instance.IsHDVersion)
		{
			m_scrollList.SetMaxRows(2);
			m_scrollList.SetButtonScale(INSettings.GetFloat(INFeature.PartSelectorScale));
		}
		else
		{
			m_scrollList.SetButtonScale(1.5f);
			base.transform.Translate(new Vector3(0f, 0.5f, 0f));
		}
		ReadPartOrder();
		if (!WPFMonoBehaviour.levelManager)
		{
			CreateTestPartList();
		}
		EventManager.Connect<CustomizePartUI.PartCustomizationEvent>(OnPartCustomized);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<CustomizePartUI.PartCustomizationEvent>(OnPartCustomized);
	}

	private void OnPartCustomized(CustomizePartUI.PartCustomizationEvent data)
	{
		if (m_partDescs == null)
		{
			return;
		}
		foreach (ConstructionUI.PartDesc partDesc in m_partDescs)
		{
			if (partDesc.part.m_partType != data.customizedPart)
			{
				continue;
			}
			partDesc.part = WPFMonoBehaviour.gameData.GetCustomPart(partDesc.part.m_partType, data.customPartIndex);
			GameObject gameObject = partDesc.part.m_constructionIconSprite.gameObject;
			partDesc.part.customPartIndex = data.customPartIndex;
			if (partDesc.currentPartIcon != null && gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				gameObject2.transform.parent = partDesc.currentPartIcon.parent;
				gameObject2.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
				gameObject2.transform.localPosition = new Vector3(0f, 0f, -0.5f);
				ConstructionUI.SetSortingOrder(gameObject2, 1, string.Empty);
				UnityEngine.Object.Destroy(partDesc.currentPartIcon.gameObject);
				partDesc.currentPartIcon = gameObject2.transform;
				if (partDesc.currentPartIcon.parent != null && partDesc.currentPartIcon.parent.GetComponent<DraggableButton>() != null)
				{
					partDesc.currentPartIcon.parent.GetComponent<DraggableButton>().Icon = gameObject2;
				}
			}
		}
	}

	private void LateUpdate()
	{
		Vector3 position = m_toolBox.position;
		if (m_scrollList.leftButton.transform.position.y > position.y)
		{
			position.y = m_scrollList.leftButton.transform.position.y;
			m_toolBox.position = position;
		}
		position = m_partList.position;
		if (m_scrollList.rightButton.transform.position.y > position.y)
		{
			position.y = m_scrollList.rightButton.transform.position.y;
			m_partList.position = position;
		}
	}

	private void ReadPartOrder()
	{
		m_partOrder.Clear();
		string text = m_gameData.m_partOrderList.text;
		char[] separator = new char[1] { '\n' };
		string[] array = text.Split(separator);
		int num = 0;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string text2 = array2[i].Trim();
			if (!(text2 != string.Empty))
			{
				continue;
			}
			BasePart.PartType key = BasePart.PartType.Unknown;
			foreach (GameObject part in m_gameData.m_parts)
			{
				BasePart.PartType partType = part.GetComponent<BasePart>().m_partType;
				if (partType.ToString() == text2)
				{
					key = partType;
					break;
				}
			}
			m_partOrder[key] = num;
			num++;
		}
	}

	private void CreatePartList(bool handleDragIcons)
	{
		m_scrollList.Clear();
		foreach (ConstructionUI.PartDesc partDesc in m_partDescs)
		{
			BasePart part = partDesc.part;
			GameObject gameObject = part.m_constructionIconSprite.gameObject;
			bool flag = true;
			int num = 0;
			int num2 = 0;
			int num3 = GameProgress.GetAllStars() + GameProgress.GetRaceLevelUnlockedStars();
			bool flag2 = false;
			if ((bool)WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.m_raceLevel && WPFMonoBehaviour.levelManager.CurrentGameMode is BaseGameMode)
			{
				string currentLevelIdentifier = Singleton<GameManager>.Instance.CurrentLevelIdentifier;
				RaceLevels.LevelUnlockablePartsData levelUnlockableData = WPFMonoBehaviour.gameData.m_raceLevels.GetLevelUnlockableData(currentLevelIdentifier);
				if (levelUnlockableData != null)
				{
					foreach (RaceLevels.UnlockableTier tier in levelUnlockableData.m_tiers)
					{
						if (tier.m_part != part.m_partType)
						{
							continue;
						}
						if (num3 < tier.m_starLimit)
						{
							flag = false;
							num = Mathf.Max(tier.m_starLimit - num3, 0);
							num2 = tier.m_starLimit;
							if (m_unlockPartTierDialog == null)
							{
								m_unlockPartTierDialog = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_unlockPartTierDialog).GetComponent<UnlockRoadHogsParts>();
								m_unlockPartTierDialog.transform.position = WPFMonoBehaviour.hudCamera.transform.position + new Vector3(0f, 0f, 10f);
								m_unlockPartTierDialog.Close();
								m_unlockPartTierDialog.onOpen += delegate
								{
									m_constructionUI.DisableFunctionality = true;
								};
								m_unlockPartTierDialog.onClose += delegate
								{
									m_constructionUI.DisableFunctionality = false;
								};
							}
						}
						else if (!GameProgress.GetRaceLevelPartUnlocked(currentLevelIdentifier, part.m_partType.ToString()))
						{
							GameProgress.SetRaceLevelPartUnlocked(currentLevelIdentifier, part.m_partType.ToString(), unlocked: true);
							flag2 = true;
						}
						break;
					}
				}
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate((!flag) ? m_partUnavailableButtonPrefab : m_partButtonPrefab);
			gameObject2.transform.parent = m_scrollList.transform;
			ConstructionUI.SetSortingOrder(gameObject2, 1, string.Empty);
			if (flag)
			{
				gameObject2.GetComponent<DraggableButton>().DragObject = partDesc;
				if (handleDragIcons)
				{
					gameObject2.GetComponent<DraggableButton>().DragIconPrefab = gameObject;
				}
				gameObject2.GetComponent<DraggableButton>().DragIconScale = 1.75f;
				Transform obj = gameObject2.transform.Find("PartCount");
				obj.GetComponent<TextMesh>().text = partDesc.CurrentCount.ToString();
				obj.GetComponent<PartCounter>().m_partType = partDesc.part.m_partType;
				obj.GetComponent<Renderer>().sortingOrder = 1;
			}
			else
			{
				Transform obj2 = gameObject2.transform.Find("Starlimit");
				obj2.GetComponent<TextMesh>().text = num.ToString();
				obj2.GetComponent<Renderer>().sortingOrder = 1;
				int productPrice = Singleton<VirtualCatalogManager>.Instance.GetProductPrice("road_hogs_star_unlock");
				if (productPrice > 0 && !Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
				{
					int totalCost = (num2 - num3) * productPrice;
					AddUnlockStarTierPopup(gameObject2.GetComponent<UnavailablePartButton>(), m_unlockPartTierDialog, num2, num3, totalCost, () => GameProgress.SnoutCoinCount() >= totalCost);
				}
			}
			GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject);
			gameObject3.transform.parent = gameObject2.transform;
			gameObject3.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
			gameObject3.transform.localPosition = new Vector3(0f, 0f, -0.5f);
			ConstructionUI.SetSortingOrder(gameObject3, 1, string.Empty);
			partDesc.currentPartIcon = gameObject3.transform;
			if (flag)
			{
				gameObject2.GetComponent<DraggableButton>().Icon = gameObject3;
				if (flag2)
				{
					GameObject obj3 = UnityEngine.Object.Instantiate(m_partUnlockAnimatedLock);
					obj3.transform.parent = gameObject2.transform;
					obj3.transform.localPosition = new Vector3(0f, 0.6f, -1f);
				}
			}
			m_scrollList.AddButton(gameObject2.GetComponent<Widget>());
		}
		foreach (ConstructionUI.PartDesc partDesc2 in m_partDescs)
		{
			if (partDesc2 != null && !(partDesc2.part == null))
			{
				int lastUsedPartIndex = CustomizationManager.GetLastUsedPartIndex(partDesc2.part.m_partType);
				if (lastUsedPartIndex > 0)
				{
					OnPartCustomized(new CustomizePartUI.PartCustomizationEvent(partDesc2.part.m_partType, lastUsedPartIndex));
				}
			}
		}
	}

	private void CreateTestPartList()
	{
		m_partDescs = new List<ConstructionUI.PartDesc>();
		foreach (GameObject part in m_gameData.m_parts)
		{
			ConstructionUI.PartDesc partDesc = new ConstructionUI.PartDesc();
			partDesc.part = part.GetComponent<BasePart>();
			partDesc.sortKey = m_partOrder[part.GetComponent<BasePart>().m_partType];
			partDesc.maxCount = UnityEngine.Random.Range(0, 20);
			m_partDescs.Add(partDesc);
		}
		m_partDescs.Sort(new PartDescOrder(WPFMonoBehaviour.gameData));
		CreatePartList(handleDragIcons: true);
	}

	public void AddUnlockStarTierPopup(UnavailablePartButton button, UnlockRoadHogsParts dialog, int tier, int currentStars, int cost, Func<bool> requirements)
	{
		if (button == null || dialog == null)
		{
			return;
		}
		button.OnPress = (Action)Delegate.Combine(button.OnPress, (Action)delegate
		{
			dialog.Cost = $"[snout] {cost}";
			dialog.ShowConfirmEnabled = () => true;
			dialog.SetOnConfirm(delegate
			{
				if (requirements() && GameProgress.UseSnoutCoins(cost))
				{
					GameProgress.AddRaceLevelUnlockedStars(tier - currentStars);
					Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
					CreatePartList(handleDragIcons: false);
					dialog.Close();
				}
				else if (!requirements() && Singleton<IapManager>.IsInstantiated())
				{
					dialog.Close();
					Singleton<IapManager>.Instance.OpenShopPage(dialog.Open, "SnoutCoinShop");
				}
				else
				{
					dialog.Close();
				}
			});
			dialog.Open();
		});
	}
}
