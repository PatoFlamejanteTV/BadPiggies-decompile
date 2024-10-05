public struct PlayFabEvent : EventManager.Event
{
	public enum Type
	{
		None,
		UserDataUploadStarted,
		UserDataUploadEnded,
		UserDeltaChangeUploadStarted,
		UserDeltaChangeUploadEnded,
		LocalDataUpdated
	}

	public Type type;

	public PlayFabEvent(Type type)
	{
		this.type = type;
	}
}
