using System;
using UnityEngine;

public class HatchManager : Singleton<HatchManager>
{
	private enum ErrorType
	{
		None = 0,
		SessionNotInitialized = -1,
		IdentityToSessionMigrationError = -2,
		FacebookLoginError = -3,
		PlayFabLoginError = -4,
		NoNetwork = -5,
		PlayerRegisterError = -6,
		RestoreSessionError = -7
	}

	public class HatchPlayer
	{
		public string HatchID { get; private set; }

		public string HatchCustomerID { get; private set; }

		public string PlayFabID { get; private set; }

		public string PlayFabDisplayName { get; private set; }

		public string FacebookToken { get; private set; }

		public string FacebookID { get; private set; }

		public HatchPlayer(string hatchID, string hatchCustomerID)
		{
			HatchID = hatchID;
			HatchCustomerID = hatchCustomerID;
		}

		public void AddPlayFabID(string playFabID)
		{
			PlayFabID = playFabID;
		}

		public void AddPlayFabDisplayName(string playFabDisplayName)
		{
			PlayFabDisplayName = playFabDisplayName;
		}
	}

	private static bool m_initialized;

	private static bool m_isLoggedIn;

	private static bool m_hasLoginError;

	public static Action onLoginSuccess;

	public static Action onLoginFailed;

	public static Action onLogout;

	public static Action onPlayFabLoginSuccess;

	public static bool IsInitialized
	{
		get
		{
			if (Singleton<HatchManager>.instance != null)
			{
				return m_initialized;
			}
			return false;
		}
	}

	public static bool IsLoggedIn
	{
		get
		{
			if (Singleton<HatchManager>.instance != null)
			{
				return m_isLoggedIn;
			}
			return false;
		}
	}

	public static HatchPlayer CurrentPlayer { get; private set; }

	public static bool HasLoginError
	{
		get
		{
			if (!(Singleton<HatchManager>.instance == null))
			{
				return m_hasLoginError;
			}
			return true;
		}
	}

	private void Awake()
	{
		if (Singleton<HatchManager>.IsInstantiated() || m_initialized)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		SetAsPersistant();
		m_initialized = true;
	}

	private void Start()
	{
		Singleton<NetworkManager>.Instance.CheckAccess(OnNetworkAccessChecked);
	}

	private void OnNetworkAccessChecked(bool hasNetwork)
	{
		if (hasNetwork)
		{
			LoginAuto();
		}
		else
		{
			OnLoginError(ErrorType.NoNetwork, "No network connection");
		}
	}

	public static bool IsProductionBuild()
	{
		return Singleton<BuildCustomizationLoader>.Instance.SkynestBackend.Equals("production");
	}

	private void LoginAuto()
	{
		m_isLoggedIn = false;
		InitPlayer(online: true);
	}

	private void OnLoginError(ErrorType errorType, string message)
	{
		InitPlayer(online: false);
		if (errorType == ErrorType.NoNetwork)
		{
			GameProgress.ChangePlayer(PlayerPrefs.GetString("offline_game_progress", string.Empty));
		}
		if (onLoginFailed != null)
		{
			onLoginFailed();
		}
		m_hasLoginError = true;
	}

	private void InitPlayer(bool online)
	{
		if (CurrentPlayer == null)
		{
			CurrentPlayer = new HatchPlayer("HatchID", "HatchCustomerID");
		}
		if (online)
		{
			Singleton<PlayFabManager>.Instance.Logout();
			LoginToPlayFab();
		}
	}

	private void PlayerIsReady()
	{
		if (Singleton<PlayFabManager>.Instance.IsSendingChunkCache)
		{
			EventManager.Connect<PlayFabEvent>(OnPlayFabEvent);
			return;
		}
		GameProgress.ChangePlayer(string.Empty);
		PlayerPrefs.SetString("offline_game_progress", string.Empty);
		m_isLoggedIn = true;
		if (onLoginSuccess != null)
		{
			onLoginSuccess();
		}
	}

	private void LoginToPlayFab()
	{
		PlayFabManager playFabManager = Singleton<PlayFabManager>.Instance;
		playFabManager.OnLogin = (Action<string, string>)Delegate.Combine(playFabManager.OnLogin, new Action<string, string>(OnPlayFabLogin));
		Singleton<PlayFabManager>.Instance.Login(CurrentPlayer);
	}

	private void OnPlayFabLogin(string playFabId, string facebookId)
	{
		PlayFabManager playFabManager = Singleton<PlayFabManager>.Instance;
		playFabManager.OnLogin = (Action<string, string>)Delegate.Remove(playFabManager.OnLogin, new Action<string, string>(OnPlayFabLogin));
		if (!string.IsNullOrEmpty(playFabId))
		{
			if (onPlayFabLoginSuccess != null)
			{
				onPlayFabLoginSuccess();
			}
			CurrentPlayer.AddPlayFabID(playFabId);
			PlayerIsReady();
		}
		else
		{
			OnLoginError(ErrorType.PlayFabLoginError, "Couldn't login to PlayFab");
		}
	}

	private void OnPlayFabEvent(PlayFabEvent data)
	{
		if (data.type == PlayFabEvent.Type.UserDataUploadEnded)
		{
			EventManager.Disconnect<PlayFabEvent>(OnPlayFabEvent);
			PlayerIsReady();
		}
	}
}
