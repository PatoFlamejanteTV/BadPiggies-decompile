using UnityEngine;

public class GoalChallenge : WPFMonoBehaviour
{
	[SerializeField]
	private Challenge.IconPlacement m_icon;

	[SerializeField]
	private GameObject m_tutorialPage;

	public Challenge.IconPlacement Icon => m_icon;

	public GameObject TutorialPage => m_tutorialPage;
}
