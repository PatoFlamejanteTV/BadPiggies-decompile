using System;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

public class PlayFabMatchMaking : MonoBehaviour
{
	public void FindOpponentReplay(int trackIndex, int playerLevel, int handiCap, Action<string> callback)
	{
		if (!Singleton<PlayFabManager>.Instance.Initialized)
		{
			return;
		}
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
		{
			FunctionName = "fetchOpponentReplay",
			FunctionParameter = new
			{
				trackIndex = trackIndex,
				playerLevel = playerLevel,
				handicap = handiCap
			}
		}, delegate(ExecuteCloudScriptResult result)
		{
			if (result.Logs != null)
			{
				foreach (LogStatement log in result.Logs)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (log.Data != null)
					{
						stringBuilder.AppendFormat("Data: '{0}'\n", log.Data);
					}
					if (!string.IsNullOrEmpty(log.Level))
					{
						stringBuilder.AppendFormat("Level: '{0}'\n", log.Level);
					}
					if (!string.IsNullOrEmpty(log.Message))
					{
						stringBuilder.AppendFormat("Message: '{0}'\n", log.Message);
					}
				}
			}
			string key = "replay";
			string obj = string.Empty;
			Debug.Log("[PlayFabManager] result.FunctionResult: " + JsonWrapper.SerializeObject(result.FunctionResult));
			JsonObject jsonObject = (JsonObject)result.FunctionResult;
			if (jsonObject != null)
			{
				if (jsonObject.ContainsKey(key) && jsonObject[key] is JsonObject && ((JsonObject)jsonObject[key]).ContainsKey("Value"))
				{
					obj = ((JsonObject)jsonObject[key])["Value"].ToString();
				}
				else
				{
					CakeRaceMenu.UseDefaultReplay = true;
				}
			}
			if (callback != null)
			{
				callback(obj);
			}
		}, delegate
		{
			CakeRaceMenu.UseDefaultReplay = true;
			if (callback != null)
			{
				callback(string.Empty);
			}
		});
	}

	public void GetCakeRaceWeek(Action<string, string> callback)
	{
		if (!Singleton<PlayFabManager>.Instance.Initialized)
		{
			return;
		}
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
		{
			FunctionName = "fetchCakeRaceWeek"
		}, delegate(ExecuteCloudScriptResult result)
		{
			if (result.Logs != null)
			{
				foreach (LogStatement log in result.Logs)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (log.Data != null)
					{
						stringBuilder.AppendFormat("Data: '{0}'\n", log.Data);
					}
					if (!string.IsNullOrEmpty(log.Level))
					{
						stringBuilder.AppendFormat("Level: '{0}'\n", log.Level);
					}
					if (!string.IsNullOrEmpty(log.Message))
					{
						stringBuilder.AppendFormat("Message: '{0}'\n", log.Message);
					}
				}
			}
			string arg = string.Empty;
			string arg2 = string.Empty;
			JsonObject jsonObject = (JsonObject)result.FunctionResult;
			if (jsonObject != null)
			{
				if (jsonObject.ContainsKey("week"))
				{
					arg = jsonObject["week"].ToString();
				}
				if (jsonObject.ContainsKey("daysleft"))
				{
					arg2 = jsonObject["daysleft"].ToString();
				}
			}
			if (callback != null)
			{
				callback(arg, arg2);
			}
		}, delegate
		{
			if (callback != null)
			{
				callback(string.Empty, string.Empty);
			}
		});
	}
}
