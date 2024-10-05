using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILocale : MonoBehaviour
{
	[Serializable]
	private class SpriteLocale
	{
		public SystemLanguage m_language;

		public UnityEngine.Sprite m_sprite;
	}

	[SerializeField]
	private List<SpriteLocale> m_sprites;

	public void Awake()
	{
		SystemLanguage language = INUnity.Language;
		Image component = GetComponent<Image>();
		if (!(component != null))
		{
			return;
		}
		foreach (SpriteLocale sprite in m_sprites)
		{
			if (sprite.m_language == language)
			{
				component.sprite = sprite.m_sprite;
			}
		}
	}
}
