using UnityEngine;

public class LevelLoadedEventSender : MonoBehaviour
{
	private void Start()
	{
		EventManager.Send(new LevelLoadedEvent(Singleton<GameManager>.Instance.GetGameState()));
		Object.Destroy(base.gameObject);
	}
}
