using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVolumeFader : MonoBehaviour
{
	private enum ActiveFade
	{
		NoFade,
		FadeOut,
		FadeIn
	}

	[SerializeField]
	private float fadeTime = 1f;

	private float originalVolume = 1f;

	private float fadeStep = 1f;

	private AudioSource fadeAudioSource;

	private const float FadeCoefficient = 2.5f;

	private ActiveFade activeFade;

	private void Start()
	{
		fadeAudioSource = base.gameObject.GetComponent<AudioSource>();
		originalVolume = fadeAudioSource.volume;
		fadeStep = originalVolume / fadeTime;
		fadeAudioSource.volume = 0f;
	}

	public float FadeIn()
	{
		activeFade = ActiveFade.FadeIn;
		StartCoroutine(DoFadeIn());
		return fadeTime;
	}

	public float FadeOut()
	{
		activeFade = ActiveFade.FadeOut;
		StartCoroutine(DoFadeOut());
		return fadeTime;
	}

	private IEnumerator DoFadeOut()
	{
		while (activeFade == ActiveFade.FadeIn)
		{
			yield return new WaitForEndOfFrame();
		}
		activeFade = ActiveFade.FadeOut;
		float linearVolume = fadeAudioSource.volume;
		while (linearVolume > 0f && activeFade != ActiveFade.FadeIn)
		{
			linearVolume -= fadeStep * Time.deltaTime;
			linearVolume = Mathf.Clamp(linearVolume, 0f, originalVolume);
			fadeAudioSource.volume = linearVolume;
			yield return new WaitForEndOfFrame();
		}
		activeFade = ActiveFade.NoFade;
	}

	private IEnumerator DoFadeIn()
	{
		while (activeFade == ActiveFade.FadeOut)
		{
			yield return new WaitForEndOfFrame();
		}
		activeFade = ActiveFade.FadeIn;
		float linearVolume = fadeAudioSource.volume;
		while (linearVolume < originalVolume && activeFade != ActiveFade.FadeOut)
		{
			linearVolume += fadeStep * Time.deltaTime;
			linearVolume = Mathf.Clamp(linearVolume, 0f, originalVolume);
			fadeAudioSource.volume = linearVolume;
			yield return new WaitForEndOfFrame();
		}
		activeFade = ActiveFade.NoFade;
	}
}
