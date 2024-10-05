using UnityEngine;

public class RewardVideoButton : WPFMonoBehaviour
{
	public GameObject m_timerCounter;

	public GameObject m_previewMenu;

	public string m_timeFormat;

	private bool hideThisSession;

	private bool updateRunning = true;

	private void Awake()
	{
		if (!RewardVideoManager.AddTimeRewardOnLevelStart)
		{
			RewardVideoManager.HadRewardAlready = false;
		}
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChange);
	}

	private void Update()
	{
		if (WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Running || !updateRunning)
		{
			return;
		}
		float num = WPFMonoBehaviour.levelManager.TimeLimit - WPFMonoBehaviour.levelManager.TimeElapsed;
		if (num < 0.3f)
		{
			if (num > float.Epsilon)
			{
				base.transform.localScale = new Vector3(num / 0.3f, num / 0.3f, 1f);
				return;
			}
			hideThisSession = true;
			base.transform.localScale = Vector3.zero;
			updateRunning = false;
		}
	}

	private void AddTime()
	{
		WPFMonoBehaviour.levelManager.AddToTimeLimit(RewardVideoManager.TimeToReward);
		RewardVideoManager.AddTimeRewardOnLevelStart = false;
		RewardVideoManager.HadRewardAlready = true;
		ShowTimer();
	}

	private void ShowTimer()
	{
		if ((bool)m_timerCounter)
		{
			m_timerCounter.GetComponent<RewardTimeCounter>().GiveReward(activates: true);
		}
	}

	private void ShowButton()
	{
		SetTimeToButton();
		base.gameObject.SetActive(value: true);
	}

	private void SetTimeToButton()
	{
		Transform transform = base.gameObject.transform.Find("TimeRewardCount");
		if (!transform)
		{
			return;
		}
		Transform transform2 = transform.Find("TimeRewardCountShadow");
		if (transform != null && transform2 != null)
		{
			TextMesh component = transform.GetComponent<TextMesh>();
			TextMesh component2 = transform2.GetComponent<TextMesh>();
			if (component != null && component2 != null)
			{
				string text2 = (component.text = string.Format(m_timeFormat, RewardVideoManager.TimeToReward));
				component2.text = text2;
			}
		}
	}

	private void ReceiveGameStateChange(GameStateChanged uiEvent)
	{
		if (uiEvent.state != LevelManager.GameState.Building)
		{
			return;
		}
		hideThisSession = false;
		RewardVideoManager.TimeToReward = 2;
		if (RewardVideoManager.AddTimeRewardOnLevelStart)
		{
			RewardVideoManager.AddTimeRewardOnLevelStart = false;
			RewardVideoManager.HadRewardAlready = true;
			RewardVideoManager.AddTimeRewardCounterOnLevelStart = true;
			if (GameTime.IsPaused())
			{
				GameTime.Pause(pause: false);
			}
			AddTime();
		}
		else if (!base.gameObject.activeInHierarchy && IsAllowedToShow())
		{
			ShowButton();
		}
	}

	public bool IsAllowedToShow()
	{
		if (WPFMonoBehaviour.levelManager.TimeLimit > 0f && !WPFMonoBehaviour.levelManager.m_raceLevel && !RewardVideoManager.HadRewardAlready)
		{
			return !WPFMonoBehaviour.levelManager.IsTimeChallengesCompleted();
		}
		return false;
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChange);
		RewardVideoManager.AddTimeRewardCounterOnLevelStart = false;
	}

	public void AddRewardOnRetryLevel()
	{
		if (RewardVideoManager.HadRewardAlready)
		{
			RewardVideoManager.AddTimeRewardOnLevelStart = true;
		}
	}

	public void StartRewardVideo()
	{
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Completed)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			RewardVideoManager.HadRewardAlready = true;
			if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Completed)
			{
				AddRewardOnRetryLevel();
				EventManager.Send(new UIEvent(UIEvent.Type.ReplayLevel));
				return;
			}
			AddTime();
			if ((bool)m_previewMenu)
			{
				PreviewMenu component = m_previewMenu.GetComponent<PreviewMenu>();
				if ((bool)component)
				{
					component.UpdateChallenges();
				}
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		if (!hideThisSession)
		{
			base.transform.localScale = Vector3.one;
		}
		updateRunning = true;
		if (!IsAllowedToShow())
		{
			base.gameObject.SetActive(value: false);
		}
		if (RewardVideoManager.HadRewardAlready)
		{
			ShowTimer();
		}
		SetTimeToButton();
	}

	private void Start()
	{
		if (RewardVideoManager.HadRewardAlready)
		{
			ShowTimer();
		}
		if (WPFMonoBehaviour.levelManager.TimeLimit <= float.Epsilon || WPFMonoBehaviour.levelManager.m_raceLevel || RewardVideoManager.HadRewardAlready || WPFMonoBehaviour.levelManager.IsTimeChallengesCompleted())
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			SetTimeToButton();
		}
	}
}
