using UnityEngine;

public class LevelEndPig : MonoBehaviour
{
	public float m_minBlinkAnimationDelay;

	public float m_maxBlinkAnimationDelay;

	private float m_blinkAnimationTimer;

	private GameObject m_wrench;

	private void Start()
	{
		m_blinkAnimationTimer = Random.Range(m_minBlinkAnimationDelay, m_maxBlinkAnimationDelay);
	}

	private void Update()
	{
		m_blinkAnimationTimer -= Time.deltaTime;
		if (m_blinkAnimationTimer <= 0f)
		{
			m_blinkAnimationTimer = Random.Range(m_minBlinkAnimationDelay, m_maxBlinkAnimationDelay);
			GetComponent<SpriteAnimation>().Play("Blink");
		}
	}
}
