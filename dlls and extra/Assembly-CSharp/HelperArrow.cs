using UnityEngine;

public class HelperArrow : WPFMonoBehaviour
{
	public Material m_materialArrow;

	protected LineRenderer m_lineRenderer;

	protected Transform m_target;

	protected float m_alpha;

	protected Camera m_gameCamera;

	public void Awake()
	{
		m_lineRenderer = GetComponent<LineRenderer>();
		if (!m_lineRenderer)
		{
			m_lineRenderer = base.gameObject.AddComponent<LineRenderer>();
		}
		m_lineRenderer.SetVertexCount(2);
		m_lineRenderer.material = m_materialArrow;
		Transform goalPosition = WPFMonoBehaviour.levelManager.GoalPosition;
		if ((bool)goalPosition)
		{
			m_target = goalPosition;
		}
		m_gameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}

	public void LateUpdate()
	{
		if ((bool)m_target)
		{
			Vector3 position = m_gameCamera.transform.position;
			Vector3 position2 = m_target.transform.position;
			position.z = 0f;
			position2.z = 0f;
			Vector3 vector = position2 - position;
			float num = 1f;
			vector = vector.normalized;
			position -= vector * num;
			position2 += vector * 0.5f;
			Vector3 vector2 = WPFMonoBehaviour.ClipAgainstViewport(position, position2);
			float num2 = Vector3.Distance(position2, vector2);
			bool flag = false;
			if (num2 < 1f)
			{
				flag = true;
			}
			vector2 -= vector * num;
			Vector3 position3 = vector2 - vector;
			Vector3 position4 = vector2;
			m_lineRenderer.SetPosition(0, position3);
			m_lineRenderer.SetPosition(1, position4);
			if (flag)
			{
				m_alpha -= Time.deltaTime * 5f;
			}
			else
			{
				m_alpha += Time.deltaTime * 5f;
			}
			m_alpha = Mathf.Clamp01(m_alpha);
			Color white = Color.white;
			white.a = m_alpha;
			m_lineRenderer.SetColors(white, white);
		}
	}
}
