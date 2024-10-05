public class GetMoreScrapDialog : TextDialog
{
	private int buyScrapAmount;

	private BasePart.PartTier partTier;

	protected override void Awake()
	{
		base.Awake();
		base.onOpen += RefreshLocalization;
	}

	protected override void Start()
	{
	}

	private void OnDestroy()
	{
		base.onOpen -= RefreshLocalization;
	}

	public void SetScrapAmount(int scrapAmount, BasePart.PartTier tier)
	{
		buyScrapAmount = scrapAmount;
		partTier = tier;
	}

	private void RefreshLocalization()
	{
		for (int i = 0; i < texts.Length; i++)
		{
			texts[i].textMesh.text = texts[i].localizationKey;
			TextMeshLocale component = texts[i].textMesh.gameObject.GetComponent<TextMeshLocale>();
			if (!(component != null))
			{
				continue;
			}
			component.RefreshTranslation();
			string text = texts[i].textMesh.text;
			if (texts[i].textMesh.name.Equals("ScrapLabel") && text.Contains("{0}") && text.Contains("{1}"))
			{
				string arg = string.Empty;
				switch (partTier)
				{
				case BasePart.PartTier.Common:
					arg = "[common_star]";
					break;
				case BasePart.PartTier.Rare:
					arg = "[rare_star][rare_star]";
					break;
				case BasePart.PartTier.Epic:
					arg = "[epic_star][epic_star][epic_star]";
					break;
				case BasePart.PartTier.Legendary:
					arg = "[legendary_icon]";
					break;
				}
				texts[i].textMesh.text = string.Format(text, buyScrapAmount, arg);
			}
			component.enabled = false;
			TextMeshSpriteIcons.EnsureSpriteIcon(texts[i].textMesh);
			TextMeshHelper.Wrap(texts[i].textMesh, (!TextMeshHelper.UsesKanjiCharacters()) ? maxCharactersInLine : maxKanjiCharacterInLine);
		}
	}
}
