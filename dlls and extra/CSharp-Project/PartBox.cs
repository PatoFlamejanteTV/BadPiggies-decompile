using UnityEngine;

public class PartBox : OneTimeCollectable
{
	public delegate void Collected();

	public new delegate void Collect(PartBox sender);

	public BasePart.PartType partType;

	public int count;

	public GameObject tutoPage;

	public AnimationClip collectAnimation;

	private Transform partSprite;

	private GameObject partSpriteParent;

	private bool showTutorial;

	public string TutorialShownKey => "IsPartTutorialShown_" + partType;

	public static event Collected onCollected;

	public event Collect onCollect;

	private void OnDataLoaded()
	{
		foreach (GameObject part in WPFMonoBehaviour.gameData.m_parts)
		{
			BasePart component = part.GetComponent<BasePart>();
			if (component.m_partType == partType)
			{
				if (partSpriteParent == null)
				{
					partSpriteParent = new GameObject($"PartVisualisation_{partType}");
				}
				GameObject gameObject = Object.Instantiate(component.m_constructionIconSprite.gameObject);
				gameObject.layer = base.gameObject.layer;
				partSprite = gameObject.transform;
				partSprite.name = "PartVisualisation";
				partSprite.parent = partSpriteParent.transform;
				partSprite.localPosition = Vector3.zero;
				partSprite.localRotation = Quaternion.identity;
				partSprite.GetComponent<Renderer>().enabled = false;
				if (collectAnimation != null)
				{
					Animation obj = partSprite.gameObject.AddComponent<Animation>();
					obj.playAutomatically = false;
					obj.AddClip(collectAnimation, collectAnimation.name);
				}
			}
		}
		if (GameProgress.HasKey($"{Singleton<GameManager>.Instance.CurrentSceneName}_star_{base.NameKey}") && !GameProgress.HasKey($"{Singleton<GameManager>.Instance.CurrentSceneName}_part_{base.NameKey}"))
		{
			GameProgress.AddPartBox(Singleton<GameManager>.Instance.CurrentSceneName, base.NameKey);
			GameProgress.AddSandboxParts(Singleton<GameManager>.Instance.CurrentSceneName, partType, count);
		}
	}

	public override bool IsDisabled()
	{
		return false;
	}

	protected override string GetNameKey()
	{
		string empty = string.Empty;
		if (base.transform.parent != null && base.transform.parent.name.Contains("FloatingPartBox"))
		{
			empty = base.transform.parent.name;
		}
		else if (base.transform.parent != null && base.transform.parent.name == "PartBoxes")
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
				Singleton<PlayerProgress>.Instance.AddExperience(PlayerProgress.ExperienceType.PartBoxCollectedSandbox);
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
		if (!isGhost)
		{
			GameProgress.AddPartBox(Singleton<GameManager>.Instance.CurrentSceneName, base.NameKey);
			GameProgress.AddSandboxParts(Singleton<GameManager>.Instance.CurrentSceneName, partType, count);
			if (partSprite != null)
			{
				partSpriteParent.transform.position = base.transform.position - Vector3.forward * 2f;
				partSprite.GetComponent<Renderer>().enabled = true;
				if (partSprite.GetComponent<Animation>() != null && collectAnimation != null)
				{
					partSprite.GetComponent<Animation>().Play(collectAnimation.name);
				}
			}
			if (tutoPage != null)
			{
				if (GameProgress.GetBool(TutorialShownKey))
				{
					showTutorial = false;
				}
				else
				{
					showTutorial = true;
				}
			}
		}
		if (PartBox.onCollected != null)
		{
			PartBox.onCollected();
		}
		if (this.onCollect != null)
		{
			this.onCollect(this);
		}
	}

	protected override void OnUIEvent(UIEvent data)
	{
		base.OnUIEvent(data);
		if (showTutorial && data.type == UIEvent.Type.Building)
		{
			WPFMonoBehaviour.levelManager.m_levelCompleteTutorialBookPagePrefab = tutoPage;
			EventManager.Send(new UIEvent(UIEvent.Type.OpenTutorial));
			GameProgress.SetBool(TutorialShownKey, value: true);
			showTutorial = false;
		}
	}
}
