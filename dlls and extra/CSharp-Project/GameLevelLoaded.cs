public class GameLevelLoaded : EventManager.Event
{
	public int levelIndex;

	public int episodeIndex;

	public GameLevelLoaded(int levelIndex, int episodeIndex)
	{
		this.levelIndex = levelIndex;
		this.episodeIndex = episodeIndex;
	}
}
