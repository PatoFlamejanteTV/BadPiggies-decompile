using UnityEngine;

public class CollectBoxAchievement : WPFMonoBehaviour
{
	public string boxToCollect;

	public string achievementId;

	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
