using System;
using System.Collections.Generic;
using UnityEngine;

public class RaceTimeCounter : WPFMonoBehaviour
{
	public Color[] m_colors = new Color[3];

	private GameObject timeCounter;

	private TextMesh timeText;

	private TextMesh timeTextShadow;

	private GameObject targetTime;

	private TextMesh targetTimeText;

	private TextMesh targetTimeTextShadow;

	private bool running;

	private List<float> m_timeLimits;

	private float m_currentTargetTime;

	private GameObject m_timeSeparator;

	private void Awake()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChangeEvent);
		timeCounter = base.transform.Find("TimeCounter").gameObject;
		timeText = timeCounter.GetComponent<TextMesh>();
		timeText.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
		timeTextShadow = timeCounter.transform.Find("TimeCounterShadow").GetComponent<TextMesh>();
		targetTime = base.transform.Find("TargetTime").gameObject;
		targetTimeText = targetTime.GetComponent<TextMesh>();
		targetTimeTextShadow = targetTime.transform.Find("TargetTimeShadow").GetComponent<TextMesh>();
		m_timeSeparator = base.transform.Find("TimeSeparator").gameObject;
	}

	private void Start()
	{
		m_timeLimits = WPFMonoBehaviour.levelManager.TimeLimits;
		UpdateTargetTime();
	}

	public static string FormatTime(float time)
	{
		time = ((time <= 3599.999f) ? time : 3599.999f);
		int num = (int)time;
		int num2 = (int)((time - (float)num) * 1000f) / 10;
		int num3 = num / 60;
		num -= num3 * 60;
		return num3.ToString("D2") + ":" + num.ToString("D2") + ":" + num2.ToString("D2");
	}

	private void UpdateTargetTime()
	{
		float num = -1f;
		int num2 = 2;
		if (WPFMonoBehaviour.levelManager.TimeElapsed <= m_timeLimits[0])
		{
			num = m_timeLimits[0];
			num2 = 0;
		}
		else if (WPFMonoBehaviour.levelManager.TimeElapsed <= m_timeLimits[1])
		{
			num = m_timeLimits[1];
			num2 = 1;
		}
		if (num == m_currentTargetTime)
		{
			return;
		}
		m_currentTargetTime = num;
		num = Mathf.Min(num, 3599.99f);
		targetTimeText.GetComponent<Renderer>().material.color = m_colors[num2];
		TimeSpan timeSpan = TimeSpan.FromSeconds(num);
		string text;
		if (num >= 0f)
		{
			text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}:{timeSpan.Milliseconds / 10:D2}";
		}
		else
		{
			text = string.Empty;
			if (m_timeSeparator.activeInHierarchy)
			{
				m_timeSeparator.SetActive(value: false);
			}
		}
		targetTimeText.text = text;
		targetTimeTextShadow.text = text;
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChangeEvent);
	}

	private void ReceiveGameStateChangeEvent(GameStateChanged newState)
	{
		if (newState.state == LevelManager.GameState.Running)
		{
			running = true;
		}
		else
		{
			running = false;
		}
	}

	private void OnEnable()
	{
		if (!WPFMonoBehaviour.levelManager.m_raceLevel || !(WPFMonoBehaviour.levelManager.CurrentGameMode is BaseGameMode))
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void UpdateTime()
	{
		string text = FormatTime(WPFMonoBehaviour.levelManager.TimeElapsed);
		timeText.text = text;
		timeTextShadow.text = text;
	}

	private void Update()
	{
		if (running)
		{
			UpdateTime();
			UpdateTargetTime();
		}
	}
}
