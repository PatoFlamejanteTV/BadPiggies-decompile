using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;

public class DailyChallengeButton : WPFMonoBehaviour
{
	private const string SEARCH_ANIM = "Search";

	private const string LOOT_FOUND_ANIM = "LootFound";

	[SerializeField]
	private GameObject woodCrate;

	[SerializeField]
	private GameObject woodCrateSilhouette;

	[SerializeField]
	private GameObject metalCrate;

	[SerializeField]
	private GameObject metalCrateSilhouette;

	[SerializeField]
	private GameObject goldCrate;

	[SerializeField]
	private GameObject goldCrateSilhouette;

	[SerializeField]
	private GameObject cardboardCrate;

	[SerializeField]
	private GameObject cardboardCrateSilhouette;

	[SerializeField]
	private GameObject glassCrate;

	[SerializeField]
	private GameObject glassCrateSilhouette;

	[SerializeField]
	private GameObject bronzeCrate;

	[SerializeField]
	private GameObject bronzeCrateSilhouette;

	[SerializeField]
	private GameObject marbleCrate;

	[SerializeField]
	private GameObject marbleCrateSilhouette;

	[SerializeField]
	private GameObject[] cratePositions;

	[SerializeField]
	private SkeletonAnimation anim;

	[SerializeField]
	private GameObject noConnection;

	[SerializeField]
	private GameObject loadingIndicator;

	private GameObject lootCrates;

	private Animation noConnAnim;

	private DailyChallengeDialog dialog;

	private bool networkFailure;

	private bool checkingNetwork;

	private bool loading;

	private bool DailyChallengeShown
	{
		get
		{
			return GameProgress.GetBool("DailyChallengeShown");
		}
		set
		{
			GameProgress.SetBool("DailyChallengeShown", value);
		}
	}

	private bool ShowingCutscene
	{
		get
		{
			return GameProgress.GetBool("DailyChallengeCutscene");
		}
		set
		{
			GameProgress.SetBool("DailyChallengeCutscene", value);
		}
	}

	private void Awake()
	{
		if (!DailyChallengeShown && WPFMonoBehaviour.levelManager != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		lootCrates = new GameObject();
		lootCrates.transform.parent = base.transform;
		lootCrates.transform.localPosition = Vector3.zero;
		lootCrates.transform.localScale = Vector3.one;
		noConnAnim = noConnection.GetComponent<Animation>();
		noConnection.SetActive(value: false);
		networkFailure = true;
	}

	private void Start()
	{
		dialog = DailyChallengeDialog.Create();
		dialog.transform.position = WPFMonoBehaviour.hudCamera.transform.position + new Vector3(0f, 0f, 10f);
		dialog.Close();
		if (ShowingCutscene)
		{
			dialog.Open();
			ShowingCutscene = false;
			DailyChallengeShown = true;
		}
		anim.state.SetAnimation(0, "Search", loop: true);
		StartCoroutine(LoadingIndicator());
		loading = true;
		checkingNetwork = true;
		Singleton<NetworkManager>.Instance.CheckAccess(OnNetworkCheck);
	}

	private void OnEnable()
	{
		if (!checkingNetwork && !networkFailure)
		{
			UpdateLootCrates();
		}
	}

	private void OnNetworkCheck(bool hasInternet)
	{
		networkFailure = !hasInternet || !HatchManager.IsLoggedIn;
		checkingNetwork = false;
		loading = false;
		loadingIndicator.SetActive(value: false);
		if (networkFailure)
		{
			UpdateLootCrates();
			return;
		}
		if (Singleton<DailyChallenge>.Instance.Initialized)
		{
			OnDailyChallengeInitialized();
			return;
		}
		DailyChallenge instance = Singleton<DailyChallenge>.Instance;
		instance.OnInitialize = (Action)Delegate.Combine(instance.OnInitialize, new Action(OnDailyChallengeInitialized));
	}

	private void OnDestroy()
	{
		if (Singleton<DailyChallenge>.IsInstantiated())
		{
			DailyChallenge instance = Singleton<DailyChallenge>.Instance;
			instance.OnDailyChallengeChanged = (Action)Delegate.Remove(instance.OnDailyChallengeChanged, new Action(UpdateLootCrates));
			DailyChallenge instance2 = Singleton<DailyChallenge>.Instance;
			instance2.OnInitialize = (Action)Delegate.Remove(instance2.OnInitialize, new Action(OnDailyChallengeInitialized));
		}
		if (Singleton<NetworkManager>.IsInstantiated())
		{
			Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(OnNetworkCheck);
		}
	}

	private void OnDailyChallengeInitialized()
	{
		UpdateLootCrates();
		DailyChallenge instance = Singleton<DailyChallenge>.Instance;
		instance.OnDailyChallengeChanged = (Action)Delegate.Combine(instance.OnDailyChallengeChanged, new Action(UpdateLootCrates));
	}

	private void UpdateLootCrates()
	{
		for (int i = 0; i < lootCrates.transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(lootCrates.transform.GetChild(i).gameObject);
		}
		noConnection.SetActive(networkFailure);
		if (networkFailure)
		{
			return;
		}
		int num = 0;
		for (int j = 0; j < cratePositions.Length; j++)
		{
			GameObject gameObject;
			GameObject gameObject2;
			switch (Singleton<DailyChallenge>.Instance.TodaysLootCrate(j))
			{
			case LootCrateType.Wood:
				gameObject = UnityEngine.Object.Instantiate(woodCrateSilhouette);
				gameObject2 = ((!Singleton<DailyChallenge>.Instance.Challenges[j].collected && !networkFailure) ? UnityEngine.Object.Instantiate(woodCrate) : null);
				break;
			case LootCrateType.Metal:
				gameObject = UnityEngine.Object.Instantiate(metalCrateSilhouette);
				gameObject2 = ((!Singleton<DailyChallenge>.Instance.Challenges[j].collected && !networkFailure) ? UnityEngine.Object.Instantiate(metalCrate) : null);
				break;
			case LootCrateType.Gold:
				gameObject = UnityEngine.Object.Instantiate(goldCrateSilhouette);
				gameObject2 = ((!Singleton<DailyChallenge>.Instance.Challenges[j].collected && !networkFailure) ? UnityEngine.Object.Instantiate(goldCrate) : null);
				break;
			case LootCrateType.Cardboard:
				gameObject = UnityEngine.Object.Instantiate(cardboardCrateSilhouette);
				gameObject2 = ((!Singleton<DailyChallenge>.Instance.Challenges[j].collected && !networkFailure) ? UnityEngine.Object.Instantiate(cardboardCrate) : null);
				break;
			case LootCrateType.Glass:
				gameObject = UnityEngine.Object.Instantiate(glassCrateSilhouette);
				gameObject2 = ((!Singleton<DailyChallenge>.Instance.Challenges[j].collected && !networkFailure) ? UnityEngine.Object.Instantiate(glassCrate) : null);
				break;
			case LootCrateType.Bronze:
				gameObject = UnityEngine.Object.Instantiate(bronzeCrateSilhouette);
				gameObject2 = ((!Singleton<DailyChallenge>.Instance.Challenges[j].collected && !networkFailure) ? UnityEngine.Object.Instantiate(bronzeCrate) : null);
				break;
			case LootCrateType.Marble:
				gameObject = UnityEngine.Object.Instantiate(marbleCrateSilhouette);
				gameObject2 = ((!Singleton<DailyChallenge>.Instance.Challenges[j].collected && !networkFailure) ? UnityEngine.Object.Instantiate(marbleCrate) : null);
				break;
			default:
				gameObject = UnityEngine.Object.Instantiate(woodCrateSilhouette);
				gameObject2 = null;
				break;
			}
			gameObject.transform.parent = lootCrates.transform;
			gameObject.transform.position = cratePositions[j].transform.position;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.layer = base.gameObject.layer;
			if (gameObject2 != null)
			{
				gameObject2.transform.parent = lootCrates.transform;
				gameObject2.transform.localPosition = gameObject.transform.localPosition + new Vector3(0f, 0f, -0.1f);
				gameObject2.transform.localScale = Vector3.one;
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.layer = base.gameObject.layer;
			}
			num += ((!Singleton<DailyChallenge>.Instance.Challenges[j].collected) ? 1 : 0);
		}
		if (num > 0)
		{
			anim.state.SetAnimation(0, "LootFound", loop: true);
		}
	}

	public void OnPress()
	{
		if (networkFailure)
		{
			noConnAnim.Play();
			if (!checkingNetwork)
			{
				checkingNetwork = true;
				Singleton<NetworkManager>.Instance.CheckAccess(OnNetworkCheck);
			}
		}
		else if (DailyChallengeShown && WPFMonoBehaviour.levelManager == null)
		{
			dialog.Open();
		}
		else if (DailyChallengeShown && WPFMonoBehaviour.levelManager != null)
		{
			WPFMonoBehaviour.levelManager.InGameGUI.Hide();
			dialog.Open(delegate
			{
				if ((bool)WPFMonoBehaviour.levelManager)
				{
					WPFMonoBehaviour.levelManager.InGameGUI.Show();
				}
			});
		}
		else
		{
			ShowingCutscene = true;
			Singleton<Loader>.Instance.LoadLevel("DailyChallenge", GameManager.GameState.Cutscene, showLoadingScreen: true);
		}
	}

	public void CreateNewChallenge()
	{
	}

	private IEnumerator LoadingIndicator()
	{
		TextMesh[] texts = loadingIndicator.GetComponentsInChildren<TextMesh>();
		loadingIndicator.gameObject.SetActive(value: true);
		float wait = 0.6f;
		for (int i = 0; i < texts.Length; i++)
		{
			texts[i].text = string.Empty;
		}
		yield return new WaitForRealSeconds(wait);
		while (loading && loadingIndicator.gameObject.activeInHierarchy && loading)
		{
			for (int j = 0; j < texts.Length; j++)
			{
				texts[j].text = ".";
			}
			yield return new WaitForRealSeconds(wait);
			if (!loadingIndicator.gameObject.activeInHierarchy || !loading)
			{
				break;
			}
			for (int k = 0; k < texts.Length; k++)
			{
				texts[k].text = "..";
			}
			yield return new WaitForRealSeconds(wait);
			if (!loadingIndicator.gameObject.activeInHierarchy || !loading)
			{
				break;
			}
			for (int l = 0; l < texts.Length; l++)
			{
				texts[l].text = "...";
			}
			yield return new WaitForRealSeconds(wait);
		}
		loadingIndicator.gameObject.SetActive(value: false);
	}
}
