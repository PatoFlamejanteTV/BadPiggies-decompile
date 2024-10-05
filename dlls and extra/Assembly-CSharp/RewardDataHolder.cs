internal struct RewardDataHolder
{
	private int mTimerMode;

	private int mLevel;

	private int mPendingRewardLevel;

	private int mRewardTime;

	private int mResetTime;

	private int mServerTime;

	private int mServerTimeUpdated;

	private int mRandomSeed;

	public int TimerMode
	{
		get
		{
			return mTimerMode;
		}
		set
		{
			mTimerMode = value;
		}
	}

	public int Level
	{
		get
		{
			return mLevel;
		}
		set
		{
			mLevel = value;
		}
	}

	public int PendingRewardLevel
	{
		get
		{
			return mPendingRewardLevel;
		}
		set
		{
			mPendingRewardLevel = value;
		}
	}

	public int RewardTime
	{
		get
		{
			return mRewardTime;
		}
		set
		{
			mRewardTime = value;
		}
	}

	public int ResetTime
	{
		get
		{
			return mResetTime;
		}
		set
		{
			mResetTime = value;
		}
	}

	public int ServerTime
	{
		get
		{
			return mServerTime;
		}
		set
		{
			mServerTime = value;
		}
	}

	public int ServerTimeUpdated
	{
		get
		{
			return mServerTimeUpdated;
		}
		set
		{
			mServerTimeUpdated = value;
		}
	}

	public int RandomSeed
	{
		get
		{
			return mRandomSeed;
		}
		set
		{
			mRandomSeed = value;
		}
	}
}
