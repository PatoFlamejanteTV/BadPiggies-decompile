public struct PressureButtonReleased : EventManager.Event
{
	public int id;

	public PressureButtonReleased(int _id)
	{
		id = _id;
	}
}
