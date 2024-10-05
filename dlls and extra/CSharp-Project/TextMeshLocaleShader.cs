using UnityEngine;

public class TextMeshLocaleShader : MonoBehaviour
{
	[SerializeField]
	private Shader shader;

	private Renderer renderer;

	private Color originalColor = Color.white;

	private void TextUpdated(GameObject caller)
	{
		if (caller != base.gameObject)
		{
			return;
		}
		if (renderer == null)
		{
			renderer = GetComponent<Renderer>();
			TextMesh component = GetComponent<TextMesh>();
			if (component != null)
			{
				originalColor = component.color;
			}
		}
		if (renderer != null)
		{
			renderer.material.shader = shader;
			renderer.material.color = originalColor;
		}
	}
}
