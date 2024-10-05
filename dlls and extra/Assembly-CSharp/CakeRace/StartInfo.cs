using System;
using System.Collections.Generic;
using UnityEngine;

namespace CakeRace;

[Serializable]
public struct StartInfo
{
	[SerializeField]
	private Vector3 m_position;

	[SerializeField]
	private int[] m_gridData;

	public Vector3 Position => m_position;

	public List<int> GridData => new List<int>(m_gridData);

	public void GetGridSize(out int columns, out int rows)
	{
		columns = 0;
		rows = 0;
		if (m_gridData == null)
		{
			return;
		}
		for (int i = 0; i < m_gridData.Length; i++)
		{
			if (m_gridData[i] > 0)
			{
				rows++;
				columns = Mathf.Max(columns, WPFMonoBehaviour.GetNumberOfHighestBit(m_gridData[i]) + 1);
			}
		}
	}
}
