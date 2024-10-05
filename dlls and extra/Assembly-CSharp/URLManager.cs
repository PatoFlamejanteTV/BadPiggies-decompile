using UnityEngine;

public class URLManager : Singleton<URLManager>
{
	public enum LinkType
	{
		Youtube,
		Facebook,
		Twitter,
		Renren,
		Weibos,
		YoutubeChina,
		Eula,
		Privacy,
		FBLike,
		GetPCRegistrationKey,
		CrossPromoClassic,
		CrossPromoSpace,
		CrossPromoAlex,
		CrossPromoShop,
		CrossPromoNewsLetter,
		CrossPromoSeasons,
		BadPiggiesAppStore,
		CrossPromoStarWars,
		CrossPromoStarWars2,
		CrossPromoAngryBirdsFriends,
		CrossPromoAngryBirdsGo,
		CrossPromoAngryBirdsRio,
		MajorLazerMusic,
		AppRaterLink
	}

	private string m_baseURLString = string.Empty;

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		Singleton<URLManager>.instance = this;
	}

	private void Start()
	{
		GenerateURLBaseString();
	}

	private void GenerateURLBaseString()
	{
		m_baseURLString = "http://cloud.rovio.com/link/redirect/?";
		switch (DeviceInfo.ActiveDeviceFamily)
		{
		case DeviceInfo.DeviceFamily.Android:
			m_baseURLString += "d=android";
			break;
		case DeviceInfo.DeviceFamily.Pc:
			m_baseURLString += "d=windows";
			break;
		case DeviceInfo.DeviceFamily.Osx:
			m_baseURLString += "d=osx";
			break;
		case DeviceInfo.DeviceFamily.BB10:
			m_baseURLString += "d=blackberry";
			break;
		case DeviceInfo.DeviceFamily.WP8:
			m_baseURLString += "d=wp8";
			break;
		}
		m_baseURLString += "&p=bps";
		if (Singleton<BuildCustomizationLoader>.Instance.IsContentLimited)
		{
			m_baseURLString += "&a=free";
		}
		else if (Singleton<BuildCustomizationLoader>.Instance.IsHDVersion)
		{
			m_baseURLString += "&a=HD";
		}
		else
		{
			m_baseURLString += "&a=full";
		}
		m_baseURLString = m_baseURLString + "&v=" + Singleton<BuildCustomizationLoader>.Instance.ApplicationVersion;
		m_baseURLString += "&r=game";
		m_baseURLString = m_baseURLString + "&c=" + Singleton<BuildCustomizationLoader>.Instance.CustomerID;
		m_baseURLString = m_baseURLString + "&i=" + SystemInfo.deviceUniqueIdentifier;
	}

	public string MakeProductTarget(string target)
	{
		if (Singleton<BuildCustomizationLoader>.Instance.CustomerID != "Rovio" && DeviceInfo.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Android)
		{
			target = target + "_" + Singleton<BuildCustomizationLoader>.Instance.CustomerID;
		}
		return target;
	}

	public void OpenURL(LinkType type)
	{
		string text = "&t=";
		switch (type)
		{
		case LinkType.Youtube:
			text += "rovioyoutube";
			break;
		case LinkType.Facebook:
			text += "facebook";
			break;
		case LinkType.Twitter:
			text += "twitter";
			break;
		case LinkType.Renren:
			text += "renren";
			break;
		case LinkType.Weibos:
			text += "weibo";
			break;
		case LinkType.YoutubeChina:
			text += "youku";
			break;
		case LinkType.Eula:
			text += "eula";
			break;
		case LinkType.Privacy:
			text += "privacypolicy";
			break;
		case LinkType.FBLike:
			text += "facebooklike";
			break;
		case LinkType.GetPCRegistrationKey:
			text += MakeProductTarget("badpiggiesfullpc");
			break;
		case LinkType.CrossPromoClassic:
			text += MakeProductTarget("angrybirdsfull");
			break;
		case LinkType.CrossPromoSpace:
			text += MakeProductTarget("angrybirdsspacefull");
			break;
		case LinkType.CrossPromoAlex:
			text += MakeProductTarget("amazingalexfull");
			break;
		case LinkType.CrossPromoShop:
			text += "shop";
			break;
		case LinkType.CrossPromoNewsLetter:
			text += string.Empty;
			break;
		case LinkType.CrossPromoSeasons:
			text += MakeProductTarget("angrybirdsseasonsfull");
			break;
		case LinkType.BadPiggiesAppStore:
			text = ((!Singleton<BuildCustomizationLoader>.Instance.IsContentLimited) ? ((!(Singleton<BuildCustomizationLoader>.Instance.CustomerID == "amazon")) ? (text + "badpiggiesfull") : (text + "badpiggiesfull_amazon")) : (text + "badpiggiesfree"));
			break;
		case LinkType.CrossPromoStarWars:
			text += "angrybirdsstarwarsfull";
			break;
		case LinkType.CrossPromoStarWars2:
			text += MakeProductTarget("angrybirdsstarwars2full");
			break;
		case LinkType.CrossPromoAngryBirdsFriends:
			text += MakeProductTarget("angrybirdsfriendsfull");
			break;
		case LinkType.CrossPromoAngryBirdsGo:
			text += MakeProductTarget("angrybirdsgofull");
			break;
		case LinkType.CrossPromoAngryBirdsRio:
			text += MakeProductTarget("angrybirdsriofull");
			break;
		case LinkType.MajorLazerMusic:
			text += MakeProductTarget("badpiggiestune");
			break;
		case LinkType.AppRaterLink:
			text = ((!Singleton<BuildCustomizationLoader>.Instance.IsContentLimited) ? (text + "badpiggiesfull") : (text + "badpiggiesfree"));
			break;
		}
		if (DeviceInfo.IsDesktop && Screen.fullScreen)
		{
			Screen.fullScreen = false;
		}
		Application.OpenURL(m_baseURLString + text);
	}
}
