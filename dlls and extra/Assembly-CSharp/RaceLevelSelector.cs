using System.Collections.Generic;
using UnityEngine;

public class RaceLevelSelector : WPFMonoBehaviour
{
	private const string RACELEVEL_BUNDLE = "Episode_Race_Levels";

	public RaceLevels m_raceLevels;

	public GameObject m_partialPageContentLimitedOverlay;

	public List<RaceLevels.LevelData> Levels => m_raceLevels.Levels;

	public RaceLevels.LevelData FindLevel(string identifier)
	{
		return m_raceLevels.GetLevelData(identifier);
	}

	public string FindLevelFile(string identifier)
	{
		RaceLevels.LevelData levelData = m_raceLevels.GetLevelData(identifier);
		if (levelData != null)
		{
			return levelData.SceneName;
		}
		return "UndefinedRaceLevelFile";
	}

	private void OnEnable()
	{
		KeyListener.keyPressed += HandleKeyListenerkeyPressed;
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		GameCenterManager.onAuthenticationSucceeded += ShowLeaderboardButton;
		IapManager.onPurchaseSucceeded += HandleIapManagerOnPurchaseSucceeded;
	}

	private void OnDisable()
	{
		KeyListener.keyPressed -= HandleKeyListenerkeyPressed;
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
		GameCenterManager.onAuthenticationSucceeded -= ShowLeaderboardButton;
		IapManager.onPurchaseSucceeded -= HandleIapManagerOnPurchaseSucceeded;
	}

	private void Awake()
	{
		if (GameTime.IsPaused())
		{
			GameTime.Pause(pause: false);
		}
	}

	private void Start()
	{
		Singleton<GameManager>.Instance.OpenRaceEpisode(this);
		ShowLeaderboardButton(show: false);
	}

	public void LoadRaceLevel(string identifier)
	{
		Singleton<GameManager>.Instance.LoadRaceLevel(identifier);
	}

	public void GoToEpisodeSelection()
	{
		Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: false);
	}

	public void ShowLeaderboardButton(bool show)
	{
		if (Singleton<SocialGameManager>.IsInstantiated())
		{
			Transform transform = base.transform.Find("LeaderboardButton");
			if (transform != null && (bool)transform.gameObject)
			{
				transform.gameObject.SetActive(show || Singleton<SocialGameManager>.Instance.Authenticated);
			}
		}
	}

	public void OpenLeaderboard()
	{
		if (Singleton<SocialGameManager>.IsInstantiated())
		{
			Singleton<SocialGameManager>.Instance.ShowLeaderboardsView();
		}
	}

	public void ReceiveUIEvent(UIEvent data)
	{
		if (data.type == UIEvent.Type.OpenUnlockFullVersionIapMenu)
		{
			Singleton<IapManager>.Instance.EnableUnlockFullVersionPurchasePage();
		}
	}

	private void HandleKeyListenerkeyPressed(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			GoToEpisodeSelection();
		}
	}

	private void HandleIapManagerOnPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
	}
}
