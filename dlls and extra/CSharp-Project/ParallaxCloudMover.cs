using UnityEngine;

public class ParallaxCloudMover : CloudMover
{
	private void Update()
	{
		Vector3 localPosition = base.transform.localPosition + Vector3.right * m_velocity * Time.deltaTime;
		if (Mathf.Sign(m_velocity) * localPosition.x > base.transform.parent.position.x + m_limits)
		{
			localPosition.x = (0f - m_limits) * Mathf.Sign(m_velocity);
		}
		if (Mathf.Sign(m_velocity) * localPosition.x < base.transform.parent.position.x - m_limits)
		{
			localPosition.x = m_limits * Mathf.Sign(m_velocity);
		}
		base.transform.localPosition = localPosition;
	}
}
