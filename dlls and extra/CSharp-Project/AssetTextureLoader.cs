using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AssetTextureLoader : MonoBehaviour
{
	[SerializeField]
	private List<MaterialTexturePair> materials;

	[SerializeField]
	private BundleSelector bundleToUnload;

	private void Awake()
	{
		if (Bundle.initialized)
		{
			InitTextures();
		}
		else
		{
			Bundle.AssetBundlesLoaded = (Action)Delegate.Combine(Bundle.AssetBundlesLoaded, new Action(InitTextures));
		}
	}

	private void OnDestroy()
	{
		Bundle.AssetBundlesLoaded = (Action)Delegate.Remove(Bundle.AssetBundlesLoaded, new Action(InitTextures));
	}

	private void InitTextures()
	{
		GC.Collect();
		Resources.UnloadUnusedAssets();
		foreach (MaterialTexturePair material in materials)
		{
			material.material.mainTexture = material.bundleData.LoadValue<Texture2D>();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
