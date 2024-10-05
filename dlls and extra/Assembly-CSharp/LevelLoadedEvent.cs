public struct LevelLoadedEvent : EventManager.Event
{
	public GameManager.GameState currentGameState;

	public LevelLoadedEvent(GameManager.GameState currentGameState)
	{
		this.currentGameState = currentGameState;
	}
}
