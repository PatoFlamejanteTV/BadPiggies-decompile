using UnityEngine;

public class KingsFavoriteDialog : TextDialog
{
	[SerializeField]
	private Transform partRoot;

	[SerializeField]
	private GameObject[] partTierBackgrounds;

	[SerializeField]
	private GameObject descriptionText;

	private GameObject favoritePart;

	protected override void Awake()
	{
		base.Awake();
		TextMesh[] componentsInChildren = descriptionText.GetComponentsInChildren<TextMesh>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			TextMeshLocale component = componentsInChildren[i].GetComponent<TextMeshLocale>();
			if (!(component == null))
			{
				component.RefreshTranslation(componentsInChildren[i].text);
				component.enabled = false;
				float value = Singleton<GameConfigurationManager>.Instance.GetValue<float>("cake_race", "kings_favorite_bonus");
				int num = 0;
				if (!Mathf.Approximately(value, 0f))
				{
					num = Mathf.RoundToInt((value - 1f) * 100f);
				}
				componentsInChildren[i].text = string.Format(componentsInChildren[i].text, num.ToString());
				if (TextMeshHelper.UsesKanjiCharacters())
				{
					TextMeshHelper.Wrap(componentsInChildren[i], maxKanjiCharacterInLine);
				}
				else
				{
					TextMeshHelper.Wrap(componentsInChildren[i], maxCharactersInLine);
				}
			}
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		UpdatePart();
	}

	public new void Open()
	{
		base.Open();
	}

	public new void Close()
	{
		base.Close();
	}

	private void UpdatePart()
	{
		if (favoritePart != null)
		{
			Object.Destroy(favoritePart);
		}
		if (Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite == null)
		{
			Close();
		}
		BasePart currentFavorite = Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite;
		favoritePart = Object.Instantiate(partTierBackgrounds[(int)currentFavorite.m_partTier]);
		favoritePart.transform.parent = partRoot;
		favoritePart.transform.localScale = Vector3.one * 0.7f;
		favoritePart.transform.localPosition = Vector3.zero;
		GameObject obj = Object.Instantiate(currentFavorite.m_constructionIconSprite.gameObject);
		obj.transform.localScale = Vector3.one;
		obj.transform.parent = favoritePart.transform;
		obj.transform.localPosition = Vector3.back * 0.1f;
		obj.transform.localRotation = Quaternion.identity;
		LayerHelper.SetLayer(favoritePart, base.gameObject.layer, children: true);
		LayerHelper.SetSortingLayer(favoritePart, "Popup", children: true);
	}
}
