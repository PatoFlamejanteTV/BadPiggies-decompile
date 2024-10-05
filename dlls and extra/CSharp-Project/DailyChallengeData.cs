using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DailyChallengeData : ScriptableObject
{
	[SerializeField]
	private List<DailyLevel> dailyLevels;

	public int Count => dailyLevels.Count;

	private void OnEnable()
	{
		if (dailyLevels == null)
		{
			dailyLevels = new List<DailyLevel>();
		}
	}

	public void AddDaily(DailyLevel daily)
	{
		if (GetDaily(daily.GetKey()) == null)
		{
			dailyLevels.Add(daily);
		}
	}

	public DailyLevel GetDaily(string key)
	{
		for (int i = 0; i < dailyLevels.Count; i++)
		{
			if (dailyLevels[i].GetKey().Equals(key))
			{
				return dailyLevels[i];
			}
		}
		return null;
	}
}
