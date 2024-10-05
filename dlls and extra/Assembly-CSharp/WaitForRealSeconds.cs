using UnityEngine;

public class WaitForRealSeconds : CustomYieldInstruction
{
	private float endTime;

	public override bool keepWaiting => Time.realtimeSinceStartup < endTime;

	public WaitForRealSeconds(float time)
	{
		endTime = Time.realtimeSinceStartup + time;
	}
}
