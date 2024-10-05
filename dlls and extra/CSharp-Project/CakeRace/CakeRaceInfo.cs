using System;
using UnityEngine;

namespace CakeRace;

[Serializable]
public struct CakeRaceInfo
{
	public enum Tag
	{
		Undefined = -1,
		Easy,
		Normal,
		Hard,
		Locked
	}

	private const int DEFAULT_TIME_LIMIT = 10;

	[SerializeField]
	private int m_episodeIndex;

	[SerializeField]
	private int m_levelIndex;

	[SerializeField]
	private string m_identifier;

	[SerializeField]
	private int m_trackIndex;

	[SerializeField]
	private int m_timeLimit;

	[SerializeField]
	private ObjectLocation[] m_cakeLocations;

	[SerializeField]
	private LevelManager.PartCount[] m_customParts;

	[SerializeField]
	private StartInfo m_startInfo;

	[SerializeField]
	private string m_jsonReplay;

	[SerializeField]
	private LevelManager.CameraLimits m_cameraLimits;

	[SerializeField]
	private ObjectLocation[] m_props;

	[SerializeField]
	private GameObject m_tutorialBookPrefab;

	[SerializeField]
	private GameObject m_gridCellPrefab;

	[SerializeField]
	private Tag m_tag;

	[SerializeField]
	private float[] m_cakeZoomLevels;

	public int EpisodeIndex => m_episodeIndex;

	public int LevelIndex => m_levelIndex;

	public int TrackIndex => m_trackIndex;

	public string Identifier => m_identifier;

	public string UniqueIdentifier
	{
		get
		{
			if (RegularLevel)
			{
				return $"{EpisodeIndex}-{LevelIndex}_{TrackIndex}";
			}
			return $"{Identifier}_{TrackIndex}";
		}
	}

	public int TimeLimit => m_timeLimit;

	public ObjectLocation[] CakeLocations => m_cakeLocations;

	public LevelManager.PartCount[] CustomParts => m_customParts;

	public StartInfo Start => m_startInfo;

	public string Replay => m_jsonReplay;

	public bool RegularLevel => string.IsNullOrEmpty(m_identifier);

	public bool SpecialLevel => !string.IsNullOrEmpty(m_identifier);

	public LevelManager.CameraLimits CameraLimits => m_cameraLimits;

	public ObjectLocation[] Props => m_props;

	public GameObject TutorialBookPrefab => m_tutorialBookPrefab;

	public GameObject GridCellPrefab => m_gridCellPrefab;

	public Tag InfoTag => m_tag;

	public float[] CakeZoomLevels => m_cakeZoomLevels;

	public CakeRaceInfo(int episodeIndex, int levelIndex, int trackIndex)
	{
		m_episodeIndex = episodeIndex;
		m_levelIndex = levelIndex;
		m_identifier = string.Empty;
		m_trackIndex = trackIndex;
		m_timeLimit = 10;
		m_cakeLocations = new ObjectLocation[0];
		m_customParts = new LevelManager.PartCount[0];
		m_startInfo = default(StartInfo);
		m_jsonReplay = string.Empty;
		m_cameraLimits = new LevelManager.CameraLimits();
		m_props = new ObjectLocation[0];
		m_tutorialBookPrefab = null;
		m_gridCellPrefab = null;
		m_tag = Tag.Undefined;
		m_cakeZoomLevels = new float[0];
	}

	public CakeRaceInfo(string identifier, int trackIndex)
	{
		m_episodeIndex = -1;
		m_levelIndex = -1;
		m_identifier = identifier;
		m_trackIndex = trackIndex;
		m_timeLimit = 10;
		m_cakeLocations = new ObjectLocation[0];
		m_customParts = new LevelManager.PartCount[0];
		m_startInfo = default(StartInfo);
		m_jsonReplay = string.Empty;
		m_cameraLimits = new LevelManager.CameraLimits();
		m_props = new ObjectLocation[0];
		m_tutorialBookPrefab = null;
		m_gridCellPrefab = null;
		m_tag = Tag.Undefined;
		m_cakeZoomLevels = new float[0];
	}

	public static bool operator ==(CakeRaceInfo a, CakeRaceInfo b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(CakeRaceInfo a, CakeRaceInfo b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CakeRaceInfo other))
		{
			return false;
		}
		return Equals(other);
	}

	public bool Equals(CakeRaceInfo other)
	{
		if (m_episodeIndex == other.m_episodeIndex && m_levelIndex == other.m_levelIndex && m_identifier == other.m_identifier)
		{
			return m_trackIndex == other.m_trackIndex;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return 17 * (m_episodeIndex * 23) * (m_levelIndex * 79) * (m_trackIndex * 773) * m_identifier.GetHashCode();
	}
}
