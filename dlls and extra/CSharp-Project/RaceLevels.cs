using System;
using System.Collections.Generic;
using UnityEngine;

public class RaceLevels : MonoBehaviour
{
	[Serializable]
	public class LevelData
	{
		public string m_identifier;

		public string m_leaderboardId;

		public string m_levelLoaderPath;

		public string m_levelLoaderGUID;

		private string m_sceneName;

		public string SceneName
		{
			get
			{
				if (string.IsNullOrEmpty(m_sceneName))
				{
					m_sceneName = m_levelLoaderPath.Substring(m_levelLoaderPath.LastIndexOf('/') + 1);
					m_sceneName = m_sceneName.Remove(m_sceneName.LastIndexOf('_'));
				}
				return m_sceneName;
			}
		}
	}

	[Serializable]
	public class LevelUnlockablePartsData
	{
		public string m_identifier;

		public string m_levelNumber;

		public List<UnlockableTier> m_tiers;
	}

	[Serializable]
	public class UnlockableTier
	{
		public int m_starLimit;

		public BasePart.PartType m_part;
	}

	[SerializeField]
	private string m_name;

	[SerializeField]
	private string m_label;

	[SerializeField]
	private string m_flurryID;

	[SerializeField]
	private List<LevelData> m_levels = new List<LevelData>();

	[SerializeField]
	private List<LevelUnlockablePartsData> m_unlockables = new List<LevelUnlockablePartsData>();

	public string Name => m_name;

	public string Label => m_label;

	public string FlurryID => m_flurryID;

	public List<LevelData> Levels => m_levels;

	public List<LevelUnlockablePartsData> LevelUnlockables => m_unlockables;

	public int GetLevelIndex(string identifier)
	{
		int result = -1;
		for (int i = 0; i < m_levels.Count; i++)
		{
			if (m_levels[i].m_identifier == identifier)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public LevelData GetLevelData(string identifier)
	{
		foreach (LevelData level in m_levels)
		{
			if (level.m_identifier == identifier)
			{
				return level;
			}
		}
		return null;
	}

	public LevelUnlockablePartsData GetLevelUnlockableData(string identifier)
	{
		foreach (LevelUnlockablePartsData unlockable in m_unlockables)
		{
			if (unlockable.m_identifier == identifier)
			{
				return unlockable;
			}
		}
		return null;
	}
}
