using UnityEngine;

public class CustomShaderSprite : MonoBehaviour
{
	[SerializeField]
	private Shader m_shader;

	[SerializeField]
	private Color m_shaderColor;

	private GameObject currentSprite;

	public void SetSprite(GameObject sprite)
	{
		if (currentSprite != null)
		{
			Object.Destroy(currentSprite);
		}
		currentSprite = Object.Instantiate(sprite);
		currentSprite.transform.parent = base.transform;
		currentSprite.transform.localPosition = Vector3.zero;
		currentSprite.transform.localRotation = Quaternion.identity;
		currentSprite.transform.localScale = Vector3.one;
		LayerHelper.SetLayer(currentSprite, base.gameObject.layer, children: true);
		LayerHelper.SetOrderInLayer(currentSprite, 1, children: true);
		LayerHelper.SetSortingLayer(currentSprite, "Default", children: true);
		Sprite[] componentsInChildren = currentSprite.GetComponentsInChildren<Sprite>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer component = componentsInChildren[i].gameObject.GetComponent<Renderer>();
			Material material = component.material;
			material.shader = m_shader;
			material.color = m_shaderColor;
			component.sharedMaterial = material;
		}
	}

	public void ClearSprite()
	{
		if ((bool)currentSprite)
		{
			Object.Destroy(currentSprite);
		}
	}
}
