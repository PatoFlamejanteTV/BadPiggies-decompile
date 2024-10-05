public class PlayerChangedEvent : EventManager.Event
{
	public string playerName;

	public PlayerChangedEvent(string playerName)
	{
		this.playerName = playerName;
	}
}
