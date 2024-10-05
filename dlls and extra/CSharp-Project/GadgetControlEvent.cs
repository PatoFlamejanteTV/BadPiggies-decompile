public struct GadgetControlEvent : EventManager.Event
{
	public BasePart.PartType partType;

	public BasePart.Direction direction;

	public GadgetControlEvent(BasePart.PartType partType, BasePart.Direction direction)
	{
		this.partType = partType;
		this.direction = direction;
	}
}
