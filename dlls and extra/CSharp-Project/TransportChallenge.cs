public class TransportChallenge : Challenge
{
	public BasePart.PartType partToTransport;

	public override ChallengeType Type => ChallengeType.Transport;

	public override bool IsCompleted()
	{
		return WPFMonoBehaviour.levelManager.IsPartTransported(partToTransport);
	}
}
