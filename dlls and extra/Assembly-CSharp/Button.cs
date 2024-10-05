using System;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Button : Widget
{
	public delegate void InputDelegate(InputEvent input);

	[SerializeField]
	private MethodCaller methodToCall;

	[SerializeField]
	private GameObject messageTargetObject;

	[SerializeField]
	private string targetComponent;

	[SerializeField]
	private string methodToInvoke;

	[SerializeField]
	private string messageParameter;

	[SerializeField]
	private EventSender eventToSend = new EventSender();

	[SerializeField]
	private bool animate = true;

	[SerializeField]
	private float mouseOverScale = 1.2f;

	[SerializeField]
	private bool activateOnHold;

	[SerializeField]
	private bool activateOnPress;

	[SerializeField]
	protected AudioSource soundEffect;

	[SerializeField]
	private bool disableSound;

	private Component component;

	private MethodInfo methodInfo;

	private object[] parameterArray;

	private bool mouseOver;

	private bool down;

	private const float HoverSoundDelay = 0.15f;

	private Vector3 originalScale;

	private InputDelegate m_inputDelegate;

	private bool activateOnRelease = true;

	private bool m_locked;

	public EventSender EventToSend => eventToSend;

	public MethodCaller MethodToCall => methodToCall;

	public void Lock(bool lockState)
	{
		m_locked = lockState;
	}

	public void SetInputDelegate(InputDelegate handler)
	{
		m_inputDelegate = (InputDelegate)Delegate.Combine(m_inputDelegate, handler);
	}

	public void RemoveInputDelegate(InputDelegate handler)
	{
		m_inputDelegate = (InputDelegate)Delegate.Remove(m_inputDelegate, handler);
	}

	public void SetActivateOnRelease(bool activate)
	{
		activateOnRelease = activate;
	}

	private void Awake()
	{
		originalScale = base.transform.localScale;
		BindTarget();
		methodToCall.PrepareCall();
		eventToSend.PrepareSend();
		ButtonAwake();
		if (activateOnHold)
		{
			activateOnRelease = false;
		}
		if (activateOnPress)
		{
			activateOnRelease = false;
		}
		if (soundEffect == null && !disableSound)
		{
			soundEffect = Singleton<GuiManager>.Instance.DefaultButtonAudio;
		}
	}

	protected virtual void ButtonAwake()
	{
	}

	private void BindTarget()
	{
	}

	protected override void OnActivate()
	{
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
		AudioManager instance = Singleton<AudioManager>.Instance;
		if (input.type == InputEvent.EventType.Press)
		{
			mouseOver = true;
			down = true;
			if (activateOnPress)
			{
				if (animate && (bool)soundEffect)
				{
					instance.Play2dEffect(soundEffect);
				}
				if (!m_locked)
				{
					Activate();
				}
			}
		}
		else if (input.type == InputEvent.EventType.Release)
		{
			down = false;
			if (activateOnRelease)
			{
				if (animate && (bool)soundEffect)
				{
					instance.Play2dEffect(soundEffect);
				}
				if (!m_locked)
				{
					Activate();
				}
			}
			if (activateOnPress)
			{
				Release();
			}
		}
		else if (input.type == InputEvent.EventType.MouseEnter)
		{
			mouseOver = true;
		}
		else if (input.type == InputEvent.EventType.MouseReturn)
		{
			down = true;
			if (activateOnPress)
			{
				Activate();
			}
		}
		else if (input.type == InputEvent.EventType.MouseLeave)
		{
			if (activateOnPress && down)
			{
				Release();
			}
			mouseOver = false;
			down = false;
		}
		if (m_inputDelegate != null)
		{
			m_inputDelegate(input);
		}
	}

	private void Update()
	{
		if (activateOnHold && down && !m_locked)
		{
			Activate();
		}
		if (animate)
		{
			bool flag = !down && mouseOver;
			if (DeviceInfo.UsesTouchInput)
			{
				flag = down || mouseOver;
			}
			float num = base.transform.localScale.x / originalScale.x;
			if (flag && num < mouseOverScale)
			{
				num = Mathf.Min(num + GameTime.RealTimeDelta * 7f, mouseOverScale);
				base.transform.localScale = num * originalScale;
			}
			else if (!flag && num > 1f)
			{
				num = Mathf.Max(num - GameTime.RealTimeDelta * 7f, 1f);
				base.transform.localScale = num * originalScale;
			}
		}
		ButtonUpdate();
	}

	protected virtual void ButtonUpdate()
	{
	}
}
