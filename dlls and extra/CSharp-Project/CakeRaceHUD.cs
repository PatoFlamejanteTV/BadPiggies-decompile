using System;
using System.Collections;
using System.Collections.Generic;
using CakeRace;
using Spine.Unity;
using UnityEngine;

public class CakeRaceHUD : WPFMonoBehaviour
{
	public enum TimerMode
	{
		Intro,
		Normal,
		Intermediate,
		Critical,
		TimesUp
	}

	[Serializable]
	private struct TimerModePair
	{
		[SerializeField]
		private TimerMode m_mode;

		[SerializeField]
		private string m_animation;

		public TimerMode Mode => m_mode;

		public string Animation => m_animation;
	}

	[SerializeField]
	private List<TimerModePair> bombAnimations;

	[SerializeField]
	private SkeletonAnimation cakeBarAnimation;

	[SerializeField]
	private string cakeBarIntroAnimation;

	[SerializeField]
	private CakeRaceMeter[] raceMeters;

	[SerializeField]
	private SkeletonAnimation timeBombAnimation;

	[SerializeField]
	private GameObject timer;

	private MeshRenderer cakeBarRenderer;

	private CakeRaceMode cakeRaceMode;

	private TextMesh[] timerLabel;

	private const string TIMER_FORMAT = "{0:00}:{1:00}:{2:00}";

	private const string SCORE_FORMAT = "{0:n0}";

	private void Awake()
	{
		if (timer != null)
		{
			timerLabel = timer.GetComponentsInChildren<TextMesh>();
		}
		if (cakeBarAnimation != null)
		{
			cakeBarRenderer = cakeBarAnimation.GetComponent<MeshRenderer>();
		}
		EventManager.Connect<UIEvent>(OnUIEvent);
	}

	private void OnEnable()
	{
		if (WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode)
		{
			cakeRaceMode = WPFMonoBehaviour.levelManager.CurrentGameMode as CakeRaceMode;
			OnScoreUpdate(0);
			CakeRaceMode obj = cakeRaceMode;
			obj.ScoreUpdated = (Action<int>)Delegate.Combine(obj.ScoreUpdated, new Action<int>(OnScoreUpdate));
			CakeRaceMode obj2 = cakeRaceMode;
			obj2.CakeCollected = (Action<int>)Delegate.Combine(obj2.CakeCollected, new Action<int>(OnCakeCollected));
			if (raceMeters.Length != 0)
			{
				if (HatchManager.CurrentPlayer != null)
				{
					raceMeters[0].SetPlayerInfo("CAKE_RACE_YOU", Singleton<PlayerProgress>.Instance.Level, refreshTranslation: true);
				}
				else
				{
					raceMeters[0].SetPlayerInfo("EditorMode", Singleton<PlayerProgress>.Instance.Level);
				}
				raceMeters[0].ResetCakes();
			}
			if (raceMeters.Length > 1)
			{
				if (CakeRaceMode.IsPreviewMode)
				{
					raceMeters[1].SetPlayerInfo("PreviewMode", Singleton<PlayerProgress>.Instance.Level);
				}
				else if (CakeRaceMode.OpponentReplay != null)
				{
					raceMeters[1].SetPlayerInfo(CakeRaceMode.OpponentReplay.GetPlayerName(), CakeRaceMode.OpponentReplay.PlayerLevel);
				}
				raceMeters[1].ResetCakes();
			}
			StartCoroutine(CakeBarIntroSequence());
			if (!CakeRaceMode.IsPreviewMode)
			{
				StartCoroutine(PlayOpponentReplay());
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(CoroutineRunner.DelayFrames(delegate
			{
				SetTimeBombMode(TimerMode.Intro, clearAnimations: true, loopAnimation: false);
				SetTimeBombMode(TimerMode.Normal, clearAnimations: false, loopAnimation: true);
			}, 1));
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<UIEvent>(OnUIEvent);
	}

	private void InitTimeBombAnimations()
	{
		float seconds = cakeRaceMode.RaceTimeLeft / 2f;
		StartCoroutine(CoroutineRunner.DelayActionSequence(delegate
		{
			SetTimeBombMode(TimerMode.Intermediate, clearAnimations: false, loopAnimation: true);
		}, seconds, realTime: false));
		float seconds2 = cakeRaceMode.RaceTimeLeft - 10f;
		StartCoroutine(CoroutineRunner.DelayActionSequence(delegate
		{
			SetTimeBombMode(TimerMode.Critical, clearAnimations: false, loopAnimation: true);
		}, seconds2, realTime: false));
	}

	private string GetPlayerName(string playerName)
	{
		string result = "guest";
		if (!string.IsNullOrEmpty(playerName))
		{
			result = ((!playerName.Contains("|")) ? playerName : HatchManager.CurrentPlayer.PlayFabDisplayName.Split('|')[0]);
		}
		return result;
	}

	private IEnumerator PlayOpponentReplay()
	{
		if (raceMeters.Length <= 1 || CakeRaceMode.OpponentReplay == null)
		{
			yield break;
		}
		cakeRaceMode.OpponentScore = 0;
		raceMeters[1].SetScoreLabel($"{cakeRaceMode.OpponentScore:n0}");
		bool[] collected = new bool[CakeRaceMode.OpponentReplay.GetCollectedCakeCount()];
		for (int i = 0; i < collected.Length; i++)
		{
			collected[i] = false;
		}
		int collectedCount = 0;
		while (collectedCount < CakeRaceMode.OpponentReplay.GetCollectedCakeCount())
		{
			yield return null;
			for (int j = 0; j < collected.Length; j++)
			{
				int num = cakeRaceMode.RaceTimeLeftInHundrethOfSeconds();
				int cakeCollectTime = CakeRaceMode.OpponentReplay.GetCakeCollectTime(j);
				if (!collected[j] && cakeCollectTime >= 0 && cakeCollectTime >= num)
				{
					collected[j] = true;
					collectedCount++;
					cakeRaceMode.OpponentScore += CakeRaceReplay.CalculateCakeScore(explosion: false, cakeCollectTime, CakeRaceMode.OpponentReplay.PlayerLevel, CakeRaceMode.OpponentReplay.HasKingsFavoritePart);
					raceMeters[1].EatCake();
					raceMeters[1].SetScoreLabel($"{cakeRaceMode.OpponentScore:n0}");
				}
			}
		}
	}

	public void SetTimeBombMode(TimerMode mode, bool clearAnimations, bool loopAnimation)
	{
		if (clearAnimations)
		{
			timeBombAnimation.state.ClearTracks();
		}
		string empty = string.Empty;
		for (int i = 0; i < bombAnimations.Count; i++)
		{
			if (bombAnimations[i].Mode == mode)
			{
				empty = bombAnimations[i].Animation;
				break;
			}
		}
		if (!string.IsNullOrEmpty(empty))
		{
			timeBombAnimation.state.SetAnimation(timeBombAnimation.state.Tracks.Count, empty, loopAnimation);
		}
	}

	private void OnDisable()
	{
		if (cakeRaceMode != null)
		{
			CakeRaceMode obj = cakeRaceMode;
			obj.ScoreUpdated = (Action<int>)Delegate.Remove(obj.ScoreUpdated, new Action<int>(OnScoreUpdate));
			CakeRaceMode obj2 = cakeRaceMode;
			obj2.CakeCollected = (Action<int>)Delegate.Remove(obj2.CakeCollected, new Action<int>(OnCakeCollected));
		}
		StopAllCoroutines();
	}

	private IEnumerator CakeBarIntroSequence()
	{
		cakeBarRenderer.enabled = false;
		yield return null;
		if (cakeBarAnimation != null)
		{
			cakeBarAnimation.state.SetAnimation(0, cakeBarIntroAnimation, loop: false);
		}
		yield return null;
		cakeBarRenderer.enabled = true;
	}

	private void Update()
	{
		if (cakeRaceMode != null)
		{
			SetTimerLabel(cakeRaceMode.RaceTimeLeft);
		}
	}

	private void SetTimerLabel(float totalSeconds)
	{
		if (timerLabel != null && timerLabel.Length >= 2)
		{
			totalSeconds = Mathf.Clamp(totalSeconds, 0f, totalSeconds);
			int num = Mathf.FloorToInt(totalSeconds / 60f);
			int num2 = Mathf.FloorToInt(totalSeconds % 60f);
			int num3 = (int)(100f * (totalSeconds - (float)(num * 60 + num2)));
			timerLabel[0].text = $"{num:00}:{num2:00}:{num3:00}";
			timerLabel[1].text = timerLabel[0].text;
		}
	}

	private void OnScoreUpdate(int newScore)
	{
		raceMeters[0].SetScoreLabel($"{newScore:n0}");
	}

	private void OnCakeCollected(int index)
	{
		raceMeters[0].EatCake();
	}

	private void OnUIEvent(UIEvent data)
	{
		if (data.type == UIEvent.Type.CakeRaceTimerStarted)
		{
			InitTimeBombAnimations();
		}
	}
}
