using UnityEngine;

public class DessertPlace : MonoBehaviour
{
	public bool isFlying;

	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		if (isFlying)
		{
			Gizmos.color = new Color(1f, 1.05f, 0f, 0.6f);
		}
		else
		{
			Gizmos.color = new Color(1f, 0.5f, 0.15f, 0.6f);
		}
		Gizmos.DrawCube(Vector3.up * 0.5f, Vector3.one);
	}
}
