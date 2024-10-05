using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DailyLevel
{
	[SerializeField]
	private int episodeIndex;

	[SerializeField]
	private int levelIndex;

	[SerializeField]
	private List<Vector3> collectablePositions;

	public int Count
	{
		get
		{
			if (collectablePositions == null)
			{
				return 0;
			}
			return collectablePositions.Count;
		}
	}

	public DailyLevel(int episodeIndex, int levelIndex)
	{
		this.episodeIndex = episodeIndex;
		this.levelIndex = levelIndex;
	}

	public string GetKey()
	{
		return episodeIndex + "-" + levelIndex;
	}

	public void AddPosition(Vector3 position)
	{
		SetPosition(int.MaxValue, position);
	}

	public void DeletePosition(int index)
	{
		if (index >= 0 && index < collectablePositions.Count)
		{
			collectablePositions.RemoveAt(index);
		}
	}

	public void SetPosition(int index, Vector3 position)
	{
		if (index >= 0)
		{
			if (collectablePositions == null)
			{
				collectablePositions = new List<Vector3>();
			}
			if (index < collectablePositions.Count)
			{
				collectablePositions[index] = position;
			}
			else
			{
				collectablePositions.Add(position);
			}
		}
	}

	public bool GetPosition(int index, out Vector3 position)
	{
		position = Vector3.zero;
		if (collectablePositions == null || index < 0 || index >= collectablePositions.Count)
		{
			return false;
		}
		position = collectablePositions[index];
		return true;
	}

	public bool ValidPositionIndex(int index)
	{
		if (collectablePositions != null && index >= 0)
		{
			return index < collectablePositions.Count;
		}
		return false;
	}
}
