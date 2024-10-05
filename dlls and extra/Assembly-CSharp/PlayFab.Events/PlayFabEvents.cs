using System;
using PlayFab.ClientModels;
using PlayFab.Internal;
using PlayFab.SharedModels;

namespace PlayFab.Events;

public class PlayFabEvents
{
	public delegate void PlayFabErrorEvent(PlayFabRequestCommon request, PlayFabError error);

	public delegate void PlayFabResultEvent<in TResult>(TResult result) where TResult : PlayFabResultCommon;

	public delegate void PlayFabRequestEvent<in TRequest>(TRequest request) where TRequest : PlayFabRequestCommon;

	private static PlayFabEvents _instance;

	public event PlayFabResultEvent<LoginResult> OnLoginResultEvent;

	public event PlayFabRequestEvent<AcceptTradeRequest> OnAcceptTradeRequestEvent;

	public event PlayFabResultEvent<AcceptTradeResponse> OnAcceptTradeResultEvent;

	public event PlayFabRequestEvent<AddFriendRequest> OnAddFriendRequestEvent;

	public event PlayFabResultEvent<AddFriendResult> OnAddFriendResultEvent;

	public event PlayFabRequestEvent<AddGenericIDRequest> OnAddGenericIDRequestEvent;

	public event PlayFabResultEvent<AddGenericIDResult> OnAddGenericIDResultEvent;

	public event PlayFabRequestEvent<AddOrUpdateContactEmailRequest> OnAddOrUpdateContactEmailRequestEvent;

	public event PlayFabResultEvent<AddOrUpdateContactEmailResult> OnAddOrUpdateContactEmailResultEvent;

	public event PlayFabRequestEvent<AddSharedGroupMembersRequest> OnAddSharedGroupMembersRequestEvent;

	public event PlayFabResultEvent<AddSharedGroupMembersResult> OnAddSharedGroupMembersResultEvent;

	public event PlayFabRequestEvent<AddUsernamePasswordRequest> OnAddUsernamePasswordRequestEvent;

	public event PlayFabResultEvent<AddUsernamePasswordResult> OnAddUsernamePasswordResultEvent;

	public event PlayFabRequestEvent<AddUserVirtualCurrencyRequest> OnAddUserVirtualCurrencyRequestEvent;

	public event PlayFabResultEvent<ModifyUserVirtualCurrencyResult> OnAddUserVirtualCurrencyResultEvent;

	public event PlayFabRequestEvent<AndroidDevicePushNotificationRegistrationRequest> OnAndroidDevicePushNotificationRegistrationRequestEvent;

	public event PlayFabResultEvent<AndroidDevicePushNotificationRegistrationResult> OnAndroidDevicePushNotificationRegistrationResultEvent;

	public event PlayFabRequestEvent<AttributeInstallRequest> OnAttributeInstallRequestEvent;

	public event PlayFabResultEvent<AttributeInstallResult> OnAttributeInstallResultEvent;

	public event PlayFabRequestEvent<CancelTradeRequest> OnCancelTradeRequestEvent;

	public event PlayFabResultEvent<CancelTradeResponse> OnCancelTradeResultEvent;

	public event PlayFabRequestEvent<ConfirmPurchaseRequest> OnConfirmPurchaseRequestEvent;

	public event PlayFabResultEvent<ConfirmPurchaseResult> OnConfirmPurchaseResultEvent;

	public event PlayFabRequestEvent<ConsumeItemRequest> OnConsumeItemRequestEvent;

	public event PlayFabResultEvent<ConsumeItemResult> OnConsumeItemResultEvent;

	public event PlayFabRequestEvent<CreateSharedGroupRequest> OnCreateSharedGroupRequestEvent;

	public event PlayFabResultEvent<CreateSharedGroupResult> OnCreateSharedGroupResultEvent;

	public event PlayFabRequestEvent<ExecuteCloudScriptRequest> OnExecuteCloudScriptRequestEvent;

	public event PlayFabResultEvent<ExecuteCloudScriptResult> OnExecuteCloudScriptResultEvent;

	public event PlayFabRequestEvent<GetAccountInfoRequest> OnGetAccountInfoRequestEvent;

	public event PlayFabResultEvent<GetAccountInfoResult> OnGetAccountInfoResultEvent;

	public event PlayFabRequestEvent<ListUsersCharactersRequest> OnGetAllUsersCharactersRequestEvent;

	public event PlayFabResultEvent<ListUsersCharactersResult> OnGetAllUsersCharactersResultEvent;

	public event PlayFabRequestEvent<GetCatalogItemsRequest> OnGetCatalogItemsRequestEvent;

	public event PlayFabResultEvent<GetCatalogItemsResult> OnGetCatalogItemsResultEvent;

	public event PlayFabRequestEvent<GetCharacterDataRequest> OnGetCharacterDataRequestEvent;

	public event PlayFabResultEvent<GetCharacterDataResult> OnGetCharacterDataResultEvent;

	public event PlayFabRequestEvent<GetCharacterInventoryRequest> OnGetCharacterInventoryRequestEvent;

	public event PlayFabResultEvent<GetCharacterInventoryResult> OnGetCharacterInventoryResultEvent;

	public event PlayFabRequestEvent<GetCharacterLeaderboardRequest> OnGetCharacterLeaderboardRequestEvent;

	public event PlayFabResultEvent<GetCharacterLeaderboardResult> OnGetCharacterLeaderboardResultEvent;

	public event PlayFabRequestEvent<GetCharacterDataRequest> OnGetCharacterReadOnlyDataRequestEvent;

	public event PlayFabResultEvent<GetCharacterDataResult> OnGetCharacterReadOnlyDataResultEvent;

	public event PlayFabRequestEvent<GetCharacterStatisticsRequest> OnGetCharacterStatisticsRequestEvent;

	public event PlayFabResultEvent<GetCharacterStatisticsResult> OnGetCharacterStatisticsResultEvent;

	public event PlayFabRequestEvent<GetContentDownloadUrlRequest> OnGetContentDownloadUrlRequestEvent;

	public event PlayFabResultEvent<GetContentDownloadUrlResult> OnGetContentDownloadUrlResultEvent;

	public event PlayFabRequestEvent<CurrentGamesRequest> OnGetCurrentGamesRequestEvent;

	public event PlayFabResultEvent<CurrentGamesResult> OnGetCurrentGamesResultEvent;

	public event PlayFabRequestEvent<GetFriendLeaderboardRequest> OnGetFriendLeaderboardRequestEvent;

	public event PlayFabResultEvent<GetLeaderboardResult> OnGetFriendLeaderboardResultEvent;

	public event PlayFabRequestEvent<GetFriendLeaderboardAroundPlayerRequest> OnGetFriendLeaderboardAroundPlayerRequestEvent;

	public event PlayFabResultEvent<GetFriendLeaderboardAroundPlayerResult> OnGetFriendLeaderboardAroundPlayerResultEvent;

	public event PlayFabRequestEvent<GetFriendsListRequest> OnGetFriendsListRequestEvent;

	public event PlayFabResultEvent<GetFriendsListResult> OnGetFriendsListResultEvent;

	public event PlayFabRequestEvent<GameServerRegionsRequest> OnGetGameServerRegionsRequestEvent;

	public event PlayFabResultEvent<GameServerRegionsResult> OnGetGameServerRegionsResultEvent;

	public event PlayFabRequestEvent<GetLeaderboardRequest> OnGetLeaderboardRequestEvent;

	public event PlayFabResultEvent<GetLeaderboardResult> OnGetLeaderboardResultEvent;

	public event PlayFabRequestEvent<GetLeaderboardAroundCharacterRequest> OnGetLeaderboardAroundCharacterRequestEvent;

	public event PlayFabResultEvent<GetLeaderboardAroundCharacterResult> OnGetLeaderboardAroundCharacterResultEvent;

	public event PlayFabRequestEvent<GetLeaderboardAroundPlayerRequest> OnGetLeaderboardAroundPlayerRequestEvent;

	public event PlayFabResultEvent<GetLeaderboardAroundPlayerResult> OnGetLeaderboardAroundPlayerResultEvent;

	public event PlayFabRequestEvent<GetLeaderboardForUsersCharactersRequest> OnGetLeaderboardForUserCharactersRequestEvent;

	public event PlayFabResultEvent<GetLeaderboardForUsersCharactersResult> OnGetLeaderboardForUserCharactersResultEvent;

	public event PlayFabRequestEvent<GetPaymentTokenRequest> OnGetPaymentTokenRequestEvent;

	public event PlayFabResultEvent<GetPaymentTokenResult> OnGetPaymentTokenResultEvent;

	public event PlayFabRequestEvent<GetPhotonAuthenticationTokenRequest> OnGetPhotonAuthenticationTokenRequestEvent;

	public event PlayFabResultEvent<GetPhotonAuthenticationTokenResult> OnGetPhotonAuthenticationTokenResultEvent;

	public event PlayFabRequestEvent<GetPlayerCombinedInfoRequest> OnGetPlayerCombinedInfoRequestEvent;

	public event PlayFabResultEvent<GetPlayerCombinedInfoResult> OnGetPlayerCombinedInfoResultEvent;

	public event PlayFabRequestEvent<GetPlayerProfileRequest> OnGetPlayerProfileRequestEvent;

	public event PlayFabResultEvent<GetPlayerProfileResult> OnGetPlayerProfileResultEvent;

	public event PlayFabRequestEvent<GetPlayerSegmentsRequest> OnGetPlayerSegmentsRequestEvent;

	public event PlayFabResultEvent<GetPlayerSegmentsResult> OnGetPlayerSegmentsResultEvent;

	public event PlayFabRequestEvent<GetPlayerStatisticsRequest> OnGetPlayerStatisticsRequestEvent;

	public event PlayFabResultEvent<GetPlayerStatisticsResult> OnGetPlayerStatisticsResultEvent;

	public event PlayFabRequestEvent<GetPlayerStatisticVersionsRequest> OnGetPlayerStatisticVersionsRequestEvent;

	public event PlayFabResultEvent<GetPlayerStatisticVersionsResult> OnGetPlayerStatisticVersionsResultEvent;

	public event PlayFabRequestEvent<GetPlayerTagsRequest> OnGetPlayerTagsRequestEvent;

	public event PlayFabResultEvent<GetPlayerTagsResult> OnGetPlayerTagsResultEvent;

	public event PlayFabRequestEvent<GetPlayerTradesRequest> OnGetPlayerTradesRequestEvent;

	public event PlayFabResultEvent<GetPlayerTradesResponse> OnGetPlayerTradesResultEvent;

	public event PlayFabRequestEvent<GetPlayFabIDsFromFacebookIDsRequest> OnGetPlayFabIDsFromFacebookIDsRequestEvent;

	public event PlayFabResultEvent<GetPlayFabIDsFromFacebookIDsResult> OnGetPlayFabIDsFromFacebookIDsResultEvent;

	public event PlayFabRequestEvent<GetPlayFabIDsFromGameCenterIDsRequest> OnGetPlayFabIDsFromGameCenterIDsRequestEvent;

	public event PlayFabResultEvent<GetPlayFabIDsFromGameCenterIDsResult> OnGetPlayFabIDsFromGameCenterIDsResultEvent;

	public event PlayFabRequestEvent<GetPlayFabIDsFromGenericIDsRequest> OnGetPlayFabIDsFromGenericIDsRequestEvent;

	public event PlayFabResultEvent<GetPlayFabIDsFromGenericIDsResult> OnGetPlayFabIDsFromGenericIDsResultEvent;

	public event PlayFabRequestEvent<GetPlayFabIDsFromGoogleIDsRequest> OnGetPlayFabIDsFromGoogleIDsRequestEvent;

	public event PlayFabResultEvent<GetPlayFabIDsFromGoogleIDsResult> OnGetPlayFabIDsFromGoogleIDsResultEvent;

	public event PlayFabRequestEvent<GetPlayFabIDsFromKongregateIDsRequest> OnGetPlayFabIDsFromKongregateIDsRequestEvent;

	public event PlayFabResultEvent<GetPlayFabIDsFromKongregateIDsResult> OnGetPlayFabIDsFromKongregateIDsResultEvent;

	public event PlayFabRequestEvent<GetPlayFabIDsFromSteamIDsRequest> OnGetPlayFabIDsFromSteamIDsRequestEvent;

	public event PlayFabResultEvent<GetPlayFabIDsFromSteamIDsResult> OnGetPlayFabIDsFromSteamIDsResultEvent;

	public event PlayFabRequestEvent<GetPlayFabIDsFromTwitchIDsRequest> OnGetPlayFabIDsFromTwitchIDsRequestEvent;

	public event PlayFabResultEvent<GetPlayFabIDsFromTwitchIDsResult> OnGetPlayFabIDsFromTwitchIDsResultEvent;

	public event PlayFabRequestEvent<GetPublisherDataRequest> OnGetPublisherDataRequestEvent;

	public event PlayFabResultEvent<GetPublisherDataResult> OnGetPublisherDataResultEvent;

	public event PlayFabRequestEvent<GetPurchaseRequest> OnGetPurchaseRequestEvent;

	public event PlayFabResultEvent<GetPurchaseResult> OnGetPurchaseResultEvent;

	public event PlayFabRequestEvent<GetSharedGroupDataRequest> OnGetSharedGroupDataRequestEvent;

	public event PlayFabResultEvent<GetSharedGroupDataResult> OnGetSharedGroupDataResultEvent;

	public event PlayFabRequestEvent<GetStoreItemsRequest> OnGetStoreItemsRequestEvent;

	public event PlayFabResultEvent<GetStoreItemsResult> OnGetStoreItemsResultEvent;

	public event PlayFabRequestEvent<GetTimeRequest> OnGetTimeRequestEvent;

	public event PlayFabResultEvent<GetTimeResult> OnGetTimeResultEvent;

	public event PlayFabRequestEvent<GetTitleDataRequest> OnGetTitleDataRequestEvent;

	public event PlayFabResultEvent<GetTitleDataResult> OnGetTitleDataResultEvent;

	public event PlayFabRequestEvent<GetTitleNewsRequest> OnGetTitleNewsRequestEvent;

	public event PlayFabResultEvent<GetTitleNewsResult> OnGetTitleNewsResultEvent;

	public event PlayFabRequestEvent<GetTitlePublicKeyRequest> OnGetTitlePublicKeyRequestEvent;

	public event PlayFabResultEvent<GetTitlePublicKeyResult> OnGetTitlePublicKeyResultEvent;

	public event PlayFabRequestEvent<GetTradeStatusRequest> OnGetTradeStatusRequestEvent;

	public event PlayFabResultEvent<GetTradeStatusResponse> OnGetTradeStatusResultEvent;

	public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserDataRequestEvent;

	public event PlayFabResultEvent<GetUserDataResult> OnGetUserDataResultEvent;

	public event PlayFabRequestEvent<GetUserInventoryRequest> OnGetUserInventoryRequestEvent;

	public event PlayFabResultEvent<GetUserInventoryResult> OnGetUserInventoryResultEvent;

	public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserPublisherDataRequestEvent;

	public event PlayFabResultEvent<GetUserDataResult> OnGetUserPublisherDataResultEvent;

	public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserPublisherReadOnlyDataRequestEvent;

	public event PlayFabResultEvent<GetUserDataResult> OnGetUserPublisherReadOnlyDataResultEvent;

	public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserReadOnlyDataRequestEvent;

	public event PlayFabResultEvent<GetUserDataResult> OnGetUserReadOnlyDataResultEvent;

	public event PlayFabRequestEvent<GetWindowsHelloChallengeRequest> OnGetWindowsHelloChallengeRequestEvent;

	public event PlayFabResultEvent<GetWindowsHelloChallengeResponse> OnGetWindowsHelloChallengeResultEvent;

	public event PlayFabRequestEvent<GrantCharacterToUserRequest> OnGrantCharacterToUserRequestEvent;

	public event PlayFabResultEvent<GrantCharacterToUserResult> OnGrantCharacterToUserResultEvent;

	public event PlayFabRequestEvent<LinkAndroidDeviceIDRequest> OnLinkAndroidDeviceIDRequestEvent;

	public event PlayFabResultEvent<LinkAndroidDeviceIDResult> OnLinkAndroidDeviceIDResultEvent;

	public event PlayFabRequestEvent<LinkCustomIDRequest> OnLinkCustomIDRequestEvent;

	public event PlayFabResultEvent<LinkCustomIDResult> OnLinkCustomIDResultEvent;

	public event PlayFabRequestEvent<LinkFacebookAccountRequest> OnLinkFacebookAccountRequestEvent;

	public event PlayFabResultEvent<LinkFacebookAccountResult> OnLinkFacebookAccountResultEvent;

	public event PlayFabRequestEvent<LinkGameCenterAccountRequest> OnLinkGameCenterAccountRequestEvent;

	public event PlayFabResultEvent<LinkGameCenterAccountResult> OnLinkGameCenterAccountResultEvent;

	public event PlayFabRequestEvent<LinkGoogleAccountRequest> OnLinkGoogleAccountRequestEvent;

	public event PlayFabResultEvent<LinkGoogleAccountResult> OnLinkGoogleAccountResultEvent;

	public event PlayFabRequestEvent<LinkIOSDeviceIDRequest> OnLinkIOSDeviceIDRequestEvent;

	public event PlayFabResultEvent<LinkIOSDeviceIDResult> OnLinkIOSDeviceIDResultEvent;

	public event PlayFabRequestEvent<LinkKongregateAccountRequest> OnLinkKongregateRequestEvent;

	public event PlayFabResultEvent<LinkKongregateAccountResult> OnLinkKongregateResultEvent;

	public event PlayFabRequestEvent<LinkSteamAccountRequest> OnLinkSteamAccountRequestEvent;

	public event PlayFabResultEvent<LinkSteamAccountResult> OnLinkSteamAccountResultEvent;

	public event PlayFabRequestEvent<LinkTwitchAccountRequest> OnLinkTwitchRequestEvent;

	public event PlayFabResultEvent<LinkTwitchAccountResult> OnLinkTwitchResultEvent;

	public event PlayFabRequestEvent<LinkWindowsHelloAccountRequest> OnLinkWindowsHelloRequestEvent;

	public event PlayFabResultEvent<LinkWindowsHelloAccountResponse> OnLinkWindowsHelloResultEvent;

	public event PlayFabRequestEvent<LoginWithAndroidDeviceIDRequest> OnLoginWithAndroidDeviceIDRequestEvent;

	public event PlayFabRequestEvent<LoginWithCustomIDRequest> OnLoginWithCustomIDRequestEvent;

	public event PlayFabRequestEvent<LoginWithEmailAddressRequest> OnLoginWithEmailAddressRequestEvent;

	public event PlayFabRequestEvent<LoginWithFacebookRequest> OnLoginWithFacebookRequestEvent;

	public event PlayFabRequestEvent<LoginWithGameCenterRequest> OnLoginWithGameCenterRequestEvent;

	public event PlayFabRequestEvent<LoginWithGoogleAccountRequest> OnLoginWithGoogleAccountRequestEvent;

	public event PlayFabRequestEvent<LoginWithIOSDeviceIDRequest> OnLoginWithIOSDeviceIDRequestEvent;

	public event PlayFabRequestEvent<LoginWithKongregateRequest> OnLoginWithKongregateRequestEvent;

	public event PlayFabRequestEvent<LoginWithPlayFabRequest> OnLoginWithPlayFabRequestEvent;

	public event PlayFabRequestEvent<LoginWithSteamRequest> OnLoginWithSteamRequestEvent;

	public event PlayFabRequestEvent<LoginWithTwitchRequest> OnLoginWithTwitchRequestEvent;

	public event PlayFabRequestEvent<LoginWithWindowsHelloRequest> OnLoginWithWindowsHelloRequestEvent;

	public event PlayFabRequestEvent<MatchmakeRequest> OnMatchmakeRequestEvent;

	public event PlayFabResultEvent<MatchmakeResult> OnMatchmakeResultEvent;

	public event PlayFabRequestEvent<OpenTradeRequest> OnOpenTradeRequestEvent;

	public event PlayFabResultEvent<OpenTradeResponse> OnOpenTradeResultEvent;

	public event PlayFabRequestEvent<PayForPurchaseRequest> OnPayForPurchaseRequestEvent;

	public event PlayFabResultEvent<PayForPurchaseResult> OnPayForPurchaseResultEvent;

	public event PlayFabRequestEvent<PurchaseItemRequest> OnPurchaseItemRequestEvent;

	public event PlayFabResultEvent<PurchaseItemResult> OnPurchaseItemResultEvent;

	public event PlayFabRequestEvent<RedeemCouponRequest> OnRedeemCouponRequestEvent;

	public event PlayFabResultEvent<RedeemCouponResult> OnRedeemCouponResultEvent;

	public event PlayFabRequestEvent<RegisterForIOSPushNotificationRequest> OnRegisterForIOSPushNotificationRequestEvent;

	public event PlayFabResultEvent<RegisterForIOSPushNotificationResult> OnRegisterForIOSPushNotificationResultEvent;

	public event PlayFabRequestEvent<RegisterPlayFabUserRequest> OnRegisterPlayFabUserRequestEvent;

	public event PlayFabResultEvent<RegisterPlayFabUserResult> OnRegisterPlayFabUserResultEvent;

	public event PlayFabRequestEvent<RegisterWithWindowsHelloRequest> OnRegisterWithWindowsHelloRequestEvent;

	public event PlayFabRequestEvent<RemoveContactEmailRequest> OnRemoveContactEmailRequestEvent;

	public event PlayFabResultEvent<RemoveContactEmailResult> OnRemoveContactEmailResultEvent;

	public event PlayFabRequestEvent<RemoveFriendRequest> OnRemoveFriendRequestEvent;

	public event PlayFabResultEvent<RemoveFriendResult> OnRemoveFriendResultEvent;

	public event PlayFabRequestEvent<RemoveGenericIDRequest> OnRemoveGenericIDRequestEvent;

	public event PlayFabResultEvent<RemoveGenericIDResult> OnRemoveGenericIDResultEvent;

	public event PlayFabRequestEvent<RemoveSharedGroupMembersRequest> OnRemoveSharedGroupMembersRequestEvent;

	public event PlayFabResultEvent<RemoveSharedGroupMembersResult> OnRemoveSharedGroupMembersResultEvent;

	public event PlayFabRequestEvent<DeviceInfoRequest> OnReportDeviceInfoRequestEvent;

	public event PlayFabResultEvent<EmptyResult> OnReportDeviceInfoResultEvent;

	public event PlayFabRequestEvent<ReportPlayerClientRequest> OnReportPlayerRequestEvent;

	public event PlayFabResultEvent<ReportPlayerClientResult> OnReportPlayerResultEvent;

	public event PlayFabRequestEvent<RestoreIOSPurchasesRequest> OnRestoreIOSPurchasesRequestEvent;

	public event PlayFabResultEvent<RestoreIOSPurchasesResult> OnRestoreIOSPurchasesResultEvent;

	public event PlayFabRequestEvent<SendAccountRecoveryEmailRequest> OnSendAccountRecoveryEmailRequestEvent;

	public event PlayFabResultEvent<SendAccountRecoveryEmailResult> OnSendAccountRecoveryEmailResultEvent;

	public event PlayFabRequestEvent<SetFriendTagsRequest> OnSetFriendTagsRequestEvent;

	public event PlayFabResultEvent<SetFriendTagsResult> OnSetFriendTagsResultEvent;

	public event PlayFabRequestEvent<SetPlayerSecretRequest> OnSetPlayerSecretRequestEvent;

	public event PlayFabResultEvent<SetPlayerSecretResult> OnSetPlayerSecretResultEvent;

	public event PlayFabRequestEvent<StartGameRequest> OnStartGameRequestEvent;

	public event PlayFabResultEvent<StartGameResult> OnStartGameResultEvent;

	public event PlayFabRequestEvent<StartPurchaseRequest> OnStartPurchaseRequestEvent;

	public event PlayFabResultEvent<StartPurchaseResult> OnStartPurchaseResultEvent;

	public event PlayFabRequestEvent<SubtractUserVirtualCurrencyRequest> OnSubtractUserVirtualCurrencyRequestEvent;

	public event PlayFabResultEvent<ModifyUserVirtualCurrencyResult> OnSubtractUserVirtualCurrencyResultEvent;

	public event PlayFabRequestEvent<UnlinkAndroidDeviceIDRequest> OnUnlinkAndroidDeviceIDRequestEvent;

	public event PlayFabResultEvent<UnlinkAndroidDeviceIDResult> OnUnlinkAndroidDeviceIDResultEvent;

	public event PlayFabRequestEvent<UnlinkCustomIDRequest> OnUnlinkCustomIDRequestEvent;

	public event PlayFabResultEvent<UnlinkCustomIDResult> OnUnlinkCustomIDResultEvent;

	public event PlayFabRequestEvent<UnlinkFacebookAccountRequest> OnUnlinkFacebookAccountRequestEvent;

	public event PlayFabResultEvent<UnlinkFacebookAccountResult> OnUnlinkFacebookAccountResultEvent;

	public event PlayFabRequestEvent<UnlinkGameCenterAccountRequest> OnUnlinkGameCenterAccountRequestEvent;

	public event PlayFabResultEvent<UnlinkGameCenterAccountResult> OnUnlinkGameCenterAccountResultEvent;

	public event PlayFabRequestEvent<UnlinkGoogleAccountRequest> OnUnlinkGoogleAccountRequestEvent;

	public event PlayFabResultEvent<UnlinkGoogleAccountResult> OnUnlinkGoogleAccountResultEvent;

	public event PlayFabRequestEvent<UnlinkIOSDeviceIDRequest> OnUnlinkIOSDeviceIDRequestEvent;

	public event PlayFabResultEvent<UnlinkIOSDeviceIDResult> OnUnlinkIOSDeviceIDResultEvent;

	public event PlayFabRequestEvent<UnlinkKongregateAccountRequest> OnUnlinkKongregateRequestEvent;

	public event PlayFabResultEvent<UnlinkKongregateAccountResult> OnUnlinkKongregateResultEvent;

	public event PlayFabRequestEvent<UnlinkSteamAccountRequest> OnUnlinkSteamAccountRequestEvent;

	public event PlayFabResultEvent<UnlinkSteamAccountResult> OnUnlinkSteamAccountResultEvent;

	public event PlayFabRequestEvent<UnlinkTwitchAccountRequest> OnUnlinkTwitchRequestEvent;

	public event PlayFabResultEvent<UnlinkTwitchAccountResult> OnUnlinkTwitchResultEvent;

	public event PlayFabRequestEvent<UnlinkWindowsHelloAccountRequest> OnUnlinkWindowsHelloRequestEvent;

	public event PlayFabResultEvent<UnlinkWindowsHelloAccountResponse> OnUnlinkWindowsHelloResultEvent;

	public event PlayFabRequestEvent<UnlockContainerInstanceRequest> OnUnlockContainerInstanceRequestEvent;

	public event PlayFabResultEvent<UnlockContainerItemResult> OnUnlockContainerInstanceResultEvent;

	public event PlayFabRequestEvent<UnlockContainerItemRequest> OnUnlockContainerItemRequestEvent;

	public event PlayFabResultEvent<UnlockContainerItemResult> OnUnlockContainerItemResultEvent;

	public event PlayFabRequestEvent<UpdateAvatarUrlRequest> OnUpdateAvatarUrlRequestEvent;

	public event PlayFabResultEvent<EmptyResult> OnUpdateAvatarUrlResultEvent;

	public event PlayFabRequestEvent<UpdateCharacterDataRequest> OnUpdateCharacterDataRequestEvent;

	public event PlayFabResultEvent<UpdateCharacterDataResult> OnUpdateCharacterDataResultEvent;

	public event PlayFabRequestEvent<UpdateCharacterStatisticsRequest> OnUpdateCharacterStatisticsRequestEvent;

	public event PlayFabResultEvent<UpdateCharacterStatisticsResult> OnUpdateCharacterStatisticsResultEvent;

	public event PlayFabRequestEvent<UpdatePlayerStatisticsRequest> OnUpdatePlayerStatisticsRequestEvent;

	public event PlayFabResultEvent<UpdatePlayerStatisticsResult> OnUpdatePlayerStatisticsResultEvent;

	public event PlayFabRequestEvent<UpdateSharedGroupDataRequest> OnUpdateSharedGroupDataRequestEvent;

	public event PlayFabResultEvent<UpdateSharedGroupDataResult> OnUpdateSharedGroupDataResultEvent;

	public event PlayFabRequestEvent<UpdateUserDataRequest> OnUpdateUserDataRequestEvent;

	public event PlayFabResultEvent<UpdateUserDataResult> OnUpdateUserDataResultEvent;

	public event PlayFabRequestEvent<UpdateUserDataRequest> OnUpdateUserPublisherDataRequestEvent;

	public event PlayFabResultEvent<UpdateUserDataResult> OnUpdateUserPublisherDataResultEvent;

	public event PlayFabRequestEvent<UpdateUserTitleDisplayNameRequest> OnUpdateUserTitleDisplayNameRequestEvent;

	public event PlayFabResultEvent<UpdateUserTitleDisplayNameResult> OnUpdateUserTitleDisplayNameResultEvent;

	public event PlayFabRequestEvent<ValidateAmazonReceiptRequest> OnValidateAmazonIAPReceiptRequestEvent;

	public event PlayFabResultEvent<ValidateAmazonReceiptResult> OnValidateAmazonIAPReceiptResultEvent;

	public event PlayFabRequestEvent<ValidateGooglePlayPurchaseRequest> OnValidateGooglePlayPurchaseRequestEvent;

	public event PlayFabResultEvent<ValidateGooglePlayPurchaseResult> OnValidateGooglePlayPurchaseResultEvent;

	public event PlayFabRequestEvent<ValidateIOSReceiptRequest> OnValidateIOSReceiptRequestEvent;

	public event PlayFabResultEvent<ValidateIOSReceiptResult> OnValidateIOSReceiptResultEvent;

	public event PlayFabRequestEvent<ValidateWindowsReceiptRequest> OnValidateWindowsStoreReceiptRequestEvent;

	public event PlayFabResultEvent<ValidateWindowsReceiptResult> OnValidateWindowsStoreReceiptResultEvent;

	public event PlayFabRequestEvent<WriteClientCharacterEventRequest> OnWriteCharacterEventRequestEvent;

	public event PlayFabResultEvent<WriteEventResponse> OnWriteCharacterEventResultEvent;

	public event PlayFabRequestEvent<WriteClientPlayerEventRequest> OnWritePlayerEventRequestEvent;

	public event PlayFabResultEvent<WriteEventResponse> OnWritePlayerEventResultEvent;

	public event PlayFabRequestEvent<WriteTitleEventRequest> OnWriteTitleEventRequestEvent;

	public event PlayFabResultEvent<WriteEventResponse> OnWriteTitleEventResultEvent;

	public event PlayFabErrorEvent OnGlobalErrorEvent;

	private PlayFabEvents()
	{
	}

	public static PlayFabEvents Init()
	{
		if (_instance == null)
		{
			_instance = new PlayFabEvents();
		}
		PlayFabHttp.ApiProcessingEventHandler += _instance.OnProcessingEvent;
		PlayFabHttp.ApiProcessingErrorEventHandler += _instance.OnProcessingErrorEvent;
		return _instance;
	}

	public void UnregisterInstance(object instance)
	{
		Delegate[] invocationList;
		if (this.OnLoginResultEvent != null)
		{
			invocationList = this.OnLoginResultEvent.GetInvocationList();
			foreach (Delegate @delegate in invocationList)
			{
				if (@delegate.Target == instance)
				{
					OnLoginResultEvent -= (PlayFabResultEvent<LoginResult>)@delegate;
				}
			}
		}
		if (this.OnAcceptTradeRequestEvent != null)
		{
			invocationList = this.OnAcceptTradeRequestEvent.GetInvocationList();
			foreach (Delegate delegate2 in invocationList)
			{
				if (delegate2.Target == instance)
				{
					OnAcceptTradeRequestEvent -= (PlayFabRequestEvent<AcceptTradeRequest>)delegate2;
				}
			}
		}
		if (this.OnAcceptTradeResultEvent != null)
		{
			invocationList = this.OnAcceptTradeResultEvent.GetInvocationList();
			foreach (Delegate delegate3 in invocationList)
			{
				if (delegate3.Target == instance)
				{
					OnAcceptTradeResultEvent -= (PlayFabResultEvent<AcceptTradeResponse>)delegate3;
				}
			}
		}
		if (this.OnAddFriendRequestEvent != null)
		{
			invocationList = this.OnAddFriendRequestEvent.GetInvocationList();
			foreach (Delegate delegate4 in invocationList)
			{
				if (delegate4.Target == instance)
				{
					OnAddFriendRequestEvent -= (PlayFabRequestEvent<AddFriendRequest>)delegate4;
				}
			}
		}
		if (this.OnAddFriendResultEvent != null)
		{
			invocationList = this.OnAddFriendResultEvent.GetInvocationList();
			foreach (Delegate delegate5 in invocationList)
			{
				if (delegate5.Target == instance)
				{
					OnAddFriendResultEvent -= (PlayFabResultEvent<AddFriendResult>)delegate5;
				}
			}
		}
		if (this.OnAddGenericIDRequestEvent != null)
		{
			invocationList = this.OnAddGenericIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate6 in invocationList)
			{
				if (delegate6.Target == instance)
				{
					OnAddGenericIDRequestEvent -= (PlayFabRequestEvent<AddGenericIDRequest>)delegate6;
				}
			}
		}
		if (this.OnAddGenericIDResultEvent != null)
		{
			invocationList = this.OnAddGenericIDResultEvent.GetInvocationList();
			foreach (Delegate delegate7 in invocationList)
			{
				if (delegate7.Target == instance)
				{
					OnAddGenericIDResultEvent -= (PlayFabResultEvent<AddGenericIDResult>)delegate7;
				}
			}
		}
		if (this.OnAddOrUpdateContactEmailRequestEvent != null)
		{
			invocationList = this.OnAddOrUpdateContactEmailRequestEvent.GetInvocationList();
			foreach (Delegate delegate8 in invocationList)
			{
				if (delegate8.Target == instance)
				{
					OnAddOrUpdateContactEmailRequestEvent -= (PlayFabRequestEvent<AddOrUpdateContactEmailRequest>)delegate8;
				}
			}
		}
		if (this.OnAddOrUpdateContactEmailResultEvent != null)
		{
			invocationList = this.OnAddOrUpdateContactEmailResultEvent.GetInvocationList();
			foreach (Delegate delegate9 in invocationList)
			{
				if (delegate9.Target == instance)
				{
					OnAddOrUpdateContactEmailResultEvent -= (PlayFabResultEvent<AddOrUpdateContactEmailResult>)delegate9;
				}
			}
		}
		if (this.OnAddSharedGroupMembersRequestEvent != null)
		{
			invocationList = this.OnAddSharedGroupMembersRequestEvent.GetInvocationList();
			foreach (Delegate delegate10 in invocationList)
			{
				if (delegate10.Target == instance)
				{
					OnAddSharedGroupMembersRequestEvent -= (PlayFabRequestEvent<AddSharedGroupMembersRequest>)delegate10;
				}
			}
		}
		if (this.OnAddSharedGroupMembersResultEvent != null)
		{
			invocationList = this.OnAddSharedGroupMembersResultEvent.GetInvocationList();
			foreach (Delegate delegate11 in invocationList)
			{
				if (delegate11.Target == instance)
				{
					OnAddSharedGroupMembersResultEvent -= (PlayFabResultEvent<AddSharedGroupMembersResult>)delegate11;
				}
			}
		}
		if (this.OnAddUsernamePasswordRequestEvent != null)
		{
			invocationList = this.OnAddUsernamePasswordRequestEvent.GetInvocationList();
			foreach (Delegate delegate12 in invocationList)
			{
				if (delegate12.Target == instance)
				{
					OnAddUsernamePasswordRequestEvent -= (PlayFabRequestEvent<AddUsernamePasswordRequest>)delegate12;
				}
			}
		}
		if (this.OnAddUsernamePasswordResultEvent != null)
		{
			invocationList = this.OnAddUsernamePasswordResultEvent.GetInvocationList();
			foreach (Delegate delegate13 in invocationList)
			{
				if (delegate13.Target == instance)
				{
					OnAddUsernamePasswordResultEvent -= (PlayFabResultEvent<AddUsernamePasswordResult>)delegate13;
				}
			}
		}
		if (this.OnAddUserVirtualCurrencyRequestEvent != null)
		{
			invocationList = this.OnAddUserVirtualCurrencyRequestEvent.GetInvocationList();
			foreach (Delegate delegate14 in invocationList)
			{
				if (delegate14.Target == instance)
				{
					OnAddUserVirtualCurrencyRequestEvent -= (PlayFabRequestEvent<AddUserVirtualCurrencyRequest>)delegate14;
				}
			}
		}
		if (this.OnAddUserVirtualCurrencyResultEvent != null)
		{
			invocationList = this.OnAddUserVirtualCurrencyResultEvent.GetInvocationList();
			foreach (Delegate delegate15 in invocationList)
			{
				if (delegate15.Target == instance)
				{
					OnAddUserVirtualCurrencyResultEvent -= (PlayFabResultEvent<ModifyUserVirtualCurrencyResult>)delegate15;
				}
			}
		}
		if (this.OnAndroidDevicePushNotificationRegistrationRequestEvent != null)
		{
			invocationList = this.OnAndroidDevicePushNotificationRegistrationRequestEvent.GetInvocationList();
			foreach (Delegate delegate16 in invocationList)
			{
				if (delegate16.Target == instance)
				{
					OnAndroidDevicePushNotificationRegistrationRequestEvent -= (PlayFabRequestEvent<AndroidDevicePushNotificationRegistrationRequest>)delegate16;
				}
			}
		}
		if (this.OnAndroidDevicePushNotificationRegistrationResultEvent != null)
		{
			invocationList = this.OnAndroidDevicePushNotificationRegistrationResultEvent.GetInvocationList();
			foreach (Delegate delegate17 in invocationList)
			{
				if (delegate17.Target == instance)
				{
					OnAndroidDevicePushNotificationRegistrationResultEvent -= (PlayFabResultEvent<AndroidDevicePushNotificationRegistrationResult>)delegate17;
				}
			}
		}
		if (this.OnAttributeInstallRequestEvent != null)
		{
			invocationList = this.OnAttributeInstallRequestEvent.GetInvocationList();
			foreach (Delegate delegate18 in invocationList)
			{
				if (delegate18.Target == instance)
				{
					OnAttributeInstallRequestEvent -= (PlayFabRequestEvent<AttributeInstallRequest>)delegate18;
				}
			}
		}
		if (this.OnAttributeInstallResultEvent != null)
		{
			invocationList = this.OnAttributeInstallResultEvent.GetInvocationList();
			foreach (Delegate delegate19 in invocationList)
			{
				if (delegate19.Target == instance)
				{
					OnAttributeInstallResultEvent -= (PlayFabResultEvent<AttributeInstallResult>)delegate19;
				}
			}
		}
		if (this.OnCancelTradeRequestEvent != null)
		{
			invocationList = this.OnCancelTradeRequestEvent.GetInvocationList();
			foreach (Delegate delegate20 in invocationList)
			{
				if (delegate20.Target == instance)
				{
					OnCancelTradeRequestEvent -= (PlayFabRequestEvent<CancelTradeRequest>)delegate20;
				}
			}
		}
		if (this.OnCancelTradeResultEvent != null)
		{
			invocationList = this.OnCancelTradeResultEvent.GetInvocationList();
			foreach (Delegate delegate21 in invocationList)
			{
				if (delegate21.Target == instance)
				{
					OnCancelTradeResultEvent -= (PlayFabResultEvent<CancelTradeResponse>)delegate21;
				}
			}
		}
		if (this.OnConfirmPurchaseRequestEvent != null)
		{
			invocationList = this.OnConfirmPurchaseRequestEvent.GetInvocationList();
			foreach (Delegate delegate22 in invocationList)
			{
				if (delegate22.Target == instance)
				{
					OnConfirmPurchaseRequestEvent -= (PlayFabRequestEvent<ConfirmPurchaseRequest>)delegate22;
				}
			}
		}
		if (this.OnConfirmPurchaseResultEvent != null)
		{
			invocationList = this.OnConfirmPurchaseResultEvent.GetInvocationList();
			foreach (Delegate delegate23 in invocationList)
			{
				if (delegate23.Target == instance)
				{
					OnConfirmPurchaseResultEvent -= (PlayFabResultEvent<ConfirmPurchaseResult>)delegate23;
				}
			}
		}
		if (this.OnConsumeItemRequestEvent != null)
		{
			invocationList = this.OnConsumeItemRequestEvent.GetInvocationList();
			foreach (Delegate delegate24 in invocationList)
			{
				if (delegate24.Target == instance)
				{
					OnConsumeItemRequestEvent -= (PlayFabRequestEvent<ConsumeItemRequest>)delegate24;
				}
			}
		}
		if (this.OnConsumeItemResultEvent != null)
		{
			invocationList = this.OnConsumeItemResultEvent.GetInvocationList();
			foreach (Delegate delegate25 in invocationList)
			{
				if (delegate25.Target == instance)
				{
					OnConsumeItemResultEvent -= (PlayFabResultEvent<ConsumeItemResult>)delegate25;
				}
			}
		}
		if (this.OnCreateSharedGroupRequestEvent != null)
		{
			invocationList = this.OnCreateSharedGroupRequestEvent.GetInvocationList();
			foreach (Delegate delegate26 in invocationList)
			{
				if (delegate26.Target == instance)
				{
					OnCreateSharedGroupRequestEvent -= (PlayFabRequestEvent<CreateSharedGroupRequest>)delegate26;
				}
			}
		}
		if (this.OnCreateSharedGroupResultEvent != null)
		{
			invocationList = this.OnCreateSharedGroupResultEvent.GetInvocationList();
			foreach (Delegate delegate27 in invocationList)
			{
				if (delegate27.Target == instance)
				{
					OnCreateSharedGroupResultEvent -= (PlayFabResultEvent<CreateSharedGroupResult>)delegate27;
				}
			}
		}
		if (this.OnExecuteCloudScriptRequestEvent != null)
		{
			invocationList = this.OnExecuteCloudScriptRequestEvent.GetInvocationList();
			foreach (Delegate delegate28 in invocationList)
			{
				if (delegate28.Target == instance)
				{
					OnExecuteCloudScriptRequestEvent -= (PlayFabRequestEvent<ExecuteCloudScriptRequest>)delegate28;
				}
			}
		}
		if (this.OnExecuteCloudScriptResultEvent != null)
		{
			invocationList = this.OnExecuteCloudScriptResultEvent.GetInvocationList();
			foreach (Delegate delegate29 in invocationList)
			{
				if (delegate29.Target == instance)
				{
					OnExecuteCloudScriptResultEvent -= (PlayFabResultEvent<ExecuteCloudScriptResult>)delegate29;
				}
			}
		}
		if (this.OnGetAccountInfoRequestEvent != null)
		{
			invocationList = this.OnGetAccountInfoRequestEvent.GetInvocationList();
			foreach (Delegate delegate30 in invocationList)
			{
				if (delegate30.Target == instance)
				{
					OnGetAccountInfoRequestEvent -= (PlayFabRequestEvent<GetAccountInfoRequest>)delegate30;
				}
			}
		}
		if (this.OnGetAccountInfoResultEvent != null)
		{
			invocationList = this.OnGetAccountInfoResultEvent.GetInvocationList();
			foreach (Delegate delegate31 in invocationList)
			{
				if (delegate31.Target == instance)
				{
					OnGetAccountInfoResultEvent -= (PlayFabResultEvent<GetAccountInfoResult>)delegate31;
				}
			}
		}
		if (this.OnGetAllUsersCharactersRequestEvent != null)
		{
			invocationList = this.OnGetAllUsersCharactersRequestEvent.GetInvocationList();
			foreach (Delegate delegate32 in invocationList)
			{
				if (delegate32.Target == instance)
				{
					OnGetAllUsersCharactersRequestEvent -= (PlayFabRequestEvent<ListUsersCharactersRequest>)delegate32;
				}
			}
		}
		if (this.OnGetAllUsersCharactersResultEvent != null)
		{
			invocationList = this.OnGetAllUsersCharactersResultEvent.GetInvocationList();
			foreach (Delegate delegate33 in invocationList)
			{
				if (delegate33.Target == instance)
				{
					OnGetAllUsersCharactersResultEvent -= (PlayFabResultEvent<ListUsersCharactersResult>)delegate33;
				}
			}
		}
		if (this.OnGetCatalogItemsRequestEvent != null)
		{
			invocationList = this.OnGetCatalogItemsRequestEvent.GetInvocationList();
			foreach (Delegate delegate34 in invocationList)
			{
				if (delegate34.Target == instance)
				{
					OnGetCatalogItemsRequestEvent -= (PlayFabRequestEvent<GetCatalogItemsRequest>)delegate34;
				}
			}
		}
		if (this.OnGetCatalogItemsResultEvent != null)
		{
			invocationList = this.OnGetCatalogItemsResultEvent.GetInvocationList();
			foreach (Delegate delegate35 in invocationList)
			{
				if (delegate35.Target == instance)
				{
					OnGetCatalogItemsResultEvent -= (PlayFabResultEvent<GetCatalogItemsResult>)delegate35;
				}
			}
		}
		if (this.OnGetCharacterDataRequestEvent != null)
		{
			invocationList = this.OnGetCharacterDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate36 in invocationList)
			{
				if (delegate36.Target == instance)
				{
					OnGetCharacterDataRequestEvent -= (PlayFabRequestEvent<GetCharacterDataRequest>)delegate36;
				}
			}
		}
		if (this.OnGetCharacterDataResultEvent != null)
		{
			invocationList = this.OnGetCharacterDataResultEvent.GetInvocationList();
			foreach (Delegate delegate37 in invocationList)
			{
				if (delegate37.Target == instance)
				{
					OnGetCharacterDataResultEvent -= (PlayFabResultEvent<GetCharacterDataResult>)delegate37;
				}
			}
		}
		if (this.OnGetCharacterInventoryRequestEvent != null)
		{
			invocationList = this.OnGetCharacterInventoryRequestEvent.GetInvocationList();
			foreach (Delegate delegate38 in invocationList)
			{
				if (delegate38.Target == instance)
				{
					OnGetCharacterInventoryRequestEvent -= (PlayFabRequestEvent<GetCharacterInventoryRequest>)delegate38;
				}
			}
		}
		if (this.OnGetCharacterInventoryResultEvent != null)
		{
			invocationList = this.OnGetCharacterInventoryResultEvent.GetInvocationList();
			foreach (Delegate delegate39 in invocationList)
			{
				if (delegate39.Target == instance)
				{
					OnGetCharacterInventoryResultEvent -= (PlayFabResultEvent<GetCharacterInventoryResult>)delegate39;
				}
			}
		}
		if (this.OnGetCharacterLeaderboardRequestEvent != null)
		{
			invocationList = this.OnGetCharacterLeaderboardRequestEvent.GetInvocationList();
			foreach (Delegate delegate40 in invocationList)
			{
				if (delegate40.Target == instance)
				{
					OnGetCharacterLeaderboardRequestEvent -= (PlayFabRequestEvent<GetCharacterLeaderboardRequest>)delegate40;
				}
			}
		}
		if (this.OnGetCharacterLeaderboardResultEvent != null)
		{
			invocationList = this.OnGetCharacterLeaderboardResultEvent.GetInvocationList();
			foreach (Delegate delegate41 in invocationList)
			{
				if (delegate41.Target == instance)
				{
					OnGetCharacterLeaderboardResultEvent -= (PlayFabResultEvent<GetCharacterLeaderboardResult>)delegate41;
				}
			}
		}
		if (this.OnGetCharacterReadOnlyDataRequestEvent != null)
		{
			invocationList = this.OnGetCharacterReadOnlyDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate42 in invocationList)
			{
				if (delegate42.Target == instance)
				{
					OnGetCharacterReadOnlyDataRequestEvent -= (PlayFabRequestEvent<GetCharacterDataRequest>)delegate42;
				}
			}
		}
		if (this.OnGetCharacterReadOnlyDataResultEvent != null)
		{
			invocationList = this.OnGetCharacterReadOnlyDataResultEvent.GetInvocationList();
			foreach (Delegate delegate43 in invocationList)
			{
				if (delegate43.Target == instance)
				{
					OnGetCharacterReadOnlyDataResultEvent -= (PlayFabResultEvent<GetCharacterDataResult>)delegate43;
				}
			}
		}
		if (this.OnGetCharacterStatisticsRequestEvent != null)
		{
			invocationList = this.OnGetCharacterStatisticsRequestEvent.GetInvocationList();
			foreach (Delegate delegate44 in invocationList)
			{
				if (delegate44.Target == instance)
				{
					OnGetCharacterStatisticsRequestEvent -= (PlayFabRequestEvent<GetCharacterStatisticsRequest>)delegate44;
				}
			}
		}
		if (this.OnGetCharacterStatisticsResultEvent != null)
		{
			invocationList = this.OnGetCharacterStatisticsResultEvent.GetInvocationList();
			foreach (Delegate delegate45 in invocationList)
			{
				if (delegate45.Target == instance)
				{
					OnGetCharacterStatisticsResultEvent -= (PlayFabResultEvent<GetCharacterStatisticsResult>)delegate45;
				}
			}
		}
		if (this.OnGetContentDownloadUrlRequestEvent != null)
		{
			invocationList = this.OnGetContentDownloadUrlRequestEvent.GetInvocationList();
			foreach (Delegate delegate46 in invocationList)
			{
				if (delegate46.Target == instance)
				{
					OnGetContentDownloadUrlRequestEvent -= (PlayFabRequestEvent<GetContentDownloadUrlRequest>)delegate46;
				}
			}
		}
		if (this.OnGetContentDownloadUrlResultEvent != null)
		{
			invocationList = this.OnGetContentDownloadUrlResultEvent.GetInvocationList();
			foreach (Delegate delegate47 in invocationList)
			{
				if (delegate47.Target == instance)
				{
					OnGetContentDownloadUrlResultEvent -= (PlayFabResultEvent<GetContentDownloadUrlResult>)delegate47;
				}
			}
		}
		if (this.OnGetCurrentGamesRequestEvent != null)
		{
			invocationList = this.OnGetCurrentGamesRequestEvent.GetInvocationList();
			foreach (Delegate delegate48 in invocationList)
			{
				if (delegate48.Target == instance)
				{
					OnGetCurrentGamesRequestEvent -= (PlayFabRequestEvent<CurrentGamesRequest>)delegate48;
				}
			}
		}
		if (this.OnGetCurrentGamesResultEvent != null)
		{
			invocationList = this.OnGetCurrentGamesResultEvent.GetInvocationList();
			foreach (Delegate delegate49 in invocationList)
			{
				if (delegate49.Target == instance)
				{
					OnGetCurrentGamesResultEvent -= (PlayFabResultEvent<CurrentGamesResult>)delegate49;
				}
			}
		}
		if (this.OnGetFriendLeaderboardRequestEvent != null)
		{
			invocationList = this.OnGetFriendLeaderboardRequestEvent.GetInvocationList();
			foreach (Delegate delegate50 in invocationList)
			{
				if (delegate50.Target == instance)
				{
					OnGetFriendLeaderboardRequestEvent -= (PlayFabRequestEvent<GetFriendLeaderboardRequest>)delegate50;
				}
			}
		}
		if (this.OnGetFriendLeaderboardResultEvent != null)
		{
			invocationList = this.OnGetFriendLeaderboardResultEvent.GetInvocationList();
			foreach (Delegate delegate51 in invocationList)
			{
				if (delegate51.Target == instance)
				{
					OnGetFriendLeaderboardResultEvent -= (PlayFabResultEvent<GetLeaderboardResult>)delegate51;
				}
			}
		}
		if (this.OnGetFriendLeaderboardAroundPlayerRequestEvent != null)
		{
			invocationList = this.OnGetFriendLeaderboardAroundPlayerRequestEvent.GetInvocationList();
			foreach (Delegate delegate52 in invocationList)
			{
				if (delegate52.Target == instance)
				{
					OnGetFriendLeaderboardAroundPlayerRequestEvent -= (PlayFabRequestEvent<GetFriendLeaderboardAroundPlayerRequest>)delegate52;
				}
			}
		}
		if (this.OnGetFriendLeaderboardAroundPlayerResultEvent != null)
		{
			invocationList = this.OnGetFriendLeaderboardAroundPlayerResultEvent.GetInvocationList();
			foreach (Delegate delegate53 in invocationList)
			{
				if (delegate53.Target == instance)
				{
					OnGetFriendLeaderboardAroundPlayerResultEvent -= (PlayFabResultEvent<GetFriendLeaderboardAroundPlayerResult>)delegate53;
				}
			}
		}
		if (this.OnGetFriendsListRequestEvent != null)
		{
			invocationList = this.OnGetFriendsListRequestEvent.GetInvocationList();
			foreach (Delegate delegate54 in invocationList)
			{
				if (delegate54.Target == instance)
				{
					OnGetFriendsListRequestEvent -= (PlayFabRequestEvent<GetFriendsListRequest>)delegate54;
				}
			}
		}
		if (this.OnGetFriendsListResultEvent != null)
		{
			invocationList = this.OnGetFriendsListResultEvent.GetInvocationList();
			foreach (Delegate delegate55 in invocationList)
			{
				if (delegate55.Target == instance)
				{
					OnGetFriendsListResultEvent -= (PlayFabResultEvent<GetFriendsListResult>)delegate55;
				}
			}
		}
		if (this.OnGetGameServerRegionsRequestEvent != null)
		{
			invocationList = this.OnGetGameServerRegionsRequestEvent.GetInvocationList();
			foreach (Delegate delegate56 in invocationList)
			{
				if (delegate56.Target == instance)
				{
					OnGetGameServerRegionsRequestEvent -= (PlayFabRequestEvent<GameServerRegionsRequest>)delegate56;
				}
			}
		}
		if (this.OnGetGameServerRegionsResultEvent != null)
		{
			invocationList = this.OnGetGameServerRegionsResultEvent.GetInvocationList();
			foreach (Delegate delegate57 in invocationList)
			{
				if (delegate57.Target == instance)
				{
					OnGetGameServerRegionsResultEvent -= (PlayFabResultEvent<GameServerRegionsResult>)delegate57;
				}
			}
		}
		if (this.OnGetLeaderboardRequestEvent != null)
		{
			invocationList = this.OnGetLeaderboardRequestEvent.GetInvocationList();
			foreach (Delegate delegate58 in invocationList)
			{
				if (delegate58.Target == instance)
				{
					OnGetLeaderboardRequestEvent -= (PlayFabRequestEvent<GetLeaderboardRequest>)delegate58;
				}
			}
		}
		if (this.OnGetLeaderboardResultEvent != null)
		{
			invocationList = this.OnGetLeaderboardResultEvent.GetInvocationList();
			foreach (Delegate delegate59 in invocationList)
			{
				if (delegate59.Target == instance)
				{
					OnGetLeaderboardResultEvent -= (PlayFabResultEvent<GetLeaderboardResult>)delegate59;
				}
			}
		}
		if (this.OnGetLeaderboardAroundCharacterRequestEvent != null)
		{
			invocationList = this.OnGetLeaderboardAroundCharacterRequestEvent.GetInvocationList();
			foreach (Delegate delegate60 in invocationList)
			{
				if (delegate60.Target == instance)
				{
					OnGetLeaderboardAroundCharacterRequestEvent -= (PlayFabRequestEvent<GetLeaderboardAroundCharacterRequest>)delegate60;
				}
			}
		}
		if (this.OnGetLeaderboardAroundCharacterResultEvent != null)
		{
			invocationList = this.OnGetLeaderboardAroundCharacterResultEvent.GetInvocationList();
			foreach (Delegate delegate61 in invocationList)
			{
				if (delegate61.Target == instance)
				{
					OnGetLeaderboardAroundCharacterResultEvent -= (PlayFabResultEvent<GetLeaderboardAroundCharacterResult>)delegate61;
				}
			}
		}
		if (this.OnGetLeaderboardAroundPlayerRequestEvent != null)
		{
			invocationList = this.OnGetLeaderboardAroundPlayerRequestEvent.GetInvocationList();
			foreach (Delegate delegate62 in invocationList)
			{
				if (delegate62.Target == instance)
				{
					OnGetLeaderboardAroundPlayerRequestEvent -= (PlayFabRequestEvent<GetLeaderboardAroundPlayerRequest>)delegate62;
				}
			}
		}
		if (this.OnGetLeaderboardAroundPlayerResultEvent != null)
		{
			invocationList = this.OnGetLeaderboardAroundPlayerResultEvent.GetInvocationList();
			foreach (Delegate delegate63 in invocationList)
			{
				if (delegate63.Target == instance)
				{
					OnGetLeaderboardAroundPlayerResultEvent -= (PlayFabResultEvent<GetLeaderboardAroundPlayerResult>)delegate63;
				}
			}
		}
		if (this.OnGetLeaderboardForUserCharactersRequestEvent != null)
		{
			invocationList = this.OnGetLeaderboardForUserCharactersRequestEvent.GetInvocationList();
			foreach (Delegate delegate64 in invocationList)
			{
				if (delegate64.Target == instance)
				{
					OnGetLeaderboardForUserCharactersRequestEvent -= (PlayFabRequestEvent<GetLeaderboardForUsersCharactersRequest>)delegate64;
				}
			}
		}
		if (this.OnGetLeaderboardForUserCharactersResultEvent != null)
		{
			invocationList = this.OnGetLeaderboardForUserCharactersResultEvent.GetInvocationList();
			foreach (Delegate delegate65 in invocationList)
			{
				if (delegate65.Target == instance)
				{
					OnGetLeaderboardForUserCharactersResultEvent -= (PlayFabResultEvent<GetLeaderboardForUsersCharactersResult>)delegate65;
				}
			}
		}
		if (this.OnGetPaymentTokenRequestEvent != null)
		{
			invocationList = this.OnGetPaymentTokenRequestEvent.GetInvocationList();
			foreach (Delegate delegate66 in invocationList)
			{
				if (delegate66.Target == instance)
				{
					OnGetPaymentTokenRequestEvent -= (PlayFabRequestEvent<GetPaymentTokenRequest>)delegate66;
				}
			}
		}
		if (this.OnGetPaymentTokenResultEvent != null)
		{
			invocationList = this.OnGetPaymentTokenResultEvent.GetInvocationList();
			foreach (Delegate delegate67 in invocationList)
			{
				if (delegate67.Target == instance)
				{
					OnGetPaymentTokenResultEvent -= (PlayFabResultEvent<GetPaymentTokenResult>)delegate67;
				}
			}
		}
		if (this.OnGetPhotonAuthenticationTokenRequestEvent != null)
		{
			invocationList = this.OnGetPhotonAuthenticationTokenRequestEvent.GetInvocationList();
			foreach (Delegate delegate68 in invocationList)
			{
				if (delegate68.Target == instance)
				{
					OnGetPhotonAuthenticationTokenRequestEvent -= (PlayFabRequestEvent<GetPhotonAuthenticationTokenRequest>)delegate68;
				}
			}
		}
		if (this.OnGetPhotonAuthenticationTokenResultEvent != null)
		{
			invocationList = this.OnGetPhotonAuthenticationTokenResultEvent.GetInvocationList();
			foreach (Delegate delegate69 in invocationList)
			{
				if (delegate69.Target == instance)
				{
					OnGetPhotonAuthenticationTokenResultEvent -= (PlayFabResultEvent<GetPhotonAuthenticationTokenResult>)delegate69;
				}
			}
		}
		if (this.OnGetPlayerCombinedInfoRequestEvent != null)
		{
			invocationList = this.OnGetPlayerCombinedInfoRequestEvent.GetInvocationList();
			foreach (Delegate delegate70 in invocationList)
			{
				if (delegate70.Target == instance)
				{
					OnGetPlayerCombinedInfoRequestEvent -= (PlayFabRequestEvent<GetPlayerCombinedInfoRequest>)delegate70;
				}
			}
		}
		if (this.OnGetPlayerCombinedInfoResultEvent != null)
		{
			invocationList = this.OnGetPlayerCombinedInfoResultEvent.GetInvocationList();
			foreach (Delegate delegate71 in invocationList)
			{
				if (delegate71.Target == instance)
				{
					OnGetPlayerCombinedInfoResultEvent -= (PlayFabResultEvent<GetPlayerCombinedInfoResult>)delegate71;
				}
			}
		}
		if (this.OnGetPlayerProfileRequestEvent != null)
		{
			invocationList = this.OnGetPlayerProfileRequestEvent.GetInvocationList();
			foreach (Delegate delegate72 in invocationList)
			{
				if (delegate72.Target == instance)
				{
					OnGetPlayerProfileRequestEvent -= (PlayFabRequestEvent<GetPlayerProfileRequest>)delegate72;
				}
			}
		}
		if (this.OnGetPlayerProfileResultEvent != null)
		{
			invocationList = this.OnGetPlayerProfileResultEvent.GetInvocationList();
			foreach (Delegate delegate73 in invocationList)
			{
				if (delegate73.Target == instance)
				{
					OnGetPlayerProfileResultEvent -= (PlayFabResultEvent<GetPlayerProfileResult>)delegate73;
				}
			}
		}
		if (this.OnGetPlayerSegmentsRequestEvent != null)
		{
			invocationList = this.OnGetPlayerSegmentsRequestEvent.GetInvocationList();
			foreach (Delegate delegate74 in invocationList)
			{
				if (delegate74.Target == instance)
				{
					OnGetPlayerSegmentsRequestEvent -= (PlayFabRequestEvent<GetPlayerSegmentsRequest>)delegate74;
				}
			}
		}
		if (this.OnGetPlayerSegmentsResultEvent != null)
		{
			invocationList = this.OnGetPlayerSegmentsResultEvent.GetInvocationList();
			foreach (Delegate delegate75 in invocationList)
			{
				if (delegate75.Target == instance)
				{
					OnGetPlayerSegmentsResultEvent -= (PlayFabResultEvent<GetPlayerSegmentsResult>)delegate75;
				}
			}
		}
		if (this.OnGetPlayerStatisticsRequestEvent != null)
		{
			invocationList = this.OnGetPlayerStatisticsRequestEvent.GetInvocationList();
			foreach (Delegate delegate76 in invocationList)
			{
				if (delegate76.Target == instance)
				{
					OnGetPlayerStatisticsRequestEvent -= (PlayFabRequestEvent<GetPlayerStatisticsRequest>)delegate76;
				}
			}
		}
		if (this.OnGetPlayerStatisticsResultEvent != null)
		{
			invocationList = this.OnGetPlayerStatisticsResultEvent.GetInvocationList();
			foreach (Delegate delegate77 in invocationList)
			{
				if (delegate77.Target == instance)
				{
					OnGetPlayerStatisticsResultEvent -= (PlayFabResultEvent<GetPlayerStatisticsResult>)delegate77;
				}
			}
		}
		if (this.OnGetPlayerStatisticVersionsRequestEvent != null)
		{
			invocationList = this.OnGetPlayerStatisticVersionsRequestEvent.GetInvocationList();
			foreach (Delegate delegate78 in invocationList)
			{
				if (delegate78.Target == instance)
				{
					OnGetPlayerStatisticVersionsRequestEvent -= (PlayFabRequestEvent<GetPlayerStatisticVersionsRequest>)delegate78;
				}
			}
		}
		if (this.OnGetPlayerStatisticVersionsResultEvent != null)
		{
			invocationList = this.OnGetPlayerStatisticVersionsResultEvent.GetInvocationList();
			foreach (Delegate delegate79 in invocationList)
			{
				if (delegate79.Target == instance)
				{
					OnGetPlayerStatisticVersionsResultEvent -= (PlayFabResultEvent<GetPlayerStatisticVersionsResult>)delegate79;
				}
			}
		}
		if (this.OnGetPlayerTagsRequestEvent != null)
		{
			invocationList = this.OnGetPlayerTagsRequestEvent.GetInvocationList();
			foreach (Delegate delegate80 in invocationList)
			{
				if (delegate80.Target == instance)
				{
					OnGetPlayerTagsRequestEvent -= (PlayFabRequestEvent<GetPlayerTagsRequest>)delegate80;
				}
			}
		}
		if (this.OnGetPlayerTagsResultEvent != null)
		{
			invocationList = this.OnGetPlayerTagsResultEvent.GetInvocationList();
			foreach (Delegate delegate81 in invocationList)
			{
				if (delegate81.Target == instance)
				{
					OnGetPlayerTagsResultEvent -= (PlayFabResultEvent<GetPlayerTagsResult>)delegate81;
				}
			}
		}
		if (this.OnGetPlayerTradesRequestEvent != null)
		{
			invocationList = this.OnGetPlayerTradesRequestEvent.GetInvocationList();
			foreach (Delegate delegate82 in invocationList)
			{
				if (delegate82.Target == instance)
				{
					OnGetPlayerTradesRequestEvent -= (PlayFabRequestEvent<GetPlayerTradesRequest>)delegate82;
				}
			}
		}
		if (this.OnGetPlayerTradesResultEvent != null)
		{
			invocationList = this.OnGetPlayerTradesResultEvent.GetInvocationList();
			foreach (Delegate delegate83 in invocationList)
			{
				if (delegate83.Target == instance)
				{
					OnGetPlayerTradesResultEvent -= (PlayFabResultEvent<GetPlayerTradesResponse>)delegate83;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromFacebookIDsRequestEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromFacebookIDsRequestEvent.GetInvocationList();
			foreach (Delegate delegate84 in invocationList)
			{
				if (delegate84.Target == instance)
				{
					OnGetPlayFabIDsFromFacebookIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromFacebookIDsRequest>)delegate84;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromFacebookIDsResultEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromFacebookIDsResultEvent.GetInvocationList();
			foreach (Delegate delegate85 in invocationList)
			{
				if (delegate85.Target == instance)
				{
					OnGetPlayFabIDsFromFacebookIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromFacebookIDsResult>)delegate85;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromGameCenterIDsRequestEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromGameCenterIDsRequestEvent.GetInvocationList();
			foreach (Delegate delegate86 in invocationList)
			{
				if (delegate86.Target == instance)
				{
					OnGetPlayFabIDsFromGameCenterIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromGameCenterIDsRequest>)delegate86;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromGameCenterIDsResultEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromGameCenterIDsResultEvent.GetInvocationList();
			foreach (Delegate delegate87 in invocationList)
			{
				if (delegate87.Target == instance)
				{
					OnGetPlayFabIDsFromGameCenterIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromGameCenterIDsResult>)delegate87;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromGenericIDsRequestEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromGenericIDsRequestEvent.GetInvocationList();
			foreach (Delegate delegate88 in invocationList)
			{
				if (delegate88.Target == instance)
				{
					OnGetPlayFabIDsFromGenericIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromGenericIDsRequest>)delegate88;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromGenericIDsResultEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromGenericIDsResultEvent.GetInvocationList();
			foreach (Delegate delegate89 in invocationList)
			{
				if (delegate89.Target == instance)
				{
					OnGetPlayFabIDsFromGenericIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromGenericIDsResult>)delegate89;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromGoogleIDsRequestEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromGoogleIDsRequestEvent.GetInvocationList();
			foreach (Delegate delegate90 in invocationList)
			{
				if (delegate90.Target == instance)
				{
					OnGetPlayFabIDsFromGoogleIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromGoogleIDsRequest>)delegate90;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromGoogleIDsResultEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromGoogleIDsResultEvent.GetInvocationList();
			foreach (Delegate delegate91 in invocationList)
			{
				if (delegate91.Target == instance)
				{
					OnGetPlayFabIDsFromGoogleIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromGoogleIDsResult>)delegate91;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromKongregateIDsRequestEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromKongregateIDsRequestEvent.GetInvocationList();
			foreach (Delegate delegate92 in invocationList)
			{
				if (delegate92.Target == instance)
				{
					OnGetPlayFabIDsFromKongregateIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromKongregateIDsRequest>)delegate92;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromKongregateIDsResultEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromKongregateIDsResultEvent.GetInvocationList();
			foreach (Delegate delegate93 in invocationList)
			{
				if (delegate93.Target == instance)
				{
					OnGetPlayFabIDsFromKongregateIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromKongregateIDsResult>)delegate93;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromSteamIDsRequestEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromSteamIDsRequestEvent.GetInvocationList();
			foreach (Delegate delegate94 in invocationList)
			{
				if (delegate94.Target == instance)
				{
					OnGetPlayFabIDsFromSteamIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromSteamIDsRequest>)delegate94;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromSteamIDsResultEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromSteamIDsResultEvent.GetInvocationList();
			foreach (Delegate delegate95 in invocationList)
			{
				if (delegate95.Target == instance)
				{
					OnGetPlayFabIDsFromSteamIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromSteamIDsResult>)delegate95;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromTwitchIDsRequestEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromTwitchIDsRequestEvent.GetInvocationList();
			foreach (Delegate delegate96 in invocationList)
			{
				if (delegate96.Target == instance)
				{
					OnGetPlayFabIDsFromTwitchIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromTwitchIDsRequest>)delegate96;
				}
			}
		}
		if (this.OnGetPlayFabIDsFromTwitchIDsResultEvent != null)
		{
			invocationList = this.OnGetPlayFabIDsFromTwitchIDsResultEvent.GetInvocationList();
			foreach (Delegate delegate97 in invocationList)
			{
				if (delegate97.Target == instance)
				{
					OnGetPlayFabIDsFromTwitchIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromTwitchIDsResult>)delegate97;
				}
			}
		}
		if (this.OnGetPublisherDataRequestEvent != null)
		{
			invocationList = this.OnGetPublisherDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate98 in invocationList)
			{
				if (delegate98.Target == instance)
				{
					OnGetPublisherDataRequestEvent -= (PlayFabRequestEvent<GetPublisherDataRequest>)delegate98;
				}
			}
		}
		if (this.OnGetPublisherDataResultEvent != null)
		{
			invocationList = this.OnGetPublisherDataResultEvent.GetInvocationList();
			foreach (Delegate delegate99 in invocationList)
			{
				if (delegate99.Target == instance)
				{
					OnGetPublisherDataResultEvent -= (PlayFabResultEvent<GetPublisherDataResult>)delegate99;
				}
			}
		}
		if (this.OnGetPurchaseRequestEvent != null)
		{
			invocationList = this.OnGetPurchaseRequestEvent.GetInvocationList();
			foreach (Delegate delegate100 in invocationList)
			{
				if (delegate100.Target == instance)
				{
					OnGetPurchaseRequestEvent -= (PlayFabRequestEvent<GetPurchaseRequest>)delegate100;
				}
			}
		}
		if (this.OnGetPurchaseResultEvent != null)
		{
			invocationList = this.OnGetPurchaseResultEvent.GetInvocationList();
			foreach (Delegate delegate101 in invocationList)
			{
				if (delegate101.Target == instance)
				{
					OnGetPurchaseResultEvent -= (PlayFabResultEvent<GetPurchaseResult>)delegate101;
				}
			}
		}
		if (this.OnGetSharedGroupDataRequestEvent != null)
		{
			invocationList = this.OnGetSharedGroupDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate102 in invocationList)
			{
				if (delegate102.Target == instance)
				{
					OnGetSharedGroupDataRequestEvent -= (PlayFabRequestEvent<GetSharedGroupDataRequest>)delegate102;
				}
			}
		}
		if (this.OnGetSharedGroupDataResultEvent != null)
		{
			invocationList = this.OnGetSharedGroupDataResultEvent.GetInvocationList();
			foreach (Delegate delegate103 in invocationList)
			{
				if (delegate103.Target == instance)
				{
					OnGetSharedGroupDataResultEvent -= (PlayFabResultEvent<GetSharedGroupDataResult>)delegate103;
				}
			}
		}
		if (this.OnGetStoreItemsRequestEvent != null)
		{
			invocationList = this.OnGetStoreItemsRequestEvent.GetInvocationList();
			foreach (Delegate delegate104 in invocationList)
			{
				if (delegate104.Target == instance)
				{
					OnGetStoreItemsRequestEvent -= (PlayFabRequestEvent<GetStoreItemsRequest>)delegate104;
				}
			}
		}
		if (this.OnGetStoreItemsResultEvent != null)
		{
			invocationList = this.OnGetStoreItemsResultEvent.GetInvocationList();
			foreach (Delegate delegate105 in invocationList)
			{
				if (delegate105.Target == instance)
				{
					OnGetStoreItemsResultEvent -= (PlayFabResultEvent<GetStoreItemsResult>)delegate105;
				}
			}
		}
		if (this.OnGetTimeRequestEvent != null)
		{
			invocationList = this.OnGetTimeRequestEvent.GetInvocationList();
			foreach (Delegate delegate106 in invocationList)
			{
				if (delegate106.Target == instance)
				{
					OnGetTimeRequestEvent -= (PlayFabRequestEvent<GetTimeRequest>)delegate106;
				}
			}
		}
		if (this.OnGetTimeResultEvent != null)
		{
			invocationList = this.OnGetTimeResultEvent.GetInvocationList();
			foreach (Delegate delegate107 in invocationList)
			{
				if (delegate107.Target == instance)
				{
					OnGetTimeResultEvent -= (PlayFabResultEvent<GetTimeResult>)delegate107;
				}
			}
		}
		if (this.OnGetTitleDataRequestEvent != null)
		{
			invocationList = this.OnGetTitleDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate108 in invocationList)
			{
				if (delegate108.Target == instance)
				{
					OnGetTitleDataRequestEvent -= (PlayFabRequestEvent<GetTitleDataRequest>)delegate108;
				}
			}
		}
		if (this.OnGetTitleDataResultEvent != null)
		{
			invocationList = this.OnGetTitleDataResultEvent.GetInvocationList();
			foreach (Delegate delegate109 in invocationList)
			{
				if (delegate109.Target == instance)
				{
					OnGetTitleDataResultEvent -= (PlayFabResultEvent<GetTitleDataResult>)delegate109;
				}
			}
		}
		if (this.OnGetTitleNewsRequestEvent != null)
		{
			invocationList = this.OnGetTitleNewsRequestEvent.GetInvocationList();
			foreach (Delegate delegate110 in invocationList)
			{
				if (delegate110.Target == instance)
				{
					OnGetTitleNewsRequestEvent -= (PlayFabRequestEvent<GetTitleNewsRequest>)delegate110;
				}
			}
		}
		if (this.OnGetTitleNewsResultEvent != null)
		{
			invocationList = this.OnGetTitleNewsResultEvent.GetInvocationList();
			foreach (Delegate delegate111 in invocationList)
			{
				if (delegate111.Target == instance)
				{
					OnGetTitleNewsResultEvent -= (PlayFabResultEvent<GetTitleNewsResult>)delegate111;
				}
			}
		}
		if (this.OnGetTitlePublicKeyRequestEvent != null)
		{
			invocationList = this.OnGetTitlePublicKeyRequestEvent.GetInvocationList();
			foreach (Delegate delegate112 in invocationList)
			{
				if (delegate112.Target == instance)
				{
					OnGetTitlePublicKeyRequestEvent -= (PlayFabRequestEvent<GetTitlePublicKeyRequest>)delegate112;
				}
			}
		}
		if (this.OnGetTitlePublicKeyResultEvent != null)
		{
			invocationList = this.OnGetTitlePublicKeyResultEvent.GetInvocationList();
			foreach (Delegate delegate113 in invocationList)
			{
				if (delegate113.Target == instance)
				{
					OnGetTitlePublicKeyResultEvent -= (PlayFabResultEvent<GetTitlePublicKeyResult>)delegate113;
				}
			}
		}
		if (this.OnGetTradeStatusRequestEvent != null)
		{
			invocationList = this.OnGetTradeStatusRequestEvent.GetInvocationList();
			foreach (Delegate delegate114 in invocationList)
			{
				if (delegate114.Target == instance)
				{
					OnGetTradeStatusRequestEvent -= (PlayFabRequestEvent<GetTradeStatusRequest>)delegate114;
				}
			}
		}
		if (this.OnGetTradeStatusResultEvent != null)
		{
			invocationList = this.OnGetTradeStatusResultEvent.GetInvocationList();
			foreach (Delegate delegate115 in invocationList)
			{
				if (delegate115.Target == instance)
				{
					OnGetTradeStatusResultEvent -= (PlayFabResultEvent<GetTradeStatusResponse>)delegate115;
				}
			}
		}
		if (this.OnGetUserDataRequestEvent != null)
		{
			invocationList = this.OnGetUserDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate116 in invocationList)
			{
				if (delegate116.Target == instance)
				{
					OnGetUserDataRequestEvent -= (PlayFabRequestEvent<GetUserDataRequest>)delegate116;
				}
			}
		}
		if (this.OnGetUserDataResultEvent != null)
		{
			invocationList = this.OnGetUserDataResultEvent.GetInvocationList();
			foreach (Delegate delegate117 in invocationList)
			{
				if (delegate117.Target == instance)
				{
					OnGetUserDataResultEvent -= (PlayFabResultEvent<GetUserDataResult>)delegate117;
				}
			}
		}
		if (this.OnGetUserInventoryRequestEvent != null)
		{
			invocationList = this.OnGetUserInventoryRequestEvent.GetInvocationList();
			foreach (Delegate delegate118 in invocationList)
			{
				if (delegate118.Target == instance)
				{
					OnGetUserInventoryRequestEvent -= (PlayFabRequestEvent<GetUserInventoryRequest>)delegate118;
				}
			}
		}
		if (this.OnGetUserInventoryResultEvent != null)
		{
			invocationList = this.OnGetUserInventoryResultEvent.GetInvocationList();
			foreach (Delegate delegate119 in invocationList)
			{
				if (delegate119.Target == instance)
				{
					OnGetUserInventoryResultEvent -= (PlayFabResultEvent<GetUserInventoryResult>)delegate119;
				}
			}
		}
		if (this.OnGetUserPublisherDataRequestEvent != null)
		{
			invocationList = this.OnGetUserPublisherDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate120 in invocationList)
			{
				if (delegate120.Target == instance)
				{
					OnGetUserPublisherDataRequestEvent -= (PlayFabRequestEvent<GetUserDataRequest>)delegate120;
				}
			}
		}
		if (this.OnGetUserPublisherDataResultEvent != null)
		{
			invocationList = this.OnGetUserPublisherDataResultEvent.GetInvocationList();
			foreach (Delegate delegate121 in invocationList)
			{
				if (delegate121.Target == instance)
				{
					OnGetUserPublisherDataResultEvent -= (PlayFabResultEvent<GetUserDataResult>)delegate121;
				}
			}
		}
		if (this.OnGetUserPublisherReadOnlyDataRequestEvent != null)
		{
			invocationList = this.OnGetUserPublisherReadOnlyDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate122 in invocationList)
			{
				if (delegate122.Target == instance)
				{
					OnGetUserPublisherReadOnlyDataRequestEvent -= (PlayFabRequestEvent<GetUserDataRequest>)delegate122;
				}
			}
		}
		if (this.OnGetUserPublisherReadOnlyDataResultEvent != null)
		{
			invocationList = this.OnGetUserPublisherReadOnlyDataResultEvent.GetInvocationList();
			foreach (Delegate delegate123 in invocationList)
			{
				if (delegate123.Target == instance)
				{
					OnGetUserPublisherReadOnlyDataResultEvent -= (PlayFabResultEvent<GetUserDataResult>)delegate123;
				}
			}
		}
		if (this.OnGetUserReadOnlyDataRequestEvent != null)
		{
			invocationList = this.OnGetUserReadOnlyDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate124 in invocationList)
			{
				if (delegate124.Target == instance)
				{
					OnGetUserReadOnlyDataRequestEvent -= (PlayFabRequestEvent<GetUserDataRequest>)delegate124;
				}
			}
		}
		if (this.OnGetUserReadOnlyDataResultEvent != null)
		{
			invocationList = this.OnGetUserReadOnlyDataResultEvent.GetInvocationList();
			foreach (Delegate delegate125 in invocationList)
			{
				if (delegate125.Target == instance)
				{
					OnGetUserReadOnlyDataResultEvent -= (PlayFabResultEvent<GetUserDataResult>)delegate125;
				}
			}
		}
		if (this.OnGetWindowsHelloChallengeRequestEvent != null)
		{
			invocationList = this.OnGetWindowsHelloChallengeRequestEvent.GetInvocationList();
			foreach (Delegate delegate126 in invocationList)
			{
				if (delegate126.Target == instance)
				{
					OnGetWindowsHelloChallengeRequestEvent -= (PlayFabRequestEvent<GetWindowsHelloChallengeRequest>)delegate126;
				}
			}
		}
		if (this.OnGetWindowsHelloChallengeResultEvent != null)
		{
			invocationList = this.OnGetWindowsHelloChallengeResultEvent.GetInvocationList();
			foreach (Delegate delegate127 in invocationList)
			{
				if (delegate127.Target == instance)
				{
					OnGetWindowsHelloChallengeResultEvent -= (PlayFabResultEvent<GetWindowsHelloChallengeResponse>)delegate127;
				}
			}
		}
		if (this.OnGrantCharacterToUserRequestEvent != null)
		{
			invocationList = this.OnGrantCharacterToUserRequestEvent.GetInvocationList();
			foreach (Delegate delegate128 in invocationList)
			{
				if (delegate128.Target == instance)
				{
					OnGrantCharacterToUserRequestEvent -= (PlayFabRequestEvent<GrantCharacterToUserRequest>)delegate128;
				}
			}
		}
		if (this.OnGrantCharacterToUserResultEvent != null)
		{
			invocationList = this.OnGrantCharacterToUserResultEvent.GetInvocationList();
			foreach (Delegate delegate129 in invocationList)
			{
				if (delegate129.Target == instance)
				{
					OnGrantCharacterToUserResultEvent -= (PlayFabResultEvent<GrantCharacterToUserResult>)delegate129;
				}
			}
		}
		if (this.OnLinkAndroidDeviceIDRequestEvent != null)
		{
			invocationList = this.OnLinkAndroidDeviceIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate130 in invocationList)
			{
				if (delegate130.Target == instance)
				{
					OnLinkAndroidDeviceIDRequestEvent -= (PlayFabRequestEvent<LinkAndroidDeviceIDRequest>)delegate130;
				}
			}
		}
		if (this.OnLinkAndroidDeviceIDResultEvent != null)
		{
			invocationList = this.OnLinkAndroidDeviceIDResultEvent.GetInvocationList();
			foreach (Delegate delegate131 in invocationList)
			{
				if (delegate131.Target == instance)
				{
					OnLinkAndroidDeviceIDResultEvent -= (PlayFabResultEvent<LinkAndroidDeviceIDResult>)delegate131;
				}
			}
		}
		if (this.OnLinkCustomIDRequestEvent != null)
		{
			invocationList = this.OnLinkCustomIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate132 in invocationList)
			{
				if (delegate132.Target == instance)
				{
					OnLinkCustomIDRequestEvent -= (PlayFabRequestEvent<LinkCustomIDRequest>)delegate132;
				}
			}
		}
		if (this.OnLinkCustomIDResultEvent != null)
		{
			invocationList = this.OnLinkCustomIDResultEvent.GetInvocationList();
			foreach (Delegate delegate133 in invocationList)
			{
				if (delegate133.Target == instance)
				{
					OnLinkCustomIDResultEvent -= (PlayFabResultEvent<LinkCustomIDResult>)delegate133;
				}
			}
		}
		if (this.OnLinkFacebookAccountRequestEvent != null)
		{
			invocationList = this.OnLinkFacebookAccountRequestEvent.GetInvocationList();
			foreach (Delegate delegate134 in invocationList)
			{
				if (delegate134.Target == instance)
				{
					OnLinkFacebookAccountRequestEvent -= (PlayFabRequestEvent<LinkFacebookAccountRequest>)delegate134;
				}
			}
		}
		if (this.OnLinkFacebookAccountResultEvent != null)
		{
			invocationList = this.OnLinkFacebookAccountResultEvent.GetInvocationList();
			foreach (Delegate delegate135 in invocationList)
			{
				if (delegate135.Target == instance)
				{
					OnLinkFacebookAccountResultEvent -= (PlayFabResultEvent<LinkFacebookAccountResult>)delegate135;
				}
			}
		}
		if (this.OnLinkGameCenterAccountRequestEvent != null)
		{
			invocationList = this.OnLinkGameCenterAccountRequestEvent.GetInvocationList();
			foreach (Delegate delegate136 in invocationList)
			{
				if (delegate136.Target == instance)
				{
					OnLinkGameCenterAccountRequestEvent -= (PlayFabRequestEvent<LinkGameCenterAccountRequest>)delegate136;
				}
			}
		}
		if (this.OnLinkGameCenterAccountResultEvent != null)
		{
			invocationList = this.OnLinkGameCenterAccountResultEvent.GetInvocationList();
			foreach (Delegate delegate137 in invocationList)
			{
				if (delegate137.Target == instance)
				{
					OnLinkGameCenterAccountResultEvent -= (PlayFabResultEvent<LinkGameCenterAccountResult>)delegate137;
				}
			}
		}
		if (this.OnLinkGoogleAccountRequestEvent != null)
		{
			invocationList = this.OnLinkGoogleAccountRequestEvent.GetInvocationList();
			foreach (Delegate delegate138 in invocationList)
			{
				if (delegate138.Target == instance)
				{
					OnLinkGoogleAccountRequestEvent -= (PlayFabRequestEvent<LinkGoogleAccountRequest>)delegate138;
				}
			}
		}
		if (this.OnLinkGoogleAccountResultEvent != null)
		{
			invocationList = this.OnLinkGoogleAccountResultEvent.GetInvocationList();
			foreach (Delegate delegate139 in invocationList)
			{
				if (delegate139.Target == instance)
				{
					OnLinkGoogleAccountResultEvent -= (PlayFabResultEvent<LinkGoogleAccountResult>)delegate139;
				}
			}
		}
		if (this.OnLinkIOSDeviceIDRequestEvent != null)
		{
			invocationList = this.OnLinkIOSDeviceIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate140 in invocationList)
			{
				if (delegate140.Target == instance)
				{
					OnLinkIOSDeviceIDRequestEvent -= (PlayFabRequestEvent<LinkIOSDeviceIDRequest>)delegate140;
				}
			}
		}
		if (this.OnLinkIOSDeviceIDResultEvent != null)
		{
			invocationList = this.OnLinkIOSDeviceIDResultEvent.GetInvocationList();
			foreach (Delegate delegate141 in invocationList)
			{
				if (delegate141.Target == instance)
				{
					OnLinkIOSDeviceIDResultEvent -= (PlayFabResultEvent<LinkIOSDeviceIDResult>)delegate141;
				}
			}
		}
		if (this.OnLinkKongregateRequestEvent != null)
		{
			invocationList = this.OnLinkKongregateRequestEvent.GetInvocationList();
			foreach (Delegate delegate142 in invocationList)
			{
				if (delegate142.Target == instance)
				{
					OnLinkKongregateRequestEvent -= (PlayFabRequestEvent<LinkKongregateAccountRequest>)delegate142;
				}
			}
		}
		if (this.OnLinkKongregateResultEvent != null)
		{
			invocationList = this.OnLinkKongregateResultEvent.GetInvocationList();
			foreach (Delegate delegate143 in invocationList)
			{
				if (delegate143.Target == instance)
				{
					OnLinkKongregateResultEvent -= (PlayFabResultEvent<LinkKongregateAccountResult>)delegate143;
				}
			}
		}
		if (this.OnLinkSteamAccountRequestEvent != null)
		{
			invocationList = this.OnLinkSteamAccountRequestEvent.GetInvocationList();
			foreach (Delegate delegate144 in invocationList)
			{
				if (delegate144.Target == instance)
				{
					OnLinkSteamAccountRequestEvent -= (PlayFabRequestEvent<LinkSteamAccountRequest>)delegate144;
				}
			}
		}
		if (this.OnLinkSteamAccountResultEvent != null)
		{
			invocationList = this.OnLinkSteamAccountResultEvent.GetInvocationList();
			foreach (Delegate delegate145 in invocationList)
			{
				if (delegate145.Target == instance)
				{
					OnLinkSteamAccountResultEvent -= (PlayFabResultEvent<LinkSteamAccountResult>)delegate145;
				}
			}
		}
		if (this.OnLinkTwitchRequestEvent != null)
		{
			invocationList = this.OnLinkTwitchRequestEvent.GetInvocationList();
			foreach (Delegate delegate146 in invocationList)
			{
				if (delegate146.Target == instance)
				{
					OnLinkTwitchRequestEvent -= (PlayFabRequestEvent<LinkTwitchAccountRequest>)delegate146;
				}
			}
		}
		if (this.OnLinkTwitchResultEvent != null)
		{
			invocationList = this.OnLinkTwitchResultEvent.GetInvocationList();
			foreach (Delegate delegate147 in invocationList)
			{
				if (delegate147.Target == instance)
				{
					OnLinkTwitchResultEvent -= (PlayFabResultEvent<LinkTwitchAccountResult>)delegate147;
				}
			}
		}
		if (this.OnLinkWindowsHelloRequestEvent != null)
		{
			invocationList = this.OnLinkWindowsHelloRequestEvent.GetInvocationList();
			foreach (Delegate delegate148 in invocationList)
			{
				if (delegate148.Target == instance)
				{
					OnLinkWindowsHelloRequestEvent -= (PlayFabRequestEvent<LinkWindowsHelloAccountRequest>)delegate148;
				}
			}
		}
		if (this.OnLinkWindowsHelloResultEvent != null)
		{
			invocationList = this.OnLinkWindowsHelloResultEvent.GetInvocationList();
			foreach (Delegate delegate149 in invocationList)
			{
				if (delegate149.Target == instance)
				{
					OnLinkWindowsHelloResultEvent -= (PlayFabResultEvent<LinkWindowsHelloAccountResponse>)delegate149;
				}
			}
		}
		if (this.OnLoginWithAndroidDeviceIDRequestEvent != null)
		{
			invocationList = this.OnLoginWithAndroidDeviceIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate150 in invocationList)
			{
				if (delegate150.Target == instance)
				{
					OnLoginWithAndroidDeviceIDRequestEvent -= (PlayFabRequestEvent<LoginWithAndroidDeviceIDRequest>)delegate150;
				}
			}
		}
		if (this.OnLoginWithCustomIDRequestEvent != null)
		{
			invocationList = this.OnLoginWithCustomIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate151 in invocationList)
			{
				if (delegate151.Target == instance)
				{
					OnLoginWithCustomIDRequestEvent -= (PlayFabRequestEvent<LoginWithCustomIDRequest>)delegate151;
				}
			}
		}
		if (this.OnLoginWithEmailAddressRequestEvent != null)
		{
			invocationList = this.OnLoginWithEmailAddressRequestEvent.GetInvocationList();
			foreach (Delegate delegate152 in invocationList)
			{
				if (delegate152.Target == instance)
				{
					OnLoginWithEmailAddressRequestEvent -= (PlayFabRequestEvent<LoginWithEmailAddressRequest>)delegate152;
				}
			}
		}
		if (this.OnLoginWithFacebookRequestEvent != null)
		{
			invocationList = this.OnLoginWithFacebookRequestEvent.GetInvocationList();
			foreach (Delegate delegate153 in invocationList)
			{
				if (delegate153.Target == instance)
				{
					OnLoginWithFacebookRequestEvent -= (PlayFabRequestEvent<LoginWithFacebookRequest>)delegate153;
				}
			}
		}
		if (this.OnLoginWithGameCenterRequestEvent != null)
		{
			invocationList = this.OnLoginWithGameCenterRequestEvent.GetInvocationList();
			foreach (Delegate delegate154 in invocationList)
			{
				if (delegate154.Target == instance)
				{
					OnLoginWithGameCenterRequestEvent -= (PlayFabRequestEvent<LoginWithGameCenterRequest>)delegate154;
				}
			}
		}
		if (this.OnLoginWithGoogleAccountRequestEvent != null)
		{
			invocationList = this.OnLoginWithGoogleAccountRequestEvent.GetInvocationList();
			foreach (Delegate delegate155 in invocationList)
			{
				if (delegate155.Target == instance)
				{
					OnLoginWithGoogleAccountRequestEvent -= (PlayFabRequestEvent<LoginWithGoogleAccountRequest>)delegate155;
				}
			}
		}
		if (this.OnLoginWithIOSDeviceIDRequestEvent != null)
		{
			invocationList = this.OnLoginWithIOSDeviceIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate156 in invocationList)
			{
				if (delegate156.Target == instance)
				{
					OnLoginWithIOSDeviceIDRequestEvent -= (PlayFabRequestEvent<LoginWithIOSDeviceIDRequest>)delegate156;
				}
			}
		}
		if (this.OnLoginWithKongregateRequestEvent != null)
		{
			invocationList = this.OnLoginWithKongregateRequestEvent.GetInvocationList();
			foreach (Delegate delegate157 in invocationList)
			{
				if (delegate157.Target == instance)
				{
					OnLoginWithKongregateRequestEvent -= (PlayFabRequestEvent<LoginWithKongregateRequest>)delegate157;
				}
			}
		}
		if (this.OnLoginWithPlayFabRequestEvent != null)
		{
			invocationList = this.OnLoginWithPlayFabRequestEvent.GetInvocationList();
			foreach (Delegate delegate158 in invocationList)
			{
				if (delegate158.Target == instance)
				{
					OnLoginWithPlayFabRequestEvent -= (PlayFabRequestEvent<LoginWithPlayFabRequest>)delegate158;
				}
			}
		}
		if (this.OnLoginWithSteamRequestEvent != null)
		{
			invocationList = this.OnLoginWithSteamRequestEvent.GetInvocationList();
			foreach (Delegate delegate159 in invocationList)
			{
				if (delegate159.Target == instance)
				{
					OnLoginWithSteamRequestEvent -= (PlayFabRequestEvent<LoginWithSteamRequest>)delegate159;
				}
			}
		}
		if (this.OnLoginWithTwitchRequestEvent != null)
		{
			invocationList = this.OnLoginWithTwitchRequestEvent.GetInvocationList();
			foreach (Delegate delegate160 in invocationList)
			{
				if (delegate160.Target == instance)
				{
					OnLoginWithTwitchRequestEvent -= (PlayFabRequestEvent<LoginWithTwitchRequest>)delegate160;
				}
			}
		}
		if (this.OnLoginWithWindowsHelloRequestEvent != null)
		{
			invocationList = this.OnLoginWithWindowsHelloRequestEvent.GetInvocationList();
			foreach (Delegate delegate161 in invocationList)
			{
				if (delegate161.Target == instance)
				{
					OnLoginWithWindowsHelloRequestEvent -= (PlayFabRequestEvent<LoginWithWindowsHelloRequest>)delegate161;
				}
			}
		}
		if (this.OnMatchmakeRequestEvent != null)
		{
			invocationList = this.OnMatchmakeRequestEvent.GetInvocationList();
			foreach (Delegate delegate162 in invocationList)
			{
				if (delegate162.Target == instance)
				{
					OnMatchmakeRequestEvent -= (PlayFabRequestEvent<MatchmakeRequest>)delegate162;
				}
			}
		}
		if (this.OnMatchmakeResultEvent != null)
		{
			invocationList = this.OnMatchmakeResultEvent.GetInvocationList();
			foreach (Delegate delegate163 in invocationList)
			{
				if (delegate163.Target == instance)
				{
					OnMatchmakeResultEvent -= (PlayFabResultEvent<MatchmakeResult>)delegate163;
				}
			}
		}
		if (this.OnOpenTradeRequestEvent != null)
		{
			invocationList = this.OnOpenTradeRequestEvent.GetInvocationList();
			foreach (Delegate delegate164 in invocationList)
			{
				if (delegate164.Target == instance)
				{
					OnOpenTradeRequestEvent -= (PlayFabRequestEvent<OpenTradeRequest>)delegate164;
				}
			}
		}
		if (this.OnOpenTradeResultEvent != null)
		{
			invocationList = this.OnOpenTradeResultEvent.GetInvocationList();
			foreach (Delegate delegate165 in invocationList)
			{
				if (delegate165.Target == instance)
				{
					OnOpenTradeResultEvent -= (PlayFabResultEvent<OpenTradeResponse>)delegate165;
				}
			}
		}
		if (this.OnPayForPurchaseRequestEvent != null)
		{
			invocationList = this.OnPayForPurchaseRequestEvent.GetInvocationList();
			foreach (Delegate delegate166 in invocationList)
			{
				if (delegate166.Target == instance)
				{
					OnPayForPurchaseRequestEvent -= (PlayFabRequestEvent<PayForPurchaseRequest>)delegate166;
				}
			}
		}
		if (this.OnPayForPurchaseResultEvent != null)
		{
			invocationList = this.OnPayForPurchaseResultEvent.GetInvocationList();
			foreach (Delegate delegate167 in invocationList)
			{
				if (delegate167.Target == instance)
				{
					OnPayForPurchaseResultEvent -= (PlayFabResultEvent<PayForPurchaseResult>)delegate167;
				}
			}
		}
		if (this.OnPurchaseItemRequestEvent != null)
		{
			invocationList = this.OnPurchaseItemRequestEvent.GetInvocationList();
			foreach (Delegate delegate168 in invocationList)
			{
				if (delegate168.Target == instance)
				{
					OnPurchaseItemRequestEvent -= (PlayFabRequestEvent<PurchaseItemRequest>)delegate168;
				}
			}
		}
		if (this.OnPurchaseItemResultEvent != null)
		{
			invocationList = this.OnPurchaseItemResultEvent.GetInvocationList();
			foreach (Delegate delegate169 in invocationList)
			{
				if (delegate169.Target == instance)
				{
					OnPurchaseItemResultEvent -= (PlayFabResultEvent<PurchaseItemResult>)delegate169;
				}
			}
		}
		if (this.OnRedeemCouponRequestEvent != null)
		{
			invocationList = this.OnRedeemCouponRequestEvent.GetInvocationList();
			foreach (Delegate delegate170 in invocationList)
			{
				if (delegate170.Target == instance)
				{
					OnRedeemCouponRequestEvent -= (PlayFabRequestEvent<RedeemCouponRequest>)delegate170;
				}
			}
		}
		if (this.OnRedeemCouponResultEvent != null)
		{
			invocationList = this.OnRedeemCouponResultEvent.GetInvocationList();
			foreach (Delegate delegate171 in invocationList)
			{
				if (delegate171.Target == instance)
				{
					OnRedeemCouponResultEvent -= (PlayFabResultEvent<RedeemCouponResult>)delegate171;
				}
			}
		}
		if (this.OnRegisterForIOSPushNotificationRequestEvent != null)
		{
			invocationList = this.OnRegisterForIOSPushNotificationRequestEvent.GetInvocationList();
			foreach (Delegate delegate172 in invocationList)
			{
				if (delegate172.Target == instance)
				{
					OnRegisterForIOSPushNotificationRequestEvent -= (PlayFabRequestEvent<RegisterForIOSPushNotificationRequest>)delegate172;
				}
			}
		}
		if (this.OnRegisterForIOSPushNotificationResultEvent != null)
		{
			invocationList = this.OnRegisterForIOSPushNotificationResultEvent.GetInvocationList();
			foreach (Delegate delegate173 in invocationList)
			{
				if (delegate173.Target == instance)
				{
					OnRegisterForIOSPushNotificationResultEvent -= (PlayFabResultEvent<RegisterForIOSPushNotificationResult>)delegate173;
				}
			}
		}
		if (this.OnRegisterPlayFabUserRequestEvent != null)
		{
			invocationList = this.OnRegisterPlayFabUserRequestEvent.GetInvocationList();
			foreach (Delegate delegate174 in invocationList)
			{
				if (delegate174.Target == instance)
				{
					OnRegisterPlayFabUserRequestEvent -= (PlayFabRequestEvent<RegisterPlayFabUserRequest>)delegate174;
				}
			}
		}
		if (this.OnRegisterPlayFabUserResultEvent != null)
		{
			invocationList = this.OnRegisterPlayFabUserResultEvent.GetInvocationList();
			foreach (Delegate delegate175 in invocationList)
			{
				if (delegate175.Target == instance)
				{
					OnRegisterPlayFabUserResultEvent -= (PlayFabResultEvent<RegisterPlayFabUserResult>)delegate175;
				}
			}
		}
		if (this.OnRegisterWithWindowsHelloRequestEvent != null)
		{
			invocationList = this.OnRegisterWithWindowsHelloRequestEvent.GetInvocationList();
			foreach (Delegate delegate176 in invocationList)
			{
				if (delegate176.Target == instance)
				{
					OnRegisterWithWindowsHelloRequestEvent -= (PlayFabRequestEvent<RegisterWithWindowsHelloRequest>)delegate176;
				}
			}
		}
		if (this.OnRemoveContactEmailRequestEvent != null)
		{
			invocationList = this.OnRemoveContactEmailRequestEvent.GetInvocationList();
			foreach (Delegate delegate177 in invocationList)
			{
				if (delegate177.Target == instance)
				{
					OnRemoveContactEmailRequestEvent -= (PlayFabRequestEvent<RemoveContactEmailRequest>)delegate177;
				}
			}
		}
		if (this.OnRemoveContactEmailResultEvent != null)
		{
			invocationList = this.OnRemoveContactEmailResultEvent.GetInvocationList();
			foreach (Delegate delegate178 in invocationList)
			{
				if (delegate178.Target == instance)
				{
					OnRemoveContactEmailResultEvent -= (PlayFabResultEvent<RemoveContactEmailResult>)delegate178;
				}
			}
		}
		if (this.OnRemoveFriendRequestEvent != null)
		{
			invocationList = this.OnRemoveFriendRequestEvent.GetInvocationList();
			foreach (Delegate delegate179 in invocationList)
			{
				if (delegate179.Target == instance)
				{
					OnRemoveFriendRequestEvent -= (PlayFabRequestEvent<RemoveFriendRequest>)delegate179;
				}
			}
		}
		if (this.OnRemoveFriendResultEvent != null)
		{
			invocationList = this.OnRemoveFriendResultEvent.GetInvocationList();
			foreach (Delegate delegate180 in invocationList)
			{
				if (delegate180.Target == instance)
				{
					OnRemoveFriendResultEvent -= (PlayFabResultEvent<RemoveFriendResult>)delegate180;
				}
			}
		}
		if (this.OnRemoveGenericIDRequestEvent != null)
		{
			invocationList = this.OnRemoveGenericIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate181 in invocationList)
			{
				if (delegate181.Target == instance)
				{
					OnRemoveGenericIDRequestEvent -= (PlayFabRequestEvent<RemoveGenericIDRequest>)delegate181;
				}
			}
		}
		if (this.OnRemoveGenericIDResultEvent != null)
		{
			invocationList = this.OnRemoveGenericIDResultEvent.GetInvocationList();
			foreach (Delegate delegate182 in invocationList)
			{
				if (delegate182.Target == instance)
				{
					OnRemoveGenericIDResultEvent -= (PlayFabResultEvent<RemoveGenericIDResult>)delegate182;
				}
			}
		}
		if (this.OnRemoveSharedGroupMembersRequestEvent != null)
		{
			invocationList = this.OnRemoveSharedGroupMembersRequestEvent.GetInvocationList();
			foreach (Delegate delegate183 in invocationList)
			{
				if (delegate183.Target == instance)
				{
					OnRemoveSharedGroupMembersRequestEvent -= (PlayFabRequestEvent<RemoveSharedGroupMembersRequest>)delegate183;
				}
			}
		}
		if (this.OnRemoveSharedGroupMembersResultEvent != null)
		{
			invocationList = this.OnRemoveSharedGroupMembersResultEvent.GetInvocationList();
			foreach (Delegate delegate184 in invocationList)
			{
				if (delegate184.Target == instance)
				{
					OnRemoveSharedGroupMembersResultEvent -= (PlayFabResultEvent<RemoveSharedGroupMembersResult>)delegate184;
				}
			}
		}
		if (this.OnReportDeviceInfoRequestEvent != null)
		{
			invocationList = this.OnReportDeviceInfoRequestEvent.GetInvocationList();
			foreach (Delegate delegate185 in invocationList)
			{
				if (delegate185.Target == instance)
				{
					OnReportDeviceInfoRequestEvent -= (PlayFabRequestEvent<DeviceInfoRequest>)delegate185;
				}
			}
		}
		if (this.OnReportDeviceInfoResultEvent != null)
		{
			invocationList = this.OnReportDeviceInfoResultEvent.GetInvocationList();
			foreach (Delegate delegate186 in invocationList)
			{
				if (delegate186.Target == instance)
				{
					OnReportDeviceInfoResultEvent -= (PlayFabResultEvent<EmptyResult>)delegate186;
				}
			}
		}
		if (this.OnReportPlayerRequestEvent != null)
		{
			invocationList = this.OnReportPlayerRequestEvent.GetInvocationList();
			foreach (Delegate delegate187 in invocationList)
			{
				if (delegate187.Target == instance)
				{
					OnReportPlayerRequestEvent -= (PlayFabRequestEvent<ReportPlayerClientRequest>)delegate187;
				}
			}
		}
		if (this.OnReportPlayerResultEvent != null)
		{
			invocationList = this.OnReportPlayerResultEvent.GetInvocationList();
			foreach (Delegate delegate188 in invocationList)
			{
				if (delegate188.Target == instance)
				{
					OnReportPlayerResultEvent -= (PlayFabResultEvent<ReportPlayerClientResult>)delegate188;
				}
			}
		}
		if (this.OnRestoreIOSPurchasesRequestEvent != null)
		{
			invocationList = this.OnRestoreIOSPurchasesRequestEvent.GetInvocationList();
			foreach (Delegate delegate189 in invocationList)
			{
				if (delegate189.Target == instance)
				{
					OnRestoreIOSPurchasesRequestEvent -= (PlayFabRequestEvent<RestoreIOSPurchasesRequest>)delegate189;
				}
			}
		}
		if (this.OnRestoreIOSPurchasesResultEvent != null)
		{
			invocationList = this.OnRestoreIOSPurchasesResultEvent.GetInvocationList();
			foreach (Delegate delegate190 in invocationList)
			{
				if (delegate190.Target == instance)
				{
					OnRestoreIOSPurchasesResultEvent -= (PlayFabResultEvent<RestoreIOSPurchasesResult>)delegate190;
				}
			}
		}
		if (this.OnSendAccountRecoveryEmailRequestEvent != null)
		{
			invocationList = this.OnSendAccountRecoveryEmailRequestEvent.GetInvocationList();
			foreach (Delegate delegate191 in invocationList)
			{
				if (delegate191.Target == instance)
				{
					OnSendAccountRecoveryEmailRequestEvent -= (PlayFabRequestEvent<SendAccountRecoveryEmailRequest>)delegate191;
				}
			}
		}
		if (this.OnSendAccountRecoveryEmailResultEvent != null)
		{
			invocationList = this.OnSendAccountRecoveryEmailResultEvent.GetInvocationList();
			foreach (Delegate delegate192 in invocationList)
			{
				if (delegate192.Target == instance)
				{
					OnSendAccountRecoveryEmailResultEvent -= (PlayFabResultEvent<SendAccountRecoveryEmailResult>)delegate192;
				}
			}
		}
		if (this.OnSetFriendTagsRequestEvent != null)
		{
			invocationList = this.OnSetFriendTagsRequestEvent.GetInvocationList();
			foreach (Delegate delegate193 in invocationList)
			{
				if (delegate193.Target == instance)
				{
					OnSetFriendTagsRequestEvent -= (PlayFabRequestEvent<SetFriendTagsRequest>)delegate193;
				}
			}
		}
		if (this.OnSetFriendTagsResultEvent != null)
		{
			invocationList = this.OnSetFriendTagsResultEvent.GetInvocationList();
			foreach (Delegate delegate194 in invocationList)
			{
				if (delegate194.Target == instance)
				{
					OnSetFriendTagsResultEvent -= (PlayFabResultEvent<SetFriendTagsResult>)delegate194;
				}
			}
		}
		if (this.OnSetPlayerSecretRequestEvent != null)
		{
			invocationList = this.OnSetPlayerSecretRequestEvent.GetInvocationList();
			foreach (Delegate delegate195 in invocationList)
			{
				if (delegate195.Target == instance)
				{
					OnSetPlayerSecretRequestEvent -= (PlayFabRequestEvent<SetPlayerSecretRequest>)delegate195;
				}
			}
		}
		if (this.OnSetPlayerSecretResultEvent != null)
		{
			invocationList = this.OnSetPlayerSecretResultEvent.GetInvocationList();
			foreach (Delegate delegate196 in invocationList)
			{
				if (delegate196.Target == instance)
				{
					OnSetPlayerSecretResultEvent -= (PlayFabResultEvent<SetPlayerSecretResult>)delegate196;
				}
			}
		}
		if (this.OnStartGameRequestEvent != null)
		{
			invocationList = this.OnStartGameRequestEvent.GetInvocationList();
			foreach (Delegate delegate197 in invocationList)
			{
				if (delegate197.Target == instance)
				{
					OnStartGameRequestEvent -= (PlayFabRequestEvent<StartGameRequest>)delegate197;
				}
			}
		}
		if (this.OnStartGameResultEvent != null)
		{
			invocationList = this.OnStartGameResultEvent.GetInvocationList();
			foreach (Delegate delegate198 in invocationList)
			{
				if (delegate198.Target == instance)
				{
					OnStartGameResultEvent -= (PlayFabResultEvent<StartGameResult>)delegate198;
				}
			}
		}
		if (this.OnStartPurchaseRequestEvent != null)
		{
			invocationList = this.OnStartPurchaseRequestEvent.GetInvocationList();
			foreach (Delegate delegate199 in invocationList)
			{
				if (delegate199.Target == instance)
				{
					OnStartPurchaseRequestEvent -= (PlayFabRequestEvent<StartPurchaseRequest>)delegate199;
				}
			}
		}
		if (this.OnStartPurchaseResultEvent != null)
		{
			invocationList = this.OnStartPurchaseResultEvent.GetInvocationList();
			foreach (Delegate delegate200 in invocationList)
			{
				if (delegate200.Target == instance)
				{
					OnStartPurchaseResultEvent -= (PlayFabResultEvent<StartPurchaseResult>)delegate200;
				}
			}
		}
		if (this.OnSubtractUserVirtualCurrencyRequestEvent != null)
		{
			invocationList = this.OnSubtractUserVirtualCurrencyRequestEvent.GetInvocationList();
			foreach (Delegate delegate201 in invocationList)
			{
				if (delegate201.Target == instance)
				{
					OnSubtractUserVirtualCurrencyRequestEvent -= (PlayFabRequestEvent<SubtractUserVirtualCurrencyRequest>)delegate201;
				}
			}
		}
		if (this.OnSubtractUserVirtualCurrencyResultEvent != null)
		{
			invocationList = this.OnSubtractUserVirtualCurrencyResultEvent.GetInvocationList();
			foreach (Delegate delegate202 in invocationList)
			{
				if (delegate202.Target == instance)
				{
					OnSubtractUserVirtualCurrencyResultEvent -= (PlayFabResultEvent<ModifyUserVirtualCurrencyResult>)delegate202;
				}
			}
		}
		if (this.OnUnlinkAndroidDeviceIDRequestEvent != null)
		{
			invocationList = this.OnUnlinkAndroidDeviceIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate203 in invocationList)
			{
				if (delegate203.Target == instance)
				{
					OnUnlinkAndroidDeviceIDRequestEvent -= (PlayFabRequestEvent<UnlinkAndroidDeviceIDRequest>)delegate203;
				}
			}
		}
		if (this.OnUnlinkAndroidDeviceIDResultEvent != null)
		{
			invocationList = this.OnUnlinkAndroidDeviceIDResultEvent.GetInvocationList();
			foreach (Delegate delegate204 in invocationList)
			{
				if (delegate204.Target == instance)
				{
					OnUnlinkAndroidDeviceIDResultEvent -= (PlayFabResultEvent<UnlinkAndroidDeviceIDResult>)delegate204;
				}
			}
		}
		if (this.OnUnlinkCustomIDRequestEvent != null)
		{
			invocationList = this.OnUnlinkCustomIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate205 in invocationList)
			{
				if (delegate205.Target == instance)
				{
					OnUnlinkCustomIDRequestEvent -= (PlayFabRequestEvent<UnlinkCustomIDRequest>)delegate205;
				}
			}
		}
		if (this.OnUnlinkCustomIDResultEvent != null)
		{
			invocationList = this.OnUnlinkCustomIDResultEvent.GetInvocationList();
			foreach (Delegate delegate206 in invocationList)
			{
				if (delegate206.Target == instance)
				{
					OnUnlinkCustomIDResultEvent -= (PlayFabResultEvent<UnlinkCustomIDResult>)delegate206;
				}
			}
		}
		if (this.OnUnlinkFacebookAccountRequestEvent != null)
		{
			invocationList = this.OnUnlinkFacebookAccountRequestEvent.GetInvocationList();
			foreach (Delegate delegate207 in invocationList)
			{
				if (delegate207.Target == instance)
				{
					OnUnlinkFacebookAccountRequestEvent -= (PlayFabRequestEvent<UnlinkFacebookAccountRequest>)delegate207;
				}
			}
		}
		if (this.OnUnlinkFacebookAccountResultEvent != null)
		{
			invocationList = this.OnUnlinkFacebookAccountResultEvent.GetInvocationList();
			foreach (Delegate delegate208 in invocationList)
			{
				if (delegate208.Target == instance)
				{
					OnUnlinkFacebookAccountResultEvent -= (PlayFabResultEvent<UnlinkFacebookAccountResult>)delegate208;
				}
			}
		}
		if (this.OnUnlinkGameCenterAccountRequestEvent != null)
		{
			invocationList = this.OnUnlinkGameCenterAccountRequestEvent.GetInvocationList();
			foreach (Delegate delegate209 in invocationList)
			{
				if (delegate209.Target == instance)
				{
					OnUnlinkGameCenterAccountRequestEvent -= (PlayFabRequestEvent<UnlinkGameCenterAccountRequest>)delegate209;
				}
			}
		}
		if (this.OnUnlinkGameCenterAccountResultEvent != null)
		{
			invocationList = this.OnUnlinkGameCenterAccountResultEvent.GetInvocationList();
			foreach (Delegate delegate210 in invocationList)
			{
				if (delegate210.Target == instance)
				{
					OnUnlinkGameCenterAccountResultEvent -= (PlayFabResultEvent<UnlinkGameCenterAccountResult>)delegate210;
				}
			}
		}
		if (this.OnUnlinkGoogleAccountRequestEvent != null)
		{
			invocationList = this.OnUnlinkGoogleAccountRequestEvent.GetInvocationList();
			foreach (Delegate delegate211 in invocationList)
			{
				if (delegate211.Target == instance)
				{
					OnUnlinkGoogleAccountRequestEvent -= (PlayFabRequestEvent<UnlinkGoogleAccountRequest>)delegate211;
				}
			}
		}
		if (this.OnUnlinkGoogleAccountResultEvent != null)
		{
			invocationList = this.OnUnlinkGoogleAccountResultEvent.GetInvocationList();
			foreach (Delegate delegate212 in invocationList)
			{
				if (delegate212.Target == instance)
				{
					OnUnlinkGoogleAccountResultEvent -= (PlayFabResultEvent<UnlinkGoogleAccountResult>)delegate212;
				}
			}
		}
		if (this.OnUnlinkIOSDeviceIDRequestEvent != null)
		{
			invocationList = this.OnUnlinkIOSDeviceIDRequestEvent.GetInvocationList();
			foreach (Delegate delegate213 in invocationList)
			{
				if (delegate213.Target == instance)
				{
					OnUnlinkIOSDeviceIDRequestEvent -= (PlayFabRequestEvent<UnlinkIOSDeviceIDRequest>)delegate213;
				}
			}
		}
		if (this.OnUnlinkIOSDeviceIDResultEvent != null)
		{
			invocationList = this.OnUnlinkIOSDeviceIDResultEvent.GetInvocationList();
			foreach (Delegate delegate214 in invocationList)
			{
				if (delegate214.Target == instance)
				{
					OnUnlinkIOSDeviceIDResultEvent -= (PlayFabResultEvent<UnlinkIOSDeviceIDResult>)delegate214;
				}
			}
		}
		if (this.OnUnlinkKongregateRequestEvent != null)
		{
			invocationList = this.OnUnlinkKongregateRequestEvent.GetInvocationList();
			foreach (Delegate delegate215 in invocationList)
			{
				if (delegate215.Target == instance)
				{
					OnUnlinkKongregateRequestEvent -= (PlayFabRequestEvent<UnlinkKongregateAccountRequest>)delegate215;
				}
			}
		}
		if (this.OnUnlinkKongregateResultEvent != null)
		{
			invocationList = this.OnUnlinkKongregateResultEvent.GetInvocationList();
			foreach (Delegate delegate216 in invocationList)
			{
				if (delegate216.Target == instance)
				{
					OnUnlinkKongregateResultEvent -= (PlayFabResultEvent<UnlinkKongregateAccountResult>)delegate216;
				}
			}
		}
		if (this.OnUnlinkSteamAccountRequestEvent != null)
		{
			invocationList = this.OnUnlinkSteamAccountRequestEvent.GetInvocationList();
			foreach (Delegate delegate217 in invocationList)
			{
				if (delegate217.Target == instance)
				{
					OnUnlinkSteamAccountRequestEvent -= (PlayFabRequestEvent<UnlinkSteamAccountRequest>)delegate217;
				}
			}
		}
		if (this.OnUnlinkSteamAccountResultEvent != null)
		{
			invocationList = this.OnUnlinkSteamAccountResultEvent.GetInvocationList();
			foreach (Delegate delegate218 in invocationList)
			{
				if (delegate218.Target == instance)
				{
					OnUnlinkSteamAccountResultEvent -= (PlayFabResultEvent<UnlinkSteamAccountResult>)delegate218;
				}
			}
		}
		if (this.OnUnlinkTwitchRequestEvent != null)
		{
			invocationList = this.OnUnlinkTwitchRequestEvent.GetInvocationList();
			foreach (Delegate delegate219 in invocationList)
			{
				if (delegate219.Target == instance)
				{
					OnUnlinkTwitchRequestEvent -= (PlayFabRequestEvent<UnlinkTwitchAccountRequest>)delegate219;
				}
			}
		}
		if (this.OnUnlinkTwitchResultEvent != null)
		{
			invocationList = this.OnUnlinkTwitchResultEvent.GetInvocationList();
			foreach (Delegate delegate220 in invocationList)
			{
				if (delegate220.Target == instance)
				{
					OnUnlinkTwitchResultEvent -= (PlayFabResultEvent<UnlinkTwitchAccountResult>)delegate220;
				}
			}
		}
		if (this.OnUnlinkWindowsHelloRequestEvent != null)
		{
			invocationList = this.OnUnlinkWindowsHelloRequestEvent.GetInvocationList();
			foreach (Delegate delegate221 in invocationList)
			{
				if (delegate221.Target == instance)
				{
					OnUnlinkWindowsHelloRequestEvent -= (PlayFabRequestEvent<UnlinkWindowsHelloAccountRequest>)delegate221;
				}
			}
		}
		if (this.OnUnlinkWindowsHelloResultEvent != null)
		{
			invocationList = this.OnUnlinkWindowsHelloResultEvent.GetInvocationList();
			foreach (Delegate delegate222 in invocationList)
			{
				if (delegate222.Target == instance)
				{
					OnUnlinkWindowsHelloResultEvent -= (PlayFabResultEvent<UnlinkWindowsHelloAccountResponse>)delegate222;
				}
			}
		}
		if (this.OnUnlockContainerInstanceRequestEvent != null)
		{
			invocationList = this.OnUnlockContainerInstanceRequestEvent.GetInvocationList();
			foreach (Delegate delegate223 in invocationList)
			{
				if (delegate223.Target == instance)
				{
					OnUnlockContainerInstanceRequestEvent -= (PlayFabRequestEvent<UnlockContainerInstanceRequest>)delegate223;
				}
			}
		}
		if (this.OnUnlockContainerInstanceResultEvent != null)
		{
			invocationList = this.OnUnlockContainerInstanceResultEvent.GetInvocationList();
			foreach (Delegate delegate224 in invocationList)
			{
				if (delegate224.Target == instance)
				{
					OnUnlockContainerInstanceResultEvent -= (PlayFabResultEvent<UnlockContainerItemResult>)delegate224;
				}
			}
		}
		if (this.OnUnlockContainerItemRequestEvent != null)
		{
			invocationList = this.OnUnlockContainerItemRequestEvent.GetInvocationList();
			foreach (Delegate delegate225 in invocationList)
			{
				if (delegate225.Target == instance)
				{
					OnUnlockContainerItemRequestEvent -= (PlayFabRequestEvent<UnlockContainerItemRequest>)delegate225;
				}
			}
		}
		if (this.OnUnlockContainerItemResultEvent != null)
		{
			invocationList = this.OnUnlockContainerItemResultEvent.GetInvocationList();
			foreach (Delegate delegate226 in invocationList)
			{
				if (delegate226.Target == instance)
				{
					OnUnlockContainerItemResultEvent -= (PlayFabResultEvent<UnlockContainerItemResult>)delegate226;
				}
			}
		}
		if (this.OnUpdateAvatarUrlRequestEvent != null)
		{
			invocationList = this.OnUpdateAvatarUrlRequestEvent.GetInvocationList();
			foreach (Delegate delegate227 in invocationList)
			{
				if (delegate227.Target == instance)
				{
					OnUpdateAvatarUrlRequestEvent -= (PlayFabRequestEvent<UpdateAvatarUrlRequest>)delegate227;
				}
			}
		}
		if (this.OnUpdateAvatarUrlResultEvent != null)
		{
			invocationList = this.OnUpdateAvatarUrlResultEvent.GetInvocationList();
			foreach (Delegate delegate228 in invocationList)
			{
				if (delegate228.Target == instance)
				{
					OnUpdateAvatarUrlResultEvent -= (PlayFabResultEvent<EmptyResult>)delegate228;
				}
			}
		}
		if (this.OnUpdateCharacterDataRequestEvent != null)
		{
			invocationList = this.OnUpdateCharacterDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate229 in invocationList)
			{
				if (delegate229.Target == instance)
				{
					OnUpdateCharacterDataRequestEvent -= (PlayFabRequestEvent<UpdateCharacterDataRequest>)delegate229;
				}
			}
		}
		if (this.OnUpdateCharacterDataResultEvent != null)
		{
			invocationList = this.OnUpdateCharacterDataResultEvent.GetInvocationList();
			foreach (Delegate delegate230 in invocationList)
			{
				if (delegate230.Target == instance)
				{
					OnUpdateCharacterDataResultEvent -= (PlayFabResultEvent<UpdateCharacterDataResult>)delegate230;
				}
			}
		}
		if (this.OnUpdateCharacterStatisticsRequestEvent != null)
		{
			invocationList = this.OnUpdateCharacterStatisticsRequestEvent.GetInvocationList();
			foreach (Delegate delegate231 in invocationList)
			{
				if (delegate231.Target == instance)
				{
					OnUpdateCharacterStatisticsRequestEvent -= (PlayFabRequestEvent<UpdateCharacterStatisticsRequest>)delegate231;
				}
			}
		}
		if (this.OnUpdateCharacterStatisticsResultEvent != null)
		{
			invocationList = this.OnUpdateCharacterStatisticsResultEvent.GetInvocationList();
			foreach (Delegate delegate232 in invocationList)
			{
				if (delegate232.Target == instance)
				{
					OnUpdateCharacterStatisticsResultEvent -= (PlayFabResultEvent<UpdateCharacterStatisticsResult>)delegate232;
				}
			}
		}
		if (this.OnUpdatePlayerStatisticsRequestEvent != null)
		{
			invocationList = this.OnUpdatePlayerStatisticsRequestEvent.GetInvocationList();
			foreach (Delegate delegate233 in invocationList)
			{
				if (delegate233.Target == instance)
				{
					OnUpdatePlayerStatisticsRequestEvent -= (PlayFabRequestEvent<UpdatePlayerStatisticsRequest>)delegate233;
				}
			}
		}
		if (this.OnUpdatePlayerStatisticsResultEvent != null)
		{
			invocationList = this.OnUpdatePlayerStatisticsResultEvent.GetInvocationList();
			foreach (Delegate delegate234 in invocationList)
			{
				if (delegate234.Target == instance)
				{
					OnUpdatePlayerStatisticsResultEvent -= (PlayFabResultEvent<UpdatePlayerStatisticsResult>)delegate234;
				}
			}
		}
		if (this.OnUpdateSharedGroupDataRequestEvent != null)
		{
			invocationList = this.OnUpdateSharedGroupDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate235 in invocationList)
			{
				if (delegate235.Target == instance)
				{
					OnUpdateSharedGroupDataRequestEvent -= (PlayFabRequestEvent<UpdateSharedGroupDataRequest>)delegate235;
				}
			}
		}
		if (this.OnUpdateSharedGroupDataResultEvent != null)
		{
			invocationList = this.OnUpdateSharedGroupDataResultEvent.GetInvocationList();
			foreach (Delegate delegate236 in invocationList)
			{
				if (delegate236.Target == instance)
				{
					OnUpdateSharedGroupDataResultEvent -= (PlayFabResultEvent<UpdateSharedGroupDataResult>)delegate236;
				}
			}
		}
		if (this.OnUpdateUserDataRequestEvent != null)
		{
			invocationList = this.OnUpdateUserDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate237 in invocationList)
			{
				if (delegate237.Target == instance)
				{
					OnUpdateUserDataRequestEvent -= (PlayFabRequestEvent<UpdateUserDataRequest>)delegate237;
				}
			}
		}
		if (this.OnUpdateUserDataResultEvent != null)
		{
			invocationList = this.OnUpdateUserDataResultEvent.GetInvocationList();
			foreach (Delegate delegate238 in invocationList)
			{
				if (delegate238.Target == instance)
				{
					OnUpdateUserDataResultEvent -= (PlayFabResultEvent<UpdateUserDataResult>)delegate238;
				}
			}
		}
		if (this.OnUpdateUserPublisherDataRequestEvent != null)
		{
			invocationList = this.OnUpdateUserPublisherDataRequestEvent.GetInvocationList();
			foreach (Delegate delegate239 in invocationList)
			{
				if (delegate239.Target == instance)
				{
					OnUpdateUserPublisherDataRequestEvent -= (PlayFabRequestEvent<UpdateUserDataRequest>)delegate239;
				}
			}
		}
		if (this.OnUpdateUserPublisherDataResultEvent != null)
		{
			invocationList = this.OnUpdateUserPublisherDataResultEvent.GetInvocationList();
			foreach (Delegate delegate240 in invocationList)
			{
				if (delegate240.Target == instance)
				{
					OnUpdateUserPublisherDataResultEvent -= (PlayFabResultEvent<UpdateUserDataResult>)delegate240;
				}
			}
		}
		if (this.OnUpdateUserTitleDisplayNameRequestEvent != null)
		{
			invocationList = this.OnUpdateUserTitleDisplayNameRequestEvent.GetInvocationList();
			foreach (Delegate delegate241 in invocationList)
			{
				if (delegate241.Target == instance)
				{
					OnUpdateUserTitleDisplayNameRequestEvent -= (PlayFabRequestEvent<UpdateUserTitleDisplayNameRequest>)delegate241;
				}
			}
		}
		if (this.OnUpdateUserTitleDisplayNameResultEvent != null)
		{
			invocationList = this.OnUpdateUserTitleDisplayNameResultEvent.GetInvocationList();
			foreach (Delegate delegate242 in invocationList)
			{
				if (delegate242.Target == instance)
				{
					OnUpdateUserTitleDisplayNameResultEvent -= (PlayFabResultEvent<UpdateUserTitleDisplayNameResult>)delegate242;
				}
			}
		}
		if (this.OnValidateAmazonIAPReceiptRequestEvent != null)
		{
			invocationList = this.OnValidateAmazonIAPReceiptRequestEvent.GetInvocationList();
			foreach (Delegate delegate243 in invocationList)
			{
				if (delegate243.Target == instance)
				{
					OnValidateAmazonIAPReceiptRequestEvent -= (PlayFabRequestEvent<ValidateAmazonReceiptRequest>)delegate243;
				}
			}
		}
		if (this.OnValidateAmazonIAPReceiptResultEvent != null)
		{
			invocationList = this.OnValidateAmazonIAPReceiptResultEvent.GetInvocationList();
			foreach (Delegate delegate244 in invocationList)
			{
				if (delegate244.Target == instance)
				{
					OnValidateAmazonIAPReceiptResultEvent -= (PlayFabResultEvent<ValidateAmazonReceiptResult>)delegate244;
				}
			}
		}
		if (this.OnValidateGooglePlayPurchaseRequestEvent != null)
		{
			invocationList = this.OnValidateGooglePlayPurchaseRequestEvent.GetInvocationList();
			foreach (Delegate delegate245 in invocationList)
			{
				if (delegate245.Target == instance)
				{
					OnValidateGooglePlayPurchaseRequestEvent -= (PlayFabRequestEvent<ValidateGooglePlayPurchaseRequest>)delegate245;
				}
			}
		}
		if (this.OnValidateGooglePlayPurchaseResultEvent != null)
		{
			invocationList = this.OnValidateGooglePlayPurchaseResultEvent.GetInvocationList();
			foreach (Delegate delegate246 in invocationList)
			{
				if (delegate246.Target == instance)
				{
					OnValidateGooglePlayPurchaseResultEvent -= (PlayFabResultEvent<ValidateGooglePlayPurchaseResult>)delegate246;
				}
			}
		}
		if (this.OnValidateIOSReceiptRequestEvent != null)
		{
			invocationList = this.OnValidateIOSReceiptRequestEvent.GetInvocationList();
			foreach (Delegate delegate247 in invocationList)
			{
				if (delegate247.Target == instance)
				{
					OnValidateIOSReceiptRequestEvent -= (PlayFabRequestEvent<ValidateIOSReceiptRequest>)delegate247;
				}
			}
		}
		if (this.OnValidateIOSReceiptResultEvent != null)
		{
			invocationList = this.OnValidateIOSReceiptResultEvent.GetInvocationList();
			foreach (Delegate delegate248 in invocationList)
			{
				if (delegate248.Target == instance)
				{
					OnValidateIOSReceiptResultEvent -= (PlayFabResultEvent<ValidateIOSReceiptResult>)delegate248;
				}
			}
		}
		if (this.OnValidateWindowsStoreReceiptRequestEvent != null)
		{
			invocationList = this.OnValidateWindowsStoreReceiptRequestEvent.GetInvocationList();
			foreach (Delegate delegate249 in invocationList)
			{
				if (delegate249.Target == instance)
				{
					OnValidateWindowsStoreReceiptRequestEvent -= (PlayFabRequestEvent<ValidateWindowsReceiptRequest>)delegate249;
				}
			}
		}
		if (this.OnValidateWindowsStoreReceiptResultEvent != null)
		{
			invocationList = this.OnValidateWindowsStoreReceiptResultEvent.GetInvocationList();
			foreach (Delegate delegate250 in invocationList)
			{
				if (delegate250.Target == instance)
				{
					OnValidateWindowsStoreReceiptResultEvent -= (PlayFabResultEvent<ValidateWindowsReceiptResult>)delegate250;
				}
			}
		}
		if (this.OnWriteCharacterEventRequestEvent != null)
		{
			invocationList = this.OnWriteCharacterEventRequestEvent.GetInvocationList();
			foreach (Delegate delegate251 in invocationList)
			{
				if (delegate251.Target == instance)
				{
					OnWriteCharacterEventRequestEvent -= (PlayFabRequestEvent<WriteClientCharacterEventRequest>)delegate251;
				}
			}
		}
		if (this.OnWriteCharacterEventResultEvent != null)
		{
			invocationList = this.OnWriteCharacterEventResultEvent.GetInvocationList();
			foreach (Delegate delegate252 in invocationList)
			{
				if (delegate252.Target == instance)
				{
					OnWriteCharacterEventResultEvent -= (PlayFabResultEvent<WriteEventResponse>)delegate252;
				}
			}
		}
		if (this.OnWritePlayerEventRequestEvent != null)
		{
			invocationList = this.OnWritePlayerEventRequestEvent.GetInvocationList();
			foreach (Delegate delegate253 in invocationList)
			{
				if (delegate253.Target == instance)
				{
					OnWritePlayerEventRequestEvent -= (PlayFabRequestEvent<WriteClientPlayerEventRequest>)delegate253;
				}
			}
		}
		if (this.OnWritePlayerEventResultEvent != null)
		{
			invocationList = this.OnWritePlayerEventResultEvent.GetInvocationList();
			foreach (Delegate delegate254 in invocationList)
			{
				if (delegate254.Target == instance)
				{
					OnWritePlayerEventResultEvent -= (PlayFabResultEvent<WriteEventResponse>)delegate254;
				}
			}
		}
		if (this.OnWriteTitleEventRequestEvent != null)
		{
			invocationList = this.OnWriteTitleEventRequestEvent.GetInvocationList();
			foreach (Delegate delegate255 in invocationList)
			{
				if (delegate255.Target == instance)
				{
					OnWriteTitleEventRequestEvent -= (PlayFabRequestEvent<WriteTitleEventRequest>)delegate255;
				}
			}
		}
		if (this.OnWriteTitleEventResultEvent == null)
		{
			return;
		}
		invocationList = this.OnWriteTitleEventResultEvent.GetInvocationList();
		foreach (Delegate delegate256 in invocationList)
		{
			if (delegate256.Target == instance)
			{
				OnWriteTitleEventResultEvent -= (PlayFabResultEvent<WriteEventResponse>)delegate256;
			}
		}
	}

	private void OnProcessingErrorEvent(PlayFabRequestCommon request, PlayFabError error)
	{
		if (_instance.OnGlobalErrorEvent != null)
		{
			_instance.OnGlobalErrorEvent(request, error);
		}
	}

	private void OnProcessingEvent(ApiProcessingEventArgs e)
	{
		if (e.EventType == ApiProcessingEventType.Pre)
		{
			Type type = e.Request.GetType();
			if (type == typeof(AcceptTradeRequest) && _instance.OnAcceptTradeRequestEvent != null)
			{
				_instance.OnAcceptTradeRequestEvent((AcceptTradeRequest)e.Request);
			}
			else if (type == typeof(AddFriendRequest) && _instance.OnAddFriendRequestEvent != null)
			{
				_instance.OnAddFriendRequestEvent((AddFriendRequest)e.Request);
			}
			else if (type == typeof(AddGenericIDRequest) && _instance.OnAddGenericIDRequestEvent != null)
			{
				_instance.OnAddGenericIDRequestEvent((AddGenericIDRequest)e.Request);
			}
			else if (type == typeof(AddOrUpdateContactEmailRequest) && _instance.OnAddOrUpdateContactEmailRequestEvent != null)
			{
				_instance.OnAddOrUpdateContactEmailRequestEvent((AddOrUpdateContactEmailRequest)e.Request);
			}
			else if (type == typeof(AddSharedGroupMembersRequest) && _instance.OnAddSharedGroupMembersRequestEvent != null)
			{
				_instance.OnAddSharedGroupMembersRequestEvent((AddSharedGroupMembersRequest)e.Request);
			}
			else if (type == typeof(AddUsernamePasswordRequest) && _instance.OnAddUsernamePasswordRequestEvent != null)
			{
				_instance.OnAddUsernamePasswordRequestEvent((AddUsernamePasswordRequest)e.Request);
			}
			else if (type == typeof(AddUserVirtualCurrencyRequest) && _instance.OnAddUserVirtualCurrencyRequestEvent != null)
			{
				_instance.OnAddUserVirtualCurrencyRequestEvent((AddUserVirtualCurrencyRequest)e.Request);
			}
			else if (type == typeof(AndroidDevicePushNotificationRegistrationRequest) && _instance.OnAndroidDevicePushNotificationRegistrationRequestEvent != null)
			{
				_instance.OnAndroidDevicePushNotificationRegistrationRequestEvent((AndroidDevicePushNotificationRegistrationRequest)e.Request);
			}
			else if (type == typeof(AttributeInstallRequest) && _instance.OnAttributeInstallRequestEvent != null)
			{
				_instance.OnAttributeInstallRequestEvent((AttributeInstallRequest)e.Request);
			}
			else if (type == typeof(CancelTradeRequest) && _instance.OnCancelTradeRequestEvent != null)
			{
				_instance.OnCancelTradeRequestEvent((CancelTradeRequest)e.Request);
			}
			else if (type == typeof(ConfirmPurchaseRequest) && _instance.OnConfirmPurchaseRequestEvent != null)
			{
				_instance.OnConfirmPurchaseRequestEvent((ConfirmPurchaseRequest)e.Request);
			}
			else if (type == typeof(ConsumeItemRequest) && _instance.OnConsumeItemRequestEvent != null)
			{
				_instance.OnConsumeItemRequestEvent((ConsumeItemRequest)e.Request);
			}
			else if (type == typeof(CreateSharedGroupRequest) && _instance.OnCreateSharedGroupRequestEvent != null)
			{
				_instance.OnCreateSharedGroupRequestEvent((CreateSharedGroupRequest)e.Request);
			}
			else if (type == typeof(ExecuteCloudScriptRequest) && _instance.OnExecuteCloudScriptRequestEvent != null)
			{
				_instance.OnExecuteCloudScriptRequestEvent((ExecuteCloudScriptRequest)e.Request);
			}
			else if (type == typeof(GetAccountInfoRequest) && _instance.OnGetAccountInfoRequestEvent != null)
			{
				_instance.OnGetAccountInfoRequestEvent((GetAccountInfoRequest)e.Request);
			}
			else if (type == typeof(ListUsersCharactersRequest) && _instance.OnGetAllUsersCharactersRequestEvent != null)
			{
				_instance.OnGetAllUsersCharactersRequestEvent((ListUsersCharactersRequest)e.Request);
			}
			else if (type == typeof(GetCatalogItemsRequest) && _instance.OnGetCatalogItemsRequestEvent != null)
			{
				_instance.OnGetCatalogItemsRequestEvent((GetCatalogItemsRequest)e.Request);
			}
			else if (type == typeof(GetCharacterDataRequest) && _instance.OnGetCharacterDataRequestEvent != null)
			{
				_instance.OnGetCharacterDataRequestEvent((GetCharacterDataRequest)e.Request);
			}
			else if (type == typeof(GetCharacterInventoryRequest) && _instance.OnGetCharacterInventoryRequestEvent != null)
			{
				_instance.OnGetCharacterInventoryRequestEvent((GetCharacterInventoryRequest)e.Request);
			}
			else if (type == typeof(GetCharacterLeaderboardRequest) && _instance.OnGetCharacterLeaderboardRequestEvent != null)
			{
				_instance.OnGetCharacterLeaderboardRequestEvent((GetCharacterLeaderboardRequest)e.Request);
			}
			else if (type == typeof(GetCharacterDataRequest) && _instance.OnGetCharacterReadOnlyDataRequestEvent != null)
			{
				_instance.OnGetCharacterReadOnlyDataRequestEvent((GetCharacterDataRequest)e.Request);
			}
			else if (type == typeof(GetCharacterStatisticsRequest) && _instance.OnGetCharacterStatisticsRequestEvent != null)
			{
				_instance.OnGetCharacterStatisticsRequestEvent((GetCharacterStatisticsRequest)e.Request);
			}
			else if (type == typeof(GetContentDownloadUrlRequest) && _instance.OnGetContentDownloadUrlRequestEvent != null)
			{
				_instance.OnGetContentDownloadUrlRequestEvent((GetContentDownloadUrlRequest)e.Request);
			}
			else if (type == typeof(CurrentGamesRequest) && _instance.OnGetCurrentGamesRequestEvent != null)
			{
				_instance.OnGetCurrentGamesRequestEvent((CurrentGamesRequest)e.Request);
			}
			else if (type == typeof(GetFriendLeaderboardRequest) && _instance.OnGetFriendLeaderboardRequestEvent != null)
			{
				_instance.OnGetFriendLeaderboardRequestEvent((GetFriendLeaderboardRequest)e.Request);
			}
			else if (type == typeof(GetFriendLeaderboardAroundPlayerRequest) && _instance.OnGetFriendLeaderboardAroundPlayerRequestEvent != null)
			{
				_instance.OnGetFriendLeaderboardAroundPlayerRequestEvent((GetFriendLeaderboardAroundPlayerRequest)e.Request);
			}
			else if (type == typeof(GetFriendsListRequest) && _instance.OnGetFriendsListRequestEvent != null)
			{
				_instance.OnGetFriendsListRequestEvent((GetFriendsListRequest)e.Request);
			}
			else if (type == typeof(GameServerRegionsRequest) && _instance.OnGetGameServerRegionsRequestEvent != null)
			{
				_instance.OnGetGameServerRegionsRequestEvent((GameServerRegionsRequest)e.Request);
			}
			else if (type == typeof(GetLeaderboardRequest) && _instance.OnGetLeaderboardRequestEvent != null)
			{
				_instance.OnGetLeaderboardRequestEvent((GetLeaderboardRequest)e.Request);
			}
			else if (type == typeof(GetLeaderboardAroundCharacterRequest) && _instance.OnGetLeaderboardAroundCharacterRequestEvent != null)
			{
				_instance.OnGetLeaderboardAroundCharacterRequestEvent((GetLeaderboardAroundCharacterRequest)e.Request);
			}
			else if (type == typeof(GetLeaderboardAroundPlayerRequest) && _instance.OnGetLeaderboardAroundPlayerRequestEvent != null)
			{
				_instance.OnGetLeaderboardAroundPlayerRequestEvent((GetLeaderboardAroundPlayerRequest)e.Request);
			}
			else if (type == typeof(GetLeaderboardForUsersCharactersRequest) && _instance.OnGetLeaderboardForUserCharactersRequestEvent != null)
			{
				_instance.OnGetLeaderboardForUserCharactersRequestEvent((GetLeaderboardForUsersCharactersRequest)e.Request);
			}
			else if (type == typeof(GetPaymentTokenRequest) && _instance.OnGetPaymentTokenRequestEvent != null)
			{
				_instance.OnGetPaymentTokenRequestEvent((GetPaymentTokenRequest)e.Request);
			}
			else if (type == typeof(GetPhotonAuthenticationTokenRequest) && _instance.OnGetPhotonAuthenticationTokenRequestEvent != null)
			{
				_instance.OnGetPhotonAuthenticationTokenRequestEvent((GetPhotonAuthenticationTokenRequest)e.Request);
			}
			else if (type == typeof(GetPlayerCombinedInfoRequest) && _instance.OnGetPlayerCombinedInfoRequestEvent != null)
			{
				_instance.OnGetPlayerCombinedInfoRequestEvent((GetPlayerCombinedInfoRequest)e.Request);
			}
			else if (type == typeof(GetPlayerProfileRequest) && _instance.OnGetPlayerProfileRequestEvent != null)
			{
				_instance.OnGetPlayerProfileRequestEvent((GetPlayerProfileRequest)e.Request);
			}
			else if (type == typeof(GetPlayerSegmentsRequest) && _instance.OnGetPlayerSegmentsRequestEvent != null)
			{
				_instance.OnGetPlayerSegmentsRequestEvent((GetPlayerSegmentsRequest)e.Request);
			}
			else if (type == typeof(GetPlayerStatisticsRequest) && _instance.OnGetPlayerStatisticsRequestEvent != null)
			{
				_instance.OnGetPlayerStatisticsRequestEvent((GetPlayerStatisticsRequest)e.Request);
			}
			else if (type == typeof(GetPlayerStatisticVersionsRequest) && _instance.OnGetPlayerStatisticVersionsRequestEvent != null)
			{
				_instance.OnGetPlayerStatisticVersionsRequestEvent((GetPlayerStatisticVersionsRequest)e.Request);
			}
			else if (type == typeof(GetPlayerTagsRequest) && _instance.OnGetPlayerTagsRequestEvent != null)
			{
				_instance.OnGetPlayerTagsRequestEvent((GetPlayerTagsRequest)e.Request);
			}
			else if (type == typeof(GetPlayerTradesRequest) && _instance.OnGetPlayerTradesRequestEvent != null)
			{
				_instance.OnGetPlayerTradesRequestEvent((GetPlayerTradesRequest)e.Request);
			}
			else if (type == typeof(GetPlayFabIDsFromFacebookIDsRequest) && _instance.OnGetPlayFabIDsFromFacebookIDsRequestEvent != null)
			{
				_instance.OnGetPlayFabIDsFromFacebookIDsRequestEvent((GetPlayFabIDsFromFacebookIDsRequest)e.Request);
			}
			else if (type == typeof(GetPlayFabIDsFromGameCenterIDsRequest) && _instance.OnGetPlayFabIDsFromGameCenterIDsRequestEvent != null)
			{
				_instance.OnGetPlayFabIDsFromGameCenterIDsRequestEvent((GetPlayFabIDsFromGameCenterIDsRequest)e.Request);
			}
			else if (type == typeof(GetPlayFabIDsFromGenericIDsRequest) && _instance.OnGetPlayFabIDsFromGenericIDsRequestEvent != null)
			{
				_instance.OnGetPlayFabIDsFromGenericIDsRequestEvent((GetPlayFabIDsFromGenericIDsRequest)e.Request);
			}
			else if (type == typeof(GetPlayFabIDsFromGoogleIDsRequest) && _instance.OnGetPlayFabIDsFromGoogleIDsRequestEvent != null)
			{
				_instance.OnGetPlayFabIDsFromGoogleIDsRequestEvent((GetPlayFabIDsFromGoogleIDsRequest)e.Request);
			}
			else if (type == typeof(GetPlayFabIDsFromKongregateIDsRequest) && _instance.OnGetPlayFabIDsFromKongregateIDsRequestEvent != null)
			{
				_instance.OnGetPlayFabIDsFromKongregateIDsRequestEvent((GetPlayFabIDsFromKongregateIDsRequest)e.Request);
			}
			else if (type == typeof(GetPlayFabIDsFromSteamIDsRequest) && _instance.OnGetPlayFabIDsFromSteamIDsRequestEvent != null)
			{
				_instance.OnGetPlayFabIDsFromSteamIDsRequestEvent((GetPlayFabIDsFromSteamIDsRequest)e.Request);
			}
			else if (type == typeof(GetPlayFabIDsFromTwitchIDsRequest) && _instance.OnGetPlayFabIDsFromTwitchIDsRequestEvent != null)
			{
				_instance.OnGetPlayFabIDsFromTwitchIDsRequestEvent((GetPlayFabIDsFromTwitchIDsRequest)e.Request);
			}
			else if (type == typeof(GetPublisherDataRequest) && _instance.OnGetPublisherDataRequestEvent != null)
			{
				_instance.OnGetPublisherDataRequestEvent((GetPublisherDataRequest)e.Request);
			}
			else if (type == typeof(GetPurchaseRequest) && _instance.OnGetPurchaseRequestEvent != null)
			{
				_instance.OnGetPurchaseRequestEvent((GetPurchaseRequest)e.Request);
			}
			else if (type == typeof(GetSharedGroupDataRequest) && _instance.OnGetSharedGroupDataRequestEvent != null)
			{
				_instance.OnGetSharedGroupDataRequestEvent((GetSharedGroupDataRequest)e.Request);
			}
			else if (type == typeof(GetStoreItemsRequest) && _instance.OnGetStoreItemsRequestEvent != null)
			{
				_instance.OnGetStoreItemsRequestEvent((GetStoreItemsRequest)e.Request);
			}
			else if (type == typeof(GetTimeRequest) && _instance.OnGetTimeRequestEvent != null)
			{
				_instance.OnGetTimeRequestEvent((GetTimeRequest)e.Request);
			}
			else if (type == typeof(GetTitleDataRequest) && _instance.OnGetTitleDataRequestEvent != null)
			{
				_instance.OnGetTitleDataRequestEvent((GetTitleDataRequest)e.Request);
			}
			else if (type == typeof(GetTitleNewsRequest) && _instance.OnGetTitleNewsRequestEvent != null)
			{
				_instance.OnGetTitleNewsRequestEvent((GetTitleNewsRequest)e.Request);
			}
			else if (type == typeof(GetTitlePublicKeyRequest) && _instance.OnGetTitlePublicKeyRequestEvent != null)
			{
				_instance.OnGetTitlePublicKeyRequestEvent((GetTitlePublicKeyRequest)e.Request);
			}
			else if (type == typeof(GetTradeStatusRequest) && _instance.OnGetTradeStatusRequestEvent != null)
			{
				_instance.OnGetTradeStatusRequestEvent((GetTradeStatusRequest)e.Request);
			}
			else if (type == typeof(GetUserDataRequest) && _instance.OnGetUserDataRequestEvent != null)
			{
				_instance.OnGetUserDataRequestEvent((GetUserDataRequest)e.Request);
			}
			else if (type == typeof(GetUserInventoryRequest) && _instance.OnGetUserInventoryRequestEvent != null)
			{
				_instance.OnGetUserInventoryRequestEvent((GetUserInventoryRequest)e.Request);
			}
			else if (type == typeof(GetUserDataRequest) && _instance.OnGetUserPublisherDataRequestEvent != null)
			{
				_instance.OnGetUserPublisherDataRequestEvent((GetUserDataRequest)e.Request);
			}
			else if (type == typeof(GetUserDataRequest) && _instance.OnGetUserPublisherReadOnlyDataRequestEvent != null)
			{
				_instance.OnGetUserPublisherReadOnlyDataRequestEvent((GetUserDataRequest)e.Request);
			}
			else if (type == typeof(GetUserDataRequest) && _instance.OnGetUserReadOnlyDataRequestEvent != null)
			{
				_instance.OnGetUserReadOnlyDataRequestEvent((GetUserDataRequest)e.Request);
			}
			else if (type == typeof(GetWindowsHelloChallengeRequest) && _instance.OnGetWindowsHelloChallengeRequestEvent != null)
			{
				_instance.OnGetWindowsHelloChallengeRequestEvent((GetWindowsHelloChallengeRequest)e.Request);
			}
			else if (type == typeof(GrantCharacterToUserRequest) && _instance.OnGrantCharacterToUserRequestEvent != null)
			{
				_instance.OnGrantCharacterToUserRequestEvent((GrantCharacterToUserRequest)e.Request);
			}
			else if (type == typeof(LinkAndroidDeviceIDRequest) && _instance.OnLinkAndroidDeviceIDRequestEvent != null)
			{
				_instance.OnLinkAndroidDeviceIDRequestEvent((LinkAndroidDeviceIDRequest)e.Request);
			}
			else if (type == typeof(LinkCustomIDRequest) && _instance.OnLinkCustomIDRequestEvent != null)
			{
				_instance.OnLinkCustomIDRequestEvent((LinkCustomIDRequest)e.Request);
			}
			else if (type == typeof(LinkFacebookAccountRequest) && _instance.OnLinkFacebookAccountRequestEvent != null)
			{
				_instance.OnLinkFacebookAccountRequestEvent((LinkFacebookAccountRequest)e.Request);
			}
			else if (type == typeof(LinkGameCenterAccountRequest) && _instance.OnLinkGameCenterAccountRequestEvent != null)
			{
				_instance.OnLinkGameCenterAccountRequestEvent((LinkGameCenterAccountRequest)e.Request);
			}
			else if (type == typeof(LinkGoogleAccountRequest) && _instance.OnLinkGoogleAccountRequestEvent != null)
			{
				_instance.OnLinkGoogleAccountRequestEvent((LinkGoogleAccountRequest)e.Request);
			}
			else if (type == typeof(LinkIOSDeviceIDRequest) && _instance.OnLinkIOSDeviceIDRequestEvent != null)
			{
				_instance.OnLinkIOSDeviceIDRequestEvent((LinkIOSDeviceIDRequest)e.Request);
			}
			else if (type == typeof(LinkKongregateAccountRequest) && _instance.OnLinkKongregateRequestEvent != null)
			{
				_instance.OnLinkKongregateRequestEvent((LinkKongregateAccountRequest)e.Request);
			}
			else if (type == typeof(LinkSteamAccountRequest) && _instance.OnLinkSteamAccountRequestEvent != null)
			{
				_instance.OnLinkSteamAccountRequestEvent((LinkSteamAccountRequest)e.Request);
			}
			else if (type == typeof(LinkTwitchAccountRequest) && _instance.OnLinkTwitchRequestEvent != null)
			{
				_instance.OnLinkTwitchRequestEvent((LinkTwitchAccountRequest)e.Request);
			}
			else if (type == typeof(LinkWindowsHelloAccountRequest) && _instance.OnLinkWindowsHelloRequestEvent != null)
			{
				_instance.OnLinkWindowsHelloRequestEvent((LinkWindowsHelloAccountRequest)e.Request);
			}
			else if (type == typeof(LoginWithAndroidDeviceIDRequest) && _instance.OnLoginWithAndroidDeviceIDRequestEvent != null)
			{
				_instance.OnLoginWithAndroidDeviceIDRequestEvent((LoginWithAndroidDeviceIDRequest)e.Request);
			}
			else if (type == typeof(LoginWithCustomIDRequest) && _instance.OnLoginWithCustomIDRequestEvent != null)
			{
				_instance.OnLoginWithCustomIDRequestEvent((LoginWithCustomIDRequest)e.Request);
			}
			else if (type == typeof(LoginWithEmailAddressRequest) && _instance.OnLoginWithEmailAddressRequestEvent != null)
			{
				_instance.OnLoginWithEmailAddressRequestEvent((LoginWithEmailAddressRequest)e.Request);
			}
			else if (type == typeof(LoginWithFacebookRequest) && _instance.OnLoginWithFacebookRequestEvent != null)
			{
				_instance.OnLoginWithFacebookRequestEvent((LoginWithFacebookRequest)e.Request);
			}
			else if (type == typeof(LoginWithGameCenterRequest) && _instance.OnLoginWithGameCenterRequestEvent != null)
			{
				_instance.OnLoginWithGameCenterRequestEvent((LoginWithGameCenterRequest)e.Request);
			}
			else if (type == typeof(LoginWithGoogleAccountRequest) && _instance.OnLoginWithGoogleAccountRequestEvent != null)
			{
				_instance.OnLoginWithGoogleAccountRequestEvent((LoginWithGoogleAccountRequest)e.Request);
			}
			else if (type == typeof(LoginWithIOSDeviceIDRequest) && _instance.OnLoginWithIOSDeviceIDRequestEvent != null)
			{
				_instance.OnLoginWithIOSDeviceIDRequestEvent((LoginWithIOSDeviceIDRequest)e.Request);
			}
			else if (type == typeof(LoginWithKongregateRequest) && _instance.OnLoginWithKongregateRequestEvent != null)
			{
				_instance.OnLoginWithKongregateRequestEvent((LoginWithKongregateRequest)e.Request);
			}
			else if (type == typeof(LoginWithPlayFabRequest) && _instance.OnLoginWithPlayFabRequestEvent != null)
			{
				_instance.OnLoginWithPlayFabRequestEvent((LoginWithPlayFabRequest)e.Request);
			}
			else if (type == typeof(LoginWithSteamRequest) && _instance.OnLoginWithSteamRequestEvent != null)
			{
				_instance.OnLoginWithSteamRequestEvent((LoginWithSteamRequest)e.Request);
			}
			else if (type == typeof(LoginWithTwitchRequest) && _instance.OnLoginWithTwitchRequestEvent != null)
			{
				_instance.OnLoginWithTwitchRequestEvent((LoginWithTwitchRequest)e.Request);
			}
			else if (type == typeof(LoginWithWindowsHelloRequest) && _instance.OnLoginWithWindowsHelloRequestEvent != null)
			{
				_instance.OnLoginWithWindowsHelloRequestEvent((LoginWithWindowsHelloRequest)e.Request);
			}
			else if (type == typeof(MatchmakeRequest) && _instance.OnMatchmakeRequestEvent != null)
			{
				_instance.OnMatchmakeRequestEvent((MatchmakeRequest)e.Request);
			}
			else if (type == typeof(OpenTradeRequest) && _instance.OnOpenTradeRequestEvent != null)
			{
				_instance.OnOpenTradeRequestEvent((OpenTradeRequest)e.Request);
			}
			else if (type == typeof(PayForPurchaseRequest) && _instance.OnPayForPurchaseRequestEvent != null)
			{
				_instance.OnPayForPurchaseRequestEvent((PayForPurchaseRequest)e.Request);
			}
			else if (type == typeof(PurchaseItemRequest) && _instance.OnPurchaseItemRequestEvent != null)
			{
				_instance.OnPurchaseItemRequestEvent((PurchaseItemRequest)e.Request);
			}
			else if (type == typeof(RedeemCouponRequest) && _instance.OnRedeemCouponRequestEvent != null)
			{
				_instance.OnRedeemCouponRequestEvent((RedeemCouponRequest)e.Request);
			}
			else if (type == typeof(RegisterForIOSPushNotificationRequest) && _instance.OnRegisterForIOSPushNotificationRequestEvent != null)
			{
				_instance.OnRegisterForIOSPushNotificationRequestEvent((RegisterForIOSPushNotificationRequest)e.Request);
			}
			else if (type == typeof(RegisterPlayFabUserRequest) && _instance.OnRegisterPlayFabUserRequestEvent != null)
			{
				_instance.OnRegisterPlayFabUserRequestEvent((RegisterPlayFabUserRequest)e.Request);
			}
			else if (type == typeof(RegisterWithWindowsHelloRequest) && _instance.OnRegisterWithWindowsHelloRequestEvent != null)
			{
				_instance.OnRegisterWithWindowsHelloRequestEvent((RegisterWithWindowsHelloRequest)e.Request);
			}
			else if (type == typeof(RemoveContactEmailRequest) && _instance.OnRemoveContactEmailRequestEvent != null)
			{
				_instance.OnRemoveContactEmailRequestEvent((RemoveContactEmailRequest)e.Request);
			}
			else if (type == typeof(RemoveFriendRequest) && _instance.OnRemoveFriendRequestEvent != null)
			{
				_instance.OnRemoveFriendRequestEvent((RemoveFriendRequest)e.Request);
			}
			else if (type == typeof(RemoveGenericIDRequest) && _instance.OnRemoveGenericIDRequestEvent != null)
			{
				_instance.OnRemoveGenericIDRequestEvent((RemoveGenericIDRequest)e.Request);
			}
			else if (type == typeof(RemoveSharedGroupMembersRequest) && _instance.OnRemoveSharedGroupMembersRequestEvent != null)
			{
				_instance.OnRemoveSharedGroupMembersRequestEvent((RemoveSharedGroupMembersRequest)e.Request);
			}
			else if (type == typeof(DeviceInfoRequest) && _instance.OnReportDeviceInfoRequestEvent != null)
			{
				_instance.OnReportDeviceInfoRequestEvent((DeviceInfoRequest)e.Request);
			}
			else if (type == typeof(ReportPlayerClientRequest) && _instance.OnReportPlayerRequestEvent != null)
			{
				_instance.OnReportPlayerRequestEvent((ReportPlayerClientRequest)e.Request);
			}
			else if (type == typeof(RestoreIOSPurchasesRequest) && _instance.OnRestoreIOSPurchasesRequestEvent != null)
			{
				_instance.OnRestoreIOSPurchasesRequestEvent((RestoreIOSPurchasesRequest)e.Request);
			}
			else if (type == typeof(SendAccountRecoveryEmailRequest) && _instance.OnSendAccountRecoveryEmailRequestEvent != null)
			{
				_instance.OnSendAccountRecoveryEmailRequestEvent((SendAccountRecoveryEmailRequest)e.Request);
			}
			else if (type == typeof(SetFriendTagsRequest) && _instance.OnSetFriendTagsRequestEvent != null)
			{
				_instance.OnSetFriendTagsRequestEvent((SetFriendTagsRequest)e.Request);
			}
			else if (type == typeof(SetPlayerSecretRequest) && _instance.OnSetPlayerSecretRequestEvent != null)
			{
				_instance.OnSetPlayerSecretRequestEvent((SetPlayerSecretRequest)e.Request);
			}
			else if (type == typeof(StartGameRequest) && _instance.OnStartGameRequestEvent != null)
			{
				_instance.OnStartGameRequestEvent((StartGameRequest)e.Request);
			}
			else if (type == typeof(StartPurchaseRequest) && _instance.OnStartPurchaseRequestEvent != null)
			{
				_instance.OnStartPurchaseRequestEvent((StartPurchaseRequest)e.Request);
			}
			else if (type == typeof(SubtractUserVirtualCurrencyRequest) && _instance.OnSubtractUserVirtualCurrencyRequestEvent != null)
			{
				_instance.OnSubtractUserVirtualCurrencyRequestEvent((SubtractUserVirtualCurrencyRequest)e.Request);
			}
			else if (type == typeof(UnlinkAndroidDeviceIDRequest) && _instance.OnUnlinkAndroidDeviceIDRequestEvent != null)
			{
				_instance.OnUnlinkAndroidDeviceIDRequestEvent((UnlinkAndroidDeviceIDRequest)e.Request);
			}
			else if (type == typeof(UnlinkCustomIDRequest) && _instance.OnUnlinkCustomIDRequestEvent != null)
			{
				_instance.OnUnlinkCustomIDRequestEvent((UnlinkCustomIDRequest)e.Request);
			}
			else if (type == typeof(UnlinkFacebookAccountRequest) && _instance.OnUnlinkFacebookAccountRequestEvent != null)
			{
				_instance.OnUnlinkFacebookAccountRequestEvent((UnlinkFacebookAccountRequest)e.Request);
			}
			else if (type == typeof(UnlinkGameCenterAccountRequest) && _instance.OnUnlinkGameCenterAccountRequestEvent != null)
			{
				_instance.OnUnlinkGameCenterAccountRequestEvent((UnlinkGameCenterAccountRequest)e.Request);
			}
			else if (type == typeof(UnlinkGoogleAccountRequest) && _instance.OnUnlinkGoogleAccountRequestEvent != null)
			{
				_instance.OnUnlinkGoogleAccountRequestEvent((UnlinkGoogleAccountRequest)e.Request);
			}
			else if (type == typeof(UnlinkIOSDeviceIDRequest) && _instance.OnUnlinkIOSDeviceIDRequestEvent != null)
			{
				_instance.OnUnlinkIOSDeviceIDRequestEvent((UnlinkIOSDeviceIDRequest)e.Request);
			}
			else if (type == typeof(UnlinkKongregateAccountRequest) && _instance.OnUnlinkKongregateRequestEvent != null)
			{
				_instance.OnUnlinkKongregateRequestEvent((UnlinkKongregateAccountRequest)e.Request);
			}
			else if (type == typeof(UnlinkSteamAccountRequest) && _instance.OnUnlinkSteamAccountRequestEvent != null)
			{
				_instance.OnUnlinkSteamAccountRequestEvent((UnlinkSteamAccountRequest)e.Request);
			}
			else if (type == typeof(UnlinkTwitchAccountRequest) && _instance.OnUnlinkTwitchRequestEvent != null)
			{
				_instance.OnUnlinkTwitchRequestEvent((UnlinkTwitchAccountRequest)e.Request);
			}
			else if (type == typeof(UnlinkWindowsHelloAccountRequest) && _instance.OnUnlinkWindowsHelloRequestEvent != null)
			{
				_instance.OnUnlinkWindowsHelloRequestEvent((UnlinkWindowsHelloAccountRequest)e.Request);
			}
			else if (type == typeof(UnlockContainerInstanceRequest) && _instance.OnUnlockContainerInstanceRequestEvent != null)
			{
				_instance.OnUnlockContainerInstanceRequestEvent((UnlockContainerInstanceRequest)e.Request);
			}
			else if (type == typeof(UnlockContainerItemRequest) && _instance.OnUnlockContainerItemRequestEvent != null)
			{
				_instance.OnUnlockContainerItemRequestEvent((UnlockContainerItemRequest)e.Request);
			}
			else if (type == typeof(UpdateAvatarUrlRequest) && _instance.OnUpdateAvatarUrlRequestEvent != null)
			{
				_instance.OnUpdateAvatarUrlRequestEvent((UpdateAvatarUrlRequest)e.Request);
			}
			else if (type == typeof(UpdateCharacterDataRequest) && _instance.OnUpdateCharacterDataRequestEvent != null)
			{
				_instance.OnUpdateCharacterDataRequestEvent((UpdateCharacterDataRequest)e.Request);
			}
			else if (type == typeof(UpdateCharacterStatisticsRequest) && _instance.OnUpdateCharacterStatisticsRequestEvent != null)
			{
				_instance.OnUpdateCharacterStatisticsRequestEvent((UpdateCharacterStatisticsRequest)e.Request);
			}
			else if (type == typeof(UpdatePlayerStatisticsRequest) && _instance.OnUpdatePlayerStatisticsRequestEvent != null)
			{
				_instance.OnUpdatePlayerStatisticsRequestEvent((UpdatePlayerStatisticsRequest)e.Request);
			}
			else if (type == typeof(UpdateSharedGroupDataRequest) && _instance.OnUpdateSharedGroupDataRequestEvent != null)
			{
				_instance.OnUpdateSharedGroupDataRequestEvent((UpdateSharedGroupDataRequest)e.Request);
			}
			else if (type == typeof(UpdateUserDataRequest) && _instance.OnUpdateUserDataRequestEvent != null)
			{
				_instance.OnUpdateUserDataRequestEvent((UpdateUserDataRequest)e.Request);
			}
			else if (type == typeof(UpdateUserDataRequest) && _instance.OnUpdateUserPublisherDataRequestEvent != null)
			{
				_instance.OnUpdateUserPublisherDataRequestEvent((UpdateUserDataRequest)e.Request);
			}
			else if (type == typeof(UpdateUserTitleDisplayNameRequest) && _instance.OnUpdateUserTitleDisplayNameRequestEvent != null)
			{
				_instance.OnUpdateUserTitleDisplayNameRequestEvent((UpdateUserTitleDisplayNameRequest)e.Request);
			}
			else if (type == typeof(ValidateAmazonReceiptRequest) && _instance.OnValidateAmazonIAPReceiptRequestEvent != null)
			{
				_instance.OnValidateAmazonIAPReceiptRequestEvent((ValidateAmazonReceiptRequest)e.Request);
			}
			else if (type == typeof(ValidateGooglePlayPurchaseRequest) && _instance.OnValidateGooglePlayPurchaseRequestEvent != null)
			{
				_instance.OnValidateGooglePlayPurchaseRequestEvent((ValidateGooglePlayPurchaseRequest)e.Request);
			}
			else if (type == typeof(ValidateIOSReceiptRequest) && _instance.OnValidateIOSReceiptRequestEvent != null)
			{
				_instance.OnValidateIOSReceiptRequestEvent((ValidateIOSReceiptRequest)e.Request);
			}
			else if (type == typeof(ValidateWindowsReceiptRequest) && _instance.OnValidateWindowsStoreReceiptRequestEvent != null)
			{
				_instance.OnValidateWindowsStoreReceiptRequestEvent((ValidateWindowsReceiptRequest)e.Request);
			}
			else if (type == typeof(WriteClientCharacterEventRequest) && _instance.OnWriteCharacterEventRequestEvent != null)
			{
				_instance.OnWriteCharacterEventRequestEvent((WriteClientCharacterEventRequest)e.Request);
			}
			else if (type == typeof(WriteClientPlayerEventRequest) && _instance.OnWritePlayerEventRequestEvent != null)
			{
				_instance.OnWritePlayerEventRequestEvent((WriteClientPlayerEventRequest)e.Request);
			}
			else if (type == typeof(WriteTitleEventRequest) && _instance.OnWriteTitleEventRequestEvent != null)
			{
				_instance.OnWriteTitleEventRequestEvent((WriteTitleEventRequest)e.Request);
			}
		}
		else
		{
			Type type2 = e.Result.GetType();
			if (type2 == typeof(LoginResult) && _instance.OnLoginResultEvent != null)
			{
				_instance.OnLoginResultEvent((LoginResult)e.Result);
			}
			else if (type2 == typeof(AcceptTradeResponse) && _instance.OnAcceptTradeResultEvent != null)
			{
				_instance.OnAcceptTradeResultEvent((AcceptTradeResponse)e.Result);
			}
			else if (type2 == typeof(AddFriendResult) && _instance.OnAddFriendResultEvent != null)
			{
				_instance.OnAddFriendResultEvent((AddFriendResult)e.Result);
			}
			else if (type2 == typeof(AddGenericIDResult) && _instance.OnAddGenericIDResultEvent != null)
			{
				_instance.OnAddGenericIDResultEvent((AddGenericIDResult)e.Result);
			}
			else if (type2 == typeof(AddOrUpdateContactEmailResult) && _instance.OnAddOrUpdateContactEmailResultEvent != null)
			{
				_instance.OnAddOrUpdateContactEmailResultEvent((AddOrUpdateContactEmailResult)e.Result);
			}
			else if (type2 == typeof(AddSharedGroupMembersResult) && _instance.OnAddSharedGroupMembersResultEvent != null)
			{
				_instance.OnAddSharedGroupMembersResultEvent((AddSharedGroupMembersResult)e.Result);
			}
			else if (type2 == typeof(AddUsernamePasswordResult) && _instance.OnAddUsernamePasswordResultEvent != null)
			{
				_instance.OnAddUsernamePasswordResultEvent((AddUsernamePasswordResult)e.Result);
			}
			else if (type2 == typeof(ModifyUserVirtualCurrencyResult) && _instance.OnAddUserVirtualCurrencyResultEvent != null)
			{
				_instance.OnAddUserVirtualCurrencyResultEvent((ModifyUserVirtualCurrencyResult)e.Result);
			}
			else if (type2 == typeof(AndroidDevicePushNotificationRegistrationResult) && _instance.OnAndroidDevicePushNotificationRegistrationResultEvent != null)
			{
				_instance.OnAndroidDevicePushNotificationRegistrationResultEvent((AndroidDevicePushNotificationRegistrationResult)e.Result);
			}
			else if (type2 == typeof(AttributeInstallResult) && _instance.OnAttributeInstallResultEvent != null)
			{
				_instance.OnAttributeInstallResultEvent((AttributeInstallResult)e.Result);
			}
			else if (type2 == typeof(CancelTradeResponse) && _instance.OnCancelTradeResultEvent != null)
			{
				_instance.OnCancelTradeResultEvent((CancelTradeResponse)e.Result);
			}
			else if (type2 == typeof(ConfirmPurchaseResult) && _instance.OnConfirmPurchaseResultEvent != null)
			{
				_instance.OnConfirmPurchaseResultEvent((ConfirmPurchaseResult)e.Result);
			}
			else if (type2 == typeof(ConsumeItemResult) && _instance.OnConsumeItemResultEvent != null)
			{
				_instance.OnConsumeItemResultEvent((ConsumeItemResult)e.Result);
			}
			else if (type2 == typeof(CreateSharedGroupResult) && _instance.OnCreateSharedGroupResultEvent != null)
			{
				_instance.OnCreateSharedGroupResultEvent((CreateSharedGroupResult)e.Result);
			}
			else if (type2 == typeof(ExecuteCloudScriptResult) && _instance.OnExecuteCloudScriptResultEvent != null)
			{
				_instance.OnExecuteCloudScriptResultEvent((ExecuteCloudScriptResult)e.Result);
			}
			else if (type2 == typeof(GetAccountInfoResult) && _instance.OnGetAccountInfoResultEvent != null)
			{
				_instance.OnGetAccountInfoResultEvent((GetAccountInfoResult)e.Result);
			}
			else if (type2 == typeof(ListUsersCharactersResult) && _instance.OnGetAllUsersCharactersResultEvent != null)
			{
				_instance.OnGetAllUsersCharactersResultEvent((ListUsersCharactersResult)e.Result);
			}
			else if (type2 == typeof(GetCatalogItemsResult) && _instance.OnGetCatalogItemsResultEvent != null)
			{
				_instance.OnGetCatalogItemsResultEvent((GetCatalogItemsResult)e.Result);
			}
			else if (type2 == typeof(GetCharacterDataResult) && _instance.OnGetCharacterDataResultEvent != null)
			{
				_instance.OnGetCharacterDataResultEvent((GetCharacterDataResult)e.Result);
			}
			else if (type2 == typeof(GetCharacterInventoryResult) && _instance.OnGetCharacterInventoryResultEvent != null)
			{
				_instance.OnGetCharacterInventoryResultEvent((GetCharacterInventoryResult)e.Result);
			}
			else if (type2 == typeof(GetCharacterLeaderboardResult) && _instance.OnGetCharacterLeaderboardResultEvent != null)
			{
				_instance.OnGetCharacterLeaderboardResultEvent((GetCharacterLeaderboardResult)e.Result);
			}
			else if (type2 == typeof(GetCharacterDataResult) && _instance.OnGetCharacterReadOnlyDataResultEvent != null)
			{
				_instance.OnGetCharacterReadOnlyDataResultEvent((GetCharacterDataResult)e.Result);
			}
			else if (type2 == typeof(GetCharacterStatisticsResult) && _instance.OnGetCharacterStatisticsResultEvent != null)
			{
				_instance.OnGetCharacterStatisticsResultEvent((GetCharacterStatisticsResult)e.Result);
			}
			else if (type2 == typeof(GetContentDownloadUrlResult) && _instance.OnGetContentDownloadUrlResultEvent != null)
			{
				_instance.OnGetContentDownloadUrlResultEvent((GetContentDownloadUrlResult)e.Result);
			}
			else if (type2 == typeof(CurrentGamesResult) && _instance.OnGetCurrentGamesResultEvent != null)
			{
				_instance.OnGetCurrentGamesResultEvent((CurrentGamesResult)e.Result);
			}
			else if (type2 == typeof(GetLeaderboardResult) && _instance.OnGetFriendLeaderboardResultEvent != null)
			{
				_instance.OnGetFriendLeaderboardResultEvent((GetLeaderboardResult)e.Result);
			}
			else if (type2 == typeof(GetFriendLeaderboardAroundPlayerResult) && _instance.OnGetFriendLeaderboardAroundPlayerResultEvent != null)
			{
				_instance.OnGetFriendLeaderboardAroundPlayerResultEvent((GetFriendLeaderboardAroundPlayerResult)e.Result);
			}
			else if (type2 == typeof(GetFriendsListResult) && _instance.OnGetFriendsListResultEvent != null)
			{
				_instance.OnGetFriendsListResultEvent((GetFriendsListResult)e.Result);
			}
			else if (type2 == typeof(GameServerRegionsResult) && _instance.OnGetGameServerRegionsResultEvent != null)
			{
				_instance.OnGetGameServerRegionsResultEvent((GameServerRegionsResult)e.Result);
			}
			else if (type2 == typeof(GetLeaderboardResult) && _instance.OnGetLeaderboardResultEvent != null)
			{
				_instance.OnGetLeaderboardResultEvent((GetLeaderboardResult)e.Result);
			}
			else if (type2 == typeof(GetLeaderboardAroundCharacterResult) && _instance.OnGetLeaderboardAroundCharacterResultEvent != null)
			{
				_instance.OnGetLeaderboardAroundCharacterResultEvent((GetLeaderboardAroundCharacterResult)e.Result);
			}
			else if (type2 == typeof(GetLeaderboardAroundPlayerResult) && _instance.OnGetLeaderboardAroundPlayerResultEvent != null)
			{
				_instance.OnGetLeaderboardAroundPlayerResultEvent((GetLeaderboardAroundPlayerResult)e.Result);
			}
			else if (type2 == typeof(GetLeaderboardForUsersCharactersResult) && _instance.OnGetLeaderboardForUserCharactersResultEvent != null)
			{
				_instance.OnGetLeaderboardForUserCharactersResultEvent((GetLeaderboardForUsersCharactersResult)e.Result);
			}
			else if (type2 == typeof(GetPaymentTokenResult) && _instance.OnGetPaymentTokenResultEvent != null)
			{
				_instance.OnGetPaymentTokenResultEvent((GetPaymentTokenResult)e.Result);
			}
			else if (type2 == typeof(GetPhotonAuthenticationTokenResult) && _instance.OnGetPhotonAuthenticationTokenResultEvent != null)
			{
				_instance.OnGetPhotonAuthenticationTokenResultEvent((GetPhotonAuthenticationTokenResult)e.Result);
			}
			else if (type2 == typeof(GetPlayerCombinedInfoResult) && _instance.OnGetPlayerCombinedInfoResultEvent != null)
			{
				_instance.OnGetPlayerCombinedInfoResultEvent((GetPlayerCombinedInfoResult)e.Result);
			}
			else if (type2 == typeof(GetPlayerProfileResult) && _instance.OnGetPlayerProfileResultEvent != null)
			{
				_instance.OnGetPlayerProfileResultEvent((GetPlayerProfileResult)e.Result);
			}
			else if (type2 == typeof(GetPlayerSegmentsResult) && _instance.OnGetPlayerSegmentsResultEvent != null)
			{
				_instance.OnGetPlayerSegmentsResultEvent((GetPlayerSegmentsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayerStatisticsResult) && _instance.OnGetPlayerStatisticsResultEvent != null)
			{
				_instance.OnGetPlayerStatisticsResultEvent((GetPlayerStatisticsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayerStatisticVersionsResult) && _instance.OnGetPlayerStatisticVersionsResultEvent != null)
			{
				_instance.OnGetPlayerStatisticVersionsResultEvent((GetPlayerStatisticVersionsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayerTagsResult) && _instance.OnGetPlayerTagsResultEvent != null)
			{
				_instance.OnGetPlayerTagsResultEvent((GetPlayerTagsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayerTradesResponse) && _instance.OnGetPlayerTradesResultEvent != null)
			{
				_instance.OnGetPlayerTradesResultEvent((GetPlayerTradesResponse)e.Result);
			}
			else if (type2 == typeof(GetPlayFabIDsFromFacebookIDsResult) && _instance.OnGetPlayFabIDsFromFacebookIDsResultEvent != null)
			{
				_instance.OnGetPlayFabIDsFromFacebookIDsResultEvent((GetPlayFabIDsFromFacebookIDsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayFabIDsFromGameCenterIDsResult) && _instance.OnGetPlayFabIDsFromGameCenterIDsResultEvent != null)
			{
				_instance.OnGetPlayFabIDsFromGameCenterIDsResultEvent((GetPlayFabIDsFromGameCenterIDsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayFabIDsFromGenericIDsResult) && _instance.OnGetPlayFabIDsFromGenericIDsResultEvent != null)
			{
				_instance.OnGetPlayFabIDsFromGenericIDsResultEvent((GetPlayFabIDsFromGenericIDsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayFabIDsFromGoogleIDsResult) && _instance.OnGetPlayFabIDsFromGoogleIDsResultEvent != null)
			{
				_instance.OnGetPlayFabIDsFromGoogleIDsResultEvent((GetPlayFabIDsFromGoogleIDsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayFabIDsFromKongregateIDsResult) && _instance.OnGetPlayFabIDsFromKongregateIDsResultEvent != null)
			{
				_instance.OnGetPlayFabIDsFromKongregateIDsResultEvent((GetPlayFabIDsFromKongregateIDsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayFabIDsFromSteamIDsResult) && _instance.OnGetPlayFabIDsFromSteamIDsResultEvent != null)
			{
				_instance.OnGetPlayFabIDsFromSteamIDsResultEvent((GetPlayFabIDsFromSteamIDsResult)e.Result);
			}
			else if (type2 == typeof(GetPlayFabIDsFromTwitchIDsResult) && _instance.OnGetPlayFabIDsFromTwitchIDsResultEvent != null)
			{
				_instance.OnGetPlayFabIDsFromTwitchIDsResultEvent((GetPlayFabIDsFromTwitchIDsResult)e.Result);
			}
			else if (type2 == typeof(GetPublisherDataResult) && _instance.OnGetPublisherDataResultEvent != null)
			{
				_instance.OnGetPublisherDataResultEvent((GetPublisherDataResult)e.Result);
			}
			else if (type2 == typeof(GetPurchaseResult) && _instance.OnGetPurchaseResultEvent != null)
			{
				_instance.OnGetPurchaseResultEvent((GetPurchaseResult)e.Result);
			}
			else if (type2 == typeof(GetSharedGroupDataResult) && _instance.OnGetSharedGroupDataResultEvent != null)
			{
				_instance.OnGetSharedGroupDataResultEvent((GetSharedGroupDataResult)e.Result);
			}
			else if (type2 == typeof(GetStoreItemsResult) && _instance.OnGetStoreItemsResultEvent != null)
			{
				_instance.OnGetStoreItemsResultEvent((GetStoreItemsResult)e.Result);
			}
			else if (type2 == typeof(GetTimeResult) && _instance.OnGetTimeResultEvent != null)
			{
				_instance.OnGetTimeResultEvent((GetTimeResult)e.Result);
			}
			else if (type2 == typeof(GetTitleDataResult) && _instance.OnGetTitleDataResultEvent != null)
			{
				_instance.OnGetTitleDataResultEvent((GetTitleDataResult)e.Result);
			}
			else if (type2 == typeof(GetTitleNewsResult) && _instance.OnGetTitleNewsResultEvent != null)
			{
				_instance.OnGetTitleNewsResultEvent((GetTitleNewsResult)e.Result);
			}
			else if (type2 == typeof(GetTitlePublicKeyResult) && _instance.OnGetTitlePublicKeyResultEvent != null)
			{
				_instance.OnGetTitlePublicKeyResultEvent((GetTitlePublicKeyResult)e.Result);
			}
			else if (type2 == typeof(GetTradeStatusResponse) && _instance.OnGetTradeStatusResultEvent != null)
			{
				_instance.OnGetTradeStatusResultEvent((GetTradeStatusResponse)e.Result);
			}
			else if (type2 == typeof(GetUserDataResult) && _instance.OnGetUserDataResultEvent != null)
			{
				_instance.OnGetUserDataResultEvent((GetUserDataResult)e.Result);
			}
			else if (type2 == typeof(GetUserInventoryResult) && _instance.OnGetUserInventoryResultEvent != null)
			{
				_instance.OnGetUserInventoryResultEvent((GetUserInventoryResult)e.Result);
			}
			else if (type2 == typeof(GetUserDataResult) && _instance.OnGetUserPublisherDataResultEvent != null)
			{
				_instance.OnGetUserPublisherDataResultEvent((GetUserDataResult)e.Result);
			}
			else if (type2 == typeof(GetUserDataResult) && _instance.OnGetUserPublisherReadOnlyDataResultEvent != null)
			{
				_instance.OnGetUserPublisherReadOnlyDataResultEvent((GetUserDataResult)e.Result);
			}
			else if (type2 == typeof(GetUserDataResult) && _instance.OnGetUserReadOnlyDataResultEvent != null)
			{
				_instance.OnGetUserReadOnlyDataResultEvent((GetUserDataResult)e.Result);
			}
			else if (type2 == typeof(GetWindowsHelloChallengeResponse) && _instance.OnGetWindowsHelloChallengeResultEvent != null)
			{
				_instance.OnGetWindowsHelloChallengeResultEvent((GetWindowsHelloChallengeResponse)e.Result);
			}
			else if (type2 == typeof(GrantCharacterToUserResult) && _instance.OnGrantCharacterToUserResultEvent != null)
			{
				_instance.OnGrantCharacterToUserResultEvent((GrantCharacterToUserResult)e.Result);
			}
			else if (type2 == typeof(LinkAndroidDeviceIDResult) && _instance.OnLinkAndroidDeviceIDResultEvent != null)
			{
				_instance.OnLinkAndroidDeviceIDResultEvent((LinkAndroidDeviceIDResult)e.Result);
			}
			else if (type2 == typeof(LinkCustomIDResult) && _instance.OnLinkCustomIDResultEvent != null)
			{
				_instance.OnLinkCustomIDResultEvent((LinkCustomIDResult)e.Result);
			}
			else if (type2 == typeof(LinkFacebookAccountResult) && _instance.OnLinkFacebookAccountResultEvent != null)
			{
				_instance.OnLinkFacebookAccountResultEvent((LinkFacebookAccountResult)e.Result);
			}
			else if (type2 == typeof(LinkGameCenterAccountResult) && _instance.OnLinkGameCenterAccountResultEvent != null)
			{
				_instance.OnLinkGameCenterAccountResultEvent((LinkGameCenterAccountResult)e.Result);
			}
			else if (type2 == typeof(LinkGoogleAccountResult) && _instance.OnLinkGoogleAccountResultEvent != null)
			{
				_instance.OnLinkGoogleAccountResultEvent((LinkGoogleAccountResult)e.Result);
			}
			else if (type2 == typeof(LinkIOSDeviceIDResult) && _instance.OnLinkIOSDeviceIDResultEvent != null)
			{
				_instance.OnLinkIOSDeviceIDResultEvent((LinkIOSDeviceIDResult)e.Result);
			}
			else if (type2 == typeof(LinkKongregateAccountResult) && _instance.OnLinkKongregateResultEvent != null)
			{
				_instance.OnLinkKongregateResultEvent((LinkKongregateAccountResult)e.Result);
			}
			else if (type2 == typeof(LinkSteamAccountResult) && _instance.OnLinkSteamAccountResultEvent != null)
			{
				_instance.OnLinkSteamAccountResultEvent((LinkSteamAccountResult)e.Result);
			}
			else if (type2 == typeof(LinkTwitchAccountResult) && _instance.OnLinkTwitchResultEvent != null)
			{
				_instance.OnLinkTwitchResultEvent((LinkTwitchAccountResult)e.Result);
			}
			else if (type2 == typeof(LinkWindowsHelloAccountResponse) && _instance.OnLinkWindowsHelloResultEvent != null)
			{
				_instance.OnLinkWindowsHelloResultEvent((LinkWindowsHelloAccountResponse)e.Result);
			}
			else if (type2 == typeof(MatchmakeResult) && _instance.OnMatchmakeResultEvent != null)
			{
				_instance.OnMatchmakeResultEvent((MatchmakeResult)e.Result);
			}
			else if (type2 == typeof(OpenTradeResponse) && _instance.OnOpenTradeResultEvent != null)
			{
				_instance.OnOpenTradeResultEvent((OpenTradeResponse)e.Result);
			}
			else if (type2 == typeof(PayForPurchaseResult) && _instance.OnPayForPurchaseResultEvent != null)
			{
				_instance.OnPayForPurchaseResultEvent((PayForPurchaseResult)e.Result);
			}
			else if (type2 == typeof(PurchaseItemResult) && _instance.OnPurchaseItemResultEvent != null)
			{
				_instance.OnPurchaseItemResultEvent((PurchaseItemResult)e.Result);
			}
			else if (type2 == typeof(RedeemCouponResult) && _instance.OnRedeemCouponResultEvent != null)
			{
				_instance.OnRedeemCouponResultEvent((RedeemCouponResult)e.Result);
			}
			else if (type2 == typeof(RegisterForIOSPushNotificationResult) && _instance.OnRegisterForIOSPushNotificationResultEvent != null)
			{
				_instance.OnRegisterForIOSPushNotificationResultEvent((RegisterForIOSPushNotificationResult)e.Result);
			}
			else if (type2 == typeof(RegisterPlayFabUserResult) && _instance.OnRegisterPlayFabUserResultEvent != null)
			{
				_instance.OnRegisterPlayFabUserResultEvent((RegisterPlayFabUserResult)e.Result);
			}
			else if (type2 == typeof(RemoveContactEmailResult) && _instance.OnRemoveContactEmailResultEvent != null)
			{
				_instance.OnRemoveContactEmailResultEvent((RemoveContactEmailResult)e.Result);
			}
			else if (type2 == typeof(RemoveFriendResult) && _instance.OnRemoveFriendResultEvent != null)
			{
				_instance.OnRemoveFriendResultEvent((RemoveFriendResult)e.Result);
			}
			else if (type2 == typeof(RemoveGenericIDResult) && _instance.OnRemoveGenericIDResultEvent != null)
			{
				_instance.OnRemoveGenericIDResultEvent((RemoveGenericIDResult)e.Result);
			}
			else if (type2 == typeof(RemoveSharedGroupMembersResult) && _instance.OnRemoveSharedGroupMembersResultEvent != null)
			{
				_instance.OnRemoveSharedGroupMembersResultEvent((RemoveSharedGroupMembersResult)e.Result);
			}
			else if (type2 == typeof(EmptyResult) && _instance.OnReportDeviceInfoResultEvent != null)
			{
				_instance.OnReportDeviceInfoResultEvent((EmptyResult)e.Result);
			}
			else if (type2 == typeof(ReportPlayerClientResult) && _instance.OnReportPlayerResultEvent != null)
			{
				_instance.OnReportPlayerResultEvent((ReportPlayerClientResult)e.Result);
			}
			else if (type2 == typeof(RestoreIOSPurchasesResult) && _instance.OnRestoreIOSPurchasesResultEvent != null)
			{
				_instance.OnRestoreIOSPurchasesResultEvent((RestoreIOSPurchasesResult)e.Result);
			}
			else if (type2 == typeof(SendAccountRecoveryEmailResult) && _instance.OnSendAccountRecoveryEmailResultEvent != null)
			{
				_instance.OnSendAccountRecoveryEmailResultEvent((SendAccountRecoveryEmailResult)e.Result);
			}
			else if (type2 == typeof(SetFriendTagsResult) && _instance.OnSetFriendTagsResultEvent != null)
			{
				_instance.OnSetFriendTagsResultEvent((SetFriendTagsResult)e.Result);
			}
			else if (type2 == typeof(SetPlayerSecretResult) && _instance.OnSetPlayerSecretResultEvent != null)
			{
				_instance.OnSetPlayerSecretResultEvent((SetPlayerSecretResult)e.Result);
			}
			else if (type2 == typeof(StartGameResult) && _instance.OnStartGameResultEvent != null)
			{
				_instance.OnStartGameResultEvent((StartGameResult)e.Result);
			}
			else if (type2 == typeof(StartPurchaseResult) && _instance.OnStartPurchaseResultEvent != null)
			{
				_instance.OnStartPurchaseResultEvent((StartPurchaseResult)e.Result);
			}
			else if (type2 == typeof(ModifyUserVirtualCurrencyResult) && _instance.OnSubtractUserVirtualCurrencyResultEvent != null)
			{
				_instance.OnSubtractUserVirtualCurrencyResultEvent((ModifyUserVirtualCurrencyResult)e.Result);
			}
			else if (type2 == typeof(UnlinkAndroidDeviceIDResult) && _instance.OnUnlinkAndroidDeviceIDResultEvent != null)
			{
				_instance.OnUnlinkAndroidDeviceIDResultEvent((UnlinkAndroidDeviceIDResult)e.Result);
			}
			else if (type2 == typeof(UnlinkCustomIDResult) && _instance.OnUnlinkCustomIDResultEvent != null)
			{
				_instance.OnUnlinkCustomIDResultEvent((UnlinkCustomIDResult)e.Result);
			}
			else if (type2 == typeof(UnlinkFacebookAccountResult) && _instance.OnUnlinkFacebookAccountResultEvent != null)
			{
				_instance.OnUnlinkFacebookAccountResultEvent((UnlinkFacebookAccountResult)e.Result);
			}
			else if (type2 == typeof(UnlinkGameCenterAccountResult) && _instance.OnUnlinkGameCenterAccountResultEvent != null)
			{
				_instance.OnUnlinkGameCenterAccountResultEvent((UnlinkGameCenterAccountResult)e.Result);
			}
			else if (type2 == typeof(UnlinkGoogleAccountResult) && _instance.OnUnlinkGoogleAccountResultEvent != null)
			{
				_instance.OnUnlinkGoogleAccountResultEvent((UnlinkGoogleAccountResult)e.Result);
			}
			else if (type2 == typeof(UnlinkIOSDeviceIDResult) && _instance.OnUnlinkIOSDeviceIDResultEvent != null)
			{
				_instance.OnUnlinkIOSDeviceIDResultEvent((UnlinkIOSDeviceIDResult)e.Result);
			}
			else if (type2 == typeof(UnlinkKongregateAccountResult) && _instance.OnUnlinkKongregateResultEvent != null)
			{
				_instance.OnUnlinkKongregateResultEvent((UnlinkKongregateAccountResult)e.Result);
			}
			else if (type2 == typeof(UnlinkSteamAccountResult) && _instance.OnUnlinkSteamAccountResultEvent != null)
			{
				_instance.OnUnlinkSteamAccountResultEvent((UnlinkSteamAccountResult)e.Result);
			}
			else if (type2 == typeof(UnlinkTwitchAccountResult) && _instance.OnUnlinkTwitchResultEvent != null)
			{
				_instance.OnUnlinkTwitchResultEvent((UnlinkTwitchAccountResult)e.Result);
			}
			else if (type2 == typeof(UnlinkWindowsHelloAccountResponse) && _instance.OnUnlinkWindowsHelloResultEvent != null)
			{
				_instance.OnUnlinkWindowsHelloResultEvent((UnlinkWindowsHelloAccountResponse)e.Result);
			}
			else if (type2 == typeof(UnlockContainerItemResult) && _instance.OnUnlockContainerInstanceResultEvent != null)
			{
				_instance.OnUnlockContainerInstanceResultEvent((UnlockContainerItemResult)e.Result);
			}
			else if (type2 == typeof(UnlockContainerItemResult) && _instance.OnUnlockContainerItemResultEvent != null)
			{
				_instance.OnUnlockContainerItemResultEvent((UnlockContainerItemResult)e.Result);
			}
			else if (type2 == typeof(EmptyResult) && _instance.OnUpdateAvatarUrlResultEvent != null)
			{
				_instance.OnUpdateAvatarUrlResultEvent((EmptyResult)e.Result);
			}
			else if (type2 == typeof(UpdateCharacterDataResult) && _instance.OnUpdateCharacterDataResultEvent != null)
			{
				_instance.OnUpdateCharacterDataResultEvent((UpdateCharacterDataResult)e.Result);
			}
			else if (type2 == typeof(UpdateCharacterStatisticsResult) && _instance.OnUpdateCharacterStatisticsResultEvent != null)
			{
				_instance.OnUpdateCharacterStatisticsResultEvent((UpdateCharacterStatisticsResult)e.Result);
			}
			else if (type2 == typeof(UpdatePlayerStatisticsResult) && _instance.OnUpdatePlayerStatisticsResultEvent != null)
			{
				_instance.OnUpdatePlayerStatisticsResultEvent((UpdatePlayerStatisticsResult)e.Result);
			}
			else if (type2 == typeof(UpdateSharedGroupDataResult) && _instance.OnUpdateSharedGroupDataResultEvent != null)
			{
				_instance.OnUpdateSharedGroupDataResultEvent((UpdateSharedGroupDataResult)e.Result);
			}
			else if (type2 == typeof(UpdateUserDataResult) && _instance.OnUpdateUserDataResultEvent != null)
			{
				_instance.OnUpdateUserDataResultEvent((UpdateUserDataResult)e.Result);
			}
			else if (type2 == typeof(UpdateUserDataResult) && _instance.OnUpdateUserPublisherDataResultEvent != null)
			{
				_instance.OnUpdateUserPublisherDataResultEvent((UpdateUserDataResult)e.Result);
			}
			else if (type2 == typeof(UpdateUserTitleDisplayNameResult) && _instance.OnUpdateUserTitleDisplayNameResultEvent != null)
			{
				_instance.OnUpdateUserTitleDisplayNameResultEvent((UpdateUserTitleDisplayNameResult)e.Result);
			}
			else if (type2 == typeof(ValidateAmazonReceiptResult) && _instance.OnValidateAmazonIAPReceiptResultEvent != null)
			{
				_instance.OnValidateAmazonIAPReceiptResultEvent((ValidateAmazonReceiptResult)e.Result);
			}
			else if (type2 == typeof(ValidateGooglePlayPurchaseResult) && _instance.OnValidateGooglePlayPurchaseResultEvent != null)
			{
				_instance.OnValidateGooglePlayPurchaseResultEvent((ValidateGooglePlayPurchaseResult)e.Result);
			}
			else if (type2 == typeof(ValidateIOSReceiptResult) && _instance.OnValidateIOSReceiptResultEvent != null)
			{
				_instance.OnValidateIOSReceiptResultEvent((ValidateIOSReceiptResult)e.Result);
			}
			else if (type2 == typeof(ValidateWindowsReceiptResult) && _instance.OnValidateWindowsStoreReceiptResultEvent != null)
			{
				_instance.OnValidateWindowsStoreReceiptResultEvent((ValidateWindowsReceiptResult)e.Result);
			}
			else if (type2 == typeof(WriteEventResponse) && _instance.OnWriteCharacterEventResultEvent != null)
			{
				_instance.OnWriteCharacterEventResultEvent((WriteEventResponse)e.Result);
			}
			else if (type2 == typeof(WriteEventResponse) && _instance.OnWritePlayerEventResultEvent != null)
			{
				_instance.OnWritePlayerEventResultEvent((WriteEventResponse)e.Result);
			}
			else if (type2 == typeof(WriteEventResponse) && _instance.OnWriteTitleEventResultEvent != null)
			{
				_instance.OnWriteTitleEventResultEvent((WriteEventResponse)e.Result);
			}
		}
	}
}
