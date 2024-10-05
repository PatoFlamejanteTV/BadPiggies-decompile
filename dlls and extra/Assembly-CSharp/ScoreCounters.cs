using UnityEngine;

public class ScoreCounters : WPFMonoBehaviour
{
	public GameObject rewardTimeCounterObject;

	private RewardTimeCounter rewardTimeCounter;

	private GameObject timeCounter;

	private TextMesh timeTextShadow;

	private TextMesh timeText;

	private bool running;

	private float lastTimeUpdate;

	private const bool showScoreFloaters = false;

	private bool isTimerRed;

	private void Awake()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChangeEvent);
		timeCounter = GameObject.Find("ScoreCounters/TimeCounter");
		timeText = GameObject.Find("ScoreCounters/TimeCounter/TimeCounter").GetComponent<TextMesh>();
		timeTextShadow = GameObject.Find("ScoreCounters/TimeCounter/TimeCounter/TimeCounterShadow").GetComponent<TextMesh>();
		rewardTimeCounter = rewardTimeCounterObject.GetComponent<RewardTimeCounter>();
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
		if ((bool)WPFMonoBehaviour.levelManager && (WPFMonoBehaviour.levelManager.TimeLimit == 0f || WPFMonoBehaviour.levelManager.m_raceLevel))
		{
			timeCounter.SetActive(value: false);
			return;
		}
		timeCounter.SetActive(value: true);
		if (rewardTimeCounter.IsActive())
		{
			UpdateTime(WPFMonoBehaviour.levelManager.OriginalTimeLimit);
		}
		else
		{
			UpdateTime(WPFMonoBehaviour.levelManager.OriginalTimeLimit - WPFMonoBehaviour.levelManager.TimeElapsed);
		}
	}

	private void UpdateTime(float timeLeft)
	{
		timeLeft = Mathf.Clamp(timeLeft, -99.999f, 99.999f);
		string text = " ";
		if (timeLeft < 0f)
		{
			timeLeft = 0f - timeLeft;
			text = "+";
			if (!isTimerRed)
			{
				timeText.GetComponent<Renderer>().material.color = new Color(1f, 0.4f, 0.4f);
				isTimerRed = true;
			}
		}
		else if (isTimerRed)
		{
			timeText.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
			isTimerRed = false;
		}
		int num = (int)timeLeft;
		int num2 = (int)((timeLeft - (float)num) * 1000f) / 10;
		string text2 = text + num.ToString("D2") + "." + num2.ToString("D2");
		timeText.text = text2;
		timeTextShadow.text = text2;
	}

	private void Update()
	{
		if (!running || !timeCounter.activeSelf)
		{
			return;
		}
		float time = Time.time;
		if (time - lastTimeUpdate > 0.03f)
		{
			if (rewardTimeCounter != null && rewardTimeCounter.IsActive())
			{
				rewardTimeCounter.UpdateTime();
				return;
			}
			lastTimeUpdate = time;
			UpdateTime(WPFMonoBehaviour.levelManager.TimeLimit - WPFMonoBehaviour.levelManager.TimeElapsed);
		}
	}
}
