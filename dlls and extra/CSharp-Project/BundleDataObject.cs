using System;
using UnityEngine;

[Serializable]
public class BundleDataObject
{
	[SerializeField]
	private string assetBundle;

	[SerializeField]
	private string assetName;

	public string AssetBundle => assetBundle;

	public string AssetName
	{
		get
		{
			return assetName;
		}
		set
		{
			assetName = value;
		}
	}

	public BundleDataObject()
	{
		assetBundle = string.Empty;
		assetName = string.Empty;
	}

	public T LoadValue<T>() where T : UnityEngine.Object
	{
		string text = assetName;
		if (typeof(T) == typeof(GameObject))
		{
			text += ".prefab";
		}
		else if (typeof(T) == typeof(TextAsset))
		{
			text += ".bytes";
		}
		if (string.IsNullOrEmpty(assetName))
		{
			return null;
		}
		if (string.IsNullOrEmpty(assetBundle))
		{
			return Bundle.LoadObject<T>(text);
		}
		return Bundle.LoadObject<T>(assetBundle, text);
	}
}
