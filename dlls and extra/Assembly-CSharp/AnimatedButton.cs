using System;
using Spine;
using Spine.Unity;
using UnityEngine;

public class AnimatedButton : Widget
{
	public Action<Spine.Event> OnOpenAnimationEvent;

	[SerializeField]
	private SkeletonAnimation skeletonAnimation;

	[SerializeField]
	private string lockedIdle;

	[SerializeField]
	private string unlockedIdle;

	[SerializeField]
	private string touchLocked;

	[SerializeField]
	private string unlock;

	[SerializeField]
	private MethodCaller methodToCall;

	[SerializeField]
	private EventSender eventToSend;

	[SerializeField]
	private PlayerLevelRequirement requirements;

	private bool locked;

	private bool idleSet;

	public MethodCaller MethodToCall => methodToCall;

	public EventSender EventSender => eventToSend;

	private void Awake()
	{
		methodToCall.PrepareCall();
		eventToSend.PrepareSend();
		if (skeletonAnimation.state == null)
		{
			skeletonAnimation.Initialize(overwrite: true);
		}
		skeletonAnimation.state.Event += OnAnimationEvent;
		if (requirements != null)
		{
			PlayerLevelRequirement playerLevelRequirement = requirements;
			playerLevelRequirement.OnUnlock = (Action<bool>)Delegate.Combine(playerLevelRequirement.OnUnlock, new Action<bool>(OnUnlockActions));
		}
	}

	private void OnAnimationEvent(Spine.AnimationState state, int trackIndex, Spine.Event e)
	{
		if (OnOpenAnimationEvent != null)
		{
			OnOpenAnimationEvent(e);
		}
	}

	private void OnDestroy()
	{
		if (requirements != null)
		{
			PlayerLevelRequirement playerLevelRequirement = requirements;
			playerLevelRequirement.OnUnlock = (Action<bool>)Delegate.Remove(playerLevelRequirement.OnUnlock, new Action<bool>(OnUnlockActions));
		}
	}

	private void Start()
	{
		SetIdle();
	}

	private void OnUnlockActions(bool unlocked)
	{
		if (unlocked)
		{
			UnlockSequence();
		}
		else
		{
			SetIdle(force: true);
		}
	}

	private void SetIdle(bool force = false)
	{
		if (!idleSet || force)
		{
			locked = requirements != null && requirements.IsLocked;
			string text = ((!locked) ? unlockedIdle : lockedIdle);
			if (!string.IsNullOrEmpty(text))
			{
				skeletonAnimation.state.ClearTracks();
				skeletonAnimation.state.AddAnimation(0, text, loop: true, 0f);
				idleSet = true;
			}
		}
	}

	public void UnlockSequence(bool forcePlayUnlock = false)
	{
		string key = $"UnlockShown_{base.gameObject.name}";
		if (!string.IsNullOrEmpty(unlock) && (forcePlayUnlock || !GameProgress.GetBool(key)))
		{
			skeletonAnimation.state.ClearTracks();
			skeletonAnimation.state.AddAnimation(0, unlock, loop: false, 0f);
			GameProgress.SetBool(key, value: true);
			if (!string.IsNullOrEmpty(unlockedIdle))
			{
				skeletonAnimation.state.AddAnimation(0, unlockedIdle, loop: true, 0f);
			}
			idleSet = true;
			locked = false;
		}
	}

	protected override void OnActivate()
	{
		if (locked)
		{
			if (!string.IsNullOrEmpty(touchLocked) || !string.IsNullOrEmpty(lockedIdle))
			{
				skeletonAnimation.state.ClearTracks();
			}
			if (!string.IsNullOrEmpty(touchLocked))
			{
				skeletonAnimation.state.AddAnimation(0, touchLocked, loop: false, 0f);
			}
			if (!string.IsNullOrEmpty(lockedIdle))
			{
				skeletonAnimation.state.AddAnimation(0, lockedIdle, loop: true, 0f);
			}
		}
		if (eventToSend.HasEvent())
		{
			eventToSend.Send();
		}
		if (methodToCall.TargetComponent != null)
		{
			methodToCall.Call();
		}
	}

	protected override void OnInput(InputEvent input)
	{
		if (input.type == InputEvent.EventType.Press)
		{
			Activate();
		}
		else if (input.type == InputEvent.EventType.Release)
		{
			Release();
		}
	}
}
