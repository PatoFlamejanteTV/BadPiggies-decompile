using UnityEngine;

public class TextMeshLocale : MonoBehaviour
{
	private TextMesh targetTextMesh;

	private string originalTextContents = string.Empty;

	private float originalCharacterSize;

	private float originalLineSpacing;

	private string postfix = string.Empty;

	private Localizer.LocaleParameters localeParameters;

	private bool isInitialized;

	public string Postfix
	{
		set
		{
			postfix = value;
			targetTextMesh.text = localeParameters.translation + postfix;
		}
	}

	private void Start()
	{
		ApplyLocale();
	}

	private void ApplyLocale(string localeName = null)
	{
		localeParameters = Singleton<Localizer>.Instance.Resolve(originalTextContents, localeName);
		Font font = Singleton<Localizer>.Instance.GetFont(localeName);
		if ((bool)font)
		{
			Color color = targetTextMesh.GetComponent<Renderer>().material.color;
			targetTextMesh.font = font;
			targetTextMesh.GetComponent<Renderer>().material = font.material;
			targetTextMesh.GetComponent<Renderer>().material.color = color;
		}
		targetTextMesh.text = localeParameters.translation + postfix;
		base.gameObject.SendMessageUpwards("TextUpdated", base.gameObject, SendMessageOptions.DontRequireReceiver);
		targetTextMesh.characterSize = originalCharacterSize * localeParameters.characterSizeFactor;
		targetTextMesh.lineSpacing = originalLineSpacing * localeParameters.lineSpacingFactor;
	}

	public void RefreshTranslation(string localeName = null)
	{
		Init();
		originalTextContents = targetTextMesh.text;
		ApplyLocale(localeName);
	}

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		if (!isInitialized)
		{
			targetTextMesh = GetComponent<TextMesh>();
			originalCharacterSize = targetTextMesh.characterSize;
			originalLineSpacing = targetTextMesh.lineSpacing;
			originalTextContents = targetTextMesh.text;
			EventManager.Connect<LocalizationReloaded>(ReceiveLocalizationReloaded);
			isInitialized = true;
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<LocalizationReloaded>(ReceiveLocalizationReloaded);
	}

	private void ReceiveLocalizationReloaded(LocalizationReloaded localizationReloaded)
	{
		ApplyLocale();
	}
}
