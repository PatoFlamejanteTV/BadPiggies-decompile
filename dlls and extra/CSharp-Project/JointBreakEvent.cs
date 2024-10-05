using UnityEngine;

public class JointBreakEvent : MonoBehaviour
{
	public delegate void JointBreak(JointBreakEvent sender);

	public JointBreak onJointBreak;

	private void OnJointBreak(float breakForce)
	{
		if (onJointBreak != null)
		{
			onJointBreak(this);
		}
	}
}
