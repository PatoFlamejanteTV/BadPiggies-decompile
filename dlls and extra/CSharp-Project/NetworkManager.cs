using System;
using System.Collections;
using System.Net;
using System.Threading;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{
	public delegate void OnNetworkChangedDelegate(bool hasNetwork);

	public delegate void OnCheckResponseDelegate(bool hasNetwork);

	public OnNetworkChangedDelegate OnNetworkChange;

	private OnCheckResponseDelegate OnCheckResponse;

	private const float MINIMUM_INTERVAL = 10f;

	private const float UPDATE_INTERVAL = 10f;

	private const float CHECK_TIMEOUT = 15f;

	[SerializeField]
	private bool continousCheck;

	private string ipAddress;

	private string hostName;

	private bool resolvingConnectivity;

	private bool resolvingIpAddress;

	private bool resolvingAddrFailed;

	private bool connected;

	private bool waitingCheck;

	private bool fallbackChecking;

	private bool fallbackCheck;

	private bool hasFocus;

	private float lastCheck;

	private Thread resolveThread;

	private Thread fallbackThread;

	private Action OnFocus;

	private bool HasAddress => !string.IsNullOrEmpty(ipAddress);

	public bool HasNetworkAccess => connected;

	public void Awake()
	{
		resolvingConnectivity = false;
		resolvingIpAddress = false;
		resolvingAddrFailed = false;
		connected = false;
		hasFocus = true;
		lastCheck = -1f;
		resolveThread = null;
		fallbackThread = null;
	}

	public void Start()
	{
		SetAsPersistant();
		hostName = "cloud.rovio.com";
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		hasFocus = focusStatus;
		if (focusStatus)
		{
			waitingCheck = true;
			if (OnFocus != null)
			{
				OnFocus();
				OnFocus = null;
			}
			return;
		}
		try
		{
			if (resolvingIpAddress && resolveThread != null)
			{
				resolveThread.Abort();
			}
			if (fallbackChecking && fallbackThread != null)
			{
				fallbackThread.Abort();
			}
		}
		catch
		{
		}
	}

	public void CheckAccess(OnCheckResponseDelegate OnResponse)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			OnResponse(hasNetwork: false);
		}
		else if (!HasAddress)
		{
			if (!resolvingIpAddress)
			{
				resolveThread = new Thread(ResolveIpAddress);
				resolveThread.Start();
			}
			waitingCheck = true;
			OnCheckResponse = (OnCheckResponseDelegate)Delegate.Combine(OnCheckResponse, OnResponse);
		}
		else if (resolvingConnectivity)
		{
			OnCheckResponse = (OnCheckResponseDelegate)Delegate.Combine(OnCheckResponse, OnResponse);
		}
		else if (Time.realtimeSinceStartup - lastCheck > 10f)
		{
			OnCheckResponse = (OnCheckResponseDelegate)Delegate.Combine(OnCheckResponse, OnResponse);
			StartCoroutine(CheckAccess(15f));
			lastCheck = Time.realtimeSinceStartup;
		}
		else
		{
			OnResponse(connected);
		}
	}

	public void UnsubscribeFromResponse(OnCheckResponseDelegate OnResponse)
	{
		OnCheckResponse = (OnCheckResponseDelegate)Delegate.Remove(OnCheckResponse, OnResponse);
	}

	public void Update()
	{
		if (waitingCheck && HasAddress && !resolvingConnectivity)
		{
			StartCoroutine(CheckAccess(15f));
		}
		else if (waitingCheck && !HasAddress && !resolvingConnectivity && resolvingAddrFailed)
		{
			if (OnCheckResponse != null)
			{
				OnCheckResponse(hasNetwork: false);
			}
			OnCheckResponse = null;
			waitingCheck = false;
		}
		if (continousCheck && !(Time.realtimeSinceStartup - lastCheck < 10f))
		{
			if (HasAddress && !resolvingConnectivity)
			{
				StartCoroutine(CheckAccess(15f));
				lastCheck = Time.realtimeSinceStartup;
			}
			else if (!HasAddress && !resolvingIpAddress && hasFocus)
			{
				resolveThread = new Thread(ResolveIpAddress);
				resolveThread.Start();
			}
		}
	}

	private void ResolveIpAddress()
	{
		resolvingIpAddress = true;
		try
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
			if (hostEntry.AddressList.Length != 0)
			{
				ipAddress = hostEntry.AddressList[0].ToString();
				resolvingAddrFailed = false;
			}
		}
		catch
		{
			resolvingAddrFailed = true;
		}
		finally
		{
			resolvingIpAddress = false;
		}
	}

	private IEnumerator CheckAccess(float timeout)
	{
		_ = Time.realtimeSinceStartup;
		resolvingConnectivity = true;
		Ping ping = new Ping(ipAddress);
		float timeLeft = timeout;
		while (!ping.isDone && timeLeft > 0f)
		{
			yield return null;
			timeLeft -= GameTime.RealTimeDelta;
		}
		if (ping.isDone && (float)ping.time > 0f)
		{
			if (!connected && OnNetworkChange != null)
			{
				OnNetworkChange(hasNetwork: true);
			}
			if (OnCheckResponse != null)
			{
				OnCheckResponse(hasNetwork: true);
			}
			connected = true;
			OnCheckResponse = null;
			resolvingConnectivity = false;
			waitingCheck = false;
		}
		else if (hasFocus)
		{
			fallbackChecking = true;
			StartCoroutine(WaitFallback());
			fallbackThread = new Thread(FallbackResolveAccess);
			fallbackThread.Start();
		}
		else
		{
			OnFocus = (Action)Delegate.Combine(OnFocus, (Action)delegate
			{
				fallbackChecking = true;
				StartCoroutine(WaitFallback());
				fallbackThread = new Thread(FallbackResolveAccess);
				fallbackThread.Start();
			});
		}
		ping.DestroyPing();
	}

	private IEnumerator WaitFallback()
	{
		while (fallbackChecking)
		{
			yield return null;
		}
		resolvingConnectivity = false;
		waitingCheck = false;
		connected = fallbackCheck;
		if (OnCheckResponse != null)
		{
			OnCheckResponse(fallbackCheck);
		}
		if (fallbackCheck != connected && OnNetworkChange != null)
		{
			OnNetworkChange(fallbackCheck);
		}
		OnCheckResponse = null;
	}

	private void FallbackResolveAccess()
	{
		try
		{
			if (Dns.GetHostEntry(hostName).AddressList.Length != 0)
			{
				fallbackCheck = true;
			}
			else
			{
				fallbackCheck = false;
			}
		}
		catch
		{
		}
		finally
		{
			fallbackChecking = false;
		}
	}
}
