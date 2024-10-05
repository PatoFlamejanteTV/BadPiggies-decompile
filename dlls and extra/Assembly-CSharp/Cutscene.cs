using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
	public enum Type
	{
		EpisodeStart,
		EpisodeEnd
	}

	private class ScrollData
	{
		public float time;

		public Vector3 delta;

		public ScrollData(float time, Vector3 delta)
		{
			this.time = time;
			this.delta = delta;
		}
	}

	[SerializeField]
	private Type m_cutsceneType;

	public GameObject m_continueButton;

	public GameObject m_comic;

	public float m_continueButtonDelay;

	private Vector3 m_position;

	private bool m_scrolling;

	private Vector3 m_dragStartPosition;

	private Queue<ScrollData> m_scrollHistory = new Queue<ScrollData>();

	private Vector3 m_scrollVelocity;

	private float m_continueButtonHeight;

	private const float WheelScrollSpeedAdjustment = 2f;

	public void Awake()
	{
		m_scrolling = true;
		float y = m_comic.GetComponent<UnmanagedSprite>().Size.y;
		m_position = new Vector3(0f, -10f - 0.5f * y + 3f);
		m_continueButtonHeight = m_continueButton.GetComponent<Sprite>().Size.y;
	}

	private void Update()
	{
		float num = (float)Screen.width / (float)Screen.height / 1.3333334f;
		m_comic.transform.localScale = new Vector3(num, num, num);
		float y = m_comic.GetComponent<UnmanagedSprite>().Size.y;
		float num2 = 0.5f * y - 10f + 1.3f * num * m_continueButtonHeight;
		float a = 10f - 0.5f * y;
		Camera component = GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>();
		GuiManager.Pointer pointer = GuiManager.GetPointer();
		if (pointer.up)
		{
			m_scrolling = true;
		}
		if (pointer.down)
		{
			m_scrolling = false;
			m_dragStartPosition = component.ScreenToWorldPoint(pointer.position);
		}
		else if (pointer.dragging)
		{
			Vector3 vector = component.ScreenToWorldPoint(pointer.position);
			Vector3 newDelta = vector - m_dragStartPosition;
			CalculateScrollVelocity(newDelta);
			m_dragStartPosition = vector;
			float min = Mathf.Min(a, m_position.y);
			m_position.y += newDelta.y;
			m_position.y = Mathf.Clamp(m_position.y, min, num2);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
			float min2 = Mathf.Min(a, m_position.y);
			m_position.y += -10f * Input.GetAxis("Mouse ScrollWheel") * 2f;
			m_position.y = Mathf.Clamp(m_position.y, min2, num2);
		}
		else if (m_scrollVelocity.magnitude > 0.01f)
		{
			Vector3 vector2 = Time.deltaTime * m_scrollVelocity;
			m_scrollVelocity *= Mathf.Pow(0.925f, Time.deltaTime / (1f / 60f));
			float min3 = Mathf.Min(a, m_position.y);
			m_position.y += vector2.y;
			m_position.y = Mathf.Clamp(m_position.y, min3, num2);
		}
		if (m_scrolling)
		{
			m_position.y += 2f * Time.deltaTime;
			m_position.y = Mathf.Min(m_position.y, num2);
		}
		m_comic.transform.position = m_position;
	}

	public void Continue()
	{
		switch (m_cutsceneType)
		{
		case Type.EpisodeEnd:
			Singleton<GameManager>.Instance.LoadEpisodeSelection(showLoadingScreen: false);
			break;
		case Type.EpisodeStart:
			if (GameProgress.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_played") == 1)
			{
				Singleton<GameManager>.Instance.LoadLevelSelection(Singleton<GameManager>.Instance.CurrentEpisode, showLoadingScreen: false);
			}
			else
			{
				Singleton<GameManager>.Instance.LoadLevel(0);
			}
			break;
		}
		GameProgress.SetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_played", 1);
	}

	private void CalculateScrollVelocity(Vector3 newDelta)
	{
		while (m_scrollHistory.Count > 0 && m_scrollHistory.Peek().time < Time.time - 0.1f)
		{
			m_scrollHistory.Dequeue();
		}
		m_scrollHistory.Enqueue(new ScrollData(Time.time, newDelta));
		Vector3 zero = Vector3.zero;
		float time = m_scrollHistory.Peek().time;
		float num = 0f;
		foreach (ScrollData item in m_scrollHistory)
		{
			zero += item.delta;
			num = item.time - time;
		}
		if (num > 0f)
		{
			zero /= num;
		}
		m_scrollVelocity = zero;
	}
}
