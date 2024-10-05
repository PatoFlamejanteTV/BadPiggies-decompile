using UnityEngine;

public class KingPigFeedLevel : MonoBehaviour
{
	[SerializeField]
	private DoubleRewardButton doubleRewardButton;

	private void Awake()
	{
		if (GameTime.IsPaused())
		{
			GameTime.Pause(pause: false);
		}
	}

	private void Start()
	{
		if ((bool)doubleRewardButton)
		{
			doubleRewardButton.gameObject.SetActive(value: true);
			doubleRewardButton.Show();
		}
	}

	private void OnEnable()
	{
		KeyListener.keyReleased += HandleKeyReleased;
	}

	private void OnDisable()
	{
		KeyListener.keyReleased -= HandleKeyReleased;
	}

	public void GoBack()
	{
		KingPigFeedButton.LastDessertCount = KingPigFeedButton.CurrentDessertCount();
		if ((Singleton<GameManager>.Instance.GetPrevGameState() == GameManager.GameState.LevelSelection || Singleton<GameManager>.Instance.GetPrevGameState() == GameManager.GameState.Level) && Singleton<GameManager>.Instance.CurrentEpisode != string.Empty)
		{
			Singleton<GameManager>.Instance.LoadLevelSelection(Singleton<GameManager>.Instance.CurrentEpisode, showLoadingScreen: true);
		}
		else
		{
			Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: true);
		}
	}

	private void OnDestroy()
	{
		if (GameTime.IsPaused())
		{
			GameTime.Pause(pause: false);
		}
		SendKingPigFeedingExitFlurryEvent();
	}

	public void SendKingPigFeedingExitFlurryEvent()
	{
	}

	private void HandleKeyReleased(KeyCode obj)
	{
		bool flag = false;
		if (obj == KeyCode.Escape && !flag)
		{
			GoBack();
		}
	}

	public void ShowTutorialScreen()
	{
		Object.Instantiate(Resources.Load("UI/TutorialPage", typeof(GameObject)) as GameObject);
	}

	public void OpenShop()
	{
		base.gameObject.SetActive(value: false);
		Singleton<IapManager>.Instance.OpenShopPage(delegate
		{
			base.gameObject.SetActive(value: true);
		});
	}
}
