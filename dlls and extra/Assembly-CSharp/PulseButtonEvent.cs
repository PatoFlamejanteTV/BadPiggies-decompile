public struct PulseButtonEvent : EventManager.Event
{
	public UIEvent.Type buttonEvent;

	public bool pulse;

	public PulseButtonEvent(UIEvent.Type buttonEvent, bool pulse = true)
	{
		this.buttonEvent = buttonEvent;
		this.pulse = pulse;
	}
}
