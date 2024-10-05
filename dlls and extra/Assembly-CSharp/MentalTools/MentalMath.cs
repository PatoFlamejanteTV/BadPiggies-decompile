using UnityEngine;

namespace MentalTools;

public abstract class MentalMath
{
	public enum AxisSpace
	{
		XY,
		XZ,
		YZ
	}

	public static float s { get; set; }

	public static bool PointInTriangle(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2, AxisSpace space)
	{
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		switch (space)
		{
		case AxisSpace.XY:
			num = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
			num2 = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;
			if (num < 0.0 != num2 < 0.0)
			{
				return false;
			}
			num3 = (0.0 - (double)p1.y) * (double)p2.x + (double)(p0.y * (p2.x - p1.x)) + (double)(p0.x * (p1.y - p2.y)) + (double)(p1.x * p2.y);
			if (num3 < 0.0)
			{
				num = 0.0 - num;
				num2 = 0.0 - num2;
				num3 = 0.0 - num3;
			}
			break;
		case AxisSpace.XZ:
			num = p0.z * p2.x - p0.x * p2.z + (p2.z - p0.z) * p.x + (p0.x - p2.x) * p.z;
			num2 = p0.x * p1.z - p0.z * p1.x + (p0.z - p1.z) * p.x + (p1.x - p0.x) * p.z;
			if (num < 0.0 != num2 < 0.0)
			{
				return false;
			}
			num3 = (0.0 - (double)p1.z) * (double)p2.x + (double)(p0.z * (p2.x - p1.x)) + (double)(p0.x * (p1.z - p2.z)) + (double)(p1.x * p2.z);
			if (num3 < 0.0)
			{
				num = 0.0 - num;
				num2 = 0.0 - num2;
				num3 = 0.0 - num3;
			}
			break;
		case AxisSpace.YZ:
			num = p0.z * p2.y - p0.y * p2.z + (p2.z - p0.z) * p.y + (p0.y - p2.y) * p.z;
			num2 = p0.y * p1.z - p0.z * p1.y + (p0.z - p1.z) * p.y + (p1.y - p0.y) * p.z;
			if (num < 0.0 != num2 < 0.0)
			{
				return false;
			}
			num3 = (0.0 - (double)p1.z) * (double)p2.y + (double)(p0.z * (p2.y - p1.y)) + (double)(p0.y * (p1.z - p2.z)) + (double)(p1.y * p2.z);
			if (num3 < 0.0)
			{
				num = 0.0 - num;
				num2 = 0.0 - num2;
				num3 = 0.0 - num3;
			}
			break;
		}
		if (num > 0.0 && num2 > 0.0)
		{
			return num + num2 < num3;
		}
		return false;
	}

	public static Vector3 LeftSideNormal(Vector3 tangent)
	{
		return new Vector3(0f - tangent.z, 0f, tangent.x).normalized;
	}

	public static Vector3 GetCounterClockwiseNormal(Vector3 tangent, AxisSpace space)
	{
		Vector3 vector = Vector3.zero;
		Vector3 zero = Vector3.zero;
		switch (space)
		{
		case AxisSpace.XY:
			zero = Vector3.back;
			vector = new Vector3(0f - tangent.y, tangent.x, 0f).normalized;
			break;
		case AxisSpace.XZ:
			zero = Vector3.up;
			vector = Vector3.Cross(tangent, zero);
			break;
		case AxisSpace.YZ:
			zero = Vector3.right;
			vector = Vector3.Cross(tangent, zero);
			break;
		}
		return vector.normalized;
	}

	public static Vector3 GetClockwiseNormal(Vector3 tangent, AxisSpace space)
	{
		Vector3 vector = Vector3.zero;
		Vector3 zero = Vector3.zero;
		switch (space)
		{
		case AxisSpace.XY:
			zero = Vector3.forward;
			vector = Vector3.Cross(tangent, zero);
			break;
		case AxisSpace.XZ:
			zero = Vector3.down;
			vector = Vector3.Cross(tangent, zero);
			break;
		case AxisSpace.YZ:
			zero = Vector3.left;
			vector = Vector3.Cross(tangent, zero);
			break;
		}
		return vector.normalized;
	}
}
