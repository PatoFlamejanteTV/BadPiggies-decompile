using System;
using UnityEngine;

public class KingsFavoriteBubble : MonoBehaviour
{
	[SerializeField]
	private Transform partContainer;

	[SerializeField]
	private GameObject[] partTierBackgrounds;

	[SerializeField]
	private RealtimeSkeletonAnimation anim;

	private KingsFavoriteDialog dialog;

	private void Awake()
	{
		if (anim != null)
		{
			anim.gameObject.SetActive(value: false);
		}
	}

	private void Start()
	{
		CakeRaceKingsFavorite instance = Singleton<CakeRaceKingsFavorite>.Instance;
		instance.OnPartAcquired = (Action)Delegate.Combine(instance.OnPartAcquired, new Action(ShowCurrentFavoritePart));
		ShowCurrentFavoritePart();
	}

	private void OnDestroy()
	{
		if (Singleton<CakeRaceKingsFavorite>.IsInstantiated())
		{
			CakeRaceKingsFavorite instance = Singleton<CakeRaceKingsFavorite>.Instance;
			instance.OnPartAcquired = (Action)Delegate.Remove(instance.OnPartAcquired, new Action(ShowCurrentFavoritePart));
		}
	}

	private void ShowCurrentFavoritePart()
	{
		if (Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite == null)
		{
			return;
		}
		if (partContainer.childCount > 0)
		{
			for (int i = 0; i < partContainer.childCount; i++)
			{
				UnityEngine.Object.Destroy(partContainer.GetChild(i).gameObject);
			}
		}
		GameObject obj = UnityEngine.Object.Instantiate(Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite.m_constructionIconSprite.gameObject);
		obj.transform.parent = partContainer;
		obj.transform.localPosition = Vector3.back * 0.5f;
		GameObject obj2 = UnityEngine.Object.Instantiate(partTierBackgrounds[(int)Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite.m_partTier]);
		obj2.transform.parent = partContainer;
		obj2.transform.localScale = Vector3.one * 0.5f;
		obj2.transform.localPosition = Vector3.zero;
		if (anim != null)
		{
			anim.gameObject.SetActive(value: true);
			anim.state.SetAnimation(0, "Intro1", loop: false);
		}
	}

	public void OpenDialog()
	{
		if (dialog == null)
		{
			dialog = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_kingsFavoriteDialog).GetComponent<KingsFavoriteDialog>();
			dialog.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 5f;
		}
		dialog.Open();
	}
}
