using UnityEngine;

public class localizeToons : MonoBehaviour
{
	public GameObject ToonsPrefab_CN;

	public GameObject ToonsPrefab;

	private bool localized;

	private void Start()
	{
		localized = false;
	}

	private void OnEnable()
	{
		EventManager.Connect<LocalizationReloaded>(RefreshPrefabLocale);
		RefreshPrefabLocale(new LocalizationReloaded(Singleton<Localizer>.Instance.CurrentLocale));
	}

	private void OnDisable()
	{
		EventManager.Disconnect<LocalizationReloaded>(RefreshPrefabLocale);
	}

	private void updateToons(GameObject from, GameObject to, string name)
	{
		GameObject obj = Object.Instantiate(to, from.transform.position, from.transform.rotation);
		obj.transform.parent = from.transform.parent;
		obj.transform.name = name;
		Object.Destroy(from);
	}

	private void RefreshPrefabLocale(LocalizationReloaded localeChange)
	{
		MonoBehaviour.print("== Language changed: " + localeChange);
		MonoBehaviour.print("locale: " + localeChange.currentLanguage);
		if (localeChange.currentLanguage == "zh-CN")
		{
			updateToons(GameObject.Find("Toons"), ToonsPrefab_CN, "Toons");
			localized = true;
		}
		else if (localized)
		{
			updateToons(GameObject.Find("Toons"), ToonsPrefab, "Toons");
			localized = false;
		}
	}

	private void Update()
	{
	}
}
