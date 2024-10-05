using System;

namespace Spine;

public class Bone : IUpdatable
{
	public static bool yDown;

	internal BoneData data;

	internal Skeleton skeleton;

	internal Bone parent;

	internal ExposedList<Bone> children = new ExposedList<Bone>();

	internal float x;

	internal float y;

	internal float rotation;

	internal float scaleX;

	internal float scaleY;

	internal float shearX;

	internal float shearY;

	internal float appliedRotation;

	internal float a;

	internal float b;

	internal float worldX;

	internal float c;

	internal float d;

	internal float worldY;

	internal float worldSignX;

	internal float worldSignY;

	internal bool sorted;

	public BoneData Data => data;

	public Skeleton Skeleton => skeleton;

	public Bone Parent => parent;

	public ExposedList<Bone> Children => children;

	public float X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public float Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
		}
	}

	public float AppliedRotation
	{
		get
		{
			return appliedRotation;
		}
		set
		{
			appliedRotation = value;
		}
	}

	public float ScaleX
	{
		get
		{
			return scaleX;
		}
		set
		{
			scaleX = value;
		}
	}

	public float ScaleY
	{
		get
		{
			return scaleY;
		}
		set
		{
			scaleY = value;
		}
	}

	public float ShearX
	{
		get
		{
			return shearX;
		}
		set
		{
			shearX = value;
		}
	}

	public float ShearY
	{
		get
		{
			return shearY;
		}
		set
		{
			shearY = value;
		}
	}

	public float A => a;

	public float B => b;

	public float C => c;

	public float D => d;

	public float WorldX => worldX;

	public float WorldY => worldY;

	public float WorldSignX => worldSignX;

	public float WorldSignY => worldSignY;

	public float WorldRotationX => MathUtils.Atan2(c, a) * (180f / (float)Math.PI);

	public float WorldRotationY => MathUtils.Atan2(d, b) * (180f / (float)Math.PI);

	public float WorldScaleX => (float)Math.Sqrt(a * a + c * c) * worldSignX;

	public float WorldScaleY => (float)Math.Sqrt(b * b + d * d) * worldSignY;

	public float WorldToLocalRotationX
	{
		get
		{
			Bone bone = parent;
			if (bone == null)
			{
				return rotation;
			}
			float num = bone.a;
			float num2 = bone.b;
			float num3 = bone.c;
			float num4 = bone.d;
			float num5 = a;
			float num6 = c;
			return MathUtils.Atan2(num * num6 - num3 * num5, num4 * num5 - num2 * num6) * (180f / (float)Math.PI);
		}
	}

	public float WorldToLocalRotationY
	{
		get
		{
			Bone bone = parent;
			if (bone == null)
			{
				return rotation;
			}
			float num = bone.a;
			float num2 = bone.b;
			float num3 = bone.c;
			float num4 = bone.d;
			float num5 = b;
			float num6 = d;
			return MathUtils.Atan2(num * num6 - num3 * num5, num4 * num5 - num2 * num6) * (180f / (float)Math.PI);
		}
	}

	public Bone(BoneData data, Skeleton skeleton, Bone parent)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data", "data cannot be null.");
		}
		if (skeleton == null)
		{
			throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
		}
		this.data = data;
		this.skeleton = skeleton;
		this.parent = parent;
		SetToSetupPose();
	}

	public void Update()
	{
		UpdateWorldTransform(x, y, rotation, scaleX, scaleY, shearX, shearY);
	}

	public void UpdateWorldTransform()
	{
		UpdateWorldTransform(x, y, rotation, scaleX, scaleY, shearX, shearY);
	}

	public void UpdateWorldTransform(float x, float y, float rotation, float scaleX, float scaleY, float shearX, float shearY)
	{
		appliedRotation = rotation;
		float degrees = rotation + 90f + shearY;
		float num = MathUtils.CosDeg(rotation + shearX) * scaleX;
		float num2 = MathUtils.CosDeg(degrees) * scaleY;
		float num3 = MathUtils.SinDeg(rotation + shearX) * scaleX;
		float num4 = MathUtils.SinDeg(degrees) * scaleY;
		Bone bone = parent;
		if (bone == null)
		{
			Skeleton obj = skeleton;
			if (obj.flipX)
			{
				x = 0f - x;
				num = 0f - num;
				num2 = 0f - num2;
			}
			if (obj.flipY != yDown)
			{
				y = 0f - y;
				num3 = 0f - num3;
				num4 = 0f - num4;
			}
			a = num;
			b = num2;
			c = num3;
			d = num4;
			worldX = x;
			worldY = y;
			worldSignX = Math.Sign(scaleX);
			worldSignY = Math.Sign(scaleY);
			return;
		}
		float num5 = bone.a;
		float num6 = bone.b;
		float num7 = bone.c;
		float num8 = bone.d;
		worldX = num5 * x + num6 * y + bone.worldX;
		worldY = num7 * x + num8 * y + bone.worldY;
		worldSignX = bone.worldSignX * (float)Math.Sign(scaleX);
		worldSignY = bone.worldSignY * (float)Math.Sign(scaleY);
		if (data.inheritRotation && data.inheritScale)
		{
			a = num5 * num + num6 * num3;
			b = num5 * num2 + num6 * num4;
			c = num7 * num + num8 * num3;
			d = num7 * num2 + num8 * num4;
			return;
		}
		if (data.inheritRotation)
		{
			num5 = 1f;
			num6 = 0f;
			num7 = 0f;
			num8 = 1f;
			do
			{
				float num9 = MathUtils.CosDeg(bone.appliedRotation);
				float num10 = MathUtils.SinDeg(bone.appliedRotation);
				float num11 = num5 * num9 + num6 * num10;
				num6 = num6 * num9 - num5 * num10;
				num5 = num11;
				float num12 = num7 * num9 + num8 * num10;
				num8 = num8 * num9 - num7 * num10;
				num7 = num12;
				if (!bone.data.inheritRotation)
				{
					break;
				}
				bone = bone.parent;
			}
			while (bone != null);
			a = num5 * num + num6 * num3;
			b = num5 * num2 + num6 * num4;
			c = num7 * num + num8 * num3;
			d = num7 * num2 + num8 * num4;
		}
		else if (data.inheritScale)
		{
			num5 = 1f;
			num6 = 0f;
			num7 = 0f;
			num8 = 1f;
			do
			{
				float num13 = MathUtils.CosDeg(bone.appliedRotation);
				float num14 = MathUtils.SinDeg(bone.appliedRotation);
				float num15 = bone.scaleX;
				float num16 = bone.scaleY;
				float num17 = num13 * num15;
				float num18 = num14 * num16;
				float num19 = num14 * num15;
				float num20 = num13 * num16;
				float num21 = num5 * num17 + num6 * num19;
				num6 = num6 * num20 - num5 * num18;
				num5 = num21;
				float num22 = num7 * num17 + num8 * num19;
				num8 = num8 * num20 - num7 * num18;
				num7 = num22;
				if (num15 >= 0f)
				{
					num14 = 0f - num14;
				}
				float num23 = num5 * num13 + num6 * num14;
				num6 = num6 * num13 - num5 * num14;
				num5 = num23;
				float num24 = num7 * num13 + num8 * num14;
				num8 = num8 * num13 - num7 * num14;
				num7 = num24;
				if (!bone.data.inheritScale)
				{
					break;
				}
				bone = bone.parent;
			}
			while (bone != null);
			a = num5 * num + num6 * num3;
			b = num5 * num2 + num6 * num4;
			c = num7 * num + num8 * num3;
			d = num7 * num2 + num8 * num4;
		}
		else
		{
			a = num;
			b = num2;
			c = num3;
			d = num4;
		}
		if (skeleton.flipX)
		{
			a = 0f - a;
			b = 0f - b;
		}
		if (skeleton.flipY != yDown)
		{
			c = 0f - c;
			d = 0f - d;
		}
	}

	public void SetToSetupPose()
	{
		BoneData boneData = data;
		x = boneData.x;
		y = boneData.y;
		rotation = boneData.rotation;
		scaleX = boneData.scaleX;
		scaleY = boneData.scaleY;
		shearX = boneData.shearX;
		shearY = boneData.shearY;
	}

	public void RotateWorld(float degrees)
	{
		float num = a;
		float num2 = b;
		float num3 = c;
		float num4 = d;
		float num5 = MathUtils.CosDeg(degrees);
		float num6 = MathUtils.SinDeg(degrees);
		a = num5 * num - num6 * num3;
		b = num5 * num2 - num6 * num4;
		c = num6 * num + num5 * num3;
		d = num6 * num2 + num5 * num4;
	}

	public void UpdateLocalTransform()
	{
		Bone bone = parent;
		if (bone == null)
		{
			x = worldX;
			y = worldY;
			rotation = MathUtils.Atan2(c, a) * (180f / (float)Math.PI);
			scaleX = (float)Math.Sqrt(a * a + c * c);
			scaleY = (float)Math.Sqrt(b * b + d * d);
			float num = a * d - b * c;
			shearX = 0f;
			shearY = MathUtils.Atan2(a * b + c * d, num) * (180f / (float)Math.PI);
			return;
		}
		float num2 = bone.a;
		float num3 = bone.b;
		float num4 = bone.c;
		float num5 = bone.d;
		float num6 = 1f / (num2 * num5 - num3 * num4);
		float num7 = worldX - bone.worldX;
		float num8 = worldY - bone.worldY;
		x = num7 * num5 * num6 - num8 * num3 * num6;
		y = num8 * num2 * num6 - num7 * num4 * num6;
		float num9 = num6 * num5;
		float num10 = num6 * num2;
		float num11 = num6 * num3;
		float num12 = num6 * num4;
		float num13 = num9 * a - num11 * c;
		float num14 = num9 * b - num11 * d;
		float num15 = num10 * c - num12 * a;
		float num16 = num10 * d - num12 * b;
		shearX = 0f;
		scaleX = (float)Math.Sqrt(num13 * num13 + num15 * num15);
		if (scaleX > 0.0001f)
		{
			float num17 = num13 * num16 - num14 * num15;
			scaleY = num17 / scaleX;
			shearY = MathUtils.Atan2(num13 * num14 + num15 * num16, num17) * (180f / (float)Math.PI);
			rotation = MathUtils.Atan2(num15, num13) * (180f / (float)Math.PI);
		}
		else
		{
			scaleX = 0f;
			scaleY = (float)Math.Sqrt(num14 * num14 + num16 * num16);
			shearY = 0f;
			rotation = 90f - MathUtils.Atan2(num16, num14) * (180f / (float)Math.PI);
		}
		appliedRotation = rotation;
	}

	public void WorldToLocal(float worldX, float worldY, out float localX, out float localY)
	{
		float num = a;
		float num2 = b;
		float num3 = c;
		float num4 = d;
		float num5 = 1f / (num * num4 - num2 * num3);
		float num6 = worldX - this.worldX;
		float num7 = worldY - this.worldY;
		localX = num6 * num4 * num5 - num7 * num2 * num5;
		localY = num7 * num * num5 - num6 * num3 * num5;
	}

	public void LocalToWorld(float localX, float localY, out float worldX, out float worldY)
	{
		worldX = localX * a + localY * b + this.worldX;
		worldY = localX * c + localY * d + this.worldY;
	}

	public override string ToString()
	{
		return data.name;
	}
}
