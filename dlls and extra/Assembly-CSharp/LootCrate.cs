using System;
using System.Collections;
using UnityEngine;

public class LootCrate : Collectable
{
	public enum AdWatched
	{
		Yes,
		No,
		NotApplicaple
	}

	public struct AnalyticData
	{
		public string receivedFrom;

		public string price;

		public AdWatched adWatched;

		public AnalyticData(string receivedFrom, string price, AdWatched adWatched)
		{
			this.receivedFrom = receivedFrom;
			this.price = price;
			this.adWatched = adWatched;
		}
	}

	public Action OnCollect;

	public Func<int> RewardExperience;

	[SerializeField]
	private LootCrateType crateType;

	private LootCrateOpenDialog dialog;

	private int dailyIndex;

	private bool isAdRevealed;

	private Vector3 originalCratePosition;

	private float bulletTimeSpeed = 3f;

	public LootCrateOpenDialog Dialog => dialog;

	public LootCrateType CrateType => crateType;

	protected override void Start()
	{
		base.Start();
		OnDataLoaded();
		originalCratePosition = base.transform.position;
		if (dialog == null)
		{
			dialog = SpawnLootCrateOpeningDialog();
		}
		base.rigidbody.useGravity = false;
		base.rigidbody.isKinematic = true;
	}

	protected override void OnGoalEnter(BasePart part)
	{
		if (collected || WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Completed)
		{
			return;
		}
		if ((bool)collectedEffect)
		{
			UnityEngine.Object.Instantiate(collectedEffect, base.transform.position, collectedEffect.transform.rotation);
		}
		if (!WorkshopMenu.FirstLootCrateCollected)
		{
			WorkshopMenu.FirstLootCrateCollected = true;
			if (Singleton<SocialGameManager>.IsInstantiated())
			{
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.HIDDEN_CRATE", 100.0);
			}
		}
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.bonusBoxCollected, base.transform.position);
		collected = true;
		CoroutineRunner.Instance.StartCoroutine(BulletTime(pause: true));
		PlayerProgressBar.Instance.DelayUpdate();
		if (RewardExperience != null)
		{
			StartCoroutine(SpawnDialog(RewardExperience()));
		}
		else
		{
			StartCoroutine(SpawnDialog(0));
		}
		DisableGoal(disable: true);
		if (OnCollect != null)
		{
			OnCollect();
		}
	}

	protected override void OnReset()
	{
		collected = false;
		base.transform.position = originalCratePosition;
		DisableGoal(disable: false);
		base.rigidbody.useGravity = false;
		base.rigidbody.isKinematic = true;
	}

	private IEnumerator SpawnDialog(int experience)
	{
		if (dialog != null)
		{
			dialog.PrepareOpening();
		}
		float waitTime = 1f / bulletTimeSpeed;
		while (waitTime > 0f)
		{
			waitTime -= GameTime.RealTimeDelta;
			yield return null;
		}
		if (dialog != null)
		{
			dialog.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 5f;
			dialog.gameObject.SetActive(value: true);
			dialog.onClose += ContinueGame;
			dialog.AddLootCrate(crateType, 1, new AnalyticData($"daily_{dailyIndex}", "found", (!isAdRevealed) ? AdWatched.No : AdWatched.Yes), fromQueue: false, experience);
		}
		ResourceBar.Instance.ShowItem(ResourceBar.Item.PlayerProgress, showItem: true, enableItem: false);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void ContinueGame()
	{
		ResourceBar.Instance.ShowItem(ResourceBar.Item.PlayerProgress, showItem: false);
		CoroutineRunner.Instance.StartCoroutine(BulletTime(pause: false));
		if (dialog != null)
		{
			dialog.onClose -= ContinueGame;
		}
	}

	private IEnumerator BulletTime(bool pause)
	{
		float fade = 0f;
		float fromTime = ((!pause) ? 0f : 1f);
		float toTime = ((!pause) ? 1f : 0f);
		while (fade < 1f)
		{
			Time.timeScale = Mathf.Lerp(fromTime, toTime, fade);
			fade += GameTime.RealTimeDelta * bulletTimeSpeed;
			yield return null;
		}
		GameTime.Pause(pause);
	}

	public void SetAnalyticData(int index, bool isAdRevealed)
	{
		dailyIndex = index;
		this.isAdRevealed = isAdRevealed;
	}

	public static LootCrateOpenDialog SpawnLootCrateOpeningDialog()
	{
		LootCrateOpenDialog lootCrateOpenDialog = LootCrateOpenDialog.CreateLootCrateOpenDialog();
		if (lootCrateOpenDialog != null)
		{
			lootCrateOpenDialog.gameObject.SetActive(value: false);
		}
		return lootCrateOpenDialog;
	}

	public static LootCrateOpenDialog SpawnLootCrateOpeningDialog(LootCrateType crateType, int amount, Camera hudCamera, Dialog.OnClose onClose, AnalyticData data)
	{
		if (Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.Undefined)
		{
			return null;
		}
		LootCrateOpenDialog lootCrateOpenDialog = LootCrateOpenDialog.CreateLootCrateOpenDialog();
		if (lootCrateOpenDialog != null && hudCamera != null)
		{
			lootCrateOpenDialog.transform.position = hudCamera.transform.position + Vector3.forward * 2.5f;
			lootCrateOpenDialog.gameObject.SetActive(value: true);
			lootCrateOpenDialog.onClose += onClose;
			lootCrateOpenDialog.AddLootCrate(crateType, amount, data);
		}
		return lootCrateOpenDialog;
	}
}
