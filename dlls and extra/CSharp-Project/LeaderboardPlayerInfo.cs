using System.Collections.Generic;
using CakeRace;
using UnityEngine;

public class LeaderboardPlayerInfo : MonoBehaviour
{
	private TextMesh[] rankLabel;

	private TextMesh[] scoreLabel;

	private TextMesh[] coinRewardLabel;

	private TextMesh[] totalCoinsWonLabel;

	private TextMesh[] highestRankLabel;

	private Transform trophies;

	private Transform[] trophyIcons;

	private MeshRenderer[] rankBackgrounds;

	private List<TextMesh[]> trophyAmountLabels;

	private Transform rewardCrateContainer;

	private Dictionary<int, CakeRaceReplayEntry> replayEntries;

	private VerticalScroller scroller;

	private bool isInit;

	public int CakeRaceReplayCount
	{
		get
		{
			if (replayEntries == null)
			{
				return 0;
			}
			return replayEntries.Count;
		}
	}

	private void Init()
	{
		if (isInit)
		{
			return;
		}
		Transform transform = base.transform.Find("RankScore/RankLabel");
		if (transform != null)
		{
			rankLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		string[] array = new string[4] { "1", "2", "3", "x" };
		rankBackgrounds = new MeshRenderer[4];
		for (int i = 0; i < rankBackgrounds.Length; i++)
		{
			transform = base.transform.Find("RankScore/RankIcons/" + array[i]);
			if (transform != null)
			{
				rankBackgrounds[i] = transform.GetComponent<MeshRenderer>();
			}
		}
		transform = base.transform.Find("RankScore/ScorePanel/ScoreLabel");
		if (transform != null)
		{
			scoreLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		transform = base.transform.Find("RankScore/Reward/CoinPrize/Amount");
		if (transform != null)
		{
			coinRewardLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		rewardCrateContainer = base.transform.Find("RankScore/Reward/CratePrize");
		trophies = base.transform.Find("Trophies");
		if (trophies != null)
		{
			transform = trophies.Find("CoinsWonPanel/CoinsLabel");
			if (transform != null)
			{
				totalCoinsWonLabel = transform.GetComponentsInChildren<TextMesh>();
			}
			transform = trophies.Find("RankPanel/RankLabel");
			if (transform != null)
			{
				highestRankLabel = transform.GetComponentsInChildren<TextMesh>();
			}
			trophyIcons = new Transform[3];
			trophyAmountLabels = new List<TextMesh[]>();
			for (int j = 1; j <= 3; j++)
			{
				transform = trophies.Find($"Trophies/{j}");
				if (transform != null)
				{
					trophyIcons[j - 1] = transform;
				}
				transform = trophies.Find($"Trophies/{j}Amount");
				if (transform != null)
				{
					trophyAmountLabels.Add(transform.GetComponentsInChildren<TextMesh>());
				}
			}
		}
		scroller = GetComponent<VerticalScroller>();
		isInit = true;
	}

	public void SetRankScoreInfo(int rank, int score, bool isLocalPlayer = false)
	{
		Init();
		trophies.gameObject.SetActive(isLocalPlayer);
		scroller.SetLowerPadding((!isLocalPlayer) ? (-5.5f) : 2f);
		TextMeshHelper.UpdateTextMeshes(rankLabel, (rank > 0) ? rank.ToString() : "-");
		TextMeshHelper.UpdateTextMeshes(scoreLabel, (score > 0) ? $"{score:n0}" : "-");
		for (int i = 0; i < rankBackgrounds.Length; i++)
		{
			if (rankBackgrounds[i] != null)
			{
				rankBackgrounds[i].enabled = i + 1 == rank || (i == 3 && (rank > 3 || rank == 0));
			}
		}
		if (isLocalPlayer)
		{
			UpdatePlayerStats();
		}
	}

	private void UpdatePlayerStats()
	{
		for (int i = 0; i < 3; i++)
		{
			int num = 0;
			switch (i)
			{
			case 2:
				num = GameProgress.GetInt("cake_race_bronze_trophies_won");
				break;
			case 1:
				num = GameProgress.GetInt("cake_race_silver_trophies_won");
				break;
			case 0:
				num = GameProgress.GetInt("cake_race_gold_trophies_won");
				break;
			}
			trophyIcons[i].gameObject.SetActive(num > 0);
			TextMeshHelper.UpdateTextMeshes(trophyAmountLabels[i], (num <= 0) ? string.Empty : num.ToString());
		}
		int @int = GameProgress.GetInt("cake_race_coins_won");
		TextMeshHelper.UpdateTextMeshes(totalCoinsWonLabel, $"[snout] {@int}");
		TextMeshSpriteIcons.EnsureSpriteIcon(totalCoinsWonLabel);
		int int2 = GameProgress.GetInt("cake_race_highest_rank");
		TextMeshHelper.UpdateTextMeshes(highestRankLabel, (int2 <= 0) ? "-" : int2.ToString());
	}

	public void SetRewards(LootCrateType crateType, int snoutCoins)
	{
		TextMeshHelper.UpdateTextMeshes(coinRewardLabel, snoutCoins.ToString());
		for (int i = 0; i < rewardCrateContainer.childCount; i++)
		{
			Transform child = rewardCrateContainer.GetChild(i);
			child.gameObject.SetActive(child.name.Equals(crateType.ToString()));
		}
	}

	public void AddCakeRaceReplayEntry(int index, CakeRaceReplayEntry entry)
	{
		if (replayEntries == null)
		{
			replayEntries = new Dictionary<int, CakeRaceReplayEntry>();
		}
		if (!replayEntries.ContainsKey(index))
		{
			replayEntries.Add(index, entry);
		}
	}

	public void UpdateReplayEntry(int index, CakeRaceReplay replay)
	{
		if (replayEntries != null && replayEntries.ContainsKey(index) && replayEntries[index] != null)
		{
			if (replay == null || !replay.IsValid)
			{
				replayEntries[index].SetInfo(index, 0, 0, kingsFavorite: false);
			}
			else
			{
				replayEntries[index].SetInfo(index, replay);
			}
		}
	}
}
