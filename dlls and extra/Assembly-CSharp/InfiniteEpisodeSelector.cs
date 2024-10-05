using System.Collections.Generic;
using UnityEngine;

public class InfiniteEpisodeSelector : MonoBehaviour
{
	[SerializeField]
	private GameData gameData;

	[SerializeField]
	private List<GameObject> m_episodes = new List<GameObject>();

	[SerializeField]
	private float Gap = 6f;

	[SerializeField]
	private int m_momentumSlide = 10;

	[SerializeField]
	private float ScaleFactor = 20f;

	[SerializeField]
	private float CenteringSpeed = 10f;

	[SerializeField]
	private bool isInfinite = true;

	private int m_screenWidth;

	private int m_screenHeight;

	private Camera m_hudCamera;

	private bool m_interacting;

	private Vector2 m_initialInputPos;

	private Vector2 m_lastInputPos;

	private float m_leftLimit;

	private float m_rightLimit;

	private GameObject m_scrollPivot;

	private Vector2 m_episodeSize;

	private float m_deltaX;

	private bool m_moveToCenter;

	private bool m_movableEpisodes = true;

	private GameObject m_centerEpisode;

	public List<GameObject> Episodes => m_episodes;

	private void Awake()
	{
		Singleton<GameManager>.Instance.CreateMenuBackground();
		m_hudCamera = GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>();
		m_scrollPivot = base.transform.Find("ScrollPivot").gameObject;
		for (int i = 0; i < m_episodes.Count; i++)
		{
			m_episodes[i] = Object.Instantiate(m_episodes[i]);
			m_episodes[i].transform.parent = m_scrollPivot.transform;
		}
		m_screenWidth = Screen.width;
		m_screenHeight = Screen.height;
		Layout();
	}

	private void Start()
	{
		if (GameProgress.GetBool("show_content_limit_popup"))
		{
			GameProgress.DeleteKey("show_content_limit_popup");
			LevelInfo.DisplayContentLimitNotification();
		}
	}

	private void OnEnable()
	{
		KeyListener.keyReleased += HandleKeyListenerkeyReleased;
		KeyListener.mouseWheel += HandleKeyListenerMouseWheel;
	}

	private void OnDisable()
	{
		KeyListener.keyReleased -= HandleKeyListenerkeyReleased;
		KeyListener.mouseWheel -= HandleKeyListenerMouseWheel;
	}

	private void Update()
	{
		if (m_screenWidth != Screen.width || m_screenHeight != Screen.height)
		{
			m_screenWidth = Screen.width;
			m_screenHeight = Screen.height;
			Layout();
		}
		ScaleEpisodes();
		GuiManager.Pointer pointer = GuiManager.GetPointer();
		if (pointer.down)
		{
			m_initialInputPos = pointer.position;
			m_lastInputPos = pointer.position;
			m_interacting = true;
		}
		if (pointer.dragging && m_interacting && m_movableEpisodes)
		{
			Vector3 vector = m_hudCamera.ScreenToWorldPoint(m_lastInputPos);
			Vector3 vector2 = m_hudCamera.ScreenToWorldPoint(pointer.position);
			m_lastInputPos = pointer.position;
			m_deltaX = vector2.x - vector.x;
			if (Mathf.Abs(m_deltaX) > 0f)
			{
				MoveEpisodes(m_deltaX);
			}
			Vector3 vector3 = m_hudCamera.ScreenToWorldPoint(m_initialInputPos);
			if (Mathf.Abs(vector2.x - vector3.x) > 1f)
			{
				Singleton<GuiManager>.Instance.ResetFocus();
			}
		}
		if (pointer.up && m_interacting)
		{
			m_interacting = false;
			m_moveToCenter = true;
		}
		if (m_interacting || !m_movableEpisodes || m_momentumSlide <= 0)
		{
			return;
		}
		float num = Mathf.Abs(m_centerEpisode.transform.localPosition.x);
		float num2 = Mathf.Sign(m_centerEpisode.transform.localPosition.x);
		if (Mathf.Abs(m_deltaX) > 0.1f)
		{
			MoveEpisodes(m_deltaX);
			m_deltaX -= m_deltaX / (float)m_momentumSlide;
			m_moveToCenter = true;
			if (m_deltaX < 0.15f && (double)num < 0.2)
			{
				m_deltaX = 0f;
			}
		}
		else if (m_centerEpisode != null && m_moveToCenter)
		{
			if ((double)num > 0.2)
			{
				MoveEpisodes((0f - num2) * Time.deltaTime * CenteringSpeed, checkCenter: false);
			}
			else
			{
				m_moveToCenter = false;
			}
		}
	}

	private void MoveEpisodes(float delta, bool checkCenter = true)
	{
		float num = m_rightLimit * 2f;
		float num2 = Mathf.Abs(base.transform.localPosition.x - m_centerEpisode.transform.localPosition.x);
		if ((m_centerEpisode == m_episodes[0] || m_centerEpisode == m_episodes[m_episodes.Count - 1]) && !isInfinite && num2 < 0.5f && m_deltaX > 0f)
		{
			m_deltaX = 0f;
			return;
		}
		for (int i = 0; i < m_episodes.Count; i++)
		{
			GameObject gameObject = m_episodes[i];
			if (isInfinite)
			{
				if (gameObject.transform.position.x > m_rightLimit)
				{
					gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x - num, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
				}
				else if (gameObject.transform.position.x < m_leftLimit)
				{
					gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + num, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
				}
			}
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + delta, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
			if (checkCenter)
			{
				float num3 = Mathf.Abs(gameObject.transform.localPosition.x);
				if (m_centerEpisode == null || num3 < Mathf.Abs(m_centerEpisode.transform.localPosition.x))
				{
					m_centerEpisode = gameObject;
				}
			}
		}
	}

	private void ScaleEpisodes()
	{
		for (int i = 0; i < m_episodes.Count; i++)
		{
			GameObject gameObject = m_episodes[i];
			float num = Mathf.Abs(gameObject.transform.localPosition.x) / ScaleFactor;
			if (num > 0f)
			{
				float num2 = Mathf.Clamp(1f / num, 0f, 1f);
				gameObject.GetComponent<Collider>().enabled = num2 >= 1f;
				gameObject.transform.localScale = new Vector3(num2, num2, 1f);
			}
		}
	}

	private void Layout()
	{
		float num = 2f * m_hudCamera.orthographicSize * (float)Screen.width / (float)Screen.height;
		int count = m_episodes.Count;
		m_episodeSize = m_episodes[0].GetComponent<Sprite>().Size;
		float num2 = num - 2f * m_episodeSize.x - m_episodeSize.x * 3f;
		Gap = num2 / 4f;
		for (int i = 0; i < count; i++)
		{
			m_episodes[i].transform.position = Vector3.right * (m_episodeSize.x + Gap) * i;
		}
		m_centerEpisode = m_episodes[0];
		m_leftLimit = 0f - (m_rightLimit = (float)((count - 1) / 2 + 1) * (m_episodeSize.x + Gap));
		MoveEpisodes(0f);
	}

	private bool isInInteractiveArea(Vector2 touchPos)
	{
		if (touchPos.y > (float)Screen.height * 0.1f)
		{
			return touchPos.y < (float)Screen.height * 0.8f;
		}
		return false;
	}

	public void GoToMainMenu()
	{
		SendExitEpisodeSelectionFlurryEvent();
		Singleton<GameManager>.Instance.LoadMainMenu(showLoadingScreen: false);
	}

	private void HandleKeyListenerkeyReleased(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			GoToMainMenu();
		}
	}

	private void HandleKeyListenerMouseWheel(float delta)
	{
	}

	public void SendExitEpisodeSelectionFlurryEvent()
	{
	}

	private void OnDrawGizmos()
	{
		if (m_centerEpisode != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(m_centerEpisode.transform.localPosition, 0.5f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.localPosition, 0.5f);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(new Vector3(m_leftLimit, 0f), 0.5f);
		Gizmos.DrawSphere(new Vector3(m_rightLimit, 0f), 0.5f);
	}
}
