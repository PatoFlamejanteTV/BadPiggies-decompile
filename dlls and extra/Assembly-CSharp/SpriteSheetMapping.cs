using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetMapping : MonoBehaviour
{
	[Serializable]
	public class SheetMapping
	{
		public Material source;

		[HideInInspector]
		public string sourceId;

		public Material atlas;
	}

	public class AtlasContents
	{
		public Texture atlasTexture;

		public List<Material> sources = new List<Material>();
	}

	public const string AssetPath = "Assets/Data/Materials/GUISystem/SpriteSheetMapping.prefab";

	public List<SheetMapping> m_data;

	public List<AtlasContents> GetSourcesByAtlasTexture()
	{
		List<AtlasContents> list = new List<AtlasContents>();
		HashSet<Texture> hashSet = new HashSet<Texture>();
		foreach (SheetMapping datum in m_data)
		{
			hashSet.Add(datum.atlas.mainTexture);
		}
		foreach (Texture item in hashSet)
		{
			AtlasContents sourcesForAtlasTexture = GetSourcesForAtlasTexture(item);
			list.Add(sourcesForAtlasTexture);
		}
		return list;
	}

	public AtlasContents GetSourcesForAtlasTexture(Texture atlasTexture)
	{
		AtlasContents atlasContents = new AtlasContents();
		atlasContents.atlasTexture = atlasTexture;
		atlasContents.atlasTexture = atlasTexture;
		foreach (SheetMapping datum in m_data)
		{
			if (datum.atlas.mainTexture == atlasTexture)
			{
				atlasContents.sources.Add(datum.source);
			}
		}
		return atlasContents;
	}

	public Material FindAtlasFor(Material material)
	{
		foreach (SheetMapping datum in m_data)
		{
			if (datum.source == material || datum.atlas == material)
			{
				return datum.atlas;
			}
		}
		return null;
	}

	public bool IsSource(Material material)
	{
		for (int i = 0; i < m_data.Count; i++)
		{
			if (m_data[i].source == material)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAtlas(Material material)
	{
		for (int i = 0; i < m_data.Count; i++)
		{
			if (m_data[i].atlas == material)
			{
				return true;
			}
		}
		return false;
	}
}
