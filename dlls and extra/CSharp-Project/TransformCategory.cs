using System;
using UnityEngine;

public static class TransformCategory
{
	[Flags]
	public enum Axis
	{
		None = 0,
		X = 1,
		Y = 2,
		Z = 4
	}

	public static Transform FindChildRecursively(this Transform root, string name)
	{
		Transform transform = root.Find(name);
		if ((bool)transform)
		{
			return transform;
		}
		int childCount = root.childCount;
		for (int i = 0; i < childCount; i++)
		{
			transform = root.GetChild(i).FindChildRecursively(name);
			if ((bool)transform)
			{
				return transform;
			}
		}
		return null;
	}

	public static Transform FindChildOrInstantiate(this Transform root, string name, UnityEngine.Object original, Vector3 position, Quaternion rotation)
	{
		Transform transform = root.Find(name);
		if (!transform)
		{
			transform = ((GameObject)UnityEngine.Object.Instantiate(original, position, rotation)).transform;
			transform.gameObject.name = name;
			transform.transform.parent = root;
		}
		return transform;
	}

	public static void ResetPosition(this Transform target, Axis axis)
	{
		Vector3 position = target.transform.position;
		if ((axis & Axis.X) == Axis.X)
		{
			position.x = 0f;
		}
		if ((axis & Axis.Y) == Axis.Y)
		{
			position.y = 0f;
		}
		if ((axis & Axis.Z) == Axis.Z)
		{
			position.z = 0f;
		}
		target.transform.position = position;
	}

	public static void ResetRotation(this Transform target, Axis axis)
	{
		Vector3 eulerAngles = target.transform.eulerAngles;
		if ((axis & Axis.X) == Axis.X)
		{
			eulerAngles.x = 0f;
		}
		if ((axis & Axis.Y) == Axis.Y)
		{
			eulerAngles.y = 0f;
		}
		if ((axis & Axis.Z) == Axis.Z)
		{
			eulerAngles.z = 0f;
		}
		target.transform.rotation = Quaternion.Euler(eulerAngles);
	}
}
