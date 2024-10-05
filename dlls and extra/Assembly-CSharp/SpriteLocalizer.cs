using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLocalizer : MonoBehaviour, IEnumerable<LocalizationEntry>, IEnumerable
{
	public const string DefaultLocale = "en-EN";

	[SerializeField]
	private List<LocalizationEntry> m_mapping = new List<LocalizationEntry>();

	private string m_defaultSprite;

	private void Awake()
	{
		m_defaultSprite = base.gameObject.GetComponent<Sprite>().Id;
	}

	private void OnEnable()
	{
		EventManager.Connect<LocalizationReloaded>(RefreshSpriteLocale);
		RefreshSpriteLocale(new LocalizationReloaded(Singleton<Localizer>.Instance.CurrentLocale));
	}

	private void OnDisable()
	{
		EventManager.Disconnect<LocalizationReloaded>(RefreshSpriteLocale);
	}

	private void RefreshSpriteLocale(LocalizationReloaded localeChange)
	{
		string currentLanguage = localeChange.currentLanguage;
		string localizedSprite = GetLocalizedSprite(currentLanguage);
		SpriteData data = Singleton<RuntimeSpriteDatabase>.Instance.Find(localizedSprite);
		base.gameObject.GetComponent<Sprite>().SelectSprite(data);
	}

	public IEnumerator<LocalizationEntry> GetEnumerator()
	{
		return m_mapping.GetEnumerator();
	}

	public void AddLocalization(string locale, string sprite = "")
	{
		m_mapping.Add(new LocalizationEntry(locale, sprite));
		m_mapping.Sort();
	}

	public void RemoveLocalization(string locale)
	{
		int? num = LocaleToIndex(locale);
		m_mapping.RemoveAt(num.Value);
	}

	public bool ContainsLocalization(string locale)
	{
		return LocaleToIndex(locale).HasValue;
	}

	public string GetDefaultSprite()
	{
		if (m_defaultSprite == null)
		{
			return base.gameObject.GetComponent<Sprite>().Id;
		}
		return m_defaultSprite;
	}

	public string GetSprite(string locale)
	{
		if (locale == "en-EN")
		{
			return GetDefaultSprite();
		}
		int? num = LocaleToIndex(locale);
		if (num.HasValue)
		{
			return m_mapping[num.Value].sprite;
		}
		return null;
	}

	public string GetLocalizedSprite(string locale)
	{
		string sprite = GetSprite(locale);
		if (sprite != null && sprite.Length > 0)
		{
			return sprite;
		}
		return m_defaultSprite;
	}

	public void SetSprite(string locale, string sprite)
	{
		int? num = LocaleToIndex(locale);
		m_mapping[num.Value].sprite = sprite;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private int? LocaleToIndex(string locale)
	{
		int num = m_mapping.BinarySearch(new LocalizationEntry(locale, null));
		if (num >= 0)
		{
			return num;
		}
		return null;
	}
}
