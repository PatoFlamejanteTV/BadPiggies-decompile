using UnityEngine;

public class AnimationStateRestore : MonoBehaviour
{
	private Animation m_animation;

	private float m_time;

	private void OnEnable()
	{
		m_animation = GetComponent<Animation>();
		if (m_time > 0f)
		{
			m_animation[GetComponent<Animation>().clip.name].time = m_time;
			m_animation.Play();
		}
	}

	private void Update()
	{
		if (m_animation.isPlaying)
		{
			m_time = m_animation[m_animation.clip.name].time;
		}
		else
		{
			m_time = 0f;
		}
	}
}
