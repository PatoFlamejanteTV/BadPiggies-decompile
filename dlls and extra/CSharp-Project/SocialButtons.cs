using UnityEngine;

public class SocialButtons : SliderButton
{
	private bool isChina;

	private void LoadButtonToSocialMedia(string name, string methodToInvoke)
	{
		GameObject obj = Object.Instantiate((GameObject)Resources.Load("SocialButtons/" + name));
		obj.transform.parent = base.gameObject.transform;
		obj.name = name;
		obj.transform.localPosition = new Vector3(0f, 0f, 3f);
		obj.GetComponent<Button>().MethodToCall.SetMethod(GameObject.Find("MainMenuLogic").GetComponent<MainMenu>(), methodToInvoke);
	}

	private void Awake()
	{
		if (Singleton<Localizer>.Instance.CurrentLocale == "zh-CN" || Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			LoadButtonToSocialMedia("WeibosButton", "LaunchWeibos");
			LoadButtonToSocialMedia("FilmButton", "LaunchYoutubeFilmChina");
			isChina = true;
		}
		else
		{
			LoadButtonToSocialMedia("FacebookButton", "LaunchFacebook");
			LoadButtonToSocialMedia("TwitterButton", "LaunchTwitter");
			LoadButtonToSocialMedia("FilmButton", "LaunchYoutubeFilm");
		}
		EventManager.Connect<LocalizationReloaded>(ReceiveLocalizationReloaded);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<LocalizationReloaded>(ReceiveLocalizationReloaded);
	}

	private void ReceiveLocalizationReloaded(LocalizationReloaded localizationReloaded)
	{
		if ((!isChina || !(localizationReloaded.currentLanguage != "zh-CN")) && (isChina || !(localizationReloaded.currentLanguage == "zh-CN")))
		{
			return;
		}
		int num = base.transform.childCount;
		while (--num >= 0)
		{
			Transform child = base.transform.GetChild(num);
			if (child.name.Contains("Button"))
			{
				Object.DestroyImmediate(child.gameObject);
			}
		}
		if (Singleton<Localizer>.Instance.CurrentLocale == "zh-CN" || Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			LoadButtonToSocialMedia("WeibosButton", "LaunchWeibos");
			LoadButtonToSocialMedia("FilmButton", "LaunchYoutubeFilmChina");
			isChina = true;
		}
		else
		{
			LoadButtonToSocialMedia("FacebookButton", "LaunchFacebook");
			LoadButtonToSocialMedia("TwitterButton", "LaunchTwitter");
			LoadButtonToSocialMedia("FilmButton", "LaunchYoutubeFilm");
			isChina = false;
		}
		SetInitialValues();
		GameObject.Find("MainMenuLogic").GetComponent<MainMenu>().SocialButtonReset();
	}
}
