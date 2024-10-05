public class NoChallenge : Challenge
{
	public override ChallengeType Type => ChallengeType.DontUseParts;

	public override bool IsCompleted()
	{
		return true;
	}
}
