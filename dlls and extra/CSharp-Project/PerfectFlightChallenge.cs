public class PerfectFlightChallenge : Challenge
{
	public override ChallengeType Type => ChallengeType.PerfectFlight;

	public override bool IsCompleted()
	{
		if ((bool)WPFMonoBehaviour.levelManager.ContraptionRunning)
		{
			return !WPFMonoBehaviour.levelManager.ContraptionRunning.IsBroken();
		}
		return false;
	}
}
