using UnityEngine;

public class OpenIapDialog : MonoBehaviour
{
	public InGameInAppPurchaseMenu m_iapMenu;

	private void Start()
	{
		m_iapMenu.OpenDialog();
	}
}
