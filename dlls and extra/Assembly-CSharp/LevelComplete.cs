using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class LevelComplete : WPFMonoBehaviour
{
	[Flags]
	public enum CoinsCollected
	{
		None = 0,
		Challenge1 = 1,
		Challenge2 = 2,
		Challenge3 = 4
	}

	private enum NextButtonState
	{
		NextLevelButton,
		CutsceneButton,
		UnlockNextLevelButton,
		None
	}

	private const string RACELEVEL_BUNDLE = "Episode_Race_Levels";

	[SerializeField]
	private GameObject m_starOne;

	[SerializeField]
	private GameObject m_starTwo;

	[SerializeField]
	private GameObject m_starThree;

	private GoalChallenge m_goal;

	private List<Challenge> m_challenges = new List<Challenge>();

	[SerializeField]
	private ObjectiveSlot m_objectiveOne;

	[SerializeField]
	private ObjectiveSlot m_objectiveTwo;

	[SerializeField]
	private ObjectiveSlot m_objectiveThree;

	[SerializeField]
	private GameObject m_controlsPanel;

	private Vector3 m_controlsPosition;

	[SerializeField]
	private GridLayout m_buttonGridLayout;

	private Vector3 m_hintHidePosition;

	private Vector3 m_hintPosition;

	private Vector3 m_controlsHidePosition;

	[SerializeField]
	private GameObject m_starPanel;

	private Vector3 m_starPanelPosition;

	private Vector3 m_starPanelHidePosition;

	[SerializeField]
	private GameObject m_background;

	private Vector3 m_backgroundPosition;

	private Vector3 m_backgroundHidePosition;

	private BasePart.PartType m_unlockedPart;

	private BasePart.PartType m_unlockedRaceLevelPart;

	private RaceLevels.LevelUnlockablePartsData m_raceLevelNumber;

	private bool m_sandboxUnlocked;

	[SerializeField]
	private GameObject m_episodeThreeStarStamp;

	private GameObject m_music;

	private bool m_jokerLevelUnlocked;

	private bool m_hasDoubleRewards;

	private bool challenge1CoinsCollected;

	private CoinsCollected coinsCollected;

	[SerializeField]
	private GameObject[] episodeTitles;

	[SerializeField]
	private GameObject roadHogsTitle;

	[SerializeField]
	private DoubleRewardButton doubleRewardButton;

	[SerializeField]
	private SkeletonAnimation doubleRewardAnimation;

	[SerializeField]
	private ExtraCoinsRewardButton extraCoinsRewardButton;

	private bool bShakeCamera;

	public GameObject homeButton;

	private float originalHomeButtonX;

	public GameObject controlsPanel;

	public GameObject rewardVideoButton;

	private float originalBackgroundScaleX;

	private bool hasTransformed;

	public GameObject pigFeedButton;

	public CoinsCollected CoinsCollectedNow => coinsCollected;

	public void SetGoal(GoalChallenge challenge)
	{
		m_goal = challenge;
	}

	public void SetChallenges(List<Challenge> challenges)
	{
		m_challenges = challenges;
	}

	public void OpenObjectiveTutorial(string slot)
	{
		int num = int.Parse(slot) - 2;
		if (num >= 0 && num <= 1)
		{
			WPFMonoBehaviour.levelManager.m_levelCompleteTutorialBookPagePrefab = m_challenges[num].m_tutorialBookPage;
			EventManager.Send(new UIEvent(UIEvent.Type.OpenTutorial));
		}
		else
		{
			WPFMonoBehaviour.levelManager.m_levelCompleteTutorialBookPagePrefab = m_goal.TutorialPage;
			EventManager.Send(new UIEvent(UIEvent.Type.OpenTutorial));
		}
	}

	private void OnEnable()
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IAPEnabled)
		{
			IapManager.onPurchaseSucceeded += HandleIapManageronPurchaseSucceeded;
		}
		KeyListener.keyPressed += HandleKeyListenerkeyPressed;
		if ((bool)m_music)
		{
			m_music.GetComponent<AudioSource>().Play();
		}
	}

	private void OnDisable()
	{
		if (Singleton<BuildCustomizationLoader>.IsInstantiated() && Singleton<BuildCustomizationLoader>.Instance.IAPEnabled)
		{
			IapManager.onPurchaseSucceeded -= HandleIapManageronPurchaseSucceeded;
		}
		KeyListener.keyPressed -= HandleKeyListenerkeyPressed;
	}

	private void HandleKeyListenerkeyPressed(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			EventManager.Send(new UIEvent(UIEvent.Type.LevelSelection));
		}
	}

	private void HandleIapManageronPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		if (type == IapManager.InAppPurchaseItemType.UnlockFullVersion || type == IapManager.InAppPurchaseItemType.UnlockTenLevels || type == IapManager.InAppPurchaseItemType.UnlockEpisode)
		{
			Singleton<GameManager>.Instance.LoadNextLevel();
		}
	}

	private void ReportLeaderboardScoreCallback(bool result)
	{
	}

	private void Awake()
	{
		doubleRewardButton.LevelComplete = this;
	}

	private IEnumerator Start()
	{
		bShakeCamera = false;
		PlayerProgressBar.Instance.CanLevelUp = false;
		m_objectiveOne.GetComponent<Collider>().enabled = false;
		m_objectiveTwo.GetComponent<Collider>().enabled = false;
		m_objectiveThree.GetComponent<Collider>().enabled = false;
		m_controlsPosition = m_controlsPanel.transform.position;
		m_controlsHidePosition = m_controlsPosition + new Vector3(0f, -5f, 0f);
		m_controlsPanel.transform.position = m_controlsHidePosition;
		m_starPanelPosition = m_starPanel.transform.position;
		m_starPanelHidePosition = m_starPanelPosition + new Vector3(0f, 12f, 0f);
		m_starPanel.transform.position = m_starPanelHidePosition;
		m_backgroundPosition = m_background.transform.position;
		m_backgroundHidePosition = m_backgroundPosition + new Vector3(0f, 12f, 0f);
		m_background.transform.position = m_backgroundHidePosition;
		m_hasDoubleRewards = Singleton<DoubleRewardManager>.Instance.HasDoubleReward;
		GameObject gameObject = base.transform.Find("StarPanel/NewBestTimeStamp").gameObject;
		gameObject.SetActive(value: false);
		GameObject gameObject2 = base.transform.Find("StarPanel/ResultTime").gameObject;
		if (!WPFMonoBehaviour.levelManager || WPFMonoBehaviour.levelManager.m_raceLevel)
		{
			base.transform.Find("Rewards/Pig").Translate(0f, -1.4f, 0f);
			float num = ((!WPFMonoBehaviour.levelManager) ? 124.56f : WPFMonoBehaviour.levelManager.TimeElapsed);
			string text = RaceTimeCounter.FormatTime(num);
			gameObject2.GetComponent<TextMesh>().text = text;
			gameObject2.transform.Find("ResultTimeShadow").GetComponent<TextMesh>().text = text;
			if (GameProgress.HasBestTime(Singleton<GameManager>.Instance.CurrentSceneName))
			{
				float bestTime = GameProgress.GetBestTime(Singleton<GameManager>.Instance.CurrentSceneName);
				if (num < bestTime)
				{
					GameProgress.SetBestTime(Singleton<GameManager>.Instance.CurrentSceneName, num);
					gameObject.SetActive(value: true);
				}
			}
			else
			{
				GameProgress.SetBestTime(Singleton<GameManager>.Instance.CurrentSceneName, num);
				gameObject.SetActive(value: true);
			}
			if (Singleton<SocialGameManager>.IsInstantiated())
			{
				long num2 = (int)(num * 100f);
				if (num2 > 0)
				{
					Singleton<SocialGameManager>.Instance.ReportLeaderboardScore(Singleton<GameManager>.Instance.CurrentLevelLeaderboard(), num2, ReportLeaderboardScoreCallback);
				}
			}
		}
		else
		{
			gameObject2.GetComponent<Renderer>().enabled = false;
			gameObject2.transform.Find("ResultTimeShadow").GetComponent<Renderer>().enabled = false;
		}
		m_starOne.SetActive(value: false);
		m_starTwo.SetActive(value: false);
		m_starThree.SetActive(value: false);
		if (m_challenges.Count >= 2 && m_challenges[1].IsCompleted() && !m_challenges[0].IsCompleted())
		{
			Challenge value = m_challenges[0];
			m_challenges[0] = m_challenges[1];
			m_challenges[1] = value;
		}
		float waitTime = 0.1f;
		while (!Mathf.Approximately(Time.timeScale, 1f) || waitTime > 0f)
		{
			waitTime -= Time.deltaTime;
			yield return null;
		}
		StartCoroutine(ShowStarPanel(1.5f));
		float num3 = 0.875f;
		int num4 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("level_complete_snout_reward", "One");
		int num5 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("level_complete_snout_reward", "Two");
		int num6 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("level_complete_snout_reward", "Three");
		if (m_hasDoubleRewards)
		{
			num4 *= 2;
			num5 *= 2;
			num6 *= 2;
		}
		float num7 = 2f;
		string currentSceneName = Singleton<GameManager>.Instance.CurrentSceneName;
		bool flag = GameProgress.IsLevelCompleted(currentSceneName);
		bool flag2 = GameProgress.IsChallengeCompleted(currentSceneName, m_challenges[0].ChallengeNumber);
		bool flag3 = GameProgress.IsChallengeCompleted(currentSceneName, m_challenges[1].ChallengeNumber);
		bool flag4 = GameProgress.HasCollectedSnoutCoins(currentSceneName, 0);
		bool flag5 = GameProgress.HasCollectedSnoutCoins(currentSceneName, m_challenges[0].ChallengeNumber);
		bool flag6 = GameProgress.HasCollectedSnoutCoins(currentSceneName, m_challenges[1].ChallengeNumber);
		bool flag7 = m_challenges.Count >= 1 && m_challenges[0].IsCompleted();
		bool flag8 = m_challenges.Count >= 2 && m_challenges[1].IsCompleted();
		m_objectiveOne.SetChallenge(m_goal);
		if (!flag4)
		{
			GameProgress.SetChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, 0, completed: true, num4 > 0);
		}
		if (m_challenges.Count >= 1)
		{
			m_objectiveTwo.SetChallenge(m_challenges[0]);
		}
		if (m_challenges.Count >= 2)
		{
			m_objectiveThree.SetChallenge(m_challenges[1]);
		}
		int num8 = 0;
		int num9 = 0;
		if (flag)
		{
			num9++;
		}
		if (flag2)
		{
			num9++;
		}
		if (flag3)
		{
			num9++;
		}
		if (!flag)
		{
			num9++;
		}
		if (flag7 && !flag2)
		{
			num9++;
		}
		if (flag8 && !flag3)
		{
			num9++;
		}
		int num10 = 1;
		if (flag7)
		{
			num10++;
		}
		if (flag8)
		{
			num10++;
		}
		int num11 = num9 - num10;
		string text2 = "0";
		string text3 = "0";
		int num12 = 0;
		if (flag)
		{
			StartCoroutine(PlayOldStarEffects(m_objectiveOne, m_starOne, num7, completedInThisRun: true, WPFMonoBehaviour.gameData.commonAudioCollection.starEffects[num11], "StarAppearing"));
			num11++;
			text2 = "1";
		}
		else
		{
			LevelCompletedForFirstTime();
			num7 += 0.2f;
			StartCoroutine(PlayStarEffects(m_objectiveOne, m_starOne, null, num7, WPFMonoBehaviour.gameData.commonAudioCollection.starEffects[num11], "StarAppearing"));
			num11++;
			GameProgress.SetLevelCompleted(Singleton<GameManager>.Instance.CurrentSceneName);
			text3 = "1";
			num12++;
		}
		if (!flag4)
		{
			num8 += num4;
			coinsCollected |= CoinsCollected.Challenge1;
			if ((bool)SnoutButton.Instance && (bool)m_starOne)
			{
				for (int i = 0; i < num4; i++)
				{
					SnoutButton.Instance.AddParticles(m_starOne, 1, num7 + num3 + 0.2f * (float)i);
				}
			}
		}
		m_objectiveOne.ShowSnoutReward(!flag4, num4);
		float num13 = num7 + 0.7f;
		if (flag2)
		{
			bool snoutCoinsCollected = true;
			if (flag7 && !flag5 && flag6)
			{
				if (num6 <= 0)
				{
					snoutCoinsCollected = false;
				}
				num8 += num6;
				coinsCollected |= CoinsCollected.Challenge3;
				if ((bool)SnoutButton.Instance && (bool)m_starTwo)
				{
					for (int j = 0; j < num6; j++)
					{
						SnoutButton.Instance.AddParticles(m_starTwo, 1, num13 + num3 + 0.2f * (float)j);
					}
				}
			}
			else if (flag7 && !flag5 && !flag6)
			{
				if (num5 <= 0)
				{
					snoutCoinsCollected = false;
				}
				num8 += num5;
				coinsCollected |= CoinsCollected.Challenge2;
				if ((bool)SnoutButton.Instance && (bool)m_starTwo)
				{
					for (int k = 0; k < num5; k++)
					{
						SnoutButton.Instance.AddParticles(m_starTwo, 1, num13 + num3 + 0.2f * (float)k);
					}
				}
			}
			if (flag7 && !flag5)
			{
				GameProgress.SetChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[0].ChallengeNumber, completed: true, snoutCoinsCollected);
			}
			AudioSource starAudio = null;
			if (flag7)
			{
				starAudio = WPFMonoBehaviour.gameData.commonAudioCollection.starEffects[num11];
				num11++;
			}
			StartCoroutine(PlayOldStarEffects(m_objectiveTwo, m_starTwo, num13, flag7, starAudio, "StarAppearing2"));
			text2 += ",2";
		}
		else if (flag7)
		{
			bool snoutCoinsCollected2 = true;
			if (!flag5 && flag6)
			{
				if (num6 <= 0)
				{
					snoutCoinsCollected2 = false;
				}
				num8 += num6;
				coinsCollected |= CoinsCollected.Challenge3;
				if ((bool)SnoutButton.Instance && (bool)m_starTwo)
				{
					for (int l = 0; l < num6; l++)
					{
						SnoutButton.Instance.AddParticles(m_starTwo, 1, num13 + num3 + 0.2f * (float)l);
					}
				}
			}
			else if (!flag5 && !flag6)
			{
				if (num5 <= 0)
				{
					snoutCoinsCollected2 = false;
				}
				num8 += num5;
				coinsCollected |= CoinsCollected.Challenge2;
				if ((bool)SnoutButton.Instance && (bool)m_starTwo)
				{
					for (int m = 0; m < num5; m++)
					{
						SnoutButton.Instance.AddParticles(m_starTwo, 1, num13 + num3 + 0.2f * (float)m);
					}
				}
			}
			StartCoroutine(PlayStarEffects(m_objectiveTwo, m_starTwo, m_challenges[0], num13, WPFMonoBehaviour.gameData.commonAudioCollection.starEffects[num11], "StarAppearing2"));
			num11++;
			GameProgress.SetChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[0].ChallengeNumber, completed: true, snoutCoinsCollected2);
			text3 += ",2";
			num12++;
		}
		if (flag5)
		{
			m_objectiveTwo.ShowSnoutReward(show: false);
		}
		else
		{
			m_objectiveTwo.ShowSnoutReward(show: true, (!flag6) ? num5 : num6);
		}
		float delay = num13 + 0.7f;
		if (flag3)
		{
			bool snoutCoinsCollected3 = true;
			if (flag8 && !flag6 && flag7)
			{
				if (num6 <= 0)
				{
					snoutCoinsCollected3 = false;
				}
				num8 += num6;
				coinsCollected |= CoinsCollected.Challenge3;
				if ((bool)SnoutButton.Instance && (bool)m_starThree)
				{
					for (int n = 0; n < num6; n++)
					{
						SnoutButton.Instance.AddParticles(m_starThree, 1, delay + num3 + 0.2f * (float)n);
					}
				}
			}
			else if (flag8 && !flag6 && !flag5)
			{
				if (num5 <= 0)
				{
					snoutCoinsCollected3 = false;
				}
				num8 += num5;
				coinsCollected |= CoinsCollected.Challenge2;
				if ((bool)SnoutButton.Instance && (bool)m_starThree)
				{
					for (int num14 = 0; num14 < num5; num14++)
					{
						SnoutButton.Instance.AddParticles(m_starThree, 1, delay + num3 + 0.2f * (float)num14);
					}
				}
			}
			if (flag8 && !flag6)
			{
				GameProgress.SetChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[1].ChallengeNumber, completed: true, snoutCoinsCollected3);
			}
			AudioSource starAudio2 = null;
			if (flag8)
			{
				starAudio2 = WPFMonoBehaviour.gameData.commonAudioCollection.starEffects[num11];
			}
			StartCoroutine(PlayOldStarEffects(m_objectiveThree, m_starThree, delay, flag8, starAudio2, "StarAppearing3"));
			text2 += ",3";
		}
		else if (flag8)
		{
			bool snoutCoinsCollected4 = true;
			if (!flag6 && flag7)
			{
				if (num6 <= 0)
				{
					snoutCoinsCollected4 = false;
				}
				num8 += num6;
				coinsCollected |= CoinsCollected.Challenge3;
				if ((bool)SnoutButton.Instance && (bool)m_starThree)
				{
					for (int num15 = 0; num15 < num6; num15++)
					{
						SnoutButton.Instance.AddParticles(m_starThree, 1, delay + num3 + 0.2f * (float)num15);
					}
				}
			}
			else if (!flag6 && !flag5)
			{
				if (num5 <= 0)
				{
					snoutCoinsCollected4 = false;
				}
				num8 += num5;
				coinsCollected |= CoinsCollected.Challenge2;
				if ((bool)SnoutButton.Instance && (bool)m_starThree)
				{
					for (int num16 = 0; num16 < num5; num16++)
					{
						SnoutButton.Instance.AddParticles(m_starThree, 1, delay + num3 + 0.2f * (float)num16);
					}
				}
			}
			StartCoroutine(PlayStarEffects(m_objectiveThree, m_starThree, m_challenges[1], delay, WPFMonoBehaviour.gameData.commonAudioCollection.starEffects[num11], "StarAppearing3"));
			GameProgress.SetChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[1].ChallengeNumber, completed: true, snoutCoinsCollected4);
			text3 += ",3";
			num12++;
		}
		if (flag6)
		{
			m_objectiveThree.ShowSnoutReward(show: false);
		}
		else
		{
			m_objectiveThree.ShowSnoutReward(show: true, (!flag5 && !flag7 && flag8) ? num5 : num6);
		}
		GameProgress.AddSnoutCoins(num8);
		bool num17 = LevelInfo.IsStarLevel(Singleton<GameManager>.Instance.CurrentEpisodeIndex, Singleton<GameManager>.Instance.CurrentLevel);
		PlayerProgress.ExperienceType experienceType = (num17 ? PlayerProgress.ExperienceType.JokerLevelComplete1Star : PlayerProgress.ExperienceType.LevelComplete1Star);
		PlayerProgress.ExperienceType experienceType2 = ((!num17) ? PlayerProgress.ExperienceType.LevelComplete2Star : PlayerProgress.ExperienceType.JokerLevelComplete2Star);
		PlayerProgress.ExperienceType experienceType3 = ((!num17) ? PlayerProgress.ExperienceType.LevelComplete3Star : PlayerProgress.ExperienceType.JokerLevelComplete3Star);
		string currentSceneName2 = Singleton<GameManager>.Instance.CurrentSceneName;
		if (GameProgress.ExperienceGiven(experienceType, currentSceneName2) == 0)
		{
			GameProgress.ReportExperienceGiven(experienceType, currentSceneName2);
			PlayerProgressBar.Instance.DelayUpdate();
			int count = Singleton<PlayerProgress>.Instance.AddExperience(experienceType);
			AddExperienceParticles(m_starOne, count, num7 + num3);
		}
		if ((flag2 || flag7 || flag3 || flag8) && GameProgress.ExperienceGiven(experienceType2, currentSceneName2) == 0)
		{
			GameProgress.ReportExperienceGiven(experienceType2, currentSceneName2);
			PlayerProgressBar.Instance.DelayUpdate();
			int count2 = Singleton<PlayerProgress>.Instance.AddExperience(experienceType2);
			AddExperienceParticles(m_starTwo, count2, num13 + num3);
		}
		if (flag7 && flag3 && GameProgress.ExperienceGiven(experienceType3, currentSceneName2) == 0)
		{
			GameProgress.ReportExperienceGiven(experienceType3, currentSceneName2);
			PlayerProgressBar.Instance.DelayUpdate();
			int count3 = Singleton<PlayerProgress>.Instance.AddExperience(experienceType3);
			AddExperienceParticles(m_starTwo, count3, num13 + num3);
		}
		else if (flag7 && flag3 && GameProgress.ExperienceGiven(experienceType3, currentSceneName2) == 0)
		{
			GameProgress.ReportExperienceGiven(experienceType3, currentSceneName2);
			PlayerProgressBar.Instance.DelayUpdate();
			int count4 = Singleton<PlayerProgress>.Instance.AddExperience(experienceType3);
			AddExperienceParticles(m_starThree, count4, delay + num3);
		}
		else if ((flag2 || flag7) && (flag3 || flag8) && GameProgress.ExperienceGiven(experienceType3, currentSceneName2) == 0)
		{
			GameProgress.ReportExperienceGiven(experienceType3, currentSceneName2);
			PlayerProgressBar.Instance.DelayUpdate();
			int count5 = Singleton<PlayerProgress>.Instance.AddExperience(experienceType3);
			AddExperienceParticles(m_starThree, count5, delay + num3);
		}
		bool flag9 = Singleton<GameManager>.Instance.CurrentStarLevelUnlocked();
		GameProgress.SetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_stars", num9);
		bool flag10 = Singleton<GameManager>.Instance.CurrentStarLevelUnlocked();
		m_jokerLevelUnlocked = flag10 && !flag9;
		StartCoroutine(StartMusic(delay + 1f, WPFMonoBehaviour.gameData.commonAudioCollection.starLoops[num9 - 1]));
		StartCoroutine(ShowBackground(1.5f, delay - 1.5f));
		BasePart.PartType newRacePart = GetNewRacePart(num12);
		if (newRacePart != 0)
		{
			m_unlockedRaceLevelPart = newRacePart;
		}
		if (!ShowRewards(ref delay))
		{
			StartCoroutine(ShowLonelyPig(delay + 1f, num9));
		}
		if (WPFMonoBehaviour.levelManager != null)
		{
			GameObject gameObject3 = GameObject.FindGameObjectWithTag("Goal");
			if (gameObject3 != null && gameObject3.transform.Find("Dessert") != null)
			{
				GameProgress.AddDesserts(WPFMonoBehaviour.gameData.m_BonusDessert.GetComponent<Dessert>().saveId, 1);
				WPFMonoBehaviour.levelManager.m_CollectedDessertsCount++;
			}
			if (WPFMonoBehaviour.levelManager.LevelDessertsCount > 0)
			{
				if (WPFMonoBehaviour.levelManager.m_CollectedDessertsCount >= WPFMonoBehaviour.levelManager.LevelDessertsCount && Singleton<SocialGameManager>.IsInstantiated())
				{
					Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.CAKE_WALK", 100.0);
					if (WPFMonoBehaviour.levelManager.ContraptionRunning.HasSuperMagnet)
					{
						bool flag11 = true;
						foreach (Challenge challenge in m_challenges)
						{
							if (challenge.Type == Challenge.ChallengeType.Box && !challenge.IsCompleted())
							{
								flag11 = false;
								break;
							}
						}
						if (flag11)
						{
							Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.MAGNET_MASTER", 100.0);
						}
					}
				}
				ShowDessertButton();
			}
		}
		StartCoroutine(CoroutineRunner.DelayActionSequence(delegate
		{
			PlayerProgressBar.Instance.CanLevelUp = true;
		}, delay + 1.5f, realTime: false));
		StartCoroutine(ShowControls(delay + 1.5f));
		StartCoroutine(ShowEpisodeThreeStarred(delay + 2f));
		if ((bool)WPFMonoBehaviour.levelManager)
		{
			SendLevelCompleteEvent(text2, text3);
		}
		if (Singleton<SocialGameManager>.IsInstantiated() && (bool)WPFMonoBehaviour.levelManager)
		{
			CheckLevelEndAchievements();
		}
		if ((flag7 || flag2) && m_challenges[0].Type == Challenge.ChallengeType.Time)
		{
			if ((bool)rewardVideoButton)
			{
				rewardVideoButton.SetActive(value: false);
			}
			if ((bool)extraCoinsRewardButton)
			{
				extraCoinsRewardButton.gameObject.SetActive(value: true);
			}
		}
		else if ((flag8 || flag3) && m_challenges[1].Type == Challenge.ChallengeType.Time)
		{
			if ((bool)rewardVideoButton)
			{
				rewardVideoButton.SetActive(value: false);
			}
			if ((bool)extraCoinsRewardButton)
			{
				extraCoinsRewardButton.gameObject.SetActive(value: true);
			}
		}
		else
		{
			bool flag12 = false;
			if ((bool)rewardVideoButton)
			{
				rewardVideoButton.SetActive(value: true);
				flag12 = true;
				hasTransformed = true;
			}
			if ((bool)extraCoinsRewardButton)
			{
				extraCoinsRewardButton.gameObject.SetActive(!flag12);
			}
		}
		if ((bool)m_buttonGridLayout)
		{
			m_buttonGridLayout.UpdateLayout();
		}
	}

	private void AddExperienceParticles(GameObject target, int count, float delay)
	{
		if ((bool)target || Singleton<PlayerProgress>.IsInstantiated() || (bool)PlayerProgressBar.Instance)
		{
			PlayerProgressBar.Instance.AddParticles(target, count, delay);
		}
	}

	private BasePart.PartType GetNewRacePart(int newStars)
	{
		int num = GameProgress.GetAllStars() + GameProgress.GetRaceLevelUnlockedStars();
		BasePart.PartType partType = BasePart.PartType.Unknown;
		int num2 = 0;
		string race_level_id = string.Empty;
		foreach (RaceLevels.LevelUnlockablePartsData levelUnlockable in WPFMonoBehaviour.gameData.m_raceLevels.LevelUnlockables)
		{
			if (!GameProgress.GetRaceLevelUnlocked(levelUnlockable.m_identifier))
			{
				continue;
			}
			for (int i = 0; i < levelUnlockable.m_tiers.Count; i++)
			{
				if (levelUnlockable.m_tiers[i].m_starLimit > num - newStars && levelUnlockable.m_tiers[i].m_starLimit <= num && levelUnlockable.m_tiers[i].m_starLimit > num2)
				{
					partType = levelUnlockable.m_tiers[i].m_part;
					num2 = levelUnlockable.m_tiers[i].m_starLimit;
					m_raceLevelNumber = levelUnlockable;
					race_level_id = levelUnlockable.m_identifier;
				}
			}
		}
		if (partType != 0)
		{
			SendUnlockedRaceLevelPartFlurryEvent(Singleton<GameManager>.Instance.CurrentLevelIdentifier, race_level_id, partType.ToString());
		}
		return partType;
	}

	private bool ShowRewards(ref float delay)
	{
		if (m_jokerLevelUnlocked)
		{
			string text = LevelSelector.DifferentiatedLevelLabel(Singleton<GameManager>.Instance.GetCurrentRowJokerLevelIndex());
			EpisodeLevelInfo currentRowJokerLevel = Singleton<GameManager>.Instance.GetCurrentRowJokerLevel();
			string empty = string.Empty;
			empty = Singleton<GameManager>.Instance.CurrentEpisodeIndex switch
			{
				5 => "BonusLevelUnlockGolden", 
				4 => "BonusLevelUnlockHalloween", 
				_ => "BonusLevelUnlock", 
			};
			Button component = base.transform.Find("Rewards").Find(empty).Find("Open")
				.Find("JokerLevelButton")
				.GetComponent<Button>();
			component.MethodToCall.SetMethod(base.gameObject.GetComponent<LevelComplete>(), "LoadJokerLevel", currentRowJokerLevel.sceneName);
			component.transform.Find("LevelNumber").GetComponent<TextMesh>().text = text;
			string currentLevelIdentifier = Singleton<GameManager>.Instance.CurrentLevelIdentifier;
			SendUnlockedBonusLevelFlurryEvent(currentLevelIdentifier, Singleton<GameManager>.Instance.CurrentEpisodeLabel + "-" + text);
			StartCoroutine(ShowUnlockedLevel(delay, empty));
		}
		else if (m_unlockedRaceLevelPart != 0)
		{
			delay += 0.5f;
			StartCoroutine(ShowUnlockedRaceLevelPart(delay + 1f));
			delay += 0.5f;
		}
		else if (m_unlockedPart != 0)
		{
			delay += 0.5f;
			StartCoroutine(ShowUnlockedPart(delay + 1f));
			delay += 0.5f;
		}
		else
		{
			if (!m_sandboxUnlocked)
			{
				return false;
			}
			base.transform.Find("Rewards/SandboxUnlock/Open/Sandbox").GetComponent<Button>().MethodToCall.SetMethod(base.gameObject.GetComponent<LevelComplete>(), "LoadSandboxLevelSelection");
			StartCoroutine(ShowUnlockedSandbox(delay + 1f));
		}
		return true;
	}

	private void CheckLevelEndAchievements()
	{
		for (int i = 0; i < Singleton<GameManager>.Instance.gameData.m_episodeLevels.Count; i++)
		{
			if (Singleton<GameManager>.Instance.gameData.m_episodeLevels[i].Name == Singleton<GameManager>.Instance.CurrentEpisode)
			{
				if (Singleton<GameManager>.Instance.IsLastLevelInEpisode() && Singleton<GameManager>.Instance.gameData.m_episodeLevels[i].ClearAchievement != null)
				{
					Singleton<SocialGameManager>.Instance.ReportAchievementProgress(Singleton<GameManager>.Instance.gameData.m_episodeLevels[i].ClearAchievement, 100.0);
				}
				if (Singleton<GameManager>.Instance.CurrentEpisodeThreeStarredNormalLevels() && Singleton<GameManager>.Instance.gameData.m_episodeLevels[i].ThreeStarAchievement != null)
				{
					Singleton<SocialGameManager>.Instance.ReportAchievementProgress(Singleton<GameManager>.Instance.gameData.m_episodeLevels[i].ThreeStarAchievement, 100.0);
				}
				if (Singleton<GameManager>.Instance.CurrentEpisodeThreeStarredSpecialLevels() && Singleton<GameManager>.Instance.gameData.m_episodeLevels[i].SpecialThreeStarAchievement != null)
				{
					Singleton<SocialGameManager>.Instance.ReportAchievementProgress(Singleton<GameManager>.Instance.gameData.m_episodeLevels[i].SpecialThreeStarAchievement, 100.0);
				}
			}
		}
		if (WPFMonoBehaviour.levelManager.IsPartTransported(BasePart.PartType.KingPig))
		{
			int n = GameProgress.GetInt("Transported_Kings");
			n++;
			GameProgress.SetInt("Transported_Kings", n);
			((Action<List<string>>)delegate(List<string> achievements)
			{
				foreach (string achievement in achievements)
				{
					if (Singleton<SocialGameManager>.Instance.TryReportAchievementProgress(achievement, 100.0, (int limit) => n >= limit))
					{
						break;
					}
				}
			})(new List<string> { "grp.HOGFFEUR", "grp.PIGSHAW" });
		}
		if (WPFMonoBehaviour.levelManager.IsPartTransported(BasePart.PartType.Pumpkin))
		{
			int @int = GameProgress.GetInt("Transported_Pumpkins");
			@int++;
			GameProgress.SetInt("Transported_Pumpkins", @int);
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.PUMPKIN_CHARIOT", 100.0);
		}
		if (!WPFMonoBehaviour.levelManager.ContraptionRunning.IsBroken())
		{
			int n = GameProgress.GetInt("Perfect_Completions");
			n++;
			GameProgress.SetInt("Perfect_Completions", n);
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.SKILLED_PILOT", 100.0, (int limit) => n >= limit);
		}
		int partCount = WPFMonoBehaviour.levelManager.ContraptionProto.GetPartCount(BasePart.PartType.TNT);
		int partCount2 = WPFMonoBehaviour.levelManager.ContraptionProto.GetPartCount(BasePart.PartType.Pig);
		int partCount3 = WPFMonoBehaviour.levelManager.ContraptionProto.GetPartCount(BasePart.PartType.KingPig);
		if (partCount + partCount2 + partCount3 >= WPFMonoBehaviour.levelManager.ContraptionProto.Parts.Count && partCount >= 1)
		{
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.PORCINE_CANNONBALL", 100.0);
		}
		if (WPFMonoBehaviour.levelManager.ContraptionProto.FindPig().enclosedInto == null)
		{
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.THINK_OUTSIDE_THE_BOX", 100.0);
		}
		Pig pig = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPig() as Pig;
		float traveledDistance = pig.traveledDistance;
		float rolledDistance = pig.rolledDistance;
		GameProgress.SetFloat("traveledDistance", traveledDistance);
		GameProgress.SetFloat("rolledDistance", rolledDistance);
		((Action<List<string>>)delegate(List<string> achievements)
		{
			foreach (string achievement2 in achievements)
			{
				if (Singleton<SocialGameManager>.Instance.TryReportAchievementProgress(achievement2, 100.0, (int limit) => traveledDistance > (float)limit))
				{
					break;
				}
			}
		})(new List<string> { "grp.TOURIST_III", "grp.TOURIST_II", "grp.TOURIST_I" });
		((Action<List<string>>)delegate(List<string> achievements)
		{
			foreach (string achievement3 in achievements)
			{
				if (Singleton<SocialGameManager>.Instance.TryReportAchievementProgress(achievement3, 100.0, (int limit) => rolledDistance > (float)limit))
				{
					break;
				}
			}
		})(new List<string> { "grp.ROLLING_LOW_III", "grp.ROLLING_LOW_II", "grp.ROLLING_LOW_I" });
	}

	private void LevelCompletedForFirstTime()
	{
		if ((bool)WPFMonoBehaviour.levelManager)
		{
			foreach (LevelManager.PartCount item in WPFMonoBehaviour.levelManager.m_partsToUnlockOnCompletion)
			{
				GameProgress.AddSandboxParts(item.type, item.count);
				m_unlockedPart = item.type;
			}
			string sandboxToOpenForCurrentLevel = Singleton<GameManager>.Instance.SandboxToOpenForCurrentLevel;
			if (sandboxToOpenForCurrentLevel != string.Empty)
			{
				GameProgress.SetSandboxUnlocked(sandboxToOpenForCurrentLevel, unlocked: true);
				GameProgress.UnlockButton("EpisodeButtonSandbox");
				m_sandboxUnlocked = true;
			}
			if (Singleton<GameManager>.Instance.CurrentEpisodeType == GameManager.EpisodeType.Race && Singleton<GameManager>.Instance.HasNextLevel())
			{
				GameProgress.SetRaceLevelUnlocked(Singleton<GameManager>.Instance.NextRaceLevel(), unlocked: true);
			}
		}
		SendLevelFirstCompleted();
	}

	private void OnDestroy()
	{
	}

	private void SetNextLevelButtons()
	{
		if (m_controlsPanel == null)
		{
			return;
		}
		string[] names = Enum.GetNames(typeof(NextButtonState));
		Transform[] array = new Transform[names.Length];
		NextButtonState nextButtonState = NextButtonState.None;
		if (Singleton<GameManager>.Instance.HasCutScene())
		{
			nextButtonState = NextButtonState.CutsceneButton;
		}
		else if (Singleton<GameManager>.Instance.HasNextLevel())
		{
			nextButtonState = NextButtonState.NextLevelButton;
		}
		for (int i = 0; i < names.Length; i++)
		{
			Transform transform = (array[i] = m_controlsPanel.transform.Find("ButtonList/" + names[i]));
			if (transform != null)
			{
				transform.gameObject.SetActive(nextButtonState == (NextButtonState)i);
			}
		}
		if (nextButtonState == NextButtonState.NextLevelButton || nextButtonState != NextButtonState.CutsceneButton || !(array[(int)nextButtonState] != null))
		{
			return;
		}
		if (Singleton<GameManager>.Instance.LevelCount <= 15)
		{
			array[(int)nextButtonState].gameObject.SetActive(value: false);
			return;
		}
		if (Singleton<GameManager>.Instance.IsLastLevelInEpisode())
		{
			array[(int)nextButtonState].GetComponent<Button>().MethodToCall.SetMethod(this, "LoadEndingCutscene");
			if (Singleton<BuildCustomizationLoader>.Instance.IsChina && GameProgress.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_stars") != 3)
			{
				array[(int)nextButtonState].gameObject.SetActive(value: false);
				return;
			}
			GameProgress.SetInt(Singleton<GameManager>.Instance.EndingCutscene + "_played", 1);
		}
		else if (Singleton<GameManager>.Instance.HasMidCutsceneEnabled())
		{
			array[(int)nextButtonState].GetComponent<Button>().MethodToCall.SetMethod(this, "LoadMidCutscene");
			if (Singleton<BuildCustomizationLoader>.Instance.IsChina && GameProgress.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_stars") != 3)
			{
				array[(int)nextButtonState].gameObject.SetActive(value: false);
				return;
			}
			GameProgress.SetInt(Singleton<GameManager>.Instance.MidCutscene + "_played", 1);
		}
		array[(int)nextButtonState].position -= Vector3.forward * 8f;
	}

	private void ShowUnlockLevelRewardButton(Transform[] buttons)
	{
		buttons[2].localPosition -= Vector3.up * 4f;
		if (buttons[2] != null && WPFMonoBehaviour.gameData.m_unlockLevelAdButtonPrefab != null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_unlockLevelAdButtonPrefab);
			obj.transform.parent = buttons[2];
			obj.transform.localPosition = -Vector3.forward * 2f;
		}
	}

	private IEnumerator ShowUnlockedLevel(float delay, string rewardBonusLevelName)
	{
		yield return new WaitForSeconds(delay);
		Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.jokerLevelUnlocked);
		GameObject.Find("Rewards").transform.Find("UnlockPart").GetComponent<RewardView>().Hide();
		RewardView reward = GameObject.Find("Rewards").transform.Find(rewardBonusLevelName).GetComponent<RewardView>();
		if (reward.HasLocked())
		{
			reward.ShowLocked();
			GameObject.Find("Rewards").GetComponent<Animation>().Play("RewardAnimation");
			yield return new WaitForSeconds(0.75f);
		}
		reward.transform.FindChildRecursively("JokerLevelButton").transform.Find("Lock").GetComponent<Animation>().Play("LevelCompleteLockAnimation");
		yield return new WaitForSeconds(0.75f);
		ParticleSystem[] componentsInChildren = GameObject.Find(rewardBonusLevelName).transform.Find("Open").GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Play();
		}
		reward.ShowOpen();
		GameObject.Find("PigLevelCompleteBL").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GameObject.Find("PigLevelCompleteBL").GetComponent<Animation>().Play("PigLevelCompleteAnimation");
		GameObject.Find("Pig_ShadowBL").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GameObject.Find("Pig_ShadowBL").GetComponent<Animation>().Play("LevelCompleteShadowAnimation");
	}

	public void ResumeAnimations()
	{
		SetNextLevelButtons();
		int @int = GameProgress.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_stars");
		if (!GameProgress.IsChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[0].ChallengeNumber))
		{
			m_starTwo.GetComponent<Renderer>().enabled = false;
		}
		if (!GameProgress.IsChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[1].ChallengeNumber))
		{
			m_starThree.GetComponent<Renderer>().enabled = false;
		}
		if (!m_jokerLevelUnlocked && m_unlockedPart == BasePart.PartType.Unknown && m_unlockedRaceLevelPart == BasePart.PartType.Unknown && !m_sandboxUnlocked)
		{
			StartCoroutine(ShowLonelyPig(0f, @int, bPlayRewardAnim: false));
		}
	}

	private IEnumerator ShowLonelyPig(float delay, int stars, bool bPlayRewardAnim = true)
	{
		yield return new WaitForSeconds(delay);
		Transform lonePig = base.transform.Find("Rewards/Pig/LonePig");
		Transform lp = base.transform.Find("Rewards/Pig/LonePig/LonePig1Star");
		Transform lp2 = base.transform.Find("Rewards/Pig/LonePig/LonePig2Star");
		Transform lp3 = base.transform.Find("Rewards/Pig/LonePig/LonePig3Star");
		ParticleSystem[] ps = base.transform.Find("Rewards/Pig/LonelyPigParticles").GetComponentsInChildren<ParticleSystem>();
		lonePig.GetComponent<Renderer>().enabled = true;
		lp.GetComponent<Renderer>().enabled = stars == 1;
		lp2.GetComponent<Renderer>().enabled = stars == 2;
		lp3.GetComponent<Renderer>().enabled = stars == 3;
		if (bPlayRewardAnim)
		{
			base.transform.Find("Rewards").GetComponent<Animation>().Play("RewardAnimation");
			yield return new WaitForSeconds(0.65f);
		}
		if (stars >= 2)
		{
			ParticleSystem[] array = ps;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
		}
		yield return new WaitForSeconds(0.1f);
		switch (stars)
		{
		case 1:
			lp.GetComponent<Animation>().Play("LevelCompleteLonePig1Star");
			yield break;
		case 2:
			lp2.GetComponent<Animation>().Play("LevelCompleteLonePig1Star");
			yield break;
		}
		lonePig.GetComponent<Animation>().Play("PigLevelCompleteAnimation");
		lp3.GetComponent<Animation>().Play("LevelCompleteLonePig1Star");
		yield return new WaitForSeconds(0.73f);
		while (true)
		{
			ParticleSystem[] array = ps;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
			yield return new WaitForSeconds(0.7f);
		}
	}

	private void ShowDessertButton()
	{
		Transform obj = base.transform.FindChildRecursively("KingPigFeedButton");
		obj.gameObject.SetActive(WPFMonoBehaviour.levelManager.m_CollectedDessertsCount > 0);
		Transform obj2 = obj.Find("DessertReward");
		string text = "+" + WPFMonoBehaviour.levelManager.m_CollectedDessertsCount;
		obj2.Find("Icon/Text").GetComponent<TextMesh>().text = text;
		obj2.Find("Icon/Shadow").GetComponent<TextMesh>().text = text;
	}

	private IEnumerator ShowUnlockedSandbox(float delay)
	{
		yield return new WaitForSeconds(delay);
		Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.sandboxLevelUnlocked);
		GameObject.Find("Rewards").transform.Find("UnlockPart").GetComponent<RewardView>().Hide();
		RewardView reward = GameObject.Find("Rewards").transform.Find("SandboxUnlock").GetComponent<RewardView>();
		if (reward.HasLocked())
		{
			reward.ShowLocked();
			GameObject.Find("Rewards").GetComponent<Animation>().Play("RewardAnimation");
			yield return new WaitForSeconds(0.75f);
		}
		GameObject.Find("Sandbox").transform.Find("Lock").GetComponent<Animation>().Play("LevelCompleteLockAnimation");
		yield return new WaitForSeconds(0.6f);
		ParticleSystem[] componentsInChildren = GameObject.Find("SandboxUnlock").transform.Find("Open").GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Play();
		}
		reward.ShowOpen();
		GameObject.Find("PigLevelCompleteSB").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GameObject.Find("PigLevelCompleteSB").GetComponent<Animation>().Play("PigLevelCompleteAnimation");
		GameObject.Find("Pig_ShadowSB").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GameObject.Find("Pig_ShadowSB").GetComponent<Animation>().Play("LevelCompleteShadowAnimation");
	}

	private IEnumerator ShowUnlockedPart(float delay)
	{
		yield return new WaitForSeconds(delay);
		RewardView component = GameObject.Find("Rewards").transform.Find("UnlockPart").GetComponent<RewardView>();
		component.ShowOpen();
		component.SetPart(m_unlockedPart);
		GameObject.Find("Rewards").GetComponent<Animation>().Play("RewardAnimation");
		GameObject.Find("PigLevelComplete").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GameObject.Find("PigLevelComplete").GetComponent<Animation>().Play("PigLevelCompleteAnimation");
		GameObject.Find("Pig_Shadow").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GameObject.Find("Pig_Shadow").GetComponent<Animation>().Play("LevelCompleteShadowAnimation");
	}

	private IEnumerator ShowUnlockedRaceLevelPart(float delay)
	{
		yield return new WaitForSeconds(delay);
		RewardView component = GameObject.Find("Rewards").transform.Find("UnlockRaceLevelPart").GetComponent<RewardView>();
		component.ShowOpen();
		component.SetPart(m_unlockedRaceLevelPart);
		if (m_raceLevelNumber != null)
		{
			component.transform.Find("Open/RaceLevelButton/LevelNumber").GetComponent<TextMesh>().text = m_raceLevelNumber.m_levelNumber;
			component.transform.Find("Open/RaceLevelButton").GetComponent<Button>().MethodToCall.SetParameter(m_raceLevelNumber.m_identifier);
		}
		GameObject.Find("Rewards").GetComponent<Animation>().Play("RewardAnimation");
		GameObject.Find("PigLevelComplete").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GameObject.Find("PigLevelComplete").GetComponent<Animation>().Play("PigLevelCompleteAnimation");
		GameObject.Find("Pig_Shadow").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GameObject.Find("Pig_Shadow").GetComponent<Animation>().Play("LevelCompleteShadowAnimation");
	}

	private IEnumerator ShowStarPanel(float delay)
	{
		yield return new WaitForSeconds(delay);
		float t = 0f;
		while (t < 1f)
		{
			t = Mathf.Min(t + Time.deltaTime / 0.3f, 1f);
			m_starPanel.transform.position = Vector3.Slerp(m_starPanelHidePosition, m_starPanelPosition, t);
			yield return 0;
		}
	}

	private IEnumerator ShowBackground(float delay1, float delay2)
	{
		yield return new WaitForSeconds(delay1);
		if (!WPFMonoBehaviour.levelManager || WPFMonoBehaviour.levelManager.m_raceLevel)
		{
			m_backgroundPosition.y -= 1.6f;
		}
		float t = 0f;
		while (t < 1f)
		{
			t = Mathf.Min(t + Time.deltaTime / 0.3f, 1f);
			m_background.transform.position = Vector3.Slerp(m_backgroundHidePosition, m_backgroundPosition, t);
			yield return 0;
		}
		if (!WPFMonoBehaviour.levelManager || WPFMonoBehaviour.levelManager.m_raceLevel)
		{
			m_backgroundPosition.y += 1.6f;
		}
		yield return new WaitForSeconds(delay2);
		m_backgroundHidePosition = m_backgroundPosition;
		m_backgroundPosition.y -= 12.5f;
		t = 0f;
		while (t < 1f)
		{
			t = Mathf.Min(t + Time.deltaTime / 0.3f, 1f);
			m_background.transform.position = Vector3.Slerp(m_backgroundHidePosition, m_backgroundPosition, t);
			yield return 0;
		}
	}

	private IEnumerator ShowControls(float delay)
	{
		SetNextLevelButtons();
		yield return new WaitForSeconds(delay);
		float t = 0f;
		while (t < 1f)
		{
			t = Mathf.Min(t + Time.deltaTime / 0.25f, 1f);
			m_controlsPanel.transform.position = Vector3.Slerp(m_controlsHidePosition, m_controlsPosition, t);
			yield return 0;
		}
		if (!m_hasDoubleRewards)
		{
			UpdateScreenPlacements(doubleRewardAnimation.transform.parent.gameObject);
			doubleRewardButton.gameObject.SetActive(value: true);
			doubleRewardButton.Show();
		}
		m_objectiveOne.GetComponent<Collider>().enabled = true;
		m_objectiveTwo.GetComponent<Collider>().enabled = true;
		m_objectiveThree.GetComponent<Collider>().enabled = true;
		if (SnoutButton.Instance != null)
		{
			SnoutButton.Instance.EnableButton(enable: true);
		}
	}

	private void UpdateScreenPlacements(GameObject target)
	{
		ScreenPlacement[] componentsInChildren = target.GetComponentsInChildren<ScreenPlacement>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(value: false);
			componentsInChildren[i].gameObject.SetActive(value: true);
		}
	}

	private IEnumerator ShowEpisodeThreeStarred(float delay)
	{
		yield return new WaitForSeconds(delay);
		string currentEpisode = Singleton<GameManager>.Instance.CurrentEpisode;
		int currentEpisodeIndex = Singleton<GameManager>.Instance.CurrentEpisodeIndex;
		if (currentEpisode != string.Empty && Singleton<GameManager>.Instance.CurrentEpisodeThreeStarred() && !GameProgress.IsEpisodeThreeStarred(currentEpisode))
		{
			GameProgress.SetEpisodeThreeStarred(currentEpisode, completed: true);
			m_episodeThreeStarStamp.GetComponent<Animation>().Play();
			GameObject obj = ((!(currentEpisode == "RaceLevelSelection")) ? UnityEngine.Object.Instantiate(episodeTitles[currentEpisodeIndex]) : UnityEngine.Object.Instantiate(roadHogsTitle));
			obj.transform.parent = m_episodeThreeStarStamp.transform;
			obj.transform.localPosition = new Vector3(0f, 0.345f, 0f);
			obj.transform.rotation = Quaternion.identity;
		}
	}

	private IEnumerator PlayStarEffects(ObjectiveSlot objective, GameObject star, Challenge challenge, float delay, AudioSource starAudio, string anim)
	{
		yield return new WaitForSeconds(delay);
		StartCoroutine(PlayStarEffects(objective, star, 0.25f, starAudio, anim));
	}

	private IEnumerator ShakeCamera(float power)
	{
		GameObject go = GameObject.Find("CameraSystem");
		bShakeCamera = true;
		Vector3 originalPos = go.transform.position;
		while (bShakeCamera)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * power;
			Vector3 position = go.transform.position;
			go.transform.position = new Vector3(position.x + vector.x * Time.deltaTime, position.y + vector.y * Time.deltaTime, position.z);
			yield return new WaitForEndOfFrame();
		}
		go.transform.position = originalPos;
	}

	private IEnumerator PlayStarEffects(ObjectiveSlot objective, GameObject star, float delay, AudioSource starAudio, string anim)
	{
		yield return new WaitForSeconds(delay);
		star.SetActive(value: true);
		star.GetComponent<Animation>().Play(anim);
		yield return new WaitForSeconds(0.5f);
		Singleton<AudioManager>.Instance.Play2dEffect(starAudio);
		StartCoroutine(ShakeCamera(16f));
		ParticleSystem[] componentsInChildren = star.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Play();
		}
		yield return new WaitForSeconds(0.1f);
		objective.SetSucceeded();
		yield return new WaitForSeconds(0.2f);
		bShakeCamera = false;
	}

	private IEnumerator PlayOldStarEffects(ObjectiveSlot objective, GameObject star, float delay, bool completedInThisRun, AudioSource starAudio, string anim)
	{
		if (completedInThisRun)
		{
			yield return new WaitForSeconds(delay);
			star.SetActive(value: true);
			star.GetComponent<Animation>().Play(anim);
			yield return new WaitForSeconds(0.5f);
			if ((bool)starAudio)
			{
				Singleton<AudioManager>.Instance.Play2dEffect(starAudio);
			}
			StartCoroutine(ShakeCamera(16f));
			ParticleSystem[] componentsInChildren = star.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Play();
			}
			yield return new WaitForSeconds(0.1f);
			objective.SetSucceeded();
			yield return new WaitForSeconds(0.2f);
			bShakeCamera = false;
		}
		else
		{
			star.SetActive(value: true);
		}
	}

	private IEnumerator StartMusic(float delay, AudioSource music)
	{
		yield return new WaitForSeconds(delay);
		WPFMonoBehaviour.levelManager.StopAmbient();
		m_music = Singleton<AudioManager>.Instance.SpawnLoopingEffect(music, base.transform);
	}

	public void LoadJokerLevel(string levelName)
	{
		SendSpecialLevelEnteredFlurryEvent(Singleton<GameManager>.Instance.CurrentLevelIdentifier, levelName);
		Singleton<GameManager>.Instance.LoadStarLevelTransition(levelName);
	}

	public void LoadSandboxLevelSelection(string levelName)
	{
		SendSpecialLevelEnteredFlurryEvent(Singleton<GameManager>.Instance.CurrentLevelIdentifier, "0");
		Singleton<GameManager>.Instance.LoadSandboxLevelSelection();
	}

	public void LoadRaceLevel(string identifier)
	{
		SendSpecialLevelEnteredFlurryEvent(Singleton<GameManager>.Instance.CurrentLevelIdentifier, identifier);
		if (Bundle.IsBundleLoaded("Episode_Race_Levels"))
		{
			Singleton<GameManager>.Instance.LoadRaceLevelFromLevelCompleteMenu(identifier);
		}
		else
		{
			StartCoroutine(LoadRaceLevelDelayed(identifier));
		}
	}

	private IEnumerator LoadRaceLevelDelayed(string identifier)
	{
		if (!Bundle.IsBundleLoaded("Episode_Race_Levels"))
		{
			Bundle.LoadBundleAsync("Episode_Race_Levels");
		}
		while (!Bundle.IsBundleLoaded("Episode_Race_Levels"))
		{
			yield return null;
		}
		Singleton<GameManager>.Instance.LoadRaceLevelFromLevelCompleteMenu(identifier);
	}

	public void LoadEndingCutscene()
	{
		Singleton<GameManager>.Instance.LoadEndingCutscene();
	}

	public void LoadMidCutscene()
	{
		Singleton<GameManager>.Instance.LoadMidCutscene();
	}

	public void OpenUnlockFullVersionPurchasePage()
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IAPEnabled)
		{
			Singleton<IapManager>.Instance.EnableUnlockFullVersionPurchasePage();
		}
	}

	public void SendUnlockedBonusLevelFlurryEvent(string played_id, string unlocked_id)
	{
	}

	public void SendUnlockedRaceLevelPartFlurryEvent(string played_level_id, string race_level_id, string part_id)
	{
	}

	public void SendSpecialLevelEnteredFlurryEvent(string played_level_id, string target_id)
	{
	}

	private static void SendLevelCompleteEvent(string previousStars, string newStars)
	{
	}

	private static void SendLevelFirstCompleted()
	{
	}
}
