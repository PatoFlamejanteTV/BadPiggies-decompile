using System;
using System.Collections;
using UnityEngine;

public class SnoutCoinAdRewardDialog : TextDialog
{
	private enum State
	{
		Loading,
		Failed,
		Ready,
		Finished,
		Stalled
	}

	private const float TIMEOUT_LENGTH = 10f;

	[SerializeField]
	private GameObject watchAdButton;

	[SerializeField]
	private GameObject loadingIndicator;

	[SerializeField]
	private GameObject failedIndicator;

	[SerializeField]
	private GameObject successIndicator;

	[SerializeField]
	private SpriteText rewardAmountSpriteText;

	private int rewardCount;

	private Button button;

	private State state;

	private string placement;

	protected override void Start()
	{
		base.Start();
		placement = AdvertisementHandler.SnoutCoinRewardVideoPlacement;
		button = watchAdButton.GetComponent<Button>();
		rewardCount = GetSnoutCoinRewardCount();
		if (rewardCount > 0)
		{
			rewardAmountSpriteText.Text = $"x{rewardCount}";
			SetState(State.Loading, forced: true);
		}
		else
		{
			rewardAmountSpriteText.Text = $"x{0}";
			SetState(State.Failed, forced: true);
		}
		base.onOpen += OnOpenResponse;
		base.onClose += OnClosedResponse;
	}

	private void OnOpenResponse()
	{
		if (rewardCount < 0)
		{
			rewardCount = GetSnoutCoinRewardCount();
		}
		if (rewardCount >= 0 && (state == State.Failed || state == State.Finished || state == State.Stalled))
		{
			rewardAmountSpriteText.Text = $"x{rewardCount}";
			SetState(State.Loading);
		}
	}

	private void OnDestroy()
	{
		base.onOpen -= OnOpenResponse;
		base.onClose -= OnClosedResponse;
		if (Singleton<NetworkManager>.IsInstantiated())
		{
			Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(HasInternet);
			Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(PlayRewardVideo);
		}
	}

	private void OnClosedResponse()
	{
		Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(HasInternet);
		Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(PlayRewardVideo);
		SetState(State.Stalled);
	}

	private void SetState(State state, bool forced = false)
	{
		if (state != this.state || forced)
		{
			switch (state)
			{
			case State.Loading:
				OnLoading();
				break;
			case State.Failed:
				OnFailed();
				break;
			case State.Ready:
				OnReady();
				break;
			case State.Stalled:
				OnStalled();
				break;
			}
			this.state = state;
		}
	}

	private void OnFailed()
	{
		failedIndicator.SetActive(value: true);
		successIndicator.SetActive(value: false);
		loadingIndicator.SetActive(value: false);
		button.MethodToCall.Reset();
	}

	private void OnLoading()
	{
		loadingIndicator.SetActive(value: true);
		successIndicator.SetActive(value: false);
		failedIndicator.SetActive(value: false);
		if (Application.internetReachability != 0)
		{
			Singleton<NetworkManager>.Instance.CheckAccess(HasInternet);
		}
		else
		{
			SetState(State.Failed);
		}
	}

	private void OnReady()
	{
		successIndicator.SetActive(value: true);
		loadingIndicator.SetActive(value: false);
		failedIndicator.SetActive(value: false);
		button.MethodToCall.SetMethod(this, "StartRewardVideo");
	}

	private void OnStalled()
	{
		button.MethodToCall.Reset();
	}

	public new void Open()
	{
		base.Open();
	}

	public new void Close()
	{
		base.Close();
	}

	public new void Confirm()
	{
		base.Confirm();
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

	private void RefreshVideo()
	{
		StartCoroutine(CheckVideoTimeout());
	}

	private IEnumerator CheckVideoTimeout()
	{
		float counter = 0f;
		while (counter <= 10f)
		{
			counter += Time.deltaTime;
			if (AdvertisementHandler.IsAdvertisementReady(AdvertisementHandler.SnoutCoinRewardVideoPlacement) || state == State.Stalled)
			{
				break;
			}
			yield return null;
		}
	}

	public void StartRewardVideo()
	{
		Singleton<NetworkManager>.Instance.CheckAccess(PlayRewardVideo);
	}

	private void PlayRewardVideo(bool hasInternet)
	{
		if (!hasInternet)
		{
			SetState(State.Failed);
		}
	}

	private IEnumerator DelayedAction(float delay, Action action)
	{
		if (action != null)
		{
			yield return new WaitForSeconds(delay);
			action();
		}
	}

	private int GetSnoutCoinRewardCount()
	{
		Hashtable productRewards = Singleton<VirtualCatalogManager>.Instance.GetProductRewards("video_ad_reward");
		if (productRewards == null)
		{
			return -1;
		}
		if (!productRewards.ContainsKey("SnoutCoin"))
		{
			return -1;
		}
		if (int.TryParse(productRewards["SnoutCoin"] as string, out var result))
		{
			return result;
		}
		return -1;
	}
}
