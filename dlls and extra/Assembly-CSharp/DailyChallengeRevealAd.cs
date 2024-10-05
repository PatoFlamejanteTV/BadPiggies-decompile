using System;
using System.Collections;
using UnityEngine;

public class DailyChallengeRevealAd : MonoBehaviour
{
	private static int? buttonId;

	private static TextDialog videoNotFoundDialog;

	private static TextDialog confirmationFailedDialog;

	[SerializeField]
	private GameObject adEnabled;

	[SerializeField]
	private GameObject adDisabled;

	[SerializeField]
	private GameObject adWatched;

	[SerializeField]
	private GameObject adLoading;

	[SerializeField]
	private TextMesh loadingIndicator;

	[SerializeField]
	private int challengeIndex;

	[SerializeField]
	private DailyChallengeLoader loaders;

	[SerializeField]
	private TextMesh debugLootCrateLocation;

	private AdReward adReward;

	private bool loading;

	private bool adReady;

	private bool watching;

	private bool initialized;

	private bool checkingNetwork;

	private bool indicatorActive;

	private void Awake()
	{
		buttonId = null;
		adLoading.SetActive(value: false);
		adDisabled.SetActive(value: false);
		adEnabled.SetActive(value: false);
		adWatched.SetActive(value: false);
		if (videoNotFoundDialog == null)
		{
			videoNotFoundDialog = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_videoNotFoundDialog).GetComponent<TextDialog>();
			videoNotFoundDialog.transform.position = WPFMonoBehaviour.hudCamera.transform.position + new Vector3(0f, 0f, 1f);
			videoNotFoundDialog.Close();
		}
	}

	private void OnDestroy()
	{
		if (adReward != null)
		{
			adReward.Dispose();
		}
		if (Singleton<DailyChallenge>.IsInstantiated())
		{
			DailyChallenge instance = Singleton<DailyChallenge>.Instance;
			instance.OnInitialize = (Action)Delegate.Remove(instance.OnInitialize, new Action(Refresh));
			DailyChallenge instance2 = Singleton<DailyChallenge>.Instance;
			instance2.OnDailyChallengeChanged = (Action)Delegate.Remove(instance2.OnDailyChallengeChanged, new Action(Refresh));
		}
		if (Singleton<NetworkManager>.IsInstantiated())
		{
			Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(Initialize);
		}
	}

	private void Initialize(bool hasInternet)
	{
		checkingNetwork = false;
		if (!hasInternet)
		{
			return;
		}
		if (Singleton<DailyChallenge>.Instance.Initialized)
		{
			Refresh();
			DailyChallenge instance = Singleton<DailyChallenge>.Instance;
			instance.OnDailyChallengeChanged = (Action)Delegate.Combine(instance.OnDailyChallengeChanged, new Action(Refresh));
		}
		else
		{
			DailyChallenge instance2 = Singleton<DailyChallenge>.Instance;
			instance2.OnInitialize = (Action)Delegate.Combine(instance2.OnInitialize, new Action(Refresh));
			DailyChallenge instance3 = Singleton<DailyChallenge>.Instance;
			instance3.OnInitialize = (Action)Delegate.Combine(instance3.OnInitialize, (Action)delegate
			{
				DailyChallenge instance4 = Singleton<DailyChallenge>.Instance;
				instance4.OnDailyChallengeChanged = (Action)Delegate.Combine(instance4.OnDailyChallengeChanged, new Action(Refresh));
			});
		}
		initialized = true;
	}

	private void Refresh()
	{
		if (Singleton<DailyChallenge>.Instance.Challenges[challengeIndex].collected)
		{
			adDisabled.SetActive(value: false);
			adEnabled.SetActive(value: false);
			adLoading.SetActive(value: false);
			adWatched.SetActive(value: false);
		}
		else if (Singleton<DailyChallenge>.Instance.Challenges[challengeIndex].revealed)
		{
			EnableLevelButton();
		}
		else
		{
			adWatched.SetActive(value: false);
			InitAdReward();
			PrepareAd();
		}
		debugLootCrateLocation.text = Singleton<DailyChallenge>.Instance.Challenges[challengeIndex].Location;
	}

	private void OnEnable()
	{
		if (!initialized && !checkingNetwork)
		{
			checkingNetwork = true;
			Singleton<NetworkManager>.Instance.CheckAccess(Initialize);
			if (!indicatorActive)
			{
				StartCoroutine(LoadingIndicator());
			}
		}
		else if (initialized)
		{
			Refresh();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		indicatorActive = false;
	}

	private void InitAdReward()
	{
		if (adReward == null || adReward.Disposed)
		{
			adReward = new AdReward(AdvertisementHandler.DailyChallengeRevealPlacement);
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
			AdReward obj6 = adReward;
			obj6.OnConfirmationFailed = (Action)Delegate.Combine(obj6.OnConfirmationFailed, new Action(OnConfirmationFailed));
		}
	}

	private void PrepareAd()
	{
		if (Singleton<DailyChallenge>.Instance.IsLocationRevealed(challengeIndex))
		{
			EnableLevelButton();
		}
		else
		{
			adReward.Load();
		}
	}

	private void EnableLevelButton()
	{
		adDisabled.SetActive(value: false);
		adEnabled.SetActive(value: false);
		adLoading.SetActive(value: false);
		Singleton<DailyChallenge>.Instance.SetLocationRevealed(challengeIndex);
		if (Singleton<DailyChallenge>.Instance.DailyChallengeCollected(challengeIndex))
		{
			adWatched.SetActive(value: false);
		}
		else if (!Singleton<DailyChallenge>.Instance.DailyChallengeCollected(challengeIndex) && loaders.ImageReady)
		{
			adWatched.SetActive(value: true);
		}
		else
		{
			adWatched.SetActive(value: false);
			DailyChallengeLoader dailyChallengeLoader = loaders;
			dailyChallengeLoader.OnImageReady = (Action)Delegate.Combine(dailyChallengeLoader.OnImageReady, (Action)delegate
			{
				if (adWatched != null)
				{
					adWatched.SetActive(value: true);
				}
			});
		}
		debugLootCrateLocation.transform.parent.gameObject.SetActive(!Singleton<DailyChallenge>.Instance.DailyChallengeCollected(challengeIndex));
		debugLootCrateLocation.text = Singleton<DailyChallenge>.Instance.Challenges[challengeIndex].Location;
	}

	public void WatchAd()
	{
		if (adReady)
		{
			buttonId = GetInstanceID();
			watching = true;
			adReward.Play();
		}
		else if (!adLoading)
		{
			adReward.Load();
		}
	}

	private void OnAdReady()
	{
		if (Singleton<DailyChallenge>.Instance.IsLocationRevealed(challengeIndex))
		{
			EnableLevelButton();
			return;
		}
		adEnabled.SetActive(value: true);
		adLoading.SetActive(value: false);
		adDisabled.SetActive(value: false);
		loading = false;
		adReady = true;
	}

	private void OnAdFailed()
	{
		if (Singleton<DailyChallenge>.Instance.IsLocationRevealed(challengeIndex))
		{
			EnableLevelButton();
			return;
		}
		if (watching && videoNotFoundDialog != null)
		{
			videoNotFoundDialog.Open();
		}
		adEnabled.SetActive(value: false);
		watching = false;
	}

	private void OnAdCancelled()
	{
		watching = false;
	}

	private void OnAdLoading()
	{
		if (Singleton<DailyChallenge>.Instance.IsLocationRevealed(challengeIndex))
		{
			EnableLevelButton();
			return;
		}
		adLoading.SetActive(value: true);
		adEnabled.SetActive(value: false);
		adDisabled.SetActive(value: false);
		adWatched.SetActive(value: false);
		adReady = false;
		loading = true;
		watching = false;
		if (base.gameObject.activeInHierarchy && !indicatorActive)
		{
			StartCoroutine(LoadingIndicator());
		}
	}

	private void OnAdFinished()
	{
		if (buttonId == GetInstanceID())
		{
			adReward.Dispose();
			EnableLevelButton();
			Singleton<DailyChallenge>.Instance.SetLocationAdRevealed(challengeIndex);
		}
		else
		{
			InitAdReward();
			PrepareAd();
		}
		watching = false;
	}

	private void OnConfirmationFailed()
	{
		watching = false;
		if (buttonId == GetInstanceID())
		{
			if (confirmationFailedDialog == null)
			{
				confirmationFailedDialog = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_confirmationFailedDialog).GetComponent<TextDialog>();
				confirmationFailedDialog.transform.position = WPFMonoBehaviour.hudCamera.transform.position;
				confirmationFailedDialog.transform.position += Vector3.forward;
				confirmationFailedDialog.Close();
			}
			confirmationFailedDialog.Open();
			PrepareAd();
		}
	}

	private IEnumerator LoadingIndicator()
	{
		indicatorActive = true;
		loadingIndicator.gameObject.SetActive(value: true);
		float wait = 0.6f;
		loadingIndicator.text = string.Empty;
		yield return new WaitForRealSeconds(wait);
		while ((loading || checkingNetwork) && loadingIndicator.gameObject.activeInHierarchy && (loading || checkingNetwork))
		{
			loadingIndicator.text = ".";
			yield return new WaitForRealSeconds(wait);
			if (!loadingIndicator.gameObject.activeInHierarchy || (!loading && !checkingNetwork))
			{
				break;
			}
			loadingIndicator.text = "..";
			yield return new WaitForRealSeconds(wait);
			if (!loadingIndicator.gameObject.activeInHierarchy || (!loading && !checkingNetwork))
			{
				break;
			}
			loadingIndicator.text = "...";
			yield return new WaitForRealSeconds(wait);
		}
		loadingIndicator.gameObject.SetActive(value: false);
		indicatorActive = false;
	}
}
