using System.Reflection;

public class InactiveButton : WPFMonoBehaviour
{
	private Button cachedButton;

	private void Awake()
	{
		cachedButton = GetComponent<Button>();
		if (cachedButton != null)
		{
			SetAnimate(cachedButton, state: false);
			cachedButton.SetInputDelegate(InputHandler);
		}
	}

	private void OnDestroy()
	{
		if (cachedButton != null)
		{
			SetAnimate(cachedButton, state: true);
			cachedButton.RemoveInputDelegate(InputHandler);
		}
	}

	private void InputHandler(InputEvent input)
	{
		if (input.type == InputEvent.EventType.Press)
		{
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.inactiveButton);
		}
	}

	public static void SetAnimate(Button target, bool state)
	{
		typeof(Button).GetField("animate", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, state);
	}
}
