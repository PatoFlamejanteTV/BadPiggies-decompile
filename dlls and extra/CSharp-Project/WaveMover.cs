using System;
using UnityEngine;

public class WaveMover : MonoBehaviour
{
	public bool m_useRealtime;

	public float m_rangeX;

	public float m_speedX;

	public float m_rangeY;

	public float m_speedY;

	public float m_startX;

	public float m_startY;

	public float m_rangeScale = 1.5f;

	public float m_speedScale;

	public float m_startScale = 1f;

	protected Vector3 m_origPos;

	protected Vector3 m_origScale;

	protected float m_periodX;

	protected float m_periodY;

	protected float m_periodScale;

	protected float m_lastRT;

	private Transform cachedTransform;

	private void Start()
	{
		m_origPos = base.transform.position;
		m_origScale = base.transform.localScale;
		cachedTransform = base.transform;
		m_periodX = m_startX;
		m_periodY = m_startY;
		m_periodScale = m_startScale;
		m_lastRT = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = Time.deltaTime;
		if (m_useRealtime)
		{
			num = realtimeSinceStartup - m_lastRT;
			if (num <= 0f || num > 0.2f)
			{
				num = 0.05f;
			}
		}
		m_lastRT = realtimeSinceStartup;
		float num2 = num * m_speedX;
		m_periodX += num2;
		if (m_periodX > (float)Math.PI)
		{
			m_periodX -= (float)Math.PI * 2f;
		}
		m_periodY += num * m_speedY;
		if (m_periodY > (float)Math.PI)
		{
			m_periodY -= (float)Math.PI * 2f;
		}
		Vector3 position = m_origPos + cachedTransform.up * Mathf.Sin(m_periodX) * m_rangeX + cachedTransform.right * Mathf.Sin(m_periodY) * m_rangeY;
		float b = ((cachedTransform.position.y - position.y > 0f) ? 50 : 0);
		cachedTransform.position = position;
		for (int i = 0; i < cachedTransform.childCount; i++)
		{
			Transform child = cachedTransform.GetChild(i);
			float x = child.eulerAngles.x;
			child.eulerAngles = new Vector3(Mathf.Lerp(x, b, num2), 0f, 0f);
		}
		m_periodScale += num * m_speedScale;
		if (m_periodScale > (float)Math.PI)
		{
			m_periodScale -= (float)Math.PI * 2f;
		}
		Vector3 localScale = m_origScale + Mathf.Sin(m_periodScale) * m_rangeScale * m_origScale;
		cachedTransform.localScale = localScale;
	}
}
