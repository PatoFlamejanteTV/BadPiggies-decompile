using UnityEngine;

public class FontFade : MonoBehaviour
{
	private bool m_fadingInProgress;

	private TextMesh m_textMesh;

	private float m_currentAlpha = 1f;

	private Vector3 m_originalPosition;

	public bool doTransform = true;

	private void Update()
	{
		if (m_fadingInProgress)
		{
			m_currentAlpha -= Time.deltaTime;
			Color color = m_textMesh.color;
			color.a = m_currentAlpha;
			m_textMesh.color = color;
			if (m_currentAlpha <= 0f)
			{
				base.gameObject.SetActive(value: false);
				m_fadingInProgress = false;
				base.transform.position = m_originalPosition;
			}
			if (doTransform)
			{
				base.transform.position = base.transform.position + new Vector3(0f, Time.deltaTime * 0.5f, 0f);
			}
		}
	}

	private void OnEnable()
	{
		m_textMesh = GetComponent<TextMesh>();
		m_fadingInProgress = false;
		m_originalPosition = base.transform.position;
		if ((bool)m_textMesh)
		{
			m_currentAlpha = 1.2f;
			Color color = m_textMesh.color;
			color.a = 1f;
			m_textMesh.color = color;
			m_fadingInProgress = true;
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
