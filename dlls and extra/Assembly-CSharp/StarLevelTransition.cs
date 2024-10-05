using UnityEngine;

public class StarLevelTransition : MonoBehaviour
{
	private bool m_skip;

	private bool m_loading;

	private float m_skipTimer;

	private void LoadLevel()
	{
		if (!m_loading)
		{
			m_loading = true;
			Singleton<GameManager>.Instance.LoadStarLevelAfterTransition();
		}
	}

	private void Start()
	{
		GameObject ambientCave = Singleton<GameManager>.Instance.gameData.commonAudioCollection.AmbientCave;
		Singleton<AudioManager>.Instance.Play2dEffect(ambientCave.GetComponent<AudioSource>());
	}

	private void Update()
	{
		m_skipTimer += Time.deltaTime;
		if (GuiManager.GetPointer().up && !m_skip && m_skipTimer > 1f)
		{
			m_skip = true;
			LoadLevel();
		}
	}
}
