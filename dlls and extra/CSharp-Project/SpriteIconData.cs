using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteIconData : MonoBehaviour
{
	[Serializable]
	public class SpriteIcon
	{
		public string tag;

		public string spriteId;

		public float scale;

		public Material atlasReference;

		[HideInInspector]
		public string quad;

		public SpriteIcon()
		{
			tag = string.Empty;
			spriteId = string.Empty;
			scale = 1f;
			atlasReference = null;
		}
	}

	private static SpriteIconData _instance;

	[SerializeField]
	private List<SpriteIcon> icons;

	private List<string> iconTags;

	public static SpriteIconData Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = Resources.Load<GameObject>("Utility/SpriteIconData");
				if (gameObject != null)
				{
					_instance = UnityEngine.Object.Instantiate(gameObject).GetComponent<SpriteIconData>();
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	public SpriteIcon GetSpriteIcon(string tag)
	{
		if (icons == null || string.IsNullOrEmpty(tag))
		{
			return null;
		}
		foreach (SpriteIcon icon in icons)
		{
			if (icon.tag.Equals(tag))
			{
				return icon;
			}
		}
		return null;
	}

	public List<string> GetTags()
	{
		if (iconTags == null || iconTags.Count == 0)
		{
			iconTags = new List<string>();
			foreach (SpriteIcon icon in icons)
			{
				iconTags.Add(icon.tag);
			}
		}
		return iconTags;
	}
}
