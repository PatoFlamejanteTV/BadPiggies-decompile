using System.Collections.Generic;
using UnityEngine;

public class CakeRaceTutorialBook : TutorialBook
{
	private bool m_initialized;

	protected override void Awake()
	{
		if (CakeRaceMenu.WeeklyTrackIdentifiers == null)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < CakeRaceMenu.WeeklyTrackIdentifiers.Length; i++)
		{
			if (WPFMonoBehaviour.gameData.m_cakeRaceData.GetInfo(CakeRaceMenu.WeeklyTrackIdentifiers[i], out var info) && info.Value.TutorialBookPrefab != null)
			{
				GameObject tutorialBookPrefab = info.Value.TutorialBookPrefab;
				GameObject item = m_pages[m_pages.IndexOf(tutorialBookPrefab) + 1];
				if (!list.Contains(tutorialBookPrefab) && !list.Contains(item))
				{
					list.Add(tutorialBookPrefab);
					list.Add(item);
				}
				if (CakeRaceMenu.WinCount <= i)
				{
					m_lastOpenedPage = ((list.IndexOf(tutorialBookPrefab) <= m_lastOpenedPage) ? m_lastOpenedPage : list.IndexOf(tutorialBookPrefab));
				}
				if (CakeRaceMode.CurrentCakeRaceInfo.UniqueIdentifier == CakeRaceMenu.WeeklyTrackIdentifiers[i])
				{
					m_currentPage = list.IndexOf(tutorialBookPrefab);
				}
			}
		}
		if (list.Count >= 2)
		{
			m_pages = list;
		}
		else
		{
			m_lastOpenedPage = m_pages.Count;
		}
		m_initialized = true;
		Initialize();
	}

	private void OnEnable()
	{
		if (!m_initialized)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public override void TurnPageLeft()
	{
		base.TurnPageLeft();
	}

	public override void TurnPageRight()
	{
		base.TurnPageRight();
	}
}
