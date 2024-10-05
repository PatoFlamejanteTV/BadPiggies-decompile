using UnityEngine;

public class AspectRatioController : MonoBehaviour
{
	private float m_aspectRatio;

	private const float MinAspectRatio = 1.3333334f;

	private void Update()
	{
		float num = (float)Screen.width / (float)Screen.height;
		if (num != m_aspectRatio)
		{
			m_aspectRatio = num;
			if (num < 1.3333334f)
			{
				Rect rect = GetComponent<Camera>().rect;
				rect.height = num / 1.3333334f;
				rect.y = (1f - rect.height) / 2f;
				GetComponent<Camera>().rect = rect;
			}
			else
			{
				GetComponent<Camera>().rect = new Rect(0f, 0f, 1f, 1f);
			}
		}
	}
}
