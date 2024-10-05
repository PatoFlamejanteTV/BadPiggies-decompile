using System;
using System.Collections.Generic;

public static class AlienCustomizationManager
{
	private const string ALIEN_CRAFT_CONFIG_NAME = "alien_part_craft_order";

	private const string ALIEN_PRICE_CONFIG_NAME = "none";

	private static bool s_initialized;

	private static List<BasePart> s_unlockOrder;

	public static bool Initialized => s_initialized;

	public static bool HasCraftableItems => UnlockablesLeft() > 0;

	static AlienCustomizationManager()
	{
		if (Singleton<GameConfigurationManager>.Instance.HasData)
		{
			Initialize();
			return;
		}
		GameConfigurationManager instance = Singleton<GameConfigurationManager>.Instance;
		Delegate onHasData = instance.OnHasData;
		instance.OnHasData = (Action)Delegate.Combine(onHasData, new Action(Initialize));
	}

	private static void Initialize()
	{
		if (s_initialized)
		{
			return;
		}
		s_unlockOrder = new List<BasePart>();
		ConfigData config = Singleton<GameConfigurationManager>.Instance.GetConfig("alien_part_craft_order");
		List<BasePart> allTierParts = CustomizationManager.GetAllTierParts(BasePart.PartTier.Legendary, CustomizationManager.PartFlags.Craftable);
		if (config == null)
		{
			return;
		}
		for (int i = 0; i < config.Count; i++)
		{
			string key = i.ToString();
			if (!config.HasKey(key))
			{
				continue;
			}
			string text = config[key];
			for (int j = 0; j < allTierParts.Count; j++)
			{
				if (allTierParts[j].name == text)
				{
					s_unlockOrder.Add(allTierParts[j]);
					break;
				}
			}
		}
		s_initialized = true;
	}

	public static bool GetNextUnlockable(out BasePart part)
	{
		part = null;
		if (!s_initialized)
		{
			return false;
		}
		for (int i = 0; i < s_unlockOrder.Count; i++)
		{
			if (!CustomizationManager.IsPartUnlocked(s_unlockOrder[i]))
			{
				part = s_unlockOrder[i];
				break;
			}
		}
		return part != null;
	}

	public static int UnlockablesLeft()
	{
		int num = 0;
		if (!s_initialized)
		{
			return 0;
		}
		for (int i = 0; i < s_unlockOrder.Count; i++)
		{
			if (!CustomizationManager.IsPartUnlocked(s_unlockOrder[i]))
			{
				num++;
			}
		}
		return num;
	}

	public static int GetPrice()
	{
		return 3000;
	}
}
