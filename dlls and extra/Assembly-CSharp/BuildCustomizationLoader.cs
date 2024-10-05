using Pathfinding.Serialization.JsonFx;
using UnityEngine;

public class BuildCustomizationLoader : Singleton<BuildCustomizationLoader>
{
	public GameObject m_gameBuildParamLoader;

	[SerializeField]
	private BuildConfiguration m_configuration;

	public bool IAPEnabled => m_configuration.EnableIAP;

	public string SkynestBackend => m_configuration.SkynestBackend;

	public bool IsChina => m_configuration.BuildTypeChina;

	public string CustomerID => m_configuration.CustomerId;

	public string SVNRevisionNumber => m_configuration.SvnRevision;

	public string ApplicationVersion => m_configuration.ApplicationVersion;

	public bool IsContentLimited => m_configuration.BuildTypeContentLimited;

	public bool IsHDVersion => m_configuration.BuildTypeHD;

	public bool IsDevelopmentBuild => m_configuration.DevelopmentBuild;

	public bool CheatsEnabled => m_configuration.EnableCheats;

	public bool LessCheats => m_configuration.LessCheats;

	public bool IsOdyssey => m_configuration.CustomerId == "AmazonUlt";

	private void Awake()
	{
		SetAsPersistant();
		TextAsset textAsset = Resources.Load<TextAsset>("gameConfig");
		m_configuration = JsonReader.Deserialize<BuildConfiguration>(textAsset.text);
		if ((bool)m_gameBuildParamLoader)
		{
			m_gameBuildParamLoader = Object.Instantiate(m_gameBuildParamLoader);
			m_gameBuildParamLoader.SetActive(value: true);
		}
	}
}
