using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxSelector : MonoBehaviour
{
	public static string SANDBOX_BUNDLE = "Episode_Sandbox_Levels";

	public static string SANDBOX_BUNDLE_2 = "Episode_Sandbox_Levels_2";

	public SandboxLevels m_sandboxLevels;

	public List<SandboxLevels.LevelData> Levels => m_sandboxLevels.Levels;

	public SandboxLevels.LevelData FindLevel(string identifier)
	{
		return m_sandboxLevels.GetLevelData(identifier);
	}

	public string FindLevelFile(string identifier)
	{
		SandboxLevels.LevelData levelData = m_sandboxLevels.GetLevelData(identifier);
		if (levelData != null)
		{
			return levelData.SceneName;
		}
		return "UndefinedSandboxFile";
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
		Singleton<GameManager>.Instance.OpenSandboxEpisode(this);
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey && !Singleton<BuildCustomizationLoader>.Instance.IAPEnabled && Singleton<BuildCustomizationLoader>.Instance.CustomerID != "nook")
		{
			UnityEngine.Object.Destroy(GameObject.Find("SandboxButtonSpecialIAP"));
		}
	}

	public void LoadSandboxLevel(string identifier)
	{
		Singleton<GameManager>.Instance.LoadSandboxLevel(identifier);
	}

	public void GoToEpisodeSelection()
	{
		Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: false);
	}

	public void OpenIAPPopup()
	{
		CompactEpisodeSelector episodeSelector = CompactEpisodeSelector.Instance;
		Action onClose = null;
		if (episodeSelector != null)
		{
			episodeSelector.gameObject.SetActive(value: false);
			onClose = delegate
			{
				episodeSelector.gameObject.SetActive(value: true);
			};
		}
		Singleton<IapManager>.Instance.OpenShopPage(onClose, "FieldOfDreams");
	}
}
