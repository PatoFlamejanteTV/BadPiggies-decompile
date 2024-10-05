using System;
using System.Collections.Generic;

namespace Spine;

public class Skeleton
{
	internal SkeletonData data;

	internal ExposedList<Bone> bones;

	internal ExposedList<Slot> slots;

	internal ExposedList<Slot> drawOrder;

	internal ExposedList<IkConstraint> ikConstraints;

	internal ExposedList<IkConstraint> ikConstraintsSorted;

	internal ExposedList<TransformConstraint> transformConstraints;

	internal ExposedList<PathConstraint> pathConstraints;

	internal ExposedList<IUpdatable> updateCache = new ExposedList<IUpdatable>();

	internal Skin skin;

	internal float r = 1f;

	internal float g = 1f;

	internal float b = 1f;

	internal float a = 1f;

	internal float time;

	internal bool flipX;

	internal bool flipY;

	internal float x;

	internal float y;

	public SkeletonData Data => data;

	public ExposedList<Bone> Bones => bones;

	public ExposedList<IUpdatable> UpdateCacheList => updateCache;

	public ExposedList<Slot> Slots => slots;

	public ExposedList<Slot> DrawOrder => drawOrder;

	public ExposedList<IkConstraint> IkConstraints => ikConstraints;

	public ExposedList<PathConstraint> PathConstraints => pathConstraints;

	public ExposedList<TransformConstraint> TransformConstraints => transformConstraints;

	public Skin Skin
	{
		get
		{
			return skin;
		}
		set
		{
			skin = value;
		}
	}

	public float R
	{
		get
		{
			return r;
		}
		set
		{
			r = value;
		}
	}

	public float G
	{
		get
		{
			return g;
		}
		set
		{
			g = value;
		}
	}

	public float B
	{
		get
		{
			return b;
		}
		set
		{
			b = value;
		}
	}

	public float A
	{
		get
		{
			return a;
		}
		set
		{
			a = value;
		}
	}

	public float Time
	{
		get
		{
			return time;
		}
		set
		{
			time = value;
		}
	}

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

	public bool FlipX
	{
		get
		{
			return flipX;
		}
		set
		{
			flipX = value;
		}
	}

	public bool FlipY
	{
		get
		{
			return flipY;
		}
		set
		{
			flipY = value;
		}
	}

	public Bone RootBone
	{
		get
		{
			if (bones.Count == 0)
			{
				return null;
			}
			return bones.Items[0];
		}
	}

	public Skeleton(SkeletonData data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data", "data cannot be null.");
		}
		this.data = data;
		bones = new ExposedList<Bone>(data.bones.Count);
		foreach (BoneData bone3 in data.bones)
		{
			Bone item;
			if (bone3.parent == null)
			{
				item = new Bone(bone3, this, null);
			}
			else
			{
				Bone bone = bones.Items[bone3.parent.index];
				item = new Bone(bone3, this, bone);
				bone.children.Add(item);
			}
			bones.Add(item);
		}
		slots = new ExposedList<Slot>(data.slots.Count);
		drawOrder = new ExposedList<Slot>(data.slots.Count);
		foreach (SlotData slot in data.slots)
		{
			Bone bone2 = bones.Items[slot.boneData.index];
			Slot item2 = new Slot(slot, bone2);
			slots.Add(item2);
			drawOrder.Add(item2);
		}
		ikConstraints = new ExposedList<IkConstraint>(data.ikConstraints.Count);
		ikConstraintsSorted = new ExposedList<IkConstraint>(data.ikConstraints.Count);
		foreach (IkConstraintData ikConstraint in data.ikConstraints)
		{
			ikConstraints.Add(new IkConstraint(ikConstraint, this));
		}
		transformConstraints = new ExposedList<TransformConstraint>(data.transformConstraints.Count);
		foreach (TransformConstraintData transformConstraint in data.transformConstraints)
		{
			transformConstraints.Add(new TransformConstraint(transformConstraint, this));
		}
		pathConstraints = new ExposedList<PathConstraint>(data.pathConstraints.Count);
		foreach (PathConstraintData pathConstraint in data.pathConstraints)
		{
			pathConstraints.Add(new PathConstraint(pathConstraint, this));
		}
		UpdateCache();
		UpdateWorldTransform();
	}

	public void UpdateCache()
	{
		ExposedList<IUpdatable> exposedList = updateCache;
		exposedList.Clear();
		ExposedList<Bone> exposedList2 = bones;
		int i = 0;
		for (int count = exposedList2.Count; i < count; i++)
		{
			exposedList2.Items[i].sorted = false;
		}
		ExposedList<IkConstraint> exposedList3 = ikConstraintsSorted;
		exposedList3.Clear();
		exposedList3.AddRange(ikConstraints);
		int count2 = exposedList3.Count;
		int j = 0;
		for (int num = count2; j < num; j++)
		{
			IkConstraint ikConstraint = exposedList3.Items[j];
			Bone parent = ikConstraint.bones.Items[0].parent;
			int num2 = 0;
			while (parent != null)
			{
				parent = parent.parent;
				num2++;
			}
			ikConstraint.level = num2;
		}
		for (int k = 1; k < count2; k++)
		{
			IkConstraint ikConstraint2 = exposedList3.Items[k];
			int level = ikConstraint2.level;
			int num3;
			for (num3 = k - 1; num3 >= 0; num3--)
			{
				IkConstraint ikConstraint3 = exposedList3.Items[num3];
				if (ikConstraint3.level < level)
				{
					break;
				}
				exposedList3.Items[num3 + 1] = ikConstraint3;
			}
			exposedList3.Items[num3 + 1] = ikConstraint2;
		}
		int l = 0;
		for (int count3 = exposedList3.Count; l < count3; l++)
		{
			IkConstraint ikConstraint4 = exposedList3.Items[l];
			Bone target = ikConstraint4.target;
			SortBone(target);
			ExposedList<Bone> exposedList4 = ikConstraint4.bones;
			Bone bone = exposedList4.Items[0];
			SortBone(bone);
			exposedList.Add(ikConstraint4);
			SortReset(bone.children);
			exposedList4.Items[exposedList4.Count - 1].sorted = true;
		}
		ExposedList<PathConstraint> exposedList5 = pathConstraints;
		int m = 0;
		for (int count4 = exposedList5.Count; m < count4; m++)
		{
			PathConstraint pathConstraint = exposedList5.Items[m];
			Slot target2 = pathConstraint.target;
			int index = target2.data.index;
			Bone bone2 = target2.bone;
			if (skin != null)
			{
				SortPathConstraintAttachment(skin, index, bone2);
			}
			if (data.defaultSkin != null && data.defaultSkin != skin)
			{
				SortPathConstraintAttachment(data.defaultSkin, index, bone2);
			}
			int n = 0;
			for (int count5 = data.skins.Count; n < count5; n++)
			{
				SortPathConstraintAttachment(data.skins.Items[n], index, bone2);
			}
			if (target2.Attachment is PathAttachment attachment)
			{
				SortPathConstraintAttachment(attachment, bone2);
			}
			ExposedList<Bone> exposedList6 = pathConstraint.bones;
			int count6 = exposedList6.Count;
			for (int num4 = 0; num4 < count6; num4++)
			{
				SortBone(exposedList6.Items[num4]);
			}
			exposedList.Add(pathConstraint);
			for (int num5 = 0; num5 < count6; num5++)
			{
				SortReset(exposedList6.Items[num5].children);
			}
			for (int num6 = 0; num6 < count6; num6++)
			{
				exposedList6.Items[num6].sorted = true;
			}
		}
		ExposedList<TransformConstraint> exposedList7 = transformConstraints;
		int num7 = 0;
		for (int count7 = exposedList7.Count; num7 < count7; num7++)
		{
			TransformConstraint transformConstraint = exposedList7.Items[num7];
			SortBone(transformConstraint.target);
			ExposedList<Bone> exposedList8 = transformConstraint.bones;
			int count8 = exposedList8.Count;
			for (int num8 = 0; num8 < count8; num8++)
			{
				SortBone(exposedList8.Items[num8]);
			}
			exposedList.Add(transformConstraint);
			for (int num9 = 0; num9 < count8; num9++)
			{
				SortReset(exposedList8.Items[num9].children);
			}
			for (int num10 = 0; num10 < count8; num10++)
			{
				exposedList8.Items[num10].sorted = true;
			}
		}
		int num11 = 0;
		for (int count9 = exposedList2.Count; num11 < count9; num11++)
		{
			SortBone(exposedList2.Items[num11]);
		}
	}

	private void SortPathConstraintAttachment(Skin skin, int slotIndex, Bone slotBone)
	{
		foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> attachment in skin.Attachments)
		{
			if (attachment.Key.slotIndex == slotIndex)
			{
				SortPathConstraintAttachment(attachment.Value, slotBone);
			}
		}
	}

	private void SortPathConstraintAttachment(Attachment attachment, Bone slotBone)
	{
		if (!(attachment is PathAttachment { bones: var array }))
		{
			return;
		}
		if (array == null)
		{
			SortBone(slotBone);
			return;
		}
		ExposedList<Bone> exposedList = bones;
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			SortBone(exposedList.Items[array[i]]);
		}
	}

	private void SortBone(Bone bone)
	{
		if (!bone.sorted)
		{
			Bone parent = bone.parent;
			if (parent != null)
			{
				SortBone(parent);
			}
			bone.sorted = true;
			updateCache.Add(bone);
		}
	}

	private void SortReset(ExposedList<Bone> bones)
	{
		Bone[] items = bones.Items;
		int i = 0;
		for (int count = bones.Count; i < count; i++)
		{
			Bone bone = items[i];
			if (bone.sorted)
			{
				SortReset(bone.children);
			}
			bone.sorted = false;
		}
	}

	public void UpdateWorldTransform()
	{
		IUpdatable[] items = updateCache.Items;
		int i = 0;
		for (int count = updateCache.Count; i < count; i++)
		{
			items[i].Update();
		}
	}

	public void SetToSetupPose()
	{
		SetBonesToSetupPose();
		SetSlotsToSetupPose();
	}

	public void SetBonesToSetupPose()
	{
		Bone[] items = bones.Items;
		int i = 0;
		for (int count = bones.Count; i < count; i++)
		{
			items[i].SetToSetupPose();
		}
		IkConstraint[] items2 = ikConstraints.Items;
		int j = 0;
		for (int count2 = ikConstraints.Count; j < count2; j++)
		{
			IkConstraint obj = items2[j];
			obj.bendDirection = obj.data.bendDirection;
			obj.mix = obj.data.mix;
		}
		TransformConstraint[] items3 = transformConstraints.Items;
		int k = 0;
		for (int count3 = transformConstraints.Count; k < count3; k++)
		{
			TransformConstraint obj2 = items3[k];
			TransformConstraintData transformConstraintData = obj2.data;
			obj2.rotateMix = transformConstraintData.rotateMix;
			obj2.translateMix = transformConstraintData.translateMix;
			obj2.scaleMix = transformConstraintData.scaleMix;
			obj2.shearMix = transformConstraintData.shearMix;
		}
		PathConstraint[] items4 = pathConstraints.Items;
		int l = 0;
		for (int count4 = pathConstraints.Count; l < count4; l++)
		{
			PathConstraint obj3 = items4[l];
			PathConstraintData pathConstraintData = obj3.data;
			obj3.position = pathConstraintData.position;
			obj3.spacing = pathConstraintData.spacing;
			obj3.rotateMix = pathConstraintData.rotateMix;
			obj3.translateMix = pathConstraintData.translateMix;
		}
	}

	public void SetSlotsToSetupPose()
	{
		ExposedList<Slot> exposedList = slots;
		Slot[] items = exposedList.Items;
		drawOrder.Clear();
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			drawOrder.Add(items[i]);
		}
		int j = 0;
		for (int count2 = exposedList.Count; j < count2; j++)
		{
			items[j].SetToSetupPose();
		}
	}

	public Bone FindBone(string boneName)
	{
		if (boneName == null)
		{
			throw new ArgumentNullException("boneName", "boneName cannot be null.");
		}
		ExposedList<Bone> exposedList = bones;
		Bone[] items = exposedList.Items;
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			Bone bone = items[i];
			if (bone.data.name == boneName)
			{
				return bone;
			}
		}
		return null;
	}

	public int FindBoneIndex(string boneName)
	{
		if (boneName == null)
		{
			throw new ArgumentNullException("boneName", "boneName cannot be null.");
		}
		ExposedList<Bone> exposedList = bones;
		Bone[] items = exposedList.Items;
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			if (items[i].data.name == boneName)
			{
				return i;
			}
		}
		return -1;
	}

	public Slot FindSlot(string slotName)
	{
		if (slotName == null)
		{
			throw new ArgumentNullException("slotName", "slotName cannot be null.");
		}
		ExposedList<Slot> exposedList = slots;
		Slot[] items = exposedList.Items;
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			Slot slot = items[i];
			if (slot.data.name == slotName)
			{
				return slot;
			}
		}
		return null;
	}

	public int FindSlotIndex(string slotName)
	{
		if (slotName == null)
		{
			throw new ArgumentNullException("slotName", "slotName cannot be null.");
		}
		ExposedList<Slot> exposedList = slots;
		Slot[] items = exposedList.Items;
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			if (items[i].data.name.Equals(slotName))
			{
				return i;
			}
		}
		return -1;
	}

	public void SetSkin(string skinName)
	{
		Skin skin = data.FindSkin(skinName);
		if (skin == null)
		{
			throw new ArgumentException("Skin not found: " + skinName, "skinName");
		}
		SetSkin(skin);
	}

	public void SetSkin(Skin newSkin)
	{
		if (newSkin != null)
		{
			if (skin != null)
			{
				newSkin.AttachAll(this, skin);
			}
			else
			{
				ExposedList<Slot> exposedList = slots;
				int i = 0;
				for (int count = exposedList.Count; i < count; i++)
				{
					Slot slot = exposedList.Items[i];
					string attachmentName = slot.data.attachmentName;
					if (attachmentName != null)
					{
						Attachment attachment = newSkin.GetAttachment(i, attachmentName);
						if (attachment != null)
						{
							slot.Attachment = attachment;
						}
					}
				}
			}
		}
		skin = newSkin;
	}

	public Attachment GetAttachment(string slotName, string attachmentName)
	{
		return GetAttachment(data.FindSlotIndex(slotName), attachmentName);
	}

	public Attachment GetAttachment(int slotIndex, string attachmentName)
	{
		if (attachmentName == null)
		{
			throw new ArgumentNullException("attachmentName", "attachmentName cannot be null.");
		}
		if (skin != null)
		{
			Attachment attachment = skin.GetAttachment(slotIndex, attachmentName);
			if (attachment != null)
			{
				return attachment;
			}
		}
		if (data.defaultSkin != null)
		{
			return data.defaultSkin.GetAttachment(slotIndex, attachmentName);
		}
		return null;
	}

	public void SetAttachment(string slotName, string attachmentName)
	{
		if (slotName == null)
		{
			throw new ArgumentNullException("slotName", "slotName cannot be null.");
		}
		ExposedList<Slot> exposedList = slots;
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			Slot slot = exposedList.Items[i];
			if (!(slot.data.name == slotName))
			{
				continue;
			}
			Attachment attachment = null;
			if (attachmentName != null)
			{
				attachment = GetAttachment(i, attachmentName);
				if (attachment == null)
				{
					throw new Exception("Attachment not found: " + attachmentName + ", for slot: " + slotName);
				}
			}
			slot.Attachment = attachment;
			return;
		}
		throw new Exception("Slot not found: " + slotName);
	}

	public IkConstraint FindIkConstraint(string constraintName)
	{
		if (constraintName == null)
		{
			throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
		}
		ExposedList<IkConstraint> exposedList = ikConstraints;
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			IkConstraint ikConstraint = exposedList.Items[i];
			if (ikConstraint.data.name == constraintName)
			{
				return ikConstraint;
			}
		}
		return null;
	}

	public TransformConstraint FindTransformConstraint(string constraintName)
	{
		if (constraintName == null)
		{
			throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
		}
		ExposedList<TransformConstraint> exposedList = transformConstraints;
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			TransformConstraint transformConstraint = exposedList.Items[i];
			if (transformConstraint.data.name == constraintName)
			{
				return transformConstraint;
			}
		}
		return null;
	}

	public PathConstraint FindPathConstraint(string constraintName)
	{
		if (constraintName == null)
		{
			throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
		}
		ExposedList<PathConstraint> exposedList = pathConstraints;
		int i = 0;
		for (int count = exposedList.Count; i < count; i++)
		{
			PathConstraint pathConstraint = exposedList.Items[i];
			if (pathConstraint.data.name.Equals(constraintName))
			{
				return pathConstraint;
			}
		}
		return null;
	}

	public void Update(float delta)
	{
		time += delta;
	}
}
