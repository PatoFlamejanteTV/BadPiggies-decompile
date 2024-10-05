using UnityEngine;

public class TextMeshFormattedLocale : MonoBehaviour
{
	private TextMesh textMesh;

	private TextMeshLocale locale;

	private string localeKey = string.Empty;

	private string format = string.Empty;

	private object[] items;

	private void Awake()
	{
		textMesh = base.gameObject.GetComponent<TextMesh>();
	}

	public void SetText(string format, string localeKey, params object[] items)
	{
		if (!(textMesh == null))
		{
			this.localeKey = localeKey;
			this.format = format;
			this.items = items;
			TextUpdated(textMesh.gameObject);
		}
	}

	private void TextUpdated(GameObject go)
	{
		if (!(textMesh == null) && items != null && items.Length >= 1)
		{
			locale = base.gameObject.GetComponent<TextMeshLocale>();
			if (locale == null)
			{
				locale = base.gameObject.AddComponent<TextMeshLocale>();
			}
			locale.enabled = false;
			Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve(localeKey);
			textMesh.text = string.Format(format, items[0], localeParameters.translation);
		}
	}
}
