using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class UnlockLootCrateSlotDialog : TextDialog
{
	public enum UnlockType
	{
		PurchaseLockedCrate,
		PurchaseInactiveCrate,
		StartUnlocking,
		DiscardPopup
	}

	[SerializeField]
	private string[] skinNames;

	[SerializeField]
	private TextMesh priceLabel;

	[SerializeField]
	private TextMesh timerLabel;

	[SerializeField]
	private Transform crateHolder;

	[SerializeField]
	private GameObject[] timerInfos;

	[SerializeField]
	private GridLayout gridLayout;

	[SerializeField]
	private GameObject epicItemPrefab;

	[SerializeField]
	private GameObject rareItemPrefab;

	[SerializeField]
	private GameObject commonItemPrefab;

	[SerializeField]
	private GameObject randomItemPrefab;

	[SerializeField]
	private SkeletonAnimation skeletonAnimation;

	[SerializeField]
	private GameObject openNowControls;

	[SerializeField]
	private GameObject unlockNowControls;

	private CakeRaceTutorial cakeRaceTutorial;

	private LootCrateType currentCrateType = LootCrateType.None;

	public int SnoutCoinPrice { get; private set; }

	private bool BeginOpeningTutorialShown
	{
		get
		{
			return GameProgress.GetBool("BeginOpeninTutorialShown");
		}
		set
		{
			GameProgress.SetBool("BeginOpeninTutorialShown", value);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		GameObject gameObject = GameObject.Find("CakeRaceTutorial");
		if (gameObject != null)
		{
			cakeRaceTutorial = gameObject.GetComponent<CakeRaceTutorial>();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		base.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 10f;
		if (!BeginOpeningTutorialShown && cakeRaceTutorial != null && unlockNowControls != null)
		{
			cakeRaceTutorial.OpenCrateTutorial(unlockNowControls.transform);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (cakeRaceTutorial != null)
		{
			cakeRaceTutorial.SetActive(active: false);
		}
	}

	public void InitPopup(int price, int secondsLeft, GameObject cratePrefab, LootCrateType lootCrate)
	{
		currentCrateType = lootCrate;
		SnoutCoinPrice = price;
		if ((bool)priceLabel)
		{
			TextMesh[] componentsInChildren = priceLabel.gameObject.GetComponentsInChildren<TextMesh>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].text = $"[snout] {SnoutCoinPrice}";
				TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren[i]);
			}
		}
		string formattedTimeFromSeconds = LootCrateSlot.GetFormattedTimeFromSeconds(secondsLeft);
		if ((bool)timerLabel)
		{
			TextMeshHelper.UpdateTextMeshes(timerLabel.gameObject.GetComponentsInChildren<TextMesh>(), formattedTimeFromSeconds);
		}
		if (unlockNowControls != null)
		{
			Transform transform = unlockNowControls.transform.Find("StartUnlockButton/TimeLabel");
			if (transform != null)
			{
				TextMeshHelper.UpdateTextMeshes(transform.gameObject.GetComponentsInChildren<TextMesh>(), formattedTimeFromSeconds);
			}
		}
		if ((bool)skeletonAnimation)
		{
			skeletonAnimation.initialSkinName = lootCrate.ToString();
			skeletonAnimation.Initialize(overwrite: true);
			skeletonAnimation.state.AddAnimation(0, "Idle", loop: true, 0f);
		}
		InitRewardItems(lootCrate);
	}

	private void InitRewardItems(LootCrateType lootCrate)
	{
		if ((bool)gridLayout)
		{
			List<Tuple<LootCrateRewards.Reward, BasePart.PartTier>> list = LootCrateRewards.MinimumRewards(lootCrate);
			for (int i = 0; i < list.Count; i++)
			{
				GameObject original = ((list[i].Item1 == LootCrateRewards.Reward.Part) ? ((list[i].Item2 != BasePart.PartTier.Common) ? ((list[i].Item2 != BasePart.PartTier.Rare) ? epicItemPrefab : rareItemPrefab) : commonItemPrefab) : randomItemPrefab);
				GameObject obj = Object.Instantiate(original, Vector3.zero, Quaternion.identity);
				LayerHelper.SetLayer(obj, base.gameObject.layer, children: true);
				LayerHelper.SetSortingLayer(obj, "Popup", children: true);
				LayerHelper.SetOrderInLayer(obj, 0, children: true);
				obj.transform.parent = gridLayout.transform;
			}
			gridLayout.UpdateLayout();
		}
	}

	public void SetInfoLabel(UnlockType infoType)
	{
		if (timerInfos == null)
		{
			return;
		}
		for (int i = 0; i < timerInfos.Length; i++)
		{
			if (timerInfos[i] != null)
			{
				timerInfos[i].SetActive(i == (int)infoType);
			}
		}
		if (openNowControls != null)
		{
			openNowControls.SetActive(infoType <= UnlockType.PurchaseInactiveCrate);
		}
		if (unlockNowControls != null)
		{
			unlockNowControls.SetActive(infoType == UnlockType.StartUnlocking);
		}
	}

	public void TryToUnlock()
	{
		if (GameProgress.SnoutCoinCount() >= SnoutCoinPrice)
		{
			Confirm();
			Close();
		}
		else
		{
			OpenShop();
		}
	}

	public void DiscardCrate()
	{
		Close();
	}

	public void StartUnlockingNow()
	{
		BeginOpeningTutorialShown = true;
		Confirm();
		Close();
	}

	public new void Close()
	{
		if ((bool)crateHolder && crateHolder.childCount > 0)
		{
			for (int i = 0; i < crateHolder.childCount; i++)
			{
				Object.Destroy(crateHolder.GetChild(i).gameObject);
			}
		}
		base.Close();
		Object.Destroy(base.gameObject);
	}
}
