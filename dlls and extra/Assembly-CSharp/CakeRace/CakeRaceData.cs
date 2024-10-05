using System;
using System.Collections.Generic;
using UnityEngine;

namespace CakeRace;

[Serializable]
public class CakeRaceData : ScriptableObject
{
	private static CakeRaceData instance;

	[SerializeField]
	private List<CakeRaceInfo> data;

	public bool GetInfo(int episodeIndex, int levelIndex, int trackIndex, out CakeRaceInfo? info)
	{
		info = null;
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].EpisodeIndex == episodeIndex && data[i].LevelIndex == levelIndex && data[i].TrackIndex == trackIndex && data[i].RegularLevel)
			{
				info = data[i];
				return true;
			}
		}
		return false;
	}

	public bool GetInfo(string identifier, int trackIndex, out CakeRaceInfo? info)
	{
		info = null;
		if (string.IsNullOrEmpty(identifier))
		{
			return false;
		}
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].Identifier == identifier && data[i].TrackIndex == trackIndex && data[i].SpecialLevel)
			{
				info = data[i];
				return true;
			}
		}
		return false;
	}

	public bool GetInfo(string uniqueIdentifier, out CakeRaceInfo? info)
	{
		info = null;
		if (string.IsNullOrEmpty(uniqueIdentifier) || !uniqueIdentifier.Contains("_") || !uniqueIdentifier.Contains("-"))
		{
			return false;
		}
		string[] array = uniqueIdentifier.Split('_');
		string text = array[0];
		string[] array2 = text.Split('-');
		int result = -1;
		int result2 = -1;
		int result3 = -1;
		if (array2.Length >= 2 && int.TryParse(array2[0], out result) && int.TryParse(array2[1], out result2))
		{
			text = string.Empty;
		}
		if (array.Length >= 2 && !int.TryParse(array[1], out result3))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(text))
		{
			return GetInfo(text, result3, out info);
		}
		if (result >= 0 && result2 >= 0)
		{
			return GetInfo(result, result2, result3, out info);
		}
		return false;
	}

	public int GetTrackCount(int episodeIndex, int levelIndex)
	{
		int num = 0;
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].EpisodeIndex == episodeIndex && data[i].LevelIndex == levelIndex && data[i].RegularLevel)
			{
				num++;
			}
		}
		return num;
	}

	public int GetTrackCount(string identifier)
	{
		int num = 0;
		if (string.IsNullOrEmpty(identifier))
		{
			return num;
		}
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].Identifier == identifier && data[i].SpecialLevel)
			{
				num++;
			}
		}
		return num;
	}
}
