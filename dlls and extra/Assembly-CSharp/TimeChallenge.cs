using UnityEngine;

public class TimeChallenge : Challenge
{
	public float m_targetTime;

	public override ChallengeType Type => ChallengeType.Time;

	public override bool IsCompleted()
	{
		return Mathf.Floor(WPFMonoBehaviour.levelManager.CompletionTime * 100f) / 100f <= m_targetTime;
	}

	public override float TimeLimit()
	{
		return m_targetTime;
	}
}
