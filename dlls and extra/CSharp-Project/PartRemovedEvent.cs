using UnityEngine;

public struct PartRemovedEvent : EventManager.Event
{
	public BasePart.PartType partType;

	public Vector3 position;

	public PartRemovedEvent(BasePart.PartType partType, Vector3 position)
	{
		this.partType = partType;
		this.position = position;
	}
}
