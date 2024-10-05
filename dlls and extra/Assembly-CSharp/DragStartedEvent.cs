public struct DragStartedEvent : EventManager.Event
{
	public BasePart.PartType partType;

	public DragStartedEvent(BasePart.PartType partType)
	{
		this.partType = partType;
	}
}
