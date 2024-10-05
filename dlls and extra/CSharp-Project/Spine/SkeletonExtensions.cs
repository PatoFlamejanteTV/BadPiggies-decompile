using System;

namespace Spine;

public static class SkeletonExtensions
{
	public static void PoseWithAnimation(this Skeleton skeleton, string animationName, float time, bool loop)
	{
		skeleton.data.FindAnimation(animationName)?.Apply(skeleton, 0f, time, loop, null);
	}

	public static void SetDrawOrderToSetupPose(this Skeleton skeleton)
	{
		Slot[] items = skeleton.slots.Items;
		int count = skeleton.slots.Count;
		ExposedList<Slot> drawOrder = skeleton.drawOrder;
		drawOrder.Clear(clearArray: false);
		drawOrder.GrowIfNeeded(count);
		Array.Copy(items, drawOrder.Items, count);
		drawOrder.Count = count;
	}

	public static void SetColorToSetupPose(this Slot slot)
	{
		slot.r = slot.data.r;
		slot.g = slot.data.g;
		slot.b = slot.data.b;
		slot.a = slot.data.a;
	}

	public static void SetAttachmentToSetupPose(this Slot slot)
	{
		SlotData data = slot.data;
		slot.Attachment = slot.bone.skeleton.GetAttachment(data.name, data.attachmentName);
	}

	public static void SetSlotAttachmentToSetupPose(this Skeleton skeleton, int slotIndex)
	{
		Slot slot = skeleton.slots.Items[slotIndex];
		if (slot.data.attachmentName == null)
		{
			slot.Attachment = null;
			return;
		}
		slot.attachment = null;
		slot.Attachment = skeleton.GetAttachment(slotIndex, slot.data.attachmentName);
	}

	public static void SetKeyedItemsToSetupPose(this Animation animation, Skeleton skeleton)
	{
		Timeline[] items = animation.timelines.Items;
		int i = 0;
		for (int num = items.Length; i < num; i++)
		{
			items[i].SetToSetupPose(skeleton);
		}
	}

	public static void SetToSetupPose(this Timeline timeline, Skeleton skeleton)
	{
		if (timeline != null)
		{
			if (timeline is RotateTimeline)
			{
				Bone obj = skeleton.bones.Items[((RotateTimeline)timeline).boneIndex];
				obj.rotation = obj.data.rotation;
			}
			else if (timeline is TranslateTimeline)
			{
				Bone obj2 = skeleton.bones.Items[((TranslateTimeline)timeline).boneIndex];
				obj2.x = obj2.data.x;
				obj2.y = obj2.data.y;
			}
			else if (timeline is ScaleTimeline)
			{
				Bone obj3 = skeleton.bones.Items[((ScaleTimeline)timeline).boneIndex];
				obj3.scaleX = obj3.data.scaleX;
				obj3.scaleY = obj3.data.scaleY;
			}
			else if (timeline is DeformTimeline)
			{
				skeleton.slots.Items[((DeformTimeline)timeline).slotIndex].attachmentVertices.Clear(clearArray: false);
			}
			else if (timeline is AttachmentTimeline)
			{
				skeleton.SetSlotAttachmentToSetupPose(((AttachmentTimeline)timeline).slotIndex);
			}
			else if (timeline is ColorTimeline)
			{
				skeleton.slots.Items[((ColorTimeline)timeline).slotIndex].SetColorToSetupPose();
			}
			else if (timeline is IkConstraintTimeline)
			{
				IkConstraintTimeline ikConstraintTimeline = (IkConstraintTimeline)timeline;
				IkConstraint obj4 = skeleton.ikConstraints.Items[ikConstraintTimeline.ikConstraintIndex];
				IkConstraintData data = obj4.data;
				obj4.bendDirection = data.bendDirection;
				obj4.mix = data.mix;
			}
			else if (timeline is DrawOrderTimeline)
			{
				skeleton.SetDrawOrderToSetupPose();
			}
		}
	}
}
