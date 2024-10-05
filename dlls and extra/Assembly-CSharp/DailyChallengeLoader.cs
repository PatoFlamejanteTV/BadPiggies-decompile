using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyChallengeLoader : WPFMonoBehaviour
{
	private enum State
	{
		None,
		Loading,
		Error,
		Ready,
		TimeOut
	}

	public Action OnImageReady;

	public Action OnImageLoading;

	private const float TIMEOUT_LIMIT = 15f;

	private const float RETRY_TIME = 5f;

	[SerializeField]
	private GameObject errorImage;

	[SerializeField]
	private GameObject loadingImage;

	[SerializeField]
	private GameObject dailyImage;

	[SerializeField]
	private GameObject disabled;

	[SerializeField]
	private Transform lootCratePos;

	[SerializeField]
	private GameObject collected;

	[SerializeField]
	private GameObject adButton;

	[SerializeField]
	private int challengeIndex;

	private bool loading;

	private bool hasImage;

	private bool initialized;

	private string currentKey;

	private GameObject crateIcon;

	private State state;

	private static Dictionary<int, DailyChallengeLoader> instances = new Dictionary<int, DailyChallengeLoader>();

	private static Dictionary<int, Material> dailyMaterials = new Dictionary<int, Material>();

	public bool ImageReady => hasImage;

	private void Awake()
	{
		initialized = false;
		instances.Add(challengeIndex, this);
		dailyMaterials.Add(challengeIndex, dailyImage.GetComponent<Renderer>().material);
		SetState(State.None);
	}

	private void OnEnable()
	{
		if (!loading)
		{
			if (!hasImage || state == State.Error || state == State.None)
			{
				SetDisabled();
				Singleton<NetworkManager>.Instance.CheckAccess(OnNetworkCheck);
			}
			else if (state == State.Ready)
			{
				SetState(state);
			}
		}
	}

	private void OnNetworkCheck(bool hasInternet)
	{
		if (!hasInternet)
		{
			SetDisabled();
			return;
		}
		if (initialized)
		{
			LoadImage();
			return;
		}
		if (Singleton<DailyChallenge>.Instance.Initialized)
		{
			Initialize();
			return;
		}
		SetDisabled();
		DailyChallenge instance = Singleton<DailyChallenge>.Instance;
		instance.OnInitialize = (Action)Delegate.Combine(instance.OnInitialize, new Action(Initialize));
	}

	private void Initialize()
	{
		if (!HatchManager.IsLoggedIn)
		{
			HatchManager.onLoginSuccess = (Action)Delegate.Combine(HatchManager.onLoginSuccess, new Action(Initialize));
			return;
		}
		initialized = true;
		DailyChallenge instance = Singleton<DailyChallenge>.Instance;
		instance.OnDailyChallengeChanged = (Action)Delegate.Combine(instance.OnDailyChallengeChanged, new Action(LoadImage));
		HatchManager.onLoginSuccess = (Action)Delegate.Remove(HatchManager.onLoginSuccess, new Action(Initialize));
		LoadImage();
	}

	private void OnDestroy()
	{
		instances.Remove(challengeIndex);
		dailyMaterials.Remove(challengeIndex);
		if (Singleton<DailyChallenge>.IsInstantiated())
		{
			DailyChallenge instance = Singleton<DailyChallenge>.Instance;
			instance.OnInitialize = (Action)Delegate.Remove(instance.OnInitialize, new Action(Initialize));
			DailyChallenge instance2 = Singleton<DailyChallenge>.Instance;
			instance2.OnDailyChallengeChanged = (Action)Delegate.Remove(instance2.OnDailyChallengeChanged, new Action(LoadImage));
		}
		if (Singleton<NetworkManager>.IsInstantiated())
		{
			Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(OnNetworkCheck);
		}
	}

	private void SetDisabled()
	{
		adButton.SetActive(value: false);
		errorImage.SetActive(value: false);
		loadingImage.SetActive(value: false);
		collected.SetActive(value: false);
		dailyImage.SetActive(value: true);
		disabled.SetActive(value: true);
	}

	private void LoadImage()
	{
	}

	private void SetState(State state)
	{
		bool flag = Singleton<DailyChallenge>.Instance.HasChallenge && Singleton<DailyChallenge>.Instance.DailyChallengeCollected(challengeIndex);
		this.state = state;
		switch (state)
		{
		case State.Loading:
			hasImage = false;
			disabled.SetActive(value: false);
			errorImage.SetActive(value: false);
			dailyImage.SetActive(value: false);
			collected.SetActive(value: false);
			loadingImage.SetActive(value: true);
			if (crateIcon != null)
			{
				crateIcon.SetActive(value: false);
			}
			if (OnImageLoading != null)
			{
				OnImageLoading();
			}
			break;
		case State.Error:
			dailyImage.SetActive(value: false);
			loadingImage.SetActive(value: false);
			collected.SetActive(value: false);
			disabled.SetActive(value: true);
			hasImage = false;
			if (crateIcon != null)
			{
				crateIcon.SetActive(value: false);
			}
			break;
		case State.Ready:
			adButton.SetActive(value: true);
			errorImage.SetActive(value: false);
			loadingImage.SetActive(value: false);
			disabled.SetActive(value: false);
			dailyImage.SetActive(value: true);
			collected.SetActive(flag);
			dailyMaterials[challengeIndex].SetFloat("_Grayness", (!flag) ? 0f : 1f);
			UpdateLootCrateImage(flag);
			hasImage = true;
			if (OnImageReady != null)
			{
				OnImageReady();
			}
			OnImageReady = null;
			break;
		case State.TimeOut:
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(Retry());
			}
			goto case State.None;
		case State.None:
			dailyImage.SetActive(value: false);
			loadingImage.SetActive(value: false);
			collected.SetActive(value: false);
			errorImage.SetActive(value: false);
			hasImage = false;
			if (crateIcon != null)
			{
				crateIcon.SetActive(value: false);
			}
			break;
		}
		loading = state == State.Loading;
	}

	private void UpdateLootCrateImage(bool collected)
	{
		if (crateIcon != null)
		{
			UnityEngine.Object.Destroy(crateIcon);
		}
		if (!collected)
		{
			GameObject gameObject = WPFMonoBehaviour.gameData.m_lootCrates[(int)Singleton<DailyChallenge>.Instance.TodaysLootCrate(challengeIndex)];
			crateIcon = UnityEngine.Object.Instantiate(gameObject.transform.Find("Icon").gameObject);
			crateIcon.gameObject.layer = lootCratePos.gameObject.layer;
			crateIcon.transform.parent = lootCratePos;
			crateIcon.transform.localPosition = Vector3.zero;
			for (int i = 0; i < crateIcon.transform.childCount; i++)
			{
				crateIcon.transform.GetChild(i).gameObject.layer = lootCratePos.gameObject.layer;
			}
			Renderer[] componentsInChildren = crateIcon.GetComponentsInChildren<Renderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].sortingLayerName = "Popup";
			}
		}
	}

	private IEnumerator TimeoutCheck()
	{
		yield return new WaitForRealSeconds(15f);
		if (state == State.Loading)
		{
			SetState(State.TimeOut);
		}
	}

	private IEnumerator Retry()
	{
		yield return new WaitForRealSeconds(5f);
		if (state == State.TimeOut)
		{
			LoadImage();
		}
	}
}
