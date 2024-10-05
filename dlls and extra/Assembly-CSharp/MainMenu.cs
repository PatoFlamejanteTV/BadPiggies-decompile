using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject mainMenuNode;

	[SerializeField]
	private GameObject creditsMenu;

	private GameObject creditsMenuInstance;

	[SerializeField]
	private GameObject m_confirmationPopup;

	[SerializeField]
	private GameObject m_PromoPopupPrefab;

	[SerializeField]
	private GameObject m_ShopDialog;

	[SerializeField]
	private GameObject m_ToonsDialog;

	[SerializeField]
	private GameObject m_settingsPopup;

	[SerializeField]
	private GameObject m_pigRescueRewardPrefab;

	[SerializeField]
	private GameObject m_crossPromotionOverlay;

	[SerializeField]
	private Button m_confirmationIAPRestoreSettingsButton;

	[SerializeField]
	private UnityEngine.Animation gearButtonAnimation;

	[SerializeField]
	private PlayerLevelRequirement cakeRaceRequirement;

	[SerializeField]
	private GameObject lockScreen;

	[SerializeField]
	private GameObject cakeRaceLockScreen;

	[SerializeField]
	private AnimatedButton cakeRaceButton;

	[SerializeField]
	private GameObject optionsNotConnectedBubble;

	private CakeRaceLockedDialog cakeRaceLockedDialog;

	private GameObject gcButton;

	private GameObject shopButton;

	private bool bSocialButtonsOut;

	private bool bInfoButtonsOut;

	private static bool s_optionsBubbleShowed;

	[SerializeField]
	private GameObject m_socialButton;

	[SerializeField]
	private GameObject m_infoButton;

	[SerializeField]
	private GameObject m_iapButton;

	public Transform toonsButton;

	public Transform startLayout;

	[SerializeField]
	private GameObject m_xboxLayout;

	private static bool isGameHallExitOpened;

	private bool CakeRaceIntroShown
	{
		get
		{
			return GameProgress.GetBool("CakeRaceIntroShown");
		}
		set
		{
			GameProgress.SetBool("CakeRaceIntroShown", value);
		}
	}

	private void Awake()
	{
		isGameHallExitOpened = false;
		shopButton = GameObject.Find("MainShopButton");
		m_crossPromotionOverlay = UnityEngine.Object.Instantiate(m_crossPromotionOverlay);
		m_crossPromotionOverlay.SetActive(value: false);
		if (m_settingsPopup != null)
		{
			m_settingsPopup.SetActive(value: false);
		}
		if (Singleton<BuildCustomizationLoader>.Instance.IAPEnabled)
		{
			if (GameProgress.GetSandboxUnlocked("S-F"))
			{
				m_iapButton.SetActive(value: false);
			}
		}
		else
		{
			m_iapButton.SetActive(value: false);
		}
		AnimatedButton animatedButton = cakeRaceButton;
		animatedButton.OnOpenAnimationEvent = (Action<Spine.Event>)Delegate.Combine(animatedButton.OnOpenAnimationEvent, new Action<Spine.Event>(OnCakeRaceUnlockAnimationEvent));
		HatchManager.onLoginSuccess = (Action)Delegate.Combine(HatchManager.onLoginSuccess, new Action(HideLockScreen));
		HatchManager.onLoginFailed = (Action)Delegate.Combine(HatchManager.onLoginFailed, new Action(HideLockScreen));
		HatchManager.onLogout = (Action)Delegate.Combine(HatchManager.onLogout, new Action(LoggedOut));
	}

	private void HideLockScreen()
	{
		lockScreen.SetActive(value: false);
	}

	private void LoggedOut()
	{
		lockScreen.SetActive(value: true);
		lockScreen.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 0.5f;
	}

	private void CheckRewardCallback(string rewardData)
	{
		RewardPigRescuePopup.ProcessReward(rewardData);
		if (RewardPigRescuePopup.HasRewardPending)
		{
			ShowRewardPopup();
		}
	}

	private void ShowRewardPopup()
	{
		UnityEngine.Object.Instantiate(m_pigRescueRewardPrefab).transform.position = -Vector3.forward * 20f;
	}

	private void OnEnable()
	{
		KeyListener.keyReleased += HandleKeyListenerKeyReleased;
		if (GameProgress.HasKey("CakeRaceUnlockShown") && !GameProgress.GetBool("CakeRaceUnlockShown"))
		{
			ForceCakeRaceButton();
		}
	}

	private void OnDisable()
	{
		KeyListener.keyReleased -= HandleKeyListenerKeyReleased;
	}

	public bool IsUserInMainMenu()
	{
		if (!mainMenuNode.activeInHierarchy)
		{
			return false;
		}
		GameObject gameObject = GameObject.Find("StartButton");
		Vector3 position = gameObject.transform.position;
		Vector3 position2 = Singleton<GuiManager>.Instance.FindCamera().transform.position;
		if (Physics.Raycast(position2, (position - position2) * 1.1f, out var hitInfo))
		{
			return hitInfo.collider.gameObject == gameObject;
		}
		return false;
	}

	private void HandleKeyListenerKeyReleased(KeyCode keyCode)
	{
		if (keyCode == KeyCode.Escape)
		{
			if (m_crossPromotionOverlay != null && m_crossPromotionOverlay.activeInHierarchy)
			{
				m_crossPromotionOverlay.GetComponent<CrossPromotionOverlay>().CloseDialog();
			}
			else if (creditsMenuInstance != null && creditsMenuInstance.activeInHierarchy)
			{
				CloseCredits();
			}
			else if (m_confirmationPopup != null && m_confirmationPopup.activeInHierarchy)
			{
				m_confirmationPopup.GetComponent<ConfirmationPopup>().DismissDialog();
			}
			else if (Singleton<SocialGameManager>.IsInstantiated() && Singleton<SocialGameManager>.Instance.ViewsActive())
			{
				Singleton<SocialGameManager>.Instance.CloseViews();
			}
			else if (mainMenuNode != null && mainMenuNode.activeInHierarchy)
			{
				OpenConfirmationPopup();
			}
		}
		if (keyCode == KeyCode.Return && m_confirmationPopup != null && m_confirmationPopup.activeInHierarchy)
		{
			Application.Quit();
		}
	}

	private void ShowGameCenterButton(bool show)
	{
		if (!gcButton)
		{
			return;
		}
		bool num = gcButton.GetComponent<Collider>().enabled;
		if (show)
		{
			gcButton.SetActive(DeviceInfo.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios);
		}
		gcButton.GetComponent<Renderer>().enabled = show || Singleton<SocialGameManager>.Instance.Authenticated;
		gcButton.GetComponent<Collider>().enabled = show || Singleton<SocialGameManager>.Instance.Authenticated;
		if (num != gcButton.GetComponent<Collider>().enabled)
		{
			GameObject gameObject = GameObject.Find("InfoButtons");
			if (gameObject != null && bInfoButtonsOut)
			{
				gameObject.GetComponent<UnityEngine.Animation>().Play();
			}
		}
	}

	private void InitButtons(DeviceInfo.DeviceFamily platform)
	{
		GameObject gameObject = GameObject.Find("HDBadge");
		if (gameObject != null)
		{
			gameObject.SetActive(Singleton<BuildCustomizationLoader>.Instance.IsHDVersion && platform != DeviceInfo.DeviceFamily.Pc);
		}
		if (Singleton<SocialGameManager>.IsInstantiated())
		{
			ShowGameCenterButton(Singleton<SocialGameManager>.Instance.Authenticated);
		}
		if ((bool)shopButton)
		{
			shopButton.SetActive(Singleton<BuildCustomizationLoader>.Instance.IAPEnabled);
		}
	}

	private void OnDestroy()
	{
		GameCenterManager.onAuthenticationSucceeded -= ShowGameCenterButton;
		HatchManager.onLoginSuccess = (Action)Delegate.Remove(HatchManager.onLoginSuccess, new Action(HideLockScreen));
		HatchManager.onLoginFailed = (Action)Delegate.Remove(HatchManager.onLoginFailed, new Action(HideLockScreen));
		HatchManager.onLogout = (Action)Delegate.Remove(HatchManager.onLogout, new Action(LoggedOut));
	}

	private void Start()
	{
		gcButton = GameObject.Find("GameCenterButton");
		GameCenterManager.onAuthenticationSucceeded += ShowGameCenterButton;
		EventManager.Connect<PlayerChangedEvent>(OnPlayerChanged);
		creditsMenuInstance = UnityEngine.Object.Instantiate(creditsMenu);
		creditsMenuInstance.SetActive(value: false);
		InitButtons(DeviceInfo.ActiveDeviceFamily);
		if (!HatchManager.IsInitialized || DeviceInfo.ActiveDeviceFamily == DeviceInfo.DeviceFamily.WP8)
		{
			Transform obj = mainMenuNode.transform.FindChildRecursively("ToonsTvLayout");
			Transform transform = obj.Find("ToonsButton");
			Transform obj2 = obj.Find("RuffleButton");
			transform.gameObject.SetActive(value: false);
			obj2.gameObject.SetActive(value: false);
			obj.GetComponent<FlowLayout>().Layout();
		}
	}

	private void OnPlayerChanged(PlayerChangedEvent data)
	{
		if (creditsMenuInstance != null)
		{
			UnityEngine.Object.Destroy(creditsMenuInstance);
		}
		creditsMenuInstance = UnityEngine.Object.Instantiate(creditsMenu);
		creditsMenuInstance.SetActive(value: false);
	}

	private void OnUpdateButtonPressed()
	{
		Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.BadPiggiesAppStore);
		Application.Quit();
	}

	public void OnToonsNewContentAvailable(int numOfNewItems)
	{
		string text = numOfNewItems.ToString();
		Transform obj = mainMenuNode.transform.FindChildRecursively("ToonsButton");
		Transform transform = obj.Find("Badge");
		if (obj != null)
		{
			transform.Find("Count").GetComponent<TextMesh>().text = text;
			transform.Find("CountShadow").GetComponent<TextMesh>().text = text;
			transform.GetComponent<Panel>().width = 2 + ((text.Length > 2) ? (text.Length / 2) : 0);
			transform.gameObject.SetActive(numOfNewItems > 0);
		}
	}

	public void ConnectShopToRestoreConfirmButton(Shop shop)
	{
		if (m_confirmationIAPRestoreSettingsButton != null)
		{
			m_confirmationIAPRestoreSettingsButton.MethodToCall.SetMethod(shop, "RestorePurchasedItems");
		}
	}

	public void OpenSandboxIAP()
	{
		Singleton<GameManager>.Instance.LoadSandboxLevelSelectionAndOpenIapMenu();
	}

	public void OpenUnlockFullVersionIAP()
	{
		Singleton<IapManager>.Instance.EnableUnlockFullVersionPurchasePage();
	}

	public void OpenLevelMenu()
	{
		if (!isGameHallExitOpened)
		{
			SendStartFlurryEvent();
			Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: false);
		}
	}

	public void OpenCakeRaceMenu()
	{
		if (isGameHallExitOpened)
		{
			return;
		}
		if (cakeRaceRequirement.IsLocked)
		{
			if (cakeRaceLockedDialog == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_cakeRaceLockedPopup);
				gameObject.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 2f;
				cakeRaceLockedDialog = gameObject.GetComponent<CakeRaceLockedDialog>();
			}
			cakeRaceLockedDialog.Open();
			cakeRaceLockedDialog.SetLevelRequirement(cakeRaceRequirement.RequiredLevel);
		}
		else if (CakeRaceIntroShown)
		{
			Singleton<GameManager>.Instance.LoadCakeRaceMenu();
		}
		else if (!CakeRaceIntroShown)
		{
			CakeRaceIntroShown = true;
			Singleton<Loader>.Instance.LoadLevel("CakeRaceIntro", GameManager.GameState.Cutscene, showLoadingScreen: true);
		}
	}

	[ContextMenu("Show cake race unlock")]
	public void ForceCakeRaceButton()
	{
		cakeRaceLockScreen.SetActive(value: true);
		cakeRaceButton.transform.parent = null;
		cakeRaceButton.transform.position = new Vector3(cakeRaceButton.transform.position.x, cakeRaceButton.transform.position.y, cakeRaceLockScreen.transform.position.z - 1f);
		UnlockCakeRaceButton();
		CreateCakeRaceButtonTutorial();
	}

	public void UnlockCakeRaceButton()
	{
		cakeRaceButton.UnlockSequence(forcePlayUnlock: true);
	}

	private void OnCakeRaceUnlockAnimationEvent(Spine.Event e)
	{
		string text = e.Data.Name;
		if (text != null && text == "LockBreak")
		{
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.jokerLevelUnlocked);
		}
	}

	private void CreateCakeRaceButtonTutorial()
	{
		GameObject pointer = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_tutorialPointer);
		GameObject gameObject = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_tutorialPointerClick);
		gameObject.SetActive(value: false);
		Tutorial.PointerTimeLine pointerTimeLine = new Tutorial.PointerTimeLine(new Tutorial.Pointer(pointer, gameObject));
		List<Vector3> list = new List<Vector3>();
		list.Add(cakeRaceButton.transform.position + 21f * Vector3.down + Vector3.back);
		list.Add(cakeRaceButton.transform.position + 5f * Vector3.up + Vector3.back);
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.1f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Move(list, 2.5f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Press());
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.5f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Release());
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.75f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Hide());
		StartCoroutine(UpdateTimeline(pointerTimeLine));
	}

	private IEnumerator UpdateTimeline(Tutorial.PointerTimeLine timeline)
	{
		timeline.Start();
		while (true)
		{
			if (timeline.IsFinished())
			{
				timeline.Start();
			}
			timeline.Update();
			yield return null;
		}
	}

	public void CloseCredits()
	{
		if (!isGameHallExitOpened)
		{
			mainMenuNode.SetActive(value: true);
			creditsMenuInstance.SetActive(value: false);
			if (Singleton<SocialGameManager>.IsInstantiated())
			{
				ShowGameCenterButton(Singleton<SocialGameManager>.Instance.Authenticated);
			}
		}
	}

	public void OpenCredits()
	{
		if (!isGameHallExitOpened)
		{
			m_settingsPopup.SetActive(value: false);
			mainMenuNode.SetActive(value: false);
			creditsMenuInstance.SetActive(value: true);
		}
	}

	public void OpenGameCenter()
	{
		if (!isGameHallExitOpened && DeviceInfo.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Ios)
		{
			if (Singleton<SocialGameManager>.Instance.Authenticated)
			{
				Singleton<SocialGameManager>.Instance.ShowAchievementsView();
			}
			else
			{
				Singleton<SocialGameManager>.Instance.Authenticate();
			}
		}
	}

	public void SocialButtonReset()
	{
		if (m_socialButton != null)
		{
			Transform transform = m_socialButton.transform.Find("BackgroundSocialBtn");
			if ((bool)m_socialButton && (bool)transform)
			{
				transform.Find("Arrow").localRotation = Quaternion.identity;
				bSocialButtonsOut = false;
				m_socialButton.GetComponent<UnityEngine.Animation>().Stop();
				m_socialButton.GetComponent<UnityEngine.Animation>().Rewind();
				transform.GetComponent<UnityEngine.Animation>().Stop();
				transform.GetComponent<UnityEngine.Animation>().Rewind();
			}
		}
	}

	public void SocialButtonpressed()
	{
		if (!isGameHallExitOpened)
		{
			GameObject gameObject = GameObject.Find("SocialButton");
			GameObject gameObject2 = GameObject.Find("BackgroundSocialBtn");
			if (!gameObject.GetComponent<UnityEngine.Animation>().isPlaying)
			{
				bool reverse = bSocialButtonsOut;
				InitAnimationStates(reverse, gameObject.GetComponent<UnityEngine.Animation>()["SocialButtonSlide"], gameObject2.GetComponent<UnityEngine.Animation>()["MainMenuArrowAnimation"]);
				gameObject2.GetComponent<UnityEngine.Animation>().Play();
				gameObject.GetComponent<UnityEngine.Animation>().Play();
				bSocialButtonsOut = !bSocialButtonsOut;
			}
		}
	}

	private void InitAnimationStates(bool reverse, params UnityEngine.AnimationState[] states)
	{
		foreach (UnityEngine.AnimationState animationState in states)
		{
			animationState.speed = ((!reverse) ? 1 : (-1));
			animationState.time = ((!reverse) ? 0f : animationState.length);
		}
	}

	public void LaunchFacebook()
	{
		if (!isGameHallExitOpened)
		{
			Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.Facebook);
		}
	}

	public void LaunchRenren()
	{
		if (!isGameHallExitOpened)
		{
			Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.Renren);
		}
	}

	public void LaunchWeibos()
	{
		if (!isGameHallExitOpened)
		{
			Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.Weibos);
		}
	}

	public void LaunchYoutubeFilm()
	{
		if (!isGameHallExitOpened)
		{
			Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.Youtube);
		}
	}

	public void LaunchYoutubeFilmChina()
	{
		if (!isGameHallExitOpened)
		{
			Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.YoutubeChina);
		}
	}

	public void LaunchTwitter()
	{
		if (!isGameHallExitOpened)
		{
			Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.Twitter);
		}
	}

	public void LaunchMoreGames()
	{
		if (!isGameHallExitOpened)
		{
			Application.OpenURL("http://wapgame.189.cn");
		}
	}

	public void ShowAboutScreen()
	{
		if (!isGameHallExitOpened)
		{
			UnityEngine.Object.Instantiate(Resources.Load("UI/AboutPage", typeof(GameObject)) as GameObject);
		}
	}

	public void OpenShop()
	{
		if (!isGameHallExitOpened && Singleton<BuildCustomizationLoader>.Instance.IAPEnabled && (!(gearButtonAnimation != null) || !gearButtonAnimation.isPlaying))
		{
			mainMenuNode.SetActive(value: false);
			Singleton<IapManager>.Instance.OpenShopPage(delegate
			{
				mainMenuNode.SetActive(value: true);
			});
		}
	}

	public void OpenRovioProductsOverlay()
	{
		if (!isGameHallExitOpened)
		{
			m_crossPromotionOverlay.SetActive(value: true);
		}
	}

	public void OpenToons()
	{
		if (!isGameHallExitOpened)
		{
			Application.OpenURL("https://www.youtube.com/user/RovioMobile");
		}
	}

	public void OpenGamesVideoChannel()
	{
	}

	public void RestoreIAP()
	{
		Singleton<IapManager>.Instance.RestorePurchasedItems();
	}

	public void OpenConfirmationPopup()
	{
		m_confirmationPopup.SetActive(value: true);
	}

	public void SetFullscreen()
	{
		Singleton<GuiManager>.Instance.SetFullscreen();
	}

	public void SendStartFlurryEvent()
	{
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && isGameHallExitOpened)
		{
			isGameHallExitOpened = false;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (Singleton<SocialGameManager>.IsInstantiated())
		{
			if (paused)
			{
				ShowGameCenterButton(show: false);
			}
			else if (!paused)
			{
				ShowGameCenterButton(Singleton<SocialGameManager>.Instance.Authenticated);
			}
		}
	}
}
