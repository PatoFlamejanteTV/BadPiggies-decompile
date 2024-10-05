public struct PressureButtonPressed : EventManager.Event
{
	public int id;

	public PressureButtonPressed(int _id)
	{
		id = _id;
	}
}
