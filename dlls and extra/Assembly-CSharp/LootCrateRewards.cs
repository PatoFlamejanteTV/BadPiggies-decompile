using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class LootCrateRewards
{
	public enum Powerup
	{
		None,
		Magnet,
		Superglue,
		Turbo,
		Supermechanic,
		NightVision
	}

	public enum Reward
	{
		None,
		Part,
		Powerup,
		Dessert,
		Scrap,
		Coin
	}

	private class RewardSorter<T> : IComparer<T> where T : Tuple<Reward, BasePart.PartTier>
	{
		public int Compare(T x, T y)
		{
			if (x.Item1 == Reward.Part && y.Item1 != Reward.Part)
			{
				return -1;
			}
			if (y.Item1 == Reward.Part && x.Item1 != Reward.Part)
			{
				return 1;
			}
			if (x.Item1 == y.Item1 && x.Item1 == Reward.Part)
			{
				return y.Item2 - x.Item2;
			}
			return 0;
		}
	}

	public struct SlotRewards
	{
		private Reward type;

		private int value;

		public Reward Type => type;

		public BasePart.PartTier PartTier
		{
			get
			{
				if (type == Reward.Part)
				{
					return (BasePart.PartTier)value;
				}
				return BasePart.PartTier.Regular;
			}
		}

		public Powerup Powerup
		{
			get
			{
				if (type == Reward.Powerup)
				{
					return (Powerup)value;
				}
				return Powerup.None;
			}
		}

		public int Desserts
		{
			get
			{
				if (type == Reward.Dessert)
				{
					return value;
				}
				return 0;
			}
		}

		public int Scrap
		{
			get
			{
				if (type == Reward.Scrap)
				{
					return value;
				}
				return 0;
			}
		}

		public int Coins
		{
			get
			{
				if (type == Reward.Coin)
				{
					return value;
				}
				return 0;
			}
		}

		public bool GoldenCupcake
		{
			get
			{
				if (type == Reward.Dessert)
				{
					return value == 0;
				}
				return false;
			}
		}

		public SlotRewards(Reward type, int value)
		{
			this.type = type;
			this.value = value;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Reward type: ");
			stringBuilder.Append(type);
			stringBuilder.Append("\n");
			switch (type)
			{
			case Reward.Part:
				stringBuilder.Append("Part tier: ");
				stringBuilder.Append(PartTier);
				break;
			case Reward.Powerup:
				stringBuilder.Append("Powerup type: ");
				stringBuilder.Append(Powerup);
				break;
			case Reward.Dessert:
				stringBuilder.Append("Dessert count: ");
				stringBuilder.Append(Desserts);
				break;
			case Reward.Scrap:
				stringBuilder.Append("Scrap count: ");
				stringBuilder.Append(Scrap);
				break;
			case Reward.Coin:
				stringBuilder.Append("Coin amount: ");
				stringBuilder.Append(Coins);
				break;
			}
			return stringBuilder.ToString();
		}
	}

	private class Slot
	{
		private const string PART_CHANCES = "PartChances";

		private const string POWERUP_CHANCES = "PowerupChances";

		private const string DESSERT_CHANCES = "DessertChances";

		private const string SCRAP_VARIATION = "ScrapVariation";

		private const string SCRAP_CHANCES = "ScrapChances";

		private const string COIN_CHANCES = "CoinChances";

		private const string PART_CHANCE = "PartChance";

		private const string POWERUP_CHANCE = "PowerupChance";

		private const string DESSERT_CHANCE = "DessertChance";

		private const string SCRAP_CHANCE = "ScrapChance";

		private const string COIN_CHANCE = "CoinChance";

		private const string LOOT_BOX_MASK = "LootBoxMask";

		private const string GOLDEN_CUPCAKE_CHANCE = "GoldenCupCakeChance";

		private List<Tuple<BasePart.PartTier, int>> partChances;

		private List<Tuple<Powerup, int>> powerupChances;

		private List<Tuple<int, int>> dessertChances;

		private List<Tuple<int, int>> scrapChances;

		private List<Tuple<int, int>> coinChances;

		private List<Tuple<Reward, int>> chances;

		private List<Tuple<Reward, int>> noDessertChances;

		private int scrapVariation;

		private int lootBoxMask;

		private List<Tuple<Reward, int>> CurrentChances
		{
			get
			{
				if (Application.isPlaying && !INSettings.GetBool(INFeature.EnableDesserts))
				{
					if (GameProgress.GetBool("ChiefPigExploded"))
					{
						return chances;
					}
					return noDessertChances;
				}
				return chances;
			}
		}

		private int ChanceMax => MaxChance(Reward.Part) + MaxChance(Reward.Powerup) + MaxChance(Reward.Scrap) + MaxChance(Reward.Coin) + MaxChance(Reward.Dessert);

		public Slot(Hashtable data)
		{
			chances = new List<Tuple<Reward, int>>();
			noDessertChances = new List<Tuple<Reward, int>>();
			int num = Convert.ToInt32(data["PartChance"]);
			int num2 = Convert.ToInt32(data["PowerupChance"]);
			int num3 = Convert.ToInt32(data["DessertChance"]);
			int num4 = Convert.ToInt32(data["ScrapChance"]);
			int num5 = Convert.ToInt32(data["CoinChance"]);
			int num6 = Convert.ToInt32(data["GoldenCupCakeChance"]);
			scrapVariation = Convert.ToInt32(data["ScrapVariation"]);
			lootBoxMask = Convert.ToInt32(data["LootBoxMask"]);
			if (num > 0)
			{
				partChances = FromHashtable<BasePart.PartTier, int>(data["PartChances"] as Hashtable);
				chances.Add(new Tuple<Reward, int>(Reward.Part, num));
				noDessertChances.Add(new Tuple<Reward, int>(Reward.Part, num));
			}
			if (num2 > 0)
			{
				powerupChances = FromHashtable<Powerup, int>(data["PowerupChances"] as Hashtable);
				int num7 = ((chances.Count > 0) ? chances[chances.Count - 1].Item2 : 0);
				chances.Add(new Tuple<Reward, int>(Reward.Powerup, num2 + num7));
				noDessertChances.Add(new Tuple<Reward, int>(Reward.Powerup, num2 + num7));
			}
			if (num3 > 0)
			{
				dessertChances = FromHashtable<int, int>(data["DessertChances"] as Hashtable);
				int num8 = ((chances.Count > 0) ? chances[chances.Count - 1].Item2 : 0);
				chances.Add(new Tuple<Reward, int>(Reward.Dessert, num3 + num8));
			}
			if (num4 > 0)
			{
				scrapChances = FromHashtable<int, int>(data["ScrapChances"] as Hashtable);
				int num9 = ((chances.Count > 0) ? chances[chances.Count - 1].Item2 : 0);
				chances.Add(new Tuple<Reward, int>(Reward.Scrap, num4 + num9));
				num9 = ((noDessertChances.Count > 0) ? noDessertChances[noDessertChances.Count - 1].Item2 : 0);
				noDessertChances.Add(new Tuple<Reward, int>(Reward.Scrap, num4 + num9));
			}
			if (num5 > 0)
			{
				coinChances = FromHashtable<int, int>(data["CoinChances"] as Hashtable);
				int num10 = ((chances.Count > 0) ? chances[chances.Count - 1].Item2 : 0);
				chances.Add(new Tuple<Reward, int>(Reward.Coin, num5 + num10));
				num10 = ((noDessertChances.Count > 0) ? noDessertChances[noDessertChances.Count - 1].Item2 : 0);
				noDessertChances.Add(new Tuple<Reward, int>(Reward.Coin, num5 + num10));
			}
			if (num6 > 0)
			{
				if (dessertChances == null)
				{
					dessertChances = new List<Tuple<int, int>>();
				}
				int num11 = ((dessertChances.Count > 0) ? dessertChances[dessertChances.Count - 1].Item2 : 0);
				dessertChances.Add(new Tuple<int, int>(0, num6 + num11));
			}
		}

		private List<Tuple<T1, T2>> FromHashtable<T1, T2>(Hashtable data)
		{
			List<Tuple<T1, T2>> list = new List<Tuple<T1, T2>>();
			int num = 0;
			foreach (object key in data.Keys)
			{
				int num2 = (int)Convert.ChangeType(data[key], typeof(int));
				if (num2 > 0)
				{
					string text = (string)Convert.ChangeType(key, typeof(string));
					T1 item = ((!typeof(T1).IsEnum) ? ((T1)Convert.ChangeType(int.Parse(text), typeof(T1))) : ((T1)Enum.Parse(typeof(T1), text)));
					list.Add(new Tuple<T1, T2>(item, (T2)Convert.ChangeType(num2 + num, typeof(T2))));
					num += num2;
				}
			}
			return list;
		}

		public SlotRewards GetRewards()
		{
			List<Tuple<Reward, int>> currentChances = CurrentChances;
			Reward reward = Reward.None;
			int value = 0;
			int num = UnityEngine.Random.Range(0, currentChances[currentChances.Count - 1].Item2);
			for (int i = 0; i < currentChances.Count; i++)
			{
				if (num < currentChances[i].Item2)
				{
					reward = currentChances[i].Item1;
					break;
				}
			}
			switch (reward)
			{
			case Reward.Part:
				num = UnityEngine.Random.Range(0, partChances[partChances.Count - 1].Item2);
				value = (int)partChances[GetRewardIndex(partChances, num)].Item1;
				break;
			case Reward.Powerup:
				num = UnityEngine.Random.Range(0, powerupChances[powerupChances.Count - 1].Item2);
				value = (int)powerupChances[GetRewardIndex(powerupChances, num)].Item1;
				break;
			case Reward.Dessert:
				num = UnityEngine.Random.Range(0, dessertChances[dessertChances.Count - 1].Item2);
				value = dessertChances[GetRewardIndex(dessertChances, num)].Item1;
				break;
			case Reward.Scrap:
			{
				num = UnityEngine.Random.Range(0, scrapChances[scrapChances.Count - 1].Item2);
				value = scrapChances[GetRewardIndex(scrapChances, num)].Item1;
				float num2 = UnityEngine.Random.Range(-scrapVariation, scrapVariation);
				value = Mathf.RoundToInt((float)value * (1f + num2 / 100f));
				break;
			}
			case Reward.Coin:
				num = UnityEngine.Random.Range(0, coinChances[coinChances.Count - 1].Item2);
				value = coinChances[GetRewardIndex(coinChances, num)].Item1;
				break;
			}
			return new SlotRewards(reward, value);
		}

		public Tuple<Reward, BasePart.PartTier> MinimumReward()
		{
			if (MaxChance(Reward.Part) == ChanceMax)
			{
				if (PartChance(BasePart.PartTier.Common) > 0)
				{
					return new Tuple<Reward, BasePart.PartTier>(Reward.Part, BasePart.PartTier.Common);
				}
				if (PartChance(BasePart.PartTier.Rare) > 0)
				{
					return new Tuple<Reward, BasePart.PartTier>(Reward.Part, BasePart.PartTier.Rare);
				}
				return new Tuple<Reward, BasePart.PartTier>(Reward.Part, BasePart.PartTier.Epic);
			}
			if (MaxChance(Reward.Dessert) > 0)
			{
				return new Tuple<Reward, BasePart.PartTier>(Reward.Dessert, BasePart.PartTier.Regular);
			}
			if (MaxChance(Reward.Scrap) > 0)
			{
				return new Tuple<Reward, BasePart.PartTier>(Reward.Scrap, BasePart.PartTier.Regular);
			}
			if (MaxChance(Reward.Powerup) > 0)
			{
				return new Tuple<Reward, BasePart.PartTier>(Reward.Powerup, BasePart.PartTier.Regular);
			}
			if (MaxChance(Reward.Coin) > 0)
			{
				return new Tuple<Reward, BasePart.PartTier>(Reward.Coin, BasePart.PartTier.Regular);
			}
			return null;
		}

		private int MaxChance(Reward type)
		{
			switch (type)
			{
			case Reward.Part:
				if (partChances != null)
				{
					return partChances[partChances.Count - 1].Item2;
				}
				return 0;
			case Reward.Powerup:
				if (powerupChances != null)
				{
					return powerupChances[powerupChances.Count - 1].Item2;
				}
				return 0;
			case Reward.Dessert:
				if (dessertChances != null)
				{
					return dessertChances[dessertChances.Count - 1].Item2;
				}
				return 0;
			case Reward.Scrap:
				if (scrapChances != null)
				{
					return scrapChances[scrapChances.Count - 1].Item2;
				}
				return 0;
			case Reward.Coin:
				if (coinChances != null)
				{
					return coinChances[coinChances.Count - 1].Item2;
				}
				return 0;
			default:
				return 0;
			}
		}

		private int PartChance(BasePart.PartTier tier)
		{
			int result = 0;
			for (int i = 0; i < partChances.Count; i++)
			{
				if (partChances[i].Item1 == tier)
				{
					result = partChances[i].Item2;
					break;
				}
			}
			return result;
		}

		private int GetRewardIndex<T1, T2>(List<Tuple<T1, T2>> list, int value)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int num = Convert.ToInt32(list[i].Item2);
				if (value < num)
				{
					return i;
				}
			}
			return 0;
		}

		public bool ForLootCrate(LootCrateType type)
		{
			int num = 1 << (int)type;
			return (num & lootBoxMask) == num;
		}
	}

	private static bool initialized;

	private static Dictionary<LootCrateType, List<Slot>> slots;

	private static SecureJsonManager secureJson;

	public static bool Initialized => initialized;

	private static Dictionary<LootCrateType, List<Slot>> Slots
	{
		get
		{
			if (slots == null)
			{
				slots = new Dictionary<LootCrateType, List<Slot>>();
				int num = Enum.GetNames(typeof(LootCrateType)).Length - 1;
				for (int i = 0; i < num; i++)
				{
					LootCrateType key = (LootCrateType)i;
					slots.Add(key, new List<Slot>());
				}
			}
			return slots;
		}
		set
		{
			slots = value;
		}
	}

	static LootCrateRewards()
	{
		secureJson = new SecureJsonManager("lootcraterewards");
		secureJson.Initialize(Initialize);
	}

	private static void Initialize(string rawData)
	{
		ArrayList obj = (MiniJSON.jsonDecode(rawData) as Hashtable)["data"] as ArrayList;
		int num = Enum.GetNames(typeof(LootCrateType)).Length - 1;
		foreach (Hashtable item in obj)
		{
			Slot slot = new Slot(item);
			for (int i = 0; i < num; i++)
			{
				LootCrateType lootCrateType = (LootCrateType)i;
				if (slot.ForLootCrate(lootCrateType))
				{
					Slots[lootCrateType].Add(slot);
				}
			}
		}
		initialized = true;
	}

	public static SlotRewards[] GetRandomRewards(LootCrateType type)
	{
		if (type == LootCrateType.None)
		{
			return null;
		}
		SlotRewards[] array = new SlotRewards[Slots[type].Count];
		for (int i = 0; i < Slots[type].Count; i++)
		{
			array[i] = Slots[type][i].GetRewards();
		}
		return array;
	}

	public static List<Tuple<Reward, BasePart.PartTier>> MinimumRewards(LootCrateType type)
	{
		List<Tuple<Reward, BasePart.PartTier>> list = new List<Tuple<Reward, BasePart.PartTier>>();
		if (!initialized)
		{
			return list;
		}
		List<Slot> list2 = slots[type];
		for (int i = 0; i < list2.Count; i++)
		{
			list.Add(list2[i].MinimumReward());
		}
		list.Sort(new RewardSorter<Tuple<Reward, BasePart.PartTier>>());
		return list;
	}
}
