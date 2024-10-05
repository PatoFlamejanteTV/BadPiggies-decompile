using System.Collections.Generic;
using UnityEngine;

public class PreviewMenu : WPFMonoBehaviour
{
	private GoalChallenge m_goal;

	private List<Challenge> m_challenges = new List<Challenge>();

	private ObjectiveSlot m_objectiveOne;

	private ObjectiveSlot m_objectiveTwo;

	private ObjectiveSlot m_objectiveThree;

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
		if (WPFMonoBehaviour.levelManager.m_sandbox || WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode)
		{
			base.transform.Find("ObjectivePanel").gameObject.SetActive(value: false);
			UpdateChallenges();
		}
	}

	private void Start()
	{
		GameObject gameObject = base.transform.Find("ObjectivePanel").gameObject;
		if (WPFMonoBehaviour.levelManager.m_sandbox)
		{
			gameObject.SetActive(value: false);
			return;
		}
		m_objectiveOne = gameObject.transform.Find("ObjectiveSlot1").GetComponent<ObjectiveSlot>();
		m_objectiveTwo = gameObject.transform.Find("ObjectiveSlot2").GetComponent<ObjectiveSlot>();
		m_objectiveThree = gameObject.transform.Find("ObjectiveSlot3").GetComponent<ObjectiveSlot>();
		if (m_challenges.Count >= 2)
		{
			bool num = GameProgress.IsChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[0].ChallengeNumber);
			bool flag = GameProgress.IsChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[1].ChallengeNumber);
			if (!num && flag)
			{
				Challenge value = m_challenges[0];
				m_challenges[0] = m_challenges[1];
				m_challenges[1] = value;
			}
		}
		if (m_challenges == null || m_challenges.Count == 0)
		{
			return;
		}
		string currentSceneName = Singleton<GameManager>.Instance.CurrentSceneName;
		bool flag2 = GameProgress.HasCollectedSnoutCoins(currentSceneName, 0);
		bool flag3 = GameProgress.HasCollectedSnoutCoins(currentSceneName, m_challenges[0].ChallengeNumber);
		bool flag4 = GameProgress.HasCollectedSnoutCoins(currentSceneName, m_challenges[1].ChallengeNumber);
		m_objectiveOne.ShowSnoutReward(!flag2, 1, parentToParent: false);
		m_objectiveTwo.ShowSnoutReward(!flag3, 1, parentToParent: false);
		m_objectiveThree.ShowSnoutReward(!flag4, 1, parentToParent: false);
		if (GameProgress.IsLevelCompleted(Singleton<GameManager>.Instance.CurrentSceneName))
		{
			m_objectiveOne.SetSucceededImmediate();
		}
		m_objectiveOne.SetChallenge(m_goal);
		if (m_challenges.Count >= 1)
		{
			m_objectiveTwo.SetChallenge(m_challenges[0]);
			if (GameProgress.IsChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[0].ChallengeNumber))
			{
				m_objectiveTwo.SetSucceededImmediate();
			}
		}
		if (m_challenges.Count >= 2)
		{
			m_objectiveThree.SetChallenge(m_challenges[1]);
			if (GameProgress.IsChallengeCompleted(Singleton<GameManager>.Instance.CurrentSceneName, m_challenges[1].ChallengeNumber))
			{
				m_objectiveThree.SetSucceededImmediate();
			}
		}
	}

	public void UpdateChallenges()
	{
		if (!WPFMonoBehaviour.levelManager.m_sandbox && !(WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode))
		{
			GameObject gameObject = base.transform.Find("ObjectivePanel").gameObject;
			m_challenges = WPFMonoBehaviour.levelManager.Challenges;
			m_objectiveTwo = gameObject.transform.Find("ObjectiveSlot2").GetComponent<ObjectiveSlot>();
			m_objectiveThree = gameObject.transform.Find("ObjectiveSlot3").GetComponent<ObjectiveSlot>();
			if (m_challenges.Count >= 1)
			{
				m_objectiveTwo.SetChallenge(m_challenges[0]);
			}
			if (m_challenges.Count >= 2)
			{
				m_objectiveThree.SetChallenge(m_challenges[1]);
			}
		}
	}
}
