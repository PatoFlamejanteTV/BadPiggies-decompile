using UnityEngine;

public class AddLevelsToSelector : MonoBehaviour
{
	public Episode m_episodeLevels;

	private void Awake()
	{
		GameObject.Find("LevelSelector").GetComponent<LevelSelector>().Levels = m_episodeLevels.LevelInfos;
	}
}
