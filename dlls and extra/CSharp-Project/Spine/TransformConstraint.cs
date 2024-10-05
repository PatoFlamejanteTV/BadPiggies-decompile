using System;

namespace Spine;

public class TransformConstraint : IUpdatable
{
	internal TransformConstraintData data;

	internal ExposedList<Bone> bones;

	internal Bone target;

	internal float rotateMix;

	internal float translateMix;

	internal float scaleMix;

	internal float shearMix;

	public TransformConstraintData Data => data;

	public ExposedList<Bone> Bones => bones;

	public Bone Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
		}
	}

	public float RotateMix
	{
		get
		{
			return rotateMix;
		}
		set
		{
			rotateMix = value;
		}
	}

	public float TranslateMix
	{
		get
		{
			return translateMix;
		}
		set
		{
			translateMix = value;
		}
	}

	public float ScaleMix
	{
		get
		{
			return scaleMix;
		}
		set
		{
			scaleMix = value;
		}
	}

	public float ShearMix
	{
		get
		{
			return shearMix;
		}
		set
		{
			shearMix = value;
		}
	}

	public TransformConstraint(TransformConstraintData data, Skeleton skeleton)
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
		rotateMix = data.rotateMix;
		translateMix = data.translateMix;
		scaleMix = data.scaleMix;
		shearMix = data.shearMix;
		bones = new ExposedList<Bone>();
		foreach (BoneData bone in data.bones)
		{
			bones.Add(skeleton.FindBone(bone.name));
		}
		target = skeleton.FindBone(data.target.name);
	}

	public void Apply()
	{
		Update();
	}

	public void Update()
	{
		float num = rotateMix;
		float num2 = translateMix;
		float num3 = scaleMix;
		float num4 = shearMix;
		Bone bone = target;
		float a = bone.a;
		float b = bone.b;
		float c = bone.c;
		float d = bone.d;
		ExposedList<Bone> exposedList = bones;
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			Bone bone2 = exposedList.Items[i];
			if (num > 0f)
			{
				float a2 = bone2.a;
				float b2 = bone2.b;
				float c2 = bone2.c;
				float d2 = bone2.d;
				float num5 = (float)Math.Atan2(c, a) - (float)Math.Atan2(c2, a2) + data.offsetRotation * ((float)Math.PI / 180f);
				if (num5 > (float)Math.PI)
				{
					num5 -= (float)Math.PI * 2f;
				}
				else if (num5 < -(float)Math.PI)
				{
					num5 += (float)Math.PI * 2f;
				}
				num5 *= num;
				float num6 = MathUtils.Cos(num5);
				float num7 = MathUtils.Sin(num5);
				bone2.a = num6 * a2 - num7 * c2;
				bone2.b = num6 * b2 - num7 * d2;
				bone2.c = num7 * a2 + num6 * c2;
				bone2.d = num7 * b2 + num6 * d2;
			}
			if (num2 > 0f)
			{
				bone.LocalToWorld(data.offsetX, data.offsetY, out var worldX, out var worldY);
				bone2.worldX += (worldX - bone2.worldX) * num2;
				bone2.worldY += (worldY - bone2.worldY) * num2;
			}
			if (num3 > 0f)
			{
				float num8 = (float)Math.Sqrt(bone2.a * bone2.a + bone2.c * bone2.c);
				float num9 = (float)Math.Sqrt(a * a + c * c);
				float num10 = ((num8 <= 1E-05f) ? 0f : ((num8 + (num9 - num8 + data.offsetScaleX) * num3) / num8));
				bone2.a *= num10;
				bone2.c *= num10;
				num8 = (float)Math.Sqrt(bone2.b * bone2.b + bone2.d * bone2.d);
				num9 = (float)Math.Sqrt(b * b + d * d);
				num10 = ((num8 <= 1E-05f) ? 0f : ((num8 + (num9 - num8 + data.offsetScaleY) * num3) / num8));
				bone2.b *= num10;
				bone2.d *= num10;
			}
			if (num4 > 0f)
			{
				float b3 = bone2.b;
				float d3 = bone2.d;
				float num11 = MathUtils.Atan2(d3, b3);
				float num12 = MathUtils.Atan2(d, b) - MathUtils.Atan2(c, a) - (num11 - MathUtils.Atan2(bone2.c, bone2.a));
				if (num12 > (float)Math.PI)
				{
					num12 -= (float)Math.PI * 2f;
				}
				else if (num12 < -(float)Math.PI)
				{
					num12 += (float)Math.PI * 2f;
				}
				num12 = num11 + (num12 + data.offsetShearY * ((float)Math.PI / 180f)) * num4;
				float num13 = (float)Math.Sqrt(b3 * b3 + d3 * d3);
				bone2.b = MathUtils.Cos(num12) * num13;
				bone2.d = MathUtils.Sin(num12) * num13;
			}
		}
	}

	public override string ToString()
	{
		return data.name;
	}
}
