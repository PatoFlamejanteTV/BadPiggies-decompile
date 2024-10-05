using UnityEngine;

[ExecuteInEditMode]
public class BundleTexture : MonoBehaviour
{
	[SerializeField]
	private BundleDataObject bundleObject;

	[SerializeField]
	private Renderer targetRenderer;

	[SerializeField]
	private int targetMaterialIndex;

	private void Awake()
	{
		if (targetRenderer == null || targetMaterialIndex < 0 || targetMaterialIndex >= targetRenderer.sharedMaterials.Length || targetRenderer.sharedMaterials[targetMaterialIndex] == null || !Application.isPlaying)
		{
			return;
		}
		Material material = targetRenderer.sharedMaterials[targetMaterialIndex];
		if (!(material == null))
		{
			Texture2D texture2D = bundleObject.LoadValue<Texture2D>();
			if (texture2D != null)
			{
				material.mainTexture = texture2D;
			}
		}
	}
}
