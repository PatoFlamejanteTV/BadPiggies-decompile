using UnityEngine;

public class SimpleFloatingAnimation : MonoBehaviour
{
	public Vector3 m_direction;

	public float m_power;

	private Vector3 m_startingPosition;

	private float m_timer;

	private Transform m_transform;

	private void Awake()
	{
		m_transform = base.transform;
		m_startingPosition = Vector3.zero;
		m_timer = 0f;
	}

	private void Update()
	{
		if (m_startingPosition == Vector3.zero)
		{
			m_startingPosition = m_transform.position;
		}
		if (base.enabled)
		{
			m_transform.position = m_startingPosition + m_direction * Mathf.Sin((m_timer - 5f) * m_power);
		}
		m_timer += Time.deltaTime;
	}
}
