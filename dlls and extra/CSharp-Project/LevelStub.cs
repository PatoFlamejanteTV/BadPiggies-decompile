using UnityEngine;

public class LevelStub : MonoBehaviour
{
	private void Awake()
	{
		Object.Instantiate(Singleton<GameManager>.Instance.CurrentLevelLoader().gameObject);
		Object.Destroy(base.gameObject);
	}
}
