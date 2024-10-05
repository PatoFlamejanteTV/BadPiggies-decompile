using UnityEngine;

public class LeaderboardButton : MonoBehaviour
{
	[SerializeField]
	private GameObject leaderboardDialogPrefab;

	private LeaderboardDialog leaderboardDialog;

	public void OpenLeaderboardDialog()
	{
		if (!(leaderboardDialogPrefab == null))
		{
			if (leaderboardDialog == null)
			{
				GameObject gameObject = Object.Instantiate(leaderboardDialogPrefab, 15f * Vector3.back + Vector3.down, Quaternion.identity);
				leaderboardDialog = gameObject.GetComponent<LeaderboardDialog>();
			}
			leaderboardDialog.Open();
		}
	}
}
