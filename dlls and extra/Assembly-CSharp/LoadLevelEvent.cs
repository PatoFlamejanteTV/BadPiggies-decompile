public struct LoadLevelEvent : EventManager.Event
{
	public GameManager.GameState currentGameState;

	public GameManager.GameState nextGameState;

	public string levelName;

	public LoadLevelEvent(GameManager.GameState currentGameState, GameManager.GameState nextGameState, string levelName)
	{
		this.currentGameState = currentGameState;
		this.nextGameState = nextGameState;
		this.levelName = levelName;
	}
}
