using UnityEngine;

public class SandboxUnlockDialog : TextDialog
{
	[SerializeField]
	private LocalizeSprite[] ep1SandboxSpriteIdLocales;

	[SerializeField]
	private LocalizeSprite[] ep2SandboxSpriteIdLocales;

	[SerializeField]
	private LocalizeSprite[] ep3SandboxSpriteIdLocales;

	[SerializeField]
	private LocalizeSprite[] ep4SandboxSpriteIdLocales;

	[SerializeField]
	private LocalizeSprite[] ep6SandboxSpriteIdLocales;

	[SerializeField]
	private GameObject ep1Sprite;

	[SerializeField]
	private GameObject ep2Sprite;

	[SerializeField]
	private GameObject ep3Sprite;

	[SerializeField]
	private GameObject ep4Sprite;

	[SerializeField]
	private GameObject ep6Sprite;

	private string sandboxIdentifier;

	private int cost;

	[SerializeField]
	private GameObject costTextEnabled;

	public string SandboxIdentifier
	{
		get
		{
			return sandboxIdentifier;
		}
		set
		{
			sandboxIdentifier = value;
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
		ep1Sprite.SetActive(value: false);
		ep2Sprite.SetActive(value: false);
		ep3Sprite.SetActive(value: false);
		ep4Sprite.SetActive(value: false);
		ep6Sprite.SetActive(value: false);
		switch (sandboxIdentifier)
		{
		case "S-1":
		case "S-2":
			ep1Sprite.SetActive(value: true);
			break;
		case "S-7":
		case "S-8":
			ep2Sprite.SetActive(value: true);
			break;
		case "S-3":
		case "S-4":
			ep3Sprite.SetActive(value: true);
			break;
		case "S-5":
		case "S-6":
			ep4Sprite.SetActive(value: true);
			break;
		case "S-9":
		case "S-10":
			ep6Sprite.SetActive(value: true);
			break;
		}
	}

	public void RebuildTexts()
	{
		string text = $"[snout] {cost}";
		TextMesh[] componentsInChildren = costTextEnabled.gameObject.GetComponentsInChildren<TextMesh>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].text = text;
			TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren[i]);
		}
	}
}
