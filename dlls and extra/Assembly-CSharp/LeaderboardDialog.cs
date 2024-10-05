using System.Collections;
using System.Collections.Generic;
using CakeRace;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class LeaderboardDialog : TextDialog
{
	private enum LeaderboardView
	{
		Error,
		Loading,
		List,
		PlayerInfo,
		CupInfo
	}

	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private GameObject singleEntryPrefab;

	[SerializeField]
	private GameObject cakeRaceReplayEntryPrefab;

	[SerializeField]
	private GameObject[] viewContainers;

	[SerializeField]
	private GameObject cupButton;

	[SerializeField]
	private TextMesh[] daysLeftTextMesh;

	[SerializeField]
	private string daysLeftKey;

	[SerializeField]
	private int topRanks = 50;

	[SerializeField]
	private int[] singleRanks;

	private GridLayout top50Grid;

	private GridLayout singleRanksGrid;

	private GridLayout replayGrid;

	private string previousTitle = string.Empty;

	private string currentTitle = string.Empty;

	private LeaderboardView previousView = LeaderboardView.List;

	private LeaderboardView currentView = LeaderboardView.List;

	private LeaderboardView loadingView = LeaderboardView.List;

	private Dictionary<int, LeaderboardEntry> leaderboardEntries;

	private Dictionary<int, CakeRaceReplayEntry> cakeRaceReplayEntries;

	private LeaderboardPlayerInfo playerInfo;

	private LeaderboardCupInfo cupInfo;

	private int fetchingSingleLeaderboardEntryPosition = -1;

	private TextMesh[] titleLabel;

	private int showingCupAnimationPhase = -1;

	private int localPlayerRank = -1;

	private VerticalScroller leaderboardListScroller;

	private List<PlayerLeaderboardEntry> currentLeaderboard;

	private List<PlayerLeaderboardEntry> currentPlayerLeaderboard;

	private bool snoutCoinShopChangedToCupView;

	public bool ShowingCupAnimation => showingCupAnimationPhase >= 0;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (titleLabel == null)
		{
			Transform transform = dialogRoot.transform.Find("Title/Label");
			if (transform != null)
			{
				titleLabel = transform.GetComponentsInChildren<TextMesh>();
			}
		}
		Transform transform2 = dialogRoot.transform.Find("ListContainer");
		leaderboardListScroller = transform2.GetComponent<VerticalScroller>();
		Transform transform3 = dialogRoot.transform.Find("ListContainer/SingleRankGrid");
		if (transform3 != null)
		{
			singleRanksGrid = transform3.GetComponent<GridLayout>();
		}
		Transform transform4 = dialogRoot.transform.Find("ListContainer/Top50Grid");
		if (transform4 != null)
		{
			top50Grid = transform4.GetComponent<GridLayout>();
		}
		if (leaderboardEntries == null)
		{
			DecreaseLeaderboardScrollerHeight(top50Grid.VerticalGap);
			leaderboardEntries = new Dictionary<int, LeaderboardEntry>();
			if (singleRanks != null)
			{
				_ = singleRanks.Length;
			}
			for (int i = 0; i < TotalEntryCount(); i++)
			{
				string text = $"{i:0000}";
				GameObject gameObject = Object.Instantiate((i < 50) ? entryPrefab : singleEntryPrefab);
				gameObject.transform.parent = ((i < 50) ? top50Grid.transform : singleRanksGrid.transform);
				gameObject.transform.SetAsLastSibling();
				gameObject.name = text;
				LeaderboardEntry component = gameObject.transform.GetComponent<LeaderboardEntry>();
				component.Init(this);
				component.ParentGrid = ((i < 50) ? top50Grid : singleRanksGrid);
				leaderboardEntries.Add(i, component);
				if (transform4 != null)
				{
					gameObject.transform.GetComponent<VerticalScrollButton>().SetScroller(leaderboardListScroller);
				}
			}
		}
		UpdateGridLayout();
		if (playerInfo == null)
		{
			transform2 = dialogRoot.transform.Find("PlayerInfoContainer");
			VerticalScroller component2 = transform2.GetComponent<VerticalScroller>();
			transform4 = transform2.Find("ReplayGrid");
			if (transform4 != null)
			{
				replayGrid = transform4.GetComponent<GridLayout>();
			}
			playerInfo = transform2.gameObject.AddComponent<LeaderboardPlayerInfo>();
			for (int j = 0; j < 7; j++)
			{
				string text2 = $"{j:0000}";
				GameObject obj = Object.Instantiate(cakeRaceReplayEntryPrefab);
				obj.transform.parent = replayGrid.transform;
				obj.transform.SetAsLastSibling();
				obj.name = text2;
				CakeRaceReplayEntry component3 = obj.transform.GetComponent<CakeRaceReplayEntry>();
				component3.SetDialog(this);
				playerInfo.AddCakeRaceReplayEntry(j, component3);
				if (replayGrid != null && j > 0)
				{
					component2.AddHeight(replayGrid.VerticalGap);
				}
			}
		}
		replayGrid.UpdateLayout();
		if (cupInfo == null)
		{
			transform2 = dialogRoot.transform.Find("CupInfoContainer");
			cupInfo = transform2.gameObject.AddComponent<LeaderboardCupInfo>();
			cupInfo.Init(this);
		}
		StartCoroutine(FetchLeaderboard());
		SetTitle(GetCupAndSeasonTitle());
		UpdateDaysLeft();
		EventManager.Connect<UIEvent>(OnReceivedUIEvent);
	}

	private IEnumerator FetchLeaderboard()
	{
		ChangeView(LeaderboardView.Loading, string.Empty);
		loadingView = LeaderboardView.List;
		currentLeaderboard = null;
		currentPlayerLeaderboard = null;
		Singleton<PlayFabManager>.Instance.Leaderboard.GetLeaderboard(CakeRaceMenu.GetCurrentLeaderboardCup(), OnTopLeaderboardResult, OnLeaderboardError, previousSeason: false, 0, topRanks);
		Singleton<PlayFabManager>.Instance.Leaderboard.GetLeaderboardAroundPlayer(CakeRaceMenu.GetCurrentLeaderboardCup(), OnPlayerLeaderboardResult, OnLeaderboardError);
		while (currentLeaderboard == null || currentPlayerLeaderboard == null)
		{
			yield return null;
		}
		if (currentView == LeaderboardView.Loading && loadingView == LeaderboardView.List)
		{
			UpdateLeaderboard(currentLeaderboard, currentPlayerLeaderboard);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		EventManager.Disconnect<UIEvent>(OnReceivedUIEvent);
	}

	public new void Close()
	{
		if (showingCupAnimationPhase != 0)
		{
			if (showingCupAnimationPhase == 1 || currentView == LeaderboardView.List || loadingView == LeaderboardView.List || currentView == LeaderboardView.Error)
			{
				showingCupAnimationPhase = -1;
				base.Close();
			}
			else if (currentView == LeaderboardView.PlayerInfo || currentView == LeaderboardView.CupInfo)
			{
				ChangeView(LeaderboardView.List, GetCupAndSeasonTitle());
			}
			else
			{
				ChangeView(previousView, previousTitle);
			}
		}
	}

	private void OnReceivedUIEvent(UIEvent data)
	{
		switch (data.type)
		{
		case UIEvent.Type.OpenedLootWheel:
		case UIEvent.Type.OpenedSnoutCoinShop:
			Hide();
			break;
		case UIEvent.Type.ClosedLootWheel:
			if (!Singleton<IapManager>.Instance.IsShopPageOpened() && !SnoutCoinShopPopup.DialogOpen)
			{
				Show();
			}
			break;
		case UIEvent.Type.ClosedSnoutCoinShop:
			if (!LootWheelPopup.DialogOpen)
			{
				Show();
			}
			break;
		}
	}

	private void SetTitle(string title)
	{
		TextMeshHelper.UpdateTextMeshes(titleLabel, title);
	}

	private string GetCupAndSeasonTitle()
	{
		Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve("LEADERBOARDS_TITLE");
		return string.Format(arg0: Singleton<Localizer>.Instance.Resolve($"CUP_{(int)CakeRaceMenu.GetCurrentLeaderboardCup():00}_NAME").translation, format: localeParameters.translation, arg1: CakeRaceMenu.CurrentCakeRaceWeek());
	}

	private int TotalEntryCount()
	{
		int num = 0;
		if (singleRanks != null)
		{
			num = singleRanks.Length;
		}
		return topRanks + num + 1;
	}

	private int HighestRankEntry()
	{
		if (singleRanks != null && singleRanks.Length != 0)
		{
			return singleRanks[singleRanks.Length - 1];
		}
		return topRanks;
	}

	private void ChangeView(LeaderboardView newView, string newTitle = "")
	{
		Debug.LogWarning("[LeaderboardDialog] ChangeView(" + newView.ToString() + ")");
		loadingView = LeaderboardView.Loading;
		previousTitle = currentTitle;
		previousView = currentView;
		if (!string.IsNullOrEmpty(newTitle))
		{
			currentTitle = newTitle;
		}
		currentView = newView;
		if (viewContainers == null)
		{
			return;
		}
		int num = (int)currentView;
		for (int i = 0; i < viewContainers.Length; i++)
		{
			if (viewContainers[i] != null)
			{
				viewContainers[i].SetActive(num == i);
			}
		}
		if (!string.IsNullOrEmpty(newTitle))
		{
			SetTitle(newTitle);
		}
		cupButton.SetActive(currentView != LeaderboardView.CupInfo && currentView != LeaderboardView.Loading);
	}

	private int GetKeyFromIndex(int index)
	{
		if (index < 0)
		{
			return TotalEntryCount() - 1;
		}
		if (index < topRanks)
		{
			return index;
		}
		int num = index - topRanks;
		if (singleRanks != null && num >= 0 && num < singleRanks.Length)
		{
			return singleRanks[num];
		}
		return -1;
	}

	private int GetIndexFromRank(int rank)
	{
		if (rank <= topRanks)
		{
			return rank - 1;
		}
		if (singleRanks != null && singleRanks.Length != 0)
		{
			for (int i = 0; i < singleRanks.Length; i++)
			{
				if (rank == singleRanks[i])
				{
					return topRanks + i;
				}
			}
		}
		return -1;
	}

	public void IncreaseLeaderboardScrollerHeight(float value)
	{
		leaderboardListScroller.AddHeight(value);
	}

	public void DecreaseLeaderboardScrollerHeight(float value)
	{
		leaderboardListScroller.AddHeight(0f - value);
	}

	private void UpdateLeaderboard(List<PlayerLeaderboardEntry> leaderboard, List<PlayerLeaderboardEntry> player)
	{
		if ((entryPrefab == null || top50Grid == null || singleRanks == null) && !ShowingCupAnimation)
		{
			ChangeView(LeaderboardView.Error, string.Empty);
		}
		localPlayerRank = ((player.Count <= 0) ? (-1) : ((player[0].StatValue <= 0) ? (-1) : player[0].Position));
		LeaderboardEntry leaderboardEntry = null;
		for (int i = 0; i < topRanks; i++)
		{
			int keyFromIndex = GetKeyFromIndex(i);
			if (!leaderboardEntries.ContainsKey(keyFromIndex))
			{
				continue;
			}
			if (i < leaderboard.Count)
			{
				if (HatchManager.CurrentPlayer.PlayFabID.Equals(leaderboard[i].PlayFabId))
				{
					leaderboardEntry = leaderboardEntries[keyFromIndex];
				}
				leaderboardEntries[keyFromIndex].SetInfo(leaderboard[i].PlayFabId, leaderboard[i].DisplayName, leaderboard[i].StatValue, leaderboard[i].Position);
			}
			else
			{
				leaderboardEntries[keyFromIndex].SetInfo(string.Empty, "- empty -", 0, i);
			}
			leaderboardEntries[keyFromIndex].SetRewards(CakeRaceMenu.GetSeasonCrateReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), i + 1), CakeRaceMenu.GetSeasonSnoutCoinReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), i + 1));
		}
		for (int j = 0; j < singleRanks.Length; j++)
		{
			int indexFromRank = GetIndexFromRank(singleRanks[j]);
			if (leaderboardEntries.ContainsKey(indexFromRank))
			{
				leaderboardEntries[indexFromRank].SetInfo(string.Empty, "- empty -", 0, singleRanks[j]);
			}
		}
		for (int k = 0; k < singleRanks.Length; k++)
		{
			if (localPlayerRank <= topRanks && localPlayerRank >= 0)
			{
				break;
			}
			int indexFromRank2 = GetIndexFromRank(singleRanks[k]);
			int num = singleRanks[k] - 1;
			if (leaderboardEntries.ContainsKey(indexFromRank2))
			{
				if (localPlayerRank <= num && localPlayerRank >= 0)
				{
					leaderboardEntry = leaderboardEntries[indexFromRank2];
					leaderboardEntry.SetInfo(player[0].PlayFabId, player[0].DisplayName, player[0].StatValue, localPlayerRank);
					leaderboardEntry.SetRewards(CakeRaceMenu.GetSeasonCrateReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), localPlayerRank + 1), CakeRaceMenu.GetSeasonSnoutCoinReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), localPlayerRank + 1));
					break;
				}
				if (num < leaderboard.Count)
				{
					leaderboardEntries[indexFromRank2].SetInfo(leaderboard[num].PlayFabId, leaderboard[num].DisplayName, leaderboard[num].StatValue, num);
					leaderboardEntries[indexFromRank2].SetRewards(CakeRaceMenu.GetSeasonCrateReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), num + 1), CakeRaceMenu.GetSeasonSnoutCoinReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), num + 1));
				}
			}
		}
		if (localPlayerRank < 0)
		{
			int keyFromIndex2 = GetKeyFromIndex((leaderboard.Count > 0) ? (-1) : 0);
			leaderboardEntry = leaderboardEntries[keyFromIndex2];
			leaderboardEntry.SetInfo(HatchManager.CurrentPlayer.PlayFabID, HatchManager.CurrentPlayer.PlayFabDisplayName, 0, -1);
			leaderboardEntry.SetRewards(LootCrateType.None, 0);
		}
		else if (localPlayerRank >= HighestRankEntry())
		{
			int keyFromIndex3 = GetKeyFromIndex(-1);
			leaderboardEntry = leaderboardEntries[keyFromIndex3];
			leaderboardEntry.SetInfo(player[0].PlayFabId, player[0].DisplayName, player[0].StatValue, localPlayerRank);
			leaderboardEntry.SetRewards(CakeRaceMenu.GetSeasonCrateReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), localPlayerRank + 1), CakeRaceMenu.GetSeasonSnoutCoinReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), localPlayerRank + 1));
		}
		else
		{
			int keyFromIndex4 = GetKeyFromIndex(-1);
			leaderboardEntries[keyFromIndex4].SetInfo(string.Empty, "- empty -", 0, -1);
		}
		if (!ShowingCupAnimation)
		{
			ChangeView(LeaderboardView.List, string.Empty);
		}
		UpdateGridLayout();
		PositionLeaderboard(leaderboardEntry);
	}

	private void UpdateGridLayout()
	{
		top50Grid.UpdateLayout();
		singleRanksGrid.UpdateLayout();
		Transform transform = null;
		for (int i = 0; i < top50Grid.transform.childCount; i++)
		{
			Transform child = top50Grid.transform.GetChild(i);
			if (child.gameObject.activeSelf)
			{
				transform = child;
			}
		}
		if (transform != null)
		{
			singleRanksGrid.transform.position = transform.transform.position + Vector3.down * singleRanksGrid.VerticalGap;
		}
	}

	private void PositionLeaderboard(LeaderboardEntry target)
	{
		if (target == null)
		{
			return;
		}
		Transform transform = null;
		for (int i = 0; i < top50Grid.transform.childCount; i++)
		{
			Transform child = top50Grid.transform.GetChild(i);
			if (child.gameObject.activeSelf)
			{
				transform = child;
			}
		}
		if (!(transform == null))
		{
			float upperBound = leaderboardListScroller.UpperBound;
			Transform transform2 = target.transform;
			upperBound -= transform2.localPosition.y;
			upperBound -= (Mathf.Abs(leaderboardListScroller.UpperBound) + Mathf.Abs(leaderboardListScroller.LowerBound)) / 2f;
			singleRanksGrid.transform.position = transform.transform.position + Vector3.down * singleRanksGrid.VerticalGap;
			if (transform2.parent == singleRanksGrid.transform)
			{
				upperBound -= transform.transform.localPosition.y + singleRanksGrid.VerticalGap;
			}
			upperBound = Mathf.Clamp(upperBound, leaderboardListScroller.UpperBound, leaderboardListScroller.LowerBound + leaderboardListScroller.TotalHeight);
			top50Grid.transform.localPosition = new Vector3(0f, upperBound);
			singleRanksGrid.transform.position = transform.transform.position + Vector3.down * singleRanksGrid.VerticalGap;
		}
	}

	private void OnTopLeaderboardResult(GetLeaderboardResult result)
	{
		currentLeaderboard = result.Leaderboard;
	}

	private void OnPlayerLeaderboardResult(GetLeaderboardAroundPlayerResult result)
	{
		currentPlayerLeaderboard = result.Leaderboard;
	}

	private void OnLeaderboardError(PlayFabError error)
	{
		if (currentView == LeaderboardView.Loading && loadingView == LeaderboardView.List)
		{
			ChangeView(LeaderboardView.Error, string.Empty);
		}
	}

	public void ShowPlayerInfo(string playerName, int playerScore, int playerRank, string playfabID)
	{
		string[] array = playerName.Split('|');
		ChangeView(LeaderboardView.Loading, array[0]);
		loadingView = LeaderboardView.PlayerInfo;
		playerInfo.SetRankScoreInfo(playerRank + 1, playerScore, playfabID.Equals(HatchManager.CurrentPlayer.PlayFabID));
		playerInfo.SetRewards(CakeRaceMenu.GetSeasonCrateReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), playerRank + 1), CakeRaceMenu.GetSeasonSnoutCoinReward((int)CakeRaceMenu.GetCurrentLeaderboardCup(), playerRank + 1));
		Singleton<PlayFabManager>.Instance.Users.GetUserReplays(playfabID, OnPlayerInfoResult, OnPlayerInfoError);
	}

	private void UpdatePlayerInfo(Dictionary<string, UserDataRecord> replayData)
	{
		if (replayData == null)
		{
			ChangeView(LeaderboardView.List, string.Empty);
			return;
		}
		for (int i = 0; i < playerInfo.CakeRaceReplayCount; i++)
		{
			string text = $"replay_track_{i}";
			if (replayData.ContainsKey(text))
			{
				CakeRaceReplay cakeRaceReplay = new CakeRaceReplay(replayData[text].Value);
				Debug.LogWarning("[UpdatePlayerInfo] replay [" + text + "][" + replayData[text].Value + "]");
				playerInfo.UpdateReplayEntry(i, (!CakeRaceMenu.IsWeeklyTrack(i, cakeRaceReplay.UniqueIdentifier, ignoreTutorial: true)) ? null : cakeRaceReplay);
			}
			else
			{
				playerInfo.UpdateReplayEntry(i, null);
			}
		}
		ChangeView(LeaderboardView.PlayerInfo, string.Empty);
	}

	private void OnPlayerInfoResult(GetUserDataResult result)
	{
		if (currentView == LeaderboardView.Loading && loadingView == LeaderboardView.PlayerInfo)
		{
			UpdatePlayerInfo(result.Data);
		}
	}

	private void OnPlayerInfoError(PlayFabError error)
	{
		if (currentView == LeaderboardView.Loading && loadingView == LeaderboardView.PlayerInfo)
		{
			UpdatePlayerInfo(null);
		}
	}

	private void UpdateDaysLeft()
	{
		string arg = $"{SeasonDaysLeft()}";
		Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve(daysLeftKey);
		TextMeshHelper.UpdateTextMeshes(daysLeftTextMesh, string.Format(localeParameters.translation, arg));
	}

	private int SeasonDaysLeft()
	{
		return (0 - Singleton<TimeManager>.Instance.ServerTime.DayOfWeek + 7) % 7;
	}

	public void OpenCupView()
	{
		ChangeView(LeaderboardView.CupInfo, GetCakeRaceCupsTitle());
	}

	public void ShowCupAnimation(int cupIndex)
	{
		showingCupAnimationPhase = 0;
		OpenCupView();
		cupInfo.ShowCupAnimation(cupIndex, delegate
		{
			showingCupAnimationPhase = 1;
		});
	}

	private string GetCakeRaceCupsTitle()
	{
		return Singleton<Localizer>.Instance.Resolve("CR_CUP_TITLE").translation;
	}
}
