using UnityEngine;

public class Billboard : MonoBehaviour
{
	public Transform m_upFrom;

	public void LateUpdate()
	{
		Vector3 normalized = (base.transform.position - Camera.main.transform.position).normalized;
		Vector3 rhs = base.transform.right;
		if ((bool)m_upFrom)
		{
			rhs = Vector3.Cross(m_upFrom.up, normalized);
		}
		Vector3 normalized2 = Vector3.Cross(normalized, rhs).normalized;
		base.transform.rotation = Quaternion.LookRotation(normalized, normalized2);
	}
}
