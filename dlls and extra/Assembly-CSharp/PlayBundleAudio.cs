using UnityEngine;

public class PlayBundleAudio : WPFMonoBehaviour
{
	[SerializeField]
	private BundleDataObject bundleAudio;

	[SerializeField]
	private bool playOnStart;

	private void Start()
	{
		if (playOnStart)
		{
			Play2dEffect();
		}
	}

	public void Play2dEffect()
	{
		AudioSource loadedAudioSource = WPFMonoBehaviour.gameData.commonAudioCollection.GetLoadedAudioSource(bundleAudio);
		if (loadedAudioSource != null)
		{
			Singleton<AudioManager>.Instance.Play2dEffect(loadedAudioSource);
		}
	}
}
