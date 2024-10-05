using System;
using System.Collections;
using UnityEngine;

public class CoroutineRunner : Singleton<CoroutineRunner>
{
	public new static CoroutineRunner Instance
	{
		get
		{
			if (Singleton<CoroutineRunner>.instance == null)
			{
				Singleton<CoroutineRunner>.instance = UnityEngine.Object.FindObjectOfType<CoroutineRunner>();
				if (Singleton<CoroutineRunner>.instance == null)
				{
					Singleton<CoroutineRunner>.instance = new GameObject("CoroutineRunner (Singleton)", typeof(CoroutineRunner)).GetComponent<CoroutineRunner>();
				}
			}
			return Singleton<CoroutineRunner>.instance;
		}
	}

	private void Awake()
	{
		SetAsPersistant();
	}

	public void DelayAction(Action action, float seconds, bool realTime = true)
	{
		StartCoroutine(DelayActionSequence(action, seconds, realTime));
	}

	public static IEnumerator DelayActionSequence(Action action, float seconds, bool realTime)
	{
		float secondsLeft = seconds;
		while (secondsLeft > 0f)
		{
			secondsLeft -= ((!realTime) ? GameTime.DeltaTime : GameTime.RealTimeDelta);
			yield return null;
		}
		action?.Invoke();
	}

	public static IEnumerator DelayFrames(Action action, int frames)
	{
		while (frames > 0)
		{
			frames--;
			yield return null;
		}
		action?.Invoke();
	}

	public static IEnumerator MoveObject(Transform tf, Vector3 position, float time, bool useLocalPosition = false)
	{
		float counter = 0f;
		float deltaTime = 1f / time;
		Vector3 originalPosition = ((!useLocalPosition) ? tf.position : tf.localPosition);
		while (counter < time)
		{
			if (useLocalPosition)
			{
				tf.localPosition = Vector3.Lerp(originalPosition, position, counter * deltaTime);
			}
			else
			{
				tf.position = Vector3.Lerp(originalPosition, position, counter * deltaTime);
			}
			counter += Time.deltaTime;
			yield return null;
		}
		if (useLocalPosition)
		{
			tf.localPosition = position;
		}
		else
		{
			tf.position = position;
		}
	}

	public static IEnumerator DeltaAction(float duration, bool realTime, Action<float> action)
	{
		float durationLeft = duration;
		do
		{
			yield return null;
			durationLeft -= ((!realTime) ? Time.deltaTime : Time.unscaledDeltaTime);
			if (action != null)
			{
				if (durationLeft <= 0f)
				{
					action(0f);
				}
				else
				{
					action(durationLeft / duration);
				}
				continue;
			}
			break;
		}
		while (!(durationLeft <= 0f));
	}
}
