using System;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
	[Serializable]
	private class AnimationInfo
	{
		public string name = string.Empty;

		public Animation animation;

		public float crossFadeTime;
	}

	[Serializable]
	private class AnimationState
	{
		public string name = string.Empty;

		public ParticleSystem[] particleSystems;

		public AnimationInfo[] animations;
	}

	[Serializable]
	private class TestKey
	{
		public string animationName = string.Empty;

		public KeyCode key;
	}

	[SerializeField]
	private SpriteAnimation m_spriteAnimation;

	[SerializeField]
	private AnimationState[] m_states;

	[SerializeField]
	private TestKey[] m_testKeys;

	private AnimationState m_currentState;

	private void Update()
	{
	}

	private AnimationState FindAnimation(string name)
	{
		AnimationState[] states = m_states;
		foreach (AnimationState animationState in states)
		{
			if (animationState.name == name)
			{
				return animationState;
			}
		}
		return null;
	}

	public void Play(string name)
	{
		if ((bool)m_spriteAnimation)
		{
			m_spriteAnimation.onPlay = OnPlay;
			m_spriteAnimation.Play(name);
		}
		else
		{
			OnPlay(name);
		}
	}

	private void OnPlay(string name)
	{
		AnimationState animationState = FindAnimation(name);
		if ((bool)m_spriteAnimation)
		{
			SpriteAnimation spriteAnimation = m_spriteAnimation;
			spriteAnimation.onPlay = (SpriteAnimation.OnPlay)Delegate.Remove(spriteAnimation.onPlay, new SpriteAnimation.OnPlay(OnPlay));
		}
		ParticleSystem[] particleSystems;
		AnimationInfo[] animations;
		if (m_currentState != null)
		{
			particleSystems = m_currentState.particleSystems;
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].Stop();
			}
			animations = m_currentState.animations;
			foreach (AnimationInfo animationInfo in animations)
			{
				bool flag = true;
				AnimationInfo[] animations2 = animationState.animations;
				for (int j = 0; j < animations2.Length; j++)
				{
					if (animations2[j].animation.gameObject == animationInfo.animation.gameObject)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					animationInfo.animation.Stop();
				}
			}
		}
		particleSystems = animationState.particleSystems;
		for (int i = 0; i < particleSystems.Length; i++)
		{
			particleSystems[i].Play();
		}
		animations = animationState.animations;
		foreach (AnimationInfo animationInfo2 in animations)
		{
			animationInfo2.animation.CrossFade(animationInfo2.name, animationInfo2.crossFadeTime);
		}
		m_currentState = animationState;
	}
}
