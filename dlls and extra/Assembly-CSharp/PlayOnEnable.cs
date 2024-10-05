using UnityEngine;

public class PlayOnEnable : MonoBehaviour
{
	public string animName;

	public AudioSource soundEffect2d;

	private void OnEnable()
	{
		if ((bool)GetComponent<Animation>() && !string.IsNullOrEmpty(animName))
		{
			GetComponent<Animation>().Play(animName);
		}
		if ((bool)GetComponent<ParticleSystem>())
		{
			GetComponent<ParticleSystem>().Play();
		}
		if ((bool)soundEffect2d)
		{
			Singleton<AudioManager>.Instance.Play2dEffect(soundEffect2d);
		}
	}
}
