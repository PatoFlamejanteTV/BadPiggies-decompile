using UnityEngine;

public class LevelSelectorUnlockButton : MonoBehaviour
{
	public int m_pulseIfLevelCompleted;

	private void Update()
	{
		float x = GameObject.Find("LevelSelector").GetComponent<LevelSelector>().UnlockFullVersionButtonX();
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = x;
		base.transform.localPosition = localPosition;
	}

	private void Start()
	{
		if (m_pulseIfLevelCompleted != 0 && LevelInfo.IsLevelCompleted(Singleton<GameManager>.Instance.CurrentEpisodeIndex, m_pulseIfLevelCompleted - 1))
		{
			EventManager.Send(new PulseButtonEvent(UIEvent.Type.OpenUnlockFullVersionIapMenu));
		}
	}
}
