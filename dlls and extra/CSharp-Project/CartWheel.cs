using System;
using UnityEngine;

public class CartWheel : BasePart
{
	private Vector3 m_lastPosition = Vector3.zero;

	private Vector3 m_lastContactDirection = Vector3.zero;

	private float m_radius;

	private float m_circumference;

	private Transform m_wheelPivot;

	private Transform m_fakeWheelPivot;

	private Transform m_axle;

	private float m_spinSpeed;

	private Vector3 m_lastForceDirection;

	private float m_angle;

	private AudioSource loopingWheelSound;

	private Collider m_supportCollider;

	private Rigidbody colliderRigidbody;

	private bool m_grounded;

	private void Start()
	{
		m_wheelPivot = base.transform.Find("WheelPivot");
		m_fakeWheelPivot = base.transform.Find("FakeWheelPivot");
		if ((bool)m_wheelPivot)
		{
			m_axle = m_wheelPivot;
			m_radius = m_wheelPivot.GetComponent<SphereCollider>().radius;
			m_circumference = (float)Math.PI * 2f * m_radius;
		}
		else
		{
			m_axle = m_fakeWheelPivot;
			m_radius = m_fakeWheelPivot.GetComponent<SphereCollider>().radius;
			m_circumference = (float)Math.PI * 2f * m_radius;
		}
		if ((bool)base.transform.Find("SupportCollider"))
		{
			m_supportCollider = base.transform.Find("SupportCollider").GetComponent<Collider>();
		}
	}

	private void SpawnSound()
	{
		switch (m_partType)
		{
		case PartType.ObsoleteWheel:
			loopingWheelSound = Singleton<AudioManager>.Instance.SpawnCombinedLoopingEffect(WPFMonoBehaviour.gameData.commonAudioCollection.woodenWheelLoop, base.transform).GetComponent<AudioSource>();
			break;
		case PartType.NormalWheel:
			loopingWheelSound = Singleton<AudioManager>.Instance.SpawnCombinedLoopingEffect(WPFMonoBehaviour.gameData.commonAudioCollection.normalWheelLoop, base.transform).GetComponent<AudioSource>();
			break;
		case PartType.SmallWheel:
			loopingWheelSound = Singleton<AudioManager>.Instance.SpawnCombinedLoopingEffect(WPFMonoBehaviour.gameData.commonAudioCollection.smallWheelLoop, base.transform).GetComponent<AudioSource>();
			break;
		case PartType.CartWheel:
			loopingWheelSound = Singleton<AudioManager>.Instance.SpawnCombinedLoopingEffect(WPFMonoBehaviour.gameData.commonAudioCollection.woodenWheelLoop, base.transform).GetComponent<AudioSource>();
			break;
		}
		loopingWheelSound.volume = 0f;
	}

	public override void OnCollisionStay(Collision collisionInfo)
	{
		base.OnCollisionStay(collisionInfo);
		for (int i = 0; i < collisionInfo.contacts.Length; i++)
		{
			if (collisionInfo.contacts[i].otherCollider != m_supportCollider)
			{
				m_lastContactDirection = (collisionInfo.contacts[i].point - m_lastPosition).normalized;
				break;
			}
		}
	}

	private void Update()
	{
		if ((bool)base.contraption && base.contraption.IsRunning)
		{
			if (m_spinSpeed == 0f)
			{
				m_spinSpeed = SpeedInDirection(m_lastForceDirection);
			}
			float z = base.transform.rotation.eulerAngles.z;
			m_angle += -360f * m_spinSpeed / m_circumference * Time.deltaTime;
			if ((bool)m_wheelPivot)
			{
				m_wheelPivot.localRotation = Quaternion.AngleAxis(0f - z + m_angle, Vector3.forward);
			}
			if ((bool)m_fakeWheelPivot)
			{
				float angle = 0f - z + Mathf.Sin(2f * m_angle * ((float)Math.PI / 180f)) * 8f;
				m_fakeWheelPivot.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
			UpdateSoundEffect();
		}
	}

	protected override void UpdateSoundEffect()
	{
		float num = 1f;
		if (Mathf.Abs(m_spinSpeed) > num && m_grounded)
		{
			if (!loopingWheelSound)
			{
				SpawnSound();
			}
			loopingWheelSound.volume = 0.2f * (Mathf.Abs(m_spinSpeed) / num - 1f);
		}
		else if ((bool)loopingWheelSound)
		{
			loopingWheelSound.volume = 0f;
		}
	}

	private void FixedUpdate()
	{
		if ((bool)base.contraption && base.contraption.IsRunning)
		{
			m_lastPosition = m_axle.position;
			if (Physics.Raycast(m_axle.position, m_lastContactDirection, out var hitInfo, m_radius + 0.1f) && hitInfo.collider != m_supportCollider)
			{
				colliderRigidbody = ((hitInfo.collider.gameObject.layer != BasePart.m_groundLayer) ? hitInfo.rigidbody : null);
				Vector3 lastForceDirection = Vector3.Cross(hitInfo.normal, Vector3.forward);
				m_lastForceDirection = lastForceDirection;
				m_spinSpeed = 0f;
				m_grounded = true;
			}
			else
			{
				m_spinSpeed *= 0.98f;
				colliderRigidbody = null;
				m_grounded = false;
			}
		}
	}

	private float SpeedInDirection(Vector3 direction)
	{
		Vector3 velocity = base.rigidbody.velocity;
		if ((bool)colliderRigidbody)
		{
			velocity -= colliderRigidbody.velocity;
		}
		return Vector3.Dot(velocity, direction);
	}
}
