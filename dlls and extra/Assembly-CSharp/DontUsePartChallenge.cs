public class DontUsePartChallenge : Challenge
{
	public BasePart.PartType m_partType;

	public override ChallengeType Type => ChallengeType.DontUseParts;

	public override bool IsCompleted()
	{
		return !WPFMonoBehaviour.levelManager.ContraptionProto.HasPart(m_partType);
	}

	private void Start()
	{
		if (m_icons.Count >= 1)
		{
			m_icons[0].icon = WPFMonoBehaviour.gameData.GetPart(m_partType).GetComponent<BasePart>().m_constructionIconSprite.gameObject;
		}
	}
}
