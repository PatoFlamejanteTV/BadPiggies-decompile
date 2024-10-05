using System;

namespace Spine;

public class IkConstraint : IUpdatable
{
	internal IkConstraintData data;

	internal ExposedList<Bone> bones = new ExposedList<Bone>();

	internal Bone target;

	internal float mix;

	internal int bendDirection;

	internal int level;

	public IkConstraintData Data => data;

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

	public int BendDirection
	{
		get
		{
			return bendDirection;
		}
		set
		{
			bendDirection = value;
		}
	}

	public float Mix
	{
		get
		{
			return mix;
		}
		set
		{
			mix = value;
		}
	}

	public IkConstraint(IkConstraintData data, Skeleton skeleton)
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
		mix = data.mix;
		bendDirection = data.bendDirection;
		bones = new ExposedList<Bone>(data.bones.Count);
		foreach (BoneData bone in data.bones)
		{
			bones.Add(skeleton.FindBone(bone.name));
		}
		target = skeleton.FindBone(data.target.name);
	}

	public void Update()
	{
		Apply();
	}

	public void Apply()
	{
		Bone bone = target;
		ExposedList<Bone> exposedList = bones;
		switch (exposedList.Count)
		{
		case 2:
			Apply(exposedList.Items[0], exposedList.Items[1], bone.worldX, bone.worldY, bendDirection, mix);
			break;
		case 1:
			Apply(exposedList.Items[0], bone.worldX, bone.worldY, mix);
			break;
		}
	}

	public override string ToString()
	{
		return data.name;
	}

	public static void Apply(Bone bone, float targetX, float targetY, float alpha)
	{
		Bone parent = bone.parent;
		float num = 1f / (parent.a * parent.d - parent.b * parent.c);
		float num2 = targetX - parent.worldX;
		float num3 = targetY - parent.worldY;
		float x = (num2 * parent.d - num3 * parent.b) * num - bone.x;
		float num4 = MathUtils.Atan2((num3 * parent.a - num2 * parent.c) * num - bone.y, x) * (180f / (float)Math.PI) - bone.shearX - bone.rotation;
		if (bone.scaleX < 0f)
		{
			num4 += 180f;
		}
		if (num4 > 180f)
		{
			num4 -= 360f;
		}
		else if (num4 < -180f)
		{
			num4 += 360f;
		}
		bone.UpdateWorldTransform(bone.x, bone.y, bone.rotation + num4 * alpha, bone.scaleX, bone.scaleY, bone.shearX, bone.shearY);
	}

	public static void Apply(Bone parent, Bone child, float targetX, float targetY, int bendDir, float alpha)
	{
		if (alpha == 0f)
		{
			child.UpdateWorldTransform();
			return;
		}
		float x = parent.x;
		float y = parent.y;
		float num = parent.scaleX;
		float num2 = parent.scaleY;
		float num3 = child.scaleX;
		int num4;
		int num5;
		if (num < 0f)
		{
			num = 0f - num;
			num4 = 180;
			num5 = -1;
		}
		else
		{
			num4 = 0;
			num5 = 1;
		}
		if (num2 < 0f)
		{
			num2 = 0f - num2;
			num5 = -num5;
		}
		int num6;
		if (num3 < 0f)
		{
			num3 = 0f - num3;
			num6 = 180;
		}
		else
		{
			num6 = 0;
		}
		float x2 = child.x;
		float a = parent.a;
		float b = parent.b;
		float c = parent.c;
		float d = parent.d;
		bool num7 = Math.Abs(num - num2) <= 0.0001f;
		float num8;
		float num9;
		float num10;
		if (!num7)
		{
			num8 = 0f;
			num9 = a * x2 + parent.worldX;
			num10 = c * x2 + parent.worldY;
		}
		else
		{
			num8 = child.y;
			num9 = a * x2 + b * num8 + parent.worldX;
			num10 = c * x2 + d * num8 + parent.worldY;
		}
		Bone parent2 = parent.parent;
		a = parent2.a;
		b = parent2.b;
		c = parent2.c;
		d = parent2.d;
		float num11 = 1f / (a * d - b * c);
		float num12 = targetX - parent2.worldX;
		float num13 = targetY - parent2.worldY;
		float num14 = (num12 * d - num13 * b) * num11 - x;
		float num15 = (num13 * a - num12 * c) * num11 - y;
		num12 = num9 - parent2.worldX;
		num13 = num10 - parent2.worldY;
		float num16 = (num12 * d - num13 * b) * num11 - x;
		float num17 = (num13 * a - num12 * c) * num11 - y;
		float num18 = (float)Math.Sqrt(num16 * num16 + num17 * num17);
		float num19 = child.data.length * num3;
		float num22;
		float num21;
		if (num7)
		{
			num19 *= num;
			float num20 = (num14 * num14 + num15 * num15 - num18 * num18 - num19 * num19) / (2f * num18 * num19);
			if (num20 < -1f)
			{
				num20 = -1f;
			}
			else if (num20 > 1f)
			{
				num20 = 1f;
			}
			num21 = (float)Math.Acos(num20) * (float)bendDir;
			a = num18 + num19 * num20;
			b = num19 * MathUtils.Sin(num21);
			num22 = MathUtils.Atan2(num15 * a - num14 * b, num14 * a + num15 * b);
		}
		else
		{
			a = num * num19;
			b = num2 * num19;
			float num23 = a * a;
			float num24 = b * b;
			float num25 = num14 * num14 + num15 * num15;
			float num26 = MathUtils.Atan2(num15, num14);
			c = num24 * num18 * num18 + num23 * num25 - num23 * num24;
			float num27 = -2f * num24 * num18;
			float num28 = num24 - num23;
			d = num27 * num27 - 4f * num28 * c;
			if (d >= 0f)
			{
				float num29 = (float)Math.Sqrt(d);
				if (num27 < 0f)
				{
					num29 = 0f - num29;
				}
				num29 = (0f - (num27 + num29)) / 2f;
				float num30 = num29 / num28;
				float num31 = c / num29;
				float num32 = ((Math.Abs(num30) >= Math.Abs(num31)) ? num31 : num30);
				if (num32 * num32 <= num25)
				{
					num13 = (float)Math.Sqrt(num25 - num32 * num32) * (float)bendDir;
					num22 = num26 - MathUtils.Atan2(num13, num32);
					num21 = MathUtils.Atan2(num13 / num2, (num32 - num18) / num);
					goto IL_04b4;
				}
			}
			float num33 = 0f;
			float num34 = float.MaxValue;
			float x3 = 0f;
			float num35 = 0f;
			float num36 = 0f;
			float num37 = 0f;
			float x4 = 0f;
			float num38 = 0f;
			num12 = num18 + a;
			d = num12 * num12;
			if (d > num37)
			{
				num36 = 0f;
				num37 = d;
				x4 = num12;
			}
			num12 = num18 - a;
			d = num12 * num12;
			if (d < num34)
			{
				num33 = (float)Math.PI;
				num34 = d;
				x3 = num12;
			}
			float num39 = (float)Math.Acos((0.0 - (double)a) * (double)num18 / (double)(num23 - num24));
			num12 = a * MathUtils.Cos(num39) + num18;
			num13 = b * MathUtils.Sin(num39);
			d = num12 * num12 + num13 * num13;
			if (d < num34)
			{
				num33 = num39;
				num34 = d;
				x3 = num12;
				num35 = num13;
			}
			if (d > num37)
			{
				num36 = num39;
				num37 = d;
				x4 = num12;
				num38 = num13;
			}
			if (num25 <= (num34 + num37) / 2f)
			{
				num22 = num26 - MathUtils.Atan2(num35 * (float)bendDir, x3);
				num21 = num33 * (float)bendDir;
			}
			else
			{
				num22 = num26 - MathUtils.Atan2(num38 * (float)bendDir, x4);
				num21 = num36 * (float)bendDir;
			}
		}
		goto IL_04b4;
		IL_04b4:
		float num40 = MathUtils.Atan2(num8, x2) * (float)num5;
		float rotation = parent.rotation;
		num22 = (num22 - num40) * (180f / (float)Math.PI) + (float)num4 - rotation;
		if (num22 > 180f)
		{
			num22 -= 360f;
		}
		else if (num22 < -180f)
		{
			num22 += 360f;
		}
		parent.UpdateWorldTransform(x, y, rotation + num22 * alpha, parent.scaleX, parent.scaleY, 0f, 0f);
		rotation = child.rotation;
		num21 = ((num21 + num40) * (180f / (float)Math.PI) - child.shearX) * (float)num5 + (float)num6 - rotation;
		if (num21 > 180f)
		{
			num21 -= 360f;
		}
		else if (num21 < -180f)
		{
			num21 += 360f;
		}
		child.UpdateWorldTransform(x2, num8, rotation + num21 * alpha, child.scaleX, child.scaleY, child.shearX, child.shearY);
	}
}
