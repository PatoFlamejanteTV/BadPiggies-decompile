using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : Singleton<KeyListener>
{
	public List<KeyCode> m_hotkeys;

	private List<object> m_focusQueue = new List<object>();

	private static int focusCount;

	public static event Action<KeyCode> keyPressed;

	public static event Action<KeyCode> keyReleased;

	public static event Action<KeyCode> keyHold;

	public static event Action<float> mouseWheel;

	public void GrabFocus(object obj)
	{
		focusCount++;
		m_focusQueue.Add(obj);
	}

	public void ReleaseFocus(object obj)
	{
		focusCount--;
		m_focusQueue.Remove(obj);
	}

	private bool HasFocus(object obj)
	{
		return m_focusQueue[m_focusQueue.Count - 1] == obj;
	}

	private void InvokeDelegates<T>(Action<T> multicastDelegate, T arg)
	{
		Delegate[] invocationList = multicastDelegate.GetInvocationList();
		Delegate[] array;
		if (m_focusQueue.Count == 0)
		{
			array = invocationList;
			for (int i = 0; i < array.Length; i++)
			{
				((Action<T>)array[i])(arg);
			}
			return;
		}
		array = invocationList;
		foreach (Delegate @delegate in array)
		{
			if (HasFocus(@delegate.Target))
			{
				((Action<T>)@delegate)(arg);
				break;
			}
		}
	}

	private void Update()
	{
		if (!Singleton<GuiManager>.Instance.IsEnabled)
		{
			return;
		}
		if (KeyListener.keyPressed != null || KeyListener.keyReleased != null || KeyListener.keyHold != null)
		{
			for (int i = 0; i < m_hotkeys.Count; i++)
			{
				KeyCode keyCode = m_hotkeys[i];
				if (Input.GetKeyUp(keyCode) && KeyListener.keyReleased != null)
				{
					InvokeDelegates(KeyListener.keyReleased, keyCode);
				}
				if (Input.GetKeyDown(keyCode) && KeyListener.keyPressed != null)
				{
					InvokeDelegates(KeyListener.keyPressed, keyCode);
				}
				if (Input.GetKey(keyCode) && KeyListener.keyHold != null)
				{
					InvokeDelegates(KeyListener.keyHold, keyCode);
				}
			}
		}
		if (KeyListener.mouseWheel != null)
		{
			float axisRaw = Input.GetAxisRaw("Mouse ScrollWheel");
			if (axisRaw != 0f)
			{
				InvokeDelegates(KeyListener.mouseWheel, axisRaw);
			}
		}
		if (INSettings.GetBool(INFeature.InputSettings) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S))
		{
			AudioManager audioManager = Singleton<AudioManager>.Instance;
			if (audioManager != null)
			{
				audioManager.ToggleMute();
			}
		}
	}

	private void Awake()
	{
		SetAsPersistant();
	}
}
