public class MarkerManager : PartManager
{
	private int[] m_indexes;

	public static MarkerManager Instance { get; private set; }

	public static bool IsInstantiated => Instance != null;

	protected override void Initialize()
	{
		base.Initialize();
		m_status = StatusCode.Running;
		Instance = this;
		Contraption.Instance.ConnectedComponentsChangedEvent += OnConnectedComponentsChanged;
	}

	public override void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public int GetTeamIndex(int connectedComponent)
	{
		return m_indexes[connectedComponent];
	}

	public int GetTeamIndex(BasePart part)
	{
		return m_indexes[part.ConnectedComponent];
	}

	public bool IsInSameTeam(BasePart partA, BasePart partB)
	{
		int teamIndex = GetTeamIndex(partA);
		int teamIndex2 = GetTeamIndex(partB);
		if (teamIndex == 0 && teamIndex2 == 0)
		{
			return partA.ConnectedComponent == partB.ConnectedComponent;
		}
		return (teamIndex & teamIndex2) != 0;
	}

	private void OnConnectedComponentsChanged()
	{
		m_indexes = new int[Contraption.Instance.ConnectedComponentCount];
		foreach (BasePart part in Contraption.Instance.Parts)
		{
			if (part.IsMarker())
			{
				m_indexes[part.ConnectedComponent] |= 1 << (int)part.m_gridRotation;
			}
		}
	}

	public static bool IsInSameTeamStatic(BasePart partA, BasePart partB)
	{
		if (IsInstantiated)
		{
			return Instance.IsInSameTeam(partA, partB);
		}
		return partA.ConnectedComponent == partB.ConnectedComponent;
	}
}
