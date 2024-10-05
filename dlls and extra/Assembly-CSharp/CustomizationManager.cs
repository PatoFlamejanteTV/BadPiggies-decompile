using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationManager
{
	[Flags]
	public enum PartFlags
	{
		None = 0,
		Locked = 1,
		Craftable = 2,
		Rewardable = 4
	}

	public static Action<BasePart> OnPartUnlocked;

	private const string NEW_PARTS_KEY = "NewParts";

	private static int cachedUnlockedPartCount = -1;

	private static int[] cachedPartTierCount = new int[4] { -1, -1, -1, -1 };

	public static void UnlockPart(BasePart part, string unlockType)
	{
		if (!IsPartUnlocked(part))
		{
			cachedUnlockedPartCount = -1;
			GameProgress.SetBool(part.name, value: true);
			SetPartNew(part, isNew: true);
			SetPartUsed(part, used: false);
			CheckUnlockPartAchievements();
			if (OnPartUnlocked != null)
			{
				OnPartUnlocked(part);
			}
		}
	}

	public static bool IsPartUnlocked(BasePart part)
	{
		if (INSettings.GetBool(INFeature.UnlockCustomParts))
		{
			return true;
		}
		return GameProgress.GetBool(part.name);
	}

	public static bool HasNewParts()
	{
		return GameProgress.GetInt("NewParts") > 0;
	}

	public static int NewPartCount()
	{
		return GameProgress.GetInt("NewParts");
	}

	public static void SetPartNew(BasePart part, bool isNew)
	{
		string key = $"{part.name}_isNew";
		if (isNew && !GameProgress.HasKey(key))
		{
			GameProgress.SetBool(key, isNew);
			GameProgress.SetInt("NewParts", GameProgress.GetInt("NewParts") + 1);
		}
		else if (!isNew && GameProgress.HasKey(key) && GameProgress.GetBool(key))
		{
			GameProgress.SetBool(key, isNew);
			GameProgress.SetInt("NewParts", GameProgress.GetInt("NewParts") - 1);
		}
	}

	public static bool IsPartNew(BasePart part)
	{
		return GameProgress.GetBool($"{part.name}_isNew");
	}

	public static bool IsPartUsed(BasePart part)
	{
		return GameProgress.GetBool($"{part.name}_isUsed");
	}

	public static void SetPartUsed(BasePart part, bool used)
	{
		string key = $"{part.name}_isUsed";
		if (!used && !GameProgress.HasKey(key))
		{
			GameProgress.SetBool(key, used);
		}
		else if (used && !GameProgress.GetBool(key))
		{
			GameProgress.SetBool(key, used);
			PlayerProgress.ExperienceType experienceType = PlayerProgress.ExperienceType.NewCommonPartViewed;
			if (part.m_partTier == BasePart.PartTier.Rare)
			{
				experienceType = PlayerProgress.ExperienceType.NewRarePartViewed;
			}
			else if (part.m_partTier == BasePart.PartTier.Epic)
			{
				experienceType = PlayerProgress.ExperienceType.NewEpicPartViewed;
			}
			else if (part.m_partTier == BasePart.PartTier.Legendary)
			{
				experienceType = PlayerProgress.ExperienceType.NewLegendaryPartViewed;
			}
			Singleton<PlayerProgress>.Instance.AddExperience(experienceType);
		}
	}

	private static bool HasPartFlags(BasePart part, PartFlags flags)
	{
		if (part == null)
		{
			return false;
		}
		PartFlags partFlags = PartFlags.None;
		if (!IsPartUnlocked(part))
		{
			partFlags |= PartFlags.Locked;
		}
		if (part.craftable)
		{
			partFlags |= PartFlags.Craftable;
		}
		if (part.lootCrateReward)
		{
			partFlags |= PartFlags.Rewardable;
		}
		return (partFlags & flags) == flags;
	}

	public static int GetUnlockedPartCount(bool useTier = false, BasePart.PartTier tier = BasePart.PartTier.Common)
	{
		int num = 0;
		if (!useTier)
		{
			num = cachedUnlockedPartCount;
		}
		if (num < 0 || useTier)
		{
			num = 0;
			List<CustomPartInfo> customParts = WPFMonoBehaviour.gameData.m_customParts;
			for (int i = 0; i < customParts.Count; i++)
			{
				if (customParts[i] == null || customParts[i].PartList == null || customParts[i].PartList.Count == 0)
				{
					continue;
				}
				for (int j = 0; j < customParts[i].PartList.Count; j++)
				{
					if (IsPartUnlocked(customParts[i].PartList[j]) && (!useTier || (useTier && customParts[i].PartList[j].m_partTier == tier)))
					{
						num++;
					}
				}
			}
		}
		if (!useTier)
		{
			cachedUnlockedPartCount = num;
		}
		return num;
	}

	public static int GetTotalPartCountForTier(BasePart.PartTier tier)
	{
		int num = cachedPartTierCount[(int)tier];
		if (num < 0)
		{
			num = GetAllTierParts(tier).Count;
		}
		return num;
	}

	public static List<BasePart> GetAllTierParts(BasePart.PartTier tier, PartFlags flags = PartFlags.None)
	{
		List<BasePart> list = new List<BasePart>();
		if (tier == BasePart.PartTier.Regular)
		{
			for (int i = 0; i < WPFMonoBehaviour.gameData.m_parts.Count; i++)
			{
				if (flags == PartFlags.None)
				{
					list.Add(WPFMonoBehaviour.gameData.m_parts[i].GetComponent<BasePart>());
				}
			}
		}
		else
		{
			List<CustomPartInfo> customParts = WPFMonoBehaviour.gameData.m_customParts;
			for (int j = 0; j < customParts.Count; j++)
			{
				if (customParts[j] == null || customParts[j].PartList == null || customParts[j].PartList.Count == 0)
				{
					continue;
				}
				for (int k = 0; k < customParts[j].PartList.Count; k++)
				{
					if (customParts[j].PartList[k].m_partTier == tier && HasPartFlags(customParts[j].PartList[k], flags))
					{
						list.Add(customParts[j].PartList[k]);
					}
				}
			}
		}
		return list;
	}

	public static List<BasePart> GetCustomParts(BasePart.PartType type, BasePart.PartTier tier, bool onlyLocked = false)
	{
		List<BasePart> list = new List<BasePart>();
		List<CustomPartInfo> customParts = WPFMonoBehaviour.gameData.m_customParts;
		for (int i = 0; i < customParts.Count; i++)
		{
			if (customParts[i].PartType != type)
			{
				continue;
			}
			for (int j = 0; j < customParts[i].PartList.Count; j++)
			{
				if (customParts[i].PartList[j].m_partTier == tier)
				{
					if (onlyLocked && !IsPartUnlocked(customParts[i].PartList[j]))
					{
						list.Add(customParts[i].PartList[j]);
					}
					else if (!onlyLocked)
					{
						list.Add(customParts[i].PartList[j]);
					}
				}
			}
			break;
		}
		return list;
	}

	public static List<BasePart> GetCustomParts(BasePart.PartType type, bool onlyLocked = false)
	{
		List<BasePart> list = new List<BasePart>();
		List<CustomPartInfo> customParts = WPFMonoBehaviour.gameData.m_customParts;
		for (int i = 0; i < customParts.Count; i++)
		{
			if (customParts[i].PartType != type)
			{
				continue;
			}
			for (int j = 0; j < customParts[i].PartList.Count; j++)
			{
				if (onlyLocked && !IsPartUnlocked(customParts[i].PartList[j]))
				{
					list.Add(customParts[i].PartList[j]);
				}
				else if (!onlyLocked)
				{
					list.Add(customParts[i].PartList[j]);
				}
			}
			break;
		}
		return list;
	}

	public static int CustomizationCount(BasePart.PartTier tier, PartFlags flags = PartFlags.None)
	{
		int num = 0;
		List<CustomPartInfo> customParts = WPFMonoBehaviour.gameData.m_customParts;
		for (int i = 0; i < customParts.Count; i++)
		{
			if (customParts[i] == null || customParts[i].PartList == null || customParts[i].PartList.Count == 0)
			{
				continue;
			}
			for (int j = 0; j < customParts[i].PartList.Count; j++)
			{
				if (customParts[i].PartList[j].m_partTier == tier && HasPartFlags(customParts[i].PartList[j], flags))
				{
					num++;
				}
			}
		}
		return num;
	}

	public static BasePart GetRandomLootCrateRewardPartFromTier(BasePart.PartTier tier, bool onlyLocked = false)
	{
		if (!Singleton<PredefinedRewards>.Instance.AllRewardsGiven && Singleton<PredefinedRewards>.Instance.GetReward(tier, out var part))
		{
			return part;
		}
		List<BasePart> allTierParts = GetAllTierParts(tier, onlyLocked ? PartFlags.Locked : PartFlags.None);
		if (allTierParts.Count == 0)
		{
			return null;
		}
		int num = UnityEngine.Random.Range(0, allTierParts.Count);
		int num2 = num;
		while (!allTierParts[num].lootCrateReward)
		{
			num++;
			if (num >= allTierParts.Count)
			{
				num = 0;
			}
			if (num2 == num)
			{
				return null;
			}
		}
		return allTierParts[num];
	}

	public static BasePart GetRandomCraftablePartFromTier(BasePart.PartTier tier, bool onlyLocked = false)
	{
		if (!Singleton<PredefinedRewards>.Instance.AllRewardsGiven && Singleton<PredefinedRewards>.Instance.GetReward(tier, out var part))
		{
			return part;
		}
		List<BasePart> allTierParts = GetAllTierParts(tier, onlyLocked ? PartFlags.Locked : PartFlags.None);
		if (allTierParts.Count == 0)
		{
			return null;
		}
		int num = UnityEngine.Random.Range(0, allTierParts.Count);
		int num2 = num;
		while (!allTierParts[num].craftable)
		{
			num++;
			if (num >= allTierParts.Count)
			{
				num = 0;
			}
			if (num2 == num)
			{
				return null;
			}
		}
		return allTierParts[num];
	}

	public static BasePart GetRandomPartFromTier(BasePart.PartTier tier, bool onlyLocked = false)
	{
		List<BasePart> allTierParts = GetAllTierParts(tier, onlyLocked ? PartFlags.Locked : PartFlags.None);
		if (allTierParts.Count == 0)
		{
			return null;
		}
		int index = UnityEngine.Random.Range(0, allTierParts.Count);
		return allTierParts[index];
	}

	public static void SetLastUsedPartIndex(BasePart.PartType partType, int index)
	{
		UserSettings.SetInt("LastUsed_" + partType, index);
	}

	public static int GetLastUsedPartIndex(BasePart.PartType partType)
	{
		int @int = UserSettings.GetInt("LastUsed_" + partType);
		if (WPFMonoBehaviour.gameData.GetCustomPart(partType, @int) != null)
		{
			return @int;
		}
		return 0;
	}

	private static void CheckUnlockPartAchievements()
	{
		if (!Singleton<SocialGameManager>.IsInstantiated())
		{
			return;
		}
		bool flag = false;
		int common = 0;
		int rare = 0;
		int epic = 0;
		List<BasePart> allTierParts = GetAllTierParts(BasePart.PartTier.Common);
		List<BasePart> allTierParts2 = GetAllTierParts(BasePart.PartTier.Rare);
		List<BasePart> allTierParts3 = GetAllTierParts(BasePart.PartTier.Epic);
		foreach (BasePart item in allTierParts)
		{
			if (IsPartUnlocked(item))
			{
				common++;
				if (item.tags != null && item.tags.Contains("Gold"))
				{
					flag = true;
				}
			}
		}
		foreach (BasePart item2 in allTierParts2)
		{
			if (IsPartUnlocked(item2))
			{
				rare++;
				if (item2.tags != null && item2.tags.Contains("Gold"))
				{
					flag = true;
				}
			}
		}
		foreach (BasePart item3 in allTierParts3)
		{
			if (IsPartUnlocked(item3))
			{
				epic++;
				if (item3.tags != null && item3.tags.Contains("Gold"))
				{
					flag = true;
				}
			}
		}
		if (common > 0)
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.COMMON_COLLECTOR", 100.0, (int limit) => common >= limit);
		}
		if (rare > 0)
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.RARE_COLLECTOR", 100.0, (int limit) => rare >= limit);
		}
		if (epic > 0)
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.EPIC_COLLECTOR", 100.0, (int limit) => epic >= limit);
		}
		if (flag)
		{
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.GET_GOLDEN_PART", 100.0);
		}
	}
}
