using System.Collections;
using UnityEngine;

public class SplashPlayer : MonoBehaviour
{
	[SerializeField]
	private GameObject singletonSpawnerPrefab;

	private IEnumerator Start()
	{
		if (INUnity.Enabled)
		{
			INInitializer initializer = Object.Instantiate(Resources.Load<GameObject>("Innovation/INInitializer")).GetComponent<INInitializer>();
			while (!initializer.Initialized)
			{
				yield return null;
			}
			Object.Instantiate(singletonSpawnerPrefab);
		}
		else
		{
			Object.Instantiate(singletonSpawnerPrefab);
			while (!SingletonSpawner.SpawnDone)
			{
				yield return null;
			}
			StartSplash();
		}
	}

	private void StartSplash()
	{
		string arg;
		if (!Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			arg = ((!Singleton<BuildCustomizationLoader>.Instance.IsHDVersion) ? "iPhone" : ((DeviceInfo.ActiveDeviceFamily != 0 && DeviceInfo.ActiveDeviceFamily != DeviceInfo.DeviceFamily.Android && DeviceInfo.ActiveDeviceFamily != DeviceInfo.DeviceFamily.BB10) ? "PC-OSX" : "iPad"));
		}
		else
		{
			string currentLocale = Singleton<Localizer>.Instance.CurrentLocale;
			if (Singleton<BuildCustomizationLoader>.Instance.CustomerID == "chinatelecom" || Singleton<BuildCustomizationLoader>.Instance.CustomerID == "chinamobile")
			{
				if (currentLocale == "zh-CN")
				{
					MonoBehaviour.print("SplashSequence_China_CN");
				}
				else
				{
					MonoBehaviour.print("SplashSequence_China");
				}
			}
			if (currentLocale == "zh-CN")
			{
				MonoBehaviour.print("SplashSequence_Talkweb_CN");
				arg = "Talkweb_CN";
			}
			else
			{
				MonoBehaviour.print("SplashSequence_PC-OSX ");
				arg = "PC-OSX";
			}
		}
		Object.Instantiate(Resources.Load<SplashScreenSequence>($"Splashes/Sequences/SplashSequence_{arg}"));
	}
}
