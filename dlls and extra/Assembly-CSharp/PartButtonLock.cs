using UnityEngine;

public class PartButtonLock : MonoBehaviour
{
	private Animation m_animation;

	private float m_time;

	private void OnEnable()
	{
		m_animation = GetComponent<Animation>();
		m_animation[GetComponent<Animation>().clip.name].time = m_time;
		m_animation.Play();
	}

	private void Update()
	{
		if (m_animation.isPlaying)
		{
			m_time = m_animation[m_animation.clip.name].time;
		}
	}
}
