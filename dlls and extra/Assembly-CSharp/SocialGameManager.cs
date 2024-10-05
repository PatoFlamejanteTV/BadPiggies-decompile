using System;
using System.Collections.Generic;

public class SocialGameManager : Singleton<SocialGameManager>
{
	private ISocialProvider m_provider;

	public bool Authenticated => m_provider.Authenticated;

	public static void RegisterProvider(ISocialProvider provider)
	{
		Singleton<SocialGameManager>.Instance.m_provider = provider;
	}

	public void Authenticate()
	{
		m_provider.Authenticate();
	}

	public void ShowAchievementsView()
	{
		m_provider.ShowAchievementsView();
	}

	public void ShowLeaderboardsView()
	{
		m_provider.ShowLeaderboardsView();
	}

	public void LoadAchievements()
	{
		m_provider.LoadAchievements();
	}

	public void LoadLeaderboardScores()
	{
		m_provider.LoadLeaderboardScores();
	}

	public void LoadScoreForLeaderboard(string leaderboardId)
	{
		m_provider.LoadScoreForLeaderboard(leaderboardId);
	}

	public bool TryReportAchievementProgress(string achievementId, double progress, Func<int, bool> condition)
	{
		try
		{
			int achievementLimit = Singleton<AchievementData>.Instance.GetAchievementLimit(achievementId);
			if (condition(achievementLimit))
			{
				ReportAchievementProgress(achievementId, progress);
				return true;
			}
		}
		catch (KeyNotFoundException)
		{
		}
		return false;
	}

	public void ReportAchievementProgress(string achievementId, double progress)
	{
		m_provider.ReportAchievementProgress(achievementId, progress);
	}

	public void ReportLeaderboardScore(string leaderboardId, long score, Action<bool> handler)
	{
		m_provider.ReportLeaderboardScore(leaderboardId, score, handler);
	}

	public void SyncAllAchievementsNow()
	{
		m_provider.SyncAllAchievementsNow();
	}

	public void ResetAchievementData()
	{
		m_provider.ResetAchievementData();
	}

	public bool ViewsActive()
	{
		return m_provider.ViewsActive();
	}

	public void CloseViews()
	{
		m_provider.CloseViews();
	}

	private void Awake()
	{
		SetAsPersistant();
	}
}
