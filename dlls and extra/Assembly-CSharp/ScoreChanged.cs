using UnityEngine;

public struct ScoreChanged : EventManager.Event
{
	public Vector3 scoreFloaterPosition;

	public ScoreChanged(Vector3 scoreFloaterPosition)
	{
		this.scoreFloaterPosition = scoreFloaterPosition;
	}
}
