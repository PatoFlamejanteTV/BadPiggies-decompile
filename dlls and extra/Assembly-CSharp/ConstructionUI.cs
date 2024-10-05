using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionUI : WPFMonoBehaviour
{
	public class PartDesc
	{
		public BasePart part;

		public Texture tex;

		public int coordX;

		public int coordY;

		public int useCount;

		public int maxCount;

		public int sortKey;

		public int customPartIndex;

		public Transform currentPartIcon;

		public int CurrentCount
		{
			get
			{
				if (INSettings.GetBool(INFeature.PartCounter))
				{
					return useCount;
				}
				return maxCount - useCount;
			}
		}
	}

	public Action OnPartsUnlocked;

	public List<Transform> m_purchasableParts = new List<Transform>();

	public Transform m_gridPrefab;

	public Transform m_cellPrefab;

	public Material m_cellMaterial;

	public int m_itemsPerRow = 14;

	public float m_offsetX = 0.2f;

	public float m_offsetY = 0.2f;

	public float m_spacingX = 0.06f;

	public float m_scale = 0.1f;

	public Texture2D m_cellTextureValid;

	public Texture2D m_cellTextureInvalid;

	public Texture2D m_textureSelected;

	protected Transform m_grid;

	protected Dictionary<int, Transform> m_cellMap = new Dictionary<int, Transform>();

	protected List<Transform> m_partInstances = new List<Transform>();

	protected List<PartDesc> m_partDescs = new List<PartDesc>();

	protected List<PartDesc> m_purchasablePartDescs = new List<PartDesc>();

	protected List<PartDesc> m_unlockedParts = new List<PartDesc>();

	protected Contraption m_contraption;

	protected int m_draggedElement = -1;

	protected int m_draggedElementCustomizationIndex;

	protected bool m_draggingFromContraption;

	private bool m_dragStartedFromContraption;

	protected int m_contraptionDragX;

	protected int m_contraptionDragY;

	private int m_dragStartedX;

	private int m_dragStartedY;

	protected int m_flipCount;

	protected int m_selectedElement = -1;

	protected BasePart.GridRotation draggedPartRotation;

	protected bool draggedPartFlipped;

	protected GUIStyle m_textStyle;

	private GameObject clearButton;

	private GameObject playButton;

	private GameObject moveButtons;

	private GameObject moveLeftButton;

	private GameObject moveRightButton;

	private GameObject moveUpButton;

	private GameObject moveDownButton;

	private List<Transform> m_parts = new List<Transform>();

	private PartSelector partSelector;

	private GameObject m_dragIcon;

	private bool m_useDragOffset = true;

	private Vector3 m_dragOffset;

	private Vector3 m_dragIconOffset = Vector3.zero;

	private Transform m_mouseOverCell;

	private bool m_dragStarted;

	private Vector3 m_dragStartPosition;

	private Vector3 m_rightDragStartPosition;

	private int m_moveCounter;

	private int m_rotationCounter;

	private float m_lastMoveTime;

	private bool m_tutorialPulseDone;

	private bool m_previewSwipeStarted;

	private Vector3 m_previewSwipeStartPosition;

	private bool m_allowDragPlacement;

	private bool m_disableFunctionality;

	private BasePart.PartType m_currentCustomizablePartType;

	private Vector3 m_pointerTime;

	public List<PartDesc> PartDescriptors => m_partDescs;

	public bool DisableFunctionality
	{
		get
		{
			return m_disableFunctionality;
		}
		set
		{
			m_disableFunctionality = value;
		}
	}

	public List<PartDesc> UnlockedParts => m_unlockedParts;

	public int RotationCount => m_rotationCounter;

	public int MoveCount => m_moveCounter;

	public int SelectedElement => m_selectedElement;

	public Vector3 PointerTime => m_pointerTime;

	private void AddMove()
	{
		m_lastMoveTime = Time.time;
		m_moveCounter++;
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
		EventManager.Disconnect<CustomizePartUI.PartCustomizationEvent>(OnPartCustomization);
	}

	public PartDesc FindPartDesc(BasePart.PartType partType)
	{
		foreach (PartDesc partDesc in m_partDescs)
		{
			if (partDesc.part.m_partType == partType)
			{
				return partDesc;
			}
		}
		return null;
	}

	public int PartSelectorRowCount()
	{
		return partSelector.UsedRows;
	}

	public void SetPartSelectorMaxRowCount(int rows)
	{
		partSelector.SetMaxRows(rows);
	}

	public GameObject FindPartButton(BasePart.PartType partType)
	{
		return partSelector.FindPartButton(FindPartDesc(partType));
	}

	public void AddUnlockedPart(BasePart.PartType partType, int count)
	{
		foreach (PartDesc partDesc in m_partDescs)
		{
			if (partDesc.part.m_partType == partType)
			{
				partDesc.maxCount += count;
				EventManager.Send(new PartCountChanged(partType, partDesc.CurrentCount));
			}
		}
	}

	private void OnEnable()
	{
		SetButtonPositions();
		m_lastMoveTime = Time.time;
		EventManager.Connect<GameTimePaused>(ReceiveGameTimePaused);
		if (INSettings.GetBool(INFeature.RenderInfiniteGrid))
		{
			m_grid.gameObject.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		EventManager.Disconnect<GameTimePaused>(ReceiveGameTimePaused);
		if (INSettings.GetBool(INFeature.RenderInfiniteGrid))
		{
			m_grid.gameObject.SetActive(value: false);
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused && m_draggedElement != -1)
		{
			partSelector.ResetSelection();
			if (m_draggedElement != -1)
			{
				SetDraggedElement(-1);
			}
		}
	}

	private void ReceiveGameTimePaused(GameTimePaused data)
	{
		if (data.paused && m_draggedElement != -1)
		{
			partSelector.ResetSelection();
			if (m_draggedElement != -1)
			{
				SetDraggedElement(-1);
			}
		}
	}

	private void Awake()
	{
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		EventManager.Connect<CustomizePartUI.PartCustomizationEvent>(OnPartCustomization);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		m_useDragOffset = DeviceInfo.UsesTouchInput && !Singleton<BuildCustomizationLoader>.Instance.IsHDVersion;
		m_allowDragPlacement = !DeviceInfo.UsesTouchInput;
		foreach (GameObject part in WPFMonoBehaviour.gameData.m_parts)
		{
			Transform transform = part.transform;
			m_parts.Add(transform);
			BasePart component = part.GetComponent<BasePart>();
			int num4 = WPFMonoBehaviour.levelManager.GetPartTypeCount(component.m_partType);
			if (INSettings.GetBool(INFeature.PartCounter) && component.m_constructionIconSprite != null)
			{
				num4 = 1061109567;
			}
			if (num4 == 0 && !m_purchasableParts.Contains(transform))
			{
				continue;
			}
			Transform obj = UnityEngine.Object.Instantiate(transform);
			component = obj.GetComponent<BasePart>();
			obj.gameObject.SetActive(value: false);
			obj.parent = base.transform;
			MeshRenderer component2 = obj.GetComponent<MeshRenderer>();
			if ((bool)component.m_constructionIconSprite)
			{
				component2 = component.m_constructionIconSprite.GetComponent<MeshRenderer>();
			}
			if (!component2 || !component2.sharedMaterial)
			{
				continue;
			}
			Texture mainTexture = component2.sharedMaterial.mainTexture;
			if (!mainTexture)
			{
				continue;
			}
			PartDesc partDesc = new PartDesc();
			partDesc.part = component;
			partDesc.tex = mainTexture;
			partDesc.coordX = num;
			partDesc.coordY = num2;
			partDesc.useCount = 0;
			partDesc.maxCount = num4;
			partDesc.customPartIndex = 0;
			if (WPFMonoBehaviour.levelManager.m_sandbox && !(WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode))
			{
				int unlockedSandboxPartCount = GameProgress.GetUnlockedSandboxPartCount(component.m_partType);
				if (unlockedSandboxPartCount > 0)
				{
					GameProgress.SetUnlockedSandboxPartCount(component.m_partType, 0);
					PartDesc partDesc2 = new PartDesc();
					partDesc2.part = partDesc.part;
					partDesc2.maxCount = unlockedSandboxPartCount;
					partDesc2.customPartIndex = 0;
					m_unlockedParts.Add(partDesc2);
					partDesc.maxCount -= unlockedSandboxPartCount;
				}
				unlockedSandboxPartCount = GameProgress.GetUnlockedSandboxPartCount(Singleton<GameManager>.Instance.CurrentSceneName, component.m_partType);
				if (unlockedSandboxPartCount > 0)
				{
					GameProgress.SetUnlockedSandboxPartCount(Singleton<GameManager>.Instance.CurrentSceneName, component.m_partType, 0);
					PartDesc partDesc3 = new PartDesc();
					partDesc3.part = partDesc.part;
					partDesc3.maxCount = unlockedSandboxPartCount;
					partDesc3.customPartIndex = 0;
					m_unlockedParts.Add(partDesc3);
					partDesc.maxCount -= unlockedSandboxPartCount;
				}
			}
			m_partDescs.Add(partDesc);
			WPFMonoBehaviour.levelManager.m_totalAvailableParts += num4;
			num++;
			if (num >= m_itemsPerRow)
			{
				num = 0;
				num2++;
			}
			num3++;
		}
		if ((bool)WPFMonoBehaviour.levelManager)
		{
			m_contraption = WPFMonoBehaviour.levelManager.ContraptionProto;
		}
		if (!m_contraption)
		{
			GameObject obj2 = new GameObject("Contraption");
			obj2.transform.parent = base.transform;
			obj2.transform.localPosition = Vector3.zero;
			m_contraption = obj2.AddComponent<Contraption>();
		}
		m_cellPrefab = ((!WPFMonoBehaviour.levelManager.GridCellPrefab) ? m_cellPrefab : WPFMonoBehaviour.levelManager.GridCellPrefab.transform);
		if ((bool)m_cellPrefab)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			for (int i = 0; i < WPFMonoBehaviour.levelManager.GridHeight; i++)
			{
				for (int j = WPFMonoBehaviour.levelManager.GridXMin; j <= WPFMonoBehaviour.levelManager.GridXMax; j++)
				{
					if (WPFMonoBehaviour.levelManager.CanPlacePartAtGridCell(j, i))
					{
						Transform transform2 = UnityEngine.Object.Instantiate(m_cellPrefab);
						transform2.transform.parent = gameObject.transform;
						transform2.localPosition = new Vector3(j, i, 1f);
						int key = i * 1000 + j;
						m_cellMap[key] = transform2;
					}
				}
			}
			m_grid = gameObject.transform;
			if (INSettings.GetBool(INFeature.RenderInfiniteGrid))
			{
				m_cellMaterial = new Material(m_cellPrefab.GetComponent<MeshRenderer>().sharedMaterial);
				m_cellMaterial.color = new Color(1f, 1f, 1f, 0.5f);
			}
		}
		GameObject gameObject2 = GameObject.Find("InGameGUI");
		if ((bool)gameObject2)
		{
			clearButton = gameObject2.transform.Find("InGameBuildMenu").Find("ClearButton").gameObject;
			playButton = gameObject2.transform.Find("InGameBuildMenu").Find("PlayButton").gameObject;
			moveButtons = gameObject2.transform.Find("InGameBuildMenu").Find("MoveButtons").gameObject;
			moveLeftButton = moveButtons.transform.Find("MoveLeftButton").gameObject;
			moveRightButton = moveButtons.transform.Find("MoveRightButton").gameObject;
			moveUpButton = moveButtons.transform.Find("MoveUpButton").gameObject;
			moveDownButton = moveButtons.transform.Find("MoveDownButton").gameObject;
			partSelector = gameObject2.transform.Find("InGameBuildMenu").Find("PartSelector").GetComponent<PartSelector>();
			partSelector.SetParts(m_partDescs);
		}
	}

	public void SetCurrentContraption()
	{
		m_contraption = WPFMonoBehaviour.levelManager.ContraptionProto;
	}

	private void Update()
	{
		if (m_disableFunctionality)
		{
			return;
		}
		if (m_draggedElement != -1)
		{
			m_lastMoveTime = Time.time;
			if ((bool)m_dragIcon)
			{
				Vector3 position = WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(GuiManager.GetPointer().position);
				position.z = WPFMonoBehaviour.hudCamera.transform.position.z + 2f;
				position += m_dragOffset;
				position += m_dragIconOffset;
				m_dragIcon.transform.position = position;
			}
		}
		SetButtonPositions();
		if (Input.touchCount > 1)
		{
			CancelDrag();
		}
		HandleDragging();
	}

	private void LateUpdate()
	{
		if (!INSettings.GetBool(INFeature.RenderInfiniteGrid))
		{
			return;
		}
		Vector3 vector = WPFMonoBehaviour.ingameCamera.transform.position - base.transform.position;
		float orthographicSize = WPFMonoBehaviour.ingameCamera.GetComponent<Camera>().orthographicSize;
		orthographicSize = ((orthographicSize > 20f) ? 20f : orthographicSize);
		float num = orthographicSize * (float)Screen.width / (float)Screen.height;
		bool flag = true;
		bool flag2 = true;
		int i = 0;
		int childCount = m_grid.childCount;
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Building)
		{
			for (int j = (int)(vector.x - num) - 1; j <= (int)(vector.x + num) + 1 && flag2; j++)
			{
				for (int k = (int)(vector.y - orthographicSize) - 1; k <= (int)(vector.y + orthographicSize) + 1 && flag2; k++)
				{
					Transform transform;
					if (i < childCount)
					{
						transform = m_grid.GetChild(i);
						MeshRenderer component = transform.GetComponent<MeshRenderer>();
						if (!component.enabled)
						{
							flag = false;
						}
						if (!flag)
						{
							component.enabled = true;
						}
					}
					else
					{
						transform = UnityEngine.Object.Instantiate(m_cellPrefab);
						transform.GetComponent<MeshRenderer>().material = m_cellMaterial;
						transform.parent = m_grid;
					}
					i++;
					transform.localPosition = new Vector3(j, k, 1f);
					flag2 = i < childCount + 200;
				}
			}
		}
		for (; i < childCount && flag; i++)
		{
			MeshRenderer component2 = m_grid.GetChild(i).GetComponent<MeshRenderer>();
			if (component2.enabled)
			{
				component2.enabled = false;
				continue;
			}
			break;
		}
	}

	private void SetButtonPositions()
	{
		if (INSettings.GetBool(INFeature.SetBuildingButtonPosition))
		{
			float num = (float)Screen.width / (float)Screen.height * 10f;
			float num2 = -1.987f;
			float num3 = 3.815f;
			clearButton.transform.localPosition = new Vector3(num + num2, 8f - num3 * 3f, 0f);
			playButton.transform.localPosition = new Vector3(num + num2, 8f - num3 * 2f, 0f);
			if (INSettings.GetBool(INFeature.SaveButton))
			{
				clearButton.transform.parent.Find("SaveButton").transform.localPosition = new Vector3(num + num2, 8f - num3, 0f);
			}
		}
		else
		{
			SetHudPositionFromRelativeLevelPosition(clearButton, new Vector3((float)WPFMonoBehaviour.levelManager.GridXMin - 1f, 0.5f * (float)(WPFMonoBehaviour.levelManager.GridHeight - 1), 0f), new Vector3(-1.2f, 0f, 0f));
			SetHudPositionFromRelativeLevelPosition(playButton, new Vector3((float)WPFMonoBehaviour.levelManager.GridXMax + 1f, 0.5f * (float)(WPFMonoBehaviour.levelManager.GridHeight - 1), 0f), new Vector3(1.2f, 0f, 0f));
		}
	}

	public void CheckUnlockedParts()
	{
		List<PartDesc> list = new List<PartDesc>();
		List<PartDesc> list2 = new List<PartDesc>();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (Transform part in m_parts)
		{
			Transform transform = UnityEngine.Object.Instantiate(part);
			BasePart component = transform.GetComponent<BasePart>();
			transform.gameObject.SetActive(value: false);
			int num5 = WPFMonoBehaviour.levelManager.GetPartTypeCount(component.m_partType);
			if (INSettings.GetBool(INFeature.PartCounter) && component.m_constructionIconSprite != null)
			{
				num5 = 1061109567;
			}
			if (num5 == 0 && !m_purchasableParts.Contains(part))
			{
				UnityEngine.Object.Destroy(transform.gameObject);
				continue;
			}
			transform.parent = base.transform;
			MeshRenderer component2 = transform.GetComponent<MeshRenderer>();
			if ((bool)transform.GetComponent<BasePart>().m_constructionIconSprite)
			{
				component2 = transform.GetComponent<BasePart>().m_constructionIconSprite.GetComponent<MeshRenderer>();
			}
			if (!component2 || !component2.sharedMaterial)
			{
				continue;
			}
			Texture mainTexture = component2.sharedMaterial.mainTexture;
			if (!mainTexture)
			{
				continue;
			}
			PartDesc partDesc = new PartDesc();
			partDesc.part = component;
			partDesc.tex = mainTexture;
			partDesc.coordX = num;
			partDesc.coordY = num2;
			partDesc.useCount = WPFMonoBehaviour.levelManager.ContraptionProto.GetPartCount(component.m_partType);
			partDesc.maxCount = num5;
			partDesc.customPartIndex = 0;
			if (WPFMonoBehaviour.levelManager.m_sandbox)
			{
				int unlockedSandboxPartCount = GameProgress.GetUnlockedSandboxPartCount(component.m_partType);
				if (unlockedSandboxPartCount > 0)
				{
					GameProgress.SetUnlockedSandboxPartCount(component.m_partType, 0);
					list.Add(new PartDesc
					{
						part = partDesc.part,
						maxCount = unlockedSandboxPartCount,
						customPartIndex = 0
					});
					partDesc.maxCount -= unlockedSandboxPartCount;
				}
				if (unlockedSandboxPartCount > 0)
				{
					num4 += unlockedSandboxPartCount;
					WPFMonoBehaviour.levelManager.m_totalAvailableParts += unlockedSandboxPartCount;
				}
				unlockedSandboxPartCount = GameProgress.GetUnlockedSandboxPartCount(Singleton<GameManager>.Instance.CurrentSceneName, component.m_partType);
				if (unlockedSandboxPartCount > 0)
				{
					GameProgress.SetUnlockedSandboxPartCount(Singleton<GameManager>.Instance.CurrentSceneName, component.m_partType, 0);
					list.Add(new PartDesc
					{
						part = partDesc.part,
						maxCount = unlockedSandboxPartCount,
						customPartIndex = 0
					});
					partDesc.maxCount -= unlockedSandboxPartCount;
				}
				if (unlockedSandboxPartCount > 0)
				{
					num4 += unlockedSandboxPartCount;
					WPFMonoBehaviour.levelManager.m_totalAvailableParts += unlockedSandboxPartCount;
				}
				num++;
				if (num >= m_itemsPerRow)
				{
					num = 0;
					num2++;
				}
				num3++;
			}
			list2.Add(partDesc);
		}
		m_unlockedParts = list;
		if (num4 > 0)
		{
			m_partDescs = list2;
			RefreshButtons();
		}
		if (OnPartsUnlocked != null)
		{
			OnPartsUnlocked();
		}
	}

	public void RefreshButtons()
	{
		partSelector.SetParts(m_partDescs);
	}

	private void HandlePreviewSwipe()
	{
		if (m_draggedElement == -1)
		{
			GuiManager.Pointer pointer = GuiManager.GetPointer();
			if (pointer.down)
			{
				m_previewSwipeStarted = true;
				m_previewSwipeStartPosition = pointer.position;
			}
			if (pointer.up)
			{
				if (m_previewSwipeStarted && Vector3.Distance(m_previewSwipeStartPosition, pointer.position) / (float)Screen.width > 0.4f)
				{
					EventManager.Send(new UIEvent(UIEvent.Type.Preview));
				}
				m_previewSwipeStarted = false;
			}
		}
		else
		{
			m_previewSwipeStarted = false;
		}
	}

	private void HandleTutorialButton()
	{
		if (m_tutorialPulseDone)
		{
			return;
		}
		int tutorialBookOpenCount = GameProgress.GetTutorialBookOpenCount();
		if (tutorialBookOpenCount < 3)
		{
			if (Time.time - m_lastMoveTime > 7.5f * (float)tutorialBookOpenCount)
			{
				EventManager.Send(new PulseButtonEvent(UIEvent.Type.OpenTutorial));
				m_tutorialPulseDone = true;
			}
		}
		else
		{
			m_tutorialPulseDone = true;
		}
	}

	public Vector3 RelativeLevelPositionToHudPosition(Vector3 levelOffset)
	{
		Vector3 position = base.transform.position + levelOffset;
		Vector3 position2 = Camera.main.WorldToScreenPoint(position);
		return WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(position2);
	}

	private void SetHudPositionFromRelativeLevelPosition(GameObject obj, Vector3 levelOffset, Vector3 hudOffset)
	{
		Vector3 position = obj.transform.position;
		float z = position.z;
		Vector3 position2 = base.transform.position + levelOffset;
		Vector3 position3 = Camera.main.WorldToScreenPoint(position2);
		Vector3 vector = WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(position3);
		vector += hudOffset;
		vector.z = z;
		if (Vector3.SqrMagnitude(position - vector) > 1E-06f)
		{
			obj.transform.position = vector;
		}
	}

	private void ReceiveUIEvent(UIEvent data)
	{
		switch (data.type)
		{
		case UIEvent.Type.MoveContraptionLeft:
			MoveContraption(-1, 0);
			break;
		case UIEvent.Type.MoveContraptionRight:
			MoveContraption(1, 0);
			break;
		case UIEvent.Type.MoveContraptionUp:
			MoveContraption(0, 1);
			break;
		case UIEvent.Type.MoveContraptionDown:
			MoveContraption(0, -1);
			break;
		}
		if (INSettings.GetBool(INFeature.SaveButton) && data.type == UIEvent.Type.SaveContraption)
		{
			GameMode currentGameMode = WPFMonoBehaviour.levelManager.CurrentGameMode;
			m_contraption.CreateAndSaveContraption(currentGameMode.GetCurrentContraptionName());
		}
	}

	public void ContraptionPartChanged(int x, int y)
	{
		if (m_contraption.Parts.Count < 2)
		{
			SetMoveButtonStates();
			return;
		}
		if (x == WPFMonoBehaviour.levelManager.GridXMin)
		{
			SetMoveButtonState(-1, 0);
		}
		if (x == WPFMonoBehaviour.levelManager.GridXMax)
		{
			SetMoveButtonState(1, 0);
		}
		if (y == 0)
		{
			SetMoveButtonState(0, -1);
		}
		if (y == WPFMonoBehaviour.levelManager.GridHeight - 1)
		{
			SetMoveButtonState(0, 1);
		}
	}

	public void SetMoveButtonStates()
	{
		SetMoveButtonState(1, 0);
		SetMoveButtonState(-1, 0);
		SetMoveButtonState(0, 1);
		SetMoveButtonState(0, -1);
	}

	private void SetMoveButtonState(int dx, int dy)
	{
		bool flag = m_contraption.CanMoveOnGrid(dx, dy);
		switch (dx)
		{
		case -1:
			moveLeftButton.GetComponent<Renderer>().enabled = flag;
			moveLeftButton.GetComponent<Collider>().enabled = flag;
			return;
		case 1:
			moveRightButton.GetComponent<Renderer>().enabled = flag;
			moveRightButton.GetComponent<Collider>().enabled = flag;
			return;
		}
		switch (dy)
		{
		case 1:
			moveUpButton.GetComponent<Renderer>().enabled = flag;
			moveUpButton.GetComponent<Collider>().enabled = flag;
			break;
		case -1:
			moveDownButton.GetComponent<Renderer>().enabled = flag;
			moveDownButton.GetComponent<Collider>().enabled = flag;
			break;
		}
	}

	private void MoveContraption(int dx, int dy)
	{
		AddMove();
		m_contraption.MoveOnGrid(dx, dy);
		if (dx != 0)
		{
			SetMoveButtonState(1, 0);
			SetMoveButtonState(-1, 0);
		}
		if (dy != 0)
		{
			SetMoveButtonState(0, 1);
			SetMoveButtonState(0, -1);
		}
	}

	public void SelectPart(PartDesc partDesc)
	{
		m_selectedElement = -1;
		for (int i = 0; i < m_partDescs.Count; i++)
		{
			if (m_partDescs[i].part == partDesc.part)
			{
				m_selectedElement = i;
				break;
			}
		}
	}

	public void StartDrag(PartDesc partDesc)
	{
		if (WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Building)
		{
			return;
		}
		int draggedElement = -1;
		for (int i = 0; i < m_partDescs.Count; i++)
		{
			if (m_partDescs[i] == partDesc)
			{
				if (m_partDescs[i].useCount < m_partDescs[i].maxCount)
				{
					m_partDescs[i].useCount++;
					EventManager.Send(new PartCountChanged(partDesc.part.m_partType, m_partDescs[i].CurrentCount));
					draggedElement = i;
				}
				break;
			}
		}
		SetDraggedElement(draggedElement);
	}

	public bool IsDragging()
	{
		return m_draggedElement != -1;
	}

	public void CancelDrag(PartDesc partDesc)
	{
		SetDraggedElement(-1);
	}

	public void CancelDrag()
	{
		if (m_draggedElement != -1)
		{
			if (m_draggingFromContraption)
			{
				PartDesc partDesc = m_partDescs[m_draggedElement];
				partDesc.useCount++;
				EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
				BasePart basePart = SetPartAt(m_contraptionDragX, m_contraptionDragY, partDesc.part);
				ContraptionPartChanged(m_contraptionDragX, m_contraptionDragY);
				basePart.OnPartPlaced();
				EventManager.Send(new PartPlacedEvent(partDesc.part.m_partType, partDesc.part.m_partTier, basePart.transform.position));
			}
			SetDraggedElement(-1);
		}
	}

	private void SetDraggedElement(int element)
	{
		SetDraggedElement(element, fromContraption: false, 0, 0);
	}

	private void SetDraggedElement(int element, bool fromContraption, int contraptionX, int contraptionY)
	{
		m_draggingFromContraption = fromContraption;
		m_contraptionDragX = contraptionX;
		m_contraptionDragY = contraptionY;
		m_dragStarted = false;
		if (m_useDragOffset)
		{
			if (fromContraption)
			{
				m_dragOffset = new Vector3(0f, 0.75f, 0f);
			}
			else
			{
				m_dragOffset = new Vector3(0f, 3f, 0f);
			}
		}
		if (m_draggedElement != -1)
		{
			PartDesc partDesc = m_partDescs[m_draggedElement];
			partDesc.useCount--;
			EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
		}
		m_draggedElement = element;
		if (element == -1)
		{
			if ((bool)m_dragIcon)
			{
				UnityEngine.Object.Destroy(m_dragIcon);
				m_dragIcon = null;
			}
			return;
		}
		if ((bool)m_dragIcon)
		{
			UnityEngine.Object.Destroy(m_dragIcon);
		}
		GameObject gameObject = null;
		BasePart basePart = null;
		if (m_draggingFromContraption)
		{
			basePart = WPFMonoBehaviour.gameData.GetCustomPart(m_partDescs[element].part.m_partType, m_draggedElementCustomizationIndex);
		}
		if (basePart != null)
		{
			gameObject = basePart.m_constructionIconSprite.gameObject;
		}
		if (gameObject == null)
		{
			gameObject = m_partDescs[element].part.m_constructionIconSprite.gameObject;
		}
		m_dragIcon = UnityEngine.Object.Instantiate(gameObject);
		float num = Vector3.Distance(GridPositionToGuiPosition(0, 0), GridPositionToGuiPosition(1, 0));
		SetSortingOrder(m_dragIcon, 0, "Popup");
		m_dragIcon.transform.localScale = new Vector3(num, num, num);
		EventManager.Send(new DragStartedEvent(m_partDescs[element].part.m_partType));
	}

	public static void SetSortingOrder(GameObject target, int sortingOrder, string sortingLayer = "")
	{
		if (target == null)
		{
			return;
		}
		Renderer[] componentsInChildren = target.GetComponentsInChildren<Renderer>();
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!string.IsNullOrEmpty(sortingLayer))
			{
				componentsInChildren[i].sortingLayerName = sortingLayer;
			}
			componentsInChildren[i].sortingOrder = sortingOrder;
		}
	}

	private int FindPartIndex(BasePart part)
	{
		for (int i = 0; i < m_partDescs.Count; i++)
		{
			if (m_partDescs[i].part.m_partType == part.m_partType)
			{
				return i;
			}
		}
		return -1;
	}

	private void OnPartCustomization(CustomizePartUI.PartCustomizationEvent data)
	{
		PartDesc partDesc = FindPartDesc(data.customizedPart);
		if (partDesc != null)
		{
			EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
		}
	}

	private void HandleDragging()
	{
		if (WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Building)
		{
			return;
		}
		GuiManager.Pointer pointer = GuiManager.GetPointer();
		if (DeviceInfo.UsesTouchInput)
		{
			if (Input.touchCount != 0)
			{
				TouchPhase phase = Input.GetTouch(0).phase;
				if (INSettings.GetBool(INFeature.PartPlacementOperation))
				{
					if (phase == TouchPhase.Moved)
					{
						m_pointerTime.x += Time.deltaTime;
					}
					if (phase == TouchPhase.Ended)
					{
						m_pointerTime.x = 0f;
					}
					if (m_pointerTime.x >= 0.4f || m_dragStarted)
					{
						m_pointerTime.y = 0f;
					}
				}
				if (INSettings.GetBool(INFeature.PartDeletionOperation))
				{
					if (phase == TouchPhase.Stationary)
					{
						m_pointerTime.y += Time.deltaTime;
					}
					if (phase == TouchPhase.Ended)
					{
						m_pointerTime.y = 0f;
					}
					if (m_pointerTime.y >= 0.4f)
					{
						m_pointerTime.x = 0f;
					}
				}
				if (INSettings.GetBool(INFeature.PartDeselectionOperation))
				{
					if (phase == TouchPhase.Stationary && pointer.onWidget)
					{
						m_pointerTime.z += Time.deltaTime;
					}
					if (phase == TouchPhase.Ended)
					{
						m_pointerTime.z = 0f;
					}
					if (m_pointerTime.z >= 0.4f)
					{
						partSelector.ResetSelection();
						m_selectedElement = -1;
					}
				}
			}
		}
		else
		{
			if (INSettings.GetBool(INFeature.PartPlacementOperation))
			{
				if (pointer.dragging)
				{
					m_pointerTime.x += Time.deltaTime;
				}
				else
				{
					m_pointerTime.x = 0f;
				}
			}
			if (INSettings.GetBool(INFeature.PartDeselectionOperation) && pointer.secondaryDown && pointer.onWidget)
			{
				partSelector.ResetSelection();
				m_selectedElement = -1;
			}
		}
		int constructionUiRows = m_partDescs.Count / m_itemsPerRow + ((m_partDescs.Count % m_itemsPerRow != 0) ? 1 : 0);
		WPFMonoBehaviour.levelManager.m_constructionUiRows = constructionUiRows;
		if (pointer.down && !pointer.onWidget && m_draggedElement == -1 && !m_dragStarted)
		{
			Vector3 vector = WPFMonoBehaviour.ScreenToZ0(pointer.position) - base.transform.position;
			int coordX = Mathf.RoundToInt(vector.x);
			int coordY = Mathf.RoundToInt(vector.y);
			ChangeCoordinatesToSelectBigPart(ref coordX, ref coordY);
			BasePart basePart = m_contraption.FindPartAt(coordX, coordY);
			if ((bool)basePart)
			{
				if ((bool)basePart.enclosedPart)
				{
					basePart = basePart.enclosedPart;
				}
				if (!basePart.m_static)
				{
					m_dragStartPosition = pointer.position;
					m_dragStarted = true;
				}
			}
		}
		if (pointer.up)
		{
			if (m_dragStarted && m_draggedElement == -1)
			{
				Vector3 vector2 = WPFMonoBehaviour.ScreenToZ0(m_dragStartPosition) - base.transform.position;
				int x = Mathf.RoundToInt(vector2.x);
				int y = Mathf.RoundToInt(vector2.y);
				BasePart basePart2 = m_contraption.FindPartAt(x, y);
				if ((bool)basePart2)
				{
					if ((bool)basePart2.enclosedPart)
					{
						basePart2 = basePart2.enclosedPart;
					}
					if (m_contraption.Flip(basePart2))
					{
						AddMove();
						m_rotationCounter++;
						Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.rotatePart);
					}
				}
			}
			m_dragStarted = false;
		}
		float num = 1f;
		if (DeviceInfo.UsesTouchInput)
		{
			num = ((!m_useDragOffset) ? 10f : 20f);
		}
		if (m_dragStarted && m_draggedElement == -1 && Vector3.Distance(pointer.position, m_dragStartPosition) >= num)
		{
			Vector3 vector3 = WPFMonoBehaviour.ScreenToZ0(m_dragStartPosition) - base.transform.position;
			int coordX2 = Mathf.RoundToInt(vector3.x);
			int coordY2 = Mathf.RoundToInt(vector3.y);
			BasePart basePart3 = m_contraption.FindPartAt(coordX2, coordY2);
			if (!basePart3 || (basePart3.m_partType != BasePart.PartType.Rope && basePart3.m_partType != BasePart.PartType.Spring))
			{
				ChangeCoordinatesToSelectBigPart(ref coordX2, ref coordY2);
			}
			BasePart basePart4 = m_contraption.RemovePartAt(coordX2, coordY2);
			if ((bool)basePart4)
			{
				m_draggedElementCustomizationIndex = basePart4.customPartIndex;
				EventManager.Send(new PartRemovedEvent(basePart4.m_partType, basePart4.transform.position));
				draggedPartRotation = basePart4.m_gridRotation;
				draggedPartFlipped = basePart4.m_flipped;
				for (int i = 0; i < m_partDescs.Count; i++)
				{
					if (m_partDescs[i].part.m_partType == basePart4.m_partType)
					{
						m_dragStartedFromContraption = true;
						SetDraggedElement(i, fromContraption: true, coordX2, coordY2);
						m_selectedElement = i;
						partSelector.SetSelection(m_partDescs[m_selectedElement]);
						break;
					}
				}
				if (INSettings.GetBool(INFeature.AutoSetPartBuildingIndex))
				{
					CustomizationManager.SetLastUsedPartIndex(basePart4.m_partType, basePart4.customPartIndex);
					EventManager.Send(new CustomizePartUI.PartCustomizationEvent(basePart4.m_partType, basePart4.customPartIndex));
				}
				if (INSettings.GetBool(INFeature.AutoSetPartBuildingRotation))
				{
					ContraptionExtensionData extensionData = INContraption.Instance.ExtensionData;
					if (extensionData != null)
					{
						(int, int) key = ((int)basePart4.m_partType, basePart4.customPartIndex);
						extensionData.PartRotations[key] = ((int)basePart4.m_gridRotation, basePart4.m_flipped);
					}
				}
				UnityEngine.Object.Destroy(basePart4.gameObject);
				ContraptionPartChanged(coordX2, coordY2);
				PlayDragSound();
			}
		}
		if (m_draggedElement != -1)
		{
			PartDesc partDesc = m_partDescs[m_draggedElement];
			Vector3 position = WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(pointer.position) + m_dragOffset;
			Vector3 position2 = WPFMonoBehaviour.hudCamera.GetComponent<Camera>().WorldToScreenPoint(position);
			Vector3 vector4 = Camera.main.ScreenToWorldPoint(position2);
			EventManager.Send(new DraggingPartEvent(partDesc.part.m_partType, vector4));
			Vector3 vector5 = vector4 - base.transform.position;
			int num2 = Mathf.RoundToInt(vector5.x);
			int num3 = Mathf.RoundToInt(vector5.y);
			if (m_useDragOffset)
			{
				int key2 = num3 * 1000 + num2;
				if (m_cellMap.TryGetValue(key2, out var value))
				{
					if ((bool)m_mouseOverCell)
					{
						m_mouseOverCell.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
					}
					m_mouseOverCell = value;
					value.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
				}
				else if ((bool)m_mouseOverCell)
				{
					m_mouseOverCell.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				}
			}
			if (pointer.up)
			{
				partDesc.customPartIndex = m_draggedElementCustomizationIndex;
				AddMove();
				if (WPFMonoBehaviour.levelManager.CanPlacePartAtGridCell(num2, num3) && m_contraption.CanPlaceSpecificPartAt(num2, num3, partDesc.part) && !pointer.onWidget)
				{
					partDesc.useCount++;
					EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
					BasePart basePart5 = partDesc.part;
					if (m_draggingFromContraption)
					{
						basePart5 = WPFMonoBehaviour.gameData.GetCustomPart(basePart5.m_partType, m_draggedElementCustomizationIndex);
					}
					BasePart basePart6 = SetPartAt(num2, num3, basePart5);
					ContraptionPartChanged(num2, num3);
					PlayPartPlacedSound();
					basePart6.OnPartPlaced();
					EventManager.Send(new PartPlacedEvent(partDesc.part.m_partType, partDesc.part.m_partTier, basePart6.transform.position));
				}
				else
				{
					PlayRemoveSound();
				}
				SetDraggedElement(-1);
			}
		}
		else if (m_selectedElement != -1 && m_draggedElement == -1 && !pointer.onWidget)
		{
			PartDesc partDesc2 = m_partDescs[m_selectedElement];
			Vector3 vector6 = WPFMonoBehaviour.ScreenToZ0(pointer.position) - base.transform.position;
			int num4 = Mathf.RoundToInt(vector6.x);
			int num5 = Mathf.RoundToInt(vector6.y);
			BasePart basePart7 = m_contraption.FindPartAt(num4, num5);
			if (!basePart7 && (m_pointerTime.x >= 0.4f || pointer.down || (m_allowDragPlacement && pointer.dragging)) && partDesc2.useCount < partDesc2.maxCount)
			{
				if (TryPlacePartAtGridCell(num4, num5, partDesc2))
				{
					AddMove();
				}
			}
			else if ((bool)basePart7 && basePart7.CanEncloseParts() && partDesc2.part.CanBeEnclosed() && (m_pointerTime.x >= 0.4f || pointer.up) && (!basePart7.enclosedPart || partDesc2.part.m_partType != basePart7.enclosedPart.m_partType) && partDesc2.useCount < partDesc2.maxCount && TryPlacePartAtGridCell(num4, num5, partDesc2))
			{
				AddMove();
			}
		}
		if (pointer.secondaryDown)
		{
			m_rightDragStartPosition = pointer.position;
		}
		if (m_draggedElement == -1 && (m_pointerTime.y >= 0.4f || pointer.secondaryDown || (pointer.secondaryDragging && pointer.position != m_rightDragStartPosition && !pointer.dragging)))
		{
			Vector3 vector7 = WPFMonoBehaviour.ScreenToZ0(pointer.position) - base.transform.position;
			int x2 = Mathf.RoundToInt(vector7.x);
			int y2 = Mathf.RoundToInt(vector7.y);
			BasePart basePart8 = m_contraption.FindPartAt(x2, y2);
			if ((bool)basePart8 && (bool)basePart8.enclosedPart)
			{
				basePart8 = basePart8.enclosedPart;
			}
			if ((bool)basePart8 && !basePart8.m_static)
			{
				BasePart basePart9 = m_contraption.RemovePartAt(x2, y2);
				if ((bool)basePart9)
				{
					EventManager.Send(new PartRemovedEvent(basePart9.m_partType, basePart9.transform.position));
					for (int j = 0; j < m_partDescs.Count; j++)
					{
						if (m_partDescs[j].part.m_partType == basePart9.m_partType)
						{
							PartDesc partDesc3 = m_partDescs[j];
							partDesc3.useCount--;
							EventManager.Send(new PartCountChanged(partDesc3.part.m_partType, partDesc3.CurrentCount));
							AddMove();
							break;
						}
					}
					UnityEngine.Object.Destroy(basePart9.gameObject);
					ContraptionPartChanged(x2, y2);
				}
			}
		}
		if (m_draggedElement == -1 && (bool)m_mouseOverCell)
		{
			m_mouseOverCell.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			m_mouseOverCell = null;
		}
	}

	private void ChangeCoordinatesToSelectBigPart(ref int coordX, ref int coordY)
	{
		for (int i = -1; i <= 0; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (j == 0 && i == 0)
				{
					continue;
				}
				int num = coordX + j;
				int num2 = coordY + i;
				BasePart basePart = m_contraption.FindPartAt(num, num2);
				if ((bool)basePart)
				{
					if ((bool)basePart.enclosedPart)
					{
						basePart = basePart.enclosedPart;
					}
					if (num + basePart.m_gridXmin <= coordX && num + basePart.m_gridXmax >= coordX && num2 + basePart.m_gridYmin <= coordY && num2 + basePart.m_gridYmax >= coordY)
					{
						coordX = num;
						coordY = num2;
						return;
					}
				}
			}
		}
	}

	private bool TryPlacePartAtGridCell(int coordX, int coordY, PartDesc partDesc)
	{
		if (WPFMonoBehaviour.levelManager.CanPlacePartAtGridCell(coordX, coordY) && m_contraption.CanPlaceSpecificPartAt(coordX, coordY, partDesc.part))
		{
			partDesc.useCount++;
			EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
			BasePart basePart = SetPartAt(coordX, coordY, partDesc.part);
			ContraptionPartChanged(coordX, coordY);
			PlayPartPlacedSound();
			basePart.OnPartPlaced();
			EventManager.Send(new PartPlacedEvent(basePart.m_partType, basePart.m_partTier, basePart.transform.position));
			return true;
		}
		return false;
	}

	private void ClearNonChassisPart(int coordX, int coordY)
	{
		BasePart basePart = m_contraption.FindPartAt(coordX, coordY);
		if (!basePart || (basePart.IsPartOfChassis() && !basePart.enclosedPart))
		{
			return;
		}
		basePart = m_contraption.RemovePartAt(coordX, coordY);
		for (int i = 0; i < m_partDescs.Count; i++)
		{
			if (m_partDescs[i].part.m_partType == basePart.m_partType)
			{
				PartDesc partDesc = m_partDescs[i];
				partDesc.useCount--;
				EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
				break;
			}
		}
		UnityEngine.Object.Destroy(basePart.gameObject);
		ContraptionPartChanged(coordX, coordY);
	}

	public Vector3 GridPositionToGuiPosition(int x, int y)
	{
		Vector3 position = m_contraption.transform.position + Vector3.right * x + Vector3.up * y;
		Vector3 position2 = Camera.main.WorldToScreenPoint(position);
		return WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(position2);
	}

	public Vector3 GridPositionToWorldPosition(int x, int y)
	{
		return m_contraption.transform.position + Vector3.right * x + Vector3.up * y;
	}

	private void ClearCollidingParts(int coordX, int coordY, BasePart part)
	{
		for (int i = part.m_gridYmin; i <= part.m_gridYmax; i++)
		{
			for (int j = part.m_gridXmin; j <= part.m_gridXmax; j++)
			{
				if (j == 0 && i == 0)
				{
					continue;
				}
				if (part.m_partType == BasePart.PartType.GoldenPig)
				{
					BasePart basePart = m_contraption.FindPartAt(coordX + j, coordY + i);
					if ((bool)basePart && (basePart.m_partType == BasePart.PartType.Rope || basePart.m_partType == BasePart.PartType.Spring))
					{
						continue;
					}
				}
				ClearNonChassisPart(coordX + j, coordY + i);
			}
		}
		if (part.IsPartOfChassis())
		{
			return;
		}
		for (int k = -1; k <= 0; k++)
		{
			for (int l = -1; l <= 1; l++)
			{
				BasePart basePart2 = m_contraption.FindPartAt(coordX + l, coordY + k);
				if ((bool)basePart2)
				{
					if ((bool)basePart2.enclosedPart)
					{
						basePart2 = basePart2.enclosedPart;
					}
					if ((basePart2.m_partType == BasePart.PartType.KingPig || basePart2.m_partType == BasePart.PartType.GoldenPig) && ((part.m_partType != BasePart.PartType.Rope && part.m_partType != BasePart.PartType.Spring) || basePart2.m_partType != BasePart.PartType.GoldenPig))
					{
						ClearNonChassisPart(coordX + l, coordY + k);
					}
				}
			}
		}
	}

	public BasePart SetPartAt(int coordX, int coordY, BasePart part, bool autoalign = true)
	{
		GameObject obj = UnityEngine.Object.Instantiate(part.gameObject);
		obj.SetActive(value: true);
		BasePart component = obj.GetComponent<BasePart>();
		BasePart basePart = m_contraption.FindPartAt(coordX, coordY);
		if (component.m_partType == BasePart.PartType.Pig && (bool)basePart && basePart.m_partType == BasePart.PartType.WoodenFrame && Singleton<SocialGameManager>.IsInstantiated())
		{
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.THINK_INSIDE_THE_BOX", 100.0);
		}
		ClearCollidingParts(coordX, coordY, component);
		component.PrePlaced();
		BasePart basePart2 = m_contraption.SetPartAt(coordX, coordY, component, autoalign);
		if (autoalign)
		{
			m_contraption.AutoAlign(component);
		}
		if ((bool)basePart2)
		{
			EventManager.Send(new PartRemovedEvent(basePart2.m_partType, basePart2.transform.position));
			CollectPart(basePart2);
		}
		if (INSettings.GetBool(INFeature.AutoSetPartBuildingRotation))
		{
			BasePart basePart3 = component;
			ContraptionExtensionData extensionData = INContraption.Instance.ExtensionData;
			if (extensionData != null)
			{
				(int, int) key = ((int)basePart3.m_partType, basePart3.customPartIndex);
				if (extensionData.PartRotations != null && extensionData.PartRotations.TryGetValue(key, out var value))
				{
					if (value.Item2)
					{
						basePart3.SetFlipped(value.Item2);
					}
					basePart3.SetRotation((BasePart.GridRotation)value.Item1);
					basePart3.OnChangeConnections();
					m_contraption.RefreshNeighboursVisual(basePart3.m_coordX, basePart3.m_coordY);
				}
			}
		}
		return component;
	}

	protected void CollectPart(BasePart part)
	{
		if (!part)
		{
			return;
		}
		foreach (PartDesc partDesc in m_partDescs)
		{
			if (partDesc != null && partDesc.part != null && partDesc.part.m_partType == part.m_partType)
			{
				partDesc.useCount--;
				EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
			}
		}
		if ((bool)part.enclosedPart)
		{
			CollectPart(part.enclosedPart);
		}
		UnityEngine.Object.Destroy(part.gameObject);
	}

	public void SetEnabled(bool enableUI, bool enableGrid)
	{
		if ((bool)m_grid)
		{
			m_grid.gameObject.SetActive(enableGrid);
		}
		partSelector.SetConstructionUI(this);
		if (enableUI)
		{
			m_contraption.RefreshConnections();
		}
		base.gameObject.SetActive(enableUI || enableGrid);
		base.enabled = enableUI;
	}

	public void ClearContraption()
	{
		m_contraption.RemoveAllDynamicParts();
		foreach (PartDesc partDesc in m_partDescs)
		{
			partDesc.useCount = 0;
			EventManager.Send(new PartCountChanged(partDesc.part.m_partType, partDesc.CurrentCount));
		}
		SetMoveButtonStates();
		m_moveCounter = 0;
	}

	public void ApplySuperGlue(bool apply)
	{
		m_contraption.ApplySuperGlue(Glue.Type.Regular);
	}

	public void ApplySuperMagnet(bool apply)
	{
		m_contraption.HasSuperMagnet = apply;
	}

	public void ApplyTurboCharge(bool apply)
	{
		m_contraption.HasTurboCharge = apply;
	}

	public void ApplyNightVision(bool apply)
	{
		m_contraption.HasNightVision = apply;
	}

	public void PlayPartPlacedSound()
	{
		if (Singleton<AudioManager>.IsInstantiated())
		{
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.placePart);
		}
	}

	public void PlayDragSound()
	{
		if (WPFMonoBehaviour.gameData.commonAudioCollection.dragPart != null && Singleton<AudioManager>.IsInstantiated())
		{
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.dragPart);
		}
	}

	public void PlayRemoveSound()
	{
		if (Singleton<AudioManager>.IsInstantiated())
		{
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.removePart);
		}
	}
}
