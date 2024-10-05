using System;
using UnityEngine;

public class ExtraCoinsRewardButton : MonoBehaviour
{
	public enum Status
	{
		Uninitialized,
		Initialized,
		Error
	}

	[SerializeField]
	private TextMesh[] labels;

	[SerializeField]
	private string stringFormat = "{0}";

	[SerializeField]
	private GameObject loadSpinner;

	[SerializeField]
	private GameObject errorContent;

	[SerializeField]
	private GameObject buttonContent;

	public Action OnAdWatched;

	public Action OnAdFailed;

	public Action<bool> OnAdLoaded;

	public Action OnConfirmationFailed;

	private AdReward adReward;

	private bool hasAd;

	private bool loadingAd;

	private Status status;

	private int rewardAmount;

	private bool initialized;

	private void Awake()
	{
		SetStatus(Status.Uninitialized);
		if (labels != null && labels.Length != 0)
		{
			rewardAmount = Singleton<GameConfigurationManager>.Instance.GetValue<int>("extra_coins_video", "coin_reward");
			string text = string.Format(stringFormat, rewardAmount);
			TextMeshHelper.UpdateTextMeshes(labels, text);
		}
		Initialize();
	}

	private void Initialize()
	{
		if (!initialized)
		{
			adReward = new AdReward(AdvertisementHandler.ExtraCoinsRewardPlacement);
			AdReward obj = adReward;
			obj.OnReady = (Action)Delegate.Combine(obj.OnReady, new Action(OnAdReady));
			AdReward obj2 = adReward;
			obj2.OnFailed = (Action)Delegate.Combine(obj2.OnFailed, new Action(OnAdFailure));
			AdReward obj3 = adReward;
			obj3.OnAdFinished = (Action)Delegate.Combine(obj3.OnAdFinished, new Action(OnAdFinished));
			AdReward obj4 = adReward;
			obj4.OnLoading = (Action)Delegate.Combine(obj4.OnLoading, new Action(OnAdLoading));
			AdReward obj5 = adReward;
			obj5.OnCancel = (Action)Delegate.Combine(obj5.OnCancel, new Action(OnAdCancel));
			AdReward obj6 = adReward;
			obj6.OnConfirmationFailed = (Action)Delegate.Combine(obj6.OnConfirmationFailed, new Action(OnAdConfirmationFailed));
			adReward.Load();
			initialized = true;
		}
	}

	public void OnPress()
	{
		if (status == Status.Initialized)
		{
			adReward.Play();
		}
	}

	public void SetStatus(Status newStatus)
	{
		switch (newStatus)
		{
		case Status.Uninitialized:
			loadSpinner.SetActive(value: true);
			errorContent.SetActive(value: false);
			buttonContent.SetActive(value: false);
			break;
		case Status.Initialized:
			loadSpinner.SetActive(value: false);
			errorContent.SetActive(value: false);
			buttonContent.SetActive(value: true);
			break;
		case Status.Error:
			loadSpinner.SetActive(value: false);
			errorContent.SetActive(value: true);
			buttonContent.SetActive(value: false);
			break;
		}
		status = newStatus;
	}

	private void OnDestroy()
	{
		if (adReward != null)
		{
			adReward.Dispose();
		}
	}

	private void OnAdReady()
	{
		hasAd = true;
		loadingAd = false;
		if (OnAdLoaded != null)
		{
			OnAdLoaded(obj: true);
			OnAdLoaded = null;
		}
		SetStatus((rewardAmount > 0) ? Status.Initialized : Status.Error);
	}

	private void OnAdFailure()
	{
		hasAd = false;
		loadingAd = false;
		if (OnAdFailed != null)
		{
			OnAdFailed();
			OnAdLoaded = null;
		}
		SetStatus(Status.Error);
	}

	private void OnAdFinished()
	{
		hasAd = false;
		if (OnAdWatched != null)
		{
			OnAdWatched();
			OnAdWatched = null;
		}
		if (rewardAmount > 0)
		{
			GameProgress.AddSnoutCoins(rewardAmount);
			base.gameObject.SetActive(value: false);
			GridLayout component = base.transform.parent.GetComponent<GridLayout>();
			if (component != null)
			{
				component.UpdateLayout();
			}
			SnoutButton.Instance.AddParticles(base.gameObject, rewardAmount, 1f / (float)rewardAmount);
			LevelManager.IncentiveVideoShown();
		}
	}

	private void OnAdLoading()
	{
		hasAd = false;
		loadingAd = true;
		SetStatus(Status.Uninitialized);
	}

	private void OnAdCancel()
	{
	}

	private void OnAdConfirmationFailed()
	{
		adReward.Load();
		if (OnConfirmationFailed != null)
		{
			OnConfirmationFailed();
			OnConfirmationFailed = null;
		}
	}
}
