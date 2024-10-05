using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public enum MusicStartOption
	{
		StartFromBeginning,
		StartFromPreviousPosition
	}

	private class MusicChange
	{
		public float time;

		public AudioSource music;

		public float fadeInTime;

		public MusicStartOption option;

		public MusicChange(float time, AudioSource music, float fadeInTime, MusicStartOption option)
		{
			this.time = time;
			this.music = music;
			this.fadeInTime = fadeInTime;
			this.option = option;
		}
	}

	[SerializeField]
	private CommonAudio commonAudio;

	private List<MusicChange> m_requestedMusic = new List<MusicChange>();

	private GameObject m_music;

	private GameObject m_musicPrefab;

	private int m_musicPrefabInstanceID;

	private Dictionary<int, float> m_musicPositions = new Dictionary<int, float>();

	private float m_globalMusicVolume = 1f;

	private bool m_fadingOutMusic;

	private float m_fadeOutSpeed;

	private static bool isNativeMusicPlaying;

	public GameObject Music => m_music;

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		EventManager.Connect<LoadLevelEvent>(ReceiveLoadingLevelEvent);
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChanged);
		m_globalMusicVolume = UserSettings.GetFloat("MusicVolume", 1f);
	}

	private void StartMusic(AudioSource music, float delay, float fadeInTime, MusicStartOption option = MusicStartOption.StartFromBeginning)
	{
		m_requestedMusic.Clear();
		m_requestedMusic.Add(new MusicChange(Time.time + delay, music, fadeInTime, option));
	}

	private void Update()
	{
		for (int i = 0; i < m_requestedMusic.Count; i++)
		{
			if (Time.time >= m_requestedMusic[i].time)
			{
				StartMusic(m_requestedMusic[i]);
				m_requestedMusic.RemoveAt(i);
				break;
			}
		}
		if (m_fadingOutMusic && (bool)m_music)
		{
			float volume = m_music.GetComponent<AudioSource>().volume;
			volume -= Time.deltaTime * m_fadeOutSpeed;
			volume = Mathf.Clamp(volume, 0f, 1f);
			m_music.GetComponent<AudioSource>().volume = volume;
			if (volume == 0f)
			{
				m_fadingOutMusic = false;
				StopMusic();
			}
		}
	}

	private void StartMusic(MusicChange data)
	{
		m_fadingOutMusic = false;
		if (m_music != null && m_musicPrefab != data.music.gameObject)
		{
			StopMusic();
		}
		if (m_music == null)
		{
			m_music = Singleton<AudioManager>.Instance.SpawnMusic(data.music);
			m_musicPrefab = data.music.gameObject;
			m_musicPrefabInstanceID = m_musicPrefab.GetInstanceID();
			if (data.option == MusicStartOption.StartFromPreviousPosition && m_musicPositions.TryGetValue(m_musicPrefabInstanceID, out var value))
			{
				m_music.GetComponent<AudioSource>().time = value;
			}
			m_music.GetComponent<AudioSource>().volume = m_musicPrefab.GetComponent<AudioSource>().volume * m_globalMusicVolume;
			m_music.GetComponent<AudioSource>().Play();
		}
		else
		{
			m_music.GetComponent<AudioSource>().volume = m_musicPrefab.GetComponent<AudioSource>().volume * m_globalMusicVolume;
			if (!m_music.GetComponent<AudioSource>().isPlaying)
			{
				m_music.GetComponent<AudioSource>().Play();
			}
		}
	}

	private void FadeOutMusic(float time)
	{
		if ((bool)m_music)
		{
			m_fadingOutMusic = true;
			m_fadeOutSpeed = m_music.GetComponent<AudioSource>().volume / time;
		}
	}

	private void StopMusic()
	{
		if (m_music != null)
		{
			m_musicPositions[m_musicPrefabInstanceID] = m_music.GetComponent<AudioSource>().time;
			m_musicPositions.Remove(m_musicPrefabInstanceID);
			Singleton<AudioManager>.Instance.RemoveMusic(m_music);
			m_music = null;
			m_musicPrefab = null;
			m_musicPrefabInstanceID = 0;
			m_fadingOutMusic = false;
			Resources.UnloadUnusedAssets();
		}
	}

	private void ReceiveLoadingLevelEvent(LoadLevelEvent data)
	{
		if (data.currentGameState == GameManager.GameState.Level && data.nextGameState != GameManager.GameState.Level)
		{
			StopMusic();
		}
		switch (data.nextGameState)
		{
		case GameManager.GameState.MainMenu:
		{
			GameObject musicTheme = commonAudio.MusicTheme;
			StartMusic(musicTheme.GetComponent<AudioSource>(), 0f, 2.2f);
			break;
		}
		case GameManager.GameState.EpisodeSelection:
		case GameManager.GameState.LevelSelection:
		case GameManager.GameState.SandboxLevelSelection:
		{
			GameObject levelSelectionMusic = commonAudio.LevelSelectionMusic;
			StartMusic(levelSelectionMusic.GetComponent<AudioSource>(), 0f, 2.2f);
			break;
		}
		case GameManager.GameState.Level:
		{
			FadeOutMusic(0.4f);
			GameObject gameObject2 = ((!Singleton<GameManager>.Instance.OverrideBuildMusic) ? commonAudio.BuildMusic : Singleton<GameManager>.Instance.OverriddenBuildMusic);
			StartMusic(gameObject2.GetComponent<AudioSource>(), 0.6f, 2.2f);
			break;
		}
		case GameManager.GameState.Cutscene:
			FadeOutMusic(0.4f);
			break;
		case GameManager.GameState.StarLevelCutscene:
			FadeOutMusic(0.4f);
			break;
		case GameManager.GameState.KingPigFeeding:
		{
			FadeOutMusic(0.4f);
			GameObject gameObject = ((!Singleton<TimeManager>.IsInstantiated() || !Singleton<TimeManager>.Instance.Initialized || Singleton<TimeManager>.Instance.CurrentTime.Month != 12) ? commonAudio.FeedingMusic : commonAudio.XmasThemeSong);
			StartMusic(gameObject.GetComponent<AudioSource>(), 0.6f, 2.2f);
			break;
		}
		case GameManager.GameState.WorkShop:
			FadeOutMusic(0.4f);
			StartMusic(commonAudio.craftAmbience, 0.6f, 2.2f);
			break;
		case GameManager.GameState.CakeRaceMenu:
		{
			FadeOutMusic(0.4f);
			GameObject cakeRaceTheme = commonAudio.CakeRaceTheme;
			StartMusic(cakeRaceTheme.GetComponent<AudioSource>(), 0.6f, 2.2f);
			break;
		}
		case GameManager.GameState.CheatsPanel:
		case GameManager.GameState.RaceLevelSelection:
			break;
		}
	}

	private void ReceiveGameStateChanged(GameStateChanged data)
	{
		switch (data.state)
		{
		case LevelManager.GameState.Running:
		{
			GameObject gameObject2 = ((!Singleton<GameManager>.Instance.OverrideInFlightMusic) ? commonAudio.InFlightMusic : Singleton<GameManager>.Instance.OverriddenInFlightMusic);
			StartMusic(gameObject2.GetComponent<AudioSource>(), 0f, 0.2f);
			break;
		}
		case LevelManager.GameState.CakeRaceCompleted:
			FadeOutMusic(0.5f);
			break;
		case LevelManager.GameState.Building:
		{
			GameObject gameObject = ((!Singleton<GameManager>.Instance.OverrideBuildMusic) ? commonAudio.BuildMusic : Singleton<GameManager>.Instance.OverriddenBuildMusic);
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if ((bool)m_musicPrefab && m_musicPrefab.name != gameObject.name)
			{
				FadeOutMusic(0.5f);
				StartMusic(component, 0.7f, 0.2f, MusicStartOption.StartFromPreviousPosition);
			}
			else
			{
				StartMusic(component, 0f, 0.2f, MusicStartOption.StartFromPreviousPosition);
			}
			break;
		}
		case LevelManager.GameState.Completed:
			FadeOutMusic(0.5f);
			break;
		}
		CheckNativeMusicPlayer();
	}

	private void CheckNativeMusicPlayer()
	{
		bool num = isNativeMusicPlaying;
		isNativeMusicPlaying = IsNativeMusicPlaying();
		if (num != isNativeMusicPlaying)
		{
			NativeMusicStateChanged(isNativeMusicPlaying);
		}
	}

	private void NativeMusicStateChanged(bool isPlaying)
	{
		if (isPlaying)
		{
			m_globalMusicVolume = 0f;
		}
		else
		{
			m_globalMusicVolume = 1f;
		}
		if ((bool)m_music && (bool)m_musicPrefab)
		{
			m_music.GetComponent<AudioSource>().volume = m_musicPrefab.GetComponent<AudioSource>().volume * m_globalMusicVolume;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			CheckNativeMusicPlayer();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		AudioListener.pause = !focus;
		if (focus)
		{
			CheckNativeMusicPlayer();
		}
	}

	private static bool _IsMusicPlaying()
	{
		return false;
	}

	public static bool IsNativeMusicPlaying()
	{
		return _IsMusicPlaying();
	}
}
