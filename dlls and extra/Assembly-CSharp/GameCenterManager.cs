using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameCenterManager : MonoBehaviour, ISocialProvider
{
	public struct AchievementQueueBlock
	{
		public string id;

		public double progress;
	}

	public struct AchievementDataStruct
	{
		public string title;

		public string description;

		public double percentComplete;

		public bool completed;

		public bool hidden;
	}

	public struct LeaderboardDataStruct
	{
		public string title;

		public int rank;

		public long score;
	}

	[SerializeField]
	private AchievementPopup m_achievementPopup;

	private Dictionary<string, AchievementDataStruct> m_achievementList = new Dictionary<string, AchievementDataStruct>();

	private Dictionary<string, LeaderboardDataStruct> m_leaderboardList = new Dictionary<string, LeaderboardDataStruct>();

	private List<AchievementQueueBlock> m_achievementsQueue = new List<AchievementQueueBlock>();

	private bool m_achievementsQueueSemaphore;

	private bool m_started;

	private string m_currentlySyncing;

	public List<string> m_leaderboardIDs = new List<string>();

	public Dictionary<string, AchievementDataStruct> Achievements => m_achievementList;

	public bool Authenticated => Social.localUser.authenticated;

	public static event Action<bool> onAuthenticationSucceeded;

	private void Awake()
	{
		SocialGameManager.RegisterProvider(this);
	}

	private void Start()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_achievementPopup.gameObject);
		m_achievementPopup = gameObject.GetComponent<AchievementPopup>();
	}

	private void OnEnable()
	{
		EventManager.Connect<LevelLoadedEvent>(ReceiveLevelLoadedEvent);
	}

	private void OnDisable()
	{
		EventManager.Disconnect<LevelLoadedEvent>(ReceiveLevelLoadedEvent);
	}

	private void ReceiveLevelLoadedEvent(LevelLoadedEvent data)
	{
		if (!m_started && data.currentGameState == GameManager.GameState.MainMenu)
		{
			m_started = true;
			StartCoroutine(WaitForInterstitialToClose());
		}
	}

	private IEnumerator WaitForInterstitialToClose()
	{
		Authenticate();
		StartCoroutine(SyncAchievementData());
		yield break;
	}

	public void Authenticate()
	{
		Social.localUser.Authenticate(AuthenticationSucceeded);
	}

	public void ShowAchievementsView()
	{
		if (Authenticated && !Application.isEditor)
		{
			Social.ShowAchievementsUI();
		}
		else
		{
			Authenticate();
		}
	}

	public void ShowLeaderboardsView()
	{
		if (Authenticated && !Application.isEditor)
		{
			Social.ShowLeaderboardUI();
		}
		else
		{
			Authenticate();
		}
	}

	public void LoadAchievements()
	{
		if (Authenticated && !Application.isEditor)
		{
			Social.LoadAchievements(AchievementsDidLoad);
		}
	}

	public void LoadLeaderboardScores()
	{
		if (!Authenticated || Application.isEditor)
		{
			return;
		}
		foreach (string leaderboardID in m_leaderboardIDs)
		{
			ILeaderboard lb = Social.CreateLeaderboard();
			lb.id = leaderboardID;
			lb.LoadScores(delegate
			{
				LeaderboardDataStruct value = default(LeaderboardDataStruct);
				value.title = lb.title;
				value.rank = lb.localUserScore.rank;
				long.TryParse(lb.localUserScore.formattedValue, out value.score);
				m_leaderboardList.Add(lb.id, value);
			});
		}
	}

	public void LoadScoreForLeaderboard(string leaderboardId)
	{
	}

	public void ReportAchievementProgress(string achievementId, double progress)
	{
		if (Authenticated && !Application.isEditor)
		{
			AchievementQueueBlock item = default(AchievementQueueBlock);
			item.id = achievementId;
			item.progress = progress;
			AchievementData.AchievementDataHolder achievement = Singleton<AchievementData>.Instance.GetAchievement(achievementId);
			achievement.progress = ((progress <= achievement.progress) ? achievement.progress : progress);
			achievement.completed = progress >= 100.0;
			Singleton<AchievementData>.Instance.SetAchievement(achievementId, achievement);
			m_achievementsQueue.Add(item);
		}
	}

	public void ReportLeaderboardScore(string leaderboardId, long score, Action<bool> handler)
	{
		if (Authenticated && !Application.isEditor)
		{
			Social.ReportScore(score, leaderboardId, handler);
		}
	}

	public void ResetAchievementData()
	{
	}

	public void SyncAllAchievementsNow()
	{
		foreach (KeyValuePair<string, double> item in Singleton<AchievementData>.Instance.AchievementsProgress)
		{
			if (item.Value > 0.0)
			{
				ReportAchievementProgress(item.Key, item.Value);
			}
		}
	}

	public bool ViewsActive()
	{
		return false;
	}

	public void CloseViews()
	{
	}

	private void AchievementsDidLoad(IAchievement[] achievementsList)
	{
		foreach (IAchievement achievement in achievementsList)
		{
			AchievementDataStruct value = default(AchievementDataStruct);
			value.percentComplete = achievement.percentCompleted;
			value.completed = achievement.completed;
			value.hidden = achievement.hidden;
			m_achievementList.Add(achievement.id, value);
		}
		Social.LoadAchievementDescriptions(AchievementDescriptionsDidLoad);
	}

	private void AchievementDescriptionsDidLoad(IAchievementDescription[] achievementsList)
	{
		foreach (IAchievementDescription achievementDescription in achievementsList)
		{
			AchievementDataStruct achievementDataStruct = Achievements[achievementDescription.id];
			achievementDataStruct.title = achievementDescription.title;
			achievementDataStruct.description = achievementDescription.achievedDescription;
		}
	}

	private void AchievementReportDidComplete(bool success)
	{
		if (m_achievementsQueue.Count <= 0 || !m_achievementsQueue[0].id.Equals(m_currentlySyncing))
		{
			return;
		}
		if (success)
		{
			AchievementData.AchievementDataHolder achievement = Singleton<AchievementData>.Instance.GetAchievement(m_achievementsQueue[0].id);
			if (m_achievementsQueue[0].progress >= 100.0 && !achievement.synced)
			{
				m_achievementPopup.Show(m_achievementsQueue[0].id);
				achievement.synced = true;
				Singleton<AchievementData>.Instance.SetAchievement(m_achievementsQueue[0].id, achievement);
			}
		}
		m_currentlySyncing = null;
		m_achievementsQueueSemaphore = false;
		m_achievementsQueue.RemoveAt(0);
	}

	private void AuthenticationSucceeded(bool success)
	{
		if (success)
		{
			SyncAllAchievementsNow();
		}
		if (GameCenterManager.onAuthenticationSucceeded != null)
		{
			GameCenterManager.onAuthenticationSucceeded(success);
		}
	}

	private IEnumerator SyncAchievementData()
	{
		while (true)
		{
			if (m_achievementsQueue.Count > 0 && !m_achievementsQueueSemaphore && string.IsNullOrEmpty(m_currentlySyncing))
			{
				m_currentlySyncing = m_achievementsQueue[0].id;
				m_achievementsQueueSemaphore = true;
				Social.ReportProgress(m_achievementsQueue[0].id, m_achievementsQueue[0].progress, AchievementReportDidComplete);
			}
			else
			{
				yield return new WaitForSeconds(3f);
			}
		}
	}
}
