using System.Collections.Generic;
using UnityEngine;

public class TutorialBook : WPFMonoBehaviour
{
	public List<GameObject> m_pages;

	protected int m_currentPage;

	private GameObject m_pagePivot;

	private Vector3 m_rightPagePosition;

	private Vector3 m_leftPagePosition;

	private GameObject m_leftPage;

	private GameObject m_rightPage;

	private GameObject m_flippingPage;

	private Vector3 m_flippingPagePosition;

	private const float m_leftPageOffset = -4.4f;

	private bool m_opening;

	private bool m_turningPageRight;

	private bool m_turningPageLeft;

	private GameObject m_leftPages;

	private GameObject m_cover;

	private GameObject m_hinge;

	private int m_pageState;

	private const int m_normalRenderQueue = 3000;

	private const int m_renderQueue = 3001;

	private const int m_sortingOrder = 100;

	private GameObject m_buttonsPanel;

	private GameObject m_nextPageButton;

	private GameObject m_previousPageButton;

	protected int m_lastOpenedPage;

	protected int m_firstOpenedPage;

	public GameObject GetPage(int pageNumber)
	{
		int num = pageNumber - 1;
		if (num >= 0 && num < m_pages.Count)
		{
			return m_pages[num];
		}
		return null;
	}

	protected virtual void Awake()
	{
		bool flag = (bool)WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.m_showPowerupTutorial;
		if (!flag && (bool)WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.m_levelCompleteTutorialBookPagePrefab != null)
		{
			m_currentPage = m_pages.IndexOf(WPFMonoBehaviour.levelManager.m_levelCompleteTutorialBookPagePrefab);
			if (m_currentPage == -1)
			{
				m_currentPage = 0;
			}
			WPFMonoBehaviour.levelManager.m_levelCompleteTutorialBookPagePrefab = null;
			if (m_currentPage > m_pages.Count - 1)
			{
				m_currentPage = m_pages.Count - 1;
			}
			if (m_currentPage % 2 != 0)
			{
				m_currentPage--;
			}
		}
		else if (!flag && (bool)WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.TutorialBookPage != null)
		{
			m_currentPage = m_pages.IndexOf(WPFMonoBehaviour.levelManager.TutorialBookPage);
			if (m_currentPage == -1)
			{
				m_currentPage = 0;
			}
			if (m_currentPage > m_pages.Count - 1)
			{
				m_currentPage = m_pages.Count - 1;
			}
			if (m_currentPage % 2 != 0)
			{
				m_currentPage--;
			}
		}
		else if (flag)
		{
			m_currentPage = 119;
		}
		m_lastOpenedPage = GameProgress.GetTutorialBookLastOpenedPage();
		if (m_currentPage > m_lastOpenedPage)
		{
			m_lastOpenedPage = m_currentPage;
			GameProgress.SetTutorialBookLastOpenedPage(m_lastOpenedPage);
		}
		m_firstOpenedPage = GameProgress.GetTutorialBookFirstOpenedPage();
		if (m_currentPage < m_firstOpenedPage)
		{
			m_firstOpenedPage = m_currentPage;
			GameProgress.SetTutorialBookFirstOpenedPage(m_firstOpenedPage);
		}
		if (flag)
		{
			m_currentPage = 110;
		}
		Initialize();
	}

	protected void Initialize()
	{
		m_hinge = base.transform.Find("Hinge").gameObject;
		m_leftPages = m_hinge.transform.Find("LeftPages").gameObject;
		m_cover = m_hinge.transform.Find("Cover").gameObject;
		m_pagePivot = base.transform.Find("PagePivot").gameObject;
		m_flippingPage = m_pagePivot.transform.Find("Page").gameObject;
		m_flippingPagePosition = m_flippingPage.transform.localPosition;
		m_rightPagePosition = m_flippingPage.transform.localPosition;
		m_leftPagePosition = m_rightPagePosition;
		m_leftPagePosition.x += -4.4f;
		m_cover.GetComponent<Renderer>().sortingOrder += 100;
		SetRightPage(m_pages[m_currentPage + 1]);
		m_leftPages.GetComponent<Renderer>().enabled = false;
		m_opening = true;
		m_pageState = 0;
		m_buttonsPanel = base.transform.Find("Buttons").gameObject;
		m_nextPageButton = m_buttonsPanel.transform.Find("NextPageButton").gameObject;
		m_previousPageButton = m_buttonsPanel.transform.Find("PreviousPageButton").gameObject;
		m_buttonsPanel.SetActive(value: false);
	}

	private void Update()
	{
		if (m_currentPage == m_pages.Count - 2)
		{
			m_nextPageButton.SetActive(value: false);
		}
		if (m_opening)
		{
			if (m_pageState == 0 && m_hinge.transform.rotation.eulerAngles.y > 270f)
			{
				m_leftPages.GetComponent<Renderer>().enabled = true;
				m_cover.GetComponent<Renderer>().enabled = false;
				m_flippingPage = Object.Instantiate(m_pages[m_currentPage]);
				m_flippingPage.transform.parent = m_hinge.transform;
				m_flippingPage.transform.localPosition = m_leftPages.transform.localPosition + new Vector3(0.43f, -0.045f, 0f);
				m_flippingPage.transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
				GameObject obj = m_flippingPage.transform.Find("Content").gameObject;
				Vector3 localScale = obj.transform.localScale;
				localScale.x *= -1f;
				obj.transform.localScale = localScale;
				obj.transform.localPosition += new Vector3(-0.53f, 0f, 0f);
				SetPageRenderOrder(m_flippingPage, 100);
				m_cover.GetComponent<Renderer>().sortingOrder -= 100;
				m_pageState = 1;
			}
			if (m_pageState == 1 && m_hinge.transform.rotation.eulerAngles.y == 0f)
			{
				Object.Destroy(m_flippingPage);
				SetLeftPage(m_pages[m_currentPage]);
				m_opening = false;
				m_buttonsPanel.SetActive(value: true);
				if (m_currentPage >= m_lastOpenedPage - 1)
				{
					m_nextPageButton.SetActive(value: false);
				}
				if (m_currentPage < m_firstOpenedPage + 2)
				{
					m_previousPageButton.SetActive(value: false);
				}
			}
		}
		if (m_turningPageRight)
		{
			if (m_pageState == 0 && m_pagePivot.transform.rotation.eulerAngles.y > 90f)
			{
				Object.Destroy(m_flippingPage);
				m_flippingPage = Object.Instantiate(m_pages[m_currentPage]);
				SetPageRenderOrder(m_flippingPage, 100);
				m_flippingPage.transform.parent = m_pagePivot.transform;
				m_flippingPage.transform.localPosition = m_flippingPagePosition;
				m_flippingPage.transform.localRotation = Quaternion.identity;
				GameObject obj2 = m_flippingPage.transform.Find("Content").gameObject;
				Vector3 localScale2 = obj2.transform.localScale;
				localScale2.x *= -1f;
				obj2.transform.localScale = localScale2;
				obj2.transform.localPosition += new Vector3(-0.53f, 0f, 0f);
				m_pageState = 1;
			}
			if (m_pageState == 1 && m_pagePivot.transform.rotation.eulerAngles.y >= 180f)
			{
				SetLeftPage(m_pages[m_currentPage]);
				Object.Destroy(m_flippingPage);
				m_turningPageRight = false;
				m_previousPageButton.SetActive(value: true);
				if (m_currentPage >= m_lastOpenedPage - 1)
				{
					m_nextPageButton.SetActive(value: false);
				}
			}
		}
		if (!m_turningPageLeft)
		{
			return;
		}
		if (m_pageState == 0 && m_pagePivot.transform.rotation.eulerAngles.y < 90f)
		{
			Object.Destroy(m_flippingPage);
			m_flippingPage = Object.Instantiate(m_pages[m_currentPage + 1]);
			SetPageRenderOrder(m_flippingPage, 100);
			m_flippingPage.transform.parent = m_pagePivot.transform;
			m_flippingPage.transform.localPosition = m_flippingPagePosition;
			m_flippingPage.transform.localRotation = Quaternion.identity;
			m_pageState = 1;
		}
		if (m_pageState == 1 && m_pagePivot.transform.rotation.eulerAngles.y <= 0f)
		{
			SetRightPage(m_pages[m_currentPage + 1]);
			Object.Destroy(m_flippingPage);
			m_turningPageLeft = false;
			m_nextPageButton.SetActive(value: true);
			if (m_currentPage < m_firstOpenedPage + 2)
			{
				m_previousPageButton.SetActive(value: false);
			}
		}
	}

	public virtual void TurnPageLeft()
	{
		if (m_opening || m_turningPageRight || m_turningPageLeft)
		{
			return;
		}
		if (m_currentPage >= m_firstOpenedPage + 2)
		{
			if ((bool)m_flippingPage)
			{
				Object.Destroy(m_flippingPage);
			}
			m_flippingPage = Object.Instantiate(m_pages[m_currentPage]);
			SetPageRenderOrder(m_flippingPage, 100);
			m_flippingPage.transform.parent = m_pagePivot.transform;
			m_flippingPage.transform.localPosition = m_flippingPagePosition;
			m_flippingPage.transform.localRotation = Quaternion.identity;
			GameObject obj = m_flippingPage.transform.Find("Content").gameObject;
			Vector3 localScale = obj.transform.localScale;
			localScale.x *= -1f;
			obj.transform.localScale = localScale;
			obj.transform.localPosition += new Vector3(-0.53f, 0f, 0f);
			m_pagePivot.GetComponent<Animation>().Play("PageTurnLeft");
			m_pagePivot.GetComponent<Animation>().Sample();
			m_turningPageLeft = true;
			m_pageState = 0;
			m_currentPage -= 2;
			SetLeftPage(m_pages[m_currentPage]);
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.tutorialFlip);
		}
		Resources.UnloadUnusedAssets();
	}

	public virtual void TurnPageRight()
	{
		if (m_opening || m_turningPageRight || m_turningPageLeft)
		{
			return;
		}
		if (m_currentPage < m_lastOpenedPage - 1)
		{
			if ((bool)m_flippingPage)
			{
				Object.Destroy(m_flippingPage);
			}
			m_flippingPage = Object.Instantiate(m_pages[m_currentPage + 1]);
			m_flippingPage.transform.parent = m_pagePivot.transform;
			m_flippingPage.transform.localPosition = m_flippingPagePosition;
			m_flippingPage.transform.localRotation = Quaternion.identity;
			SetPageRenderOrder(m_flippingPage, 100);
			m_pagePivot.GetComponent<Animation>().Play();
			m_pagePivot.GetComponent<Animation>().Sample();
			m_turningPageRight = true;
			m_pageState = 0;
			m_currentPage += 2;
			SetRightPage(m_pages[m_currentPage + 1]);
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.tutorialFlip);
		}
		Resources.UnloadUnusedAssets();
	}

	private void SetLeftPage(GameObject obj)
	{
		if ((bool)m_leftPage)
		{
			Object.Destroy(m_leftPage);
		}
		m_leftPage = Object.Instantiate(m_pages[m_currentPage]);
		m_leftPage.transform.parent = base.transform;
		m_leftPage.transform.localPosition = m_leftPagePosition;
		m_leftPage.GetComponent<Renderer>().enabled = false;
	}

	private void SetRightPage(GameObject obj)
	{
		if ((bool)m_rightPage)
		{
			Object.Destroy(m_rightPage);
		}
		m_rightPage = Object.Instantiate(m_pages[m_currentPage + 1]);
		m_rightPage.transform.parent = base.transform;
		m_rightPage.transform.localPosition = m_rightPagePosition;
		m_rightPage.GetComponent<Renderer>().enabled = false;
	}

	private void SetPageRenderOrder(GameObject obj, int sortingOrder)
	{
		if ((bool)obj.GetComponent<Renderer>())
		{
			obj.GetComponent<Renderer>().sortingOrder += sortingOrder;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			SetRenderQueueRecursively(obj.transform.GetChild(i).gameObject, sortingOrder + 1);
		}
	}

	private void SetRenderQueueRecursively(GameObject obj, int sortingOrder)
	{
		if ((bool)obj.GetComponent<Renderer>())
		{
			obj.GetComponent<Renderer>().sortingOrder += sortingOrder;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			SetRenderQueueRecursively(obj.transform.GetChild(i).gameObject, sortingOrder + 1);
		}
	}
}
