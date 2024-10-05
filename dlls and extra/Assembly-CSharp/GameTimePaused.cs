public struct GameTimePaused : EventManager.Event
{
	public bool paused;

	public GameTimePaused(bool paused)
	{
		this.paused = paused;
	}
}
