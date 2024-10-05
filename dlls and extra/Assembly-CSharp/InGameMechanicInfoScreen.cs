using UnityEngine;

public class InGameMechanicInfoScreen : MonoBehaviour
{
	public GameObject m_SuperAutoBuildButton;

	public GameObject m_AutoBuildButton;

	private void OnEnable()
	{
		if (WPFMonoBehaviour.levelManager != null)
		{
			m_SuperAutoBuildButton.GetComponent<Button>().Lock(!WPFMonoBehaviour.levelManager.SuperBluePrintsAllowed);
			m_SuperAutoBuildButton.transform.Find("Disabled").gameObject.SetActive(!WPFMonoBehaviour.levelManager.SuperBluePrintsAllowed);
			m_SuperAutoBuildButton.transform.Find("Disabled").GetComponent<Renderer>().enabled = true;
			int num = GameProgress.BluePrintCount();
			if (WPFMonoBehaviour.levelManager.SuperBluePrintsAllowed && num > 0)
			{
				GameObject gameObject = m_SuperAutoBuildButton.transform.Find("AmountText").gameObject;
				GameObject gameObject2 = m_SuperAutoBuildButton.transform.Find("AmountTextShadow").gameObject;
				if ((bool)gameObject && (bool)gameObject2)
				{
					gameObject.GetComponent<TextMesh>().text = num.ToString();
					gameObject2.GetComponent<TextMesh>().text = num.ToString();
				}
			}
		}
		Singleton<KeyListener>.Instance.GrabFocus(this);
		KeyListener.keyReleased += HandleKeyListenerkeyReleased;
	}

	private void Start()
	{
		if (WPFMonoBehaviour.levelManager != null && !WPFMonoBehaviour.levelManager.SuperBluePrintsAllowed)
		{
			m_SuperAutoBuildButton.GetComponent<ButtonPulse>().StopPulse();
			m_AutoBuildButton.GetComponent<ButtonPulse>().Pulse();
		}
	}

	private void OnDisable()
	{
		KeyListener.keyReleased -= HandleKeyListenerkeyReleased;
		Singleton<KeyListener>.Instance.ReleaseFocus(this);
	}

	private void HandleKeyListenerkeyReleased(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			EventManager.Send(new UIEvent(UIEvent.Type.CloseMechanicInfo));
		}
	}

	public void CloseAndUsePremanentMechanic(UIEvent.Type eventType)
	{
		base.gameObject.SetActive(value: false);
		EventManager.Send(new UIEvent(eventType));
	}

	public void CloseAndUseSuperMechanic(UIEvent.Type eventType)
	{
		base.gameObject.SetActive(value: false);
		EventManager.Send(new UIEvent(eventType));
	}
}
