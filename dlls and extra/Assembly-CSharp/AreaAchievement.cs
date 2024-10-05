using UnityEngine;

public class AreaAchievement : MonoBehaviour
{
	public string achievementId;

	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
