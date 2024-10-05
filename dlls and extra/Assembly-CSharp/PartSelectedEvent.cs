public class PartSelectedEvent : EventManager.Event
{
	public BasePart.PartType type;

	public PartSelectedEvent(BasePart.PartType type)
	{
		this.type = type;
	}
}
