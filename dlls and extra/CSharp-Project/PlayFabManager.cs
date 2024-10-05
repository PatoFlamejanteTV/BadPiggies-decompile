using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab.SharedModels;
using UnityEngine;

public class PlayFabManager : Singleton<PlayFabManager>
{
	public enum SyncType
	{
		None,
		IntArray,
		Base64String,
		Base64StringArray
	}

	public class DeltaData
	{
		public string Data { get; private set; }

		public SyncType DataSyncType { get; private set; }

		public DeltaData(string data, SyncType syncType)
		{
			Data = data;
			DataSyncType = syncType;
		}
	}

	private class PendingData
	{
		public string Data { get; private set; }

		public UserDataPermission Permission { get; private set; }

		public PendingData(string data, UserDataPermission permission)
		{
			SetData(data, permission);
		}

		public void SetData(string data, UserDataPermission permission)
		{
			Data = data;
			Permission = permission;
		}
	}

	[SerializeField]
	private string productionTitleID = "27DC";

	[SerializeField]
	private string devTitleID = "7988";

	private bool initialized;

	private UserDataPermission sendCachePermission;

	private Dictionary<string, string> sendCache;

	private Dictionary<string, PendingData> pendingCloudData;

	public Action<string, string> OnLogin;

	private bool waitingForMoreData;

	private float waitForSeconds = 2f;

	public PlayFabLeaderboard Leaderboard { get; private set; }

	public PlayFabMatchMaking MatchMaking { get; private set; }

	public PlayFabUserHandling Users { get; private set; }

	public bool Initialized => initialized;

	public bool IsSendingChunkCache { get; private set; }

	public string SessionTicket { get; private set; }

	private void Awake()
	{
		SetAsPersistant();
		sendCache = new Dictionary<string, string>();
		initialized = false;
		PlayFabSettings.TitleId = GetPlayFabTitleID();
		Leaderboard = base.gameObject.AddComponent<PlayFabLeaderboard>();
		MatchMaking = base.gameObject.AddComponent<PlayFabMatchMaking>();
		Users = base.gameObject.AddComponent<PlayFabUserHandling>();
		HatchManager.onLogout = (Action)Delegate.Combine(HatchManager.onLogout, new Action(Logout));
	}

	private string GetPlayFabTitleID()
	{
		if (HatchManager.IsProductionBuild())
		{
			return productionTitleID;
		}
		return devTitleID;
	}

	private void ResetCakeRacePersonalBests()
	{
		for (int i = 0; i < 7; i++)
		{
			string key = $"cake_race_track_{i}_pb";
			if (GameProgress.HasKey(key))
			{
				GameProgress.DeleteKey(key);
			}
		}
	}

	private bool HasCakeRacePersonalBests()
	{
		for (int i = 0; i < 7; i++)
		{
			if (GameProgress.HasKey($"cake_race_track_{i}_pb"))
			{
				return true;
			}
		}
		return false;
	}

	public void Login(HatchManager.HatchPlayer player)
	{
		GetPlayerCombinedInfoRequestParams infoRequestParameters = new GetPlayerCombinedInfoRequestParams
		{
			GetUserAccountInfo = true,
			GetPlayerStatistics = true
		};
		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
		{
			TitleId = GetPlayFabTitleID(),
			CustomId = player.HatchID,
			CreateAccount = true,
			InfoRequestParameters = infoRequestParameters
		}, OnLoginSuccess, OnLoginError);
	}

	private void OnLoginSuccess(LoginResult result)
	{
		string arg = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PlayFabManager] - OnLoginSuccessfull!\n");
		stringBuilder.AppendFormat("SessionTicket: {0}\n", result.SessionTicket);
		stringBuilder.AppendFormat("PlayFabId: {0}\n", result.PlayFabId);
		stringBuilder.AppendFormat("NewlyCreated: {0}\n", result.NewlyCreated);
		stringBuilder.AppendFormat("LastLoginTime: {0}\n", result.LastLoginTime);
		if (result.CustomData != null && result.CustomData is JsonObject)
		{
			stringBuilder.AppendLine("CustomData: " + JsonWrapper.SerializeObject(result.CustomData));
		}
		bool flag = HasCakeRacePersonalBests();
		GameProgress.DeleteKey("Statistics_" + PlayFabLeaderboard.Leaderboard.CakeRaceWins);
		GameProgress.DeleteKey("Statistics_" + PlayFabLeaderboard.Leaderboard.CakeRaceWins.ToString() + "_Version");
		if (result.InfoResultPayload != null)
		{
			stringBuilder.Append("InfoResultPayload.AccountInfo:\n");
			UserAccountInfo accountInfo = result.InfoResultPayload.AccountInfo;
			if (accountInfo != null && accountInfo.TitleInfo != null)
			{
				stringBuilder.AppendFormat("DisplayName: {0}\n", accountInfo.TitleInfo.DisplayName);
				HatchManager.CurrentPlayer.AddPlayFabDisplayName(accountInfo.TitleInfo.DisplayName);
			}
			if (accountInfo != null && accountInfo.FacebookInfo != null)
			{
				arg = accountInfo.FacebookInfo.FacebookId;
			}
			stringBuilder.Append("InfoResultPayload.PlayerStatistics:\n");
			List<StatisticValue> playerStatistics = result.InfoResultPayload.PlayerStatistics;
			if (playerStatistics != null)
			{
				foreach (StatisticValue item in playerStatistics)
				{
					stringBuilder.AppendFormat("{0}: {1}\n", item.StatisticName, item.Value);
					GameProgress.SetInt("Statistics_" + item.StatisticName, item.Value);
					GameProgress.SetInt("Statistics_" + item.StatisticName + "_Version", (int)item.Version);
					if (item.StatisticName == PlayFabLeaderboard.Leaderboard.CakeRaceWins.ToString() && flag && item.Value <= 0)
					{
						ResetCakeRacePersonalBests();
					}
				}
			}
		}
		stringBuilder.AppendLine("result.Request: " + result.Request.ToString());
		SessionTicket = result.SessionTicket;
		initialized = true;
		if (OnLogin != null)
		{
			OnLogin(result.PlayFabId, arg);
		}
		OnFacebookNameCallback(string.Empty, string.Empty);
	}

	private void OnFacebookNameCallback(string firstName, string lastName)
	{
		string arg = ((!string.IsNullOrEmpty(firstName)) ? firstName : "guest");
		string text = ((!string.IsNullOrEmpty(lastName) && lastName.Length >= 1) ? lastName : "-");
		string text2 = $"{arg}{text[0]}";
		if (string.IsNullOrEmpty(lastName))
		{
			text2 += HatchManager.CurrentPlayer.HatchCustomerID.Substring(0, 6);
		}
		int num = 24 - HatchManager.CurrentPlayer.HatchCustomerID.Length;
		if (text2.Length > num)
		{
			text2 = text2.Substring(0, num);
		}
		string text3 = $"{text2}|{HatchManager.CurrentPlayer.HatchCustomerID}";
		SetDisplayName(text3.ToString());
	}

	private void OnLoginError(PlayFabError error)
	{
		if (OnLogin != null)
		{
			OnLogin(string.Empty, string.Empty);
		}
	}

	public void Logout()
	{
		sendCache = new Dictionary<string, string>();
		initialized = false;
	}

	public void GetTitleData(List<string> keys, Action<GetTitleDataResult> cb, Action<PlayFabError> errorCb)
	{
		PlayFabClientAPI.GetTitleData(new GetTitleDataRequest
		{
			Keys = keys
		}, cb, errorCb);
	}

	private void SetDisplayName(string displayName)
	{
		if (initialized && (string.IsNullOrEmpty(HatchManager.CurrentPlayer.PlayFabDisplayName) || !HatchManager.CurrentPlayer.PlayFabDisplayName.Equals(displayName)))
		{
			PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = displayName
			}, OnUpdateUserTitleDisplayNameUpdated, OnError);
		}
	}

	private void OnUpdateUserTitleDisplayNameUpdated(UpdateUserTitleDisplayNameResult result)
	{
		HatchManager.CurrentPlayer.AddPlayFabDisplayName(result.DisplayName);
	}

	public void UpdateUserData(Dictionary<string, string> data, UserDataPermission permission)
	{
		if (initialized)
		{
			StartCoroutine(UpdateUserDataInChunks(data, permission));
		}
	}

	private IEnumerator UpdateUserDataInChunks(Dictionary<string, string> data, UserDataPermission permission)
	{
		AddToPendingCloudCache(data, permission);
		if (IsSendingChunkCache || waitingForMoreData)
		{
			if (waitingForMoreData)
			{
				waitForSeconds = 2f;
			}
			yield break;
		}
		waitingForMoreData = true;
		waitForSeconds = 2f;
		while (waitForSeconds > 0f)
		{
			waitForSeconds -= GameTime.RealTimeDelta;
			yield return null;
		}
		waitingForMoreData = false;
		EventManager.Send(new PlayFabEvent(PlayFabEvent.Type.UserDataUploadStarted));
		UserDataPermission sendPermission = UserDataPermission.Private;
		while (Initialized && UpdateDictionaryChunk(10, sendPermission))
		{
			while (IsSendingChunkCache)
			{
				yield return null;
			}
			sendPermission = ((sendPermission == UserDataPermission.Private) ? UserDataPermission.Public : UserDataPermission.Private);
		}
		EventManager.Send(new PlayFabEvent(PlayFabEvent.Type.UserDataUploadEnded));
	}

	private bool UpdateDictionaryChunk(int maxChunkSize, UserDataPermission newPermission)
	{
		if (IsSendingChunkCache)
		{
			if (sendCache.Count == 0)
			{
				IsSendingChunkCache = false;
			}
			return pendingCloudData.Count > 0;
		}
		sendCachePermission = newPermission;
		IsSendingChunkCache = true;
		sendCache.Clear();
		Dictionary<string, PendingData>.KeyCollection keys = pendingCloudData.Keys;
		string[] array = new string[pendingCloudData.Count];
		keys.CopyTo(array, 0);
		for (int i = 0; i < array.Length && i < maxChunkSize && pendingCloudData[array[i]].Permission == sendCachePermission; i++)
		{
			sendCache.Add(array[i], pendingCloudData[array[i]].Data);
			pendingCloudData.Remove(array[i]);
		}
		if (sendCache.Count > 0)
		{
			PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
			{
				Data = sendCache,
				Permission = sendCachePermission
			}, OnUserDataUpdated, OnUserDataUpdateError);
		}
		else
		{
			IsSendingChunkCache = false;
		}
		return pendingCloudData.Count > 0;
	}

	private void AddToPendingCloudCache(Dictionary<string, string> data, UserDataPermission permission)
	{
		if (pendingCloudData == null)
		{
			pendingCloudData = new Dictionary<string, PendingData>();
		}
		foreach (KeyValuePair<string, string> datum in data)
		{
			if (pendingCloudData.ContainsKey(datum.Key))
			{
				pendingCloudData[datum.Key].SetData(datum.Value, permission);
			}
			else
			{
				pendingCloudData.Add(datum.Key, new PendingData(datum.Value, permission));
			}
		}
	}

	private void OnUserDataUpdated(UpdateUserDataResult result)
	{
		if (Initialized)
		{
			IsSendingChunkCache = false;
			sendCache.Clear();
		}
	}

	private void OnUserDataUpdateError(PlayFabError error)
	{
		if (sendCache != null && sendCache.Count > 0)
		{
			AddToPendingCloudCache(sendCache, sendCachePermission);
			sendCache.Clear();
		}
		StartCoroutine(WaitAndResend());
	}

	private IEnumerator WaitAndResend()
	{
		float seconds = 10f;
		while (seconds > 0f)
		{
			seconds -= GameTime.RealTimeDelta;
			yield return null;
		}
		IsSendingChunkCache = false;
	}

	private void OnError(PlayFabError error)
	{
	}

	private void OnSuccess(PlayFabResultCommon result)
	{
	}
}
