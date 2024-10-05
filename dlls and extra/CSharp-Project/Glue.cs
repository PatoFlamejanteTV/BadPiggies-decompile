using System.Collections.Generic;
using UnityEngine;

public class Glue : MonoBehaviour
{
	public enum Direction
	{
		Left = 1,
		Right = 2,
		Up = 4,
		Down = 8
	}

	public enum Type
	{
		None,
		Regular,
		Alien
	}

	public int m_GlueDirectionMask;

	public static void ShowSuperGlue(Contraption contraption, Type type)
	{
		if (type != 0)
		{
			if (contraption.CurrentGlue != 0 || contraption.CurrentGlue != type)
			{
				RemoveSuperGlue(contraption);
			}
			ShowSuperGlue(contraption, type, contraption.Parts);
		}
	}

	public static void ShowSuperGlue(Contraption contraption, Type type, List<BasePart> parts)
	{
		foreach (BasePart part in parts)
		{
			if (IsPartTypeRequireGlue(part.m_partType))
			{
				BasePart anotherPart = contraption.FindPartAt(part.m_coordX - 1, part.m_coordY);
				BasePart anotherPart2 = contraption.FindPartAt(part.m_coordX + 1, part.m_coordY);
				BasePart anotherPart3 = contraption.FindPartAt(part.m_coordX, part.m_coordY - 1);
				BasePart anotherPart4 = contraption.FindPartAt(part.m_coordX, part.m_coordY + 1);
				if (part.GetComponent<Glue>() == null)
				{
					part.gameObject.AddComponent<Glue>();
				}
				CheckSideAndAppendGlue(part, anotherPart, Direction.Left, type);
				CheckSideAndAppendGlue(part, anotherPart2, Direction.Right, type);
				CheckSideAndAppendGlue(part, anotherPart4, Direction.Up, type);
				CheckSideAndAppendGlue(part, anotherPart3, Direction.Down, type);
			}
		}
	}

	public static void RemoveSuperGlue(Contraption contraption)
	{
		RemoveSuperGlue(contraption.Parts);
	}

	public static void RemoveSuperGlue(List<BasePart> parts)
	{
		foreach (BasePart part in parts)
		{
			if (!IsPartTypeRequireGlue(part.m_partType))
			{
				continue;
			}
			Glue component = part.GetComponent<Glue>();
			if (!(component != null))
			{
				continue;
			}
			for (int num = part.transform.childCount - 1; num >= 0; num--)
			{
				GameObject gameObject = part.transform.GetChild(num).gameObject;
				if (gameObject.name.StartsWith(WPFMonoBehaviour.gameData.m_glueSprite.name) || gameObject.name.StartsWith(WPFMonoBehaviour.gameData.m_alienGlueSprite.name))
				{
					Object.DestroyImmediate(gameObject);
				}
			}
			Object.DestroyImmediate(component);
		}
	}

	public static bool ContraptionHasGluedParts(Contraption contraption)
	{
		foreach (BasePart part in contraption.Parts)
		{
			if (part.GetComponent<Glue>() != null)
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsPartTypeRequireGlue(BasePart.PartType type)
	{
		if (type != BasePart.PartType.WoodenFrame)
		{
			return type == BasePart.PartType.MetalFrame;
		}
		return true;
	}

	private static void AddSuperGlueSprite(Glue glue, Direction dir, Type type)
	{
		GameObject original = ((type != Type.Alien) ? WPFMonoBehaviour.gameData.m_glueSprite : WPFMonoBehaviour.gameData.m_alienGlueSprite);
		Vector3 localPosition = new Vector3(dir switch
		{
			Direction.Left => -0.5f, 
			Direction.Right => 0.5f, 
			_ => 0f, 
		}, dir switch
		{
			Direction.Down => -0.5f, 
			Direction.Up => 0.5f, 
			_ => 0f, 
		}, -0.01f);
		Quaternion localRotation = Quaternion.AngleAxis((dir != Direction.Up && dir != Direction.Down) ? 0f : 90f, Vector3.back);
		Transform obj = Object.Instantiate(original).transform;
		obj.parent = glue.transform;
		obj.localPosition = localPosition;
		obj.localRotation = localRotation;
		glue.m_GlueDirectionMask |= (int)dir;
	}

	private static void CheckSideAndAppendGlue(BasePart partWithGlue, BasePart anotherPart, Direction dir, Type type)
	{
		bool flag = dir == Direction.Up || dir == Direction.Down;
		bool flag2 = dir == Direction.Left || dir == Direction.Right;
		Glue component = partWithGlue.GetComponent<Glue>();
		if (anotherPart != null)
		{
			Glue component2 = anotherPart.GetComponent<Glue>();
			BasePart.JointConnectionDirection jointConnectionDirection = anotherPart.GetJointConnectionDirection();
			bool flag3 = (BasePart.IsDirection(jointConnectionDirection) && OppositeGlueDir(BasePart.ConvertDirection(jointConnectionDirection)) == dir) || (flag && jointConnectionDirection == BasePart.JointConnectionDirection.UpAndDown) || (flag2 && jointConnectionDirection == BasePart.JointConnectionDirection.LeftAndRight) || (jointConnectionDirection == BasePart.JointConnectionDirection.Any && anotherPart.GetJointConnectionType() != BasePart.JointConnectionType.None);
			if (((uint)component.m_GlueDirectionMask & (uint)dir) == 0)
			{
				if ((bool)component2)
				{
					if (((uint)component2.m_GlueDirectionMask & (uint)OppositeGlueDir(dir)) == 0)
					{
						AddSuperGlueSprite(component, dir, type);
					}
				}
				else if (IsPartTypeRequireGlue(anotherPart.m_partType) || flag3)
				{
					AddSuperGlueSprite(component, dir, type);
				}
				else
				{
					RemoveSuperGlueSprite(component, dir);
				}
			}
			else if (!IsPartTypeRequireGlue(anotherPart.m_partType) && !flag3)
			{
				RemoveSuperGlueSprite(component, dir);
			}
		}
		else if (!anotherPart && ((uint)component.m_GlueDirectionMask & (uint)dir) != 0)
		{
			RemoveSuperGlueSprite(component, dir);
		}
	}

	private static void RemoveSuperGlueSprite(Glue glue, Direction dir)
	{
		if (!(glue != null) || ((uint)glue.m_GlueDirectionMask & (uint)dir) == 0)
		{
			return;
		}
		for (int num = glue.transform.childCount - 1; num >= 0; num--)
		{
			GameObject gameObject = glue.transform.GetChild(num).gameObject;
			if (gameObject.name.StartsWith(WPFMonoBehaviour.gameData.m_glueSprite.name) || gameObject.name.StartsWith(WPFMonoBehaviour.gameData.m_alienGlueSprite.name))
			{
				switch (dir)
				{
				case Direction.Left:
					if (gameObject.transform.localPosition.x < 0f)
					{
						Object.DestroyImmediate(gameObject);
					}
					break;
				case Direction.Right:
					if (gameObject.transform.localPosition.x > 0f)
					{
						Object.DestroyImmediate(gameObject);
					}
					break;
				case Direction.Up:
					if (gameObject.transform.localPosition.y > 0f)
					{
						Object.DestroyImmediate(gameObject);
					}
					break;
				case Direction.Down:
					if (gameObject.transform.localPosition.y < 0f)
					{
						Object.DestroyImmediate(gameObject);
					}
					break;
				}
			}
		}
		glue.m_GlueDirectionMask &= (int)(~dir);
	}

	public static Direction OppositeGlueDir(Direction dir)
	{
		switch (dir)
		{
		case Direction.Left:
		case Direction.Right:
			if (dir == Direction.Left)
			{
				return Direction.Right;
			}
			return Direction.Left;
		case Direction.Up:
			return Direction.Down;
		default:
			return Direction.Up;
		}
	}

	public static Direction OppositeGlueDir(BasePart.Direction dir)
	{
		switch (dir)
		{
		case BasePart.Direction.Right:
		case BasePart.Direction.Left:
			if (dir == BasePart.Direction.Left)
			{
				return Direction.Right;
			}
			return Direction.Left;
		case BasePart.Direction.Up:
			return Direction.Down;
		default:
			return Direction.Up;
		}
	}
}
