using System;
using UnityEngine;

public class LootWheelRewards
{
	public enum RewardType
	{
		None,
		Dessert,
		Scrap,
		Powerup,
		Part
	}

	public enum WheelReward
	{
		None,
		Dessert1,
		Dessert2,
		Dessert3,
		Scrap1,
		Scrap2,
		Powerup,
		CommonPart,
		RarePart,
		EpicPart
	}

	public struct LootWheelReward
	{
		private int m_amount;

		private int m_value;

		private RewardType m_type;

		private LootCrateRewards.Powerup m_powerup;

		private BasePart m_part;

		public int Amount => m_amount;

		public int SingleValue => m_value;

		public int TotalValue => m_amount * m_value;

		public RewardType Type => m_type;

		public LootCrateRewards.Powerup PowerupReward => m_powerup;

		public BasePart PartReward => m_part;

		public static LootWheelReward Empty => default(LootWheelReward);

		public LootWheelReward(int amount, int value, RewardType type)
		{
			m_amount = amount;
			m_value = value;
			m_type = type;
			m_powerup = LootCrateRewards.Powerup.None;
			m_part = null;
		}

		public LootWheelReward(string key, ConfigData amounts, ConfigData values, RewardType type)
		{
			m_amount = int.Parse(amounts[key]);
			m_value = int.Parse(values[key]);
			m_type = type;
			m_powerup = LootCrateRewards.Powerup.None;
			m_part = null;
		}

		public LootWheelReward(string key, ConfigData amounts, ConfigData values, RewardType type, LootCrateRewards.Powerup powerup)
		{
			m_amount = int.Parse(amounts[key]);
			m_value = int.Parse(values[key]);
			m_type = type;
			m_powerup = powerup;
			m_part = null;
		}

		public LootWheelReward(string key, ConfigData amounts, ConfigData values, RewardType type, BasePart part)
		{
			m_amount = int.Parse(amounts[key]);
			m_value = int.Parse(values[key]);
			m_type = type;
			m_powerup = LootCrateRewards.Powerup.None;
			m_part = part;
		}
	}

	private const string PRIZE_AMOUNTS_CONFIG = "loot_wheel_prize_amounts";

	private const string PRIZE_VALUES_CONFIG = "loot_wheel_prize_values";

	private const string SPIN_PRICE_PARAMS = "loot_wheel_spin_price_params";

	private const string DESSERT_0 = "dessert_0";

	private const string DESSERT_1 = "dessert_1";

	private const string DESSERT_2 = "dessert_2";

	private const string SCRAP_0 = "scrap_0";

	private const string SCRAP_1 = "scrap_1";

	private const string POWERUP = "powerup";

	private const string COMMON = "common";

	private const string RARE = "rare";

	private const string EPIC = "epic";

	private const string VAR_PERCENTAGE = "variation_percentage";

	private const string PRICE_MULTIPLIER = "price_multiplier";

	private bool m_initialized;

	private int m_totalValue;

	private float m_totalInverseValue;

	private float m_rewardValueAvg;

	private float m_spinPriceVariation;

	private float m_spinPriceMultiplier;

	private ConfigData m_amounts;

	private ConfigData m_values;

	public Action OnInitialized;

	public bool Initialized => m_initialized;

	public int TotalRewardValues => m_totalValue;

	public float TotalRewardInverseValues => m_totalInverseValue;

	public float RewardValueAvg => m_rewardValueAvg;

	public float SpinPriceVariation => m_spinPriceVariation;

	public float SpinPriceMultiplier => m_spinPriceMultiplier;

	public LootWheelRewards()
	{
		if (Singleton<GameConfigurationManager>.Instance.HasData)
		{
			Initialize();
			return;
		}
		GameConfigurationManager instance = Singleton<GameConfigurationManager>.Instance;
		instance.OnHasData = (Action)Delegate.Combine(instance.OnHasData, new Action(Initialize));
	}

	private void Initialize()
	{
		GameConfigurationManager instance = Singleton<GameConfigurationManager>.Instance;
		instance.OnHasData = (Action)Delegate.Remove(instance.OnHasData, new Action(Initialize));
		m_amounts = Singleton<GameConfigurationManager>.Instance.GetConfig("loot_wheel_prize_amounts");
		m_values = Singleton<GameConfigurationManager>.Instance.GetConfig("loot_wheel_prize_values");
		m_totalValue = 0;
		for (int i = 0; i < m_values.Keys.Length; i++)
		{
			int num = int.Parse(m_amounts[m_amounts.Keys[i]]);
			int num2 = int.Parse(m_values[m_values.Keys[i]]);
			m_totalValue += num * num2;
		}
		m_totalInverseValue = 0f;
		for (int j = 0; j < m_values.Keys.Length; j++)
		{
			int num3 = int.Parse(m_amounts[m_amounts.Keys[j]]);
			int num4 = int.Parse(m_values[m_values.Keys[j]]);
			m_totalInverseValue += (float)m_totalValue / ((float)num3 * (float)num4);
		}
		m_rewardValueAvg = (float)m_totalValue / (float)m_values.Count;
		if (Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			m_spinPriceMultiplier = 0f;
			m_spinPriceVariation = 0f;
		}
		else
		{
			ConfigData config = Singleton<GameConfigurationManager>.Instance.GetConfig("loot_wheel_spin_price_params");
			m_spinPriceVariation = float.Parse(config["variation_percentage"]);
			m_spinPriceMultiplier = float.Parse(config["price_multiplier"]);
		}
		m_initialized = true;
		if (OnInitialized != null)
		{
			OnInitialized();
		}
	}

	public LootWheelReward GetReward(WheelReward slot)
	{
		return slot switch
		{
			WheelReward.Dessert1 => new LootWheelReward("dessert_0", m_amounts, m_values, RewardType.Dessert), 
			WheelReward.Dessert2 => new LootWheelReward("dessert_1", m_amounts, m_values, RewardType.Dessert), 
			WheelReward.Dessert3 => new LootWheelReward("dessert_2", m_amounts, m_values, RewardType.Dessert), 
			WheelReward.Scrap1 => new LootWheelReward("scrap_0", m_amounts, m_values, RewardType.Scrap), 
			WheelReward.Scrap2 => new LootWheelReward("scrap_1", m_amounts, m_values, RewardType.Scrap), 
			WheelReward.Powerup => new LootWheelReward("powerup", m_amounts, m_values, RewardType.Powerup, GetRandomPowerup()), 
			WheelReward.CommonPart => new LootWheelReward("common", m_amounts, m_values, RewardType.Part, GetRandomPart(BasePart.PartTier.Common)), 
			WheelReward.RarePart => new LootWheelReward("rare", m_amounts, m_values, RewardType.Part, GetRandomPart(BasePart.PartTier.Rare)), 
			WheelReward.EpicPart => new LootWheelReward("epic", m_amounts, m_values, RewardType.Part, GetRandomPart(BasePart.PartTier.Epic)), 
			_ => throw new ArgumentException("Not a valid argument!"), 
		};
	}

	private BasePart GetRandomPart(BasePart.PartTier tier)
	{
		BasePart randomCraftablePartFromTier = CustomizationManager.GetRandomCraftablePartFromTier(tier, onlyLocked: true);
		if (randomCraftablePartFromTier == null)
		{
			randomCraftablePartFromTier = CustomizationManager.GetRandomCraftablePartFromTier(tier);
		}
		return randomCraftablePartFromTier;
	}

	private LootCrateRewards.Powerup GetRandomPowerup()
	{
		return UnityEngine.Random.Range(0, 4) switch
		{
			0 => LootCrateRewards.Powerup.Magnet, 
			1 => LootCrateRewards.Powerup.NightVision, 
			2 => LootCrateRewards.Powerup.Superglue, 
			3 => LootCrateRewards.Powerup.Turbo, 
			_ => LootCrateRewards.Powerup.None, 
		};
	}
}
