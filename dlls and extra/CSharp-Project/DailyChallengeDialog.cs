using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DailyChallengeDialog : WPFMonoBehaviour
{
	[SerializeField]
	private AnimationCurve ribbonCurve;

	[SerializeField]
	private float ribbonAnimTime;

	[SerializeField]
	private GameObject ribbon;

	private int prevHour = -1;

	private int prevMinutes = -1;

	private string timeLeftlocalizationKey;

	private static bool s_dialogOpen;

	private RefreshLocalizer localizer;

	private Dialog.OnClose OnClose;

	private static DailyChallengeDialog instance;

	private Action OnSceneWasLoaded;

	private Action OnGameLevelLoaded;

	private static readonly string[] LevelSelectionPages = new string[6] { "Episode1LevelSelection", "Episode3LevelSelection", "Episode4LevelSelection", "Episode2LevelSelection", "Episode5LevelSelection", "Episode6LevelSelection" };

	private static Vector3 DefaultPosition
	{
		get
		{
			if ((bool)WPFMonoBehaviour.hudCamera)
			{
				return WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 10f;
			}
			return Vector3.zero;
		}
	}

	public static bool DialogOpen => s_dialogOpen;

	private void Awake()
	{
		instance = this;
		localizer = new RefreshLocalizer(base.transform.Find("TimeLeft/Text").GetComponent<TextMesh>());
		localizer.Update = () => $"{Singleton<DailyChallenge>.Instance.DailyChallengeTimeLeft.Hours}h {Singleton<DailyChallenge>.Instance.DailyChallengeTimeLeft.Minutes}m";
		UnityEngine.Object.DontDestroyOnLoad(this);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnEnable()
	{
		Singleton<GuiManager>.Instance.GrabPointer(this);
		Singleton<KeyListener>.Instance.GrabFocus(this);
		KeyListener.keyReleased += HandleKeyRelease;
		EventManager.Send(new UIEvent(UIEvent.Type.OpenedDailyChallengeDialog));
		StartCoroutine(UpdateTimeLeft());
		s_dialogOpen = true;
	}

	private void OnDisable()
	{
		if (Singleton<GuiManager>.IsInstantiated())
		{
			Singleton<GuiManager>.Instance.ReleasePointer(this);
		}
		if (Singleton<KeyListener>.IsInstantiated())
		{
			Singleton<KeyListener>.Instance.ReleaseFocus(this);
		}
		EventManager.Send(new UIEvent(UIEvent.Type.ClosedDailyChallengeDialog));
		KeyListener.keyReleased -= HandleKeyRelease;
		s_dialogOpen = false;
	}

	private void HandleKeyRelease(KeyCode key)
	{
		if (key == KeyCode.Escape)
		{
			Close();
		}
	}

	private void OnDestroy()
	{
		localizer.Dispose();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public static DailyChallengeDialog Create()
	{
		if ((bool)instance)
		{
			return instance;
		}
		UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_dailyChallengeDialog).transform.position = DefaultPosition;
		return instance;
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
	}

	public void Open(Dialog.OnClose OnClose)
	{
		this.OnClose = (Dialog.OnClose)Delegate.Combine(this.OnClose, OnClose);
		base.gameObject.SetActive(value: true);
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
		if (OnClose != null)
		{
			OnClose();
			OnClose = null;
		}
	}

	public void OpenChallengeLevel(int index)
	{
		if (!Singleton<DailyChallenge>.Instance.Initialized || index < 0 || index >= Singleton<DailyChallenge>.Instance.Count)
		{
			return;
		}
		int levelIndex = Singleton<DailyChallenge>.Instance.Challenges[index].levelIndex;
		int episodeIndex = Singleton<DailyChallenge>.Instance.Challenges[index].episodeIndex;
		if (Singleton<GameManager>.Instance.IsInGame() && levelIndex == Singleton<GameManager>.Instance.CurrentLevel && episodeIndex == Singleton<GameManager>.Instance.CurrentEpisodeIndex)
		{
			Close();
			WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Continue);
			return;
		}
		int page = ((levelIndex <= 14) ? 1 : ((levelIndex <= 29) ? 2 : 3));
		if (LevelInfo.IsLevelUnlocked(episodeIndex, levelIndex))
		{
			BackgroundMask.SetParent(base.transform);
			Singleton<GameManager>.Instance.LoadLevelSelectionAndLevel(LevelSelectionPages[episodeIndex], levelIndex);
			OnGameLevelLoaded = (Action)Delegate.Combine(OnGameLevelLoaded, (Action)delegate
			{
				BackgroundMask.SetParent(null);
				base.gameObject.SetActive(value: false);
			});
			return;
		}
		Singleton<GameManager>.Instance.LoadLevelSelection(LevelSelectionPages[episodeIndex], showLoadingScreen: true);
		OnSceneWasLoaded = (Action)Delegate.Combine(OnSceneWasLoaded, (Action)delegate
		{
			base.gameObject.SetActive(value: false);
			if ((bool)WPFMonoBehaviour.levelSelector)
			{
				if (WPFMonoBehaviour.levelSelector.CurrentPage > page)
				{
					for (int i = 0; i < WPFMonoBehaviour.levelSelector.CurrentPage - page; i++)
					{
						WPFMonoBehaviour.levelSelector.PreviousPage();
					}
				}
				else if (WPFMonoBehaviour.levelSelector.CurrentPage < page)
				{
					for (int j = 0; j < page - WPFMonoBehaviour.levelSelector.CurrentPage; j++)
					{
						WPFMonoBehaviour.levelSelector.NextPage();
					}
				}
			}
		});
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		base.transform.position = DefaultPosition;
		if (OnSceneWasLoaded != null)
		{
			OnSceneWasLoaded();
			OnSceneWasLoaded = null;
		}
		if (Singleton<GameManager>.Instance.IsInGame() && OnGameLevelLoaded != null)
		{
			OnGameLevelLoaded();
			OnGameLevelLoaded = null;
		}
	}

	public void ChangeLootCrate()
	{
		switch (Singleton<DailyChallenge>.Instance.TodaysLootCrate(0))
		{
		case LootCrateType.Gold:
			Singleton<DailyChallenge>.Instance.SetDailyLootCrate(LootCrateType.Wood);
			break;
		case LootCrateType.Metal:
			Singleton<DailyChallenge>.Instance.SetDailyLootCrate(LootCrateType.Gold);
			break;
		case LootCrateType.Wood:
			Singleton<DailyChallenge>.Instance.SetDailyLootCrate(LootCrateType.Metal);
			break;
		}
	}

	public void ChangeDays()
	{
		for (int i = 0; i < Singleton<DailyChallenge>.Instance.Count; i++)
		{
			ChangeDay(i);
		}
	}

	public void ChangeDay(int index)
	{
		if (index >= 0 && index < Singleton<DailyChallenge>.Instance.Count)
		{
			int levelIndex = Singleton<DailyChallenge>.Instance.Challenges[index].levelIndex;
			int episodeIndex = Singleton<DailyChallenge>.Instance.Challenges[index].episodeIndex;
			if (++levelIndex >= LevelInfo.GetLevelNames(episodeIndex).Count)
			{
				levelIndex = 0;
				episodeIndex = ((episodeIndex < 5) ? (episodeIndex + 1) : 0);
				Singleton<DailyChallenge>.Instance.SetDailyChallenge(index, episodeIndex, levelIndex);
			}
			else
			{
				Singleton<DailyChallenge>.Instance.SetDailyChallenge(index, episodeIndex, levelIndex);
			}
		}
	}

	private IEnumerator UpdateTimeLeft()
	{
		localizer.Target.gameObject.SetActive(value: false);
		while ((!Singleton<DailyChallenge>.IsInstantiated() || !Singleton<DailyChallenge>.Instance.Initialized) && base.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		localizer.Target.gameObject.SetActive(value: true);
		while (Singleton<DailyChallenge>.Instance.HasChallenge && base.gameObject.activeInHierarchy)
		{
			TimeSpan dailyChallengeTimeLeft = Singleton<DailyChallenge>.Instance.DailyChallengeTimeLeft;
			if (NeedsUpdate(dailyChallengeTimeLeft))
			{
				localizer.Refresh();
			}
			yield return null;
		}
	}

	private bool NeedsUpdate(TimeSpan time)
	{
		bool result = time.Hours != prevHour || time.Minutes != prevMinutes;
		prevHour = time.Hours;
		prevMinutes = time.Minutes;
		return result;
	}

	public void OpenLootCrateShop()
	{
		if (!Singleton<BuildCustomizationLoader>.Instance.IAPEnabled)
		{
			return;
		}
		if (WPFMonoBehaviour.levelManager == null && CompactEpisodeSelector.Instance != null)
		{
			CompactEpisodeSelector.Instance.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: false);
			Singleton<IapManager>.Instance.GetShop().Open(delegate
			{
				CompactEpisodeSelector.Instance.gameObject.SetActive(value: true);
				base.gameObject.SetActive(value: true);
			}, "LootCrates");
		}
		else
		{
			base.gameObject.SetActive(value: false);
			Singleton<IapManager>.Instance.GetShop().Open(delegate
			{
				base.gameObject.SetActive(value: true);
			}, "LootCrates");
		}
	}
}
