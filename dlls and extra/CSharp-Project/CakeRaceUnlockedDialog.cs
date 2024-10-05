using UnityEngine;

public class CakeRaceUnlockedDialog : TextDialog
{
	private bool m_try;

	protected override void Awake()
	{
		base.Awake();
		base.onClose += HandleClosed;
		m_try = false;
		ResourceBar.Instance.ShowItem(ResourceBar.Item.PlayerProgress, showItem: true, enableItem: false);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		EventManager.Send(new UIEvent(UIEvent.Type.OpenedCakeRaceUnlockedPopup));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		EventManager.Send(new UIEvent(UIEvent.Type.ClosedCakeRaceUnlockedPopup));
	}

	private void OnDestroy()
	{
		base.onClose -= HandleClosed;
	}

	public void TryNow()
	{
		m_try = true;
		Close();
	}

	public new void Close()
	{
		if (m_try)
		{
			ForceCakeRace();
		}
		else
		{
			UnlockCakeRace();
		}
		base.Close();
	}

	private void ForceCakeRace()
	{
		MainMenu mainMenu = Object.FindObjectOfType<MainMenu>();
		if (Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.MainMenu && mainMenu != null)
		{
			mainMenu.ForceCakeRaceButton();
		}
		else
		{
			Singleton<GameManager>.Instance.LoadMainMenu(showLoadingScreen: true);
		}
	}

	private void UnlockCakeRace()
	{
		MainMenu mainMenu = Object.FindObjectOfType<MainMenu>();
		if (Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.MainMenu && mainMenu != null)
		{
			mainMenu.UnlockCakeRaceButton();
		}
	}

	private void HandleClosed()
	{
		Object.Destroy(base.gameObject);
	}
}
