using UnityEngine;

public class PigMechanic : MonoBehaviour
{
	public float m_minWrenchAnimationDelay;

	public float m_maxWrenchAnimationDelay;

	public float m_minBlinkAnimationDelay;

	public float m_maxBlinkAnimationDelay;

	private float m_wrenchAnimationTimer;

	private float m_blinkAnimationTimer;

	private GameObject m_pig;

	private SpriteAnimation m_blinkAnimation;

	private GameObject m_wrench;

	private void Start()
	{
		m_wrenchAnimationTimer = Random.Range(m_minWrenchAnimationDelay, m_minWrenchAnimationDelay);
		m_blinkAnimationTimer = Random.Range(m_minBlinkAnimationDelay, m_maxBlinkAnimationDelay);
		m_pig = base.transform.Find("Pig").gameObject;
		m_blinkAnimation = m_pig.GetComponent<SpriteAnimation>();
		m_wrench = base.transform.Find("Wrench").gameObject;
	}

	public void SetTime(float time)
	{
		m_wrench.GetComponent<Animation>()["PigMechanicWrench"].time = time;
		m_pig.GetComponent<Animation>()["PigMechanic"].time = time;
	}

	public void Play()
	{
		m_wrench.GetComponent<Animation>().Play();
		m_pig.GetComponent<Animation>().Play();
	}

	private void Update()
	{
		m_wrenchAnimationTimer -= Time.deltaTime;
		m_blinkAnimationTimer -= Time.deltaTime;
		if (m_wrenchAnimationTimer <= 0f)
		{
			m_wrench.GetComponent<Animation>().Play();
			m_pig.GetComponent<Animation>().Play();
			m_wrenchAnimationTimer = Random.Range(m_minWrenchAnimationDelay, m_minWrenchAnimationDelay);
		}
		if (m_blinkAnimationTimer <= 0f)
		{
			m_blinkAnimationTimer = Random.Range(m_minBlinkAnimationDelay, m_maxBlinkAnimationDelay);
			m_blinkAnimation.Play("Blink");
		}
	}
}
