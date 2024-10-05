using System;
using UnityEngine;

public class FreeLootCrate : MonoBehaviour
{
	public static Action OnFreeLootCrateCollected;

	[SerializeField]
	private LootCrateType crateType;

	[SerializeField]
	private string crateID = string.Empty;

	private bool isEnabled;

	public static bool FreeShopLootCrateCollected => GameProgress.GetInt("FreeShopCrateWood") > 0;

	private void Awake()
	{
		Singleton<NetworkManager>.Instance.CheckAccess(OnCheckNetworkResponse);
		NetworkManager instance = Singleton<NetworkManager>.Instance;
		instance.OnNetworkChange = (NetworkManager.OnNetworkChangedDelegate)Delegate.Combine(instance.OnNetworkChange, new NetworkManager.OnNetworkChangedDelegate(OnNetworkChange));
	}

	private void OnEnable()
	{
		if (string.IsNullOrEmpty(crateID))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Initialize(isEnabled);
		}
	}

	private void OnDestroy()
	{
		if (Singleton<NetworkManager>.IsInstantiated())
		{
			NetworkManager instance = Singleton<NetworkManager>.Instance;
			instance.OnNetworkChange = (NetworkManager.OnNetworkChangedDelegate)Delegate.Remove(instance.OnNetworkChange, new NetworkManager.OnNetworkChangedDelegate(OnNetworkChange));
			Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(OnCheckNetworkResponse);
		}
	}

	private void OnCheckNetworkResponse(bool hasNetwork)
	{
		Singleton<NetworkManager>.Instance.UnsubscribeFromResponse(OnCheckNetworkResponse);
		Initialize(hasNetwork);
	}

	private void OnNetworkChange(bool hasNetwork)
	{
		Initialize(hasNetwork);
	}

	private void Initialize(bool enable)
	{
		isEnabled = GameProgress.GetInt(crateID + crateType) == 0 && enable;
		base.gameObject.SetActive(isEnabled);
	}

	public void GiveReward()
	{
		int @int = GameProgress.GetInt(crateID + crateType);
		if (@int <= 0)
		{
			GameProgress.SetInt(crateID + crateType, @int + 1);
			WorkshopMenu.AnyLootCrateCollected = true;
			if (OnFreeLootCrateCollected != null)
			{
				OnFreeLootCrateCollected();
			}
			Camera hudCamera = Singleton<GuiManager>.Instance.FindCamera();
			LootCrate.SpawnLootCrateOpeningDialog(crateType, 1, hudCamera, null, new LootCrate.AnalyticData(crateID, "free", LootCrate.AdWatched.NotApplicaple));
			TryReportAchievements();
		}
		base.gameObject.SetActive(value: false);
	}

	private void TryReportAchievements()
	{
		if (GameProgress.GetInt(crateID + crateType) == 1 && crateID.Equals("FreeShopCrate") && Singleton<SocialGameManager>.IsInstantiated())
		{
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.FREE_CRATE", 100.0);
		}
	}
}
