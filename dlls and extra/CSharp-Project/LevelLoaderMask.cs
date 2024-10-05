using UnityEngine;

public class LevelLoaderMask : MonoBehaviour
{
	private float fade;

	private Color targetColor = new Color(0f, 0f, 0f, 0f);

	private void Update()
	{
		fade += Time.deltaTime * 10f;
		GetComponent<Renderer>().material.color = Color.Lerp(Color.black, targetColor, fade);
		if (fade >= 1f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
