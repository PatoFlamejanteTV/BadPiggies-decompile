using System;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using UnityEngine;

namespace PlayFab.Internal;

public static class PlayFabDeviceUtil
{
	private class DeviceInfoRequest : PlayFabRequestCommon
	{
		public PlayFabDataGatherer Info;
	}

	private static bool _needsAttribution;

	private static bool _gatherInfo;

	private static void DoAttributeInstall()
	{
		if (_needsAttribution && !PlayFabSettings.DisableAdvertising)
		{
			AttributeInstallRequest attributeInstallRequest = new AttributeInstallRequest();
			switch (PlayFabSettings.AdvertisingIdType)
			{
			case "Idfa":
				attributeInstallRequest.Idfa = PlayFabSettings.AdvertisingIdValue;
				break;
			case "Adid":
				attributeInstallRequest.Adid = PlayFabSettings.AdvertisingIdValue;
				break;
			}
			PlayFabClientAPI.AttributeInstall(attributeInstallRequest, OnAttributeInstall, null);
		}
	}

	private static void OnAttributeInstall(AttributeInstallResult result)
	{
		PlayFabSettings.AdvertisingIdType += "_Successful";
	}

	private static void SendDeviceInfoToPlayFab()
	{
		if (!PlayFabSettings.DisableDeviceInfo && _gatherInfo)
		{
			DeviceInfoRequest obj = new DeviceInfoRequest
			{
				Info = new PlayFabDataGatherer()
			};
			string apiEndpoint = "/Client/ReportDeviceInfo";
			PlayFabRequestCommon request = obj;
			AuthType authType = AuthType.LoginSession;
			Action<EmptyResult> resultCallback = OnGatherSuccess;
			PlayFabHttp.MakeApiCall(apiEndpoint, request, authType, resultCallback, OnGatherFail);
		}
	}

	private static void OnGatherSuccess(EmptyResult result)
	{
	}

	private static void OnGatherFail(PlayFabError error)
	{
	}

	public static void OnPlayFabLogin(PlayFabResultCommon result)
	{
		LoginResult loginResult = result as LoginResult;
		RegisterPlayFabUserResult registerPlayFabUserResult = result as RegisterPlayFabUserResult;
		if (loginResult != null || registerPlayFabUserResult != null)
		{
			_needsAttribution = false;
			_gatherInfo = false;
			if (loginResult != null && loginResult.SettingsForUser != null)
			{
				_needsAttribution = loginResult.SettingsForUser.NeedsAttribution;
			}
			else if (registerPlayFabUserResult != null && registerPlayFabUserResult.SettingsForUser != null)
			{
				_needsAttribution = registerPlayFabUserResult.SettingsForUser.NeedsAttribution;
			}
			if (loginResult != null && loginResult.SettingsForUser != null)
			{
				_gatherInfo = loginResult.SettingsForUser.GatherDeviceInfo;
			}
			else if (registerPlayFabUserResult != null && registerPlayFabUserResult.SettingsForUser != null)
			{
				_gatherInfo = registerPlayFabUserResult.SettingsForUser.GatherDeviceInfo;
			}
			if (PlayFabSettings.AdvertisingIdType != null && PlayFabSettings.AdvertisingIdValue != null)
			{
				DoAttributeInstall();
			}
			else
			{
				GetAdvertIdFromUnity();
			}
			SendDeviceInfoToPlayFab();
		}
	}

	private static void GetAdvertIdFromUnity()
	{
		Application.RequestAdvertisingIdentifierAsync(delegate(string advertisingId, bool trackingEnabled, string error)
		{
			PlayFabSettings.DisableAdvertising = !trackingEnabled;
			if (trackingEnabled)
			{
				PlayFabSettings.AdvertisingIdType = "Adid";
				PlayFabSettings.AdvertisingIdValue = advertisingId;
				DoAttributeInstall();
			}
		});
	}
}
