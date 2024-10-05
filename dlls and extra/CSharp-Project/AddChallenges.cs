using System.Collections.Generic;
using UnityEngine;

public class AddChallenges : MonoBehaviour
{
	public GoalChallenge m_goal;

	public List<Challenge> m_challenges;

	private void Awake()
	{
		LevelComplete component = GameObject.Find("InGameLevelCompleteMenu").GetComponent<LevelComplete>();
		component.SetGoal(m_goal);
		component.SetChallenges(m_challenges);
	}
}
