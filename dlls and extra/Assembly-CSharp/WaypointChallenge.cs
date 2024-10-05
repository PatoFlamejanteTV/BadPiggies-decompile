public class WaypointChallenge : Challenge
{
	public Collectable m_target;

	public override ChallengeType Type => ChallengeType.Box;

	public override bool IsCompleted()
	{
		return m_target.collected;
	}
}
