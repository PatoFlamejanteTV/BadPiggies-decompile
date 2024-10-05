using UnityEngine;

public class SpecialSandboxUnlockDialog : TextDialog
{
	public enum UnlockType
	{
		Statue,
		Skull
	}

	[SerializeField]
	private LocalizeSprite[] statueEpisodeIcon;

	[SerializeField]
	private LocalizeSprite[] skullEpisodeIcon;

	private UnlockType unlockType;

	private int collected;

	private int required;

	private int cost;

	[SerializeField]
	private Sprite sandboxLogo;

	[SerializeField]
	private TextMesh costTextEnabled;

	[SerializeField]
	private TextMesh costTextDisabled;

	[SerializeField]
	private TextMesh[] collectedTexts;

	public UnlockType Type
	{
		get
		{
			return unlockType;
		}
		set
		{
			unlockType = value;
		}
	}

	public int Collected
	{
		get
		{
			return collected;
		}
		set
		{
			collected = value;
		}
	}

	public int Required
	{
		get
		{
			return required;
		}
		set
		{
			required = value;
		}
	}

	public int Cost
	{
		get
		{
			return cost;
		}
		set
		{
			cost = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		if (!isOpened)
		{
			RebuildIcons();
			RebuildTexts();
		}
	}

	public new void Open()
	{
		RebuildIcons();
		RebuildTexts();
		base.Open();
	}

	public new void Close()
	{
		base.Close();
	}

	public new void Confirm()
	{
		base.Confirm();
	}

	public new void OpenShop()
	{
		base.OpenShop();
	}

	public void RebuildIcons()
	{
		if (Singleton<RuntimeSpriteDatabase>.Instance != null)
		{
			RuntimeSpriteDatabase instance = Singleton<RuntimeSpriteDatabase>.Instance;
			switch (unlockType)
			{
			case UnlockType.Statue:
				sandboxLogo.SelectSprite(instance.Find(LocalizeSprite.GetLocalizedSprite(statueEpisodeIcon, Singleton<Localizer>.Instance.CurrentLocale)), forceResetMesh: true);
				break;
			case UnlockType.Skull:
				sandboxLogo.SelectSprite(instance.Find(LocalizeSprite.GetLocalizedSprite(skullEpisodeIcon, Singleton<Localizer>.Instance.CurrentLocale)), forceResetMesh: true);
				break;
			}
		}
	}

	public void RebuildTexts()
	{
		string arg = string.Empty;
		string text = $"[snout] {cost}";
		switch (unlockType)
		{
		case UnlockType.Statue:
			arg = "[statue]";
			break;
		case UnlockType.Skull:
			arg = "[skull]";
			break;
		}
		string text2 = $"{arg} {collected}/{required}";
		for (int i = 0; i < collectedTexts.Length; i++)
		{
			collectedTexts[i].text = text2;
			TextMeshSpriteIcons.EnsureSpriteIcon(collectedTexts[i]);
		}
		costTextDisabled.text = text;
		TextMeshSpriteIcons.EnsureSpriteIcon(costTextDisabled);
		costTextEnabled.text = text;
		TextMeshSpriteIcons.EnsureSpriteIcon(costTextEnabled);
	}
}
