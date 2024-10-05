using UnityEngine;

public class KingPigFeedButton : Button
{
	[SerializeField]
	private GameObject newTag;

	private bool wiggling;

	public static int LastDessertCount
	{
		get
		{
			return GameProgress.GetInt("LastDessertCount");
		}
		set
		{
			GameProgress.SetInt("LastDessertCount", value);
		}
	}

	protected override void ButtonAwake()
	{
		CheckWiggle();
		EventManager.Connect<UIEvent>(OnUIEvent);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<UIEvent>(OnUIEvent);
	}

	private void CheckWiggle()
	{
		bool active = GameProgress.GetBool("ChiefPigExploded");
		if (INSettings.GetBool(INFeature.EnableKingFeedButton))
		{
			active = true;
		}
		bool @bool = GameProgress.GetBool("Kingpig_Visited");
		if (!Singleton<BuildCustomizationLoader>.Instance.IsChina)
		{
			base.gameObject.SetActive(active);
		}
		if (!@bool || (HasDesserts() && LastDessertCount != CurrentDessertCount()))
		{
			Wiggle();
		}
	}

	private bool HasDesserts()
	{
		for (int i = 0; i < WPFMonoBehaviour.gameData.m_desserts.Count; i++)
		{
			if (GameProgress.DessertCount(WPFMonoBehaviour.gameData.m_desserts[i].name) > 0)
			{
				return true;
			}
		}
		return false;
	}

	public static int CurrentDessertCount()
	{
		int num = 0;
		for (int i = 0; i < WPFMonoBehaviour.gameData.m_desserts.Count; i++)
		{
			num += GameProgress.DessertCount(WPFMonoBehaviour.gameData.m_desserts[i].name);
		}
		return num;
	}

	private void Wiggle()
	{
		if (newTag != null && !wiggling)
		{
			GameObject obj = Object.Instantiate(newTag);
			obj.transform.parent = base.transform;
			obj.transform.localPosition = new Vector3(1.1f, 1.1f, -1f);
			wiggling = true;
		}
	}

	protected override void OnActivate()
	{
		GameProgress.SetBool("Kingpig_Visited", value: true);
		Singleton<GameManager>.Instance.LoadKingPigFeed(showLoadingScreen: false);
	}

	private void OnUIEvent(UIEvent data)
	{
		if (data.type == UIEvent.Type.ClosedLootWheel)
		{
			CheckWiggle();
		}
	}
}
