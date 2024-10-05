using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity;

[RequireComponent(typeof(Animator))]
public class SkeletonAnimator : SkeletonRenderer, ISkeletonAnimation
{
	public enum MixMode
	{
		AlwaysMix,
		MixNext,
		SpineStyle
	}

	public MixMode[] layerMixModes = new MixMode[0];

	private readonly Dictionary<int, Animation> animationTable = new Dictionary<int, Animation>();

	private readonly Dictionary<AnimationClip, int> clipNameHashCodeTable = new Dictionary<AnimationClip, int>();

	private Animator animator;

	private float lastTime;

	public readonly ExposedList<Event> events;

	public event UpdateBonesDelegate UpdateLocal
	{
		add
		{
			_UpdateLocal += value;
		}
		remove
		{
			_UpdateLocal -= value;
		}
	}

	public event UpdateBonesDelegate UpdateWorld
	{
		add
		{
			_UpdateWorld += value;
		}
		remove
		{
			_UpdateWorld -= value;
		}
	}

	public event UpdateBonesDelegate UpdateComplete
	{
		add
		{
			_UpdateComplete += value;
		}
		remove
		{
			_UpdateComplete -= value;
		}
	}

	protected event UpdateBonesDelegate _UpdateLocal;

	protected event UpdateBonesDelegate _UpdateWorld;

	protected event UpdateBonesDelegate _UpdateComplete;

	public override void Initialize(bool overwrite)
	{
		if (valid && !overwrite)
		{
			return;
		}
		base.Initialize(overwrite);
		if (!valid)
		{
			return;
		}
		animationTable.Clear();
		clipNameHashCodeTable.Clear();
		foreach (Animation animation in skeletonDataAsset.GetSkeletonData(quiet: true).Animations)
		{
			animationTable.Add(animation.Name.GetHashCode(), animation);
		}
		animator = GetComponent<Animator>();
		lastTime = Time.time;
	}

	private void Update()
	{
		if (!valid)
		{
			return;
		}
		if (layerMixModes.Length != animator.layerCount)
		{
			Array.Resize(ref layerMixModes, animator.layerCount);
		}
		float num = Time.time - lastTime;
		skeleton.Update(Time.deltaTime);
		int layerCount = animator.layerCount;
		for (int i = 0; i < layerCount; i++)
		{
			float num2 = animator.GetLayerWeight(i);
			if (i == 0)
			{
				num2 = 1f;
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
			AnimatorStateInfo nextAnimatorStateInfo = animator.GetNextAnimatorStateInfo(i);
			AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(i);
			AnimatorClipInfo[] nextAnimatorClipInfo = animator.GetNextAnimatorClipInfo(i);
			MixMode mixMode = layerMixModes[i];
			if (mixMode == MixMode.AlwaysMix)
			{
				AnimatorClipInfo[] array = currentAnimatorClipInfo;
				for (int j = 0; j < array.Length; j++)
				{
					AnimatorClipInfo animatorClipInfo = array[j];
					float num3 = animatorClipInfo.weight * num2;
					if (num3 != 0f)
					{
						float num4 = currentAnimatorStateInfo.normalizedTime * animatorClipInfo.clip.length;
						animationTable[GetAnimationClipNameHashCode(animatorClipInfo.clip)].Mix(skeleton, Mathf.Max(0f, num4 - num), num4, currentAnimatorStateInfo.loop, events, num3);
					}
				}
				if (nextAnimatorStateInfo.nameHash == 0)
				{
					continue;
				}
				array = nextAnimatorClipInfo;
				for (int j = 0; j < array.Length; j++)
				{
					AnimatorClipInfo animatorClipInfo2 = array[j];
					float num5 = animatorClipInfo2.weight * num2;
					if (num5 != 0f)
					{
						float num6 = nextAnimatorStateInfo.normalizedTime * animatorClipInfo2.clip.length;
						animationTable[GetAnimationClipNameHashCode(animatorClipInfo2.clip)].Mix(skeleton, Mathf.Max(0f, num6 - num), num6, nextAnimatorStateInfo.loop, events, num5);
					}
				}
			}
			else
			{
				if (mixMode < MixMode.MixNext)
				{
					continue;
				}
				int k;
				for (k = 0; k < currentAnimatorClipInfo.Length; k++)
				{
					AnimatorClipInfo animatorClipInfo3 = currentAnimatorClipInfo[k];
					if (animatorClipInfo3.weight * num2 != 0f)
					{
						float num7 = currentAnimatorStateInfo.normalizedTime * animatorClipInfo3.clip.length;
						animationTable[GetAnimationClipNameHashCode(animatorClipInfo3.clip)].Apply(skeleton, Mathf.Max(0f, num7 - num), num7, currentAnimatorStateInfo.loop, events);
						break;
					}
				}
				for (; k < currentAnimatorClipInfo.Length; k++)
				{
					AnimatorClipInfo animatorClipInfo4 = currentAnimatorClipInfo[k];
					float num8 = animatorClipInfo4.weight * num2;
					if (num8 != 0f)
					{
						float num9 = currentAnimatorStateInfo.normalizedTime * animatorClipInfo4.clip.length;
						animationTable[GetAnimationClipNameHashCode(animatorClipInfo4.clip)].Mix(skeleton, Mathf.Max(0f, num9 - num), num9, currentAnimatorStateInfo.loop, events, num8);
					}
				}
				k = 0;
				if (nextAnimatorStateInfo.nameHash == 0)
				{
					continue;
				}
				if (mixMode == MixMode.SpineStyle)
				{
					for (; k < nextAnimatorClipInfo.Length; k++)
					{
						AnimatorClipInfo animatorClipInfo5 = nextAnimatorClipInfo[k];
						if (animatorClipInfo5.weight * num2 != 0f)
						{
							float num10 = nextAnimatorStateInfo.normalizedTime * animatorClipInfo5.clip.length;
							animationTable[GetAnimationClipNameHashCode(animatorClipInfo5.clip)].Apply(skeleton, Mathf.Max(0f, num10 - num), num10, nextAnimatorStateInfo.loop, events);
							break;
						}
					}
				}
				for (; k < nextAnimatorClipInfo.Length; k++)
				{
					AnimatorClipInfo animatorClipInfo6 = nextAnimatorClipInfo[k];
					float num11 = animatorClipInfo6.weight * num2;
					if (num11 != 0f)
					{
						float num12 = nextAnimatorStateInfo.normalizedTime * animatorClipInfo6.clip.length;
						animationTable[GetAnimationClipNameHashCode(animatorClipInfo6.clip)].Mix(skeleton, Mathf.Max(0f, num12 - num), num12, nextAnimatorStateInfo.loop, events, num11);
					}
				}
			}
		}
		if (this._UpdateLocal != null)
		{
			this._UpdateLocal(this);
		}
		skeleton.UpdateWorldTransform();
		if (this._UpdateWorld != null)
		{
			this._UpdateWorld(this);
			skeleton.UpdateWorldTransform();
		}
		if (this._UpdateComplete != null)
		{
			this._UpdateComplete(this);
		}
		lastTime = Time.time;
	}

	private int GetAnimationClipNameHashCode(AnimationClip clip)
	{
		if (!clipNameHashCodeTable.TryGetValue(clip, out var value))
		{
			value = clip.name.GetHashCode();
			clipNameHashCodeTable.Add(clip, value);
		}
		return value;
	}
}
