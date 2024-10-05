using System.Collections;
using UnityEngine;

public class AppRater : Singleton<AppRater>
{
	private void Awake()
	{
		SetAsPersistant();
		if (!GameProgress.GetBool("AppRaterDisabled"))
		{
			int @int = GameProgress.GetInt("AppRaterInterval");
			@int++;
			if (@int >= 15)
			{
				StartCoroutine(ShowRatingPrompt());
				GameProgress.SetInt("AppRaterInterval", 0);
			}
			else
			{
				GameProgress.SetInt("AppRaterInterval", @int);
			}
		}
	}

	public void OnButtonPressed(string index)
	{
		if (index == "1")
		{
			Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.AppRaterLink);
		}
		GameProgress.SetBool("AppRaterDisabled", value: true);
	}

	private IEnumerator ShowRatingPrompt()
	{
		yield return new WaitForSeconds(1f);
		Singleton<Localizer>.Instance.Resolve("ITEM_RATE_US_TITLE");
		Singleton<Localizer>.Instance.Resolve("ITEM_RATE_US_PROMPT");
		Singleton<Localizer>.Instance.Resolve("ITEM_RATE_US_SELECT_YES");
		Singleton<Localizer>.Instance.Resolve("ITEM_RATE_US_SELECT_NO");
	}
}
