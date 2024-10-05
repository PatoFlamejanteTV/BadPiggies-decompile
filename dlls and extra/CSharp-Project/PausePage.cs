using UnityEngine;

public class PausePage : MonoBehaviour
{
	private TextMesh m_levelNumber;

	private void OnEnable()
	{
		m_levelNumber = base.transform.Find("LevelNumber").GetComponent<TextMesh>();
		string currentLevelIdentifier = Singleton<GameManager>.Instance.CurrentLevelIdentifier;
		m_levelNumber.text = currentLevelIdentifier;
	}
}
