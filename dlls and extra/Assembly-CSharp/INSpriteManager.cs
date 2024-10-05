using System;
using System.Collections.Generic;
using UnityEngine;

public class INSpriteManager : Singleton<INSpriteManager>
{
	[SerializeField]
	private List<TextAsset> m_textAssets;

	private bool m_initialized;

	private Dictionary<string, Dictionary<string, INSpriteData>> m_atlasData;

	private void Awake()
	{
		SetAsPersistant();
	}

	public void Initialize()
	{
		m_initialized = true;
		m_atlasData = new Dictionary<string, Dictionary<string, INSpriteData>>(m_textAssets.Count);
		foreach (TextAsset textAsset in m_textAssets)
		{
			LoadAtlasData(textAsset.text);
		}
	}

	public Dictionary<string, INSpriteData> GetAtlasData(string textureName)
	{
		if (!m_initialized)
		{
			Initialize();
		}
		return m_atlasData[textureName];
	}

	private void LoadAtlasData(string text)
	{
		Dictionary<string, INSpriteData> dictionary = new Dictionary<string, INSpriteData>();
		string[] array = text.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		string[] array2 = array[0].Split(new char[2] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
		string key = array2[0];
		Vector2Int textureSize = new Vector2Int(int.Parse(array2[1]), int.Parse(array2[2]));
		for (int i = 1; i < array.Length; i++)
		{
			array2 = array[i].Split(new char[2] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			string key2 = array2[0];
			Rect rect = new Rect(float.Parse(array2[1]), float.Parse(array2[2]), float.Parse(array2[3]), float.Parse(array2[4]));
			Vector2 scale = new Vector2(float.Parse(array2[5]), float.Parse(array2[6]));
			int screenHeight = int.Parse(array2[7]);
			dictionary[key2] = new INSpriteData(rect, scale, screenHeight, textureSize);
		}
		m_atlasData.Add(key, dictionary);
	}
}
