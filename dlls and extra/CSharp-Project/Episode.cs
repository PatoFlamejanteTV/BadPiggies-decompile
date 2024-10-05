using System.Collections.Generic;
using UnityEngine;

public class Episode : MonoBehaviour
{
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
	private bool m_overrideInFlightMusic;

	[SerializeField]
	private BundleDataObject inFlightMusic;

	[SerializeField]
	private bool m_overrideBuildingMusic;

	[SerializeField]
	private BundleDataObject buildingMusic;

	[SerializeField]
	private int totalLevelCount;

	public bool m_showStarLevelTransition = true;

	[HideInInspector]
	public List<EpisodeLevelInfo> m_levelInfos = new List<EpisodeLevelInfo>();

	[SerializeField]
	private List<int> m_starLimits = new List<int>();

	public string Name => m_name;

	public string Label => m_label;

	public string FlurryID => m_flurryID;

	public string ClearAchievement => m_clearAchievement;

	public string ThreeStarAchievement => m_3starAchievement;

	public string SpecialThreeStarAchievement => m_special3StarAchievement;

	public bool OverrideInFlightMusic => m_overrideInFlightMusic;

	public bool OverrideBuildMusic => m_overrideBuildingMusic;

	public GameObject InFlightMusic => inFlightMusic.LoadValue<GameObject>();

	public GameObject BuildingMusic => buildingMusic.LoadValue<GameObject>();

	public List<EpisodeLevelInfo> LevelInfos => m_levelInfos;

	public List<int> StarLevelLimits => m_starLimits;

	public int TotalLevelCount => totalLevelCount;

	private void Awake()
	{
	}
}
