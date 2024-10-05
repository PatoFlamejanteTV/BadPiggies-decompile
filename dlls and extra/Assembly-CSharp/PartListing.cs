using System;
using System.Collections.Generic;
using UnityEngine;

public class PartListing : Widget
{
	private class PartData
	{
		private static PartListing partList;

		public Dictionary<BasePart.PartTier, List<BasePart>> parts;

		public Dictionary<BasePart.PartTier, List<GameObject>> partInstances;

		private Transform selectedIcon;

		private BasePart.PartType type;

		public BasePart.PartType PartType => type;

		public Transform SelectedIcon => selectedIcon;

		public PartData(BasePart part, PartListing partListInstance)
		{
			partList = partListInstance;
			parts = new Dictionary<BasePart.PartTier, List<BasePart>>();
			parts.Add(part.m_partTier, new List<BasePart>());
			parts[part.m_partTier].Add(part);
			type = part.m_partType;
			CustomPartInfo customPart = WPFMonoBehaviour.gameData.GetCustomPart(part.m_partType);
			partInstances = new Dictionary<BasePart.PartTier, List<GameObject>>();
			partInstances.Add(part.m_partTier, new List<GameObject>());
			selectedIcon = null;
			if (customPart == null)
			{
				return;
			}
			for (int i = 0; i < customPart.PartList.Count; i++)
			{
				if (customPart.PartList[i].VisibleOnPartListBeforeUnlocking)
				{
					if (!parts.ContainsKey(customPart.PartList[i].m_partTier))
					{
						parts.Add(customPart.PartList[i].m_partTier, new List<BasePart>());
					}
					parts[customPart.PartList[i].m_partTier].Add(customPart.PartList[i]);
					if (!partInstances.ContainsKey(customPart.PartList[i].m_partTier))
					{
						partInstances.Add(customPart.PartList[i].m_partTier, new List<GameObject>());
					}
				}
			}
		}

		public int PartCount(BasePart.PartTier tier)
		{
			if (parts.ContainsKey(tier))
			{
				return parts[tier].Count;
			}
			return 0;
		}

		public int RowWidth()
		{
			int num = 0;
			foreach (KeyValuePair<BasePart.PartTier, List<BasePart>> part in parts)
			{
				if (part.Value.Count > num)
				{
					num = part.Value.Count;
				}
			}
			if (parts.ContainsKey(BasePart.PartTier.Epic) && parts.ContainsKey(BasePart.PartTier.Legendary))
			{
				int num2 = parts[BasePart.PartTier.Epic].Count + parts[BasePart.PartTier.Legendary].Count;
				num = ((num2 <= num) ? num : num2);
			}
			return num;
		}

		public Sprite GetIcon(BasePart.PartTier tier, int index)
		{
			return ((index >= PartCount(tier)) ? null : parts[tier][index]).m_constructionIconSprite;
		}

		public void AddPartRoot(BasePart.PartTier partTier, GameObject partRoot)
		{
			if (partInstances.ContainsKey(partTier) && parts.ContainsKey(partTier))
			{
				partRoot.name = parts[partTier][partInstances[partTier].Count].name;
				partInstances[partTier].Add(partRoot);
			}
		}

		public void AddSelectedIcon(Transform icon)
		{
			selectedIcon = icon;
			int lastUsedPartIndex = CustomizationManager.GetLastUsedPartIndex(type);
			UpdateSelectionIcon(WPFMonoBehaviour.gameData.GetCustomPart(type, lastUsedPartIndex).name);
		}

		public void UpdateSelectionIcon(string partName)
		{
			if (partInstances == null)
			{
				return;
			}
			foreach (KeyValuePair<BasePart.PartTier, List<GameObject>> partInstance in partInstances)
			{
				int num = 0;
				foreach (GameObject item in partInstance.Value)
				{
					if (partName == item.name)
					{
						partList.ToDark(item, dark: false);
						partList.ClearNewTags(item);
						if (partList.scrollbarNewTags.ContainsKey(item))
						{
							UnityEngine.Object.Destroy(partList.scrollbarNewTags[item]);
							partList.scrollbarNewTags.Remove(item);
						}
					}
					else if (item != null)
					{
						partList.ToDark(item, dark: true);
					}
					num++;
				}
			}
		}

		public void ShowXPParticles(string partName)
		{
			if (partInstances == null)
			{
				return;
			}
			foreach (KeyValuePair<BasePart.PartTier, List<GameObject>> partInstance in partInstances)
			{
				for (int i = 0; i < partInstance.Value.Count; i++)
				{
					if (partName == partInstance.Value[i].name)
					{
						GameObject gameObject;
						switch (partInstance.Key)
						{
						case BasePart.PartTier.Common:
							gameObject = WPFMonoBehaviour.gameData.m_xpParticlesSmall;
							break;
						case BasePart.PartTier.Rare:
							gameObject = WPFMonoBehaviour.gameData.m_xpParticlesMedium;
							break;
						case BasePart.PartTier.Epic:
						case BasePart.PartTier.Legendary:
							gameObject = WPFMonoBehaviour.gameData.m_xpParticlesLarge;
							break;
						default:
							gameObject = null;
							break;
						}
						if (!(gameObject == null))
						{
							GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, partInstance.Value[i].transform.position + Vector3.back, Quaternion.identity);
							LayerHelper.SetLayer(gameObject2, partInstance.Value[i].layer, children: true);
							LayerHelper.SetOrderInLayer(gameObject2, 1, children: true);
							LayerHelper.SetSortingLayer(gameObject2, partInstance.Value[i].GetComponent<Renderer>().sortingLayerName, children: true);
							gameObject2.GetComponent<Renderer>().sortingLayerID = partInstance.Value[i].GetComponent<Renderer>().sortingLayerID;
							ParticleSystem component = gameObject2.GetComponent<ParticleSystem>();
							UnityEngine.Object.Destroy(gameObject2, component.time + component.startLifetime);
						}
						return;
					}
				}
			}
		}

		public void PlaySelectionAudio(string partName)
		{
			if (parts == null)
			{
				return;
			}
			foreach (KeyValuePair<BasePart.PartTier, List<BasePart>> part in parts)
			{
				for (int i = 0; i < part.Value.Count; i++)
				{
					if (!(part.Value[i].name == partName))
					{
						continue;
					}
					for (int j = 0; j < part.Value[i].tags.Count; j++)
					{
						switch (part.Value[i].tags[j])
						{
						case "Alien_part":
							Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.alienBeepBoop, WPFMonoBehaviour.mainCamera.transform.position);
							break;
						case "Alien_pig":
						{
							AudioSource[] alienLanguage = WPFMonoBehaviour.gameData.commonAudioCollection.alienLanguage;
							AudioSource effectSource = alienLanguage[UnityEngine.Random.Range(0, alienLanguage.Length)];
							Singleton<AudioManager>.Instance.SpawnOneShotEffect(effectSource, WPFMonoBehaviour.mainCamera.transform.position);
							break;
						}
						case "Xmas_pig":
							Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.jingleBell, WPFMonoBehaviour.mainCamera.transform.position);
							break;
						}
					}
				}
			}
		}

		public bool ContainsNewParts()
		{
			foreach (KeyValuePair<BasePart.PartTier, List<BasePart>> part in parts)
			{
				for (int i = 0; i < part.Value.Count; i++)
				{
					if (CustomizationManager.IsPartNew(part.Value[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void ClearPartRoots()
		{
			foreach (KeyValuePair<BasePart.PartTier, List<GameObject>> partInstance in partInstances)
			{
				partInstance.Value.Clear();
			}
		}
	}

	public Action<float> OnPartListingMoved;

	public Action OnPartListDragBegin;

	[SerializeField]
	private float verticalPadding;

	[SerializeField]
	private float horizontalPadding;

	[SerializeField]
	private float partPadding;

	[SerializeField]
	private float firstRowPadding;

	[SerializeField]
	private GameObject regularIcon;

	[SerializeField]
	private GameObject commonIcon;

	[SerializeField]
	private GameObject rareIcon;

	[SerializeField]
	private GameObject epicIcon;

	[SerializeField]
	private GameObject legendaryIcon;

	[SerializeField]
	private GameObject emptyIcon;

	[SerializeField]
	private GameObject selectedIcon;

	[SerializeField]
	private PartListingScrollbar scrollbar;

	[SerializeField]
	private int momentumSlide;

	[SerializeField]
	private float limitMargin;

	[SerializeField]
	private float sideMargin;

	[SerializeField]
	private float iconScale;

	[SerializeField]
	private Color darken;

	[SerializeField]
	private Color darkenGray;

	[SerializeField]
	private Material grayMaterial;

	[SerializeField]
	private GameObject newContentTag;

	[SerializeField]
	private GameObject kingsFavoriteTag;

	[SerializeField]
	private GameObject toggleButton;

	[SerializeField]
	private string sortingLayer;

	private static PartListing instance;

	private Dictionary<string, Material> darkMaterials;

	private Dictionary<string, Material> normalMaterials;

	private Vector2 lastInputPos;

	private float deltaX;

	private float totalWidth;

	private float targetPosition;

	private float lastMovement;

	private bool interacting;

	private bool targeting;

	private Transform scrollPivot;

	private List<float> columns;

	private float xVelocity;

	private bool isInit;

	private bool showAll = true;

	private Action onClose;

	private Dictionary<BasePart.PartType, PartData> parts;

	private Dictionary<GameObject, GameObject> scrollbarNewTags;

	private List<GameObject> newButtons;

	private List<BasePart.PartType> partOrder;

	private List<BasePart.PartType> customPartScope;

	private List<GameObject> emptyParts;

	private BasePart.PartType centerPart;

	private GameData gameData;

	private string[] cachedPartTypeNames;

	private string[] cachedPartTierNames;

	private int PartTypeCount
	{
		get
		{
			if (cachedPartTypeNames == null)
			{
				cachedPartTypeNames = Enum.GetNames(typeof(BasePart.PartType));
			}
			return cachedPartTypeNames.Length;
		}
	}

	private int PartTierCount
	{
		get
		{
			if (cachedPartTierNames == null)
			{
				cachedPartTierNames = Enum.GetNames(typeof(BasePart.PartTier));
			}
			return cachedPartTierNames.Length;
		}
	}

	private float MoveLimit => WPFMonoBehaviour.hudCamera.orthographicSize * (float)Screen.width / (float)Screen.height - sideMargin;

	private float RightLimit => 0f - MoveLimit;

	private float LeftLimit
	{
		get
		{
			if (RightLimit < 0f - (totalWidth - MoveLimit))
			{
				return RightLimit;
			}
			return 0f - (totalWidth - MoveLimit);
		}
	}

	public float LastMovement => lastMovement;

	private void Awake()
	{
		Init();
	}

	public void Init()
	{
		if (!isInit)
		{
			newButtons = new List<GameObject>();
			scrollbarNewTags = new Dictionary<GameObject, GameObject>();
			gameData = WPFMonoBehaviour.gameData;
			toggleButton.SetActive(value: false);
			emptyParts = new List<GameObject>();
			darkMaterials = new Dictionary<string, Material>();
			normalMaterials = new Dictionary<string, Material>();
			base.transform.position = WPFMonoBehaviour.hudCamera.transform.position + new Vector3(0f, 0f, 6f);
			columns = new List<float>();
			instance = this;
			FillPartData();
			ReadPartOrder();
			isInit = true;
		}
	}

	private void OnEnable()
	{
		if (scrollPivot == null)
		{
			CreateGUI();
		}
		BackgroundMask.Show(show: true, this, sortingLayer, base.transform, new Vector3(0f, 0f, 0.1f));
		Singleton<KeyListener>.Instance.GrabFocus(this);
		KeyListener.keyReleased += HandleKeyReleased;
	}

	private void OnDisable()
	{
		if (BackgroundMask.Instantiated)
		{
			BackgroundMask.Show(show: false, this, string.Empty);
		}
		if (Singleton<KeyListener>.IsInstantiated())
		{
			Singleton<KeyListener>.Instance.ReleaseFocus(this);
			KeyListener.keyReleased -= HandleKeyReleased;
		}
	}

	private void OnDestroy()
	{
		if (!AtlasMaterials.IsInstantiated || darkMaterials == null)
		{
			return;
		}
		foreach (KeyValuePair<string, Material> darkMaterial in darkMaterials)
		{
			if (darkMaterial.Value != null)
			{
				AtlasMaterials.Instance.RemoveMaterialInstance(darkMaterial.Value);
			}
		}
	}

	public static PartListing Create()
	{
		if (instance != null)
		{
			return instance;
		}
		instance = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_partListing).GetComponent<PartListing>();
		return instance;
	}

	private void HandleKeyReleased(KeyCode key)
	{
		if (key == KeyCode.Escape)
		{
			Close();
		}
	}

	public void CreateSelectionIcons()
	{
		foreach (KeyValuePair<BasePart.PartType, PartData> part in parts)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(selectedIcon);
			gameObject.transform.parent = scrollPivot;
			gameObject.name = part.Key.ToString();
			SetSortingLayer(gameObject, sortingLayer);
		}
	}

	public void UpdateSelectionIcon(BasePart.PartType partType, string partName)
	{
		if (parts.ContainsKey(partType))
		{
			parts[partType].UpdateSelectionIcon(partName);
		}
	}

	public void ShowExperienceParticles(BasePart.PartType partType, string partName)
	{
		if (parts.ContainsKey(partType))
		{
			parts[partType].ShowXPParticles(partName);
		}
	}

	public void PlaySelectionAudio(BasePart.PartType partType, string partName)
	{
		if (parts.ContainsKey(partType))
		{
			parts[partType].PlaySelectionAudio(partName);
		}
	}

	public void OpenList()
	{
		SetPartScope(null);
		Open(null);
	}

	public void SetPartScope(List<BasePart.PartType> partScope)
	{
		customPartScope = partScope;
		if (customPartScope == null)
		{
			toggleButton.SetActive(value: false);
			showAll = true;
		}
		else
		{
			showAll = false;
			toggleButton.SetActive(value: true);
			ToggleScope();
		}
	}

	public void ToggleScope()
	{
		if (!isInit)
		{
			Init();
		}
		ClearNewTags(toggleButton);
		showAll = !showAll;
		int num = 0;
		int num2 = 0;
		columns.Clear();
		DisableEmptyIcons();
		if (showAll)
		{
			for (int i = 0; i < partOrder.Count; i++)
			{
				BasePart.PartType partType = partOrder[i];
				if (ValidPart(partType) && parts.ContainsKey(partType))
				{
					float num3 = (float)num * horizontalPadding + (float)num2 * partPadding;
					totalWidth = num3 + (float)(parts[partType].RowWidth() - 1) * horizontalPadding;
					RepositionIcons(parts[partType], num3);
					EnablePartIcons(parts[partType], enable: true);
					num2++;
					num += parts[partType].RowWidth();
				}
			}
		}
		else if (!showAll && customPartScope != null)
		{
			for (int j = 0; j < partOrder.Count; j++)
			{
				BasePart.PartType partType2 = partOrder[j];
				if (!ValidPart(partType2) || !parts.ContainsKey(partType2))
				{
					continue;
				}
				if (customPartScope.Contains(partType2))
				{
					if (INSettings.GetBool(INFeature.ColoredFrame) && partType2 == BasePart.PartType.MetalFrame)
					{
						float xPos = (float)num * horizontalPadding + (float)num2 * partPadding;
						totalWidth = (float)num * horizontalPadding + (float)num2 * partPadding + 3f * horizontalPadding;
						RepositionFrameIcons(parts[partType2], xPos);
						num2++;
						num += 4;
					}
					else
					{
						float num4 = (float)num * horizontalPadding + (float)num2 * partPadding;
						totalWidth = num4 + (float)(parts[partType2].RowWidth() - 1) * horizontalPadding;
						RepositionIcons(parts[partType2], num4);
						num2++;
						num += parts[partType2].RowWidth();
						EnablePartIcons(parts[partType2], enable: true);
					}
				}
				else
				{
					if (parts[partType2].ContainsNewParts())
					{
						AddNewContentTag(toggleButton);
					}
					EnablePartIcons(parts[partType2], enable: false);
				}
			}
		}
		toggleButton.transform.Find("Scoped").gameObject.SetActive(!showAll);
		toggleButton.transform.Find("Extended").gameObject.SetActive(showAll);
		targetPosition = GetTargetPosition();
		scrollbarNewTags.Clear();
		scrollbar.ClearNewPartButtons();
		InitNewButtons();
	}

	public void CenterOnPart(BasePart.PartType centerPart)
	{
		if (centerPart == BasePart.PartType.Unknown || (customPartScope != null && customPartScope.Contains(centerPart)))
		{
			this.centerPart = centerPart;
		}
	}

	public void Open(Action onClose)
	{
		this.onClose = onClose;
		Init();
		base.gameObject.SetActive(value: true);
		if (parts != null && parts.ContainsKey(centerPart))
		{
			MoveToPart(parts[centerPart].partInstances[BasePart.PartTier.Regular][0].transform);
		}
		scrollbarNewTags.Clear();
		scrollbar.ClearNewPartButtons();
		InitNewButtons();
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
		if (onClose != null)
		{
			onClose();
		}
		onClose = null;
	}

	public List<GameObject> GetPartTierInstances(BasePart.PartType partType, BasePart.PartTier partTier)
	{
		if (parts != null && parts.ContainsKey(partType) && parts[partType].partInstances != null && parts[partType].partInstances.ContainsKey(partTier))
		{
			return parts[partType].partInstances[partTier];
		}
		return null;
	}

	public void SetRelativePosition(float x)
	{
		deltaX = 0f;
		targetPosition = GetTargetPosition((0f - Mathf.Abs(LeftLimit - RightLimit)) * x + RightLimit);
	}

	private void FillPartData()
	{
		parts = new Dictionary<BasePart.PartType, PartData>();
		for (int i = 0; i < PartTypeCount; i++)
		{
			BasePart.PartType partType = (BasePart.PartType)i;
			if (ValidPart(partType))
			{
				GameObject part = gameData.GetPart(partType);
				if (part != null)
				{
					parts.Add(partType, new PartData(part.GetComponent<BasePart>(), this));
				}
			}
		}
	}

	private void ReadPartOrder()
	{
		partOrder = new List<BasePart.PartType>();
		string[] array = gameData.m_partOrderList.text.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				try
				{
					partOrder.Add((BasePart.PartType)Enum.Parse(typeof(BasePart.PartType), array[i]));
				}
				catch
				{
				}
			}
		}
	}

	private bool ValidPart(BasePart.PartType type)
	{
		switch (type)
		{
		case BasePart.PartType.Pumpkin:
			if (!GameProgress.HasKey("SecretPumpkin"))
			{
				return INSettings.GetBool(INFeature.UnlockCustomParts);
			}
			return true;
		default:
			return type != BasePart.PartType.TimeBomb;
		case BasePart.PartType.Unknown:
		case BasePart.PartType.JetEngine:
		case BasePart.PartType.ObsoleteWheel:
		case BasePart.PartType.MAX:
			return false;
		}
	}

	private void ClearGUI()
	{
		if (scrollPivot != null)
		{
			UnityEngine.Object.Destroy(scrollPivot.gameObject);
		}
		columns.Clear();
		for (int i = 0; i < PartTypeCount; i++)
		{
			if (parts.ContainsKey((BasePart.PartType)i))
			{
				parts[(BasePart.PartType)i].ClearPartRoots();
			}
		}
	}

	private void CreateGUI()
	{
		ClearNewbuttons();
		int num = 0;
		int num2 = 0;
		GameObject gameObject = new GameObject("Pivot");
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		scrollPivot = gameObject.transform;
		for (int i = 0; i < partOrder.Count; i++)
		{
			BasePart.PartType partType = partOrder[i];
			if (ValidPart(partType))
			{
				float num3 = (float)num * horizontalPadding + (float)num2 * partPadding;
				PartData partData = parts[partType];
				totalWidth = num3 + (float)(partData.RowWidth() - 1) * horizontalPadding;
				CreatePartIcons(partData, num3, gameObject.transform, newButtons);
				num2++;
				num += partData.RowWidth();
			}
		}
		scrollPivot.localPosition = new Vector3(GetTargetPosition(), scrollPivot.localPosition.y, scrollPivot.localPosition.z);
		targetPosition = GetTargetPosition();
		InitNewButtons();
	}

	private void CreatePartIcons(PartData data, float xPos, Transform parent, List<GameObject> newButtons)
	{
		for (int i = 0; i < PartTierCount - 1; i++)
		{
			for (int j = 0; j < data.RowWidth(); j++)
			{
				int index = j;
				Vector3 localPosition = new Vector3(xPos + (float)index * horizontalPadding, (float)(PartTierCount / 2) - (float)i * verticalPadding + verticalPadding);
				localPosition.y += ((i != 0) ? 0f : firstRowPadding);
				BasePart.PartTier tier = (BasePart.PartTier)i;
				bool flag;
				if (tier == BasePart.PartTier.Epic)
				{
					flag = index < data.PartCount(tier) + data.PartCount(BasePart.PartTier.Legendary);
					if (index >= data.PartCount(tier))
					{
						index -= data.PartCount(tier);
						tier = BasePart.PartTier.Legendary;
					}
				}
				else
				{
					flag = index < data.PartCount(tier);
				}
				if (!columns.Contains(localPosition.x))
				{
					columns.Add(localPosition.x);
				}
				if (!flag)
				{
					continue;
				}
				GameObject bg = UnityEngine.Object.Instantiate(GetIconBackground(tier));
				bg.transform.parent = parent;
				bg.transform.localPosition = localPosition;
				Sprite icon2 = data.GetIcon(tier, index);
				GameObject icon = null;
				if (icon2 != null)
				{
					icon = UnityEngine.Object.Instantiate(icon2.gameObject);
					icon.transform.parent = bg.transform;
					icon.transform.localPosition = new Vector3(0f, 0f, -0.1f);
					icon.transform.localScale = Vector3.one * iconScale;
				}
				if (tier != 0)
				{
					ToGray(bg, !CustomizationManager.IsPartUnlocked(data.parts[tier][index]));
				}
				if (IsKingsFavorite(data.parts[tier][index]))
				{
					AddKingsFavoriteTag(bg, out var _);
				}
				if (CustomizationManager.IsPartNew(data.parts[tier][index]) && AddNewContentTag(bg, out var item))
				{
					newButtons.Add(item);
				}
				data.AddPartRoot(tier, bg);
				Button button = bg.GetComponentInChildren<Button>();
				GameObjectEvents gameObjectEvents = bg.AddComponent<GameObjectEvents>();
				gameObjectEvents.OnEnabled = (Action<bool>)Delegate.Combine(gameObjectEvents.OnEnabled, (Action<bool>)delegate(bool enabled)
				{
					if (enabled)
					{
						if (IsKingsFavorite(data.parts[tier][index]))
						{
							AddKingsFavoriteTag(bg, out var _);
						}
						else
						{
							ClearKingsFavoriteTag(bg);
						}
						if (tier != 0)
						{
							bool flag2 = CustomizationManager.IsPartUnlocked(data.parts[tier][index]);
							ToGray(bg, !flag2);
							bg.GetComponent<Collider>().enabled = flag2;
							button.enabled = flag2;
							if (tier == BasePart.PartTier.Legendary)
							{
								if ((bool)icon)
								{
									icon.SetActive(flag2);
								}
								bg.transform.Find("QuestionMark").gameObject.SetActive(!flag2);
							}
							int lastUsedPartIndex = CustomizationManager.GetLastUsedPartIndex(data.PartType);
							data.UpdateSelectionIcon(gameData.GetCustomPart(data.PartType, lastUsedPartIndex).name);
							if (CustomizationManager.IsPartNew(data.parts[tier][index]))
							{
								if (AddNewContentTag(bg, out var item2))
								{
									newButtons.Add(item2);
								}
							}
							else
							{
								ClearNewTags(bg);
							}
						}
					}
				});
				gameObjectEvents.OnVisible = (Action<bool>)Delegate.Combine(gameObjectEvents.OnVisible, (Action<bool>)delegate(bool visible)
				{
					button.enabled = visible;
				});
				SetSortingLayer(bg, sortingLayer);
			}
		}
	}

	private void EnablePartIcons(PartData data, bool enable)
	{
		if (data.SelectedIcon != null)
		{
			data.SelectedIcon.gameObject.SetActive(enable);
		}
		for (int i = 0; i < PartTierCount; i++)
		{
			BasePart.PartTier key = (BasePart.PartTier)i;
			if (data.partInstances != null && data.partInstances.ContainsKey(key))
			{
				for (int j = 0; j < data.partInstances[key].Count; j++)
				{
					data.partInstances[key][j].SetActive(enable);
				}
			}
		}
	}

	private void RepositionIcons(PartData data, float xPos)
	{
		for (int i = 0; i < PartTierCount - 1; i++)
		{
			for (int j = 0; j < data.RowWidth(); j++)
			{
				int index = j;
				BasePart.PartTier tier = (BasePart.PartTier)i;
				Vector3 localPosition = new Vector3(xPos + (float)index * horizontalPadding, (float)(PartTierCount / 2) - (float)i * verticalPadding + verticalPadding);
				localPosition.y += ((i != 0) ? 0f : firstRowPadding);
				if (!columns.Contains(localPosition.x))
				{
					columns.Add(localPosition.x);
				}
				if (tier == BasePart.PartTier.Epic && index >= data.PartCount(tier))
				{
					index -= data.PartCount(tier);
					tier = BasePart.PartTier.Legendary;
				}
				if (!data.partInstances.ContainsKey(tier) || index >= data.partInstances[tier].Count)
				{
					continue;
				}
				data.partInstances[tier][index].transform.localPosition = localPosition;
				if (!CustomizationManager.IsPartNew(data.parts[tier][index]))
				{
					continue;
				}
				PartData data_ = data;
				if (!AddNewContentTag(data.partInstances[tier][index], out var gameObject))
				{
					continue;
				}
				GameObjectEvents gameObjectEvents = gameObject.AddComponent<GameObjectEvents>();
				gameObjectEvents.OnVisible = (Action<bool>)Delegate.Combine(gameObjectEvents.OnVisible, (Action<bool>)delegate(bool visible)
				{
					if (visible)
					{
						CustomizationManager.SetPartNew(data_.parts[tier][index], isNew: false);
					}
				});
				newButtons.Add(gameObject);
			}
		}
		int lastUsedPartIndex = CustomizationManager.GetLastUsedPartIndex(data.PartType);
		data.UpdateSelectionIcon(gameData.GetCustomPart(data.PartType, lastUsedPartIndex).name);
	}

	private void RepositionFrameIcons(PartData data, float xPos)
	{
		for (int i = 0; i < PartTierCount - 1; i++)
		{
			int num = 0;
			for (int j = 0; j < data.RowWidth(); j++)
			{
				int index = j;
				BasePart.PartTier tier = (BasePart.PartTier)i;
				Vector3 localPosition = new Vector3(xPos + (float)num * horizontalPadding, (float)(PartTierCount / 2) - (float)i * verticalPadding + verticalPadding);
				localPosition.y += ((i != 0) ? 0f : firstRowPadding);
				if (!columns.Contains(localPosition.x))
				{
					columns.Add(localPosition.x);
				}
				if (tier == BasePart.PartTier.Epic && index >= data.PartCount(tier))
				{
					index -= data.PartCount(tier);
					tier = BasePart.PartTier.Legendary;
				}
				if (!data.partInstances.ContainsKey(tier) || index >= data.partInstances[tier].Count)
				{
					continue;
				}
				string[] array = data.partInstances[tier][index].name.Split('_');
				if (array.Length <= 2 || !int.TryParse(array[2], out var _) || data.parts[tier][index].IsColoredrame())
				{
					continue;
				}
				num++;
				data.partInstances[tier][index].transform.localPosition = localPosition;
				if (!CustomizationManager.IsPartNew(data.parts[tier][index]))
				{
					continue;
				}
				PartData data_ = data;
				if (!AddNewContentTag(data.partInstances[tier][index], out var gameObject))
				{
					continue;
				}
				GameObjectEvents gameObjectEvents = gameObject.AddComponent<GameObjectEvents>();
				gameObjectEvents.OnVisible = (Action<bool>)Delegate.Combine(gameObjectEvents.OnVisible, (Action<bool>)delegate(bool visible)
				{
					if (visible)
					{
						CustomizationManager.SetPartNew(data_.parts[tier][index], isNew: false);
					}
				});
				newButtons.Add(gameObject);
			}
		}
		int lastUsedPartIndex = CustomizationManager.GetLastUsedPartIndex(data.PartType);
		data.UpdateSelectionIcon(gameData.GetCustomPart(data.PartType, lastUsedPartIndex).name);
		if (data.SelectedIcon != null)
		{
			data.SelectedIcon.gameObject.SetActive(value: true);
		}
		for (int k = 0; k < PartTierCount; k++)
		{
			BasePart.PartTier key = (BasePart.PartTier)k;
			if (data.partInstances != null && data.partInstances.ContainsKey(key))
			{
				for (int l = 0; l < data.partInstances[key].Count; l++)
				{
					string[] array2 = data.partInstances[key][l].name.Split('_');
					data.partInstances[key][l].SetActive(array2.Length > 2 && int.TryParse(array2[2], out var _) && !data.parts[key][l].IsColoredrame());
				}
			}
		}
	}

	private void ClearNewbuttons()
	{
		while (newButtons.Count > 0)
		{
			UnityEngine.Object.Destroy(newButtons[0]);
			newButtons.RemoveAt(0);
		}
		scrollbarNewTags.Clear();
		scrollbar.ClearNewPartButtons();
	}

	private void ClearNewTags(GameObject go)
	{
		Transform transform = go.transform.Find("NewContentTag");
		if (!(transform != null))
		{
			return;
		}
		for (int i = 0; i < transform.transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.name == "NewTag")
			{
				child.parent = null;
				UnityEngine.Object.Destroy(child.gameObject);
				i--;
			}
		}
	}

	private void ClearKingsFavoriteTag(GameObject go)
	{
		Transform transform = go.transform.Find("NewContentTag");
		if (!(transform != null))
		{
			return;
		}
		for (int i = 0; i < transform.transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.name == "KingsFavorite")
			{
				child.parent = null;
				UnityEngine.Object.Destroy(child.gameObject);
				i--;
			}
		}
	}

	private void InitNewButtons()
	{
		for (int i = 0; i < newButtons.Count; i++)
		{
			if (newButtons[i] == null)
			{
				newButtons.RemoveAt(i--);
			}
			else if (newButtons[i].activeInHierarchy && !(newButtons[i].transform.parent == null) && !(newButtons[i].transform.parent.parent == null))
			{
				GameObject value = scrollbar.SetNewPartButton(CalculateRelativePosition(RightLimit - newButtons[i].transform.parent.parent.localPosition.x));
				scrollbarNewTags.Add(newButtons[i].transform.parent.parent.gameObject, value);
			}
		}
	}

	private void ToDark(GameObject go, bool dark)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			string text = renderer.gameObject.name;
			if (text != "NewTag" && text != "New" && text != "KingsFavorite")
			{
				renderer.sharedMaterial = ((!dark) ? GetNormalMaterial(renderer.sharedMaterial) : GetDarkMaterial(renderer.sharedMaterial));
			}
		}
	}

	private void ToGray(GameObject go, bool gray)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i].gameObject.name == "NewTag") && !(componentsInChildren[i].gameObject.name == "New") && !(componentsInChildren[i].gameObject.name == "KingsFavorite") && (!gray || !IsGrayMaterial(componentsInChildren[i].sharedMaterial)))
			{
				componentsInChildren[i].sharedMaterial = ((!gray) ? GetNormalMaterial(componentsInChildren[i].sharedMaterial) : GetGreyMaterial(componentsInChildren[i].sharedMaterial));
			}
		}
	}

	private void SetSortingLayer(GameObject go, string layer)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingLayerName = layer;
		}
	}

	private Material GetDarkMaterial(Material mat)
	{
		string key = mat.name;
		if (darkMaterials.TryGetValue(key, out var value))
		{
			return value;
		}
		Material material = new Material(mat);
		AtlasMaterials.Instance.AddMaterialInstance(material);
		material.color *= ((!IsGrayMaterial(mat)) ? darken : darkenGray);
		darkMaterials.Add(key, material);
		if (!normalMaterials.ContainsKey(key))
		{
			normalMaterials.Add(key, mat);
		}
		return material;
	}

	private Material GetNormalMaterial(Material mat)
	{
		string key = mat.name;
		if (normalMaterials.TryGetValue(key, out var value))
		{
			return value;
		}
		return mat;
	}

	private bool AddNewContentTag(GameObject parent)
	{
		Transform transform = parent.transform.Find("NewContentTag");
		if (transform != null && transform.childCount <= 0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(newContentTag);
			gameObject.transform.parent = transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.name = "NewTag";
			SetSortingLayer(gameObject, sortingLayer);
			return true;
		}
		return false;
	}

	private bool AddNewContentTag(GameObject parent, out GameObject tag)
	{
		Transform transform = parent.transform.Find("NewContentTag");
		if (transform != null && transform.childCount <= 0)
		{
			tag = UnityEngine.Object.Instantiate(newContentTag);
			tag.transform.parent = transform;
			tag.transform.localPosition = Vector3.zero;
			tag.name = "NewTag";
			SetSortingLayer(tag, sortingLayer);
			return true;
		}
		tag = null;
		return false;
	}

	private bool AddKingsFavoriteTag(GameObject parent, out GameObject tag)
	{
		Transform transform = parent.transform.Find("NewContentTag");
		if (transform != null && transform.childCount <= 0)
		{
			tag = UnityEngine.Object.Instantiate(kingsFavoriteTag);
			tag.transform.parent = transform;
			tag.transform.localPosition = Vector3.zero;
			tag.name = "KingsFavorite";
			SetSortingLayer(tag, sortingLayer);
			return true;
		}
		tag = null;
		return false;
	}

	private GameObject GetIconBackground(BasePart.PartTier tier)
	{
		return tier switch
		{
			BasePart.PartTier.Common => commonIcon, 
			BasePart.PartTier.Rare => rareIcon, 
			BasePart.PartTier.Epic => epicIcon, 
			BasePart.PartTier.Legendary => legendaryIcon, 
			_ => regularIcon, 
		};
	}

	private void DisableEmptyIcons()
	{
		for (int i = 0; i < emptyParts.Count; i++)
		{
			if (emptyParts[i] == null)
			{
				emptyParts.RemoveAt(i--);
			}
			else
			{
				emptyParts[i].SetActive(value: false);
			}
		}
	}

	private GameObject GetUnusedEmptyIcon()
	{
		for (int i = 0; i < emptyParts.Count; i++)
		{
			if (!(emptyParts[i] == null) && !emptyParts[i].activeSelf)
			{
				return emptyParts[i];
			}
		}
		return UnityEngine.Object.Instantiate(emptyIcon);
	}

	private bool IsKingsFavorite(BasePart part)
	{
		if ((WPFMonoBehaviour.levelManager == null || WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode) && GameProgress.GetBool("CakeRaceUnlockShown") && Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite != null)
		{
			return Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite.name == part.name;
		}
		return false;
	}

	private void Update()
	{
		GuiManager.Pointer pointer = GuiManager.GetPointer();
		if (pointer.dragging && !interacting && PointerIsTouching(pointer.position))
		{
			lastInputPos = pointer.position;
			interacting = true;
			if (OnPartListDragBegin != null)
			{
				OnPartListDragBegin();
			}
		}
		if (pointer.dragging && interacting)
		{
			Vector3 vector = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(lastInputPos);
			Vector3 vector2 = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(pointer.position);
			lastInputPos = pointer.position;
			deltaX = vector2.x - vector.x;
			lastMovement += Mathf.Abs(deltaX);
			if (Mathf.Abs(deltaX) > 0f)
			{
				Move(deltaX);
			}
		}
		if (pointer.up && interacting)
		{
			interacting = false;
			lastMovement = 0f;
		}
		if (!interacting)
		{
			if (Mathf.Abs(deltaX) > 0.1f)
			{
				Move(deltaX);
				deltaX -= deltaX / (float)momentumSlide;
			}
			else
			{
				float num = scrollPivot.localPosition.x - targetPosition;
				float num2 = Mathf.SmoothDamp(num, 0f, ref xVelocity, 0.2f);
				Move(num2 - num, checkTarget: false);
			}
		}
	}

	private bool PointerIsTouching(Vector3 pointerPos)
	{
		Vector3 vector = base.transform.InverseTransformPoint(WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(pointerPos));
		if (vector.y < 6.5f)
		{
			return vector.y > -6f;
		}
		return false;
	}

	private void Move(float delta, bool checkTarget = true)
	{
		if (checkTarget && scrollPivot.localPosition.x < LeftLimit - limitMargin && delta < 0f)
		{
			deltaX = 0f;
			targetPosition = GetTargetPosition();
		}
		else if (checkTarget && scrollPivot.localPosition.x > RightLimit + limitMargin && delta > 0f)
		{
			deltaX = 0f;
			targetPosition = GetTargetPosition();
		}
		else
		{
			scrollPivot.localPosition = new Vector3(scrollPivot.localPosition.x + delta, scrollPivot.localPosition.y, scrollPivot.localPosition.z);
			if (OnPartListingMoved != null)
			{
				OnPartListingMoved(CalculateRelativePosition(scrollPivot.localPosition.x));
			}
		}
		if (checkTarget)
		{
			targetPosition = GetTargetPosition();
		}
	}

	private void MoveToPart(Transform targetTf)
	{
		if (!(targetTf == null))
		{
			targetPosition = GetTargetPosition(0f - targetTf.localPosition.x);
			float num = scrollPivot.localPosition.x - targetPosition;
			deltaX = 0f;
			Move(0f - num, checkTarget: false);
		}
	}

	private float GetTargetPosition(float forPosition)
	{
		float num = float.MaxValue;
		for (int i = 0; i < columns.Count; i++)
		{
			if (num > Mathf.Abs(forPosition + columns[i] - RightLimit))
			{
				num = Mathf.Abs(forPosition + columns[i] - RightLimit);
				continue;
			}
			if (i == 1)
			{
				return Mathf.Clamp(0f - columns[i - 1] + RightLimit, LeftLimit, RightLimit);
			}
			return Mathf.Clamp(0f - columns[i - 1] + RightLimit - sideMargin + iconScale, LeftLimit, RightLimit);
		}
		return Mathf.Clamp(0f - num, LeftLimit, RightLimit);
	}

	private float GetTargetPosition()
	{
		return GetTargetPosition(scrollPivot.localPosition.x);
	}

	private Material GetGreyMaterial(Material material)
	{
		AtlasMaterials atlasMaterials = AtlasMaterials.Instance;
		for (int i = 0; i < atlasMaterials.NormalMaterials.Count; i++)
		{
			if (material.name.Equals(atlasMaterials.NormalMaterials[i].name))
			{
				if (!normalMaterials.ContainsKey(atlasMaterials.GrayMaterials[i].name))
				{
					normalMaterials.Add(atlasMaterials.GrayMaterials[i].name, material);
				}
				return atlasMaterials.GrayMaterials[i];
			}
		}
		for (int j = 0; j < atlasMaterials.NormalMaterials.Count; j++)
		{
			if (material.name.Equals(atlasMaterials.RenderQueueMaterials[j].name))
			{
				if (!normalMaterials.ContainsKey(atlasMaterials.GrayMaterials[j].name))
				{
					normalMaterials.Add(atlasMaterials.GrayMaterials[j].name, material);
				}
				return atlasMaterials.GrayMaterials[j];
			}
		}
		if (!normalMaterials.ContainsKey(grayMaterial.name))
		{
			normalMaterials.Add(grayMaterial.name, material);
		}
		return grayMaterial;
	}

	private bool IsGrayMaterial(Material material)
	{
		AtlasMaterials atlasMaterials = AtlasMaterials.Instance;
		for (int i = 0; i < atlasMaterials.GrayMaterials.Count; i++)
		{
			if (material.name.Equals(atlasMaterials.GrayMaterials[i].name))
			{
				return true;
			}
		}
		return material.name.Contains("Gray");
	}

	private float CalculateRelativePosition(float position)
	{
		float num = Mathf.Abs(position - RightLimit);
		float num2 = Mathf.Abs(LeftLimit - RightLimit);
		if (num2 == 0f)
		{
			return 0.5f;
		}
		if (position > RightLimit)
		{
			return Mathf.Clamp01(0f - num / num2);
		}
		return Mathf.Clamp01(num / num2);
	}

	public void OpenShop()
	{
		base.gameObject.SetActive(value: false);
		LevelManager levelManager = WPFMonoBehaviour.levelManager;
		if (levelManager != null)
		{
			levelManager.InGameGUI.Hide();
			Singleton<IapManager>.Instance.OpenShopPage(delegate
			{
				base.gameObject.SetActive(value: true);
				levelManager.InGameGUI.Show();
			}, "FieldOfDreams");
		}
		else
		{
			Singleton<IapManager>.Instance.OpenShopPage(delegate
			{
				base.gameObject.SetActive(value: true);
			}, "FieldOfDreams");
		}
	}
}
