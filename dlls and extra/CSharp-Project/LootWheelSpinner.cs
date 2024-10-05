using System;
using System.Collections;
using UnityEngine;

public class LootWheelSpinner
{
	private Rigidbody m_wheel;

	private Transform m_needle;

	private AnimationCurve m_needleMovement;

	private bool m_spinning;

	private float m_currentSpinVelocity;

	private LootWheel.WheelSlot[] m_slots;

	public bool IsSpinning => m_spinning;

	private float WheelRotation
	{
		get
		{
			return m_wheel.rotation.eulerAngles.z;
		}
		set
		{
			m_wheel.rotation = Quaternion.Euler(new Vector3(0f, 0f, value));
		}
	}

	public LootWheelSpinner(Rigidbody wheel, Transform needle, LootWheel.WheelSlot[] slots)
	{
		m_wheel = wheel;
		m_needle = needle;
		m_slots = slots;
		m_spinning = false;
		m_needleMovement = new AnimationCurve();
		m_needleMovement.AddKey(new Keyframe(0f, 0f));
		m_needleMovement.AddKey(new Keyframe(1f, 1f));
	}

	public void Spin(LootWheel.WheelSlot target, float initialSpin, float velocity, float deceleration, float tickingRate, Action OnSpinEnd)
	{
		float num = target.RotationBegin - target.RotationEnd;
		if (num < 0f)
		{
			num += 360f;
		}
		float num2 = UnityEngine.Random.Range(1.5f, num - 1.5f);
		CoroutineRunner.Instance.StartCoroutine(SpinRoutine(initialSpin, velocity, target.RotationBegin - num2, deceleration, tickingRate, OnSpinEnd));
	}

	private IEnumerator SpinRoutine(float initialSpinTime, float angularVelocity, float targetRotation, float deceleration, float tickingRate, Action OnSpinEnd)
	{
		m_wheel.interpolation = RigidbodyInterpolation.Interpolate;
		deceleration = Mathf.Max(deceleration, 0.05f);
		m_spinning = true;
		float num = angularVelocity / 2f * (angularVelocity / deceleration + 1f);
		float decelerationAngle;
		if (num > targetRotation)
		{
			decelerationAngle = (targetRotation + num) % 360f;
		}
		else
		{
			float num2 = Mathf.Ceil((targetRotation - num) / -360f);
			decelerationAngle = targetRotation - num + num2 * 360f;
		}
		CoroutineRunner.Instance.StartCoroutine(PlayTickSounds(tickingRate));
		CoroutineRunner.Instance.StartCoroutine(MoveNeedle());
		yield return CoroutineRunner.Instance.StartCoroutine(InitialSpin(initialSpinTime, angularVelocity));
		yield return CoroutineRunner.Instance.StartCoroutine(SpinTo(decelerationAngle, angularVelocity, deceleration));
		yield return CoroutineRunner.Instance.StartCoroutine(Decelerate(angularVelocity, deceleration));
		m_wheel.interpolation = RigidbodyInterpolation.None;
		m_spinning = false;
		OnSpinEnd?.Invoke();
	}

	private IEnumerator InitialSpin(float spinTime, float angularVelocity)
	{
		m_currentSpinVelocity = angularVelocity;
		while (spinTime > 0f)
		{
			yield return new WaitForFixedUpdate();
			WheelRotation -= angularVelocity;
			spinTime -= Time.deltaTime;
		}
	}

	private IEnumerator SpinTo(float targetRotation, float angularVelocity, float deceleration)
	{
		FindVelocity(targetRotation, WheelRotation, angularVelocity - deceleration, angularVelocity, out var targetVelocity, out var iterations);
		m_currentSpinVelocity = targetVelocity;
		for (int i = 0; i < iterations; i++)
		{
			yield return new WaitForFixedUpdate();
			WheelRotation -= targetVelocity;
		}
	}

	private void FindVelocity(float target, float current, float min, float max, out float velocity, out int iterations)
	{
		velocity = 0f;
		iterations = 0;
		float num = ((current <= target) ? (360f - (target - current)) : (current - target));
		while (velocity < min || velocity > max)
		{
			iterations = Mathf.CeilToInt(num / max);
			velocity = num / (float)iterations;
			num += 360f;
		}
	}

	private IEnumerator Decelerate(float currentVelocity, float rate)
	{
		while (currentVelocity > 0f)
		{
			yield return new WaitForFixedUpdate();
			WheelRotation -= currentVelocity;
			currentVelocity -= rate;
			m_currentSpinVelocity = currentVelocity;
		}
	}

	private IEnumerator PlayTickSounds(float rate)
	{
		float current = 0f;
		AudioSource[] spinClicks = WPFMonoBehaviour.gameData.commonAudioCollection.lootWheelTickSounds;
		while (m_spinning)
		{
			current += m_currentSpinVelocity * Time.deltaTime;
			if (current > rate)
			{
				Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(spinClicks[UnityEngine.Random.Range(0, spinClicks.Length)]);
				current = 0f;
			}
			yield return null;
		}
	}

	private IEnumerator MoveNeedle()
	{
		Vector3 needleRotation = m_needle.rotation.eulerAngles;
		Vector3 eulerAngles = m_wheel.rotation.eulerAngles;
		float nextTick = NextSlotBegin(eulerAngles.z);
		float initialRotation = needleRotation.z;
		float range = 80f;
		float t = 0f;
		float rate = 5f;
		bool rising = false;
		float previousRotation = eulerAngles.z;
		while (m_spinning)
		{
			eulerAngles = m_wheel.rotation.eulerAngles;
			float z = eulerAngles.z;
			if (InRange(previousRotation, z, nextTick))
			{
				nextTick = NextSlotBegin(eulerAngles.z);
				rising = t < 0.85f || rising;
			}
			float num = m_needleMovement.Evaluate(t);
			needleRotation.z = initialRotation + num * range;
			if (rising)
			{
				t += rate * Time.deltaTime;
				rising = t < 1f;
				t = Mathf.Clamp01(t);
			}
			else
			{
				t -= rate * Time.deltaTime;
				t = Mathf.Clamp01(t);
			}
			m_needle.rotation = Quaternion.Euler(needleRotation);
			previousRotation = eulerAngles.z;
			yield return null;
		}
		while (t > 0f)
		{
			t -= rate * Time.deltaTime;
			float num2 = m_needleMovement.Evaluate(t);
			needleRotation.z = initialRotation + num2 * range;
			m_needle.rotation = Quaternion.Euler(needleRotation);
			yield return null;
		}
	}

	private float NextSlotBegin(float rotation)
	{
		for (int i = 0; i < m_slots.Length; i++)
		{
			float rotationBegin = m_slots[i].RotationBegin;
			float rotationEnd = m_slots[i].RotationEnd;
			if (InRange(rotationBegin, rotationEnd, rotation))
			{
				return rotationEnd;
			}
		}
		return 0f;
	}

	private bool InRange(float begin, float end, float value)
	{
		if (begin < end)
		{
			if (!(value < begin) || !(value >= 0f))
			{
				if (value < 360f)
				{
					return value > end;
				}
				return false;
			}
			return true;
		}
		if (value < begin)
		{
			return value > end;
		}
		return false;
	}
}
