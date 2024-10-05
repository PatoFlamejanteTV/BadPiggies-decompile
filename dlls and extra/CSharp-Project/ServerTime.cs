using System;
using UnityEngine;

public class ServerTime
{
	public enum Status
	{
		STATUS_OK,
		STATUS_NOK
	}

	private const string TAG = "LOG - ServerTime, ";

	private bool mPendingRequest;

	private Status mStatus;

	public event Action<int> StatusChanged;

	public ServerTime()
	{
		mStatus = Status.STATUS_NOK;
	}

	public void Destroy()
	{
		this.StatusChanged = null;
	}

	public Status GetStatus()
	{
		return mStatus;
	}

	public void RefreshServerTime()
	{
		if (!mPendingRequest)
		{
			mPendingRequest = true;
			mStatus = Status.STATUS_NOK;
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				ServerTimeErrorCallback(0, "No internet connection");
			}
		}
	}

	private void ServerTimeSuccessCallback(ulong currentTime)
	{
		int obj = (int)currentTime;
		mPendingRequest = false;
		mStatus = Status.STATUS_OK;
		if (this.StatusChanged != null)
		{
			this.StatusChanged(obj);
		}
	}

	private void ServerTimeErrorCallback(int errorCode, string message)
	{
		mPendingRequest = false;
		mStatus = Status.STATUS_NOK;
	}
}
