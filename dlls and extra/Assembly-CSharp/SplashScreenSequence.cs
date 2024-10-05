using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenSequence : MonoBehaviour
{
	[Serializable]
	public class SplashFrame
	{
		public GameObject m_splash;

		public float m_time;
	}

	public List<SplashFrame> m_splashes = new List<SplashFrame>();

	private IEnumerator Start()
	{
		StartCoroutine(PlaySplashSequence());
		yield return null;
		StartCoroutine(RunStartUpChecks());
	}

	private GameObject LoadSplash(int id)
	{
		GameObject obj = UnityEngine.Object.Instantiate(m_splashes[id].m_splash, Vector3.zero, Quaternion.identity);
		obj.SetActive(value: true);
		return obj;
	}

	private void ReleaseSplash(GameObject splash)
	{
		splash.SetActive(value: false);
		UnityEngine.Object.Destroy(splash);
	}

	private IEnumerator PlaySplashSequence()
	{
		int currentSplash = 0;
		GameObject splash = LoadSplash(currentSplash);
		yield return new WaitForSeconds(m_splashes[currentSplash].m_time);
		while (true)
		{
			int num = currentSplash + 1;
			currentSplash = num;
			if (num < m_splashes.Count)
			{
				GameObject gameObject = LoadSplash(currentSplash);
				ReleaseSplash(splash);
				splash = gameObject;
				yield return new WaitForSeconds(m_splashes[currentSplash].m_time);
				continue;
			}
			break;
		}
	}

	private IEnumerator RunStartUpChecks()
	{
		while (!Bundle.initialized || Bundle.checkingBundles)
		{
			yield return null;
		}
		float timeout = 5f;
		while (!Singleton<GameConfigurationManager>.Instance.HasData && timeout > 0f)
		{
			timeout -= GameTime.RealTimeDelta;
			yield return null;
		}
		LoadMainMenu();
	}

	private void LoadMainMenu()
	{
		UnityEngine.Object.Destroy(this);
		Singleton<GameManager>.Instance.LoadMainMenu(showLoadingScreen: true);
	}
}
