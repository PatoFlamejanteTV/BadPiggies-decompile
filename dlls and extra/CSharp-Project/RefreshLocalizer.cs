using System;
using UnityEngine;

public class RefreshLocalizer : IDisposable
{
	private TextMesh target;

	private string originalText = string.Empty;

	private string localizedText = string.Empty;

	private float originalCharacterSize;

	private float originalLineSpacing;

	private bool disposed;

	private Func<string> update;

	public Func<string> Update
	{
		get
		{
			return update;
		}
		set
		{
			update = value;
		}
	}

	public TextMesh Target => target;

	public RefreshLocalizer(TextMesh target)
	{
		this.target = target;
		originalText = target.text;
		originalCharacterSize = target.characterSize;
		originalLineSpacing = target.lineSpacing;
		ReloadLocalization(default(LocalizationReloaded));
		EventManager.Connect<LocalizationReloaded>(ReloadLocalization);
	}

	~RefreshLocalizer()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (!disposed)
		{
			EventManager.Disconnect<LocalizationReloaded>(ReloadLocalization);
			target = null;
			originalText = null;
			localizedText = null;
			update = null;
			GC.SuppressFinalize(this);
		}
	}

	private void ReloadLocalization(LocalizationReloaded localizationReloaded)
	{
		ApplyLocale();
		Refresh();
	}

	private void ApplyLocale(string localeName = null)
	{
		Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve(originalText, localeName);
		Font font = Singleton<Localizer>.Instance.GetFont(localeName);
		if ((bool)font)
		{
			Color color = target.GetComponent<Renderer>().material.color;
			target.font = font;
			target.GetComponent<Renderer>().material = font.material;
			target.GetComponent<Renderer>().material.color = color;
		}
		localizedText = localeParameters.translation;
		target.characterSize = originalCharacterSize * localeParameters.characterSizeFactor;
		target.lineSpacing = originalLineSpacing * localeParameters.lineSpacingFactor;
	}

	public void Refresh()
	{
		if (update != null)
		{
			target.text = string.Format(localizedText, update());
		}
	}
}
