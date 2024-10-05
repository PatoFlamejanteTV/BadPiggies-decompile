using System.Collections;
using UnityEngine;

public class PressureButton : WPFMonoBehaviour
{
	[SerializeField]
	private int m_pressureID;

	[SerializeField]
	private float m_timer;

	[SerializeField]
	private bool m_oneTimer;

	[SerializeField]
	private GameObject m_button;

	private bool m_isPressed;

	private LevelManager.GameState lastTrackedState;

	private void OnDataLoaded()
	{
		if (!m_button)
		{
			m_button = base.transform.Find("Button").gameObject;
		}
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
		StopAllCoroutines();
	}

	public void OnTriggerEnter(Collider other)
	{
		if ((!m_oneTimer || !m_isPressed) && (other.gameObject.layer == LayerMask.NameToLayer("Contraption") || other.gameObject.tag == "Dynamic"))
		{
			StopAllCoroutines();
			Pressed();
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if ((!m_oneTimer || !m_isPressed) && (other.gameObject.layer == LayerMask.NameToLayer("Contraption") || other.gameObject.tag == "Dynamic") && !m_isPressed)
		{
			Pressed();
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if ((!m_oneTimer && other.gameObject.layer == LayerMask.NameToLayer("Contraption")) || other.gameObject.tag == "Dynamic")
		{
			StartCoroutine(ReleaseDelayed(0.1f));
		}
	}

	private IEnumerator ReleaseDelayed(float delay)
	{
		m_isPressed = false;
		if (m_timer > 0f)
		{
			delay = m_timer;
		}
		yield return new WaitForSeconds(delay);
		if (!m_isPressed)
		{
			Released();
		}
	}

	private void Released()
	{
		if (!m_isPressed)
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.pressureButtonRelease, base.transform);
		}
		if ((bool)m_button)
		{
			m_button.SetActive(value: true);
		}
		EventManager.Send(new PressureButtonReleased(m_pressureID));
		m_isPressed = false;
	}

	private void Pressed()
	{
		if (!m_isPressed)
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.pressureButtonClick, base.transform);
		}
		m_isPressed = true;
		if ((bool)m_button)
		{
			m_button.SetActive(value: false);
		}
		EventManager.Send(new PressureButtonPressed(m_pressureID));
	}

	private void OnGameStateChanged(GameStateChanged newState)
	{
		if ((newState.state == LevelManager.GameState.Running && lastTrackedState == LevelManager.GameState.Building) || ((newState.state == LevelManager.GameState.Building || newState.state == LevelManager.GameState.ShowingUnlockedParts) && (lastTrackedState == LevelManager.GameState.Running || lastTrackedState == LevelManager.GameState.PausedWhileRunning)))
		{
			Released();
		}
		lastTrackedState = newState.state;
	}
}
