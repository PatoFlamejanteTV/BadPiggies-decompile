using System;
using UnityEngine;

[RequireComponent(typeof(Sprite))]
[RequireComponent(typeof(BoxCollider))]
public class UnavailablePartButton : Widget
{
	public Action OnPress;

	private Animation m_lockAnimation;

	private Animation m_starAnimation;

	private Animation m_starlimitAnimation;

	private void Start()
	{
		m_lockAnimation = base.transform.Find("Lock").GetComponent<Animation>();
		m_starAnimation = base.transform.Find("Star").GetComponent<Animation>();
		m_starlimitAnimation = base.transform.Find("Starlimit").GetComponent<Animation>();
	}

	protected override void OnInput(InputEvent input)
	{
		if (input.type == InputEvent.EventType.Press)
		{
			if (m_lockAnimation != null && !m_lockAnimation.isPlaying)
			{
				m_lockAnimation.Play();
			}
			if (m_starAnimation != null && !m_starAnimation.isPlaying)
			{
				m_starAnimation.Play();
			}
			if (m_starlimitAnimation != null && !m_starlimitAnimation.isPlaying)
			{
				m_starlimitAnimation.Play();
			}
		}
		if (input.type == InputEvent.EventType.Release && OnPress != null)
		{
			OnPress();
		}
	}
}
