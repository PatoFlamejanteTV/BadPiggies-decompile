using UnityEngine;

public class BundleData : ScriptableObject
{
	[SerializeField]
	private string bundleName;

	[SerializeField]
	private string[] bundleAssets;

	[SerializeField]
	private bool includeInBuild;

	public string BundleName => bundleName;

	public string[] Assets => bundleAssets;

	public void SetData(string bundleName, string[] assets)
	{
		this.bundleName = bundleName;
		bundleAssets = assets;
	}
}
