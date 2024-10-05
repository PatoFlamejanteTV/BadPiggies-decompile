using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DailyRewardBundle
{
	[SerializeField]
	private bool isBundle;

	[SerializeField]
	private List<DailyReward> rewards;

	public List<DailyReward> GetRewards(int level)
	{
		if (isBundle)
		{
			return rewards;
		}
		UnityEngine.Random.InitState(RewardSystem.RandomSeed(level));
		int index = UnityEngine.Random.Range(0, rewards.Count);
		return new List<DailyReward> { rewards[index] };
	}
}
