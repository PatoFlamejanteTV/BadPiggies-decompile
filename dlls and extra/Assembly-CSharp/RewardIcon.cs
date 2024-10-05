using UnityEngine;

public class RewardIcon : MonoBehaviour
{
	public enum State
	{
		NotAvailable,
		ClaimNow,
		Claimed
	}

	[SerializeField]
	public GameObject disabledSprite;

	[SerializeField]
	public GameObject claimNowSprite;

	private State m_buttonState;

	public State ButtonState => m_buttonState;

	public void Awake()
	{
		RefreshButtonImage();
	}

	public RewardIcon SetButtonState(State state)
	{
		m_buttonState = state;
		RefreshButtonImage();
		return this;
	}

	private void RefreshButtonImage()
	{
		switch (m_buttonState)
		{
		case State.Claimed:
			disabledSprite.SetActive(value: true);
			claimNowSprite.SetActive(value: false);
			break;
		case State.ClaimNow:
			disabledSprite.SetActive(value: false);
			claimNowSprite.SetActive(value: true);
			break;
		case State.NotAvailable:
			disabledSprite.SetActive(value: true);
			claimNowSprite.SetActive(value: false);
			break;
		}
	}
}
