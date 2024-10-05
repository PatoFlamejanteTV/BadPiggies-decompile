using UnityEngine;

public class RewardTimeCounter : WPFMonoBehaviour
{
	public GameObject m_timeRewardText;

	public GameObject m_timeRewardTextShadow;

	public GameObject m_rewardVideoButtonObject;

	public GameObject m_timerBackground;

	private TextMesh m_timeText;

	private TextMesh m_timeTextShadow;

	private float m_timeReward;

	private float m_timeLeft;

	private bool m_wasStopped = true;

	private bool m_isRunning;

	private bool m_isSetuped;

	private bool m_needRestart;

	private bool m_animationRunning;

	private float m_timerBackgroundOldYPosition;

	private float m_timerBackgroundNewYPosition;

	private float m_timerBackgroundOldYScale;

	private float m_timerBackgroundNewYScale;

	private Vector3 m_timerDefaultScale;

	private float m_animationTime = 0.25f;

	private float m_elapsedAnimationTime;

	private void Awake()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChangeEvent);
		m_timeText = m_timeRewardText.GetComponent<TextMesh>();
		m_timeTextShadow = m_timeRewardTextShadow.GetComponent<TextMesh>();
	}

	private void Start()
	{
	}

	public bool IsActive()
	{
		if (base.gameObject.activeInHierarchy)
		{
			return m_isRunning;
		}
		return false;
	}

	public bool IsRunning()
	{
		if (base.gameObject.activeInHierarchy)
		{
			return m_isRunning;
		}
		return false;
	}

	public void GiveReward(bool activates)
	{
		if (!m_isSetuped)
		{
			m_timeReward = RewardVideoManager.TimeToReward;
			UpdateText(m_timeReward);
			SetupCounter();
			m_isRunning = true;
		}
		if (activates)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	private void OnEnable()
	{
		if (!base.gameObject.activeSelf || !m_isSetuped)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void ReceiveGameStateChangeEvent(GameStateChanged newState)
	{
		if (newState.state == LevelManager.GameState.Running)
		{
			if (RewardVideoManager.AddTimeRewardCounterOnLevelStart)
			{
				GiveReward(activates: true);
				RewardVideoManager.AddTimeRewardCounterOnLevelStart = false;
			}
			if (m_isSetuped && m_wasStopped && m_needRestart)
			{
				m_isRunning = true;
				m_needRestart = false;
			}
		}
		else if (newState.state == LevelManager.GameState.Building && m_isSetuped)
		{
			UpdateText(m_timeReward);
			m_timeReward = RewardVideoManager.TimeToReward;
			m_timeLeft = m_timeReward;
			Stop();
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChangeEvent);
	}

	private void Stop()
	{
		m_wasStopped = true;
		m_isRunning = false;
		m_timeLeft = 0f;
		m_needRestart = true;
		m_timeRewardText.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
	}

	public void UpdateTime()
	{
		if (WPFMonoBehaviour.levelManager.TimeStarted)
		{
			m_timeLeft -= Time.deltaTime;
			if (m_wasStopped)
			{
				m_timeLeft = m_timeReward;
				m_wasStopped = false;
				m_timeRewardText.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
			}
			if (m_timeLeft <= float.Epsilon)
			{
				m_wasStopped = true;
				m_isRunning = false;
				m_timeLeft = 0f;
				m_timeRewardText.GetComponent<Renderer>().material.color = new Color(1f, 0.4f, 0.4f);
			}
			UpdateText(m_timeLeft);
		}
	}

	private void SetupCounter()
	{
		SetTime(RewardVideoManager.TimeToReward);
		m_timerBackgroundOldYPosition = m_timerBackground.transform.localPosition.y;
		m_timerBackgroundNewYPosition = -0.88f;
		m_timerBackgroundOldYScale = m_timerBackground.transform.localScale.y;
		m_timerBackgroundNewYScale = 1.8f;
		m_timerDefaultScale = m_timeRewardText.transform.localScale;
		m_timeRewardText.transform.localScale = Vector3.zero;
		m_animationRunning = true;
		m_isSetuped = true;
	}

	private void Update()
	{
		if (m_animationRunning)
		{
			m_timerBackground.transform.localPosition = new Vector3(m_timerBackground.transform.localPosition.x, Mathf.SmoothStep(m_timerBackgroundOldYPosition, m_timerBackgroundNewYPosition, m_elapsedAnimationTime / m_animationTime), m_timerBackground.transform.localPosition.z);
			m_timerBackground.transform.localScale = new Vector3(m_timerBackground.transform.localScale.x, Mathf.SmoothStep(m_timerBackgroundOldYScale, m_timerBackgroundNewYScale, m_elapsedAnimationTime / m_animationTime), m_timerBackground.transform.localScale.z);
			m_timeRewardText.transform.localScale = Vector3.Lerp(Vector3.zero, m_timerDefaultScale, m_elapsedAnimationTime / m_animationTime);
			m_elapsedAnimationTime += Time.deltaTime;
			if (m_elapsedAnimationTime >= m_animationTime)
			{
				m_animationRunning = false;
				m_elapsedAnimationTime = 0f;
			}
		}
	}

	private void UpdateText(float time)
	{
		int num = (int)time;
		int num2 = (int)((time - (float)num) * 1000f) / 10;
		string text = "+" + num.ToString("D2") + "." + num2.ToString("D2");
		if ((bool)m_timeText)
		{
			m_timeText.text = text;
			m_timeTextShadow.text = text;
		}
	}

	public void SetTime(float timeAdd)
	{
		m_timeReward = timeAdd;
	}
}
