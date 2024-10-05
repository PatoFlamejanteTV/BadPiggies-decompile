using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class AudioArea : MonoBehaviour
{
	private enum TriggeringType
	{
		Pig,
		AudioListener
	}

	[SerializeField]
	private TriggeringType triggeringType;

	private BoxCollider boxCollider;

	private AudioSource areaAudioSource;

	private AudioManager audioManager;

	private void Start()
	{
		boxCollider = GetComponent<BoxCollider>();
		areaAudioSource = GetComponent<AudioSource>();
		audioManager = Singleton<AudioManager>.Instance;
	}

	private bool CheckTriggerType(ref Collider other)
	{
		if (triggeringType != TriggeringType.AudioListener || !other.GetComponent<AudioListener>())
		{
			if (triggeringType == TriggeringType.Pig)
			{
				return other.GetComponent<Pig>();
			}
			return false;
		}
		return true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (CheckTriggerType(ref other) && !areaAudioSource.isPlaying)
		{
			if (areaAudioSource.loop)
			{
				audioManager.PlayLoopingEffect(ref areaAudioSource);
			}
			else
			{
				audioManager.Play2dEffect(areaAudioSource);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (CheckTriggerType(ref other) && areaAudioSource.isPlaying && areaAudioSource.loop)
		{
			audioManager.StopLoopingEffect(areaAudioSource);
		}
	}

	private void OnDrawGizmos()
	{
		if (!boxCollider)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(base.transform.position, boxCollider.bounds.size);
	}
}
