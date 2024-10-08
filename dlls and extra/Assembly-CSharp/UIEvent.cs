public struct UIEvent : EventManager.Event
{
	public enum Type
	{
		None,
		Building,
		Play,
		LevelSelection,
		NextLevel,
		Home,
		Preview,
		Clear,
		Pause,
		Blueprint,
		ReplayLevel,
		ActivateRockets,
		ActivateEngines,
		BackFromPreview,
		ContinueFromPause,
		ReplayFlight,
		IapPurchaseCurrency,
		IapPurchaseRocket,
		IapPurchaseEngine,
		OpenIapMenu,
		CloseIapMenu,
		QuestModeBuild,
		MoveContraptionLeft,
		MoveContraptionRight,
		MoveContraptionUp,
		MoveContraptionDown,
		OpenTutorial,
		CloseTutorial,
		Snapshot,
		CloseMechanicInfo,
		CloseMechanicInfoAndUseMechanic,
		OpenUnlockFullVersionIapMenu,
		CloseUnlockFullVersionIapMenu,
		EpisodeSelection,
		ApplySuperGlue,
		ApplySuperMagnet,
		ApplyTurboCharge,
		SuperBlueprint,
		RotateSuperBluePrints,
		CloseMechanicInfoAndUseSuperMechanic,
		LoadContraptionSlot1,
		LoadContraptionSlot2,
		LoadContraptionSlot3,
		UnlockNextLevelWithVideoAd,
		ApplyNightVision,
		OpenedLootWheel,
		ClosedLootWheel,
		OpenedSnoutCoinShop,
		ClosedSnoutCoinShop,
		OpenedLootCrateDialog,
		ClosedLootCrateDialog,
		CakeRaceTimerStarted,
		OpenedMainMenuPromo,
		ClosedMainMenuPromo,
		OpenedPurchaseConfirmation,
		ClosedPurchaseConfirmation,
		OpenedNotificationPopup,
		ClosedNotificationPopup,
		ShopLockedScreen,
		ShopUnlockedScreen,
		OpenedCakeRaceUnlockedPopup,
		ClosedCakeRaceUnlockedPopup,
		OpenedDailyChallengeDialog,
		ClosedDailyChallengeDialog,
		OpenedWorkshopIntroduction,
		ClosedWorkshopIntroduction,
		OpenedPurchaseRoadHogsParts,
		ClosedPurchaseRoadHogsParts,
		SaveContraption
	}

	public Type type;

	public UIEvent(Type type)
	{
		this.type = type;
	}
}
