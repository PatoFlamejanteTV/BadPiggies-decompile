using System.Collections.Generic;
using UnityEngine;

public class ResponseCurve
{
	private struct DataPoint
	{
		public float x;

		public float y;

		public DataPoint(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
	}

	private List<DataPoint> data = new List<DataPoint>();

	public void AddPoint(float x, float value)
	{
		data.Add(new DataPoint(x, value));
	}

	public float Get(float x)
	{
		if (data.Count < 2)
		{
			return 0f;
		}
		DataPoint dataPoint = data[0];
		DataPoint dataPoint2 = data[data.Count - 1];
		if (x < dataPoint.x)
		{
			return dataPoint.y;
		}
		if (x > dataPoint2.x)
		{
			return dataPoint2.y;
		}
		for (int i = 0; i < data.Count - 1; i++)
		{
			if (x >= data[i].x && x <= data[i + 1].x)
			{
				dataPoint = data[i];
				dataPoint2 = data[i + 1];
				break;
			}
		}
		return Mathf.Lerp(dataPoint.y, dataPoint2.y, (x - dataPoint.x) / (dataPoint2.x - dataPoint.x));
	}
}
