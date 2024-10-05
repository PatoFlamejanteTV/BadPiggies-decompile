using UnityEngine;

public class FacebookLoginButton : MonoBehaviour
{
	[SerializeField]
	private GameObject loginButton;

	[SerializeField]
	private GameObject bubble;

	[SerializeField]
	private GameObject logoutButton;

	[SerializeField]
	private GameObject hideOnOpen;

	private ConnectToFacebookDialog connectDialog;

	private TextDialog disconnectDialog;

	private void Awake()
	{
		SetButtons(showLogin: true);
	}

	private void SetButtons(bool showLogin)
	{
		loginButton.SetActive(showLogin);
		bubble.SetActive(showLogin);
		logoutButton.SetActive(!showLogin);
	}

	public void ButtonPressed()
	{
	}
}
