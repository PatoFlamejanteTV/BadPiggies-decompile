using System;
using System.Collections;
using UnityEngine;

public class FreeCrateAdReward : WPFMonoBehaviour
{
	private const string CRATE_CONFIG_NAME = "free_crate_ad_reward";

	private const string CRATE_TYPE = "LootCrateType";

	[SerializeField]
	private Transform cratePosition;

	[SerializeField]
	private GameObject m_errorPopup;

	private NotificationPopup errorPopup;

	private AdReward adReward;

	private Renderer[] renderers;

	private Collider[] colliders;

	private LootCrateType reward;

	private GameObject currentCrateIcon;

	private void Awake()
	{
		renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
		colliders = GetComponentsInChildren<Collider>(includeInactive: true);
		Activate(activate: false);
		if (Singleton<GameConfigurationManager>.Instance.HasData)
		{
			Initialize();
			return;
		}
		GameConfigurationManager instance = Singleton<GameConfigurationManager>.Instance;
		instance.OnHasData = (Action)Delegate.Combine(instance.OnHasData, new Action(Initialize));
	}

	private void Initialize()
	{
		GameConfigurationManager instance = Singleton<GameConfigurationManager>.Instance;
		instance.OnHasData = (Action)Delegate.Remove(instance.OnHasData, new Action(Initialize));
		string value = Singleton<GameConfigurationManager>.Instance.GetValue<string>("free_crate_ad_reward", "LootCrateType");
		try
		{
			reward = (LootCrateType)Enum.Parse(typeof(LootCrateType), value);
		}
		catch (ArgumentException)
		{
			reward = LootCrateType.Cardboard;
		}
		adReward = new AdReward(AdvertisementHandler.FreeLootCratePlacement);
		AdReward obj = adReward;
		obj.OnAdFinished = (Action)Delegate.Combine(obj.OnAdFinished, new Action(OnAdFinished));
		AdReward obj2 = adReward;
		obj2.OnCancel = (Action)Delegate.Combine(obj2.OnCancel, new Action(OnAdCancel));
		AdReward obj3 = adReward;
		obj3.OnConfirmationFailed = (Action)Delegate.Combine(obj3.OnConfirmationFailed, new Action(OnConfirmationFailed));
		AdReward obj4 = adReward;
		obj4.OnFailed = (Action)Delegate.Combine(obj4.OnFailed, new Action(OnAdFailed));
		AdReward obj5 = adReward;
		obj5.OnLoading = (Action)Delegate.Combine(obj5.OnLoading, new Action(OnAdLoading));
		AdReward obj6 = adReward;
		obj6.OnReady = (Action)Delegate.Combine(obj6.OnReady, new Action(OnAdReady));
		AdReward obj7 = adReward;
		obj7.OnAdPlayFailed = (Action)Delegate.Combine(obj7.OnAdPlayFailed, new Action(OnAdPlayFailed));
		adReward.Load();
	}

	private void OnDestroy()
	{
		if (adReward != null)
		{
			adReward.Dispose();
		}
	}

	private void Activate(bool activate)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = activate;
			}
		}
		for (int j = 0; j < colliders.Length; j++)
		{
			if (colliders[j] != null)
			{
				colliders[j].enabled = activate;
			}
		}
		if (currentCrateIcon != null)
		{
			currentCrateIcon.SetActive(activate);
		}
	}

	private void OnAdReady()
	{
		Activate(activate: true);
		if (currentCrateIcon != null)
		{
			UnityEngine.Object.Destroy(currentCrateIcon);
		}
		currentCrateIcon = LootCrateGraphicSpawner.CreateCrate(reward, cratePosition, Vector3.zero, Vector3.one, Quaternion.identity);
		LayerHelper.SetLayer(currentCrateIcon, base.gameObject.layer, children: true);
	}

	private void OnAdLoading()
	{
		Activate(activate: false);
	}

	private void OnAdFailed()
	{
		Activate(activate: false);
		StartCoroutine(WaitAndLoad());
	}

	private IEnumerator WaitAndLoad()
	{
		float waitTime = 10f;
		while (waitTime > 0f)
		{
			waitTime -= Time.deltaTime;
			yield return null;
		}
		adReward.Load();
	}

	private void OnConfirmationFailed()
	{
		adReward.Load();
	}

	private void OnAdCancel()
	{
		adReward.Load();
	}

	private void OnAdFinished()
	{
		LootCrate.SpawnLootCrateOpeningDialog(reward, 1, WPFMonoBehaviour.hudCamera, null, new LootCrate.AnalyticData("Advertisement", "0", LootCrate.AdWatched.Yes));
		adReward.Load();
	}

	private void OnAdPlayFailed()
	{
		if (errorPopup == null)
		{
			errorPopup = CreateErrorPopup();
		}
		errorPopup.Open();
	}

	public void PlayVideo()
	{
		adReward.Play();
	}

	private NotificationPopup CreateErrorPopup()
	{
		GameObject obj = UnityEngine.Object.Instantiate(m_errorPopup);
		obj.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 3f;
		NotificationPopup component = obj.GetComponent<NotificationPopup>();
		component.Close();
		component.SetText("IN_APP_PURCHASE_NOT_READY");
		return component;
	}
}
