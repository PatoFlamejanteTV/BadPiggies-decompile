public struct BirdWakeUpEvent : EventManager.Event
{
	public Bird bird;

	public BirdWakeUpEvent(Bird bird)
	{
		this.bird = bird;
	}
}
