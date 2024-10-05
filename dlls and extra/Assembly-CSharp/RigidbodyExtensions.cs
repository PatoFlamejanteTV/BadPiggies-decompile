using UnityEngine;

public static class RigidbodyExtensions
{
	public static bool IsFixed(this Rigidbody rigidbody)
	{
		if (!rigidbody.isKinematic)
		{
			return rigidbody.constraints == RigidbodyConstraints.FreezeAll;
		}
		return true;
	}
}
