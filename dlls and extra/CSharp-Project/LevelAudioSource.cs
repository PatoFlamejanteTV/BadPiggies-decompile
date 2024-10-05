using UnityEngine;

public class LevelAudioSource : MonoBehaviour
{
	[SerializeField]
	private LevelManager.GameState playOnGameState = LevelManager.GameState.Running;

	[SerializeField]
	private bool playOnlyOnce;

	private AudioSource baseAudioSource;

	private AudioManager audioManager;

	private bool audioPlayed;

	private void Awake()
	{
		baseAudioSource = GetComponent<AudioSource>();
		baseAudioSource.playOnAwake = false;
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChangeEvent);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChangeEvent);
	}

	private void Start()
	{
		audioManager = Singleton<AudioManager>.Instance;
	}

	private void ReceiveGameStateChangeEvent(GameStateChanged newState)
	{
		if (newState.state == playOnGameState && (!playOnlyOnce || !audioPlayed))
		{
			audioManager.Play2dEffect(baseAudioSource);
			audioPlayed = true;
		}
	}
}
