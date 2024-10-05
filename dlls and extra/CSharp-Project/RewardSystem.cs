using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem : Singleton<RewardSystem>
{
	[SerializeField]
	private List<DailyRewardBundle> rewards;

	public RewardDialog rewardDialog;

	private static bool mFreezeResetTime;

	private const string RANDOM_REWARD_SEED_KEY = "AmazonRandomRewardSeed";

	private const string PENDING_REWARD_LEVEL_KEY = "AmazonPendingRewardLevel";

	private const string LEVEL_KEY = "AmazonRewardLevel";

	private const string REWARD_KEY = "AmazonRewardTime";

	private const string RESET_KEY = "AmazonResetTime";

	private const string TIMER_KEY = "AmazonTimerMode";

	private const string TAG = "LOG - RewardSystem, ";

	private bool mLoginListener;

	private ServerTime mServerTime;

	private bool mNeedReset;

	private static RewardDataHolder CurrentRewardStatus;

	public List<DailyRewardBundle> Rewards => rewards;

	public bool HasTime
	{
		get
		{
			if (mServerTime != null)
			{
				return mServerTime.GetStatus() == ServerTime.Status.STATUS_OK;
			}
			return false;
		}
	}

	public static int AmountOfDays => 30;

	public static int CurrentLevel => CurrentRewardStatus.Level;

	public static int PendingRewardLevel => CurrentRewardStatus.PendingRewardLevel;

	public static bool FreezeResetTime
	{
		get
		{
			return mFreezeResetTime;
		}
		set
		{
			mFreezeResetTime = value;
		}
	}

	private void Awake()
	{
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		SetAsPersistant();
		mServerTime = new ServerTime();
		mServerTime.StatusChanged += HandleRewardLogic;
		CurrentRewardStatus = LoadSavedData();
		HandleServerTime();
	}

	private void OnDestroy()
	{
		if (mLoginListener)
		{
			HatchManager.onLoginSuccess = (Action)Delegate.Remove(HatchManager.onLoginSuccess, new Action(HandleServerTime));
		}
		if (mServerTime != null)
		{
			mServerTime.Destroy();
		}
	}

	public void HandleServerTime()
	{
		if (HatchManager.IsLoggedIn)
		{
			CurrentRewardStatus = LoadSavedData();
			mServerTime.RefreshServerTime();
		}
		else if (!mLoginListener)
		{
			mLoginListener = true;
			HatchManager.onLoginSuccess = (Action)Delegate.Combine(HatchManager.onLoginSuccess, new Action(HandleServerTime));
		}
	}

	public static int RandomSeed(int level)
	{
		return CurrentRewardStatus.RandomSeed + level;
	}

	public static RewardIcon.State GetRewardStateForDay(int dayIndex)
	{
		int level = CurrentRewardStatus.Level;
		if (dayIndex == level && Singleton<RewardSystem>.Instance.SecondsToNextReward() <= 0)
		{
			return RewardIcon.State.ClaimNow;
		}
		if (dayIndex < level)
		{
			return RewardIcon.State.Claimed;
		}
		return RewardIcon.State.NotAvailable;
	}

	public void ResetData()
	{
		SaveData(new RewardDataHolder
		{
			RewardTime = -1,
			Level = 0,
			TimerMode = (Singleton<BuildCustomizationLoader>.Instance.CheatsEnabled ? CurrentRewardStatus.TimerMode : 0),
			RandomSeed = UnityEngine.Random.Range(0, int.MaxValue),
			ServerTime = -1,
			ResetTime = -1,
			ServerTimeUpdated = -1,
			PendingRewardLevel = -1
		});
	}

	public int GetTimerMode()
	{
		return CurrentRewardStatus.TimerMode;
	}

	public void ChangeTimerMode()
	{
		if (Singleton<BuildCustomizationLoader>.Instance.CheatsEnabled)
		{
			if (CurrentRewardStatus.TimerMode < 4)
			{
				CurrentRewardStatus.TimerMode++;
			}
			else
			{
				CurrentRewardStatus.TimerMode = 0;
			}
		}
		else
		{
			CurrentRewardStatus.TimerMode = 0;
		}
		GameProgress.SetInt("AmazonTimerMode", CurrentRewardStatus.TimerMode);
		CurrentRewardStatus = LoadSavedData();
		mNeedReset = true;
	}

	public void OpenDialog()
	{
		if (rewardDialog != null)
		{
			rewardDialog.Open();
		}
	}

	public void SetDialogView(RewardDialog dialog)
	{
		rewardDialog = dialog;
		HandleServerTime();
	}

	private RewardDataHolder LoadSavedData()
	{
		RewardDataHolder result = default(RewardDataHolder);
		result.Level = GetSavedRewardLevel();
		result.RewardTime = GetSavedRewardTime();
		result.ResetTime = GetSavedResetTime();
		result.TimerMode = GetSavedTimerMode();
		result.RandomSeed = GetSavedRandomSeed();
		result.PendingRewardLevel = GetSavedPendingRewardLevel();
		return result;
	}

	private int GetSavedTimerMode()
	{
		return GameProgress.GetInt("AmazonTimerMode");
	}

	private int GetSavedRewardLevel()
	{
		return GameProgress.GetInt("AmazonRewardLevel");
	}

	private int GetSavedRewardTime()
	{
		return GameProgress.GetInt("AmazonRewardTime");
	}

	private int GetSavedResetTime()
	{
		return GameProgress.GetInt("AmazonResetTime");
	}

	private int GetSavedRandomSeed()
	{
		return GameProgress.GetInt("AmazonRandomRewardSeed", UnityEngine.Random.Range(0, int.MaxValue));
	}

	private int GetSavedPendingRewardLevel()
	{
		return GameProgress.GetInt("AmazonPendingRewardLevel");
	}

	private RewardDataHolder CalculateNewTimers(int time)
	{
		RewardDataHolder result = default(RewardDataHolder);
		result.ServerTime = time;
		result.ServerTimeUpdated = Mathf.RoundToInt(Time.realtimeSinceStartup);
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(time);
		DateTime dateTime2;
		DateTime dateTime3;
		if (Singleton<BuildCustomizationLoader>.Instance.CheatsEnabled)
		{
			dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Utc);
			dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Utc);
			switch (CurrentRewardStatus.TimerMode)
			{
			case 0:
				dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1.0);
				dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(2.0);
				break;
			case 1:
				dateTime2 = dateTime2.AddMinutes(15.0);
				dateTime3 = dateTime3.AddMinutes(30.0);
				break;
			case 2:
				dateTime2 = dateTime2.AddMinutes(5.0);
				dateTime3 = dateTime3.AddMinutes(15.0);
				break;
			case 3:
				dateTime2 = dateTime2.AddMinutes(1.0);
				dateTime3 = dateTime3.AddMinutes(1.0).AddSeconds(15.0);
				break;
			case 4:
				dateTime2 = dateTime2.AddSeconds(5.0);
				dateTime3 = dateTime3.AddSeconds(10.0);
				break;
			}
		}
		else
		{
			dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1.0);
			dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(2.0);
		}
		result.RewardTime = Convert.ToInt32(dateTime2.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
		result.ResetTime = Convert.ToInt32(dateTime3.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
		return result;
	}

	public bool IsRewardReady()
	{
		if (SecondsToNextReward() > 0)
		{
			return PendingRewardLevel >= 0;
		}
		return true;
	}

	public int SecondsToNextReward()
	{
		return SecondsToTriggerTime(CurrentRewardStatus.ServerTime, CurrentRewardStatus.RewardTime) - (Mathf.RoundToInt(Time.realtimeSinceStartup) - CurrentRewardStatus.ServerTimeUpdated);
	}

	public int SecondsToNextReset()
	{
		return SecondsToTriggerTime(CurrentRewardStatus.ServerTime, CurrentRewardStatus.ResetTime) - (Mathf.RoundToInt(Time.realtimeSinceStartup) - CurrentRewardStatus.ServerTimeUpdated);
	}

	private int CurrentTime()
	{
		return CurrentRewardStatus.ServerTime + (Mathf.RoundToInt(Time.realtimeSinceStartup) - CurrentRewardStatus.ServerTimeUpdated);
	}

	private int SecondsToTriggerTime(int currentTime, int triggerTime)
	{
		return triggerTime - currentTime;
	}

	private string TimeToNextTriggerText(int currentTime, int triggerTime)
	{
		int num = triggerTime - currentTime;
		if (num <= 60)
		{
			return triggerTime - currentTime + " seconds";
		}
		if (num <= 3600)
		{
			return (int)Math.Round((double)(triggerTime - currentTime) / 60.0) + " minutes";
		}
		return (int)Math.Round((double)(triggerTime - currentTime) / 3600.0) + " hours";
	}

	private void SaveData(RewardDataHolder data)
	{
		GameProgress.SetInt("AmazonPendingRewardLevel", data.PendingRewardLevel);
		GameProgress.SetInt("AmazonRandomRewardSeed", data.RandomSeed);
		GameProgress.SetInt("AmazonRewardTime", data.RewardTime);
		GameProgress.SetInt("AmazonResetTime", data.ResetTime);
		GameProgress.SetInt("AmazonRewardLevel", data.Level);
		GameProgress.SetInt("AmazonTimerMode", data.TimerMode);
	}

	private PrizeType GetRewardObjectType(int level)
	{
		return (level % 4) switch
		{
			1 => PrizeType.SuperMagnet, 
			2 => PrizeType.SuperGlue, 
			3 => PrizeType.SuperMechanic, 
			_ => PrizeType.TurboCharge, 
		};
	}

	private void GiveReward(DailyRewardBundle rewardBundle)
	{
		if (rewardBundle == null)
		{
			return;
		}
		List<DailyReward> list = rewardBundle.GetRewards(CurrentRewardStatus.PendingRewardLevel);
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			DailyReward dailyReward = list[i];
			string customTypeOfGain = "Odyssey daily reward";
			switch (dailyReward.prize)
			{
			case PrizeType.SuperGlue:
				GameProgress.AddSuperGlue(dailyReward.prizeCount);
				if (Singleton<IapManager>.Instance != null)
				{
					Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.SuperGlueSingle, dailyReward.prizeCount, customTypeOfGain);
				}
				break;
			case PrizeType.SuperMagnet:
				GameProgress.AddSuperMagnet(dailyReward.prizeCount);
				if (Singleton<IapManager>.Instance != null)
				{
					Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.SuperMagnetSingle, dailyReward.prizeCount, customTypeOfGain);
				}
				break;
			case PrizeType.TurboCharge:
				GameProgress.AddTurboCharge(dailyReward.prizeCount);
				if (Singleton<IapManager>.Instance != null)
				{
					Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.TurboChargeSingle, dailyReward.prizeCount, customTypeOfGain);
				}
				break;
			case PrizeType.SuperMechanic:
				GameProgress.AddBluePrints(dailyReward.prizeCount);
				if (Singleton<IapManager>.Instance != null)
				{
					Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.BlueprintSingle, dailyReward.prizeCount, customTypeOfGain);
				}
				break;
			case PrizeType.NightVision:
				GameProgress.AddNightVision(dailyReward.prizeCount);
				if (Singleton<IapManager>.Instance != null)
				{
					Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.NightVisionSingle, dailyReward.prizeCount, customTypeOfGain);
				}
				break;
			}
		}
	}

	private void HandleRewardLogic(int time)
	{
		if (mNeedReset)
		{
			mNeedReset = false;
			RewardDataHolder rewardDataHolder = CalculateNewTimers(time);
			CurrentRewardStatus.ServerTime = time;
			CurrentRewardStatus.RewardTime = rewardDataHolder.RewardTime;
			CurrentRewardStatus.ResetTime = rewardDataHolder.ResetTime;
			CurrentRewardStatus.ServerTimeUpdated = rewardDataHolder.ServerTimeUpdated;
			SaveData(CurrentRewardStatus);
		}
		if (ResetTimePassed(time, CurrentRewardStatus.ResetTime) || CurrentRewardStatus.Level >= AmountOfDays)
		{
			if (CurrentRewardStatus.Level >= AmountOfDays)
			{
				mNeedReset = true;
			}
			ResetData();
			CurrentRewardStatus = LoadSavedData();
			HandleRewardLogic(time);
		}
		else if (EligibleForReward(time, CurrentRewardStatus.RewardTime))
		{
			int level = CurrentRewardStatus.Level;
			RewardDataHolder data = CalculateNewTimers(time);
			data.Level = CurrentRewardStatus.Level;
			data.TimerMode = CurrentRewardStatus.TimerMode;
			CurrentRewardStatus.ServerTime = time;
			SaveData(data);
			if (rewardDialog != null)
			{
				if (!rewardDialog.gameObject.activeInHierarchy)
				{
					OpenDialog();
				}
				CurrentRewardStatus.PendingRewardLevel = level;
			}
		}
		else
		{
			CurrentRewardStatus.ServerTime = time;
			CurrentRewardStatus.ServerTimeUpdated = Mathf.RoundToInt(Time.realtimeSinceStartup);
		}
	}

	public void RefreshData()
	{
		mServerTime.RefreshServerTime();
	}

	public void ClaimReward()
	{
		int currentTime = CurrentTime();
		if (!mFreezeResetTime && ResetTimePassed(currentTime, CurrentRewardStatus.ResetTime))
		{
			mServerTime.RefreshServerTime();
			return;
		}
		if (EligibleForReward(currentTime, CurrentRewardStatus.RewardTime))
		{
			CurrentRewardStatus.PendingRewardLevel = CurrentRewardStatus.Level;
		}
		else if (CurrentRewardStatus.PendingRewardLevel < 0)
		{
			return;
		}
		DailyRewardBundle rewardBundle = rewards[CurrentRewardStatus.PendingRewardLevel];
		GiveReward(rewardBundle);
		CurrentRewardStatus.PendingRewardLevel = -1;
		CurrentRewardStatus.Level++;
		mNeedReset = true;
		mServerTime.RefreshServerTime();
	}

	private bool ResetTimePassed(int currentTime, int resetTime)
	{
		if (!mFreezeResetTime && resetTime > 0)
		{
			return currentTime > resetTime;
		}
		return false;
	}

	private bool EligibleForReward(int currentTime, int rewardTime)
	{
		if (mServerTime.GetStatus() == ServerTime.Status.STATUS_OK)
		{
			if (currentTime < rewardTime)
			{
				return rewardTime == -1;
			}
			return true;
		}
		return false;
	}
}
