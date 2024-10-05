using System;
using UnityEngine;

namespace Spine.Unity;

[ExecuteInEditMode]
[AddComponentMenu("Spine/SkeletonUtilityBone")]
public class SkeletonUtilityBone : MonoBehaviour
{
	public enum Mode
	{
		Follow,
		Override
	}

	[NonSerialized]
	public bool valid;

	[NonSerialized]
	public SkeletonUtility skeletonUtility;

	[NonSerialized]
	public Bone bone;

	public Mode mode;

	public bool zPosition = true;

	public bool position;

	public bool rotation;

	public bool scale;

	public bool flip;

	public bool flipX;

	[Range(0f, 1f)]
	public float overrideAlpha = 1f;

	public string boneName;

	public Transform parentReference;

	[NonSerialized]
	public bool transformLerpComplete;

	protected Transform cachedTransform;

	protected Transform skeletonTransform;

	private bool disableInheritScaleWarning;

	public bool DisableInheritScaleWarning => disableInheritScaleWarning;

	public void Reset()
	{
		bone = null;
		cachedTransform = base.transform;
		valid = skeletonUtility != null && skeletonUtility.skeletonRenderer != null && skeletonUtility.skeletonRenderer.valid;
		if (valid)
		{
			skeletonTransform = skeletonUtility.transform;
			skeletonUtility.OnReset -= HandleOnReset;
			skeletonUtility.OnReset += HandleOnReset;
			DoUpdate();
		}
	}

	private void OnEnable()
	{
		skeletonUtility = SkeletonUtility.GetInParent<SkeletonUtility>(base.transform);
		if (!(skeletonUtility == null))
		{
			skeletonUtility.RegisterBone(this);
			skeletonUtility.OnReset += HandleOnReset;
		}
	}

	private void HandleOnReset()
	{
		Reset();
	}

	private void OnDisable()
	{
		if (skeletonUtility != null)
		{
			skeletonUtility.OnReset -= HandleOnReset;
			skeletonUtility.UnregisterBone(this);
		}
	}

	public void DoUpdate()
	{
		if (!valid)
		{
			Reset();
			return;
		}
		Skeleton skeleton = skeletonUtility.skeletonRenderer.skeleton;
		if (bone == null)
		{
			if (boneName == null || boneName.Length == 0)
			{
				return;
			}
			bone = skeleton.FindBone(boneName);
			if (bone == null)
			{
				return;
			}
		}
		float num = ((!(skeleton.flipX ^ skeleton.flipY)) ? 1f : (-1f));
		if (mode == Mode.Follow)
		{
			if (position)
			{
				cachedTransform.localPosition = new Vector3(bone.x, bone.y, 0f);
			}
			if (rotation)
			{
				if (bone.Data.InheritRotation)
				{
					cachedTransform.localRotation = Quaternion.Euler(0f, 0f, bone.AppliedRotation);
				}
				else
				{
					Vector3 eulerAngles = skeletonTransform.rotation.eulerAngles;
					cachedTransform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z + bone.WorldRotationX * num);
				}
			}
			if (scale)
			{
				cachedTransform.localScale = new Vector3(bone.scaleX, bone.scaleY, bone.WorldSignX);
				disableInheritScaleWarning = !bone.data.inheritScale;
			}
		}
		else
		{
			if (mode != Mode.Override || transformLerpComplete)
			{
				return;
			}
			if (parentReference == null)
			{
				if (position)
				{
					bone.x = Mathf.Lerp(bone.x, cachedTransform.localPosition.x, overrideAlpha);
					bone.y = Mathf.Lerp(bone.y, cachedTransform.localPosition.y, overrideAlpha);
				}
				if (rotation)
				{
					float appliedRotation = Mathf.LerpAngle(bone.Rotation, cachedTransform.localRotation.eulerAngles.z, overrideAlpha);
					bone.Rotation = appliedRotation;
					bone.AppliedRotation = appliedRotation;
				}
				if (scale)
				{
					bone.scaleX = Mathf.Lerp(bone.scaleX, cachedTransform.localScale.x, overrideAlpha);
					bone.scaleY = Mathf.Lerp(bone.scaleY, cachedTransform.localScale.y, overrideAlpha);
				}
			}
			else
			{
				if (transformLerpComplete)
				{
					return;
				}
				if (position)
				{
					Vector3 vector = parentReference.InverseTransformPoint(cachedTransform.position);
					bone.x = Mathf.Lerp(bone.x, vector.x, overrideAlpha);
					bone.y = Mathf.Lerp(bone.y, vector.y, overrideAlpha);
				}
				if (rotation)
				{
					float appliedRotation2 = Mathf.LerpAngle(bone.Rotation, Quaternion.LookRotation((!flipX) ? Vector3.forward : (Vector3.forward * -1f), parentReference.InverseTransformDirection(cachedTransform.up)).eulerAngles.z, overrideAlpha);
					bone.Rotation = appliedRotation2;
					bone.AppliedRotation = appliedRotation2;
				}
				if (scale)
				{
					bone.scaleX = Mathf.Lerp(bone.scaleX, cachedTransform.localScale.x, overrideAlpha);
					bone.scaleY = Mathf.Lerp(bone.scaleY, cachedTransform.localScale.y, overrideAlpha);
				}
				disableInheritScaleWarning = !bone.data.inheritScale;
			}
			transformLerpComplete = true;
		}
	}

	public void AddBoundingBox(string skinName, string slotName, string attachmentName)
	{
		SkeletonUtility.AddBoundingBox(bone.skeleton, skinName, slotName, attachmentName, base.transform);
	}
}
