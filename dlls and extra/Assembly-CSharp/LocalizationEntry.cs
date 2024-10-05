using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Sprite))]
public class LocalizationEntry : IComparable<LocalizationEntry>
{
	public string locale;

	public string sprite;

	public LocalizationEntry(string locale, string sprite)
	{
		this.locale = locale;
		this.sprite = sprite;
	}

	public int CompareTo(LocalizationEntry other)
	{
		return locale.CompareTo(other.locale);
	}
}
