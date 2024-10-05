public struct PartCountChanged : EventManager.Event
{
	public BasePart.PartType partType;

	public int count;

	public PartCountChanged(BasePart.PartType partType, int count)
	{
		this.partType = partType;
		this.count = count;
	}
}
