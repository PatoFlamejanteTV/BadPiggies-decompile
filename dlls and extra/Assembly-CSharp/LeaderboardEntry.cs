using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer[] rankIcons;

	[SerializeField]
	private MeshRenderer[] trophyIcons;

	[SerializeField]
	private GameObject[] crateIcons;

	private string playFabId = string.Empty;

	private string playerName = string.Empty;

	private int playerRank = int.MaxValue;

	private int playerScore;

	private TextMesh[] nameLabel;

	private TextMesh[] scoreLabel;

	private TextMesh[] rankLabel;

	private TextMesh[] coinPrizeLabel;

	private MeshRenderer otherPlayerBackground;

	private MeshRenderer currentPlayerBackground;

	private MeshRenderer currentPlayerInfoButton;

	private Transform coinPrizeContainer;

	private LeaderboardDialog dialog;

	private bool heightAdded;

	public GridLayout ParentGrid { get; set; }

	private void Awake()
	{
		Transform transform = base.transform.Find("PlayerLabel");
		if (transform != null)
		{
			nameLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		transform = base.transform.Find("ScoreLabel");
		if (transform != null)
		{
			scoreLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		transform = base.transform.Find("RankLabel");
		if (transform != null)
		{
			rankLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		transform = base.transform.Find("Background");
		if (transform != null)
		{
			otherPlayerBackground = transform.GetComponent<MeshRenderer>();
		}
		transform = base.transform.Find("BackgroundYou");
		if (transform != null)
		{
			currentPlayerBackground = transform.GetComponent<MeshRenderer>();
		}
		transform = base.transform.Find("BackgroundYou/InfoButton");
		if (transform != null)
		{
			currentPlayerInfoButton = transform.GetComponent<MeshRenderer>();
		}
		coinPrizeContainer = base.transform.Find("CoinPrize");
		transform = coinPrizeContainer.Find("Amount");
		if (transform != null)
		{
			coinPrizeLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		coinPrizeContainer.gameObject.SetActive(value: false);
	}

	public void Init(LeaderboardDialog parentDialog)
	{
		dialog = parentDialog;
		base.gameObject.SetActive(value: false);
		heightAdded = false;
	}

	public void Show(bool show = true)
	{
		base.gameObject.SetActive(show);
		if (show && !heightAdded)
		{
			dialog.IncreaseLeaderboardScrollerHeight(ParentGrid.VerticalGap);
			heightAdded = true;
		}
		else if (!show && heightAdded)
		{
			dialog.DecreaseLeaderboardScrollerHeight(ParentGrid.VerticalGap);
			heightAdded = false;
		}
	}

	public void SetInfo(string playFabId, string name, int score, int rank)
	{
		this.playFabId = playFabId;
		playerName = name;
		playerScore = score;
		playerRank = rank;
		if (string.IsNullOrEmpty(playFabId))
		{
			Show(show: false);
			return;
		}
		Show();
		bool flag = playFabId.Equals(HatchManager.CurrentPlayer.PlayFabID);
		string format = ((!flag) ? "{0}" : string.Format("{0} ({1})", GetLocalizedPlayerName(), "{0}"));
		string[] array = name.Split('|');
		if (array != null && array.Length != 0)
		{
			playerName = string.Format(format, array[0]);
		}
		else
		{
			playerName = string.Format(format, name);
		}
		TextMeshHelper.UpdateTextMeshes(nameLabel, playerName);
		TextMeshHelper.UpdateTextMeshes(scoreLabel, $"{score:n0}");
		if (rank >= 0)
		{
			SetRank(rank + 1);
		}
		else
		{
			SetRank(rank);
		}
		if (otherPlayerBackground != null)
		{
			otherPlayerBackground.enabled = !flag;
		}
		if (currentPlayerBackground != null)
		{
			currentPlayerBackground.enabled = flag;
		}
		if (currentPlayerInfoButton != null)
		{
			currentPlayerInfoButton.enabled = flag;
		}
	}

	public void SetRewards(LootCrateType crateType, int coinAmount)
	{
		for (int i = 0; i < crateIcons.Length; i++)
		{
			if (crateIcons[i] != null)
			{
				crateIcons[i].SetActive(crateType != LootCrateType.None && i == Mathf.Clamp((int)crateType, 0, 6));
			}
		}
		if (coinPrizeContainer != null)
		{
			TextMeshHelper.UpdateTextMeshes(coinPrizeLabel, coinAmount.ToString());
			coinPrizeContainer.gameObject.SetActive(coinAmount > 0);
		}
	}

	private void SetRank(int rank)
	{
		TextMeshHelper.ForceWrapText(rankLabel, (rank >= 0) ? rank.ToString() : "-", 3);
		rankLabel[0].transform.localScale = ((rank < 100) ? new Vector3(0.12f, 0.12f) : new Vector3(0.09f, 0.09f));
		if (rankIcons == null || trophyIcons == null)
		{
			return;
		}
		if (rank < 0)
		{
			rank = 4;
		}
		for (int i = 0; i < rankIcons.Length; i++)
		{
			if (rankIcons[i] != null)
			{
				rankIcons[i].enabled = i + 1 == Mathf.Clamp(rank, 1, 4);
			}
			if (i < trophyIcons.Length && trophyIcons[i] != null)
			{
				trophyIcons[i].enabled = i + 1 == Mathf.Clamp(rank, 1, 4);
			}
		}
	}

	private string GetLocalizedPlayerName()
	{
		return Singleton<Localizer>.Instance.Resolve("YOUR_NAME_LEADERBOARDS").translation;
	}

	public void VerticalScrollButtonActivate()
	{
		if (dialog != null && !string.IsNullOrEmpty(playFabId))
		{
			dialog.ShowPlayerInfo(playerName, playerScore, playerRank, playFabId);
		}
	}
}
