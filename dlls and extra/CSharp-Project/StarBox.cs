using System.Collections.Generic;
using UnityEngine;

public class StarBox : OneTimeCollectable
{
	public delegate void Collected();

	private static List<StarBox> starBoxes = new List<StarBox>();

	public static List<StarBox> StarBoxes => starBoxes;

	public static event Collected onCollected;

	public event Collected onCollect;

	protected virtual void Awake()
	{
		starBoxes.Add(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		starBoxes.Remove(this);
	}

	public override bool IsDisabled()
	{
		return false;
	}

	protected override string GetNameKey()
	{
		string empty = string.Empty;
		if (base.transform.parent != null && base.transform.parent.name.Contains("FloatingStarBox"))
		{
			empty = base.transform.parent.name;
		}
		else if ((bool)base.transform.parent && base.transform.parent.name == "StarBoxes")
		{
			empty = base.name;
		}
		else
		{
			DisableGoal();
		}
		return empty;
	}

	public override void OnCollected()
	{
		int sandboxStarCollectCount = GameProgress.GetSandboxStarCollectCount(Singleton<GameManager>.Instance.CurrentSceneName, base.NameKey);
		if (sandboxStarCollectCount <= 1)
		{
			int value = Singleton<GameConfigurationManager>.Instance.GetValue<int>("star_box_snout_value", "amount");
			if (value > 0 && !Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
			{
				GameProgress.AddSandboxStar(Singleton<GameManager>.Instance.CurrentSceneName, base.NameKey, snoutCoinsCollected: true);
				value = ((!Singleton<DoubleRewardManager>.Instance.HasDoubleReward) ? value : (value * 2));
				GameProgress.AddSnoutCoins(value);
				Singleton<PlayerProgress>.Instance.AddExperience(PlayerProgress.ExperienceType.StarBoxCollectedSandbox);
				ShowXPParticles();
				for (int i = 0; i < value; i++)
				{
					SnoutCoinSingle.Spawn(base.transform.position - Vector3.forward, 1f * (float)i);
				}
			}
			else if (sandboxStarCollectCount < 1)
			{
				GameProgress.AddSandboxStar(Singleton<GameManager>.Instance.CurrentSceneName, base.NameKey);
			}
		}
		if (StarBox.onCollected != null)
		{
			StarBox.onCollected();
		}
		if (this.onCollect != null)
		{
			this.onCollect();
		}
	}
}
