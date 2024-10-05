using System.Collections.Generic;
using UnityEngine;

public class PageSelector : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> m_pageElements = new List<GameObject>();

	[SerializeField]
	private float MinGap = 6f;

	[SerializeField]
	private float MaxGap = 6f;

	[SerializeField]
	private float EdgeMargin = 0.65f;

	[SerializeField]
	private GameObject m_pageDot;

	[SerializeField]
	private GameObject m_leftScroll;

	[SerializeField]
	private GameObject m_rightScroll;

	[SerializeField]
	private float m_scrollButtonMargin = 0.5f;

	[SerializeField]
	private float m_elementWidth = 1f;

	[SerializeField]
	private bool m_instantiateMenuBackground = true;

	[SerializeField]
	private int m_maxElementsPerPage;

	private int m_screenWidth;

	private int m_screenHeight;

	private Camera m_hudCamera;

	private int m_elementsPerPage;

	private int m_pageCount;

	private int m_page;

	private List<PageDot> m_dotsList = new List<PageDot>();

	private bool m_interacting;

	private Vector2 m_initialInputPos;

	private Vector2 m_lastInputPos;

	private float m_leftDragLimit;

	private float m_rightDragLimit;

	private GameObject m_scrollPivot;

	public bool LockScrolling { get; set; }

	public int CurrentPage => Mathf.Clamp(Mathf.RoundToInt(m_scrollPivot.transform.localPosition.x / (0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x)), 0, m_pageCount - 1);

	public List<GameObject> Elements => m_pageElements;

	private void Awake()
	{
		if (m_instantiateMenuBackground)
		{
			Singleton<GameManager>.Instance.CreateMenuBackground();
		}
		m_hudCamera = GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>();
		m_scrollPivot = base.transform.Find("ScrollPivot").gameObject;
		m_screenWidth = Screen.width;
		m_screenHeight = Screen.height;
		Layout();
		if (m_pageCount > 1)
		{
			CreatePageDots();
		}
		SetPage(UserSettings.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_active_page"));
	}

	private void SetPage(int page)
	{
		m_page = Mathf.Clamp(page, 0, m_pageCount - 1);
		Vector3 position = new Vector3(0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2 + Screen.width * m_page, 0f, 0f)).x, m_scrollPivot.transform.localPosition.y, m_scrollPivot.transform.localPosition.z);
		m_scrollPivot.transform.position = position;
		for (int i = 0; i < m_dotsList.Count; i++)
		{
			if (i == m_page)
			{
				m_dotsList[i].Enable();
			}
			else
			{
				m_dotsList[i].Disable();
			}
		}
	}

	public void NextPage()
	{
		m_page = Mathf.Clamp(m_page + 1, 0, m_pageCount - 1);
		for (int i = 0; i < m_dotsList.Count; i++)
		{
			if (i == m_page)
			{
				m_dotsList[i].Enable();
			}
			else
			{
				m_dotsList[i].Disable();
			}
		}
	}

	public void PreviousPage()
	{
		m_page = Mathf.Clamp(m_page - 1, 0, m_pageCount - 1);
		for (int i = 0; i < m_dotsList.Count; i++)
		{
			if (i == m_page)
			{
				m_dotsList[i].Enable();
			}
			else
			{
				m_dotsList[i].Disable();
			}
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
			SetPage(0);
			Layout();
			Transform transform = base.transform.Find("PageDots");
			int num = 0;
			if (transform != null)
			{
				num = transform.childCount;
			}
			if ((m_pageCount > 1 && transform == null) || num <= 1)
			{
				CreatePageDots();
			}
			else if (m_pageCount <= 1 && num >= 1)
			{
				transform.gameObject.SetActive(value: false);
			}
			else if (m_pageCount > 1 && num >= 1)
			{
				for (int i = 0; i < m_dotsList.Count; i++)
				{
					if (i == m_page)
					{
						m_dotsList[i].Enable();
					}
					else
					{
						m_dotsList[i].Disable();
					}
				}
			}
		}
		if (m_pageCount <= 1)
		{
			return;
		}
		if (!m_interacting)
		{
			Vector3 vector = new Vector3(0f - m_hudCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2 + Screen.width * m_page, 0f, 0f)).x, m_scrollPivot.transform.localPosition.y, m_scrollPivot.transform.localPosition.z);
			m_scrollPivot.transform.position += (vector - m_scrollPivot.transform.position) * Time.deltaTime * 4f;
			if ((vector - m_scrollPivot.transform.position).magnitude < 1f)
			{
				if (UserSettings.GetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_active_page", -1) != m_page)
				{
					UserSettings.SetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_active_page", m_page);
				}
				if (!DeviceInfo.UsesTouchInput)
				{
					m_rightScroll.SetActive(value: true);
					m_leftScroll.SetActive(value: true);
				}
			}
			else if (!DeviceInfo.UsesTouchInput)
			{
				m_rightScroll.SetActive(value: false);
				m_leftScroll.SetActive(value: false);
			}
			if (!DeviceInfo.UsesTouchInput)
			{
				if (CurrentPage == 0)
				{
					m_leftScroll.SetActive(value: false);
				}
				if (CurrentPage == m_pageCount - 1 || m_pageCount == 1)
				{
					m_rightScroll.SetActive(value: false);
				}
			}
		}
		if (LockScrolling)
		{
			return;
		}
		GuiManager.Pointer pointer = GuiManager.GetPointer();
		if (pointer.down && pointer.widget != m_leftScroll.GetComponent<Widget>() && pointer.widget != m_rightScroll.GetComponent<Widget>())
		{
			m_initialInputPos = pointer.position;
			m_lastInputPos = pointer.position;
			m_interacting = true;
		}
		if (pointer.dragging && m_interacting)
		{
			Vector3 vector2 = m_hudCamera.ScreenToWorldPoint(m_lastInputPos);
			Vector3 vector3 = m_hudCamera.ScreenToWorldPoint(pointer.position);
			m_lastInputPos = pointer.position;
			float num2 = vector3.x - vector2.x;
			m_scrollPivot.transform.localPosition = new Vector3(Mathf.Clamp(m_scrollPivot.transform.localPosition.x + num2, m_rightDragLimit, m_leftDragLimit), m_scrollPivot.transform.localPosition.y, m_scrollPivot.transform.localPosition.z);
			Vector3 vector4 = m_hudCamera.ScreenToWorldPoint(m_initialInputPos);
			if (!DeviceInfo.UsesTouchInput && Mathf.Abs(vector3.x - vector4.x) > 0.2f)
			{
				m_rightScroll.SetActive(value: false);
				m_leftScroll.SetActive(value: false);
			}
			if (Mathf.Abs(vector3.x - vector4.x) > 1f)
			{
				Singleton<GuiManager>.Instance.ResetFocus();
			}
		}
		if (!pointer.up || !m_interacting)
		{
			return;
		}
		float num3 = m_lastInputPos.x - m_initialInputPos.x;
		if (num3 < (0f - (float)Screen.width) / 16f)
		{
			m_page++;
			m_page = Mathf.Clamp(m_page, 0, m_pageCount - 1);
			for (int j = 0; j < m_dotsList.Count; j++)
			{
				if (j == m_page)
				{
					m_dotsList[j].Enable();
				}
				else
				{
					m_dotsList[j].Disable();
				}
			}
		}
		else if (num3 > (float)(Screen.width / 16))
		{
			m_page--;
			m_page = Mathf.Clamp(m_page, 0, m_pageCount - 1);
			for (int k = 0; k < m_dotsList.Count; k++)
			{
				if (k == m_page)
				{
					m_dotsList[k].Enable();
				}
				else
				{
					m_dotsList[k].Disable();
				}
			}
		}
		m_page = Mathf.Clamp(m_page, 0, m_pageCount - 1);
		m_interacting = false;
	}

	private void Layout()
	{
		float num = 2f * m_hudCamera.orthographicSize * (float)Screen.width / (float)Screen.height;
		int count = m_pageElements.Count;
		float num2 = EdgeMargin + m_elementWidth / 2f + m_scrollButtonMargin;
		float y = 0f;
		Vector3 vector = new Vector3((0f - num) / 2f + num2, y);
		float num3 = num - 2f * num2;
		float num4 = num3 / (float)(count - 1);
		if (num4 < MinGap || m_maxElementsPerPage > 0)
		{
			EdgeMargin = 2.5f;
			num2 = EdgeMargin + m_elementWidth / 2f + m_scrollButtonMargin;
			y = 0f;
			vector = new Vector3((0f - num) / 2f + num2, y);
			num3 = num - 2f * num2;
			m_elementsPerPage = (int)(num3 / MinGap) + 1;
			if (m_maxElementsPerPage > 0 && m_elementsPerPage > m_maxElementsPerPage)
			{
				m_elementsPerPage = m_maxElementsPerPage;
			}
			vector.x = (0f - (float)(m_elementsPerPage - 1) * MinGap) / 2f;
			num4 = MinGap;
			m_pageCount = m_pageElements.Count / m_elementsPerPage + ((m_pageElements.Count % m_elementsPerPage != 0) ? 1 : 0);
		}
		else if (num4 > MaxGap)
		{
			m_pageCount = 1;
			vector.x += (num4 - MaxGap) * (float)(count - 1) / 2f;
			num4 = MaxGap;
		}
		if (m_pageCount == 1)
		{
			for (int i = 0; i < count; i++)
			{
				m_pageElements[i].transform.position = vector + Vector3.right * num4 * i;
			}
		}
		else
		{
			Vector3 vector2 = vector;
			int num5 = 0;
			int num6 = 0;
			while (num5 < count)
			{
				int num7 = m_elementsPerPage;
				if (num7 > count - num5)
				{
					num7 = count - num5;
				}
				vector2.x = (0f - (float)(num7 - 1) * MinGap) / 2f;
				vector = vector2 + (float)num6 * num * Vector3.right;
				for (int j = 0; j < num7; j++)
				{
					m_pageElements[num5].transform.position = vector + Vector3.right * num4 * j;
					num5++;
					if (num5 >= count)
					{
						break;
					}
				}
				num6++;
			}
		}
		m_rightDragLimit = 0f - m_hudCamera.ScreenToWorldPoint(new Vector3((float)(Screen.width * m_pageCount) - EdgeMargin, 0f, 0f)).x;
		m_leftDragLimit = 0f - m_hudCamera.ScreenToWorldPoint(new Vector3(EdgeMargin, 0f, 0f)).x;
		if (DeviceInfo.UsesTouchInput || m_pageCount <= 1)
		{
			m_leftScroll.SetActive(value: false);
			m_rightScroll.SetActive(value: false);
		}
	}

	public int GetElementPage(GameObject element)
	{
		float num = 2f * m_hudCamera.orthographicSize * (float)Screen.width / (float)Screen.height;
		float num2 = EdgeMargin + m_elementWidth / 2f + m_scrollButtonMargin;
		int num3 = (int)((num - 2f * num2) / MinGap) + 1;
		if (m_maxElementsPerPage > 0 && num3 > m_maxElementsPerPage)
		{
			num3 = m_maxElementsPerPage;
		}
		for (int i = 0; i < m_pageElements.Count; i++)
		{
			if (m_pageElements[i] == element)
			{
				return i / num3;
			}
		}
		return 0;
	}

	private void CreatePageDots()
	{
		Vector3 position = -Vector3.up * m_hudCamera.orthographicSize / 1.25f;
		GameObject gameObject = Object.Instantiate(new GameObject(), position, Quaternion.identity);
		gameObject.name = "PageDots";
		gameObject.transform.parent = base.transform;
		float num = (0f - (float)m_pageCount) / 2f * 1.2f;
		for (int i = 0; i < m_pageCount; i++)
		{
			GameObject obj = Object.Instantiate(m_pageDot);
			obj.transform.parent = gameObject.transform;
			obj.transform.localPosition = new Vector3(num + (float)i * 1.2f, 0f, -95f);
			obj.name = "Dot" + i + 1;
			PageDot component = obj.GetComponent<PageDot>();
			m_dotsList.Add(component);
			if (i == m_page)
			{
				m_dotsList[i].Enable();
			}
			else
			{
				m_dotsList[i].Disable();
			}
		}
	}

	private bool isInInteractiveArea(Vector2 touchPos)
	{
		if (touchPos.y > (float)Screen.height * 0.1f)
		{
			return touchPos.y < (float)Screen.height * 0.8f;
		}
		return false;
	}

	private void HandleKeyListenerkeyReleased(KeyCode obj)
	{
		if (DeviceInfo.ActiveDeviceFamily != DeviceInfo.DeviceFamily.Android && obj == KeyCode.RightArrow && m_rightScroll.activeInHierarchy)
		{
			NextPage();
		}
		else if (DeviceInfo.ActiveDeviceFamily != DeviceInfo.DeviceFamily.Android && obj == KeyCode.LeftArrow && m_leftScroll.activeInHierarchy)
		{
			PreviousPage();
		}
	}

	private void HandleKeyListenerMouseWheel(float delta)
	{
		if (delta < 0f && m_rightScroll.activeInHierarchy && !m_interacting)
		{
			NextPage();
		}
		else if (delta > 0f && m_leftScroll.activeInHierarchy && !m_interacting)
		{
			PreviousPage();
		}
	}
}
