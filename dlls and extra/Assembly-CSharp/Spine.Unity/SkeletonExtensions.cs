using UnityEngine;

namespace Spine.Unity;

public static class SkeletonExtensions
{
	private const float ByteToFloat = 0.003921569f;

	public static Color GetColor(this Skeleton s)
	{
		return new Color(s.r, s.g, s.b, s.a);
	}

	public static Color GetColor(this RegionAttachment a)
	{
		return new Color(a.r, a.g, a.b, a.a);
	}

	public static Color GetColor(this MeshAttachment a)
	{
		return new Color(a.r, a.g, a.b, a.a);
	}

	public static void SetColor(this Skeleton skeleton, Color color)
	{
		skeleton.A = color.a;
		skeleton.R = color.r;
		skeleton.G = color.g;
		skeleton.B = color.b;
	}

	public static void SetColor(this Skeleton skeleton, Color32 color)
	{
		skeleton.A = (float)(int)color.a * 0.003921569f;
		skeleton.R = (float)(int)color.r * 0.003921569f;
		skeleton.G = (float)(int)color.g * 0.003921569f;
		skeleton.B = (float)(int)color.b * 0.003921569f;
	}

	public static void SetColor(this Slot slot, Color color)
	{
		slot.A = color.a;
		slot.R = color.r;
		slot.G = color.g;
		slot.B = color.b;
	}

	public static void SetColor(this Slot slot, Color32 color)
	{
		slot.A = (float)(int)color.a * 0.003921569f;
		slot.R = (float)(int)color.r * 0.003921569f;
		slot.G = (float)(int)color.g * 0.003921569f;
		slot.B = (float)(int)color.b * 0.003921569f;
	}

	public static void SetColor(this RegionAttachment attachment, Color color)
	{
		attachment.A = color.a;
		attachment.R = color.r;
		attachment.G = color.g;
		attachment.B = color.b;
	}

	public static void SetColor(this RegionAttachment attachment, Color32 color)
	{
		attachment.A = (float)(int)color.a * 0.003921569f;
		attachment.R = (float)(int)color.r * 0.003921569f;
		attachment.G = (float)(int)color.g * 0.003921569f;
		attachment.B = (float)(int)color.b * 0.003921569f;
	}

	public static void SetColor(this MeshAttachment attachment, Color color)
	{
		attachment.A = color.a;
		attachment.R = color.r;
		attachment.G = color.g;
		attachment.B = color.b;
	}

	public static void SetColor(this MeshAttachment attachment, Color32 color)
	{
		attachment.A = (float)(int)color.a * 0.003921569f;
		attachment.R = (float)(int)color.r * 0.003921569f;
		attachment.G = (float)(int)color.g * 0.003921569f;
		attachment.B = (float)(int)color.b * 0.003921569f;
	}

	public static void SetPosition(this Bone bone, Vector2 position)
	{
		bone.X = position.x;
		bone.Y = position.y;
	}

	public static void SetPosition(this Bone bone, Vector3 position)
	{
		bone.X = position.x;
		bone.Y = position.y;
	}

	public static Vector2 GetSkeletonSpacePosition(this Bone bone)
	{
		return new Vector2(bone.worldX, bone.worldY);
	}

	public static Vector3 GetWorldPosition(this Bone bone, Transform parentTransform)
	{
		return parentTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY));
	}

	public static Matrix4x4 GetMatrix4x4(this Bone bone)
	{
		Matrix4x4 result = default(Matrix4x4);
		result.m00 = bone.a;
		result.m01 = bone.b;
		result.m03 = bone.worldX;
		result.m10 = bone.c;
		result.m11 = bone.d;
		result.m13 = bone.worldY;
		result.m33 = 1f;
		return result;
	}
}
