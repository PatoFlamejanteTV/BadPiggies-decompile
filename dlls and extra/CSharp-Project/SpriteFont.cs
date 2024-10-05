using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteReference))]
public class SpriteFont : MonoBehaviour
{
	[Serializable]
	public class SpriteSymbol
	{
		public string symbol = string.Empty;

		public string spriteId = string.Empty;

		public float spriteScaleX = 1f;

		public float spriteScaleY = 1f;

		[NonSerialized]
		public Vector2[] uv;

		[NonSerialized]
		[HideInInspector]
		public SpriteData spriteData;
	}

	public Dictionary<char, SpriteSymbol> m_charToSpriteSymbol = new Dictionary<char, SpriteSymbol>();

	public List<SpriteSymbol> m_symbols = new List<SpriteSymbol>();

	public SpriteSymbol GetSymbol(char c)
	{
		return m_charToSpriteSymbol[c];
	}

	public void Initialize(RuntimeSpriteDatabase db)
	{
		m_charToSpriteSymbol.Clear();
		int count = m_symbols.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_symbols[i].spriteId != string.Empty)
			{
				SpriteData spriteData = db.Find(m_symbols[i].spriteId);
				m_symbols[i].spriteData = spriteData;
				m_charToSpriteSymbol.Add(m_symbols[i].symbol[0], m_symbols[i]);
				Rect uv = spriteData.uv;
				Vector2[] array = new Vector2[4];
				array[0].x = uv.x;
				array[0].y = uv.y;
				array[1].x = uv.x;
				array[1].y = uv.y + uv.height;
				array[2].x = uv.x + uv.width;
				array[2].y = uv.y + uv.height;
				array[3].x = uv.x + uv.width;
				array[3].y = uv.y;
				m_symbols[i].uv = array;
			}
		}
	}
}
