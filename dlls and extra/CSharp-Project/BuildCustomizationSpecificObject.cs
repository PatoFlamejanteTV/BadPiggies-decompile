using UnityEngine;

public class BuildCustomizationSpecificObject : WPFMonoBehaviour
{
	public enum Condition
	{
		IsContentLimited,
		IsContentLimitedHasFieldOfDreams,
		HasLeaderboards
	}

	public Condition m_condition;

	public bool m_not;

	private void Start()
	{
		switch (m_condition)
		{
		case Condition.HasLeaderboards:
			DestroyIf(flag: true);
			break;
		case Condition.IsContentLimitedHasFieldOfDreams:
			DestroyIf(GameProgress.GetFullVersionUnlocked() || !GameProgress.GetSandboxUnlocked("S-F"));
			break;
		case Condition.IsContentLimited:
			DestroyIf(GameProgress.GetFullVersionUnlocked());
			break;
		}
	}

	private void DestroyIf(bool flag)
	{
		if (flag && !m_not)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
