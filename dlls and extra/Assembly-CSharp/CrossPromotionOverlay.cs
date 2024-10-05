using UnityEngine;

public class CrossPromotionOverlay : MonoBehaviour
{
	private Vector3 shopStartPosition;

	private Vector3 spaceStartPosition;

	public void Start()
	{
		shopStartPosition = base.transform.Find("Layout/ShopButton").gameObject.transform.localPosition;
		spaceStartPosition = base.transform.Find("Layout/SpaceButton").gameObject.transform.localPosition;
	}

	public void Update()
	{
		if (Singleton<Localizer>.Instance.CurrentLocale == "zh-CN" || Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			base.transform.Find("Layout/SeasonsButton").gameObject.SetActive(value: false);
			base.transform.Find("Layout/ShopButton").gameObject.transform.localPosition = shopStartPosition + Vector3.left * 3.74f;
			base.transform.Find("Layout/SpaceButton").gameObject.transform.localPosition = spaceStartPosition + Vector3.left * 3.74f;
		}
		else
		{
			base.transform.Find("Layout/SeasonsButton").gameObject.SetActive(value: true);
			base.transform.Find("Layout/ShopButton").gameObject.transform.localPosition = shopStartPosition;
			base.transform.Find("Layout/SpaceButton").gameObject.transform.localPosition = spaceStartPosition;
		}
	}

	public void DismissDialog()
	{
		base.gameObject.SetActive(value: false);
	}

	public void OpenUrl(URLManager.LinkType linkType)
	{
		Singleton<URLManager>.Instance.OpenURL(linkType);
	}

	public void CloseDialog()
	{
		GetComponent<Animation>().Play("CrossPromotionOverlayFadeOut");
	}
}
