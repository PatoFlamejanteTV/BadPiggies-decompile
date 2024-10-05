using System;
using System.Collections.Generic;

public class ComplexVisibilityManager : Singleton<ComplexVisibilityManager>
{
	public enum Condition
	{
		Always,
		ShopOpen,
		SnoutCoinShopOpen,
		LootWheelOpen,
		LootCrateDialogOpen,
		InMainMenu,
		InEpisodeSelection,
		InLevelSelection,
		InWorkshop,
		InCakeRaceMenu,
		InKingPigFeed,
		InLevel,
		LevelCompleteScreen,
		CakeRaceCompleteScreen,
		MainMenuPromo,
		PurchaseProductConfirmation,
		NotificationPopupOpen,
		ShopLockedScreen,
		IsOdyssey,
		CakeRaceUnlockDialogOpen,
		DailyChallengeDialogOpen,
		WorkshopIntroductionOpen,
		PurchaseRoadHogsPartsOpen
	}

	private Dictionary<Condition, bool> states;

	private Dictionary<IComplexVisibilityObject, bool> listeners;

	private void Awake()
	{
		SetAsPersistant();
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
		EventManager.Connect<LevelLoadedEvent>(OnLevelLoaded);
		EventManager.Connect<LoadLevelEvent>(OnLoadlevel);
		EventManager.Connect<UIEvent>(OnReceivedUIEvent);
		states = new Dictionary<Condition, bool>();
		for (int i = 0; i < Enum.GetNames(typeof(Condition)).Length; i++)
		{
			states.Add((Condition)i, value: false);
		}
		states[Condition.Always] = true;
		states[Condition.IsOdyssey] = Singleton<BuildCustomizationLoader>.instance.IsOdyssey;
		listeners = new Dictionary<IComplexVisibilityObject, bool>();
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
		EventManager.Disconnect<LevelLoadedEvent>(OnLevelLoaded);
		EventManager.Disconnect<LoadLevelEvent>(OnLoadlevel);
		EventManager.Disconnect<UIEvent>(OnReceivedUIEvent);
	}

	private void OnReceivedUIEvent(UIEvent data)
	{
		switch (data.type)
		{
		case UIEvent.Type.OpenedLootWheel:
			SwitchCondition(Condition.LootWheelOpen, newState: true);
			break;
		case UIEvent.Type.ClosedLootWheel:
			SwitchCondition(Condition.LootWheelOpen, newState: false);
			break;
		case UIEvent.Type.OpenedSnoutCoinShop:
			SwitchCondition(Condition.SnoutCoinShopOpen, newState: true);
			break;
		case UIEvent.Type.ClosedSnoutCoinShop:
			SwitchCondition(Condition.SnoutCoinShopOpen, newState: false);
			break;
		case UIEvent.Type.OpenedLootCrateDialog:
			SwitchCondition(Condition.LootCrateDialogOpen, newState: true);
			break;
		case UIEvent.Type.ClosedLootCrateDialog:
			SwitchCondition(Condition.LootCrateDialogOpen, newState: false);
			break;
		case UIEvent.Type.CloseIapMenu:
			SwitchCondition(Condition.ShopOpen, newState: false);
			break;
		case UIEvent.Type.OpenIapMenu:
			SwitchCondition(Condition.ShopOpen, newState: true);
			break;
		case UIEvent.Type.OpenedMainMenuPromo:
			SwitchCondition(Condition.MainMenuPromo, newState: true);
			break;
		case UIEvent.Type.ClosedMainMenuPromo:
			SwitchCondition(Condition.MainMenuPromo, newState: false);
			break;
		case UIEvent.Type.OpenedPurchaseConfirmation:
			SwitchCondition(Condition.PurchaseProductConfirmation, newState: true);
			break;
		case UIEvent.Type.ClosedPurchaseConfirmation:
			SwitchCondition(Condition.PurchaseProductConfirmation, newState: false);
			break;
		case UIEvent.Type.OpenedNotificationPopup:
			SwitchCondition(Condition.NotificationPopupOpen, newState: true);
			break;
		case UIEvent.Type.ClosedNotificationPopup:
			SwitchCondition(Condition.NotificationPopupOpen, newState: false);
			break;
		case UIEvent.Type.ShopLockedScreen:
			SwitchCondition(Condition.ShopLockedScreen, newState: true);
			break;
		case UIEvent.Type.ShopUnlockedScreen:
			SwitchCondition(Condition.ShopLockedScreen, newState: false);
			break;
		case UIEvent.Type.OpenedCakeRaceUnlockedPopup:
			SwitchCondition(Condition.CakeRaceUnlockDialogOpen, newState: true);
			break;
		case UIEvent.Type.ClosedCakeRaceUnlockedPopup:
			SwitchCondition(Condition.CakeRaceUnlockDialogOpen, newState: false);
			break;
		case UIEvent.Type.OpenedDailyChallengeDialog:
			SwitchCondition(Condition.DailyChallengeDialogOpen, newState: true);
			break;
		case UIEvent.Type.ClosedDailyChallengeDialog:
			SwitchCondition(Condition.DailyChallengeDialogOpen, newState: false);
			break;
		case UIEvent.Type.OpenedWorkshopIntroduction:
			SwitchCondition(Condition.WorkshopIntroductionOpen, newState: true);
			break;
		case UIEvent.Type.ClosedWorkshopIntroduction:
			SwitchCondition(Condition.WorkshopIntroductionOpen, newState: false);
			break;
		case UIEvent.Type.OpenedPurchaseRoadHogsParts:
			SwitchCondition(Condition.PurchaseRoadHogsPartsOpen, newState: true);
			break;
		case UIEvent.Type.ClosedPurchaseRoadHogsParts:
			SwitchCondition(Condition.PurchaseRoadHogsPartsOpen, newState: false);
			break;
		}
	}

	private void OnLoadlevel(LoadLevelEvent data)
	{
		ChangeGeneralGameState(data.currentGameState, newState: false);
	}

	private void OnLevelLoaded(LevelLoadedEvent data)
	{
		ChangeGeneralGameState(data.currentGameState, newState: true);
	}

	private void ChangeGeneralGameState(GameManager.GameState state, bool newState)
	{
		switch (state)
		{
		case GameManager.GameState.MainMenu:
			SwitchCondition(Condition.InMainMenu, newState);
			break;
		case GameManager.GameState.EpisodeSelection:
			SwitchCondition(Condition.InEpisodeSelection, newState);
			break;
		case GameManager.GameState.LevelSelection:
			SwitchCondition(Condition.InLevelSelection, newState);
			break;
		case GameManager.GameState.Level:
			SwitchCondition(Condition.InLevel, newState);
			break;
		case GameManager.GameState.KingPigFeeding:
			SwitchCondition(Condition.InKingPigFeed, newState);
			break;
		case GameManager.GameState.WorkShop:
			SwitchCondition(Condition.InWorkshop, newState);
			break;
		case GameManager.GameState.CakeRaceMenu:
			SwitchCondition(Condition.InCakeRaceMenu, newState);
			break;
		case GameManager.GameState.Cutscene:
		case GameManager.GameState.CheatsPanel:
		case GameManager.GameState.StarLevelCutscene:
		case GameManager.GameState.SandboxLevelSelection:
		case GameManager.GameState.RaceLevelSelection:
			break;
		}
	}

	private void OnGameStateChanged(GameStateChanged data)
	{
		ChangeLevelGameState(data.prevState, newState: false);
		ChangeLevelGameState(data.state, newState: true);
	}

	private void ChangeLevelGameState(LevelManager.GameState state, bool newState)
	{
		switch (state)
		{
		case LevelManager.GameState.CakeRaceCompleted:
			SwitchCondition(Condition.CakeRaceCompleteScreen, newState);
			break;
		case LevelManager.GameState.Completed:
			SwitchCondition(Condition.LevelCompleteScreen, newState);
			break;
		}
	}

	private void SwitchCondition(Condition condition, bool newState)
	{
		if (states[condition] != newState)
		{
			states[condition] = newState;
			Evaluate();
		}
	}

	public void Subscribe(IComplexVisibilityObject listener, bool currentState)
	{
		if (!listeners.ContainsKey(listener))
		{
			listeners.Add(listener, currentState);
		}
		Evaluate();
	}

	public void Unsubscribe(IComplexVisibilityObject listener)
	{
		if (listeners.ContainsKey(listener))
		{
			listeners.Remove(listener);
		}
	}

	public void StateChanged(IComplexVisibilityObject listener, bool newState)
	{
		if (listeners.ContainsKey(listener))
		{
			listeners[listener] = newState;
		}
	}

	private void Evaluate()
	{
		Action action = null;
		foreach (KeyValuePair<IComplexVisibilityObject, bool> listener in listeners)
		{
			bool num = Evaluate(listener.Key.ShowConditions);
			bool flag = Evaluate(listener.Key.HideConditions);
			bool flag2 = num && !flag;
			if (listener.Value != flag2)
			{
				IComplexVisibilityObject target = listener.Key;
				bool newState = flag2;
				action = (Action)Delegate.Combine(action, (Action)delegate
				{
					listeners[target] = newState;
					target.OnStateChanged(newState);
				});
			}
		}
		action?.Invoke();
	}

	private bool Evaluate(List<Condition> conditions)
	{
		for (int i = 0; i < conditions.Count; i++)
		{
			if (states[conditions[i]])
			{
				return true;
			}
		}
		return false;
	}
}
