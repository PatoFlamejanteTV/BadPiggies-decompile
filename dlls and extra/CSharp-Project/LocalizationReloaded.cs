public struct LocalizationReloaded : EventManager.Event
{
	public string currentLanguage;

	public LocalizationReloaded(string currentLanguage)
	{
		this.currentLanguage = currentLanguage;
	}
}
