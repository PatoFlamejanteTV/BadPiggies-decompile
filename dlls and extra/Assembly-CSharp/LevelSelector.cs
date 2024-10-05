using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : WPFMonoBehaviour
{
	[Serializable]
	public class OneTimeCutScene
	{
		public bool enabled;

		public string cutScene;

		public string saveId;
	}

	public GameObject m_levelButtonPrefab;

	public GameObject m_levelJokerButtonPrefab;

	private const int m_levelsPerRow = 5;

	public int m_levelsPerPage = 15;

	[SerializeField]
	private int m_episodeLevelsGameDataIndex;

	[SerializeField]
	private Transform m_startingCutsceneButton;

	[SerializeField]
	private Transform m_midCutsceneButton;

	[SerializeField]
	private int m_midCutsceneButtonPage;

	[SerializeField]
	private Transform m_endingCutsceneButton;

	[SerializeField]
	private GameObject m_leftScroll;

	[SerializeField]
	private GameObject m_rightScroll;

	[SerializeField]
	private GameObject m_comingSoonIcon;

	[SerializeField]
	private GameObject m_pageContentLimitedOverlay;

	[SerializeField]
	private GameObject m_partialPageContentLimitedOverlay;

	[SerializeField]
	private GameObject m_pageDot;

	[SerializeField]
	private GameObject m_starEffect;

	[SerializeField]
	private GameObject m_woodCrate;

	[SerializeField]
	private GameObject m_metalCrate;

	[SerializeField]
	private GameObject m_goldCrate;

	[SerializeField]
	private GameObject m_cardboardCrate;

	[SerializeField]
	private GameObject m_glassCrate;

	[SerializeField]
	private GameObject m_bronzeCrate;

	[SerializeField]
	private GameObject m_marbleCrate;

	public bool m_pageTwoComingSoon;

	public bool m_pageThreeComingSoon;

	public OneTimeCutScene m_oneTimeCutscene;

	private List<EpisodeLevelInfo> m_levels = new List<EpisodeLevelInfo>();

	private List<int> m_starlimitsLevels = new List<int>();

	private List<PageDot> m_dotsList = new List<PageDot>();

	private int m_page;

	private int m_pageCount;

	private Vector2 m_initialInputPos;

	private Vector2 m_lastInputPos;

	private ButtonGrid m_buttonGrid;

	private float m_leftDragLimit;

	private float m_rightDragLimit;

	private bool m_interacting;

	private int m_currentScreenWidth;

	private int m_currentScreenHeight;

	private bool m_isIapOpen;

	private bool m_isDialogOpen;

	private GameObject m_extraDarkLayerRight;

	private static string[] romanNumeralStrings = new string[9] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

	private bool startedLevelLoading;

	public List<EpisodeLevelInfo> Levels
	{
		get
		{
			return m_levels;
		}
		set
		{
			m_levels = value;
		}
	}

	public List<int> StarLevelLimits
	{
		get
		{
			return m_starlimitsLevels;
		}
		set
		{
			m_starlimitsLevels = value;
		}
	}

	public int EpisodeIndex => m_episodeLevelsGameDataIndex;

	public string OpeningCutscene => m_startingCutsceneButton.GetComponent<Button>().MethodToCall.GetParameter<string>(0);

	public string MidCutscene
	{
		get
		{
			if (m_midCutsceneButton != null)
			{
				return m_midCutsceneButton.GetComponent<Button>().MethodToCall.GetParameter<string>(0);
			}
			return null;
		}
	}

	public string EndingCutscene => m_endingCutsceneButton.GetComponent<Button>().MethodToCall.GetParameter<string>(0);

	public int CurrentPage => Mathf.Clamp(Mathf.RoundToInt(m_buttonGrid.transform.localPosition.x / (0f - WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x)), 1, m_pageCount);

	private float ButtonXGap => Mathf.Clamp((float)Screen.width / 7f, (float)(80 * Screen.width) / 1024f, (float)(160 * Screen.height) / 768f);

	public static string DifferentiatedLevelLabel(int levelIndex)
	{
		int num = levelIndex + 1;
		if (num % 5 != 0)
		{
			return (num - num / 5).ToString();
		}
		int num2 = num / 5;
		if (num2 >= 1 && num2 <= 9)
		{
			return romanNumeralStrings[num2 - 1];
		}
		return "Z";
	}

	private void OnEnable()
	{
		IapManager.onPurchaseSucceeded += HandleIapManageronPurchaseSucceeded;
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		KeyListener.keyReleased += HandleKeyListenerkeyReleased;
		if (DeviceInfo.IsDesktop)
		{
			KeyListener.mouseWheel += HandleKeyListenerMouseWheel;
		}
	}

	private void HandleKeyListenerkeyReleased(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			GoToEpisodeSelection();
		}
		else if (DeviceInfo.ActiveDeviceFamily != DeviceInfo.DeviceFamily.Android && obj == KeyCode.RightArrow && m_rightScroll.activeInHierarchy)
		{
			NextPage();
		}
		else if (DeviceInfo.ActiveDeviceFamily != DeviceInfo.DeviceFamily.Android && obj == KeyCode.LeftArrow && m_leftScroll.activeInHierarchy)
		{
			PreviousPage();
		}
	}

	private void HandleKeyListenerMouseWheel(float delta)
	{
		if (delta < 0f && m_rightScroll.activeInHierarchy && !m_interacting)
		{
			NextPage();
		}
		else if (delta > 0f && m_leftScroll.activeInHierarchy && !m_interacting)
		{
			PreviousPage();
		}
	}

	private void OnDisable()
	{
		IapManager.onPurchaseSucceeded -= HandleIapManageronPurchaseSucceeded;
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
		KeyListener.keyReleased -= HandleKeyListenerkeyReleased;
		if (DeviceInfo.IsDesktop)
		{
			KeyListener.mouseWheel -= HandleKeyListenerMouseWheel;
		}
	}

	private void Awake()
	{
		if (GameTime.IsPaused())
		{
			GameTime.Pause(pause: false);
		}
		m_page = UserSettings.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_active_page");
		if (GameProgress.GetBool("show_content_limit_popup"))
		{
			GameProgress.DeleteKey("show_content_limit_popup");
			if (Singleton<BuildCustomizationLoader>.Instance.IsContentLimited)
			{
				if (m_page == 0)
				{
					EventManager.SendOnNextUpdate(this, new PulseButtonEvent(UIEvent.Type.OpenUnlockFullVersionIapMenu, pulse: false));
					LevelInfo.DisplayContentLimitNotification();
				}
			}
			else
			{
				LevelInfo.DisplayContentLimitNotification();
			}
		}
		Levels = WPFMonoBehaviour.gameData.m_episodeLevels[EpisodeIndex].m_levelInfos;
		StarLevelLimits = WPFMonoBehaviour.gameData.m_episodeLevels[EpisodeIndex].StarLevelLimits;
		m_pageCount = Mathf.RoundToInt(m_levels.Count / m_levelsPerPage);
		m_buttonGrid = base.transform.Find("ButtonGrid").GetComponent<ButtonGrid>();
		m_currentScreenWidth = Screen.width;
		m_currentScreenHeight = Screen.height;
		m_page = Mathf.Min(m_page, m_pageCount);
		Singleton<GameManager>.Instance.OpenEpisode(this);
		CreateButtons();
		CreatePageDots();
		LayoutButtons(m_page);
		if (DeviceInfo.UsesTouchInput)
		{
			m_leftScroll.SetActive(value: false);
			m_rightScroll.SetActive(value: false);
		}
		if (GameProgress.TotalDessertCount() > 0)
		{
			EventManager.Send(new PulseButtonEvent(UIEvent.Type.None));
		}
	}

	private void OnPageChanged()
	{
	}

	public void ReceiveUIEvent(UIEvent data)
	{
		_ = data.type;
		_ = 31;
	}

	public void SendExitLevelSelectionFlurryEvent()
	{
	}

	public void SendStandardFlurryEvent(string eventName, string id)
	{
	}

	public void GoToEpisodeSelection()
	{
		Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: false);
	}

	public void GoToKingPigFeed()
	{
		Singleton<GameManager>.Instance.LoadKingPigFeed(showLoadingScreen: false);
	}

	public void LoadLevel(string levelIndex)
	{
		if (startedLevelLoading)
		{
			return;
		}
		startedLevelLoading = true;
		SendStandardFlurryEvent("Select Level", levelIndex);
		int num = int.Parse(levelIndex);
		if (num >= 0)
		{
			if (m_oneTimeCutscene.enabled && !GameProgress.GetBool(m_oneTimeCutscene.saveId))
			{
				Singleton<GameManager>.Instance.LoadLevelAfterCutScene(m_levels[num], m_oneTimeCutscene.cutScene);
				GameProgress.SetBool(m_oneTimeCutscene.saveId, value: true);
			}
			else
			{
				Singleton<GameManager>.Instance.LoadLevel(num);
			}
		}
		else
		{
			startedLevelLoading = false;
		}
	}

	public IEnumerator LoadLevelDelayed(string episodeBundleId, int levelIndex)
	{
		while (!Bundle.IsBundleLoaded(episodeBundleId))
		{
			yield return null;
		}
		if (m_oneTimeCutscene.enabled && !GameProgress.GetBool(m_oneTimeCutscene.saveId))
		{
			Singleton<GameManager>.Instance.LoadLevelAfterCutScene(m_levels[levelIndex], m_oneTimeCutscene.cutScene);
			GameProgress.SetBool(m_oneTimeCutscene.saveId, value: true);
		}
		else
		{
			Singleton<GameManager>.Instance.LoadLevel(levelIndex);
		}
	}

	public void LoadStarLevel(string levelIndex)
	{
		SendStandardFlurryEvent("Select Level", levelIndex);
		if (m_oneTimeCutscene.enabled && !GameProgress.GetBool(m_oneTimeCutscene.saveId))
		{
			Singleton<GameManager>.Instance.LoadLevelAfterCutScene(m_levels[int.Parse(levelIndex)], m_oneTimeCutscene.cutScene);
			GameProgress.SetBool(m_oneTimeCutscene.saveId, value: true);
		}
		else
		{
			Singleton<GameManager>.Instance.LoadStarLevelTransition(m_levels[int.Parse(levelIndex)]);
		}
	}

	public void LoadOpeningCutscene(string cutscene)
	{
		Singleton<GameManager>.Instance.LoadOpeningCutscene();
		if (m_oneTimeCutscene.enabled)
		{
			GameProgress.SetBool(m_oneTimeCutscene.saveId, value: true);
		}
	}

	public void LoadMidCutscene(string cutscene, bool isStartedFromLevelSelection = false)
	{
		Singleton<GameManager>.Instance.LoadMidCutscene(isStartedFromLevelSelection);
	}

	public void LoadEndingCutscene(string cutscene)
	{
		Singleton<GameManager>.Instance.LoadEndingCutscene();
	}

	public void NextPage()
	{
		int page = m_page;
		m_page = Mathf.Clamp(m_page + 1, 0, m_pageCount - 1);
		if (page != m_page)
		{
			OnPageChanged();
		}
		for (int i = 0; i < m_dotsList.Count; i++)
		{
			if (i == m_page)
			{
				m_dotsList[i].Enable();
			}
			else
			{
				m_dotsList[i].Disable();
			}
		}
	}

	public void PreviousPage()
	{
		int page = m_page;
		m_page = Mathf.Clamp(m_page - 1, 0, m_pageCount - 1);
		if (page != m_page)
		{
			OnPageChanged();
		}
		for (int i = 0; i < m_dotsList.Count; i++)
		{
			if (i == m_page)
			{
				m_dotsList[i].Enable();
			}
			else
			{
				m_dotsList[i].Disable();
			}
		}
	}

	private void CreateButtons()
	{
		int num = 0;
		m_buttonGrid.Clear();
		for (int i = 0; i < m_levels.Count; i++)
		{
			int num2 = i / 5;
			int page = num2 / 3;
			bool flag = LevelInfo.IsContentLimited(EpisodeIndex, i);
			bool flag2 = LevelInfo.IsStarLevel(EpisodeIndex, i);
			bool num3 = LevelInfo.IsLevelUnlocked(EpisodeIndex, i);
			bool flag3 = LevelInfo.CanAdUnlock(EpisodeIndex, i);
			bool flag4 = i % 5 == 0;
			bool flag5 = i == 0 || LevelInfo.IsLevelCompleted(EpisodeIndex, (!flag4) ? (i - 1) : (i - 2));
			bool flag6 = true;
			bool flag7 = LevelInfo.IsLevelUnlocked(EpisodeIndex, num2 * 5);
			bool showRowUnlockStarEffect = GameProgress.GetShowRowUnlockStarEffect(EpisodeIndex, num2);
			bool flag8 = num2 != 0 || true;
			Button button = ((!flag2) ? UnityEngine.Object.Instantiate(m_levelButtonPrefab).GetComponent<Button>() : UnityEngine.Object.Instantiate(m_levelJokerButtonPrefab).GetComponent<Button>());
			button.name = "LevelButton";
			button.transform.parent = m_buttonGrid.transform;
			m_buttonGrid.AddButton(button);
			if (num3 || ((flag6 || flag7) && flag5 && !flag2))
			{
				num = i;
				UnlockLevel(button, i, flag2);
			}
			else if (flag2 && (flag6 || !flag))
			{
				LockLevel(button, i, flag2, isContentLimited: false, isAdUnlocked: true);
			}
			else if (!flag)
			{
				LockLevel(button, i, flag2, isContentLimited: false, isAdUnlocked: false);
			}
			else if (flag4 && flag8 && flag3)
			{
				AddUnlockPanel(button, num2, page, EpisodeIndex, flag5);
				LockLevel(button, i, flag2, LevelInfo.IsContentLimited(EpisodeIndex, i), flag6 || flag7);
			}
			else if (flag4 && !flag8)
			{
				AddLockedPanel(button);
				LockLevel(button, i, flag2, LevelInfo.IsContentLimited(EpisodeIndex, i), flag6 || flag7);
			}
			else
			{
				bool isContentLimited = LevelInfo.IsContentLimited(EpisodeIndex, i);
				LockLevel(button, i, flag2, isContentLimited, flag6 || flag7);
			}
			if (showRowUnlockStarEffect)
			{
				ShowStarEffect();
				GameProgress.SetShowRowUnlockStarEffect(EpisodeIndex, num2, show: false);
			}
			if (i == 0 && GameProgress.GetInt(OpeningCutscene + "_played") == 0)
			{
				button.MethodToCall.SetMethod(button.MethodToCall.TargetObject.GetComponent(button.MethodToCall.TargetComponent), "LoadOpeningCutscene");
			}
			if (MidCutscene != null && i == 15 && LevelInfo.IsLevelUnlocked(EpisodeIndex, i) && GameProgress.GetInt(MidCutscene + "_played") == 0)
			{
				button.MethodToCall.SetMethod(button.MethodToCall.TargetObject.GetComponent(button.MethodToCall.TargetComponent), "LoadMidCutscene");
			}
			if (Singleton<DailyChallenge>.Instance.IsDailyChallenge(EpisodeIndex, i, out var index) && Singleton<DailyChallenge>.Instance.IsLocationRevealed(index) && !Singleton<DailyChallenge>.Instance.DailyChallengeCollected(index))
			{
				GameObject gameObject = Singleton<DailyChallenge>.Instance.TodaysLootCrate(index) switch
				{
					LootCrateType.Metal => UnityEngine.Object.Instantiate(m_metalCrate), 
					LootCrateType.Gold => UnityEngine.Object.Instantiate(m_goldCrate), 
					LootCrateType.Cardboard => UnityEngine.Object.Instantiate(m_cardboardCrate), 
					LootCrateType.Glass => UnityEngine.Object.Instantiate(m_glassCrate), 
					LootCrateType.Bronze => UnityEngine.Object.Instantiate(m_bronzeCrate), 
					LootCrateType.Marble => UnityEngine.Object.Instantiate(m_marbleCrate), 
					_ => UnityEngine.Object.Instantiate(m_woodCrate), 
				};
				gameObject.layer = button.gameObject.layer;
				gameObject.transform.parent = button.transform;
				gameObject.transform.localPosition = new Vector3(1.25f, 1.25f, -1f);
				gameObject.GetComponent<Animation>().Play();
			}
		}
		if (m_pageTwoComingSoon && m_levels.Count > 15)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(m_comingSoonIcon);
			gameObject2.transform.parent = m_buttonGrid.transform.GetChild(22);
			gameObject2.transform.localPosition = Vector3.zero - Vector3.forward * 4f;
			if (!m_pageThreeComingSoon || m_levels.Count <= 30)
			{
				Transform transform = UnityEngine.Object.Instantiate(gameObject2.transform.GetChild(0));
				transform.parent = gameObject2.transform;
				Vector3 localPosition = Vector3.zero + Vector3.right * WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3((float)Screen.width * 1.5f, 0f, 0f)).x;
				localPosition.y = gameObject2.transform.GetChild(0).transform.localPosition.y;
				transform.transform.localPosition = localPosition;
				m_extraDarkLayerRight = transform.gameObject;
			}
		}
		if (m_pageThreeComingSoon && m_levels.Count > 30)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(m_comingSoonIcon);
			gameObject3.transform.parent = m_buttonGrid.transform.GetChild(37);
			gameObject3.transform.localPosition = Vector3.zero - Vector3.forward * 4f;
			Transform transform2 = UnityEngine.Object.Instantiate(gameObject3.transform.GetChild(0));
			transform2.parent = gameObject3.transform;
			Vector3 localPosition2 = Vector3.zero + Vector3.right * WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3((float)Screen.width * 1.5f, 0f, 0f)).x;
			localPosition2.y = gameObject3.transform.GetChild(0).transform.localPosition.y;
			transform2.transform.localPosition = localPosition2;
			m_extraDarkLayerRight = transform2.gameObject;
		}
		m_startingCutsceneButton.gameObject.SetActive(GameProgress.GetInt(m_startingCutsceneButton.GetComponent<Button>().MethodToCall.GetParameter<string>(0) + "_played") == 1);
		bool active = num >= m_midCutsceneButtonPage * 15 && m_levels.Count > 15 && !string.IsNullOrEmpty(MidCutscene) && GameProgress.GetInt(m_midCutsceneButton.GetComponent<Button>().MethodToCall.GetParameter<string>(0) + "_played") == 1;
		bool active2 = m_levels.Count > 15 && !string.IsNullOrEmpty(EndingCutscene) && GameProgress.GetInt(m_endingCutsceneButton.GetComponent<Button>().MethodToCall.GetParameter<string>(0) + "_played") == 1;
		if ((bool)m_midCutsceneButton)
		{
			m_midCutsceneButton.gameObject.SetActive(active);
		}
		if ((bool)m_endingCutsceneButton)
		{
			m_endingCutsceneButton.gameObject.SetActive(active2);
		}
	}

	private void CreatePageDots()
	{
		Vector3 position = -Vector3.up * WPFMonoBehaviour.hudCamera.orthographicSize / 1.25f;
		GameObject gameObject = UnityEngine.Object.Instantiate(new GameObject(), position, Quaternion.identity);
		gameObject.name = "PageDots";
		gameObject.transform.parent = base.transform;
		float num = (0f - (float)m_pageCount) / 2f * 1.2f;
		for (int i = 0; i < m_pageCount; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(m_pageDot);
			obj.transform.parent = gameObject.transform;
			obj.transform.localPosition = new Vector3(num + (float)i * 1.2f, 0f, -95f);
			obj.name = "Dot" + i + 1;
			PageDot component = obj.GetComponent<PageDot>();
			m_dotsList.Add(component);
			if (i == m_page)
			{
				m_dotsList[i].Enable();
			}
			else
			{
				m_dotsList[i].Disable();
			}
		}
	}

	public float UnlockFullVersionButtonX()
	{
		float num = 4f;
		float num2 = (float)Screen.width / (float)Screen.height;
		if (num2 < 1.45f)
		{
			num /= 1.45f / num2;
		}
		float num3 = Mathf.Clamp((float)Screen.width / 7f, (float)(80 * Screen.width) / 1024f, (float)(160 * Screen.height) / 768f);
		Vector2 vector = new Vector2(((float)Screen.width - num3 * 4f) / 2f, (float)Screen.height * 0.75f);
		return WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(vector).x - num;
	}

	private void LayoutButtons(int activePage)
	{
		int page = m_page;
		m_page = activePage;
		if (page != m_page)
		{
			OnPageChanged();
		}
		float num = Mathf.Clamp((float)Screen.width / 7f, (float)(80 * Screen.width) / 1024f, (float)(160 * Screen.height) / 768f);
		Vector2 vector = new Vector2(((float)Screen.width - num * (float)(m_buttonGrid.horizontalCount - 1)) / 2f, (float)Screen.height * 0.75f);
		Vector2 vector2 = new Vector2(num, (float)Screen.height * 0.22f);
		int num2 = Screen.width / 4;
		m_rightDragLimit = 0f - WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3(Screen.width * m_pageCount - num2, 0f, 0f)).x;
		m_leftDragLimit = 0f - WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3(num2, 0f, 0f)).x;
		int num3 = Screen.width * m_page;
		m_buttonGrid.transform.position = new Vector3(0f - WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2 + num3, 0f, 0f)).x, m_buttonGrid.transform.localPosition.y, m_buttonGrid.transform.localPosition.z);
		for (int num4 = m_buttonGrid.transform.childCount - 1; num4 >= 0; num4--)
		{
			int num5 = num4 % m_levelsPerPage / 5;
			int num6 = num4 % 5;
			int num7 = num4 / m_levelsPerPage;
			int num8 = Screen.width * num7;
			Button component = m_buttonGrid.transform.GetChild(num4).GetComponent<Button>();
			Vector3 position = new Vector3(vector.x + (float)num6 * vector2.x + (float)num8 - (float)num3, vector.y - (float)num5 * vector2.y, 20f);
			Vector3 position2 = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(position);
			if (!GameProgress.GetFullVersionUnlocked() && m_episodeLevelsGameDataIndex > 0 && num7 == 0 && num5 == 0)
			{
				position2.z -= 10f;
			}
			component.transform.position = position2;
		}
		for (int i = 0; i < m_dotsList.Count; i++)
		{
			if (i == m_page)
			{
				m_dotsList[i].Enable();
			}
			else
			{
				m_dotsList[i].Disable();
			}
		}
		if ((bool)m_extraDarkLayerRight)
		{
			Vector3 localPosition = m_extraDarkLayerRight.transform.localPosition;
			localPosition.x = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3((float)Screen.width * 1.5f, 0f, 0f)).x;
			m_extraDarkLayerRight.transform.localPosition = localPosition;
		}
	}

	private void Update()
	{
		if (m_currentScreenWidth != Screen.width || m_currentScreenHeight != Screen.height)
		{
			LayoutButtons(m_page);
			m_currentScreenWidth = Screen.width;
			m_currentScreenHeight = Screen.height;
		}
		if (!m_interacting)
		{
			Vector3 vector = new Vector3(0f - WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2 + Screen.width * m_page, 0f, 0f)).x, m_buttonGrid.transform.localPosition.y, m_buttonGrid.transform.localPosition.z);
			m_buttonGrid.transform.position += (vector - m_buttonGrid.transform.position) * Time.deltaTime * 4f;
			if ((vector - m_buttonGrid.transform.position).magnitude < 1f)
			{
				if (UserSettings.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_active_page", -1) != m_page)
				{
					UserSettings.SetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_active_page", m_page);
				}
				if (!DeviceInfo.UsesTouchInput)
				{
					m_rightScroll.SetActive(value: true);
					m_leftScroll.SetActive(value: true);
				}
			}
			else if (!DeviceInfo.UsesTouchInput)
			{
				m_rightScroll.SetActive(value: false);
				m_leftScroll.SetActive(value: false);
			}
			if (!DeviceInfo.UsesTouchInput)
			{
				if (CurrentPage == 0)
				{
					m_leftScroll.SetActive(value: false);
				}
				if (CurrentPage == m_pageCount || m_pageCount == 1)
				{
					m_rightScroll.SetActive(value: false);
				}
			}
		}
		if (m_isIapOpen || m_isDialogOpen)
		{
			return;
		}
		GuiManager.Pointer pointer = GuiManager.GetPointer();
		if (pointer.down && pointer.widget != m_leftScroll.GetComponent<Widget>() && pointer.widget != m_rightScroll.GetComponent<Widget>())
		{
			m_initialInputPos = pointer.position;
			m_lastInputPos = pointer.position;
			m_interacting = true;
		}
		if (pointer.dragging && m_interacting)
		{
			Vector3 vector2 = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(m_lastInputPos);
			Vector3 vector3 = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(pointer.position);
			m_lastInputPos = pointer.position;
			float num = vector3.x - vector2.x;
			m_buttonGrid.transform.localPosition = new Vector3(Mathf.Clamp(m_buttonGrid.transform.localPosition.x + num, m_rightDragLimit, m_leftDragLimit), m_buttonGrid.transform.localPosition.y, m_buttonGrid.transform.localPosition.z);
			Vector3 vector4 = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(m_initialInputPos);
			if (!DeviceInfo.UsesTouchInput && Mathf.Abs(vector3.x - vector4.x) > 0.2f)
			{
				m_rightScroll.SetActive(value: false);
				m_leftScroll.SetActive(value: false);
			}
			if (Mathf.Abs(vector3.x - vector4.x) > 1f)
			{
				Singleton<GuiManager>.Instance.ResetFocus();
			}
		}
		if (pointer.up && m_interacting)
		{
			float num2 = m_lastInputPos.x - m_initialInputPos.x;
			int page = m_page;
			if (num2 < (0f - (float)Screen.width) / 16f)
			{
				m_page++;
				m_page = Mathf.Clamp(m_page, 0, m_pageCount - 1);
				for (int i = 0; i < m_dotsList.Count; i++)
				{
					if (i == m_page)
					{
						m_dotsList[i].Enable();
					}
					else
					{
						m_dotsList[i].Disable();
					}
				}
			}
			else if (num2 > (float)(Screen.width / 16))
			{
				m_page--;
				m_page = Mathf.Clamp(m_page, 0, m_pageCount - 1);
				for (int j = 0; j < m_dotsList.Count; j++)
				{
					if (j == m_page)
					{
						m_dotsList[j].Enable();
					}
					else
					{
						m_dotsList[j].Disable();
					}
				}
			}
			m_page = Mathf.Clamp(m_page, 0, m_pageCount - 1);
			if (page != m_page)
			{
				OnPageChanged();
			}
			m_interacting = false;
		}
		if (m_startingCutsceneButton.gameObject.activeInHierarchy)
		{
			float num3 = 4f;
			float num4 = (float)Screen.width / (float)Screen.height;
			if (num4 < 1.45f)
			{
				num3 /= 1.45f / num4;
			}
			Vector3 position = m_buttonGrid.transform.GetChild(0).position - Vector3.right * num3;
			m_startingCutsceneButton.position = position;
		}
		if ((bool)m_midCutsceneButton && m_midCutsceneButton.gameObject.activeInHierarchy && m_midCutsceneButtonPage * 15 < m_buttonGrid.transform.childCount)
		{
			float num5 = 4f;
			float num6 = (float)Screen.width / (float)Screen.height;
			if (num6 < 1.45f)
			{
				num5 /= 1.45f / num6;
			}
			int num7 = m_midCutsceneButtonPage * 15;
			Mathf.Clamp(num7, 0, m_buttonGrid.transform.childCount - 1);
			Vector3 position2 = m_buttonGrid.transform.GetChild(num7).position - Vector3.right * num5;
			m_midCutsceneButton.position = position2;
		}
		if (m_endingCutsceneButton.gameObject.activeInHierarchy)
		{
			float num8 = 4f;
			float num9 = (float)Screen.width / (float)Screen.height;
			if (num9 < 1.45f)
			{
				num8 /= 1.45f / num9;
			}
			m_endingCutsceneButton.position = m_buttonGrid.transform.GetChild(m_buttonGrid.transform.childCount - 1).position + Vector3.right * num8;
		}
	}

	private bool isInInteractiveArea(Vector2 touchPos)
	{
		if (touchPos.y > (float)Screen.height * 0.1f)
		{
			return touchPos.y < (float)Screen.height * 0.8f;
		}
		return false;
	}

	private void ShowStarEffect()
	{
		StartCoroutine(PlaySoundEffect());
		GameObject obj = UnityEngine.Object.Instantiate(m_starEffect);
		obj.transform.position = new Vector3(0f, 0f, -90f);
		obj.GetComponent<ParticleSystem>().Play();
	}

	private IEnumerator PlaySoundEffect()
	{
		yield return null;
		Singleton<AudioManager>.Instance.Play2dEffect(Singleton<GameManager>.Instance.gameData.commonAudioCollection.sandboxLevelUnlocked);
	}

	private void UnlockLevel(Button button, int index, bool isJoker)
	{
		button.transform.Find("LevelNumber").GetComponent<TextMesh>().text = DifferentiatedLevelLabel(index);
		button.transform.Find("Lock").gameObject.SetActive(value: false);
		button.MethodToCall.SetMethod(base.gameObject.GetComponent<LevelSelector>(), (!isJoker) ? "LoadLevel" : "LoadStarLevel", index.ToString());
		ButtonAnimation component = button.GetComponent<ButtonAnimation>();
		if (component != null)
		{
			component.RemoveInputListener();
		}
		string sceneName = Levels[index].sceneName;
		int @int = GameProgress.GetInt(sceneName + "_stars");
		bool flag = GameProgress.HasCollectedSnoutCoins(sceneName, 0);
		bool flag2 = GameProgress.HasCollectedSnoutCoins(sceneName, 1);
		bool flag3 = GameProgress.HasCollectedSnoutCoins(sceneName, 2);
		GameObject[] array = new GameObject[6]
		{
			button.transform.Find("StarSet/Star1").gameObject,
			button.transform.Find("StarSet/Star2").gameObject,
			button.transform.Find("StarSet/Star3").gameObject,
			button.transform.Find("CoinSet/Star1").gameObject,
			button.transform.Find("CoinSet/Star2").gameObject,
			button.transform.Find("CoinSet/Star3").gameObject
		};
		int num = 0;
		if (flag)
		{
			num++;
		}
		if (flag2)
		{
			num++;
		}
		if (flag3)
		{
			num++;
		}
		for (int i = 0; i < 3; i++)
		{
			bool flag4 = i + 1 <= @int;
			bool flag5 = i + 1 <= num || Singleton<BuildCustomizationLoader>.Instance.IsOdyssey;
			array[i].SetActive(flag4 && !flag5);
			array[i + 3].SetActive(flag4 && flag5);
		}
		if (isJoker)
		{
			button.transform.Find("StarSetsLocked").gameObject.SetActive(value: false);
		}
	}

	private void AddUnlockPanel(Button button, int row, int page, int episodeIndex, bool pulse)
	{
		if (!WPFMonoBehaviour.gameData.m_levelRowUnlockPanel)
		{
			return;
		}
		GameObject obj = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_levelRowUnlockPanel);
		UnlockLevelRowPanel component = obj.GetComponent<UnlockLevelRowPanel>();
		obj.transform.parent = button.transform;
		float num = Mathf.Abs((WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(Vector3.zero) - WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3(ButtonXGap, 0f, 0f))).x);
		float x = button.GetComponent<BoxCollider>().size.x;
		float num2 = 16.666666f + x;
		float num3 = num * 4f + x;
		component.BackgroundScale = new Vector2(num3 / num2, 1f);
		float x2 = 2f * num - component.RealSize.x / component.PanelSize.x * (component.PanelSize.x - 25f) / 2f;
		obj.transform.localPosition = new Vector3(x2, 0f, 1f);
		string[] levels = new string[5]
		{
			LevelInfo.GetLevelNames(episodeIndex)[row * 5],
			LevelInfo.GetLevelNames(episodeIndex)[row * 5 + 1],
			LevelInfo.GetLevelNames(episodeIndex)[row * 5 + 2],
			LevelInfo.GetLevelNames(episodeIndex)[row * 5 + 3],
			LevelInfo.GetLevelNames(episodeIndex)[row * 5 + 4]
		};
		component.UnlockDialog.OnAdFinishedSuccesfully = delegate
		{
			GameProgress.SetShowRowUnlockStarEffect(episodeIndex, row, show: true);
			Singleton<GameManager>.Instance.ReloadCurrentLevel(showLoadingScreen: true);
		};
		if (pulse)
		{
			component.PulseButton();
		}
		int cost = Singleton<VirtualCatalogManager>.Instance.GetProductPrice($"level_row_{row}_unlock");
		if (cost <= 0)
		{
			cost = 20 + row * 5;
		}
		component.Page = page;
		component.SetCost(cost);
		GameObject gameObject = component.UnlockDialog.transform.Find("PayUnlockBtn").gameObject;
		if (gameObject != null)
		{
			gameObject.GetComponent<Button>().enabled = GameProgress.SnoutCoinCount() >= cost;
			component.Pay += delegate
			{
				if (!GameProgress.IsLevelAdUnlocked(levels[0]) && GameProgress.UseSnoutCoins(cost))
				{
					GameProgress.SetShowRowUnlockStarEffect(episodeIndex, row, show: true);
					Singleton<GameManager>.Instance.ReloadCurrentLevel(showLoadingScreen: true);
					EventManager.Connect<LevelLoadedEvent>(DelayedPurchaseSound);
				}
			};
		}
		component.UnlockDialog.onOpen += delegate
		{
			m_isDialogOpen = true;
		};
		component.UnlockDialog.onClose += delegate
		{
			m_isDialogOpen = false;
		};
	}

	private void DelayedPurchaseSound(LevelLoadedEvent data)
	{
		EventManager.Disconnect<LevelLoadedEvent>(DelayedPurchaseSound);
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
	}

	private void AddLockedPanel(Button button)
	{
		if ((bool)WPFMonoBehaviour.gameData.m_lockedLevelRowPanel)
		{
			float num = Mathf.Abs((WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(Vector3.zero) - WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(new Vector3(ButtonXGap, 0f, 0f))).x);
			GameObject obj = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_lockedLevelRowPanel);
			obj.transform.parent = button.transform;
			obj.transform.localPosition = new Vector3(2f * num, 0f, 1f);
			float x = button.GetComponent<BoxCollider>().size.x;
			float num2 = 16.666666f + x;
			float num3 = num * 4f + x;
			obj.transform.localScale = new Vector3(num3 / num2, 1f, 1f);
		}
	}

	private void LockLevel(Button button, int index, bool isJoker, bool isContentLimited, bool isAdUnlocked)
	{
		button.transform.Find("LevelNumber").gameObject.SetActive(value: false);
		button.transform.Find("StarSet").gameObject.SetActive(value: false);
		button.gameObject.AddComponent<InactiveButton>();
		Transform transform = button.transform.Find("CoinSet");
		if (transform != null)
		{
			transform.gameObject.SetActive(value: false);
		}
		if (isContentLimited)
		{
			button.MethodToCall.SetMethod(base.gameObject.GetComponent<LevelSelector>(), "ShowContentLimitNotification");
		}
		if (isJoker)
		{
			UpdateJokerStars(button, index, isAdUnlocked);
		}
	}

	private void UpdateJokerStars(Button jokerButton, int levelIndex, bool isAdUnlocked)
	{
		if (isAdUnlocked)
		{
			LevelInfo.GetStarLevelStars(EpisodeIndex, levelIndex, out var current, out var limit);
			jokerButton.transform.Find("StarSetsLocked/StarsCollected").GetComponent<TextMesh>().text = string.Empty + current + "/" + limit;
		}
		else
		{
			jokerButton.transform.Find("StarSet").gameObject.SetActive(value: false);
			jokerButton.transform.Find("StarSetsLocked").gameObject.SetActive(value: false);
		}
	}

	public void OpenUnlockFullVersionPurchasePage()
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IAPEnabled)
		{
			m_isIapOpen = true;
			Singleton<IapManager>.Instance.EnableUnlockFullVersionPurchasePage();
		}
	}

	public void ShowContentLimitNotification()
	{
		LevelInfo.DisplayContentLimitNotification();
	}

	private void HandleIapManageronPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			m_interacting = false;
		}
	}
}
