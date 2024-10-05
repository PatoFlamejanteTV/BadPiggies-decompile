using System;
using System.Collections;
using UnityEngine;

public static class EventManager
{
	public delegate void OnEvent<T>(T data) where T : Event;

	public interface Event
	{
	}

	private class EventTypeManager<T> where T : Event
	{
		public static OnEvent<T> handler;
	}

	public static void Send<T>(T data) where T : Event
	{
		EventTypeManager<T>.handler?.Invoke(data);
	}

	public static void Connect<T>(OnEvent<T> handler) where T : Event
	{
		EventTypeManager<T>.handler = (OnEvent<T>)Delegate.Combine(EventTypeManager<T>.handler, handler);
	}

	public static void Disconnect<T>(OnEvent<T> handler) where T : Event
	{
		EventTypeManager<T>.handler = (OnEvent<T>)Delegate.Remove(EventTypeManager<T>.handler, handler);
	}

	public static void SendOnNextUpdate<T>(MonoBehaviour sender, T data) where T : Event
	{
		sender.StartCoroutine(SendCoroutine(data));
	}

	public static IEnumerator SendCoroutine<T>(T data) where T : Event
	{
		yield return 0;
		Send(data);
	}
}
