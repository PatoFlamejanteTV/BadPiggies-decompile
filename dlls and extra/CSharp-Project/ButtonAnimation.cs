using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class ButtonAnimation : MonoBehaviour
{
	[SerializeField]
	private bool playWholeAnimation = true;

	[SerializeField]
	private string activateAnimationName = string.Empty;

	private Button cachedButton;

	private Animation cachedAnimation;

	private bool mouseOver;

	private bool hasDelegateSet;

	public bool PlayWholeAnimation
	{
		get
		{
			return playWholeAnimation;
		}
		set
		{
			playWholeAnimation = value;
		}
	}

	public string ActivateAnimationName
	{
		get
		{
			return activateAnimationName;
		}
		set
		{
			activateAnimationName = value;
		}
	}

	private void OnEnable()
	{
		mouseOver = false;
		base.transform.localScale = Vector3.one;
		ResetAnimation();
	}

	private void Awake()
	{
		cachedButton = GetComponent<Button>();
		cachedAnimation = GetComponent<Animation>();
		if (cachedButton != null)
		{
			cachedButton.SetInputDelegate(InputHandler);
			hasDelegateSet = true;
		}
	}

	private void OnDestroy()
	{
		RemoveInputListener();
	}

	public void RemoveInputListener()
	{
		if (cachedButton != null && hasDelegateSet)
		{
			cachedButton.RemoveInputDelegate(InputHandler);
			hasDelegateSet = false;
		}
	}

	public void ResetAnimation()
	{
		if (!(cachedAnimation == null))
		{
			AnimationState animationState = cachedAnimation[activateAnimationName];
			if (animationState != null)
			{
				animationState.enabled = true;
				animationState.time = 0f;
				cachedAnimation.Sample();
				StopAnimation();
			}
		}
	}

	public void PlayAnimation(string animationName)
	{
		if (!(cachedAnimation == null))
		{
			cachedAnimation.Play(animationName);
		}
	}

	public void StopAnimation()
	{
		if (!(cachedAnimation == null))
		{
			cachedAnimation.Stop();
		}
	}

	private float GetAnimationClipLength(string clipName)
	{
		if (cachedAnimation == null)
		{
			return 0f;
		}
		AnimationClip clip = cachedAnimation.GetClip(clipName);
		if (clip == null)
		{
			return 0f;
		}
		return clip.length;
	}

	private void InputHandler(InputEvent input)
	{
		if (input.type == InputEvent.EventType.Press)
		{
			if (!string.IsNullOrEmpty(activateAnimationName))
			{
				mouseOver = true;
				PlayAnimation(activateAnimationName);
				float animationClipLength = GetAnimationClipLength(activateAnimationName);
				StartCoroutine(DelayScaling(animationClipLength));
			}
		}
		else if (input.type == InputEvent.EventType.Release || input.type == InputEvent.EventType.MouseLeave)
		{
			mouseOver = false;
		}
	}

	private IEnumerator DelayScaling(float delay)
	{
		if (playWholeAnimation)
		{
			yield return new WaitForSeconds(delay * 1.05f);
		}
		while (mouseOver)
		{
			yield return null;
		}
		if (!playWholeAnimation)
		{
			StopAnimation();
		}
		base.transform.localScale = Vector3.one;
	}
}
