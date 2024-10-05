using System;
using UnityEngine;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Animation))]
public class ButtonPulse : MonoBehaviour
{
	public string m_animationName = "ButtonPulse";

	public bool m_playAutomatically;

	private Button m_button;

	private UIEvent.Type m_buttonEvent;

	private Animation m_animation;

	private bool m_initialized;

	private void Awake()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (!m_initialized)
		{
			m_button = GetComponent<Button>();
			m_button.SetInputDelegate(OnButtonInput);
			m_animation = GetComponent<Animation>();
			if (m_button.EventToSend.EventName == "UIEvent")
			{
				m_buttonEvent = (UIEvent.Type)Enum.Parse(typeof(UIEvent.Type), m_button.EventToSend.GetParameters()[0].stringValue);
			}
			EventManager.Connect<PulseButtonEvent>(ReceivePulseButtonEvent);
			m_initialized = true;
		}
	}

	private void OnEnable()
	{
		if (m_playAutomatically)
		{
			ReceivePulseButtonEvent(new PulseButtonEvent(m_buttonEvent));
		}
	}

	private void OnDisable()
	{
		EventManager.Disconnect<PulseButtonEvent>(ReceivePulseButtonEvent);
	}

	private void OnButtonInput(InputEvent input)
	{
		m_animation.Stop();
	}

	public void Pulse()
	{
		Initialize();
		m_animation.Play(m_animationName);
	}

	public void StopPulse()
	{
		EventManager.Send(new PulseButtonEvent(m_buttonEvent, pulse: false));
	}

	private void ReceivePulseButtonEvent(PulseButtonEvent data)
	{
		if (data.buttonEvent == m_buttonEvent)
		{
			if (data.pulse)
			{
				m_animation.Play(m_animationName);
			}
			else
			{
				m_animation.Stop();
			}
		}
	}
}
