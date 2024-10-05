using System;
using UnityEngine;

public class WorkshopButton : Button
{
	[SerializeField]
	private GameObject newTag;

	private bool tagAdded;

	protected override void ButtonAwake()
	{
		bool flag = WorkshopMenu.FirstLootCrateCollected || WorkshopMenu.AnyLootCrateCollected;
		if (INSettings.GetBool(INFeature.EnableWorkShopButton))
		{
			flag = true;
		}
		bool @bool = GameProgress.GetBool("Workshop_Visited");
		base.gameObject.SetActive(flag);
		if (flag && !@bool)
		{
			Wiggle();
		}
		else if (!flag && !@bool)
		{
			FreeLootCrate.OnFreeLootCrateCollected = (Action)Delegate.Combine(FreeLootCrate.OnFreeLootCrateCollected, new Action(ButtonAwake));
			IapManager.onPurchaseSucceeded += OnItemPurchase;
		}
	}

	private void OnDestroy()
	{
		FreeLootCrate.OnFreeLootCrateCollected = (Action)Delegate.Remove(FreeLootCrate.OnFreeLootCrateCollected, new Action(ButtonAwake));
		IapManager.onPurchaseSucceeded -= OnItemPurchase;
	}

	private void Wiggle()
	{
		if (!tagAdded)
		{
			GameObject obj = UnityEngine.Object.Instantiate(newTag);
			obj.transform.parent = base.transform;
			obj.transform.localPosition = new Vector3(1.1f, 1.1f, -1f);
			tagAdded = true;
		}
	}

	private void Start()
	{
	}

	private void OnItemPurchase(IapManager.InAppPurchaseItemType type)
	{
		if ((uint)(type - 48) <= 5u)
		{
			ButtonAwake();
		}
	}

	protected override void OnActivate()
	{
		Singleton<GameManager>.Instance.LoadWorkshop(showLoadingScreen: false);
	}
}
