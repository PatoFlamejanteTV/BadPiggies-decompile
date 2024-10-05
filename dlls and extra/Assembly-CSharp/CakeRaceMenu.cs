using System;
using System.Collections;
using System.Collections.Generic;
using CakeRace;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class CakeRaceMenu : WPFMonoBehaviour
{
	[SerializeField]
	private Color skyColor;

	[SerializeField]
	private MeshRenderer cloudRenderer;

	[SerializeField]
	private float cloudSpeed = 0.1f;

	[SerializeField]
	private TextMesh[] infoLabel;

	[SerializeField]
	private TextMesh[] daysLeftLabel;

	[SerializeField]
	private string daysLeftLabelKey;

	[SerializeField]
	private TextMesh[] winsLabel;

	[SerializeField]
	private string winsLabelKey;

	[SerializeField]
	private LootCrateSlots lootCrateSlots;

	[SerializeField]
	private GameObject lockScreen;

	[SerializeField]
	private GameObject leaderboardButton;

	[SerializeField]
	private GameObject lockScreenLoading;

	[SerializeField]
	private string cakeRaceVersionErrorKey;

	[SerializeField]
	private string offlineErrorKey;

	[SerializeField]
	private string cakeRaceDisabledErrorKey;

	[SerializeField]
	private GameObject leaderboardDialogPrefab;

	[SerializeField]
	private GameObject seasonEndDialogPopup;

	[SerializeField]
	private GameObject[] cupIcons;

	private LeaderboardDialog leaderboardDialog;

	private LeaderboardSeasonEndDialog seasonEndDialog;

	private float currentOffset;

	private bool fetchingWeeklyTrackData;

	private bool isRewardingPlayer;

	private int currentWeeklyTrackIndex;

	private PlayFabLeaderboard.Leaderboard rewardPendingCup;

	private GameObject backButton;

	public static int WinCount = -1;

	public const string GAMEPROGRESS_CAKE_RACE_CUP_KEY = "cake_race_current_cup";

	public const string GAMEPROGRESS_CAKE_RACE_WEEK_KEY = "cake_race_current_week";

	private const string GAMEPROGRESS_SHOW_CUP_ANIMATION = "cake_race_show_cup_animation";

	public const string GAMEPROGRESS_GOLD_TROPHIES_WON = "cake_race_gold_trophies_won";

	public const string GAMEPROGRESS_SILVER_TROPHIES_WON = "cake_race_silver_trophies_won";

	public const string GAMEPROGRESS_BRONZE_TROPHIES_WON = "cake_race_bronze_trophies_won";

	public const string GAMEPROGRESS_CAKE_RACE_COINS_WON = "cake_race_coins_won";

	public const string GAMEPROGRESS_CAKE_RACE_HIGHEST_RANK = "cake_race_highest_rank";

	private const string WEEKLY_CAKE_RACE_TRACKS_KEY = "weekly_cake_race_tracks";

	private const string WEEKLY_CAKE_RACE_OFFSET_KEY = "week_offset";

	private const string CAKE_RACE_KEY = "cake_race";

	private const string CAKE_RACE_CUP_REQUIREMENTS = "cake_race_cup_requirements";

	private const string CAKE_RACE_CUP_SNOUT_REWARDS = "cake_race_cup_snout_rewards";

	private const string CAKE_RACE_CUP_REWARD_MULTIPLIERS = "cake_race_cup_reward_multipliers";

	private const string CAKE_RACE_CUP_CRATE_REWARDS = "cake_race_cup_crate_rewards";

	private static int CurrentSeasonIndex { get; set; }

	public static bool FindNewPlayer { get; set; }

	public static bool UseDefaultReplay { get; set; }

	public static bool IsCakeRaceMenuOpen { get; private set; }

	public static bool CakeRaceDisabled
	{
		get
		{
			if (Singleton<GameConfigurationManager>.IsInstantiated())
			{
				return Singleton<GameConfigurationManager>.Instance.GetValue<bool>("cake_race", "cake_race_disabled");
			}
			return false;
		}
	}

	public static bool IsTutorial => GameProgress.GetInt("cake_race_total_wins") < 3;

	public static string[] WeeklyTrackIdentifiers
	{
		get
		{
			int currentSeasonIndex = CurrentSeasonIndex;
			if (AllSeasonTracks != null && AllSeasonTracks.ContainsKey(currentSeasonIndex))
			{
				return AllSeasonTracks[currentSeasonIndex].ToArray();
			}
			return null;
		}
	}

	public static Dictionary<int, List<string>> AllSeasonTracks { get; private set; }

	private void Awake()
	{
		IsCakeRaceMenuOpen = true;
		EventManager.Connect<PlayerChangedEvent>(OnPlayerChanged);
		rewardPendingCup = PlayFabLeaderboard.LowestCup();
		Transform transform = base.transform.Find("HomeButton");
		if (transform != null)
		{
			backButton = transform.gameObject;
		}
	}

	private void OnDestroy()
	{
		IsCakeRaceMenuOpen = false;
		EventManager.Disconnect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnEnable()
	{
		TextMeshHelper.UpdateTextMeshes(daysLeftLabel, string.Empty);
		TextMeshHelper.UpdateTextMeshes(winsLabel, string.Empty);
		WPFMonoBehaviour.mainCamera.backgroundColor = skyColor;
		KeyListener.keyReleased += HandleKeyListenerkeyReleased;
		if (!CakeRaceDisabled)
		{
			SetLockScreen(locked: true, string.Empty, loading: true);
			Singleton<NetworkManager>.Instance.CheckAccess(delegate(bool online)
			{
				if (online && !HatchManager.HasLoginError)
				{
					InitRaceMenu();
				}
				else
				{
					SetLockScreen(locked: true, offlineErrorKey, loading: false);
				}
			});
		}
		else
		{
			SetLockScreen(locked: true, cakeRaceDisabledErrorKey, loading: false);
		}
		if (!GameProgress.GetBool("CakeRaceUnlockShown"))
		{
			GameProgress.SetBool("CakeRaceUnlockShown", value: true);
		}
		InitSeasonTracks();
		bool active = false;
		if (infoLabel != null)
		{
			for (int i = 0; i < infoLabel.Length; i++)
			{
				if (infoLabel[i] != null)
				{
					infoLabel[i].gameObject.SetActive(active);
				}
			}
		}
		if (leaderboardButton != null)
		{
			Transform transform = leaderboardButton.transform.Find("Lock");
			if (IsTutorial)
			{
				transform.gameObject.SetActive(value: true);
				leaderboardButton.GetComponent<Button>().MethodToCall.Reset();
				leaderboardButton.AddComponent<ButtonAnimation>().ActivateAnimationName = "ButtonShake";
			}
			else
			{
				transform.gameObject.SetActive(value: false);
			}
			bool @bool = GameProgress.GetBool("leaderboard_opened");
			leaderboardButton.transform.Find("NewContentTag").gameObject.SetActive(!IsTutorial && !@bool);
		}
		UpdateCupIcon();
	}

	private void OnPlayerChanged(PlayerChangedEvent data)
	{
		if (Singleton<PlayerProgress>.IsInstantiated())
		{
			if (Singleton<PlayerProgress>.Instance.Level < PlayerLevelRequirement.GetRequiredLevel("cake_race"))
			{
				Singleton<GameManager>.Instance.LoadMainMenu(showLoadingScreen: false);
			}
			else
			{
				Singleton<GameManager>.Instance.LoadCakeRaceMenu();
			}
		}
		else
		{
			Singleton<GameManager>.Instance.LoadMainMenu(showLoadingScreen: false);
		}
	}

	private void InitSeasonTracks()
	{
		CurrentSeasonIndex = 0;
		AllSeasonTracks = new Dictionary<int, List<string>>();
		if (!Singleton<GameConfigurationManager>.Instance.HasConfig("weekly_cake_race_tracks"))
		{
			return;
		}
		ConfigData config = Singleton<GameConfigurationManager>.Instance.GetConfig("weekly_cake_race_tracks");
		for (int i = 0; i < config.Count; i++)
		{
			string key = i.ToString();
			if (config.HasKey(key))
			{
				List<string> value = new List<string>(config[key].Split(','));
				AllSeasonTracks.Add(i, value);
			}
		}
	}

	private void InitRaceMenu()
	{
		if (FindNewPlayer && !lootCrateSlots.IsPopUpOpen())
		{
			SetInfoLabel("Seaching for new opponent...");
			StartRace();
			return;
		}
		UpdateWinCount();
		UpdateDaysLeft();
		SetInfoLabel("Connecting...");
		UpdateWeeklyTracks();
		if (!fetchingWeeklyTrackData)
		{
			SetInfoLabel("Ready to go");
		}
	}

	private void SetLockScreen(bool locked, string localeKey, bool loading)
	{
		if (lockScreen != null)
		{
			lockScreen.SetActive(locked);
			TextMeshHelper.UpdateTextMeshes(lockScreen.GetComponentsInChildren<TextMesh>(), localeKey, !string.IsNullOrEmpty(localeKey));
		}
		if (lockScreenLoading != null)
		{
			lockScreenLoading.SetActive(loading);
		}
	}

	private void UpdateCupIcon()
	{
		int num = -1;
		if (!IsTutorial)
		{
			num = (int)(GetCurrentLeaderboardCup() - 1);
		}
		for (int i = 0; i < cupIcons.Length; i++)
		{
			cupIcons[i].SetActive(num == i);
		}
	}

	private void UpdateWeeklyTracks()
	{
		if (!fetchingWeeklyTrackData)
		{
			fetchingWeeklyTrackData = true;
			Singleton<PlayFabManager>.Instance.MatchMaking.GetCakeRaceWeek(OnCakeRaceWeekFetched);
		}
	}

	private void OnCakeRaceWeekFetched(string week, string daysLeft)
	{
		if (!IsCakeRaceMenuOpen)
		{
			return;
		}
		fetchingWeeklyTrackData = false;
		int num = GameProgress.GetInt("cake_race_current_cup", (int)PlayFabLeaderboard.LowestCup());
		if (int.TryParse(week, out var result))
		{
			int num2 = CurrentCakeRaceWeek();
			GameProgress.SetInt("cake_race_current_week", result);
			if (!IsTutorial && num2 != result)
			{
				PlayFabLeaderboard.Leaderboard currentLeaderboardCup = GetCurrentLeaderboardCup();
				Singleton<CakeRaceKingsFavorite>.Instance.ClearCurrentFavorite();
				CakeRaceMode.ClearPersonalBestData();
				ClearCloudTrackData();
				int cupIndexFromPlayerLevel = GetCupIndexFromPlayerLevel();
				if (num != cupIndexFromPlayerLevel)
				{
					num = cupIndexFromPlayerLevel;
					GameProgress.SetInt("cake_race_current_cup", num);
					GameProgress.SetBool("cake_race_show_cup_animation", value: true);
				}
				if (GameProgress.HasKey("cake_race_current_cup"))
				{
					RewardCupPlayer(currentLeaderboardCup);
				}
				else
				{
					GameProgress.SetInt("cake_race_current_cup", num);
					StartCoroutine(WaitPopUpAndShowCupEndAnimation());
				}
			}
		}
		else
		{
			result = 0;
		}
		PlayFabLeaderboard.Leaderboard leaderboard = (PlayFabLeaderboard.Leaderboard)num;
		Debug.LogWarning("[CakeRaceMenu] current cup is " + leaderboard);
		int num3 = 0;
		if (Singleton<GameConfigurationManager>.Instance.HasValue("cake_race", "week_offset"))
		{
			num3 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("cake_race", "week_offset");
		}
		CurrentSeasonIndex = Mathf.Clamp(result + num3, 0, int.MaxValue) % WeeklyTrackIdentifiers.Length;
		if (!AllSeasonTracks.ContainsKey(CurrentSeasonIndex))
		{
			CurrentSeasonIndex = 0;
		}
		if (HasWeeklyData())
		{
			SetInfoLabel("Ready to go");
		}
		else
		{
			SetInfoLabel("Error fetching tracks");
		}
		UpdateDaysLeft();
		if (!isRewardingPlayer)
		{
			TryToUnlockCakeRaceLockScreen();
		}
	}

	private IEnumerator WaitPopUpAndShowCupEndAnimation()
	{
		WaitForSeconds wfs = new WaitForSeconds(0.1f);
		while (lootCrateSlots.IsPopUpOpen())
		{
			yield return wfs;
		}
		TryToShowCupEndAnimation(forceShow: true);
	}

	private void TryToUnlockCakeRaceLockScreen()
	{
		bool locked = true;
		if (Singleton<GameConfigurationManager>.Instance.HasValue("cake_race", "version"))
		{
			string value = Singleton<GameConfigurationManager>.Instance.GetValue<string>("cake_race", "version");
			if (!string.IsNullOrEmpty(value) && value.Equals("1.0.0"))
			{
				locked = false;
			}
		}
		SetLockScreen(locked, cakeRaceVersionErrorKey, loading: false);
	}

	public static int GetCupIndexFromPlayerLevel()
	{
		PlayFabLeaderboard.LowestCup();
		int level = Singleton<PlayerProgress>.Instance.Level;
		if (Singleton<PlayerProgress>.IsInstantiated())
		{
			for (int num = (int)PlayFabLeaderboard.HighestCup(); num >= (int)PlayFabLeaderboard.LowestCup(); num--)
			{
				string valueKey = num.ToString();
				if (Singleton<GameConfigurationManager>.Instance.HasValue("cake_race_cup_requirements", valueKey) && level >= Singleton<GameConfigurationManager>.Instance.GetValue<int>("cake_race_cup_requirements", valueKey))
				{
					return num;
				}
			}
		}
		return (int)PlayFabLeaderboard.LowestCup();
	}

	private void RewardCupPlayer(PlayFabLeaderboard.Leaderboard currentLeaderboard)
	{
		rewardPendingCup = currentLeaderboard;
		isRewardingPlayer = true;
		Singleton<PlayFabManager>.Instance.Leaderboard.GetLeaderboardAroundPlayer(rewardPendingCup, OnRankFetched, OnRankError, previousSeason: true);
	}

	private void TryToShowCupEndAnimation(bool forceShow = false)
	{
		if (forceShow || GameProgress.GetBool("cake_race_show_cup_animation"))
		{
			GameProgress.SetBool("cake_race_show_cup_animation", value: false);
			OpenLeaderboardDialog();
			leaderboardDialog.ShowCupAnimation(GameProgress.GetInt("cake_race_current_cup", (int)PlayFabLeaderboard.LowestCup()));
		}
	}

	private void OnRankError(PlayFabError error)
	{
		if (IsCakeRaceMenuOpen)
		{
			rewardPendingCup = PlayFabLeaderboard.LowestCup();
			isRewardingPlayer = false;
			TryToUnlockCakeRaceLockScreen();
		}
	}

	private IEnumerator WaitPopUpAndTryRankReward(GetLeaderboardAroundPlayerResult result)
	{
		WaitForSeconds wfs = new WaitForSeconds(0.1f);
		while (lootCrateSlots.IsPopUpOpen())
		{
			yield return wfs;
		}
		OnRankFetched(result);
	}

	private void OnRankFetched(GetLeaderboardAroundPlayerResult result)
	{
		if (!IsCakeRaceMenuOpen)
		{
			return;
		}
		if (lootCrateSlots.IsPopUpOpen())
		{
			StartCoroutine(WaitPopUpAndTryRankReward(result));
			return;
		}
		if (result.Leaderboard == null || (result.Leaderboard.Count > 0 && (result.Leaderboard[0].StatValue <= 0 || result.Leaderboard[0].Position >= 500)))
		{
			TryToShowCupEndAnimation();
			TryToUnlockCakeRaceLockScreen();
			return;
		}
		GameObject go = UnityEngine.Object.Instantiate(seasonEndDialogPopup, Vector3.zero, Quaternion.identity);
		go.transform.position += Vector3.back * 20f;
		seasonEndDialog = go.GetComponent<LeaderboardSeasonEndDialog>();
		seasonEndDialog.SetLoading(loading: true);
		seasonEndDialog.onClose += delegate
		{
			UnityEngine.Object.Destroy(go);
		};
		int currentCupIndex = (int)rewardPendingCup;
		int num = result.Leaderboard[0].Position + 1;
		int @int = GameProgress.GetInt("cake_race_highest_rank");
		if (@int <= 0 || num < @int)
		{
			GameProgress.SetInt("cake_race_highest_rank", num);
		}
		string text = string.Empty;
		switch (num)
		{
		case 3:
			text = "cake_race_bronze_trophies_won";
			break;
		case 2:
			text = "cake_race_silver_trophies_won";
			break;
		case 1:
			text = "cake_race_gold_trophies_won";
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			int int2 = GameProgress.GetInt(text);
			GameProgress.SetInt(text, int2 + 1);
		}
		int seasonSnoutCoinReward = GetSeasonSnoutCoinReward(currentCupIndex, num);
		if (seasonSnoutCoinReward > 0)
		{
			GameProgress.AddSnoutCoins(seasonSnoutCoinReward);
			int int3 = GameProgress.GetInt("cake_race_coins_won");
			GameProgress.SetInt("cake_race_coins_won", int3 + seasonSnoutCoinReward);
		}
		LootCrateType crateType = GetSeasonCrateReward(currentCupIndex, num);
		if (crateType != LootCrateType.None)
		{
			seasonEndDialog.onClose += delegate
			{
				LootCrate.SpawnLootCrateOpeningDialog(crateType, 1, WPFMonoBehaviour.s_hudCamera, OnDialogClosed, new LootCrate.AnalyticData($"{(PlayFabLeaderboard.Leaderboard)currentCupIndex}_reward", "0", LootCrate.AdWatched.NotApplicaple));
			};
		}
		else
		{
			seasonEndDialog.onClose += OnDialogClosed;
		}
		seasonEndDialog.SetCrateRankAndReward(crateType, num, seasonSnoutCoinReward);
		isRewardingPlayer = false;
	}

	private void OnDialogClosed()
	{
		TryToShowCupEndAnimation();
		TryToUnlockCakeRaceLockScreen();
	}

	public static int GetSeasonSnoutCoinReward(int cupIndex, int rank)
	{
		if (rank <= 0)
		{
			return 0;
		}
		int num = 0;
		if (Singleton<GameConfigurationManager>.Instance.HasValue("cake_race_cup_reward_multipliers", cupIndex.ToString()))
		{
			num = Singleton<GameConfigurationManager>.Instance.GetValue<int>("cake_race_cup_reward_multipliers", cupIndex.ToString());
		}
		ConfigData config = Singleton<GameConfigurationManager>.Instance.GetConfig("cake_race_cup_snout_rewards");
		int result = 0;
		if (config != null && config.Keys != null && config.Keys.Length != 0)
		{
			int num2 = -1;
			int num3 = int.MaxValue;
			for (int i = 0; i < config.Keys.Length; i++)
			{
				if (int.TryParse(config.Keys[i], out var result2) && result2 > 0 && result2 < num3 && rank <= result2)
				{
					num3 = result2;
					num2 = i;
				}
			}
			result = ((num2 >= 0 && num2 < config.Values.Length && int.TryParse(config.Values[num2], out result)) ? (result + Mathf.RoundToInt((float)result * (0.01f * (float)num))) : 0);
		}
		return result;
	}

	public static LootCrateType GetSeasonCrateReward(int cupIndex, int rank)
	{
		LootCrateType result = LootCrateType.None;
		if (rank > 0 && rank <= 5 && Singleton<GameConfigurationManager>.Instance.HasValue("cake_race_cup_crate_rewards", cupIndex.ToString()))
		{
			result = (LootCrateType)Singleton<GameConfigurationManager>.Instance.GetValue<int>("cake_race_cup_crate_rewards", cupIndex.ToString());
		}
		return result;
	}

	private void OnDisable()
	{
		KeyListener.keyReleased -= HandleKeyListenerkeyReleased;
	}

	private void HandleKeyListenerkeyReleased(KeyCode obj)
	{
		if (Singleton<GuiManager>.Instance.enabled && obj == KeyCode.Escape)
		{
			GoToMainMenu();
		}
	}

	private void Update()
	{
		if (cloudRenderer != null)
		{
			currentOffset += GameTime.RealTimeDelta * cloudSpeed;
			if (currentOffset > 1f)
			{
				currentOffset -= 1f;
			}
			else if (currentOffset < -1f)
			{
				currentOffset += 1f;
			}
			cloudRenderer.material.SetTextureOffset("_MainTex", Vector2.right * currentOffset);
		}
	}

	private bool HasWeeklyData()
	{
		if (WeeklyTrackIdentifiers != null)
		{
			return WeeklyTrackIdentifiers.Length >= 7;
		}
		return false;
	}

	public static bool IsWeeklyTrack(int index, string uniqueIdentifier, bool ignoreTutorial = false)
	{
		return GetTrackIndex(uniqueIdentifier, ignoreTutorial) == index;
	}

	private int GetWeeklyTrackIndex()
	{
		return currentWeeklyTrackIndex;
	}

	private int GenerateNewWeeklyTrackIndex()
	{
		if (WinCount >= 14)
		{
			currentWeeklyTrackIndex = UnityEngine.Random.Range(0, 7);
		}
		else
		{
			currentWeeklyTrackIndex = WinCount % 7;
		}
		return currentWeeklyTrackIndex;
	}

	public void StartRace()
	{
		if (WinCount < 0 || !HasWeeklyData())
		{
			SetInfoLabel("Error getting cake race data");
			UpdateWeeklyTracks();
			return;
		}
		FindNewPlayer = false;
		UseDefaultReplay = false;
		DateTime serverTime = Singleton<TimeManager>.Instance.ServerTime;
		DateTime value = serverTime;
		if (!GameProgress.HasKey("cake_race_first_day"))
		{
			GameProgress.SetString("cake_race_first_day", value.ToShortDateString());
		}
		else
		{
			value = DateTime.Parse(GameProgress.GetString("cake_race_first_day", string.Empty));
		}
		serverTime.Subtract(value);
		DateTime value2 = serverTime;
		if (GameProgress.HasKey("cake_race_last_played"))
		{
			value2 = DateTime.Parse(GameProgress.GetString("cake_race_last_played", DateTime.MinValue.ToShortDateString()));
		}
		GameProgress.SetString("cake_race_last_played", serverTime.ToShortDateString());
		int @int = GameProgress.GetInt("cake_race_days_played", 1);
		if (serverTime.Subtract(value2).TotalHours > 12.0)
		{
			GameProgress.SetInt("cake_race_days_played", ++@int);
		}
		GenerateNewWeeklyTrackIndex();
		if (IsTutorial)
		{
			UseDefaultReplay = true;
			OnReplayFetched(null);
		}
		else
		{
			FindOpponent();
		}
	}

	private void FindOpponent()
	{
		if (HasWeeklyData())
		{
			Singleton<GuiManager>.Instance.enabled = false;
			SetInfoLabel("Searching for opponent");
			string currentSeasonID = CakeRaceMode.GetCurrentSeasonID();
			string key = $"Season_{currentSeasonID}_wins";
			string key2 = $"Season_{currentSeasonID}_losses";
			int @int = GameProgress.GetInt(key);
			int int2 = GameProgress.GetInt(key2);
			Singleton<PlayFabManager>.Instance.MatchMaking.FindOpponentReplay(GetWeeklyTrackIndex(), Singleton<PlayerProgress>.Instance.Level, @int - int2, OnReplayFetched);
		}
		else
		{
			SetInfoLabel("Error fetching tracks");
			UpdateWeeklyTracks();
		}
	}

	private void OnReplayFetched(string replayJson)
	{
		if (!IsCakeRaceMenuOpen)
		{
			return;
		}
		if (UseDefaultReplay || string.IsNullOrEmpty(replayJson))
		{
			LoadDefaultReplay();
			return;
		}
		Debug.LogWarning("[OnReplayFetched]\n" + replayJson);
		Singleton<GuiManager>.Instance.enabled = true;
		CakeRaceReplay cakeRaceReplay = new CakeRaceReplay(replayJson);
		if (cakeRaceReplay.IsValid)
		{
			if (IsWeeklyTrack(currentWeeklyTrackIndex, cakeRaceReplay.UniqueIdentifier))
			{
				if (WPFMonoBehaviour.gameData.m_cakeRaceData.GetInfo(cakeRaceReplay.UniqueIdentifier, out var info))
				{
					CakeRaceMode.OpponentReplay = cakeRaceReplay;
					SetInfoLabel("Opponent found");
					LoadCakeRaceLevel(info.Value);
				}
				else
				{
					SetInfoLabel("Opponent replay has unknown level");
					LoadDefaultReplay();
				}
			}
			else
			{
				SetInfoLabel("Opponent replay not from this week");
				LoadDefaultReplay();
			}
		}
		else
		{
			SetInfoLabel("Opponent replay is invalid");
			LoadDefaultReplay();
		}
	}

	private void LoadDefaultReplay()
	{
		UseDefaultReplay = false;
		if (WPFMonoBehaviour.gameData.m_cakeRaceData.GetInfo(WeeklyTrackIdentifiers[GetWeeklyTrackIndex()], out var info))
		{
			CakeRaceMode.OpponentReplay = new CakeRaceReplay(info.Value.Replay);
			CakeRaceMode.OpponentReplay.SetPlayerLevel(Singleton<PlayerProgress>.Instance.Level);
			CakeRaceMode.OpponentReplay.SetPlayerName("Hogster");
			Debug.LogWarning("[OnReplayFetched] DefaultOpponent\n" + info.Value.Replay);
			SetInfoLabel("Default Opponent found");
			LoadCakeRaceLevel(info.Value);
		}
		else
		{
			SetInfoLabel("Error 404 for track " + WeeklyTrackIdentifiers[GetWeeklyTrackIndex()]);
		}
	}

	private void LoadCakeRaceLevel(CakeRaceInfo info)
	{
		CakeRaceMode.CurrentCakeRaceInfo = info;
		if (!string.IsNullOrEmpty(info.Identifier))
		{
			if (info.Identifier.StartsWith("S"))
			{
				LevelManager.GameModeIndex = 1;
				Singleton<GameManager>.Instance.LoadSandboxLevel(info.Identifier);
				Singleton<GuiManager>.Instance.enabled = false;
			}
			else if (info.Identifier.StartsWith("R"))
			{
				LevelManager.GameModeIndex = 1;
				Singleton<GameManager>.Instance.LoadRaceLevel(info.Identifier);
				Singleton<GuiManager>.Instance.enabled = false;
			}
			else
			{
				SetInfoLabel("Replay not supported");
			}
		}
		else
		{
			SetInfoLabel("Level type not supported");
		}
	}

	private void UpdateWinCount()
	{
		if (WinCount >= 0)
		{
			SetWinCount(WinCount);
			return;
		}
		string key = "Statistics_" + PlayFabLeaderboard.Leaderboard.CakeRaceWins;
		if (GameProgress.HasKey(key))
		{
			SetWinCount(GameProgress.GetInt(key));
		}
		else if (Singleton<PlayFabManager>.Instance.Initialized)
		{
			Singleton<PlayFabManager>.Instance.Leaderboard.GetScore(PlayFabLeaderboard.Leaderboard.CakeRaceWins, OnWinCountSuccess, OnWinCountError);
		}
		else
		{
			SetWinCount(0);
		}
	}

	private void OnWinCountSuccess(GetPlayerStatisticsResult result)
	{
		if (!IsCakeRaceMenuOpen)
		{
			return;
		}
		string value = PlayFabLeaderboard.Leaderboard.CakeRaceWins.ToString();
		foreach (StatisticValue statistic in result.Statistics)
		{
			if (statistic.StatisticName.Equals(value))
			{
				SetWinCount(statistic.Value);
				return;
			}
		}
		SetWinCount(0);
	}

	private void OnWinCountError(PlayFabError error)
	{
		if (IsCakeRaceMenuOpen)
		{
			SetWinCount(0);
		}
	}

	private void SetWinCount(int wins)
	{
		WinCount = wins;
		if (IsTutorial)
		{
			Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve(winsLabelKey);
			TextMeshHelper.UpdateTextMeshes(winsLabel, string.Format(localeParameters.translation, WinCount));
			TextMeshHelper.Wrap(winsLabel, (!TextMeshHelper.UsesKanjiCharacters()) ? 1 : 6);
		}
		else
		{
			TextMeshHelper.UpdateTextMeshes(winsLabel, string.Empty);
		}
	}

	private void UpdateDaysLeft()
	{
		string arg = $"{SeasonDaysLeft()}";
		Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve(daysLeftLabelKey);
		TextMeshHelper.UpdateTextMeshes(daysLeftLabel, string.Format(localeParameters.translation, arg));
		TextMeshHelper.Wrap(daysLeftLabel, 5);
	}

	private int SeasonDaysLeft()
	{
		return (0 - Singleton<TimeManager>.Instance.ServerTime.DayOfWeek + 7) % 7;
	}

	public void GoToMainMenu()
	{
		LevelManager.GameModeIndex = 0;
		Singleton<GameManager>.Instance.LoadMainMenu(showLoadingScreen: false);
	}

	private void SetInfoLabel(string text = "")
	{
	}

	public void OpenLeaderboardDialog()
	{
		if (!(leaderboardDialogPrefab == null))
		{
			if (leaderboardDialog == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(leaderboardDialogPrefab, 15f * Vector3.back + Vector3.down, Quaternion.identity);
				leaderboardDialog = gameObject.GetComponent<LeaderboardDialog>();
			}
			leaderboardDialog.Open();
			if (!GameProgress.GetBool("leaderboard_opened"))
			{
				GameProgress.SetBool("leaderboard_opened", value: true);
				leaderboardButton.transform.Find("NewContentTag").gameObject.SetActive(value: false);
			}
		}
	}

	public static int GetTrackIndex(string uniqueIdentifier, bool ignoreTutorial = false)
	{
		string[] array = WeeklyTrackIdentifiers;
		if (ignoreTutorial)
		{
			if (AllSeasonTracks == null || !AllSeasonTracks.ContainsKey(CurrentSeasonIndex))
			{
				return -1;
			}
			array = AllSeasonTracks[CurrentSeasonIndex].ToArray();
		}
		if (array != null && array.Length != 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].Equals(uniqueIdentifier))
				{
					return i;
				}
			}
		}
		return -1;
	}

	public static PlayFabLeaderboard.Leaderboard GetCurrentLeaderboardCup()
	{
		return (PlayFabLeaderboard.Leaderboard)GameProgress.GetInt("cake_race_current_cup", (int)PlayFabLeaderboard.LowestCup());
	}

	public static int CurrentCakeRaceWeek()
	{
		return GameProgress.GetInt("cake_race_current_week");
	}

	public static void ClearCloudTrackData()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < 7; i++)
		{
			dictionary.Add($"replay_track_{i}", string.Empty);
		}
		Singleton<PlayFabManager>.Instance.UpdateUserData(dictionary, UserDataPermission.Public);
	}
}
