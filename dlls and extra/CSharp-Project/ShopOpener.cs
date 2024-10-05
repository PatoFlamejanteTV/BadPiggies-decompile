using UnityEngine;

public class ShopOpener : MonoBehaviour
{
	[SerializeField]
	private GameObject toggledGameObject;

	public void OpenShop()
	{
		if (!Loader.isLoadingLevel && !Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			toggledGameObject.SetActive(value: false);
			Singleton<IapManager>.Instance.OpenShopPage(delegate
			{
				toggledGameObject.SetActive(value: true);
			});
		}
	}
}
