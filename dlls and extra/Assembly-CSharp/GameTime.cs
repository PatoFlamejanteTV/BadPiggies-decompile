using UnityEngine;

public class GameTime : MonoBehaviour
{
	private static GameTime m_instance;

	private static float m_realTimeDelta;

	private static float m_previousRealTime;

	private static float m_previousFixedUpdateTime;

	private static float m_fixedUpdateRealTimeDelta;

	private static bool m_isFixedUpdate;

	private static bool m_paused;

	private static bool m_prevPaused;

	private static bool m_isExternalPaused;

	public static float RealTimeDelta
	{
		get
		{
			if (m_isFixedUpdate)
			{
				return m_fixedUpdateRealTimeDelta;
			}
			return m_realTimeDelta;
		}
	}

	public static float DeltaTime => Time.deltaTime;

	public static bool IsPaused()
	{
		return m_paused;
	}

	public static void Pause(bool pause)
	{
		m_paused = pause;
		if (pause)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = INSettings.GetFloat(INFeature.TimeScale);
		}
		EventManager.Send(new GameTimePaused(m_paused));
	}

	public void ExternalPause(string msg)
	{
		if (msg == "true")
		{
			if (!m_isExternalPaused)
			{
				m_prevPaused = m_paused;
			}
			m_isExternalPaused = true;
			Pause(pause: true);
		}
		else if (msg == "false")
		{
			m_isExternalPaused = false;
			if (!m_prevPaused)
			{
				Pause(pause: false);
			}
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		m_instance = this;
	}

	private void Update()
	{
		m_isFixedUpdate = false;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		m_realTimeDelta = realtimeSinceStartup - m_previousRealTime;
		m_realTimeDelta = Mathf.Clamp(m_realTimeDelta, 0f, 1f);
		m_previousRealTime = realtimeSinceStartup;
	}

	private void FixedUpdate()
	{
		m_isFixedUpdate = true;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		m_fixedUpdateRealTimeDelta = realtimeSinceStartup - m_previousFixedUpdateTime;
		m_fixedUpdateRealTimeDelta = Mathf.Clamp(m_fixedUpdateRealTimeDelta, 0f, 1f);
		m_previousFixedUpdateTime = realtimeSinceStartup;
	}
}
