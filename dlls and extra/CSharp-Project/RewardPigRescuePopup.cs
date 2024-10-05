using System.Collections;
using UnityEngine;

public class RewardPigRescuePopup : MonoBehaviour
{
	public enum RewardType
	{
		None = -1,
		Turbo,
		Glue,
		Magnet,
		Nightvision,
		Supermechanic
	}

	public MeshRenderer[] rewardIcons;

	public TextMesh rewardCountText;

	private const string PIG_RESCUE_REWARD_TYPE_KEY = "PigRescueRewardType";

	private const string PIG_RESCUE_REWARD_COUNT_KEY = "PigRescueRewardCount";

	private bool isClaimSequence;

	private static RewardType CurrentRewardType => (RewardType)GameProgress.GetInt("PigRescueRewardType", -1);

	private static int CurrentRewardCount => GameProgress.GetInt("PigRescueRewardCount", -1);

	public static bool HasRewardPending
	{
		get
		{
			if (CurrentRewardType != RewardType.None)
			{
				return CurrentRewardCount >= 0;
			}
			return false;
		}
	}

	public static void SetRewardData(RewardType rewardType, int rewardCount)
	{
		GameProgress.SetInt("PigRescueRewardType", (int)rewardType);
		GameProgress.SetInt("PigRescueRewardCount", rewardCount);
	}

	public static void CheckReward(GameObject go, string callbackMethodName)
	{
	}

	public static void ProcessReward(string rewardData)
	{
		string[] array = rewardData.Split(',');
		if (array.Length == 2 && !string.IsNullOrEmpty(array[0]) && !string.IsNullOrEmpty(array[1]) && int.TryParse(array[0], out var result) && result >= 0 && result <= 4)
		{
			RewardType rewardType = (RewardType)result;
			if (int.TryParse(array[1], out var result2))
			{
				SetRewardData(rewardType, result2);
			}
		}
	}

	private void OnEnable()
	{
		if (rewardIcons == null || !HasRewardPending)
		{
			ClosePopup();
			return;
		}
		int currentRewardType = (int)CurrentRewardType;
		for (int i = 0; i < rewardIcons.Length; i++)
		{
			if (rewardIcons[i] != null)
			{
				rewardIcons[i].enabled = i == currentRewardType;
			}
		}
		rewardCountText.text = $"x{CurrentRewardCount}";
	}

	private void OnDisable()
	{
		if (isClaimSequence)
		{
			ClosePopup();
		}
	}

	public void ClaimReward()
	{
		RewardType currentRewardType = CurrentRewardType;
		int currentRewardCount = CurrentRewardCount;
		GameProgress.DeleteKey("PigRescueRewardType");
		GameProgress.DeleteKey("PigRescueRewardCount");
		if (currentRewardType != RewardType.None && currentRewardCount > 0)
		{
			switch (currentRewardType)
			{
			case RewardType.Turbo:
				GameProgress.AddTurboCharge(currentRewardCount);
				break;
			case RewardType.Glue:
				GameProgress.AddSuperGlue(currentRewardCount);
				break;
			case RewardType.Magnet:
				GameProgress.AddSuperMagnet(currentRewardCount);
				break;
			case RewardType.Nightvision:
				GameProgress.AddNightVision(currentRewardCount);
				break;
			case RewardType.Supermechanic:
				GameProgress.AddBluePrints(currentRewardCount);
				break;
			}
			StartCoroutine(ClaimSequence());
		}
	}

	private IEnumerator ClaimSequence()
	{
		isClaimSequence = true;
		Transform transform = base.transform.Find("RewardIcons");
		if (transform != null)
		{
			Transform transform2 = transform.Find("RewardCountText");
			if (transform2 != null)
			{
				TextMesh component = transform2.GetComponent<TextMesh>();
				if (component != null)
				{
					component.text = component.text.Replace("x", "+");
				}
			}
			Animation anim = transform.GetComponent<Animation>();
			if (anim != null)
			{
				anim.Play("RewardClaim");
				while (anim.IsPlaying("RewardClaim"))
				{
					yield return null;
				}
			}
		}
		isClaimSequence = false;
		ClosePopup();
	}

	private void ClosePopup()
	{
		Object.Destroy(base.gameObject);
	}
}
