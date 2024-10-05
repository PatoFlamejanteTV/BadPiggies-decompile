using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
	private const string GLUE_REWARD_PREFAB = "UI/Amazon/RewardSuperGlue";

	private const string MAGNET_REWARD_PREFAB = "UI/Amazon/RewardSuperMagnet";

	private const string MECHANIC_REWARD_PREFAB = "UI/Amazon/RewardSuperMechanic";

	private const string TURBO_REWARD_PREFAB = "UI/Amazon/RewardTurboCharger";

	private const string NIGHT_VISION_REWARD_PREFAB = "UI/Amazon/RewardNightVision";

	private const string BUNDLE_REWARD_PREFAB = "UI/Amazon/RewardBundle";

	[SerializeField]
	private Transform rewardPosition;

	[SerializeField]
	private GameObject rewardCount;

	[SerializeField]
	private GameObject claimedIcon;

	[SerializeField]
	private GameObject rewardBG;

	[SerializeField]
	private GameObject rewardBGLit;

	private TextMesh countTxt;

	private RewardIcon rewardIcon;

	private Dictionary<PrizeType, GameObject> rewardPrefabs;

	public RewardIcon RewardIcon => rewardIcon;

	public TextMesh RewardCount => countTxt;

	private void Awake()
	{
		countTxt = rewardCount.GetComponent<TextMesh>();
		UpdateBackground();
	}

	private void UpdateBackground(bool lit = false)
	{
		rewardBG.SetActive(!lit);
		rewardBGLit.SetActive(lit);
	}

	public void SetRewards(List<DailyReward> rewards)
	{
		if (rewardIcon != null)
		{
			Object.Destroy(rewardIcon.gameObject);
		}
		if (rewards.Count > 1)
		{
			rewardIcon = ((GameObject)Object.Instantiate(Resources.Load("UI/Amazon/RewardBundle"))).GetComponent<RewardIcon>();
			SetRewardCount(0);
		}
		else
		{
			if (rewards.Count <= 0)
			{
				return;
			}
			rewardIcon = Object.Instantiate(GetRewardPrefab(rewards[0].prize)).GetComponent<RewardIcon>();
			SetRewardCount(rewards[0].prizeCount);
		}
		rewardIcon.transform.parent = base.transform;
		rewardIcon.transform.localPosition = rewardPosition.localPosition;
		rewardIcon.SetButtonState(RewardIcon.State.NotAvailable);
	}

	public GameObject GetRewardPrefab(PrizeType prizeType)
	{
		if (rewardPrefabs == null)
		{
			rewardPrefabs = new Dictionary<PrizeType, GameObject>();
		}
		if (rewardPrefabs.ContainsKey(prizeType))
		{
			return rewardPrefabs[prizeType];
		}
		GameObject gameObject = null;
		switch (prizeType)
		{
		case PrizeType.SuperGlue:
			gameObject = Resources.Load("UI/Amazon/RewardSuperGlue") as GameObject;
			break;
		case PrizeType.SuperMagnet:
			gameObject = Resources.Load("UI/Amazon/RewardSuperMagnet") as GameObject;
			break;
		case PrizeType.TurboCharge:
			gameObject = Resources.Load("UI/Amazon/RewardTurboCharger") as GameObject;
			break;
		case PrizeType.SuperMechanic:
			gameObject = Resources.Load("UI/Amazon/RewardSuperMechanic") as GameObject;
			break;
		case PrizeType.NightVision:
			gameObject = Resources.Load("UI/Amazon/RewardNightVision") as GameObject;
			break;
		}
		rewardPrefabs.Add(prizeType, gameObject);
		return gameObject;
	}

	public void SetState(RewardIcon.State newState)
	{
		if (!(rewardIcon == null))
		{
			rewardIcon.SetButtonState(newState);
			claimedIcon.SetActive(newState == RewardIcon.State.Claimed);
			countTxt.gameObject.SetActive(newState != RewardIcon.State.Claimed);
			UpdateBackground(newState == RewardIcon.State.ClaimNow);
		}
	}

	public void SetDayText(string textKey, int day)
	{
	}

	public void SetRewardCount(int count, string prefix = "x")
	{
		if (!(countTxt == null))
		{
			if (count > 0)
			{
				countTxt.GetComponent<Renderer>().enabled = true;
				countTxt.text = $"{prefix}{count}";
			}
			else
			{
				countTxt.text = string.Empty;
				countTxt.GetComponent<Renderer>().enabled = false;
			}
		}
	}
}
