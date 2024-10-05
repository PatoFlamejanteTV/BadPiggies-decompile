using UnityEngine;

public class CloudMover : MonoBehaviour
{
	[HideInInspector]
	public float m_velocity = 10f;

	[HideInInspector]
	public float m_limits = 100f;

	private void Update()
	{
		Vector3 position = base.transform.position + Vector3.right * m_velocity * Time.deltaTime;
		if (Mathf.Sign(m_velocity) * position.x > base.transform.parent.position.x + m_limits)
		{
			position.x = (0f - m_limits) * Mathf.Sign(m_velocity);
		}
		if (Mathf.Sign(m_velocity) * position.x < base.transform.parent.position.x - m_limits)
		{
			position.x = m_limits * Mathf.Sign(m_velocity);
		}
		base.transform.position = position;
	}
}
