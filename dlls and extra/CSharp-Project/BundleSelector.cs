using System;
using UnityEngine;

[Serializable]
public class BundleSelector
{
	[SerializeField]
	private string assetBundle;

	public string AssetBundle => assetBundle;

	public BundleSelector()
	{
		assetBundle = string.Empty;
	}

	public void LoadAssetBundle()
	{
	}

	public void UnloadAssetBundle()
	{
	}
}
