using UnityEngine;

public class InGameTutorialMenu : WPFMonoBehaviour
{
	public GameObject m_tutorialBookPrefab;

	[SerializeField]
	private GameObject m_cakeRaceTutorialBookPrefab;

	private GameObject m_tutorialBook;

	private void OnEnable()
	{
		Resources.UnloadUnusedAssets();
		if (WPFMonoBehaviour.levelManager != null && WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode)
		{
			m_tutorialBook = Object.Instantiate(m_cakeRaceTutorialBookPrefab);
		}
		else
		{
			m_tutorialBook = Object.Instantiate(m_tutorialBookPrefab);
		}
		m_tutorialBook.transform.parent = base.transform;
	}

	private void OnDisable()
	{
		Object.Destroy(m_tutorialBook);
		if (WPFMonoBehaviour.levelManager.m_showPowerupTutorial)
		{
			WPFMonoBehaviour.levelManager.m_showPowerupTutorial = false;
			GameProgress.SetBool("PowerupTutorialShown", value: true);
		}
		Resources.UnloadUnusedAssets();
	}
}
