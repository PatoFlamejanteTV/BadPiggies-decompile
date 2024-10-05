using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INInitializer : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> m_splashes;

	[SerializeField]
	private List<GameObject> m_prefabs;

	[SerializeField]
	private INGameData m_gameData;

	private bool m_initialized;

	private float m_time;

	public bool Initialized => m_initialized;

	private void Awake()
	{
		m_time = 0.5f;
		StartCoroutine(Initialize());
	}

	private IEnumerator Initialize()
	{
		for (int i = 0; i < m_splashes.Count; i++)
		{
			GameObject splash = Object.Instantiate(m_splashes[i], Vector3.zero, Quaternion.identity);
			yield return new WaitForSeconds(m_time);
			Object.Destroy(splash);
		}
		INUnity.Initialize(m_gameData);
		foreach (GameObject prefab in m_prefabs)
		{
			Object.Instantiate(prefab);
		}
		while (!INSettings.VersionSelected)
		{
			yield return null;
		}
		m_initialized = true;
		yield return LoadMainMenu();
	}

	private IEnumerator LoadMainMenu()
	{
		while (!SingletonSpawner.SpawnDone)
		{
			yield return null;
		}
		while (!Bundle.initialized || Bundle.checkingBundles || !Singleton<GameConfigurationManager>.Instance.HasData)
		{
			yield return null;
		}
		PostInitialize();
		Singleton<GameManager>.Instance.LoadMainMenu(showLoadingScreen: true);
	}

	private void PostInitialize()
	{
		if (INSettings.GetBool(INFeature.RuntimeGameData))
		{
			Object.Instantiate(INUnity.LoadGameObject("INRuntimeGameData"));
		}
	}
}
