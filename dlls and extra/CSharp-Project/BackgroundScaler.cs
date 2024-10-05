using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
	private Vector3 originalScale;

	private void Awake()
	{
		originalScale = base.transform.localScale;
	}

	private void Update()
	{
		base.transform.localScale = new Vector3(originalScale.x * (float)Screen.width / (float)Screen.height / 1.3333334f, originalScale.y, originalScale.z);
	}
}
