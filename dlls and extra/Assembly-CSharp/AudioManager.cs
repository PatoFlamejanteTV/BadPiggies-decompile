using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	public delegate void OnAudioMuted(bool muted);

	public enum AudioMaterial
	{
		None,
		Wood,
		Metal
	}

	private class CombinedLoopingEffect
	{
		public AudioSource prefab;

		public List<GameObject> sources = new List<GameObject>();

		public GameObject activeSound;

		public CombinedLoopingEffect(AudioSource prefab)
		{
			this.prefab = prefab;
		}

		public GameObject AddLoop(Transform soundHost)
		{
			GameObject gameObject = Singleton<AudioManager>.Instance.SpawnLoopingEffect(prefab, soundHost);
			gameObject.GetComponent<AudioSource>().Stop();
			gameObject.GetComponent<AudioSource>().enabled = false;
			sources.Add(gameObject);
			Update();
			return gameObject;
		}

		public void RemoveLoop(GameObject effect)
		{
			sources.Remove(effect);
			Singleton<AudioManager>.Instance.RemoveLoopingEffect(ref effect);
			Update();
		}

		public void Update()
		{
			GameObject gameObject = null;
			float num = 10000f;
			GameObject gameObject2 = GameObject.FindGameObjectWithTag("MainCamera");
			if (gameObject2 == null)
			{
				return;
			}
			Vector3 position = gameObject2.transform.position;
			bool flag = false;
			for (int i = 0; i < sources.Count; i++)
			{
				GameObject gameObject3 = sources[i];
				if ((bool)gameObject3)
				{
					float num2 = Vector3.Distance(gameObject3.transform.position, position) / (0.25f + gameObject3.GetComponent<AudioSource>().volume);
					if (num2 < num)
					{
						num = num2;
						gameObject = gameObject3;
					}
				}
				else
				{
					flag = true;
				}
			}
			if ((bool)activeSound)
			{
				float num3 = 0f;
				if ((bool)gameObject)
				{
					for (int j = 0; j < sources.Count; j++)
					{
						GameObject gameObject4 = sources[j];
						if (gameObject4 != null && gameObject4 != gameObject)
						{
							num3 += 0.7f * gameObject4.GetComponent<AudioSource>().volume * (4f / (4f + Vector3.Distance(gameObject.transform.position, gameObject4.transform.position)));
						}
					}
					float max = Mathf.Min(1.75f * prefab.GetComponent<AudioSource>().volume, 1f);
					activeSound.GetComponent<AudioSource>().volume = Mathf.Clamp(gameObject.GetComponent<AudioSource>().volume + num3, 0f, max);
				}
			}
			if (!gameObject)
			{
				if ((bool)activeSound)
				{
					Singleton<AudioManager>.Instance.StopLoopingEffect(activeSound.GetComponent<AudioSource>());
				}
			}
			else if (!activeSound)
			{
				activeSound = Object.Instantiate(prefab).gameObject;
				activeSound.gameObject.name = "LoopingSoundCombined-" + prefab.GetComponent<AudioSource>().clip.name;
				NoiseLevel component = activeSound.GetComponent<NoiseLevel>();
				if ((bool)component)
				{
					Object.Destroy(component);
				}
				Singleton<AudioManager>.Instance.StartLoopingEffect(activeSound.GetComponent<AudioSource>(), gameObject.transform.parent);
			}
			else if (!activeSound.GetComponent<AudioSource>().isPlaying && !Singleton<AudioManager>.Instance.Paused && activeSound.activeInHierarchy && activeSound.GetComponent<AudioSource>().enabled)
			{
				Singleton<AudioManager>.Instance.StartLoopingEffect(activeSound.GetComponent<AudioSource>(), gameObject.transform.parent);
			}
			if ((bool)gameObject && (bool)activeSound && activeSound.transform.parent != gameObject.transform.parent)
			{
				activeSound.transform.parent = gameObject.transform.parent;
				activeSound.transform.localPosition = Vector3.zero;
			}
			if (!flag)
			{
				return;
			}
			for (int k = 0; k < sources.Count; k++)
			{
				if (!sources[k])
				{
					sources.RemoveAt(k);
					break;
				}
			}
		}
	}

	private bool audioMuted;

	private Dictionary<int, float> previousPlayTimes = new Dictionary<int, float>();

	private List<AudioSource> activeLoopingSounds = new List<AudioSource>();

	private List<AudioSource> active3dOneShotSounds = new List<AudioSource>();

	private List<AudioSource> active2dOneShotSounds = new List<AudioSource>();

	private Dictionary<AudioSource, CombinedLoopingEffect> m_combinedLoops = new Dictionary<AudioSource, CombinedLoopingEffect>();

	private IEnumerator<CombinedLoopingEffect> m_combinedLoopValues;

	private List<AudioSource> m_activeMusic = new List<AudioSource>();

	private int m_counter;

	private bool m_paused;

	private const string AudioMuteKey = "AudioMuted";

	private const float AudioClipRepeatLimit = 0.15f;

	private bool m_applicationPaused;

	public bool AudioMuted => audioMuted;

	public bool Paused => m_paused;

	public static event OnAudioMuted onAudioMuted;

	public List<AudioSource> GetActiveLoopingSounds()
	{
		return activeLoopingSounds;
	}

	public List<AudioSource> GetActive3dOneShotSounds()
	{
		return active3dOneShotSounds;
	}

	public List<AudioSource> GetActive2dOneShotSounds()
	{
		return active2dOneShotSounds;
	}

	private void OnApplicationPause(bool pause)
	{
		m_applicationPaused = pause;
	}

	public void Play2dEffect(AudioClip effectClip)
	{
		if (!AudioMuted && CheckRepeatLimit(ref effectClip))
		{
			AudioSource.PlayClipAtPoint(effectClip, Vector3.zero);
		}
	}

	public AudioSource Play2dEffect(AudioSource effectSource)
	{
		return Spawn2dOneShotEffect(effectSource);
	}

	public AudioSource SpawnOneShotEffect(AudioSource[] effectSources, Vector3 soundPosition)
	{
		if (CheckRepeatLimit(ref effectSources[0]))
		{
			int num = Random.Range(0, effectSources.Length);
			return SpawnOneShotEffect(effectSources[num], soundPosition);
		}
		return null;
	}

	public AudioSource SpawnOneShotEffect(AudioSource[] effectSources, Transform sourceParent)
	{
		if (CheckRepeatLimit(ref effectSources[0]))
		{
			int num = Random.Range(0, effectSources.Length);
			return SpawnOneShotEffect(effectSources[num], sourceParent);
		}
		return null;
	}

	public void PlayLoopingEffect(ref AudioSource effectSource)
	{
		StartLoopingEffect(effectSource, null);
	}

	public AudioSource Spawn2dOneShotEffect(AudioSource effectSource)
	{
		if (base.gameObject.activeInHierarchy && !AudioMuted && active2dOneShotSounds.Count < 20)
		{
			AudioSource audioSource = Object.Instantiate(effectSource);
			audioSource.gameObject.name = "AudioOneShot -" + effectSource.name;
			audioSource.gameObject.transform.parent = base.transform;
			audioSource.Play();
			active2dOneShotSounds.Add(audioSource);
			StartCoroutine(Destroy2dOneShotEffect(audioSource));
			return audioSource;
		}
		return null;
	}

	public AudioSource SpawnOneShotEffect(AudioSource effectSource, Vector3 soundPosition)
	{
		if (base.gameObject.activeInHierarchy && active3dOneShotSounds.Count < 20)
		{
			AudioSource audioSource = Object.Instantiate(effectSource);
			audioSource.mute = AudioMuted;
			audioSource.transform.position = soundPosition;
			audioSource.gameObject.name = "AudioOneShot -" + effectSource.name;
			audioSource.gameObject.transform.parent = base.transform;
			audioSource.Play();
			active3dOneShotSounds.Add(audioSource);
			StartCoroutine(Destroy3dOneShotEffect(audioSource));
			return audioSource;
		}
		return null;
	}

	public AudioSource SpawnOneShotEffect(AudioSource effectSource, Transform sourceParent)
	{
		if (base.gameObject.activeInHierarchy && active3dOneShotSounds.Count < 20 && effectSource != null)
		{
			AudioSource audioSource = Object.Instantiate(effectSource);
			audioSource.mute = AudioMuted;
			audioSource.transform.parent = sourceParent;
			audioSource.transform.localPosition = Vector3.zero;
			audioSource.gameObject.name = "AudioOneShot -" + effectSource.name;
			audioSource.Play();
			active3dOneShotSounds.Add(audioSource);
			StartCoroutine(Destroy3dOneShotEffect(audioSource));
			return audioSource;
		}
		return null;
	}

	private IEnumerator Destroy2dOneShotEffect(AudioSource audioSource)
	{
		yield return new WaitForSeconds(audioSource.clip.length);
		if (audioSource != null)
		{
			active2dOneShotSounds.Remove(audioSource);
			Object.Destroy(audioSource.gameObject);
		}
	}

	private IEnumerator Destroy3dOneShotEffect(AudioSource audioSource)
	{
		yield return new WaitForSeconds(audioSource.clip.length);
		if (audioSource != null)
		{
			active3dOneShotSounds.Remove(audioSource);
			Object.Destroy(audioSource.gameObject);
		}
	}

	public GameObject SpawnLoopingEffect(AudioSource effectSource, Transform soundHost)
	{
		Transform transform = soundHost.Find("LoopingSound-" + effectSource.GetComponent<AudioSource>().clip.name);
		AudioSource audioSource = ((!(transform == null)) ? transform.GetComponent<AudioSource>() : Object.Instantiate(effectSource));
		audioSource.gameObject.name = "LoopingSound-" + audioSource.GetComponent<AudioSource>().clip.name;
		StartLoopingEffect(audioSource, soundHost);
		return audioSource.gameObject;
	}

	public void StopLoopingEffect(AudioSource loopingEffect)
	{
		if ((bool)loopingEffect && loopingEffect.isPlaying)
		{
			activeLoopingSounds.Remove(loopingEffect);
			AudioVolumeFader component = loopingEffect.gameObject.GetComponent<AudioVolumeFader>();
			if ((bool)component && base.gameObject.activeInHierarchy)
			{
				float delay = component.FadeOut();
				StartCoroutine(DelayedStop(loopingEffect, delay));
			}
			else
			{
				loopingEffect.Stop();
			}
		}
	}

	public GameObject SpawnCombinedLoopingEffect(AudioSource effectSource, Transform soundHost)
	{
		if (!m_combinedLoops.TryGetValue(effectSource, out var value))
		{
			value = new CombinedLoopingEffect(effectSource);
			m_combinedLoops[effectSource] = value;
			m_combinedLoopValues = m_combinedLoops.Values.GetEnumerator();
		}
		return value.AddLoop(soundHost);
	}

	public void RemoveCombinedLoopingEffect(AudioSource prefab, GameObject loopingEffect)
	{
		if (m_combinedLoops.TryGetValue(prefab, out var value))
		{
			value.RemoveLoop(loopingEffect);
		}
	}

	private IEnumerator DelayedStop(AudioSource stoppingSource, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (stoppingSource != null)
		{
			stoppingSource.Stop();
		}
	}

	public void RemoveLoopingEffect(ref GameObject loopingEffect)
	{
		if ((bool)loopingEffect && (bool)loopingEffect.GetComponent<AudioSource>())
		{
			loopingEffect.GetComponent<AudioSource>().Stop();
			activeLoopingSounds.Remove(loopingEffect.GetComponent<AudioSource>());
			loopingEffect = null;
		}
	}

	public void MuteSounds(List<AudioSource> sounds, bool mute)
	{
		foreach (AudioSource sound in sounds)
		{
			if ((bool)sound)
			{
				sound.mute = mute;
			}
		}
	}

	public void PauseSounds(List<AudioSource> sounds, bool pause)
	{
		foreach (AudioSource sound in sounds)
		{
			if ((bool)sound)
			{
				if (pause && sound.isPlaying)
				{
					sound.Pause();
				}
				else if (!pause && !sound.isPlaying && sound.gameObject.activeInHierarchy && sound.enabled)
				{
					sound.Play();
				}
			}
		}
	}

	public void AbortSounds(List<AudioSource> sounds, bool pause)
	{
		if (!pause)
		{
			return;
		}
		foreach (AudioSource sound in sounds)
		{
			if ((bool)sound)
			{
				sound.mute = true;
				sound.Stop();
			}
		}
	}

	public GameObject SpawnMusic(AudioSource musicPrefab)
	{
		GameObject gameObject = Object.Instantiate(musicPrefab.gameObject);
		gameObject.GetComponent<AudioSource>().mute = audioMuted;
		Object.DontDestroyOnLoad(gameObject);
		m_activeMusic.Add(gameObject.GetComponent<AudioSource>());
		return gameObject;
	}

	public void RemoveMusic(GameObject music)
	{
		if (music.GetComponent<AudioSource>().isPlaying)
		{
			music.GetComponent<AudioSource>().Stop();
		}
		m_activeMusic.Remove(music.GetComponent<AudioSource>());
		Object.DestroyImmediate(music);
	}

	private void Awake()
	{
		SetAsPersistant();
		m_combinedLoopValues = m_combinedLoops.Values.GetEnumerator();
		LoadAudioParams();
		EventManager.Connect<GameTimePaused>(ReceiveGameTimePaused);
		EventManager.Connect<LevelLoadedEvent>(OnLevelLoaded);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameTimePaused>(ReceiveGameTimePaused);
		EventManager.Disconnect<LevelLoadedEvent>(OnLevelLoaded);
	}

	private void OnEnable()
	{
		KeyListener.keyPressed += HandleKeyListenerkeyPressed;
	}

	private void OnDisable()
	{
		KeyListener.keyPressed -= HandleKeyListenerkeyPressed;
	}

	private void OnLevelLoaded(LevelLoadedEvent e)
	{
		AbortSounds(active2dOneShotSounds, pause: true);
		AbortSounds(active3dOneShotSounds, pause: true);
	}

	private void HandleKeyListenerkeyPressed(KeyCode obj)
	{
		if (!INSettings.GetBool(INFeature.InputSettings) && obj == KeyCode.S)
		{
			ToggleMute();
		}
	}

	private void StartLoopingEffect(AudioSource loopingSource, Transform optionalSoundHost)
	{
		loopingSource.loop = true;
		if ((bool)optionalSoundHost)
		{
			loopingSource.transform.parent = optionalSoundHost;
			loopingSource.transform.localPosition = Vector3.zero;
		}
		if (loopingSource.enabled)
		{
			AudioVolumeFader component = loopingSource.gameObject.GetComponent<AudioVolumeFader>();
			if ((bool)component)
			{
				component.FadeIn();
			}
			loopingSource.Play();
		}
		loopingSource.mute = audioMuted;
		activeLoopingSounds.Add(loopingSource);
	}

	private bool CheckRepeatLimit(ref AudioClip audioClip)
	{
		int instanceID = audioClip.GetInstanceID();
		if (!previousPlayTimes.ContainsKey(instanceID))
		{
			previousPlayTimes[instanceID] = Time.realtimeSinceStartup;
			return true;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (previousPlayTimes[instanceID] < realtimeSinceStartup - 0.15f)
		{
			previousPlayTimes[instanceID] = realtimeSinceStartup;
			return true;
		}
		return false;
	}

	private bool CheckRepeatLimit(ref AudioSource audioSource)
	{
		int instanceID = audioSource.clip.GetInstanceID();
		if (!previousPlayTimes.ContainsKey(instanceID))
		{
			previousPlayTimes[instanceID] = Time.realtimeSinceStartup;
			return true;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (previousPlayTimes[instanceID] < realtimeSinceStartup - 0.15f)
		{
			previousPlayTimes[instanceID] = realtimeSinceStartup;
			return true;
		}
		return false;
	}

	private void LoadAudioParams()
	{
		audioMuted = UserSettings.GetBool("AudioMuted");
		if (audioMuted)
		{
			AudioListener.volume = 0f;
		}
		else
		{
			AudioListener.volume = 1f;
		}
	}

	private void SaveAudioParams()
	{
		UserSettings.SetBool("AudioMuted", audioMuted);
		UserSettings.Save();
	}

	public void ToggleMute()
	{
		audioMuted = !audioMuted;
		if (audioMuted)
		{
			AudioListener.volume = 0f;
		}
		else
		{
			AudioListener.volume = 1f;
		}
		MuteSounds(activeLoopingSounds, audioMuted);
		MuteSounds(active3dOneShotSounds, audioMuted);
		MuteSounds(m_activeMusic, audioMuted);
		SaveAudioParams();
		if (AudioManager.onAudioMuted != null)
		{
			AudioManager.onAudioMuted(audioMuted);
		}
	}

	private void ReceiveGameTimePaused(GameTimePaused data)
	{
		m_paused = data.paused;
		PauseSounds(activeLoopingSounds, data.paused);
		PauseSounds(m_activeMusic, data.paused);
		PauseSounds(active3dOneShotSounds, data.paused);
		AbortSounds(active2dOneShotSounds, data.paused);
	}

	private void Update()
	{
		if (m_applicationPaused)
		{
			return;
		}
		m_counter++;
		if (m_counter > 10)
		{
			m_counter = 0;
		}
		if (m_counter == 1)
		{
			for (int i = 0; i < activeLoopingSounds.Count; i++)
			{
				if (!activeLoopingSounds[i])
				{
					activeLoopingSounds.RemoveAt(i);
					break;
				}
			}
		}
		if (m_counter == 2)
		{
			for (int j = 0; j < active3dOneShotSounds.Count; j++)
			{
				if (!active3dOneShotSounds[j])
				{
					active3dOneShotSounds.RemoveAt(j);
					break;
				}
			}
		}
		if (m_counter == 3)
		{
			for (int k = 0; k < active2dOneShotSounds.Count; k++)
			{
				if (!active2dOneShotSounds[k])
				{
					active2dOneShotSounds.RemoveAt(k);
					break;
				}
			}
		}
		m_combinedLoopValues.Reset();
		while (m_combinedLoopValues.MoveNext())
		{
			m_combinedLoopValues.Current.Update();
		}
	}
}
