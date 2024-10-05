using System.Collections;
using UnityEngine;

public class AudioTimeRandomizer : MonoBehaviour
{
	[SerializeField]
	private float minDelay = 1f;

	[SerializeField]
	private float maxDelay = 2f;

	[SerializeField]
	private bool is3DSound;

	private AudioSource baseAudioSource;

	private AudioManager audioManager;

	private bool doRandomize = true;

	private void Start()
	{
		baseAudioSource = GetComponent<AudioSource>();
		audioManager = Singleton<AudioManager>.Instance;
		StartCoroutine(SpawnAudios());
	}

	private void OnDestroy()
	{
		doRandomize = false;
	}

	private IEnumerator SpawnAudios()
	{
		while (doRandomize)
		{
			yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
			if (is3DSound)
			{
				audioManager.SpawnOneShotEffect(baseAudioSource, base.transform);
			}
			else
			{
				audioManager.Play2dEffect(baseAudioSource);
			}
		}
	}
}
