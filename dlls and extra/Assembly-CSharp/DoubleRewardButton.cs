using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class DoubleRewardButton : WPFMonoBehaviour
{
	[SerializeField]
	private GameObject starOne;

	[SerializeField]
	private GameObject starTwo;

	[SerializeField]
	private GameObject starThree;

	[SerializeField]
	[FormerlySerializedAs("animation")]
	private SkeletonAnimation sklAnimation;

	[SerializeField]
	private int maxCharacterInLine;

	[SerializeField]
	private int maxKanjiCharactersInLine;

	[SerializeField]
	private GameObject unlockGo;

	private Collider[] colliders;

	private GameObject enabledIcon;

	private GameObject disabledIcon;

	private GameObject texts;

	private GameObject button;

	private GameObject snoutCoins;

	private TextDialog videoNotFoundDialog;

	private TextDialog confirmationFailedDialog;

	private LevelComplete levelComplete;

	private bool isWatchingAd;

	public LevelComplete LevelComplete
	{
		get
		{
			return levelComplete;
		}
		set
		{
			levelComplete = value;
		}
	}

	private bool CanShowDisabledButton()
	{
		return !Singleton<BuildCustomizationLoader>.Instance.IsOdyssey;
	}

	private void Awake()
	{
		enabledIcon = base.transform.Find("Button/EnabledIcon").gameObject;
		disabledIcon = base.transform.Find("Button/DisabledIcon").gameObject;
		texts = base.transform.Find("Button/Texts").gameObject;
		button = base.transform.Find("Button/Button").gameObject;
		snoutCoins = base.transform.Find("Button/SnoutCoins").gameObject;
		colliders = GetComponentsInChildren<Collider>();
		GameObject gameObject = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_videoNotFoundDialog);
		videoNotFoundDialog = gameObject.GetComponent<TextDialog>();
		videoNotFoundDialog.transform.position = WPFMonoBehaviour.hudCamera.transform.position + new Vector3(0f, 0f, 1f);
		videoNotFoundDialog.Close();
		SetRewardText(snoutCoins.transform);
		Disable();
	}

	private void Start()
	{
		Localize(texts.transform);
	}

	private void OnDestroy()
	{
		if (Singleton<DoubleRewardManager>.IsInstantiated())
		{
			DoubleRewardManager instance = Singleton<DoubleRewardManager>.Instance;
			instance.OnInitialize = (Action)Delegate.Remove(instance.OnInitialize, new Action(Enable));
			DoubleRewardManager instance2 = Singleton<DoubleRewardManager>.Instance;
			instance2.OnAdWatched = (Action)Delegate.Remove(instance2.OnAdWatched, new Action(DoubleRewardAdWatched));
			DoubleRewardManager instance3 = Singleton<DoubleRewardManager>.Instance;
			instance3.OnAdLoaded = (Action<bool>)Delegate.Remove(instance3.OnAdLoaded, new Action<bool>(AdLoadedResponse));
			DoubleRewardManager instance4 = Singleton<DoubleRewardManager>.Instance;
			instance4.OnAdFailed = (Action)Delegate.Remove(instance4.OnAdFailed, new Action(AdFailed));
			DoubleRewardManager instance5 = Singleton<DoubleRewardManager>.Instance;
			instance5.OnConfirmationFailed = (Action)Delegate.Remove(instance5.OnConfirmationFailed, new Action(OnConfirmationFailed));
		}
	}

	private void Localize(Transform texts)
	{
		if (texts == null)
		{
			return;
		}
		TextMesh[] componentsInChildren = texts.GetComponentsInChildren<TextMesh>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			TextMeshLocale component = componentsInChildren[i].GetComponent<TextMeshLocale>();
			if (!(component == null))
			{
				component.RefreshTranslation(componentsInChildren[i].text);
				component.enabled = false;
				componentsInChildren[i].text = string.Format(componentsInChildren[i].text, Singleton<DoubleRewardManager>.Instance.FormattedRewardTime);
				Wrap(componentsInChildren[i]);
			}
		}
	}

	private void SetRewardText(Transform texts)
	{
		TextMesh[] componentsInChildren = texts.GetComponentsInChildren<TextMesh>();
		string text = $"+{Singleton<DoubleRewardManager>.Instance.RewardCoins}";
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].text = text;
		}
	}

	private void Wrap(TextMesh target)
	{
		if (TextMeshHelper.UsesKanjiCharacters())
		{
			TextMeshHelper.Wrap(target, maxKanjiCharactersInLine);
		}
		else
		{
			TextMeshHelper.Wrap(target, maxCharacterInLine);
		}
	}

	public void Show()
	{
		if (Singleton<DoubleRewardManager>.Instance.CurrentStatus == DoubleRewardManager.Status.Initialized)
		{
			Enable();
			return;
		}
		DoubleRewardManager instance = Singleton<DoubleRewardManager>.Instance;
		instance.OnInitialize = (Action)Delegate.Combine(instance.OnInitialize, new Action(Enable));
	}

	private void Enable()
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
		if (!Singleton<DoubleRewardManager>.Instance.HasDoubleReward)
		{
			if (Singleton<DoubleRewardManager>.Instance.HasAd)
			{
				Singleton<NetworkManager>.Instance.CheckAccess(OnNetworkCheck);
			}
			else if (Singleton<DoubleRewardManager>.Instance.LoadingAd)
			{
				SetRenderers(texts, mode: false);
				SetRenderers(enabledIcon, mode: false);
				SetRenderers(snoutCoins, mode: false);
				SetRenderers(disabledIcon, mode: false);
				SetRenderers(button, mode: false);
				DoubleRewardManager instance = Singleton<DoubleRewardManager>.Instance;
				instance.OnAdLoaded = (Action<bool>)Delegate.Combine(instance.OnAdLoaded, new Action<bool>(AdLoadedResponse));
			}
			else
			{
				SetRenderers(texts, mode: false);
				SetRenderers(enabledIcon, mode: false);
				SetRenderers(snoutCoins, mode: false);
				SetRenderers(disabledIcon, mode: false);
				SetRenderers(button, mode: false);
				DoubleRewardManager instance2 = Singleton<DoubleRewardManager>.Instance;
				instance2.OnAdLoaded = (Action<bool>)Delegate.Combine(instance2.OnAdLoaded, new Action<bool>(AdLoadedResponse));
				Singleton<DoubleRewardManager>.Instance.RefreshAd();
			}
		}
	}

	private void OnNetworkCheck(bool hasInternet)
	{
		if (hasInternet && HatchManager.IsLoggedIn && this != null)
		{
			StartCoroutine(ShowAnimation());
		}
	}

	private void SetRenderers(GameObject target, bool mode)
	{
		if (!(target == null))
		{
			Renderer[] componentsInChildren = target.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = mode;
			}
		}
	}

	private void AdLoadedResponse(bool success)
	{
		DoubleRewardManager instance = Singleton<DoubleRewardManager>.Instance;
		instance.OnAdLoaded = (Action<bool>)Delegate.Remove(instance.OnAdLoaded, new Action<bool>(AdLoadedResponse));
		Enable();
	}

	private void Disable()
	{
		SetRenderers(texts, mode: false);
		SetRenderers(enabledIcon, mode: false);
		SetRenderers(snoutCoins, mode: false);
		SetRenderers(disabledIcon, mode: false);
		SetRenderers(button, mode: false);
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
	}

	public void WatchDoubleRewardAd()
	{
		if (isWatchingAd)
		{
			return;
		}
		if (Singleton<DoubleRewardManager>.Instance.HasAd)
		{
			isWatchingAd = true;
			DoubleRewardManager instance = Singleton<DoubleRewardManager>.Instance;
			instance.OnAdWatched = (Action)Delegate.Combine(instance.OnAdWatched, new Action(DoubleRewardAdWatched));
			DoubleRewardManager instance2 = Singleton<DoubleRewardManager>.Instance;
			instance2.OnAdFailed = (Action)Delegate.Combine(instance2.OnAdFailed, new Action(AdFailed));
			DoubleRewardManager instance3 = Singleton<DoubleRewardManager>.Instance;
			instance3.OnConfirmationFailed = (Action)Delegate.Combine(instance3.OnConfirmationFailed, new Action(OnConfirmationFailed));
			Singleton<DoubleRewardManager>.Instance.PlayAd();
			if (unlockGo != null)
			{
				unlockGo.SetActive(value: true);
			}
		}
		else
		{
			AdFailed();
		}
	}

	public void UnlockUI()
	{
		GameTime.Pause(pause: false);
		Singleton<GuiManager>.Instance.enabled = true;
		isWatchingAd = false;
		if (unlockGo != null)
		{
			unlockGo.SetActive(value: false);
		}
	}

	private void AdFailed()
	{
		if (!confirmationFailedDialog || !confirmationFailedDialog.IsOpen)
		{
			videoNotFoundDialog.Open();
		}
		snoutCoins.SetActive(value: false);
		texts.SetActive(value: false);
		enabledIcon.SetActive(value: false);
		button.SetActive(value: false);
		disabledIcon.SetActive(value: false);
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
		UnlockUI();
	}

	private void OnConfirmationFailed()
	{
		isWatchingAd = false;
		if (confirmationFailedDialog == null)
		{
			confirmationFailedDialog = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_confirmationFailedDialog).GetComponent<TextDialog>();
			confirmationFailedDialog.transform.position = WPFMonoBehaviour.hudCamera.transform.position;
			confirmationFailedDialog.transform.position += Vector3.forward;
			confirmationFailedDialog.Close();
		}
		if (!videoNotFoundDialog || !videoNotFoundDialog.IsOpen)
		{
			confirmationFailedDialog.Open();
		}
		Enable();
	}

	private void DoubleRewardAdWatched()
	{
		int value = Singleton<GameConfigurationManager>.Instance.GetValue<int>("level_complete_snout_reward", "One");
		int value2 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("level_complete_snout_reward", "Two");
		int value3 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("level_complete_snout_reward", "Three");
		int num = 0;
		num += Singleton<DoubleRewardManager>.Instance.RewardCoins;
		SnoutButton.Instance.AddParticles(base.gameObject, Singleton<DoubleRewardManager>.Instance.RewardCoins);
		if ((bool)levelComplete && (levelComplete.CoinsCollectedNow & LevelComplete.CoinsCollected.Challenge1) == LevelComplete.CoinsCollected.Challenge1)
		{
			SnoutButton.Instance.AddParticles(starOne, value);
			num += value;
		}
		if ((bool)levelComplete && (levelComplete.CoinsCollectedNow & LevelComplete.CoinsCollected.Challenge2) == LevelComplete.CoinsCollected.Challenge2)
		{
			SnoutButton.Instance.AddParticles(starTwo, value2);
			num += value2;
		}
		if ((bool)levelComplete && (levelComplete.CoinsCollectedNow & LevelComplete.CoinsCollected.Challenge3) == LevelComplete.CoinsCollected.Challenge3)
		{
			SnoutButton.Instance.AddParticles(starThree, value3);
			num += value3;
		}
		GameProgress.AddSnoutCoins(num);
		UnlockUI();
		Disable();
		DoubleRewardManager instance = Singleton<DoubleRewardManager>.Instance;
		instance.OnAdWatched = (Action)Delegate.Remove(instance.OnAdWatched, new Action(DoubleRewardAdWatched));
		DoubleRewardManager instance2 = Singleton<DoubleRewardManager>.Instance;
		instance2.OnAdFailed = (Action)Delegate.Remove(instance2.OnAdFailed, new Action(AdFailed));
		isWatchingAd = false;
	}

	private IEnumerator UpdateText(TextMesh target, Func<string> update)
	{
		target.text = update();
		Wrap(target);
		yield return new WaitForSeconds(0.1f);
	}

	private IEnumerator ShowAnimation()
	{
		if (sklAnimation != null)
		{
			sklAnimation.state.SetAnimation(0, "Intro1", loop: false);
		}
		yield return null;
		SetRenderers(texts, mode: true);
		SetRenderers(enabledIcon, mode: true);
		SetRenderers(snoutCoins, mode: true);
		SetRenderers(disabledIcon, mode: false);
		SetRenderers(button, mode: true);
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = true;
		}
	}
}
