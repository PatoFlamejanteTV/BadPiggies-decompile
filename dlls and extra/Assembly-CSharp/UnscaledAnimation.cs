using UnityEngine;

public class UnscaledAnimation : MonoBehaviour
{
	[SerializeField]
	private Animation animation;

	private bool playing;

	private float time;

	private void Update()
	{
		if (animation.isPlaying && Time.timeScale != 1f)
		{
			animation[animation.clip.name].time = (time += Time.unscaledDeltaTime);
			animation.Sample();
		}
		if (animation[animation.clip.name].time > animation.clip.length)
		{
			animation.Stop();
			time = 0f;
		}
	}
}
