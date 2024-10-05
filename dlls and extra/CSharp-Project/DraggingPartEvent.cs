using UnityEngine;

public struct DraggingPartEvent : EventManager.Event
{
	public BasePart.PartType partType;

	public Vector3 position;

	public DraggingPartEvent(BasePart.PartType partType, Vector3 position)
	{
		this.partType = partType;
		this.position = position;
	}
}
