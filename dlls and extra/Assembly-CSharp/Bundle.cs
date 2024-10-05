using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bundle : MonoBehaviour
{
	public class BundleObject
	{
		private string bundleId;

		private string bundleFileExtension;

		private string bundleLocation;

		private bool loadAtStart;

		private AssetBundle assetBundle;

		public string BundleFileName => bundleId + "." + bundleFileExtension;

		public string BundleId => bundleId;

		public string BundleLocation => bundleLocation;

		public bool LoadAtStart => loadAtStart;

		public bool IsAssetBundleInMemory => assetBundle != null;

		public AssetBundle LoadedAssetBundle => assetBundle;

		public BundleObject(string newBundleId, string newBundleFileExtension, bool newLoadAtStart, string newBundleLocation = "")
		{
			bundleLocation = newBundleLocation;
			bundleId = newBundleId;
			bundleFileExtension = newBundleFileExtension;
			loadAtStart = newLoadAtStart;
		}

		public void SetBundleLocation(string newBundleLocation)
		{
			bundleLocation = newBundleLocation;
		}

		public void SetLoadedBundle(AssetBundle newAssetBundle)
		{
			assetBundle = newAssetBundle;
		}

		public void UnloadBundle(bool unloadAllObjects)
		{
			if (!(assetBundle == null))
			{
				assetBundle.Unload(unloadAllObjects);
				assetBundle = null;
			}
		}
	}

	public static Action AssetBundlesLoaded;

	public static Action AssetBundleLoadFailed;

	private static Bundle instance;

	private static Dictionary<string, BundleObject> bundleObjects;

	private int checkAssetBundleCount;

	public static bool checkingBundles;

	public static bool initialized;

	public List<LoadObject> assetBundles;

	public GameObject exitConfirmation;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			EventManager.Connect<LoadLevelEvent>(OnLoadLevelEvent);
			checkingBundles = true;
			if (SceneManager.GetActiveScene().name.Equals("SplashScreen"))
			{
				StartCoroutine(CheckAssetBundles());
			}
			else
			{
				StartCoroutine(DelayAssetChecking());
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private IEnumerator DelayAssetChecking()
	{
		while (!SceneManager.GetActiveScene().name.Equals("SplashScreen"))
		{
			yield return null;
		}
		yield return null;
		StartCoroutine(CheckAssetBundles());
	}

	private void OnDestroy()
	{
		if (this != instance)
		{
			return;
		}
		if (bundleObjects != null)
		{
			foreach (KeyValuePair<string, BundleObject> bundleObject in bundleObjects)
			{
				bundleObjects[bundleObject.Key].UnloadBundle(unloadAllObjects: true);
			}
		}
		EventManager.Disconnect<LoadLevelEvent>(OnLoadLevelEvent);
		bundleObjects = null;
		initialized = false;
	}

	public static void UnloadAssetBundle(string bundleId, bool unloadAllLoadedObjects)
	{
		if (bundleObjects != null && bundleObjects.TryGetValue(bundleId, out var value) && value.IsAssetBundleInMemory)
		{
			value.UnloadBundle(unloadAllLoadedObjects);
		}
	}

	private IEnumerator CheckAssetBundles(bool offlineMode = false)
	{
		if (checkAssetBundleCount == 0)
		{
			bundleObjects = new Dictionary<string, BundleObject>();
		}
		initialized = false;
		checkAssetBundleCount++;
		for (int i = 0; i < assetBundles.Count; i++)
		{
			string assetBundleId = assetBundles[i].assetBundleId;
			BundleObject bundleObject = ((!bundleObjects.ContainsKey(assetBundleId)) ? new BundleObject(assetBundleId, "unity3d", assetBundles[i].loadAtStart, string.Empty) : bundleObjects[assetBundleId]);
			string bundleLocation = Path.Combine(Application.streamingAssetsPath, "AssetBundles/" + bundleObject.BundleFileName);
			bundleObject.SetBundleLocation(bundleLocation);
			if (!bundleObjects.ContainsKey(assetBundleId))
			{
				bundleObjects.Add(assetBundleId, bundleObject);
			}
		}
		foreach (KeyValuePair<string, BundleObject> bundleObject2 in bundleObjects)
		{
			if (bundleObject2.Value.LoadAtStart)
			{
				string bundleId = bundleObject2.Key;
				StartCoroutine(LoadAssetBundle(bundleId, delegate
				{
					BundleLoaded(bundleId);
				}));
			}
		}
		checkingBundles = false;
		yield break;
	}

	private IEnumerator LoadAssetBundle(string bundleId, Action onFinish)
	{
		if (bundleObjects == null || !bundleObjects.ContainsKey(bundleId))
		{
			yield break;
		}
		BundleObject bo = bundleObjects[bundleId];
		if (bo.IsAssetBundleInMemory)
		{
			onFinish?.Invoke();
			yield break;
		}
		string bundleLocation = bo.BundleLocation;
		if (!string.IsNullOrEmpty(bundleLocation))
		{
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundleLocation);
			yield return request;
			AssetBundle assetBundle = request.assetBundle;
			if (assetBundle != null)
			{
				assetBundle.name = bo.BundleId;
				bo.SetLoadedBundle(assetBundle);
				onFinish?.Invoke();
			}
		}
	}

	public static void LoadBundleAsync(string bundleId, Action onFinish = null)
	{
		if (!IsBundleLoaded(bundleId))
		{
			instance.StartCoroutine(instance.LoadAssetBundle(bundleId, onFinish));
		}
	}

	public static bool IsBundleLoaded(string bundleId)
	{
		if (bundleObjects != null && bundleObjects.ContainsKey(bundleId))
		{
			return bundleObjects[bundleId].IsAssetBundleInMemory;
		}
		return false;
	}

	public static bool HasBundle(string bundleId)
	{
		if (bundleObjects != null && bundleObjects.ContainsKey(bundleId))
		{
			return !string.IsNullOrEmpty(bundleObjects[bundleId].BundleLocation);
		}
		return false;
	}

	public static string GetAssetBundleID(int episodeIndex)
	{
		return episodeIndex switch
		{
			1 => instance.assetBundles[2].assetBundleId, 
			2 => instance.assetBundles[3].assetBundleId, 
			3 => instance.assetBundles[1].assetBundleId, 
			4 => instance.assetBundles[4].assetBundleId, 
			5 => instance.assetBundles[5].assetBundleId, 
			_ => instance.assetBundles[0].assetBundleId, 
		};
	}

	public static T LoadObject<T>(string bundleId, string objectName) where T : UnityEngine.Object
	{
		if (string.IsNullOrEmpty(objectName))
		{
			return null;
		}
		if (bundleObjects == null)
		{
			return null;
		}
		if (!bundleObjects.ContainsKey(bundleId))
		{
			return null;
		}
		if (!bundleObjects[bundleId].IsAssetBundleInMemory)
		{
			return null;
		}
		try
		{
			return (T)bundleObjects[bundleId].LoadedAssetBundle.LoadAsset(objectName, typeof(T));
		}
		catch
		{
			return null;
		}
	}

	public static T LoadObject<T>(string objectName) where T : UnityEngine.Object
	{
		if (string.IsNullOrEmpty(objectName))
		{
			return null;
		}
		T val = null;
		foreach (KeyValuePair<string, BundleObject> bundleObject in bundleObjects)
		{
			val = LoadObject<T>(bundleObject.Key, objectName);
			if (!EqualityComparer<T>.Default.Equals(val, null))
			{
				break;
			}
		}
		return val;
	}

	private void BundleLoaded(string bundleId)
	{
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<string, BundleObject> bundleObject in bundleObjects)
		{
			if (bundleObject.Value.LoadAtStart)
			{
				num++;
			}
			if (bundleObject.Value.IsAssetBundleInMemory)
			{
				num2++;
			}
		}
		if (num == num2)
		{
			initialized = true;
			if (AssetBundlesLoaded != null)
			{
				AssetBundlesLoaded();
			}
		}
	}

	private void OnLoadLevelEvent(LoadLevelEvent data)
	{
		string levelName = data.levelName;
		if (string.IsNullOrEmpty(levelName) || (!(levelName == "MainMenu") && !(levelName == "EpisodeSelection")))
		{
			return;
		}
		for (int i = 0; i < assetBundles.Count; i++)
		{
			if (assetBundles[i].assetBundleId.StartsWith("Episode"))
			{
				UnloadAssetBundle(assetBundles[i].assetBundleId, unloadAllLoadedObjects: true);
			}
		}
	}
}
