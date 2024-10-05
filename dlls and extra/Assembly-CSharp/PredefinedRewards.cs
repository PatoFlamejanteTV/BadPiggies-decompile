using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredefinedRewards : Singleton<PredefinedRewards>
{
	private BasePart.PartType firstReward;

	private List<BasePart.PartType> rewards;

	private bool initialized;

	private int rewardsAmount;

	public bool Initialized => initialized;

	public bool AllRewardsGiven => RewardsGiven >= rewardsAmount;

	private bool FirstRewardGiven
	{
		get
		{
			return GameProgress.GetBool("Pre_FirstRewardGiven");
		}
		set
		{
			GameProgress.SetBool("Pre_FirstRewardGiven", value);
		}
	}

	private int RewardsGiven
	{
		get
		{
			return GameProgress.GetInt("Pre_RewardsGiven");
		}
		set
		{
			GameProgress.SetInt("Pre_RewardsGiven", value);
		}
	}

	private void Awake()
	{
		SetAsPersistant();
		if (Singleton<GameConfigurationManager>.Instance.HasData)
		{
			Initialize();
			return;
		}
		GameConfigurationManager gameConfigurationManager = Singleton<GameConfigurationManager>.Instance;
		gameConfigurationManager.OnHasData = (Action)Delegate.Combine(gameConfigurationManager.OnHasData, new Action(Initialize));
	}

	private void Initialize()
	{
		rewards = new List<BasePart.PartType>();
		Hashtable values = Singleton<GameConfigurationManager>.Instance.GetValues("first_predefined_reward");
		Hashtable values2 = Singleton<GameConfigurationManager>.Instance.GetValues("predefined_rewards");
		if (values.ContainsKey("reward"))
		{
			if (!TryParse(typeof(BasePart.PartType), values["reward"] as string, out var target))
			{
				return;
			}
			firstReward = (BasePart.PartType)target;
		}
		if (values2.ContainsKey("amount") && int.TryParse(values2["amount"] as string, out rewardsAmount))
		{
			for (int i = 0; i < rewardsAmount; i++)
			{
				if (!values2.ContainsKey("reward_" + i) || !TryParse(typeof(BasePart.PartType), values2["reward_" + i] as string, out var target2))
				{
					return;
				}
				BasePart.PartType type = (BasePart.PartType)target2;
				if (!GetRewardGiven(type))
				{
					rewards.Add((BasePart.PartType)target2);
				}
			}
		}
		initialized = true;
	}

	private bool TryParse(Type type, string value, out object target)
	{
		target = null;
		try
		{
			target = Enum.Parse(type, value);
			return target != null;
		}
		catch (ArgumentException ex)
		{
			_ = ex is ArgumentNullException;
		}
		return false;
	}

	public bool GetReward(BasePart.PartTier tier, out BasePart part)
	{
		part = null;
		if (rewards != null && rewards.Count > 0)
		{
			int num = UnityEngine.Random.Range(0, rewards.Count);
			int num2 = num;
			do
			{
				num++;
				if (num >= rewards.Count)
				{
					num = 0;
				}
				BasePart.PartType type = ((!FirstRewardGiven) ? firstReward : rewards[num]);
				List<BasePart> customParts = CustomizationManager.GetCustomParts(type, tier, onlyLocked: true);
				if (customParts.Count <= 0)
				{
					continue;
				}
				int num3 = UnityEngine.Random.Range(0, customParts.Count);
				int num4 = num3;
				do
				{
					num3++;
					if (num3 >= customParts.Count)
					{
						num3 = 0;
					}
					if (customParts[num3].craftable)
					{
						part = customParts[num3];
						if (FirstRewardGiven)
						{
							SetRewardGiven(type, given: true);
						}
						else
						{
							FirstRewardGiven = true;
						}
						num = num2;
						num3 = num4;
					}
				}
				while (num3 != num4);
			}
			while (num != num2);
		}
		return part != null;
	}

	public static PredefinedRewards Create()
	{
		if (Singleton<PredefinedRewards>.Instance == null)
		{
			return new GameObject("PredefinedRewards (Singleton)").AddComponent<PredefinedRewards>();
		}
		return Singleton<PredefinedRewards>.Instance;
	}

	private bool GetRewardGiven(BasePart.PartType type)
	{
		return GameProgress.GetBool("Pre_RewardGiven_" + type);
	}

	private void SetRewardGiven(BasePart.PartType type, bool given)
	{
		if (given && rewards.Contains(type))
		{
			rewards.Remove(type);
		}
		if (given)
		{
			RewardsGiven++;
		}
		GameProgress.SetBool("Pre_RewardGiven_" + type, given);
	}
}
