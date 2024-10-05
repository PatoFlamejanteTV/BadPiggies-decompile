using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxLevels : MonoBehaviour
{
	[Serializable]
	public class LevelData
	{
		public string m_identifier;

		public int m_starBoxCount;

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

	[SerializeField]
	private string m_name;

	[SerializeField]
	private string m_label;

	[SerializeField]
	private string m_flurryID;

	[SerializeField]
	private string m_clearAchievement;

	[SerializeField]
	private string m_3starAchievement;

	[SerializeField]
	private string m_special3StarAchievement;

	[SerializeField]
	private List<LevelData> m_levels = new List<LevelData>();

	public string Name => m_name;

	public string Label => m_label;

	public string FlurryID => m_flurryID;

	public string ClearAchievement => m_clearAchievement;

	public string ThreeStarAchievement => m_3starAchievement;

	public string SpecialThreeStarAchievement => m_special3StarAchievement;

	public List<LevelData> Levels => m_levels;

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
}
