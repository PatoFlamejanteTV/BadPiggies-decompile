using System;
using UnityEngine;

public class DoubleRewardManager : Singleton<DoubleRewardManager>
{
	public enum Status
	{
		Uninitialized,
		Initialized,
		Error
	}

	public Action OnInitialize;

	public Action OnAdWatched;

	public Action OnAdFailed;

	public Action<bool> OnAdLoaded;

	public Action OnConfirmationFailed;

	private AdReward adReward;

	private ServerTime serverTime;

	private int rewardTime;

	private int rewardCoins;

	private float lastCheckTime;

	private float doubleRewardEndTime;

	private bool hasAd;

	private bool loadingAd;

	private Status status;

	public bool HasAd => hasAd;

	public bool LoadingAd => loadingAd;

	public Status CurrentStatus => status;

	public float DoubleRewardTimeRemaining => doubleRewardEndTime - Time.realtimeSinceStartup;

	public bool HasDoubleReward => DoubleRewardTimeRemaining > 0f;

	public int RewardCoins => rewardCoins;

	public string FormattedRewardTime
	{
		get
		{
			TimeSpan time = TimeSpan.FromSeconds(Mathf.Clamp(rewardTime, 0f, float.MaxValue));
			if (time.TotalMinutes > 0.0)
			{
				return TimeFormatter.Format2Minutes(time);
			}
			return TimeFormatter.Format2Seconds(time);
		}
	}

	public string FormattedDoubleRewardTimeRemaining
	{
		get
		{
			TimeSpan time = TimeSpan.FromSeconds(Mathf.Clamp(DoubleRewardTimeRemaining, 0f, float.MaxValue));
			if (time.TotalMinutes > 0.0)
			{
				return TimeFormatter.Format2Minutes(time);
			}
			return TimeFormatter.Format2Seconds(time);
		}
	}

	private float TimeSinceLastCheck
	{
		get
		{
			if (lastCheckTime < 0f)
			{
				return float.MaxValue;
			}
			return Time.realtimeSinceStartup - lastCheckTime;
		}
	}

	private void Awake()
	{
		SetAsPersistant();
		status = Status.Uninitialized;
		lastCheckTime = -1f;
		if (Singleton<GameConfigurationManager>.Instance.HasData)
		{
			rewardTime = Singleton<GameConfigurationManager>.Instance.GetValue<int>("double_reward_duration", "seconds");
			rewardCoins = Singleton<GameConfigurationManager>.Instance.GetValue<int>("double_reward_coin_reward", "coin_reward");
			Initialize();
			return;
		}
		GameConfigurationManager gameConfigurationManager = Singleton<GameConfigurationManager>.Instance;
		gameConfigurationManager.OnHasData = (Action)Delegate.Combine(gameConfigurationManager.OnHasData, (Action)delegate
		{
			rewardTime = Singleton<GameConfigurationManager>.Instance.GetValue<int>("double_reward_duration", "seconds");
			rewardCoins = Singleton<GameConfigurationManager>.Instance.GetValue<int>("double_reward_coin_reward", "coin_reward");
			Initialize();
		});
	}

	private void Initialize()
	{
		adReward = new AdReward(AdvertisementHandler.DoubleRewardPlacement);
		adReward.OnReady = (Action)Delegate.Combine(adReward.OnReady, new Action(OnAdReady));
		adReward.OnFailed = (Action)Delegate.Combine(adReward.OnFailed, new Action(OnAdFailure));
		adReward.OnAdFinished = (Action)Delegate.Combine(adReward.OnAdFinished, new Action(OnAdFinished));
		adReward.OnLoading = (Action)Delegate.Combine(adReward.OnLoading, new Action(OnAdLoading));
		adReward.OnCancel = (Action)Delegate.Combine(adReward.OnCancel, new Action(OnAdCancel));
		adReward.OnConfirmationFailed = (Action)Delegate.Combine(adReward.OnConfirmationFailed, new Action(OnAdConfirmationFailed));
		adReward.Load();
		serverTime = new ServerTime();
		serverTime.StatusChanged += OnServerTimeStatusChanged;
		serverTime.RefreshServerTime();
	}

	private void OnDestroy()
	{
		if (adReward != null)
		{
			adReward.Dispose();
		}
		if (serverTime != null)
		{
			serverTime.Destroy();
		}
	}

	private void OnAdReady()
	{
		hasAd = true;
		loadingAd = false;
		if (OnAdLoaded != null)
		{
			OnAdLoaded(obj: true);
			OnAdLoaded = null;
		}
	}

	private void OnAdFailure()
	{
		hasAd = false;
		loadingAd = false;
		if (OnAdFailed != null)
		{
			OnAdFailed();
			OnAdLoaded = null;
		}
	}

	private void OnAdFinished()
	{
		hasAd = false;
		if (OnAdWatched != null)
		{
			OnAdWatched();
			OnAdWatched = null;
		}
	}

	private void OnAdLoading()
	{
		hasAd = false;
		loadingAd = true;
	}

	private void OnAdCancel()
	{
	}

	private void OnAdConfirmationFailed()
	{
		adReward.Load();
		if (OnConfirmationFailed != null)
		{
			OnConfirmationFailed();
			OnConfirmationFailed = null;
		}
	}

	private void OnServerTimeStatusChanged(int serverTime)
	{
		lastCheckTime = Time.realtimeSinceStartup;
		int doubleRewardStartTime = GameProgress.GetDoubleRewardStartTime();
		if (doubleRewardStartTime > 0)
		{
			int num = doubleRewardStartTime + rewardTime - serverTime;
			if (num > 0)
			{
				doubleRewardEndTime = Time.realtimeSinceStartup + (float)num;
			}
			else
			{
				doubleRewardEndTime = -1f;
			}
		}
		else
		{
			doubleRewardEndTime = -1f;
		}
		if (status == Status.Uninitialized)
		{
			status = Status.Initialized;
			if (OnInitialize != null)
			{
				OnInitialize();
			}
		}
	}

	private void SetDoubleRewardStartTime(bool success, int time)
	{
		if (success)
		{
			GameProgress.SetDoubleRewardStartTime(time);
			doubleRewardEndTime = Time.realtimeSinceStartup + (float)rewardTime;
			if (DoubleRewardIcon.Instance != null)
			{
				DoubleRewardIcon.Instance.gameObject.SetActive(value: true);
			}
		}
	}

	public void RefreshServerTime()
	{
		serverTime.RefreshServerTime();
	}

	public void RefreshAd()
	{
		if (!loadingAd)
		{
			adReward.Load();
		}
	}

	public void PlayAd()
	{
		if (hasAd)
		{
			adReward.Play();
		}
	}
}
