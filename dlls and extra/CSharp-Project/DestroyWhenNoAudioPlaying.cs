using UnityEngine;

public class DestroyWhenNoAudioPlaying : MonoBehaviour
{
	private void Update()
	{
		if ((bool)GetComponent<AudioSource>() && !GetComponent<AudioSource>().isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
