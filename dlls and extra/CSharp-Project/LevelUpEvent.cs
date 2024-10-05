public class LevelUpEvent : EventManager.Event
{
	public int newLevel;

	public LevelUpEvent(int newLevel)
	{
		this.newLevel = newLevel;
	}
}
