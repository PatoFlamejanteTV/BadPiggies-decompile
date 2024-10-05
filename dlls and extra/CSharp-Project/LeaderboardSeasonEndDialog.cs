using System.Collections;
using Spine.Unity;
using UnityEngine;

public class LeaderboardSeasonEndDialog : TextDialog
{
	[SerializeField]
	private string titleLocalizationKey;

	[SerializeField]
	private string rankLocalizationKey;

	[SerializeField]
	private string snoutRewardLocalizationKey;

	[SerializeField]
	private GameObject rewardState;

	[SerializeField]
	private GameObject loadingState;

	[SerializeField]
	private SkeletonAnimation lootCrate;

	[SerializeField]
	private GameObject barrelContainer;

	private TextMesh[] titleLabel;

	private TextMesh[] rankLabel;

	private TextMesh[] rewardLabel;

	private bool isLoading;

	private int reward;

	protected override void Awake()
	{
		base.Awake();
		Transform transform = dialogRoot.transform.Find("TitleRibbon/TitleLabel");
		if (transform != null)
		{
			titleLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		transform = dialogRoot.transform.Find("RewardContainer/RankLabel");
		if (transform != null)
		{
			rankLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		transform = dialogRoot.transform.Find("RewardContainer/RewardLabel");
		if (transform != null)
		{
			rewardLabel = transform.GetComponentsInChildren<TextMesh>();
		}
		Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve(titleLocalizationKey);
		TextMeshHelper.UpdateTextMeshes(titleLabel, string.Format(localeParameters.translation, CakeRaceMenu.CurrentCakeRaceWeek() - 1));
	}

	public void SetLoading(bool loading)
	{
		isLoading = loading;
		loadingState.SetActive(loading);
		rewardState.SetActive(!loading);
	}

	public void SetCrateRankAndReward(LootCrateType crateType, int rank, int reward)
	{
		this.reward = reward;
		Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve(rankLocalizationKey);
		TextMeshHelper.UpdateTextMeshes(rankLabel, string.Format(localeParameters.translation, rank));
		TextMeshHelper.Wrap(rankLabel, 15);
		Localizer.LocaleParameters localeParameters2 = Singleton<Localizer>.Instance.Resolve(snoutRewardLocalizationKey);
		TextMeshHelper.UpdateTextMeshes(rewardLabel, string.Format(localeParameters2.translation, reward));
		TextMeshSpriteIcons.EnsureSpriteIcon(rewardLabel);
		TextMeshHelper.Wrap(rewardLabel, 15);
		lootCrate.gameObject.SetActive(crateType != LootCrateType.None);
		barrelContainer.SetActive(crateType == LootCrateType.None);
		if (crateType != LootCrateType.None)
		{
			lootCrate.initialSkinName = crateType.ToString();
			lootCrate.Initialize(overwrite: true);
			lootCrate.state.SetAnimation(0, "Idle", loop: true);
			lootCrate.Update(Time.deltaTime);
		}
		SetLoading(loading: false);
	}

	private void OnDestroy()
	{
	}

	public new void Close()
	{
		if (!isLoading)
		{
			base.Close();
			if (reward > 0)
			{
				CoroutineRunner.Instance.StartCoroutine(SnoutCoinBurst(reward));
			}
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator SnoutCoinBurst(int reward)
	{
		GameObject go = new GameObject("CurrencyParticleBurst");
		go.transform.position = Vector3.zero;
		SnoutButton.Instance.AddParticles(go, Mathf.Clamp(reward, 1, 50), 0f, Mathf.Clamp(reward, 1, 50));
		yield return new WaitForSeconds(10f);
		if (go != null)
		{
			Object.Destroy(go);
		}
	}
}
