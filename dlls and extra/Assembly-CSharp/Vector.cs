using System;
using UnityEngine;

public static class Vector
{
	public static void Transform(float valueX, float valueY, float directionX, float directionY, out float resultX, out float resultY)
	{
		resultX = valueX * directionX + valueY * directionY;
		resultY = valueX * (0f - directionY) + valueY * directionX;
	}

	public static Vector2 Transform(Vector2 value, Vector2 direction)
	{
		float x = value.x * direction.x + value.y * direction.y;
		float y = value.x * (0f - direction.y) + value.y * direction.x;
		return new Vector2(x, y);
	}

	public static Vector3 Transform(Vector3 value, Vector3 direction)
	{
		float x = value.x * direction.x + value.y * direction.y;
		float y = value.x * (0f - direction.y) + value.y * direction.x;
		return new Vector3(x, y);
	}

	public static float DistanceSquared(float leftX, float leftY, float rightX, float rightY)
	{
		float num = leftX - rightX;
		float num2 = leftY - rightY;
		return num * num + num2 * num2;
	}

	public static float DistanceSquared(Vector2 left, Vector2 right)
	{
		float num = left.x - right.x;
		float num2 = left.y - right.y;
		return num * num + num2 * num2;
	}

	public static float DistanceSquared(Vector3 left, Vector3 right)
	{
		float num = left.x - right.x;
		float num2 = left.y - right.y;
		return num * num + num2 * num2;
	}

	public static float Distance(float leftX, float leftY, float rightX, float rightY)
	{
		float num = leftX - rightX;
		float num2 = leftY - rightY;
		return (float)Math.Sqrt(num * num + num2 * num2);
	}

	public static float Distance(Vector2 left, Vector2 right)
	{
		float num = left.x - right.x;
		float num2 = left.y - right.y;
		return (float)Math.Sqrt(num * num + num2 * num2);
	}

	public static float Distance(Vector3 left, Vector3 right)
	{
		float num = left.x - right.x;
		float num2 = left.y - right.y;
		return (float)Math.Sqrt(num * num + num2 * num2);
	}
}
