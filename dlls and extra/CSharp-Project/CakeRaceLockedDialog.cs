using UnityEngine;

public class CakeRaceLockedDialog : TextDialog
{
	[SerializeField]
	private TextMesh[] levelRequirementLabel;

	[SerializeField]
	private TextMesh[] descriptionLabel;

	[SerializeField]
	private string descriptionKey;

	public new void Close()
	{
		base.Close();
	}

	public void SetLevelRequirement(int levelRequirement)
	{
		Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve(descriptionKey);
		if (localeParameters.translation.Contains("{0}"))
		{
			TextMeshHelper.UpdateTextMeshes(descriptionLabel, string.Format(localeParameters.translation, levelRequirement));
		}
		else
		{
			TextMeshHelper.UpdateTextMeshes(descriptionLabel, localeParameters.translation);
		}
		TextMeshHelper.Wrap(descriptionLabel, (!TextMeshHelper.UsesKanjiCharacters()) ? 16 : 8);
		TextMeshHelper.UpdateTextMeshes(levelRequirementLabel, levelRequirement.ToString());
	}
}
