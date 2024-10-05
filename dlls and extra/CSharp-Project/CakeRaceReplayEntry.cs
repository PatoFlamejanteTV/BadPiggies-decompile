using CakeRace;
using UnityEngine;

public class CakeRaceReplayEntry : MonoBehaviour
{
	private LeaderboardDialog dialog;

	private TextMesh[] trackLabel;

	private TextMesh[] scoreLabel;

	private Renderer[] cakes;

	private Renderer kingsFavoriteIcon;

	private void Awake()
	{
		Transform transform = base.transform.Find("TrackLabel");
		if (transform != null)
		{
			trackLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		transform = base.transform.Find("ScoreLabel");
		if (transform != null)
		{
			scoreLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		transform = base.transform.Find("KingsFavorite/KingsFavoriteActive");
		if (transform != null)
		{
			kingsFavoriteIcon = transform.GetComponent<Renderer>();
		}
		transform = base.transform.Find("CakeGrid");
		if (!(transform != null))
		{
			return;
		}
		cakes = new Renderer[5];
		for (int i = 0; i < 5; i++)
		{
			string n = $"Cake{i + 1}";
			Transform transform2 = transform.Find(n);
			if (transform2 != null)
			{
				cakes[i] = transform2.GetComponent<Renderer>();
			}
		}
	}

	public void SetDialog(LeaderboardDialog parentDialog)
	{
		dialog = parentDialog;
	}

	public void SetInfo(int track, CakeRaceReplay replay)
	{
		if (replay == null || !replay.IsValid)
		{
			SetInfo(track, 0, 0, kingsFavorite: false);
			return;
		}
		int score = CakeRaceReplay.TotalScore(replay);
		int num = replay.GetCollectedCakeCount();
		if (replay.GetCakeCollectTime(-1) >= 0)
		{
			num--;
		}
		SetInfo(track, score, num, replay.HasKingsFavoritePart);
	}

	public void SetInfo(int track, int score, int cakeCount, bool kingsFavorite)
	{
		TextMeshHelper.UpdateTextMeshes(trackLabel, $"{track + 1}");
		TextMeshHelper.UpdateTextMeshes(scoreLabel, (score > 0) ? $"{score:n0}" : "-");
		kingsFavoriteIcon.enabled = kingsFavorite;
		for (int i = 0; i < cakes.Length; i++)
		{
			cakes[i].enabled = i < cakeCount;
		}
	}
}
