using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicPlayer : MonoBehaviour
{
	public enum Type
	{
		EpisodeStart,
		EpisodeEnd,
		EpisodeOneTime,
		EpisodeContinue,
		DailyChallenge,
		CakeRace
	}

	[Serializable]
	public class ComicStrip
	{
		public GameObject m_strip;

		public float m_speed = 2f;

		public float m_fadingSpeed = 1f;
	}

	[Serializable]
	public class SoundEffect
	{
		public float time;

		public AudioSource source;

		public float volume = 1f;
	}

	public AudioSource m_soundTrack;

	public GameObject m_continueButton;

	private float m_startPos_3_2;

	private float m_startPos;

	private int m_soundIndex;

	private float m_time;

	public int m_ContinueToLevelIndex;

	public bool m_muteDefaultMusic;

	public List<ComicStrip> m_comicStrips = new List<ComicStrip>();

	[SerializeField]
	private Type m_cutsceneType;

	[SerializeField]
	private List<SoundEffect> m_soundEffects = new List<SoundEffect>();

	private int m_currentStrip;

	public GameObject m_crossPromoConfirmationPopup;

	private IEnumerator Start()
	{
		for (int i = 0; i < m_comicStrips.Count; i++)
		{
			ComicStrip comicStrip = m_comicStrips[i];
			float x = comicStrip.m_strip.GetComponent<UnmanagedSprite>().Size.x;
			m_startPos = -10f * (float)Screen.width / (float)Screen.height + 0.5f * x + 4f * (float)Screen.width / (float)Screen.height;
			m_startPos_3_2 = -15f + 0.5f * x + 6f;
			comicStrip.m_strip.transform.position = new Vector3(m_startPos, 0f, i);
			float num = 0f;
			m_comicStrips[i].m_strip.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, num));
			FadeChildren(num, m_comicStrips[i], activateChild: false);
		}
		m_continueButton.SetActive(GameProgress.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_played") == 1);
		if (ScreenPlacement.IsAspectRatioNarrowerThan(4f, 3f))
		{
			m_continueButton.transform.Translate(-2.5f, 0f, 0f);
		}
		StartCoroutine(UpdateStrips());
		yield return null;
		if ((bool)m_soundTrack && m_currentStrip == 0)
		{
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(m_soundTrack);
		}
		for (int j = 0; j < m_currentStrip; j++)
		{
			m_time += GetComicPageTime(j);
		}
	}

	private float GetComicPageTime(int index)
	{
		float x = m_comicStrips[index].m_strip.GetComponent<UnmanagedSprite>().Size.x;
		float num = -0.15f * x;
		if (ScreenPlacement.IsAspectRatioNarrowerThan(4f, 3f))
		{
			num -= 2.5f;
		}
		float num2 = (m_startPos - num) / (m_startPos_3_2 - num) * m_comicStrips[index].m_speed;
		float num3 = Mathf.Abs(m_startPos - num) / num2;
		float num4 = 2f * (1f / m_comicStrips[index].m_fadingSpeed);
		return num3 + num4;
	}

	private IEnumerator UpdateStrips()
	{
		float alpha = 0f;
		while (m_currentStrip < m_comicStrips.Count)
		{
			float x = m_comicStrips[m_currentStrip].m_strip.GetComponent<UnmanagedSprite>().Size.x;
			float num = -0.15f * x;
			float speedFactor = (m_startPos - num) / (m_startPos_3_2 - num);
			if (ScreenPlacement.IsAspectRatioNarrowerThan(4f, 3f))
			{
				num -= 2.5f;
			}
			if (m_comicStrips[m_currentStrip].m_strip.transform.position.x <= num)
			{
				m_comicStrips[m_currentStrip].m_strip.transform.position = new Vector3(num, 0f);
				if (m_currentStrip < m_comicStrips.Count - 1)
				{
					while (alpha > 0f)
					{
						alpha -= Time.deltaTime * m_comicStrips[m_currentStrip].m_fadingSpeed;
						m_comicStrips[m_currentStrip].m_strip.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
						FadeChildren(alpha, m_comicStrips[m_currentStrip], activateChild: false);
						yield return new WaitForEndOfFrame();
					}
					UnityEngine.Object.Destroy(m_comicStrips[m_currentStrip].m_strip);
				}
				m_comicStrips[m_currentStrip] = null;
				m_currentStrip++;
			}
			else
			{
				while (alpha < 1f)
				{
					alpha += Time.deltaTime * m_comicStrips[m_currentStrip].m_fadingSpeed;
					m_comicStrips[m_currentStrip].m_strip.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
					FadeChildren(alpha, m_comicStrips[m_currentStrip], activateChild: true);
					yield return new WaitForEndOfFrame();
				}
				m_comicStrips[m_currentStrip].m_strip.transform.position -= Vector3.right * speedFactor * m_comicStrips[m_currentStrip].m_speed * Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}
		m_continueButton.SetActive(value: true);
	}

	private void FadeChildren(float alpha, ComicStrip currentStrip, bool activateChild)
	{
		for (int i = 0; i < currentStrip.m_strip.transform.childCount; i++)
		{
			Transform child = currentStrip.m_strip.transform.GetChild(i);
			if (activateChild)
			{
				child.gameObject.SetActive(value: true);
			}
			child.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
		}
	}

	private void Update()
	{
		if (GuiManager.GetPointer().up)
		{
			m_continueButton.SetActive(value: true);
		}
		UpdateSounds();
		m_time += Time.deltaTime;
	}

	private void UpdateSounds()
	{
		if (m_soundIndex >= m_soundEffects.Count)
		{
			return;
		}
		SoundEffect soundEffect = m_soundEffects[m_soundIndex];
		if (!(m_time >= soundEffect.time))
		{
			return;
		}
		if (!(soundEffect.source == null))
		{
			AudioSource audioSource = Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(soundEffect.source);
			if ((bool)audioSource)
			{
				audioSource.volume = soundEffect.volume;
			}
		}
		m_soundIndex++;
	}

	public void Continue()
	{
		if (m_cutsceneType != 0 && m_cutsceneType != Type.CakeRace && LevelInfo.CanAdUnlockNextLevel())
		{
			Singleton<GameManager>.Instance.LoadLevelSelection(Singleton<GameManager>.Instance.CurrentEpisode, showLoadingScreen: false);
			return;
		}
		switch (m_cutsceneType)
		{
		case Type.EpisodeStart:
			if (GameProgress.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_played") == 1)
			{
				Singleton<GameManager>.Instance.LoadLevelSelection(Singleton<GameManager>.Instance.CurrentEpisode, showLoadingScreen: false);
			}
			else
			{
				Singleton<GameManager>.Instance.LoadLevel(0);
			}
			break;
		case Type.EpisodeEnd:
			Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: false);
			break;
		case Type.EpisodeOneTime:
			Singleton<GameManager>.Instance.LoadLevel(Singleton<GameManager>.Instance.CurrentLevel);
			break;
		case Type.EpisodeContinue:
			if (Singleton<GameManager>.Instance.IsCutsceneStartedFromLevelSelection || (Singleton<BuildCustomizationLoader>.Instance.IsChina && GameProgress.GetInt("scenario_31_stars") != 3))
			{
				Singleton<GameManager>.Instance.LoadLevelSelection(Singleton<GameManager>.Instance.CurrentEpisode, showLoadingScreen: false);
			}
			else if (LevelInfo.ValidLevelIndex(Singleton<GameManager>.Instance.CurrentEpisodeIndex, Singleton<GameManager>.Instance.NextLevel()))
			{
				Singleton<GameManager>.Instance.LoadNextLevelAfterCutScene();
			}
			else
			{
				Singleton<GameManager>.Instance.LoadLevelSelection(Singleton<GameManager>.Instance.CurrentEpisode, showLoadingScreen: false);
			}
			break;
		case Type.DailyChallenge:
			Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: false);
			break;
		case Type.CakeRace:
			Singleton<GameManager>.Instance.LoadCakeRaceMenu();
			break;
		}
		GameProgress.SetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_played", 1);
	}

	public void CrossPromoLink()
	{
		Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.MajorLazerMusic);
	}

	public void ShowConfirmationPopup()
	{
		if (!Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			m_crossPromoConfirmationPopup.SetActive(value: true);
			SendStandardFlurryEvent("Music Promo dialog opened", "Music Promo Dialog Opened");
		}
	}

	public void DismissConfirmationPopup()
	{
		m_crossPromoConfirmationPopup.SetActive(value: false);
		SendStandardFlurryEvent("Music Promo dialog closed", "Music Promo Dialog Closed");
	}

	public void SendStandardFlurryEvent(string eventName, string id)
	{
	}
}
