public class ExternalMessageManager : Singleton<ExternalMessageManager>
{
	public delegate void ExternalAppMessageReceived(string message);

	public static event ExternalAppMessageReceived onExternalAppMessageReceived;

	public void OnMessageReceived(string message)
	{
		if (ExternalMessageManager.onExternalAppMessageReceived != null)
		{
			ExternalMessageManager.onExternalAppMessageReceived(message);
		}
	}

	private void Awake()
	{
		SetAsPersistant();
	}
}
