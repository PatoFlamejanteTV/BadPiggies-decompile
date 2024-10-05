using UnityEngine;
using UnityEngine.Rendering;

public class EpisodeButton : WPFMonoBehaviour
{
	public Color m_bgcolor;

	[SerializeField]
	private GameManager.EpisodeType m_type = GameManager.EpisodeType.Normal;

	[SerializeField]
	private int m_episodeLevelsGameDataIndex;

	[SerializeField]
	private GameObject m_newContent;

	[SerializeField]
	private GameObject m_contentLock;

	[SerializeField]
	private GameObject m_contentNotAvailable;

	[SerializeField]
	private bool pageTwoComingSoon;

	[SerializeField]
	private bool pageThreeComingSoon;

	[SerializeField]
	private GameObject[] m_hideOnContentNotAvailable;

	[SerializeField]
	private string m_episodeBundleName = string.Empty;

	private Material materialInstance;

	public int Index => m_episodeLevelsGameDataIndex;

	private void Awake()
	{
		if (string.IsNullOrEmpty(m_episodeBundleName))
		{
			switch (m_type)
			{
			case GameManager.EpisodeType.Sandbox:
				m_episodeBundleName = "Episode_Sandbox_Levels";
				break;
			case GameManager.EpisodeType.Race:
				m_episodeBundleName = "Episode_Race_Levels";
				break;
			case GameManager.EpisodeType.Normal:
				m_episodeBundleName = Bundle.GetAssetBundleID(m_episodeLevelsGameDataIndex);
				break;
			}
		}
		if ((bool)m_contentLock)
		{
			GameObject obj = Object.Instantiate(m_contentLock);
			obj.transform.parent = base.gameObject.transform;
			obj.transform.localPosition = new Vector3(0f, -0.5f, 0f);
			obj.GetComponent<ContentLock>().Activate();
		}
		if (!Bundle.HasBundle(m_episodeBundleName) && (bool)m_contentNotAvailable)
		{
			Button component = GetComponent<Button>();
			if (component != null)
			{
				Object.DestroyImmediate(component);
			}
			GameObject obj2 = Object.Instantiate(m_contentNotAvailable);
			obj2.transform.parent = base.gameObject.transform;
			obj2.transform.localPosition = new Vector3(0f, -0.5f, 0f);
			if (m_hideOnContentNotAvailable != null)
			{
				for (int i = 0; i < m_hideOnContentNotAvailable.Length; i++)
				{
					if (m_hideOnContentNotAvailable[i] != null)
					{
						m_hideOnContentNotAvailable[i].SetActive(value: false);
					}
				}
			}
		}
		if ((bool)m_newContent && Bundle.HasBundle(m_episodeBundleName) && GameProgress.GetBool(m_episodeBundleName + "_new_content", defaultValue: true))
		{
			Transform transform = base.transform.Find("NewTag");
			if ((bool)transform)
			{
				m_newContent = Object.Instantiate(m_newContent);
				m_newContent.transform.parent = transform;
				m_newContent.transform.localPosition = Vector3.zero;
			}
			else
			{
				m_newContent = null;
			}
		}
		Transform transform2 = base.transform.Find("Background");
		if (transform2 != null)
		{
			materialInstance = transform2.GetComponent<Renderer>().material;
			materialInstance.color = m_bgcolor;
			AtlasMaterials.Instance.AddMaterialInstance(materialInstance);
		}
		int num = 0;
		int num2 = 0;
		if (m_type == GameManager.EpisodeType.Sandbox)
		{
			num = CalculateSandboxStars();
			foreach (SandboxLevels.LevelData level in WPFMonoBehaviour.gameData.m_sandboxLevels.Levels)
			{
				num2 += level.m_starBoxCount;
			}
		}
		else if (m_type == GameManager.EpisodeType.Race)
		{
			num = CalculateRaceLevelStars();
			num2 = 3 * WPFMonoBehaviour.gameData.m_raceLevels.Levels.Count;
		}
		else
		{
			num = CalculateEpisodeStars();
			num2 = 3 * WPFMonoBehaviour.gameData.m_episodeLevels[m_episodeLevelsGameDataIndex].LevelInfos.Count;
			if ((pageTwoComingSoon || pageThreeComingSoon) && num2 >= 90)
			{
				num2 -= 45 * ((!pageTwoComingSoon || !pageThreeComingSoon || num2 < 135) ? 1 : 2);
			}
		}
		Transform transform3 = base.transform.Find("StarText");
		if (transform3 != null)
		{
			transform3.GetComponent<TextMesh>().text = string.Empty + num + "/" + num2;
		}
		Transform transform4 = base.transform.Find("EpisodeAnimation");
		if (transform4 != null)
		{
			transform4.gameObject.AddComponent<SortingGroup>().sortingOrder = 1;
		}
	}

	private void OnDestroy()
	{
		if (AtlasMaterials.IsInstantiated)
		{
			AtlasMaterials.Instance.RemoveMaterialInstance(materialInstance);
		}
	}

	public void GoToLevelSelection(string levelSelectionPage)
	{
		if ((bool)m_newContent)
		{
			GameProgress.SetBool(m_episodeBundleName + "_new_content", value: false);
		}
		UserSettings.SetString(CompactEpisodeSelector.CurrentEpisodeKey, base.gameObject.name);
		Singleton<GameManager>.Instance.LoadLevelSelection(levelSelectionPage, showLoadingScreen: false);
	}

	private int CalculateEpisodeStars()
	{
		int num = 0;
		foreach (EpisodeLevelInfo levelInfo in WPFMonoBehaviour.gameData.m_episodeLevels[m_episodeLevelsGameDataIndex].LevelInfos)
		{
			num += GameProgress.GetInt(levelInfo.sceneName + "_stars");
		}
		return num;
	}

	private int CalculateSandboxStars()
	{
		int num = 0;
		foreach (SandboxLevels.LevelData level in WPFMonoBehaviour.gameData.m_sandboxLevels.Levels)
		{
			num += GameProgress.GetInt(level.SceneName + "_stars");
		}
		return num;
	}

	private int CalculateRaceLevelStars()
	{
		int num = 0;
		foreach (RaceLevels.LevelData level in WPFMonoBehaviour.gameData.m_raceLevels.Levels)
		{
			num += GameProgress.GetInt(level.SceneName + "_stars");
		}
		return num;
	}
}
