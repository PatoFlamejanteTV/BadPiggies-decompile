using System;
using System.Collections;
using UnityEngine;

public class AdReward : IDisposable
{
	private enum State
	{
		Loading,
		WaitingFail,
		Failed,
		Ready,
		Finished,
		Stalled,
		Cancelled,
		WaitingConfirmation,
		ConfirmationFailed
	}

	public Action OnFailed;

	public Action OnReady;

	public Action OnAdFinished;

	public Action OnLoading;

	public Action OnCancel;

	public Action OnConfirmationFailed;

	public Action OnAdPlayFailed;

	private const float TIMEOUT_LENGTH = 10f;

	private bool disposed;

	private bool wasPaused;

	private bool waitingFailure;

	private string placement;

	private State state;

	public bool Disposed => disposed;

	public AdReward(string placement)
	{
		this.placement = placement;
		SetState(State.Stalled, forced: true);
	}

	~AdReward()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (!disposed)
		{
			OnFailed = null;
			OnReady = null;
			OnAdFinished = null;
			OnLoading = null;
			OnCancel = null;
			OnConfirmationFailed = null;
			if (Singleton<NetworkManager>.IsInstantiated())
			{
				Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(HasInternet);
				Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(PlayRewardVideo);
			}
			disposed = true;
			GC.SuppressFinalize(this);
		}
	}

	public void Stall()
	{
		if (Singleton<NetworkManager>.IsInstantiated())
		{
			Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(HasInternet);
			Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(PlayRewardVideo);
		}
		SetState(State.Stalled);
	}

	public void Play()
	{
		Singleton<GuiManager>.Instance.enabled = false;
		Singleton<NetworkManager>.Instance.CheckAccess(PlayRewardVideo);
	}

	public void Load()
	{
		if (AdvertisementHandler.IsAdvertisementReady(placement))
		{
			SetState(State.Ready);
		}
		else
		{
			SetState(State.Loading);
		}
	}

	private void SetState(State state, bool forced = false)
	{
		if (state == this.state && !forced)
		{
			return;
		}
		this.state = state;
		switch (state)
		{
		case State.Loading:
			if (OnLoading != null)
			{
				OnLoading();
			}
			StartLoading();
			break;
		case State.WaitingFail:
			CoroutineRunner.Instance.StartCoroutine(FailureWait());
			break;
		case State.Failed:
			if (OnFailed != null)
			{
				OnFailed();
			}
			break;
		case State.Ready:
			if (OnReady != null)
			{
				OnReady();
			}
			break;
		case State.Finished:
			if (OnAdFinished != null)
			{
				OnAdFinished();
			}
			break;
		case State.Stalled:
			if (OnFailed != null)
			{
				OnFailed();
			}
			break;
		case State.Cancelled:
			if (OnCancel != null)
			{
				OnCancel();
			}
			break;
		case State.ConfirmationFailed:
			if (OnConfirmationFailed != null)
			{
				OnConfirmationFailed();
			}
			break;
		case State.WaitingConfirmation:
			break;
		}
	}

	private void StartLoading()
	{
		if (Application.internetReachability != 0)
		{
			Singleton<NetworkManager>.Instance.CheckAccess(HasInternet);
		}
		else
		{
			SetState(State.Failed);
		}
	}

	private void HasInternet(bool internet)
	{
		if (state != State.Stalled)
		{
			if (!internet)
			{
				SetState(State.Failed);
			}
			else if (internet && state == State.Loading)
			{
				RefreshVideo();
			}
		}
	}

	public void RefreshVideo()
	{
		CoroutineRunner.Instance.StartCoroutine(CheckVideoTimeout());
	}

	private void PlayRewardVideo(bool canPlay)
	{
		if (canPlay)
		{
			if (OnAdPlayFailed != null)
			{
				OnAdPlayFailed();
			}
		}
		else
		{
			if (OnAdPlayFailed != null)
			{
				OnAdPlayFailed();
			}
			SetState(State.Failed);
		}
		Singleton<GuiManager>.Instance.enabled = true;
	}

	private IEnumerator DelayedAction(float delay, Action action)
	{
		if (action != null)
		{
			yield return new WaitForRealSeconds(delay);
			if (!disposed)
			{
				action();
			}
		}
	}

	private IEnumerator CheckVideoTimeout()
	{
		float counter = 0f;
		while (counter <= 10f)
		{
			counter += Time.unscaledDeltaTime;
			if (AdvertisementHandler.IsAdvertisementReady(placement) || state == State.Stalled || disposed)
			{
				break;
			}
			yield return null;
		}
	}

	private IEnumerator FailureWait()
	{
		float counter = 0f;
		while (counter <= 10f)
		{
			counter += Time.unscaledDeltaTime;
			if (state == State.Stalled || disposed)
			{
				yield break;
			}
			yield return null;
		}
		if (state == State.WaitingFail)
		{
			SetState(State.Failed);
		}
	}
}
