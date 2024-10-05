using System.Collections.Generic;

public class LevelInfo
{
	private const int LevelsPerRow = 5;

	private const string popupName = "ContentLimitNotification";

	public static void DisplayContentLimitNotification()
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			LevelsUnlockDialog.Create().SetActive(value: true);
		}
		else
		{
			EventManager.Send(new UIEvent(UIEvent.Type.OpenUnlockFullVersionIapMenu));
		}
	}

	public static bool IsContentLimited(int episodeIndex, int levelIndex = 0)
	{
		bool flag = GameProgress.GetFullVersionUnlocked() || (episodeIndex == 0 && levelIndex < 15) || (episodeIndex == -1 && levelIndex < 4) || (episodeIndex > 0 && levelIndex < 5);
		if (Singleton<BuildCustomizationLoader>.Instance.IsChina && episodeIndex >= 0)
		{
			if (!IsStarLevel(episodeIndex, levelIndex) && levelIndex + 1 - (levelIndex + 1) / 5 + 1 == GameProgress.GetMinimumLockedLevel(episodeIndex) && GetStars(episodeIndex, levelIndex) == 3)
			{
				GameProgress.SetMinimumLockedLevel(episodeIndex, GameProgress.GetMinimumLockedLevel(episodeIndex) + 1);
			}
			flag = episodeIndex >= 0 && levelIndex + 1 - (levelIndex + 1) / 5 < GameProgress.GetMinimumLockedLevel(episodeIndex);
			if (IsStarLevel(episodeIndex, levelIndex))
			{
				flag = true;
			}
		}
		return !flag;
	}

	public static bool IsNormalEpisode(int episodeIndex)
	{
		if (episodeIndex >= 0)
		{
			return episodeIndex < Singleton<GameManager>.Instance.gameData.m_episodeLevels.Count;
		}
		return false;
	}

	public static bool ValidLevelIndex(int episodeIndex, int levelIndex)
	{
		List<string> levelNames = GetLevelNames(episodeIndex);
		if (levelIndex >= 0)
		{
			return levelIndex < levelNames.Count;
		}
		return false;
	}

	public static bool IsStarLevel(int episodeIndex, int levelIndex)
	{
		return (levelIndex + 1) % 5 == 0;
	}

	public static void GetStarLevelStars(int episodeIndex, int levelIndex, out int current, out int limit)
	{
		List<string> levelNames = GetLevelNames(episodeIndex);
		current = 0;
		for (int i = 1; i < 5; i++)
		{
			current += GameProgress.GetInt(levelNames[levelIndex - i] + "_stars");
		}
		limit = GetStarLevelLimit(episodeIndex, levelIndex);
	}

	public static int GetStarLevelLimit(int episodeIndex, int levelIndex)
	{
		int index = levelIndex / 5;
		return GetEpisodeData(episodeIndex).StarLevelLimits[index];
	}

	public static int GetStars(int episodeIndex, int levelIndex)
	{
		return GameProgress.GetInt(GetLevelNames(episodeIndex)[levelIndex] + "_stars");
	}

	public static bool IsLevelUnlocked(int episodeIndex, int levelIndex)
	{
		bool flag = IsStarLevel(episodeIndex, levelIndex);
		if (GameProgress.AllLevelsUnlocked() || levelIndex == 0)
		{
			return true;
		}
		if (Singleton<BuildCustomizationLoader>.Instance.CheatsEnabled && GameProgress.AllFreeLevelsUnlocked() && !IsContentLimited(episodeIndex, levelIndex))
		{
			return true;
		}
		int index = PreviousNormalLevelIndex(episodeIndex, levelIndex);
		bool result = GameProgress.IsLevelCompleted(GetLevelNames(episodeIndex)[index]);
		if (!flag && !Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			return result;
		}
		if (!flag && Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			return true;
		}
		GetStarLevelStars(episodeIndex, levelIndex, out var current, out var limit);
		return current >= limit;
	}

	public static bool IsLevelCompleted(int episodeIndex, int levelIndex)
	{
		return GameProgress.IsLevelCompleted(GetLevelNames(episodeIndex)[levelIndex]);
	}

	public static List<string> GetLevelNames(int episodeIndex)
	{
		List<string> list = new List<string>();
		List<EpisodeLevelInfo> levelInfos = GetEpisodeData(episodeIndex).LevelInfos;
		for (int i = 0; i < levelInfos.Count; i++)
		{
			list.Add(levelInfos[i].sceneName);
		}
		return list;
	}

	public static bool AllBasicContentPlayed()
	{
		for (int i = 0; i < Singleton<GameManager>.Instance.gameData.m_episodeLevels.Count; i++)
		{
			Episode episodeData = GetEpisodeData(i);
			for (int j = 0; j < episodeData.LevelInfos.Count; j++)
			{
				if ((!Singleton<BuildCustomizationLoader>.Instance.IsContentLimited || !IsContentLimited(i, j)) && !IsStarLevel(i, j) && !IsLevelCompleted(i, j))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool AllNonIapContentPlayed()
	{
		for (int i = 0; i < Singleton<GameManager>.Instance.gameData.m_episodeLevels.Count; i++)
		{
			Episode episodeData = GetEpisodeData(i);
			for (int j = 0; j < episodeData.LevelInfos.Count; j++)
			{
				if ((!Singleton<BuildCustomizationLoader>.Instance.IsContentLimited || !IsContentLimited(i, j)) && !IsLevelCompleted(i, j))
				{
					return false;
				}
			}
		}
		foreach (SandboxLevels.LevelData level in Singleton<GameManager>.Instance.gameData.m_sandboxLevels.Levels)
		{
			if (GameProgress.SandboxStarCount(level.SceneName) == 0 && level.m_identifier != "S-F")
			{
				return false;
			}
		}
		return true;
	}

	public static bool AllContentPlayed()
	{
		for (int i = 0; i < Singleton<GameManager>.Instance.gameData.m_episodeLevels.Count; i++)
		{
			Episode episodeData = GetEpisodeData(i);
			for (int j = 0; j < episodeData.LevelInfos.Count; j++)
			{
				if (!IsLevelCompleted(i, j))
				{
					return false;
				}
			}
		}
		foreach (SandboxLevels.LevelData level in Singleton<GameManager>.Instance.gameData.m_sandboxLevels.Levels)
		{
			if (GameProgress.SandboxStarCount(level.SceneName) == 0)
			{
				return false;
			}
		}
		return true;
	}

	public static bool CanAdUnlockNextLevel(LevelManager levelManager = null)
	{
		int episodeIndex = Singleton<GameManager>.Instance.CurrentEpisodeIndex;
		int levelIndex = Singleton<GameManager>.Instance.NextLevel();
		if (levelManager != null && levelManager.m_raceLevel)
		{
			episodeIndex = -1;
			string text = Singleton<GameManager>.Instance.NextRaceLevel();
			for (int i = 0; i < Singleton<GameManager>.Instance.gameData.m_raceLevels.Levels.Count; i++)
			{
				if (text.Equals(Singleton<GameManager>.Instance.gameData.m_raceLevels.Levels[i].m_identifier))
				{
					levelIndex = i;
					break;
				}
			}
		}
		return CanAdUnlock(episodeIndex, levelIndex);
	}

	public static bool CanAdUnlock(int episodeIndex, int levelIndex)
	{
		if (!Singleton<BuildCustomizationLoader>.Instance.IsContentLimited)
		{
			return false;
		}
		bool flag = false;
		List<string> list;
		if (episodeIndex == -1)
		{
			flag = true;
			list = new List<string>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < Singleton<GameManager>.Instance.gameData.m_raceLevels.Levels.Count; i++)
			{
				list.Add(Singleton<GameManager>.Instance.gameData.m_raceLevels.Levels[i].SceneName);
				list2.Add(Singleton<GameManager>.Instance.gameData.m_raceLevels.Levels[i].m_identifier);
			}
		}
		else
		{
			list = GetLevelNames(episodeIndex);
		}
		bool flag2 = true;
		if (!flag && IsStarLevel(episodeIndex, levelIndex))
		{
			GetStarLevelStars(episodeIndex, levelIndex, out var current, out var limit);
			if (!flag2 && current >= limit)
			{
				return IsContentLimited(episodeIndex, levelIndex);
			}
			return false;
		}
		int num = levelIndex - 1;
		if (!flag && num > 0 && IsStarLevel(episodeIndex, num))
		{
			num--;
		}
		if (num < 0)
		{
			return true;
		}
		if (GameProgress.IsLevelCompleted(list[num]) && levelIndex < list.Count && !GameProgress.IsLevelCompleted(list[levelIndex]) && IsContentLimited(episodeIndex, (!flag) ? levelIndex : (levelIndex + 1)) && !flag2)
		{
			return true;
		}
		int num2 = levelIndex / 5;
		int num3 = num2 - 1;
		if (!flag && !flag2 && num2 != 0)
		{
			if (!IsLevelUnlocked(episodeIndex, num3 * 5))
			{
				return !IsContentLimited(episodeIndex, num3 * 5);
			}
			return true;
		}
		return false;
	}

	public static int PreviousNormalLevelIndex(int episodeIndex, int levelIndex)
	{
		if (IsStarLevel(episodeIndex, levelIndex - 1))
		{
			return levelIndex - 2;
		}
		return levelIndex - 1;
	}

	private static Episode GetEpisodeData(int episodeIndex)
	{
		return Singleton<GameManager>.Instance.gameData.m_episodeLevels[episodeIndex];
	}
}
