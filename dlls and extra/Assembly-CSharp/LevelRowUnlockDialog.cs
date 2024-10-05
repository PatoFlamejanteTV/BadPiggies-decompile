using System;
using System.Collections;
using UnityEngine;

public class LevelRowUnlockDialog : TextDialog
{
	public Action OnAdFinishedSuccesfully;

	[SerializeField]
	private GameObject enabledAdButton;

	[SerializeField]
	private GameObject disabledAdButton;

	[SerializeField]
	private Button enabledWatchAdButton;

	[SerializeField]
	private Button disabledWatchAdButton;

	[SerializeField]
	private TextMesh watchButtonText;

	[SerializeField]
	private TextMesh loadingIndicator;

	private TextDialog videoNotFoundDialog;

	private AdReward adReward;

	private bool loading;

	private bool hasAd;

	private string placement;

	protected override void Start()
	{
		base.Start();
		adReward = new AdReward(AdvertisementHandler.LevelRewardVideoPlacement);
		AdReward obj = adReward;
		obj.OnAdFinished = (Action)Delegate.Combine(obj.OnAdFinished, new Action(OnAdFinished));
		AdReward obj2 = adReward;
		obj2.OnLoading = (Action)Delegate.Combine(obj2.OnLoading, new Action(OnAdLoading));
		AdReward obj3 = adReward;
		obj3.OnCancel = (Action)Delegate.Combine(obj3.OnCancel, new Action(OnAdCancelled));
		AdReward obj4 = adReward;
		obj4.OnFailed = (Action)Delegate.Combine(obj4.OnFailed, new Action(OnAdFailed));
		AdReward obj5 = adReward;
		obj5.OnReady = (Action)Delegate.Combine(obj5.OnReady, new Action(OnAdReady));
		adReward.Load();
		base.onOpen += delegate
		{
			if (adReward != null)
			{
				adReward.Load();
			}
		};
		base.onClose += delegate
		{
			if (adReward != null)
			{
				adReward.Stall();
			}
		};
		GameObject gameObject = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_videoNotFoundDialog);
		gameObject.transform.position = WPFMonoBehaviour.hudCamera.transform.position + new Vector3(0f, 0f, 1f);
		videoNotFoundDialog = gameObject.GetComponent<TextDialog>();
		videoNotFoundDialog.Close();
	}

	private void OnDestroy()
	{
		if (adReward != null)
		{
			AdReward obj = adReward;
			obj.OnAdFinished = (Action)Delegate.Remove(obj.OnAdFinished, new Action(OnAdFinished));
			AdReward obj2 = adReward;
			obj2.OnLoading = (Action)Delegate.Remove(obj2.OnLoading, new Action(OnAdLoading));
			AdReward obj3 = adReward;
			obj3.OnCancel = (Action)Delegate.Remove(obj3.OnCancel, new Action(OnAdCancelled));
			AdReward obj4 = adReward;
			obj4.OnFailed = (Action)Delegate.Remove(obj4.OnFailed, new Action(OnAdFailed));
			AdReward obj5 = adReward;
			obj5.OnReady = (Action)Delegate.Remove(obj5.OnReady, new Action(OnAdReady));
			adReward.Dispose();
		}
	}

	private void OnAdLoading()
	{
		watchButtonText.gameObject.SetActive(value: false);
		disabledAdButton.SetActive(value: false);
		enabledAdButton.SetActive(value: true);
		enabledWatchAdButton.MethodToCall.Reset();
		disabledWatchAdButton.MethodToCall.Reset();
		loadingIndicator.gameObject.SetActive(value: true);
		loading = true;
		hasAd = false;
		StartCoroutine(LoadingIndicator());
	}

	private void OnAdReady()
	{
		disabledAdButton.SetActive(value: false);
		enabledAdButton.SetActive(value: true);
		watchButtonText.gameObject.SetActive(value: true);
		enabledWatchAdButton.MethodToCall.SetMethod(this, "WatchAd");
		loadingIndicator.gameObject.SetActive(value: false);
		loading = false;
		hasAd = true;
	}

	private void OnAdFailed()
	{
		disabledAdButton.SetActive(value: true);
		enabledAdButton.SetActive(value: false);
		disabledWatchAdButton.MethodToCall.SetMethod(this, "WatchAd");
		loading = false;
		hasAd = false;
	}

	private void OnAdCancelled()
	{
		loading = false;
	}

	private void OnAdFinished()
	{
		if (OnAdFinishedSuccesfully != null)
		{
			OnAdFinishedSuccesfully();
		}
		loading = false;
	}

	public void WatchAd()
	{
		if (hasAd)
		{
			adReward.Play();
		}
		else
		{
			videoNotFoundDialog.Open();
		}
	}

	private IEnumerator LoadingIndicator()
	{
		float wait = 0.6f;
		loadingIndicator.text = string.Empty;
		yield return new WaitForSeconds(wait);
		while (loading)
		{
			if (!loadingIndicator.gameObject.activeInHierarchy || !loading)
			{
				yield break;
			}
			loadingIndicator.text = ".";
			yield return new WaitForSeconds(wait);
			if (!loadingIndicator.gameObject.activeInHierarchy || !loading)
			{
				yield break;
			}
			loadingIndicator.text = "..";
			yield return new WaitForSeconds(wait);
			if (!loadingIndicator.gameObject.activeInHierarchy || !loading)
			{
				yield break;
			}
			loadingIndicator.text = "...";
			yield return new WaitForSeconds(wait);
		}
		loadingIndicator.gameObject.SetActive(value: false);
	}
}
